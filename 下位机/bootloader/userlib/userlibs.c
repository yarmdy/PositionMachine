#include "userlibs.h"

short IndexOfByte(u8* source,u16 sourceLen, u8 data,u16 start){
	while(start<sourceLen){
		if(source[start]!=data){
			continue;
		}
		return start;
	}
	return -1;
}
short IndexOfBytes(u8* source,u16 sourceLen, u8* data,u16 dataLen, u16 start){
	if (sourceLen <= 0 || dataLen <= 0 || start + dataLen > sourceLen) return -1;
	short index = IndexOfByte(source,sourceLen, data[0], start);
	if (index < 0) return -1;
	if (dataLen == 1) return index;
	u16 i;
	for (i = 1; i < dataLen; i++)
	{
		if (data[i] == source[++start]) continue;
		return IndexOfBytes(source,sourceLen, data,dataLen, start);
	}
	return index;
}

void GetCRC16(u8* data,u16 dataLen,u8* res){
	u8 CRC16Lo;
	u8 CRC16Hi;   //CRC???           
	u8 CL; u8 CH;       //????&HA001             
	u8 SaveHi; u8 SaveLo;
	u8* tmpData;
	//int I;
	u16 Flag;
	u16 i;
	CRC16Lo = 0xFF;
	CRC16Hi = 0xFF;
	CL = 0x01; //00000001
	CH = 0xA0;// 10100000
	tmpData = data;
	for (i = 0; i < dataLen; i++)
	{
		CRC16Lo = (u8)(CRC16Lo ^ tmpData[i]); //??????CRC???????       
		for (Flag = 0; Flag <= 7; Flag++)
		{
			SaveHi = CRC16Hi;
			SaveLo = CRC16Lo;
			CRC16Hi = (u8)(CRC16Hi >> 1);      //??????   
			CRC16Lo = (u8)(CRC16Lo >> 1);      //??????        
			if ((SaveHi & 0x01) == 0x01) //???????????1       
			{
				CRC16Lo = (u8)(CRC16Lo | 0x80);   //???????????1     
			}             //?????0                
			if ((SaveLo & 0x01) == 0x01) //??LSB?1,??????????                    
			{
				CRC16Hi = (u8)(CRC16Hi ^ CH);
				CRC16Lo = (u8)(CRC16Lo ^ CL);
			}
		}
	}
	res[1] = CRC16Hi;       //CRC??       
	res[0] = CRC16Lo;       //CRC??    
}

u8 AnalysisAZHFrame(u8*data,u16 dataLen,u16* index,u16* length){
	index[0] = 0;
	length[0] = 0;
	u8 phead[3]="@ZH";
	u8 ptail[2]="\r\n";
	short headIndex = IndexOfBytes(data,dataLen, phead,3, 0);
	if (headIndex < 0)
	{
		//prossError(-1,"????,??\r\n"+String.Join(" ",data.Select(a=>a.ToString("X2"))),null);
		index[0]=0;
		length[0]=dataLen;
		return 0;
	}
	if (dataLen < headIndex + 15)
	{
		//prossError(-1, "?????,??\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
		return 0;
	}

	if (data[headIndex + 3]!= 0x50)
	{
		//prossError(-1, "??????,????\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
		index[0] = headIndex;
		length[0] = 3;
		return 0;
	}

	u16 len = *(u16*)(&data[headIndex+8]);
	if(dataLen< headIndex + 8 + 2 + len+2+2)
	{
		//prossError(-1, "????,??\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
		return 0;
	}
	u8 crc[2];
	GetCRC16(&data[headIndex],10+len,crc);
	if (IndexOfBytes(data,dataLen, crc,2, headIndex + 10 + len) != headIndex + 10 + len)
	{
		//prossError(-1, "????,????\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
		index[0] = headIndex;
		length[0] = 3;
		return 0;
	}
	if (IndexOfBytes(data,dataLen, ptail,2, headIndex + 10 + len + 2) != headIndex + 10 + len + 2) {
		//prossError(-1, "?????,????\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
		index[0] = headIndex;
		length[0] = 3;
		return 0;
	}

	//prossError(-1, "????\r\n" + String.Join(" ", data.Select(a => a.ToString("X2"))), null);
	index[0] = headIndex;
	length[0] = 10 + len + 2 + 2;
	return 1;
}
