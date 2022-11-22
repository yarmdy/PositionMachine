#include "sys.h"

short IndexOfByte(u8* source,u16 sourceLen, u8 data,u16 start);
	
short IndexOfBytes(u8* source,u16 sourceLen, u8* data,u16 dataLen, u16 start);

void RemoveBytes(u8* source,u16 sourceLen,u16 index,u16 len);

void TakeBytes(u8* data,u8* source,u16 index,u16 len);

void GetCRC16(u8* data,u16 dataLen,u8* res);

u8 AnalysisAZHFrame(u8*data,u16 dataLen,u16* index,u16* length);

