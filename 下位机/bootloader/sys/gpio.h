#ifndef __GPIO_H
#define __GPIO_H	 
#include "sys.h"


#define Run_Led       PAout(15)		        //���е�
#define BEEP_Enable   PGout(8)		        //������
#define Uart4_485_RX  GPIOG->BRR  = GPIO_Pin_5		  //485ʹ�ܽ���
#define Uart4_485_TX  GPIOG->BSRR = GPIO_Pin_5		  //485ʹ�ܷ���


extern u16 Beep_Timer;        //��������ʱ
extern u8 Beep_Sing;         //��������ģʽ

void GPIO_Out_Init(u32 RCC_APB, GPIO_TypeDef*GPIO, u16 Pin);
void GPIO_Int_Init(u32 RCC_APB, GPIO_TypeDef*GPIO, u16 Pin);
void GPIOX_Init(void);//��ʼ��
void Beep_Di_Di(void);

#endif
