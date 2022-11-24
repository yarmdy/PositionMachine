#include "usart.h"

USART USART1_REC;
u16 USART1_BUF_Length;     //串口1接收数组长度
u16 USART1_INDEX;
u16 USART1_INDEX2;
u16 USART1_R_INDEX;
u8 USART1_RX_BUF[USART_BUF_Total1];
u8 USART1_RX_BUF2[USART_BUF_Total2];
u8 USART1_TX_BUF[40];
u8 USART1_REC_TIMEOUT;
u8 USART1_REC_OK;

//***************************************************
//功能：串口1初始化
//入口：波特率
//出口：无
//***************************************************
void uart1_init(u32 bound)
{
 
	GPIO_InitTypeDef GPIO_InitStructure;
	USART_InitTypeDef USART_InitStructure;
	NVIC_InitTypeDef NVIC_InitStructure;
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART1|RCC_APB2Periph_GPIOA,ENABLE);
	USART_DeInit(USART1);  //复位串口1

//USART1_TX(发送数据)  PA.9引脚
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9; 
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;	//复用推挽输出
	GPIO_Init(GPIOA, &GPIO_InitStructure);          //初始化PA.9

//USART1_RX(接收数据)  PA.10引脚
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;//浮空输入
	GPIO_Init(GPIOA, &GPIO_InitStructure);               //初始化PA.10

//NVIC中断向量配置
	NVIC_InitStructure.NVIC_IRQChannel = USART1_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0;//抢占优先级 置为
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;     //子优先级   置为		
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;			   //IRQ通道使能
	NVIC_Init(&NVIC_InitStructure);	//根据上面设置的参数初始化NVIC寄存器

//USART初始化设置
	USART_InitStructure.USART_BaudRate = bound;           //波特率为9600
	USART_InitStructure.USART_WordLength = USART_WordLength_8b;  //字长为8位数据
	USART_InitStructure.USART_StopBits = USART_StopBits_1;      //1个停止位
	USART_InitStructure.USART_Parity = USART_Parity_No;          //无奇偶校验位
	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;//无硬件数据流控制
	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;	//收发模式

	USART_Init(USART1, &USART_InitStructure);       //串口初始化
	USART_ITConfig(USART1, USART_IT_RXNE, ENABLE); //中断开启
	USART_Cmd(USART1, ENABLE);                    //串口使能	

}

//***************************************************
//功能：串口1 单字节发送    访客
//入口：发送的数据
//出口：无
//***************************************************
void Uart1_PutChar(u8 ch)
{                                                                                                                             
	USART_SendData(USART1, (u8)ch);
	while(USART_GetFlagStatus(USART1, USART_FLAG_TXE) == RESET);
}
void UART1_Send(u8 *Buffer, u32 Length)
{
	while(Length != 0)
	{
		USART_SendData(USART1, *Buffer);
		while(USART_GetFlagStatus(USART1, USART_FLAG_TXE) == RESET);
		Buffer++;
		Length--;
	}
	//delay_ms(1);
}

//***************************************************
//功能：串口1 中断服务函数
//入口：无
//出口：无
//***************************************************
void USART1_IRQHandler(void)                	
{		
	if(USART_GetITStatus(USART1, USART_IT_RXNE) == RESET)  
	{
		return;
	} 
	USART_ClearITPendingBit(USART1,USART_IT_RXNE);			
	
	USART1_RX_BUF[USART1_INDEX] =USART_ReceiveData(USART1);
	USART1_INDEX++;
	USART1_BUF_Length++;
	if (USART1_INDEX > USART_BUF_Total1-1) {USART1_INDEX = 0;}
	if(USART1_BUF_Length>USART_BUF_Total1){USART1_BUF_Length=USART_BUF_Total1;}
}

u8 GetUSART1ABuffChar(u8* data){
	if(USART1_BUF_Length<=0){
		return 0;
	}
	*data = USART1_RX_BUF[USART1_R_INDEX++];
	if(USART1_R_INDEX>USART_BUF_Total1-1){USART1_R_INDEX=0;}
	USART_ITConfig(USART1, USART_IT_RXNE, DISABLE);
	USART1_BUF_Length--;
	USART_ITConfig(USART1, USART_IT_RXNE, ENABLE);
	return 1;
}
void GetUSART1AllBuff(void){
	u16 len=USART1_BUF_Length;
	while(len--){
		GetUSART1ABuffChar(&USART1_RX_BUF2[USART1_INDEX2++]);
		if(USART1_INDEX2>USART_BUF_Total2-1){
			USART1_INDEX2=0;
		}
	}
}
