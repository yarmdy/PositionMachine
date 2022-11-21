#ifndef __GPIO_H
#define __GPIO_H	 
#include "sys.h"


#define Run_Led       PAout(15)		        //运行灯
#define BEEP_Enable   PGout(8)		        //蜂鸣器
#define Uart4_485_RX  GPIOG->BRR  = GPIO_Pin_5		  //485使能接收
#define Uart4_485_TX  GPIOG->BSRR = GPIO_Pin_5		  //485使能发送


extern u16 Beep_Timer;        //蜂鸣器定时
extern u8 Beep_Sing;         //蜂鸣声响模式

void GPIO_Out_Init(u32 RCC_APB, GPIO_TypeDef*GPIO, u16 Pin);
void GPIO_Int_Init(u32 RCC_APB, GPIO_TypeDef*GPIO, u16 Pin);
void GPIOX_Init(void);//初始化
void Beep_Di_Di(void);

#endif
