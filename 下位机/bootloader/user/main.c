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
	}
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
void ProssPortMessage(u16 length){
	USART1_RX_BUF2[3]^=USART1_RX_BUF2[4];
	USART1_RX_BUF2[4]^=USART1_RX_BUF2[3];
	USART1_RX_BUF2[3]^=USART1_RX_BUF2[4];
	u8 crc[2];
	GetCRC16(USART1_RX_BUF2,length-4,crc);
	USART1_RX_BUF2[length-4]=crc[0];
	USART1_RX_BUF2[length-3]=crc[1];
	UART1_Send(USART1_RX_BUF2,length);
}
