/*
BbBProtocol.h
Simple serial communication protocol for Arduino
Created by Gregorio Juliana, 2013
*/

#include "Arduino.h"
#include "BbBProtocol.h"

BbBProtocol::BbBPRotocol(int messageSize, int commandsize, long serialSpeed){
	_serialSpeed = serialSpeed;
	_messageSize = messageSize;
	_commandSize = commandSize;
}

void BbBProtocol::begin(){
	Serial.begin(_serialSpeed);
}


void BbBProtocol::dataBytesFormatter(char dataBytes[], int &data){
	int counter = 0;
	for (int i = 0; i < _messageSize; i = i + _commandSize){
		float counter2 = 0;
		for (int j = 0; j < _commandSize){
			data[counter] += dataBytes[j]*pow((float)10, counter2);
		}
		counter++;
	}
}

BbBProtocol::read(int &dataRead){
	if (Serial.available()>_messageSize){
		char dataBytes[_messageSize];
		byte nbytes = Serial.readBytes(dataBytes, _messageSize);
		dataBytesFormatter(dataBytes, dataRead);
		Serial.flush();
	}
}

void BbBProtocol::write(int data[]){

}

