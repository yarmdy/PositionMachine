#include "gpio.h"
//#include "SysRun.h"

u16 Beep_Timer;        //��������ʱ
u8 Beep_Sing;         //��������ģʽ


void GPIO_Out_Init(u32 RCC_APB, GPIO_TypeDef*GPIO, u16 Pin)
{
	GPIO_InitTypeDef GPIO_InitStructure;		
 	RCC_APB2PeriphClockCmd(RCC_APB,ENABLE);//	
	
	GPIO_InitStructure.GPIO_Pin  = Pin;//
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP; // PL��CPʱ������Ϊ�������
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;		 //IO���ٶ�Ϊ50MHz
 	GPIO_Init(GPIO, &GPIO_InitStructure);//                  
}

void GPIO_Int_Init(u32 RCC_APB, GPIO_TypeDef*GPIO, u16 Pin)
{
	GPIO_InitTypeDef GPIO_InitStructure;		
 	RCC_APB2PeriphClockCmd(RCC_APB,ENABLE);//	
	
	GPIO_InitStructure.GPIO_Pin  = Pin;//
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IPU; // PL��CPʱ������Ϊ��������
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;		 //IO���ٶ�Ϊ50MHz
 	GPIO_Init(GPIO, &GPIO_InitStructure);// 
}

//����IO��ʼ��-
void Input_Init(void)
{
 	GPIO_Int_Init(RCC_APB2Periph_GPIOF,GPIOF,GPIO_Pin_15);  //��ʼ�� ��������λ���1  /IOΪ��������
 	GPIO_Int_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_0);   //��ʼ�� ��������λ���2  /IOΪ��������
 	GPIO_Int_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_1);   //��ʼ�� ��������λ���1  /IOΪ��������	
 	GPIO_Int_Init(RCC_APB2Periph_GPIOE,GPIOE,GPIO_Pin_7);   //��ʼ�� ��������λ���2  /IOΪ��������
 	GPIO_Int_Init(RCC_APB2Periph_GPIOE,GPIOE,GPIO_Pin_8);   //��ʼ�� ���ϼ����       /IOΪ��������	
 	GPIO_Int_Init(RCC_APB2Periph_GPIOE,GPIOG,GPIO_Pin_9);   //��ʼ�� ѹ�������λ���1  /IOΪ��������	
 	GPIO_Int_Init(RCC_APB2Periph_GPIOE,GPIOE,GPIO_Pin_10);  //��ʼ�� ѹ�������λ���2  /IOΪ��������	
 	GPIO_Int_Init(RCC_APB2Periph_GPIOE,GPIOE,GPIO_Pin_11);  //��ʼ�� Ѳ�߶�λ���       /IOΪ��������
}
//���IO��ʼ��-
void Output_Init(void)
{
  GPIO_Out_Init(RCC_APB2Periph_GPIOA,GPIOA,GPIO_Pin_15);  //��ʼ�� ����LED/IOΪ�������		
 	GPIO_Out_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_8);   //��ʼ�� ������/IOΪ�������
  GPIO_Out_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_5);   //��ʼ�� 485ʹ�� /IOΪ�������
	
 	GPIO_Out_Init(RCC_APB2Periph_GPIOB,GPIOB,GPIO_Pin_1);  //��ʼ�� ������ /IOΪ�������
	GPIO_Out_Init(RCC_APB2Periph_GPIOF,GPIOF,GPIO_Pin_11); //��ʼ�� ������ /IOΪ�������
	GPIO_Out_Init(RCC_APB2Periph_GPIOF,GPIOF,GPIO_Pin_13); //��ʼ�� ������� /IOΪ�������

  GPIO_Out_Init(RCC_APB2Periph_GPIOB,GPIOB,GPIO_Pin_2);   //��ʼ�� ���������� /IOΪ�������	
  GPIO_Out_Init(RCC_APB2Periph_GPIOF,GPIOF,GPIO_Pin_12);  //��ʼ�� ���������� /IOΪ�������	
  GPIO_Out_Init(RCC_APB2Periph_GPIOF,GPIOF,GPIO_Pin_14);  //��ʼ�� ����������� /IOΪ�������	

	
  GPIO_Out_Init(RCC_APB2Periph_GPIOC,GPIOC,GPIO_Pin_5); //��ʼ�� ѹ�� /IOΪ����������̵�����	
  GPIO_Out_Init(RCC_APB2Periph_GPIOB,GPIOB,GPIO_Pin_0); //��ʼ�� ��צ /IOΪ����������̵�����	
	
	GPIO_ResetBits  (GPIOC,GPIO_Pin_5);  //ѹ��ʧ��	
	GPIO_ResetBits  (GPIOB,GPIO_Pin_0);  //��צʧ��
	GPIO_ResetBits  (GPIOG,GPIO_Pin_8);  //������
	GPIO_ResetBits  (GPIOG,GPIO_Pin_5);  //485ʹ��	
}

/*
   CH444Gģ�⿪�ؽӿڳ�ʼ������
*/
void CH444G_Init(void)
{ 	
  GPIO_Out_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_7); //��ʼ�� CH444G_IN0/IOΪ�������
  GPIO_Out_Init(RCC_APB2Periph_GPIOG,GPIOG,GPIO_Pin_6); //��ʼ�� CH444G_IN1/IOΪ�������
} 

void GPIOX_Init(void)
{
  Input_Init();  //����IO��ʼ��
  Output_Init(); //���IO��ʼ��
  CH444G_Init(); //CH444Gģ�⿪�ؽӿڳ�ʼ��	
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

