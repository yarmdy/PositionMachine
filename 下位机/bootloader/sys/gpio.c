#include "gpio.h"
//#include "SysRun.h"

u16 Beep_Timer;        //蜂鸣器定时
u8 Beep_Sing;         //蜂鸣声响模式


void GPIO_Out_Init(u32 RCC_APB, GPIO_TypeDef*GPIO, u16 Pin)
{
	GPIO_InitTypeDef GPIO_InitStructure;		
 	RCC_APB2PeriphClockCmd(RCC_APB,ENABLE);//	
	
	GPIO_InitStructure.GPIO_Pin  = Pin;//
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP; // PL、CP时钟设置为推挽输出
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;		 //IO口速度为50MHz
 	GPIO_Init(GPIO, &GPIO_InitStructure);//                  
}

void GPIO_Int_Init(u32 RCC_APB, GPIO_TypeDef*GPIO, u16 Pin)
{
	GPIO_InitTypeDef GPIO_InitStructure;		
 	RCC_APB2PeriphClockCmd(RCC_APB,ENABLE);//	
	
	GPIO_InitStructure.GPIO_Pin  = Pin;//
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IPU; // PL、CP时钟设置为上拉输入
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;		 //IO口速度为50MHz
 	GPIO_Init(GPIO, &GPIO_InitStructure);// 
}

//输入IO初始化-
void Input_Init(void)
{
 	GPIO_Int_Init(RCC_APB2Periph_GPIOF,GPIOF,GPIO_Pin_15);  //初始化 横向电机定位光电1  /IO为上拉输入
 	GPIO_Int_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_0);   //初始化 横向电机定位光电2  /IO为上拉输入
 	GPIO_Int_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_1);   //初始化 纵向电机定位光电1  /IO为上拉输入	
 	GPIO_Int_Init(RCC_APB2Periph_GPIOE,GPIOE,GPIO_Pin_7);   //初始化 纵向电机定位光电2  /IO为上拉输入
 	GPIO_Int_Init(RCC_APB2Periph_GPIOE,GPIOE,GPIO_Pin_8);   //初始化 有料检测光电       /IO为上拉输入	
 	GPIO_Int_Init(RCC_APB2Periph_GPIOE,GPIOG,GPIO_Pin_9);   //初始化 压辊电机定位光电1  /IO为上拉输入	
 	GPIO_Int_Init(RCC_APB2Periph_GPIOE,GPIOE,GPIO_Pin_10);  //初始化 压辊电机定位光电2  /IO为上拉输入	
 	GPIO_Int_Init(RCC_APB2Periph_GPIOE,GPIOE,GPIO_Pin_11);  //初始化 巡边定位光电       /IO为上拉输入
}
//输出IO初始化-
void Output_Init(void)
{
  GPIO_Out_Init(RCC_APB2Periph_GPIOA,GPIOA,GPIO_Pin_15);  //初始化 运行LED/IO为上拉输出		
 	GPIO_Out_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_8);   //初始化 蜂鸣器/IO为上拉输出
  GPIO_Out_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_5);   //初始化 485使能 /IO为上拉输出
	
 	GPIO_Out_Init(RCC_APB2Periph_GPIOB,GPIOB,GPIO_Pin_1);  //初始化 横向电机 /IO为上拉输出
	GPIO_Out_Init(RCC_APB2Periph_GPIOF,GPIOF,GPIO_Pin_11); //初始化 纵向电机 /IO为上拉输出
	GPIO_Out_Init(RCC_APB2Periph_GPIOF,GPIOF,GPIO_Pin_13); //初始化 主辊电机 /IO为上拉输出

  GPIO_Out_Init(RCC_APB2Periph_GPIOB,GPIOB,GPIO_Pin_2);   //初始化 横向电机方向 /IO为上拉输出	
  GPIO_Out_Init(RCC_APB2Periph_GPIOF,GPIOF,GPIO_Pin_12);  //初始化 纵向电机方向 /IO为上拉输出	
  GPIO_Out_Init(RCC_APB2Periph_GPIOF,GPIOF,GPIO_Pin_14);  //初始化 主辊电机方向 /IO为上拉输出	

	
  GPIO_Out_Init(RCC_APB2Periph_GPIOC,GPIOC,GPIO_Pin_5); //初始化 压辊 /IO为上拉输出（继电器）	
  GPIO_Out_Init(RCC_APB2Periph_GPIOB,GPIOB,GPIO_Pin_0); //初始化 夹爪 /IO为上拉输出（继电器）	
	
	GPIO_ResetBits  (GPIOC,GPIO_Pin_5);  //压辊失电	
	GPIO_ResetBits  (GPIOB,GPIO_Pin_0);  //夹爪失电
	GPIO_ResetBits  (GPIOG,GPIO_Pin_8);  //蜂鸣器
	GPIO_ResetBits  (GPIOG,GPIO_Pin_5);  //485使能	
}

/*
   CH444G模拟开关接口初始化函数
*/
void CH444G_Init(void)
{ 	
  GPIO_Out_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_7); //初始化 CH444G_IN0/IO为上拉输出
  GPIO_Out_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_6); //初始化 CH444G_IN1/IO为上拉输出
} 

void GPIOX_Init(void)
{
  Input_Init();  //输入IO初始化
  Output_Init(); //输出IO初始化
  CH444G_Init(); //CH444G模拟开关接口初始化	
}


void Beep_Di_Di(void)
{
  if (Beep_Sing)
	{
	   if (Beep_Timer == 0)
		 {
				 switch (Beep_Sing)
				 {
					case 0x30: Beep_Timer = 20; BEEP_Enable = 1; Beep_Sing = 0x31; break;
					case 0x31: Beep_Timer = 20; BEEP_Enable = 0; Beep_Sing = 0x32; break;
					case 0x32: Beep_Timer = 20; BEEP_Enable = 1; Beep_Sing = 0x33; break;
					case 0x33: Beep_Timer = 20; BEEP_Enable = 0; Beep_Sing = 0x34; break;
					case 0x34: Beep_Timer = 20; BEEP_Enable = 1; Beep_Sing = 0x35; break;
					case 0x35:                  BEEP_Enable = 0; Beep_Sing = 0x00; break;	

					case 0x20: Beep_Timer = 20; BEEP_Enable = 1; Beep_Sing = 0x33; break;					 
					case 0x10: Beep_Timer = 20; BEEP_Enable = 1; Beep_Sing = 0x35; break;					 
					case 0x11: Beep_Timer = 100; BEEP_Enable = 1; Beep_Sing = 0x35; break;					 
					default:                                                        break;
				 }		 
		 }
	}
}

