#include "usart.h"


#if 1
#pragma import(__use_no_semihosting)             
//��׼����Ҫ��֧�ֺ���                 
struct __FILE 
{ 
	int handle; 

}; 

FILE __stdout;       
//����_sys_exit()�Ա���ʹ�ð�����ģʽ    
_sys_exit(int x) 
{ 
	x = x; 
} 
///******************************************************
// * ��������fputc
// * ����  ���ض���c�⺯��printf��USART1
// * ����  ����
// * ���  ����
// * ����  ����printf����
//***************************************************** */ 
int fputc(int ch, FILE *f)
{      
//	while((USART2->SR&0X40)==0) //ѭ������,ֱ���������   
//    USART2->DR = (u8) ch;      
//	return ch;
  USART_SendData(UART5, (u8) ch);
  while(USART_GetFlagStatus(UART5, USART_FLAG_TXE) == RESET);
	return (ch);	
}
/////�ض���c�⺯��scanf�����ڣ���д����ʹ��scanf��getchar�Ⱥ���
//int fgetc(FILE *f)
//{
//		/* �ȴ������������� */
//		while (USART_GetFlagStatus(USART3, USART_FLAG_RXNE) == RESET);

//		return (int)USART_ReceiveData(USART3);
//}
#endif 

///******************************************************
// * ��������fputc
// * ����  ���ض���c�⺯��printf��USART1
// * ����  ����
// * ���  ����
// * ����  ����printf����
//***************************************************** */
//int fputc(int ch, FILE *f)
//{
//	/* ��Printf���ݷ������� */
//	USART_SendData(USART1, (unsigned char) ch);
//	while (!(USART1->SR & USART_FLAG_TXE));
//	
//	return (ch);
//}


void uart_init (void)
{
		uart1_init(115200);          //��ʼ������1(PC1)
		uart2_init(115200);          //��ʼ������2(����)
		uart3_init(115200);          //��ʼ������3(PC2)
		uart4_init(115200);          //��ʼ������4(��ӡ��)
		uart5_init(115200);           //��ʼ������5	
}


 
