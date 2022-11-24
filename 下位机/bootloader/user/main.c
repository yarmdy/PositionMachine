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
u8 CommId=0;
void JTAG_Init(void);
void GetPortMessage(void);
void ProssPortMessage(u16);
void EnterSystemReq(void);
u16 CreateCommand(u8 comm,u8*data,u16 len,u8 commId,u8* res);
int main(void)
{
  /* Infinite loop */
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);	  //����NVIC�жϷ���1:2λ��ռ���ȼ���2λ��Ӧ���ȼ�
	JTAG_Init();
	delay_init();	    	       //��ʱ������ʼ��	
	TIM_Init();                //��ʼ����ʱ��
	uart_init ();              //���ڳ�ʼ��	  		
	GPIOX_Init();		  	 	    //��������˿ڳ�ʼ��	 
	Run_Led=~Run_Led;
	while (1){
		GetPortMessage();
		
		EnterSystemReq();
	}
}
u8 ReadData[1024];
u8 DisabledEnterSystemReq=0;
void EnterSystemReq(void){
	if(SendEnterSystemTimeSpan>0 && DisabledEnterSystemReq==0){
		return;
	}
	u8 commLine[14+1];
	u8 data[1]={1};
	u16 len = CreateCommand(1,data,1,CommId++,commLine);
//	u8 commLine[14+64];
//	u32 addr = FLASH_SAVE_ADDR;
//	STMFLASH_Read(addr,(u16*)ReadData,64/2);
//	u16 len = CreateCommand(1,ReadData,64,CommId++,commLine);
	
	UART1_Send(commLine,len);
	SendEnterSystemTimeSpan=200;
}
void JTAG_Init(void)
{
	
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_AFIO, ENABLE);
	GPIO_PinRemapConfig(GPIO_Remap_SWJ_Disable, ENABLE);
	GPIO_PinRemapConfig(GPIO_Remap_SWJ_JTAGDisable , ENABLE);
}
void GetPortMessage(void){
	GetUSART1AllBuff();
	if(USART1_INDEX2>0 && ReceiveTimeSpan<=0){
		USART1_INDEX2=0;
	}
	ReceiveTimeSpan=20;
	if(USART1_INDEX2<15){
		return;
	}
	
	u16 index;
	u16 length;
	u8 res=0;
	//BEEP_Enable=1;
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
	//DATAPACKAGE package = *(DATAPACKAGE*)(USART1_RX_BUF2+5);
	DATAPACKAGE package;
	package.Func = *(USART1_RX_BUF2+5);
	package.Comm = *(USART1_RX_BUF2+6);
	package.CommId = *(USART1_RX_BUF2+7);
	package.Length = *(u16*)(USART1_RX_BUF2+8);
	package.Data=(u8*)(USART1_RX_BUF2+10);
	return package;
}
typedef void (*Action)(void);
void ProssEnterSystem(DATAPACKAGE* package){
//	uint32_t i = 0;
//    __disable_irq();
//    SysTick->CTRL = 0;
//    SysTick->LOAD = 0;
//    SysTick->VAL = 0;
//    RCC_DeInit();
//    for (i = 0; i < 8; i++)
//    {
//        NVIC->ICER[i] = 0xFFFFFFFF;
//        NVIC->ICPR[i] = 0xFFFFFFFF;
//    } 

//    __enable_irq();
//    __set_MSP(*(uint32_t *)FLASH_SAVE_ADDR);
	
	__set_MSP(*(uint32_t *)FLASH_SAVE_ADDR);
//	__disable_irq();
//	__disable_fault_irq ();
	
	Action func=(Action)*(u32*)(FLASH_SAVE_ADDR+4);
	func();
//	u8 commLine[14+1];
//	u8 data[1]={1};
//	u16 len = CreateCommand(1,data,1,package->CommId,commLine);
//	UART1_Send(commLine,len);
}

void ProssWriteBin(DATAPACKAGE* package){
	u32 addr=*(u32*)(package->Data);
	addr+=FLASH_SAVE_ADDR;
	u16 size=*(u16*)(package->Data+4);
	
	STMFLASH_Write(addr,(u16*)(package->Data+6),size/2);
	
	STMFLASH_Read(addr,(u16*)ReadData,size/2);
	
	u8 succ=1;
	while(size--){
		if(((u8*)(package->Data+6))[size]==ReadData[size]){
			continue;
		}
		succ=0;
		break;
	}
	
	u8 commLine[14+1];
	u8 resData[1]={succ};
	u16 len = CreateCommand(0x02,resData,1,package->CommId,commLine);
	UART1_Send(commLine,len);
}
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

u16 CreateCommand(u8 comm,u8*data,u16 len,u8 commId,u8* res){
	u16 i=0;
	res[i++]='@';
	res[i++]='Z';
	res[i++]='H';
	res[i++]=0x01;
	res[i++]=0x50;
	res[i++]=0xFF;
	res[i++]=comm;
	res[i++]=commId;
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
