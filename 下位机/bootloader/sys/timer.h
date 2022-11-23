#ifndef __TIMER_H
#define __TIMER_H
#include "sys.h" 

//typedef struct
//{
//uint8_t rgb_red;
//uint8_t rgb_green;
//uint8_t rgb_blue;
//}RGB;
//#define TIMx TIM3
//#define TIMx_IRQn TIM3_IRQn
//#define TIMx_IRQHandler TIM3_IRQHandler
//#define TIM_PRESCALER ((36*10)-1) // 频率 200kHz
//#define TIM_PERIOD (2000-1) // 周期 2000/f_tim = 10ms
//#define RGB_RED (0) // 占空比
//#define RGB_GREEN (0) // 占空比
//#define RGB_BLUE (0) // 占空比
//#define TIM_RLED_PIN GPIO_PIN_0
//#define TIM_RLED_PORT GPIOB
//#define TIM_RLED_CHANNEL TIM_CHANNEL_3
//#define TIM_GLED_PIN GPIO_PIN_1
//#define TIM_GLED_PORT GPIOB
//#define TIM_GLED_CHANNEL TIM_CHANNEL_4
//#define TIM_BLED_PIN GPIO_PIN_5
//#define TIM_BLED_PORT GPIOB
//#define TIM_BLED_CHANNEL TIM_CHANNEL_2
//#define TIM_PWM_CLK_EN() __HAL_RCC_TIM3_CLK_ENABLE()
//#define TIM_PWM_GPIO_CLK_EN() __HAL_RCC_GPIOB_CLK_ENABLE()
extern u16 Run_Timer;
extern u16 SendEnterSystemTimeSpan;

void TIM2_Int_Init(u16 arr,u16 psc);
void TIM3_Int_Init(u16 arr,u16 psc); 
void TIM4_Int_Init(u16 arr,u16 psc);

void TIM1_PWM_Init(u16 arr,u16 psc);
void TIM8_PWM_Init(u16 arr,u16 psc);

//u8 Speed_Value_plus(u8 time);

void TIM_Init(void);
#endif
