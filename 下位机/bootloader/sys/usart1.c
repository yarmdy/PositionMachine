#include "usart.h"

USART USART1_REC;
u16 USART1_BUF_Length;     //����1�������鳤��
u16 USART1_INDEX;
u16 USART1_INDEX2;
u16 USART1_R_INDEX;
u8 USART1_RX_BUF[USART_BUF_Total1];
u8 USART1_RX_BUF2[USART_BUF_Total2];
u8 USART1_TX_BUF[40];
u8 USART1_REC_TIMEOUT;
u8 USART1_REC_OK;

//***************************************************
//���ܣ�����1��ʼ��
//��ڣ�������
//���ڣ���
//***************************************************
void uart1_init(u32 bound)
{
 
	GPIO_InitTypeDef GPIO_InitStructure;
	USART_InitTypeDef USART_InitStructure;
	NVIC_InitTypeDef NVIC_InitStructure;
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART1|RCC_APB2Periph_GPIOA,ENABLE);
	USART_DeInit(USART1);  //��λ����1

//USART1_TX(��������)  PA.9����
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9; 
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;	//�����������
	GPIO_Init(GPIOA, &GPIO_InitStructure);          //��ʼ��PA.9

//USART1_RX(��������)  PA.10����
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;//��������
	GPIO_Init(GPIOA, &GPIO_InitStructure);               //��ʼ��PA.10

//NVIC�ж���������
	NVIC_InitStructure.NVIC_IRQChannel = USART1_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority=0;//��ռ���ȼ� ��Ϊ
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;     //�����ȼ�   ��Ϊ		
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;			   //IRQͨ��ʹ��
	NVIC_Init(&NVIC_InitStructure);	//�����������õĲ�����ʼ��NVIC�Ĵ���

//USART��ʼ������
	USART_InitStructure.USART_BaudRate = bound;           //������Ϊ9600
	USART_InitStructure.USART_WordLength = USART_WordLength_8b;  //�ֳ�Ϊ8λ����
	USART_InitStructure.USART_StopBits = USART_StopBits_1;      //1��ֹͣλ
	USART_InitStructure.USART_Parity = USART_Parity_No;          //����żУ��λ
	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;//��Ӳ������������
	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;	//�շ�ģʽ

	USART_Init(USART1, &USART_InitStructure);       //���ڳ�ʼ��
	USART_ITConfig(USART1, USART_IT_RXNE, ENABLE); //�жϿ���
	USART_Cmd(USART1, ENABLE);                    //����ʹ��	

}

//***************************************************
//���ܣ�����1 ���ֽڷ���    �ÿ�
//��ڣ����͵�����
//���ڣ���
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
//���ܣ�����1 �жϷ�����
//��ڣ���
//���ڣ���
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
