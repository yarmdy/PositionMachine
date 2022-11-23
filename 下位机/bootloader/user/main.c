/**
  ******************************************************************************
  * @file    Project/STM32F10x_StdPeriph_Template/main.c 
  * @author  MCD Application Team
  * @version V3.5.0
  * @date    08-April-2011
  * @brief   Main program body
  ******************************************************************************
  * @attention
  *
  * THE PRESENT FIRMWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING CUSTOMERS
  * WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER FOR THEM TO SAVE
  * TIME. AS A RESULT, STMICROELECTRONICS SHALL NOT BE HELD LIABLE FOR ANY
  * DIRECT, INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY CLAIMS ARISING
  * FROM THE CONTENT OF SUCH FIRMWARE AND/OR THE USE MADE BY CUSTOMERS OF THE
  * CODING INFORMATION CONTAINED HEREIN IN CONNECTION WITH THEIR PRODUCTS.
  *
  * <h2><center>&copy; COPYRIGHT 2011 STMicroelectronics</center></h2>
  ******************************************************************************
  */  

/* Includes ------------------------------------------------------------------*/

#include "stdlib.h"
#include "global.h"


/**
  * @brief  Main program.
  * @param  None
  * @retval None
  */
void JTAG_Init(void);
void GetPortMessage(void);
void ProssPortMessage(u16);
void EnterSystemReq(void);
u16 CreateCommand(u8 comm,u8*data,u16 len,u8* res);
int main(void)
{
  /* Infinite loop */
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);	  //设置NVIC中断分组1:2位抢占优先级，2位响应优先级
	JTAG_Init();
	delay_init();	    	       //延时函数初始化	
	TIM_Init();                //初始化定时器
	uart_init ();              //串口初始化	  		
	GPIOX_Init();		  	 	    //输入输出端口初始化	 
	Run_Led=~Run_Led;
	while (1){
		GetPortMessage();
		
		EnterSystemReq();
	}
}


void EnterSystemReq(void){
	if(SendEnterSystemTimeSpan>0){
		return;
	}
	u8 commLine[14+1];
	u8 data[1]={1};
	u16 len = CreateCommand(1,data,1,commLine);
	UART1_Send(commLine,len);
	SendEnterSystemTimeSpan=100;
}
void JTAG_Init(void)
{
	
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_AFIO, ENABLE);
	GPIO_PinRemapConfig(GPIO_Remap_SWJ_Disable, ENABLE);
	GPIO_PinRemapConfig(GPIO_Remap_SWJ_JTAGDisable , ENABLE);
}
void GetPortMessage(void){
	GetUSART1AllBuff();
	if(USART1_INDEX2<15){
		return;
	}
	u16 index;
	u16 length;
	u8 res=0;
	while(1){
		res=AnalysisAZHFrame(USART1_RX_BUF2,USART1_INDEX2,&index,&length);
		
		if (res)
		{
			RemoveBytes(USART1_RX_BUF2,USART_BUF_Total2,0,index);
			USART1_INDEX2-=index;
		}
		
		if (res == 0 && index + length > 0)
		{
			RemoveBytes(USART1_RX_BUF2,USART_BUF_Total2,0,index + length);
			USART1_INDEX2-=index + length;
		}
		if (res || index + length <= 0 || USART1_INDEX2 <= 0)
		{
			break;
		}
	}
	if(res==0){
		return;
	}
	
	
	ProssPortMessage(length);
	
	RemoveBytes(USART1_RX_BUF2,USART_BUF_Total2,0,length);
	USART1_INDEX2-=length;
}
DATAPACKAGE GetDATAPACKAGE(void){
	DATAPACKAGE package = *(DATAPACKAGE*)(USART1_RX_BUF2+5);
	package.Data=USART1_RX_BUF2+sizeof(DATAPACKAGE)-sizeof(u8*);
	return package;
}
void ProssEnterSystem(DATAPACKAGE* package){}
void ProssWriteBin(DATAPACKAGE* package){}
//void ProssWriteFinish(DATAPACKAGE* package){}
void ProssPortMessage(u16 length){
	DATAPACKAGE package=GetDATAPACKAGE();
	if(package.Func!=0xff)return;
	switch(package.Comm){
		case 0x01://enter system
			ProssEnterSystem(&package);
			break;
		case 0x02://write bin
			ProssWriteBin(&package);
			break;
//		case 0x03://write finish
//			ProssWriteFinish(&package);
//			break;
	}
}

u16 CreateCommand(u8 comm,u8*data,u16 len,u8* res){
	u16 i=0;
	res[i++]='@';
	res[i++]='Z';
	res[i++]='H';
	res[i++]=0x01;
	res[i++]=0x50;
	res[i++]=0xFF;
	res[i++]=comm;
	res[i++]=0x01;
	*(u16*)&res[i]=len;
	i+=2;
	TakeBytes(&res[i],data,0,len);
	i+=len;
	u8 crc[2];
	GetCRC16(res,len+10,crc);
	res[i++]=crc[0];
	res[i++]=crc[1];
	res[i++]='\r';
	res[i++]='\n';
	return 14+len;
}
