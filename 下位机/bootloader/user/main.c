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
void ProssPortMessage(void);
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
		ProssPortMessage();
	}
}



void JTAG_Init(void)
{
	
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_AFIO, ENABLE);
	GPIO_PinRemapConfig(GPIO_Remap_SWJ_Disable, ENABLE);
	GPIO_PinRemapConfig(GPIO_Remap_SWJ_JTAGDisable , ENABLE);
}
void ProssPortMessage(void){
	if(USART1_INDEX<15){
		return;
	}
	u16 index;
	u16 length;
	u8 res=0;
	while(1){
		res=AnalysisAZHFrame(USART1_RX_BUF,USART1_INDEX,&index,&length);
		
		if (res)
		{
			TakeBytes(USART1_RX_BUF2,USART1_RX_BUF,index,length);
		}
		
		if (index + length > 0)
		{
			RemoveBytes(USART1_RX_BUF,USART_BUF_Total,0,index + length);
			USART1_INDEX-=index + length;
		}
		if (res || index + length <= 0 || USART1_INDEX <= 0)
		{
			break;
		}
	}
	if(res==0){
		return;
	}
	USART1_RX_BUF2[3]^=USART1_RX_BUF2[4];
	USART1_RX_BUF2[4]^=USART1_RX_BUF2[3];
	USART1_RX_BUF2[3]^=USART1_RX_BUF2[4];
	u8 crc[2];
	GetCRC16(USART1_RX_BUF2,length-4,crc);
	USART1_RX_BUF2[length-4]=crc[0];
	USART1_RX_BUF2[length-3]=crc[1];
	UART1_Send(USART1_RX_BUF2,length);
}
