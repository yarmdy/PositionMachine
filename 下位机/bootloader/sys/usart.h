#ifndef __USART_H
#define __USART_H
#include "stdio.h"	
#include "sys.h" 
#include "delay.h"


#define CH444G_IN0_H          GPIOG->BSRR = GPIO_Pin_7
#define CH444G_IN0_L          GPIOG->BRR  = GPIO_Pin_7
#define CH444G_IN1_H          GPIOG->BSRR = GPIO_Pin_6
#define CH444G_IN1_L          GPIOG->BRR  = GPIO_Pin_6


#define CH444G_Printer1  0x00
#define CH444G_Printer2  0x01
#define CH444G_Printer   0x02
#define USART_BUF_Total  100   //接收数组大小
#define USART_BUF_Total1  1024*2   //接收数组大小
#define USART_BUF_Total2  1024*4   //接收数组大小
typedef struct 
{
	 u16 Intput_INDEX;   //输入指针
   u16 Output_INDEX;	 //输出指针
   u16 Push_Length;    //本段数组长度	
   u16 Total_Length;   //总数组长度	
}USART;
extern USART USART1_REC;


extern u16 USART1_BUF_Length;     //串口1接收数组长度
extern u16 USART1_INDEX;
extern u16 USART1_INDEX2;
extern u16 USART1_R_INDEX;
extern u8 USART1_RX_BUF[USART_BUF_Total1];
extern u8 USART1_RX_BUF2[USART_BUF_Total2];
extern u8 USART1_TX_BUF[40];
extern u8 USART1_REC_TIMEOUT;
extern u8 USART1_REC_OK;

extern u16 USART2_BUF_Length;     //串口2接收数组长度
extern u16 USART2_INDEX;
extern u8 USART2_RX_BUF[USART_BUF_Total];
extern u8 USART2_TX_BUF[40];
extern u8 USART2_REC_TIMEOUT;
extern u8 USART2_REC_OK;

extern u16 USART3_BUF_Length;     //串口3接收数组长度
extern u16 USART3_INDEX;
extern u8 USART3_RX_BUF[USART_BUF_Total];
extern u8 USART3_TX_BUF[40];
extern u8 USART3_REC_TIMEOUT;
extern u8 USART3_REC_OK;

extern u16 USART4_BUF_Length;     //串口4接收数组长度
extern u16 USART4_INDEX;
extern u8 USART4_RX_BUF[USART_BUF_Total];
extern u8 USART4_TX_BUF[40];
extern u8 USART4_REC_TIMEOUT;
extern u8 USART4_REC_OK;

extern u16 USART5_BUF_Length;     //串口5接收数组长度
extern u16 USART5_INDEX;
extern u8 USART5_RX_BUF[USART_BUF_Total];
extern u8 USART5_TX_BUF[40];
extern u8 USART5_REC_TIMEOUT;
extern u8 USART5_REC_OK;

extern void uart_init (void);
extern void uart1_init(u32 bound);
extern void uart2_init(u32 bound);
extern void uart3_init(u32 bound);
extern void uart4_init(u32 bound);
extern void uart5_init(u32 bound);
extern void Uart1_PutChar(u8 ch);
extern void Uart2_PutChar(u8 ch);
extern void Uart3_PutChar(u8 ch);
extern void Uart4_PutChar(u8 ch);
extern void Uart5_PutChar(u8 adder,u8 ch);
extern void UART1_Send(u8 *Buffer, u32 Length);
extern void UART2_Send(u8 *Buffer, u32 Length);
extern void UART3_Send(u8 *Buffer, u32 Length);
extern void UART4_Send(u8 *Buffer, u32 Length);
extern void UART5_Send(u8 adder,u8 *Buffer, u32 Length);

extern void GetUSART1AllBuff(void);
#endif


