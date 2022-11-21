#include "delay.h"
#include "usart.h"

u16 USART5_BUF_Length;     //串口4接收数组长度
u16 USART5_INDEX;
u8 USART5_RX_BUF[USART_BUF_Total];
u8 USART5_TX_BUF[40];
u8 USART5_REC_TIMEOUT;
u8 USART5_REC_OK;

//***************************************************
//功能：串口5初始化         扩展
//入口：波特率
//出口：无
//***************************************************
void uart5_init(u32 bound)
{
    
				GPIO_InitTypeDef GPIO_InitStructure;
				USART_InitTypeDef USART_InitStructure;
				NVIC_InitTypeDef NVIC_InitStructure;

				RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC | RCC_APB2Periph_GPIOD, ENABLE);	
				RCC_APB1PeriphClockCmd(RCC_APB1Periph_UART5,ENABLE);
				USART_DeInit(UART5);  

				GPIO_InitStructure.GPIO_Pin = GPIO_Pin_12; 
				GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
				GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;	
				GPIO_Init(GPIOC, &GPIO_InitStructure); 

				GPIO_InitStructure.GPIO_Pin = GPIO_Pin_2;
				GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;	
				GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
				GPIO_Init(GPIOD, &GPIO_InitStructure);  


				NVIC_InitStructure.NVIC_IRQChannel = UART5_IRQn;
				NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0;
				NVIC_InitStructure.NVIC_IRQChannelSubPriority = 4;		
				NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;			
				NVIC_Init(&NVIC_InitStructure);	


				USART_InitStructure.USART_BaudRate = bound;
				USART_InitStructure.USART_WordLength = USART_WordLength_8b;
				USART_InitStructure.USART_StopBits = USART_StopBits_1;
				USART_InitStructure.USART_Parity = USART_Parity_No;
				USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
				USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;	

				USART_Init(UART5, &USART_InitStructure); 
				USART_ITConfig(UART5, USART_IT_RXNE, ENABLE);
				USART_Cmd(UART5, ENABLE);            
}

//***************************************************
//功能：串口5 单字节发送
//入口：发送的数据
//出口：无
//***************************************************
void Uart5_PutChar(u8 adder,u8 ch)
{
	switch (adder)
	{
		case 1:   CH444G_IN0_L; CH444G_IN1_L; break;  //通道1
		case 2:   CH444G_IN0_H; CH444G_IN1_L; break;  //通道2
		case 3:   CH444G_IN0_L; CH444G_IN1_H; break;  //通道3
		case 4:   CH444G_IN0_H; CH444G_IN1_H; break;  //通道4				
		default:               break;
	} 	
	delay_us(10);	
  USART_SendData(UART5, (u8)ch);
  while(USART_GetFlagStatus(UART5, USART_FLAG_TXE) == RESET);
		delay_ms(1);
}

//***************************************************
//功能：串口4 单字节发送
//入口：发送的数据 adder 0x00 打印机1
//                 adder 0x01 打印机2
//                 adder 0x02 打印机（贴标）
//出口：无
//***************************************************
void UART5_Send(u8 adder,u8 *Buffer, u32 Length)
{
		switch (adder)
	{
		case 1:   CH444G_IN0_L; CH444G_IN1_L; break;  //通道1
		case 2:   CH444G_IN0_H; CH444G_IN1_L; break;  //通道2
		case 3:   CH444G_IN0_L; CH444G_IN1_H; break;  //通道3
		case 4:   CH444G_IN0_H; CH444G_IN1_H; break;  //通道4				
		default:               break;
	} 
	
	delay_us(10);	
	while(Length != 0)
	{
		USART_SendData(UART5, *Buffer);
		while(USART_GetFlagStatus(UART5, USART_FLAG_TXE) == RESET);
		Buffer++;
		Length--;
	}
	delay_ms(1);
}
//***************************************************
//功能：串口5 中断服务函数    复核
//入口：无
//出口：无
//***************************************************
void UART5_IRQHandler(void)                	
{		
		if(USART_GetITStatus(UART5, USART_IT_RXNE) != RESET)  
		{
				USART_ClearITPendingBit(UART5,USART_IT_RXNE);			
				USART5_REC_TIMEOUT = 3;
				USART5_RX_BUF[USART5_INDEX] =USART_ReceiveData(UART5);
				USART5_INDEX++;				
		} 
}
