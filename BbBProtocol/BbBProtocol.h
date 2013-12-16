/*
BbBProtocol.h 
Simple serial communication protocol for Arduino
Created by Gregorio Juliana, 2013
*/
#ifndef BbBProtocol_h
#define BbBProtocol_h

#include "Arduino.h"

class BbBProtocol
{
public:
	BbBProtocol(int messageSize, int commandSize, long serialSpeed);
	void write(int data[]);
	void read(int &dataRead);
	void begin();

private:
	int _messageSize;
	int _commandSize;
	int _serialSpeed;
	int* dataBytesFormatter(char dataBytes[], int &data);
};


#endif