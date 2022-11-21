#include "usart.h"
#include "delay.h"

u16 USART2_BUF_Length;     //串口1接收数组长度
u16 USART2_INDEX;
u8 USART2_RX_BUF[USART_BUF_Total];
u8 USART2_TX_BUF[40];
u8 USART2_REC_TIMEOUT;
u8 USART2_REC_OK;

//***************************************************
//功能：串口2初始化
//入口：波特率
//出口：无
//***************************************************
void uart2_init(u32 bound)
{
    
				GPIO_InitTypeDef GPIO_InitStructure;
				USART_InitTypeDef USART_InitStructure;
				NVIC_InitTypeDef NVIC_InitStructure;

				RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOA, ENABLE);	
				RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART2,ENABLE);
				USART_DeInit(USART2);  

				GPIO_InitStructure.GPIO_Pin = GPIO_Pin_2; 
				GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
				GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;	
				GPIO_Init(GPIOA, &GPIO_InitStructure); 

				GPIO_InitStructure.GPIO_Pin = GPIO_Pin_3;
				GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
				GPIO_Init(GPIOA, &GPIO_InitStructure);  


				NVIC_InitStructure.NVIC_IRQChannel = USART2_IRQn;
				NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0;
				NVIC_InitStructure.NVIC_IRQChannelSubPriority = 1;		
				NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;			
				NVIC_Init(&NVIC_InitStructure);	


				USART_InitStructure.USART_BaudRate = bound;
				USART_InitStructure.USART_WordLength = USART_WordLength_8b;
				USART_InitStructure.USART_StopBits = USART_StopBits_1;
				USART_InitStructure.USART_Parity = USART_Parity_No;
				USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
				USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;	

				USART_Init(USART2, &USART_InitStructure); 
				USART_ITConfig(USART2, USART_IT_RXNE, ENABLE);
				USART_Cmd(USART2, ENABLE);            

				USART2_INDEX = 0;
}

//***************************************************
//功能：串口2 单字节发送 
//入口：发送的数据
//出口：无
//***************************************************
void Uart2_PutChar(u8 ch)
{                                                                                                                             
  USART_SendData(USART2, (u8) ch);
  while(USART_GetFlagStatus(USART2, USART_FLAG_TXE) == RESET);
}
void UART2_Send(u8 *Buffer, u32 Length)
{
	while(Length != 0)
	{
		USART_SendData(USART2, *Buffer);
		while(USART_GetFlagStatus(USART2, USART_FLAG_TXE) == RESET);
		Buffer++;
		Length--;
	}
	delay_ms(1);
}

//***************************************************
//功能：串口2 中断服务函数
//入口：无
//出口：无
//***************************************************
void USART2_IRQHandler(void)                	
{
		if(USART_GetITStatus(USART2, USART_IT_RXNE) != RESET)  
		{
			
				USART_ClearITPendingBit(USART2,USART_IT_RXNE);			
				USART2_REC_TIMEOUT = 3;
				USART2_RX_BUF[USART2_INDEX] =USART_ReceiveData(USART2);
				USART2_INDEX++;


//        if (USART2_REC_OK == 0)
//				{
//            USART2_RX_BUF[USART2_INDEX] =USART_ReceiveData(USART2);	
//						if (USART2_RX_BUF[USART2_INDEX] == 0x0D) 
//						{
//							 USART2_BUF_Length = USART2_INDEX;     //串口2接收数组长度
//							 USART2_INDEX      = 0;		
//							 USART2_REC_OK = 0x0D;					
//						}
//						else if (USART2_RX_BUF[USART2_INDEX] != 0x0A)
//						{
//								 USART2_INDEX++;
//								 if (USART2_INDEX > 39) {USART2_INDEX = 0;}					 
//						}						
//				}							
				USART_ClearITPendingBit(USART2,USART_IT_RXNE);			
		} 
} 
