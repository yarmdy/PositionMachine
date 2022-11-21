#include "delay.h"
#include "usart.h"

u16 USART3_BUF_Length;     //串口3接收数组长度
u16 USART3_INDEX;
u8 USART3_RX_BUF[USART_BUF_Total];
u8 USART3_TX_BUF[40];
u8 USART3_REC_TIMEOUT;
u8 USART3_REC_OK;

//***************************************************
//功能：串口3初始化         扩展
//入口：波特率
//出口：无
//***************************************************
void uart3_init(u32 bound)
{
    
				GPIO_InitTypeDef GPIO_InitStructure;
				USART_InitTypeDef USART_InitStructure;
				NVIC_InitTypeDef NVIC_InitStructure;

				RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB, ENABLE);	
				RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART3,ENABLE);
				USART_DeInit(USART3);  

				GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10; 
				GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
				GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;	
				GPIO_Init(GPIOB, &GPIO_InitStructure); 

				GPIO_InitStructure.GPIO_Pin = GPIO_Pin_11;
				GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
				GPIO_Init(GPIOB, &GPIO_InitStructure);  


				NVIC_InitStructure.NVIC_IRQChannel = USART3_IRQn;
				NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0;
				NVIC_InitStructure.NVIC_IRQChannelSubPriority = 2;		
				NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;			
				NVIC_Init(&NVIC_InitStructure);	


				USART_InitStructure.USART_BaudRate = bound;
				USART_InitStructure.USART_WordLength = USART_WordLength_8b;
				USART_InitStructure.USART_StopBits = USART_StopBits_1;
				USART_InitStructure.USART_Parity = USART_Parity_No;
				USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
				USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;	

				USART_Init(USART3, &USART_InitStructure); 
				USART_ITConfig(USART3, USART_IT_RXNE, ENABLE);
				USART_Cmd(USART3, ENABLE);            

				USART3_INDEX = 0;
}
 
//***************************************************
//功能：串口3 单字节发送
//入口：发送的数据
//出口：无
//***************************************************
void Uart3_PutChar(u8 ch)
{                                                                                                                             
  USART_SendData(USART3, (u8) ch);
  while(USART_GetFlagStatus(USART3, USART_FLAG_TXE) == RESET);
}
void UART3_Send(u8 *Buffer, u32 Length)
{
	while(Length != 0)
	{
		USART_SendData(USART3, *Buffer);
		while(USART_GetFlagStatus(USART3, USART_FLAG_TXE) == RESET);
		Buffer++;
		Length--;
	}
	delay_ms(1);
}

//***************************************************
//功能：串口3 中断服务函数    扩展
//入口：无
//出口：无
//***************************************************
void USART3_IRQHandler(void)                	
{		
		if(USART_GetITStatus(USART3, USART_IT_RXNE) != RESET)  
		{
				USART3_REC_TIMEOUT = 3;
				USART3_RX_BUF[USART3_INDEX] =USART_ReceiveData(USART3);
				USART3_INDEX++;
				USART_ClearITPendingBit(USART3,USART_IT_RXNE);	
		} 
}
