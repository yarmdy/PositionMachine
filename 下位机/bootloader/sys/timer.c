#include "timer.h"
#include "usart.h"

u16 Run_Timer;
u16 SendEnterSystemTimeSpan=0;

void TIM_Init(void)
{		
		TIM2_Int_Init( 79,71);    //定时器2初始化 100us 
		TIM4_Int_Init( 4,719);    //定时器2初始化 50us 	
		TIM3_Int_Init(49,7199);   //定时器3初始化 5ms

}

void TIM2_Int_Init(u16 arr,u16 psc)
{
  TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure;
	NVIC_InitTypeDef NVIC_InitStructure;

	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM2, ENABLE); 

	TIM_TimeBaseStructure.TIM_Period = arr;              //重新加载值
	TIM_TimeBaseStructure.TIM_Prescaler =psc;            //指定预分频值
	TIM_TimeBaseStructure.TIM_ClockDivision = TIM_CKD_DIV1; 
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up; 
	TIM_TimeBaseStructure.TIM_RepetitionCounter = 0;
	TIM_TimeBaseInit(TIM2, &TIM_TimeBaseStructure); 
 
	TIM_ITConfig(TIM2, TIM_IT_Update ,ENABLE  );
	NVIC_InitStructure.NVIC_IRQChannel = TIM2_IRQn;  
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;   //抢断优先级为2 响应优先级为1
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 2;         
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE; 
	NVIC_Init(&NVIC_InitStructure);  

	TIM_Cmd(TIM2, ENABLE);  
}
void TIM3_Int_Init(u16 arr,u16 psc)
{
  TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure;
	NVIC_InitTypeDef NVIC_InitStructure;

	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM3, ENABLE); 

	TIM_TimeBaseStructure.TIM_Period = arr; 
	TIM_TimeBaseStructure.TIM_Prescaler =psc;  
	TIM_TimeBaseStructure.TIM_ClockDivision = TIM_CKD_DIV1; 
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;  
	TIM_TimeBaseInit(TIM3, &TIM_TimeBaseStructure); 
 
	TIM_ITConfig(TIM3, TIM_IT_Update ,ENABLE  );
	NVIC_InitStructure.NVIC_IRQChannel = TIM3_IRQn;  
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;   //抢断优先级为2 响应优先级为1
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 1;         
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE; 
	NVIC_Init(&NVIC_InitStructure);  

	TIM_Cmd(TIM3, ENABLE);  
}
void TIM4_Int_Init(u16 arr,u16 psc)
{
  TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure;
	NVIC_InitTypeDef NVIC_InitStructure;

	RCC_APB1PeriphClockCmd(RCC_APB1Periph_TIM4, ENABLE); 

	TIM_TimeBaseStructure.TIM_Period = arr;              //重新加载值
	TIM_TimeBaseStructure.TIM_Prescaler =psc;            //指定预分频值
	TIM_TimeBaseStructure.TIM_ClockDivision = TIM_CKD_DIV1; 
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up; 
	TIM_TimeBaseStructure.TIM_RepetitionCounter = 0;
	TIM_TimeBaseInit(TIM4, &TIM_TimeBaseStructure); 
 
	TIM_ITConfig(TIM4, TIM_IT_Update ,ENABLE  );
	NVIC_InitStructure.NVIC_IRQChannel = TIM4_IRQn;  
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;   //抢断优先级为1 响应优先级为2
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;         
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE; 
	NVIC_Init(&NVIC_InitStructure);  

	TIM_Cmd(TIM4, ENABLE);  
} 

void TIM2_IRQHandler(void)   //TIM2   100us中断一次，中断两次输出一个PWM波
{
	if (TIM_GetITStatus(TIM2, TIM_IT_Update) == RESET) 
	{		
		return;
	}
	TIM_ClearITPendingBit(TIM2, TIM_IT_Update); 	
}
void TIM3_IRQHandler(void)   //TIM3   5ms
{
	if (TIM_GetITStatus(TIM3, TIM_IT_Update) == RESET) 
	{	
		return;
	}
	TIM_ClearITPendingBit(TIM3, TIM_IT_Update); 
	if(Run_Timer) {Run_Timer --;}          //系统运行计时
	
	if(SendEnterSystemTimeSpan){
		SendEnterSystemTimeSpan--;
	}
	
}
void TIM4_IRQHandler(void)   //TIM4   40us
{	
	if (TIM_GetITStatus(TIM4, TIM_IT_Update) == RESET) 
	{
		return;
	}
	TIM_ClearITPendingBit(TIM4, TIM_IT_Update); 
		
//	//串口超时接收
//	if(USART1_REC_TIMEOUT)
//	{
//			USART1_REC_TIMEOUT --;
//			if(USART1_REC_TIMEOUT == 0)
//			{
//					USART1_BUF_Length = USART1_INDEX;       //串口1接收数组长度
//					//USART1_REC.Intput_INDEX = USART1_INDEX;
//					USART1_INDEX = 0;
//					USART1_REC_OK = 1;
//			}
//	}
//	if(USART2_REC_TIMEOUT)
//	{
//			USART2_REC_TIMEOUT --;
//			if(USART2_REC_TIMEOUT == 0)
//			{
//					USART2_BUF_Length = USART2_INDEX;     //串口2接收数组长度
//					USART2_INDEX = 0;
//					USART2_REC_OK = 1;
//			}
//	}

//	if(USART5_REC_TIMEOUT)
//	{
//			USART5_REC_TIMEOUT --;
//			if(USART5_REC_TIMEOUT == 0)
//			{
//					USART5_BUF_Length = USART5_INDEX;     //串口5接收数组长度
//					USART5_INDEX = 0;
//					USART5_REC_OK = 2;
//			}
//	}		
}


