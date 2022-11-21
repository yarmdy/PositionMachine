#include "usart.h"


#if 1
#pragma import(__use_no_semihosting)             
//标准库需要的支持函数                 
struct __FILE 
{ 
	int handle; 

}; 

FILE __stdout;       
//定义_sys_exit()以避免使用半主机模式    
_sys_exit(int x) 
{ 
	x = x; 
} 
///******************************************************
// * 函数名：fputc
// * 描述  ：重定向c库函数printf到USART1
// * 输入  ：无
// * 输出  ：无
// * 调用  ：由printf调用
//***************************************************** */ 
int fputc(int ch, FILE *f)
{      
//	while((USART2->SR&0X40)==0) //循环发送,直到发送完毕   
//    USART2->DR = (u8) ch;      
//	return ch;
  USART_SendData(UART5, (u8) ch);
  while(USART_GetFlagStatus(UART5, USART_FLAG_TXE) == RESET);
	return (ch);	
}
/////重定向c库函数scanf到串口，重写向后可使用scanf、getchar等函数
//int fgetc(FILE *f)
//{
//		/* 等待串口输入数据 */
//		while (USART_GetFlagStatus(USART3, USART_FLAG_RXNE) == RESET);

//		return (int)USART_ReceiveData(USART3);
//}
#endif 

///******************************************************
// * 函数名：fputc
// * 描述  ：重定向c库函数printf到USART1
// * 输入  ：无
// * 输出  ：无
// * 调用  ：由printf调用
//***************************************************** */
//int fputc(int ch, FILE *f)
//{
//	/* 将Printf内容发往串口 */
//	USART_SendData(USART1, (unsigned char) ch);
//	while (!(USART1->SR & USART_FLAG_TXE));
//	
//	return (ch);
//}


void uart_init (void)
{
		uart1_init(115200);          //初始化串口1(PC1)
		uart2_init(115200);          //初始化串口2(测试)
		uart3_init(115200);          //初始化串口3(PC2)
		uart4_init(115200);          //初始化串口4(打印机)
		uart5_init(115200);           //初始化串口5	
}


 
