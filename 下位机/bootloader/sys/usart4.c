
#include "usart.h"
#include "delay.h"
#include "gpio.h"

u16 USART4_BUF_Length;     //串口3接收数组长度
u16 USART4_INDEX;
u8 USART4_RX_BUF[USART_BUF_Total];
u8 USART4_TX_BUF[40];
u8 USART4_REC_TIMEOUT;
u8 USART4_REC_OK;
//***************************************************
//功能：串口4初始化         扩展
//入口：波特率
//出口：无
//***************************************************
void uart4_init(u32 bound)
{
    
				GPIO_InitTypeDef GPIO_InitStructure;
				USART_InitTypeDef USART_InitStructure;
				NVIC_InitTypeDef NVIC_InitStructure;

				RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC, ENABLE);	
				RCC_APB1PeriphClockCmd(RCC_APB1Periph_UART4,ENABLE);
				USART_DeInit(UART4);  

				GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10; 
				GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
				GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;	
				GPIO_Init(GPIOC, &GPIO_InitStructure); 

				GPIO_InitStructure.GPIO_Pin = GPIO_Pin_11;
				GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
				GPIO_Init(GPIOC, &GPIO_InitStructure);  


				NVIC_InitStructure.NVIC_IRQChannel = UART4_IRQn;
				NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0;
				NVIC_InitStructure.NVIC_IRQChannelSubPriority = 3;		
				NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;			
				NVIC_Init(&NVIC_InitStructure);	


				USART_InitStructure.USART_BaudRate = bound;
				USART_InitStructure.USART_WordLength = USART_WordLength_8b;
				USART_InitStructure.USART_StopBits = USART_StopBits_1;
				USART_InitStructure.USART_Parity = USART_Parity_No;
				USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
				USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;	

				USART_Init(UART4, &USART_InitStructure); 
				USART_ITConfig(UART4, USART_IT_RXNE, ENABLE);
				USART_Cmd(UART4, ENABLE);            

}

//***************************************************
//功能：串口4 单字节发送
//入口：发送的数据
//出口：无
//***************************************************
void Uart4_PutChar(u8 ch)
{
  Uart4_485_TX;		  //485使能发送	
  USART_SendData(UART4, (u8)ch);
  while(USART_GetFlagStatus(UART4, USART_FLAG_TXE) == RESET);
	Uart4_485_RX;		  //485使能接收
}
void UART4_Send(u8 *Buffer, u32 Length)
{
	Uart4_485_TX;		  //485使能发送
	while(Length != 0)
	{
		USART_SendData(UART4, *Buffer);
		while(USART_GetFlagStatus(UART4, USART_FLAG_TXE) == RESET);
		Buffer++;
		Length--;
	}		
	delay_ms(1);
	Uart4_485_RX;		  //485使能接收
}

//***************************************************
//功能：串口4 中断服务函数    扩展
//入口：无
//出口：无
//***************************************************
void UART4_IRQHandler(void)                	
{		
		if(USART_GetITStatus(UART4, USART_IT_RXNE) != RESET)  
		{
				USART4_REC_TIMEOUT = 3;
				USART4_RX_BUF[USART4_INDEX] =USART_ReceiveData(UART4);
				USART4_INDEX++;
			
				USART_ClearITPendingBit(UART4,USART_IT_RXNE);	
		} 
}
