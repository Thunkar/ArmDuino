#include <Servo.h>

Servo base;
Servo horizontal1;
Servo vertical1;
Servo horizontal2;
Servo vertical2;
Servo horizontal3;
Servo pinza;
Servo servos[] = {base, horizontal1, vertical1, horizontal2, vertical2, horizontal3, pinza};
int positions[] = {90,90,90,90,90,90,90};
int targets[] = {90,90,90,90,90,90,90};
boolean movementStatus[] = {true, true, true, true, true, true, true};
long stepTimer[7];
long lastSteps[7];
long movementPeriod = 300000;
int data[8];
char dataBytes[24];
boolean iAmConnected = false;
boolean iWasConnected = false;

#define DEBUG

void setFlagsToFalse() {
  for(int i = 0; i < 7; i++){
    movementStatus[i] = false; 
  }
}

void setTargets(int data[]){
  targets[0] = data[1];
  targets[1] = data[2];
  targets[2] = data[3];
  targets[3] = 180- data[4];
  targets[4] = data[5];
  targets[5] = data[6];
  targets[6] = data[7];
  for(int i = 0; i < 7; i++){
    int difference = abs(positions[i]-targets[i]);
    if(difference == 0){
      movementStatus[i] = true;
    }
    else{
      movementStatus[i] = false;
      stepTimer[i] = movementPeriod/difference;
    }
  }
}

boolean isItDoneYet(){
  return movementStatus[0] & movementStatus[1] & movementStatus[2] & movementStatus[3] & movementStatus[4] & movementStatus[5] & movementStatus[6];
}

void moveStep(int servo, int target){
  if(movementStatus[servo] == true) return;
  if((micros()-lastSteps[servo])>=stepTimer[servo]){
    if(positions[servo]<target){
      positions[servo] += 1;
      servos[servo].write(positions[servo]);
      lastSteps[servo] = micros();
    }
    else{
      positions[servo] -= 1;
      servos[servo].write(positions[servo]);
      lastSteps[servo] = micros();
    }
  }
  if(positions[servo] == target) movementStatus[servo] = true;
}

void moveSegments(){
    for(int i = 0; i < 7; i++){
      moveStep(i, targets[i]);
    }
}


boolean sameTargets(int data[]){
  return
  targets[0] == data[1] &
  targets[1] == data[2] &
  targets[2] == data[3] &
  targets[3] == 180- data[4] &
  targets[4] == data[5] &
  targets[5] == data[6] &
  targets[6] == data[7];
}

void dataBytesFormatter(char dataBytes[]){
  int counter = 0;
  for(int i = 0; i < 24; i=i+3){
    data[counter] = dataBytes[i]*100 + dataBytes[i+1]*10 + dataBytes[i+2];
    counter++;
  }
}

void reset()
{
    for(int i = 0; i < 7; i++)
    {
      positions[i] = 90;
      targets[i] = 90;
    }
    digitalWrite(13, LOW);
    pinza.write(170);
    delay(500);
    base.write(90);
    delay(500);
    vertical1.write(90);
    delay(500);
    vertical2.write(90);
    delay(500);
    horizontal1.write(90);
    delay(500);
    horizontal2.write(90);
    delay(500);
    horizontal3.write(90);
    digitalWrite(13, HIGH);
}



void setup(){
    pinMode(13, OUTPUT);
    digitalWrite(13, LOW);
    pinza.attach(8);
    pinza.write(170);
    delay(500);
    base.attach(2);
    delay(500);
    vertical1.attach(4);
    delay(500);
    vertical2.attach(6);
    delay(500);
    horizontal1.attach(3);
    delay(500);
    horizontal2.attach(5);
    delay(500);
    horizontal3.attach(7);
    Serial.begin(115200);
    digitalWrite(13, HIGH);
}


void processData()
{
  byte initCode = data[0];
  switch(initCode)
  {
    case 200: 
    {
       digitalWrite(13, LOW);
       iWasConnected = true;
       moveStuff();
       break;
    }
    case 201: 
    {
      if(iWasConnected)
      {
        reset();
      }
      iWasConnected = false;
      break;
    }
  }
}

void readData()
{
  if(Serial.available()>23)
  {
    byte nbytes = Serial.readBytes(dataBytes,24);
    dataBytesFormatter(dataBytes);
    #ifdef DEBUG
    Serial.print(nbytes);
    Serial.println(" bytes read");
    for(int i = 0; i < 8; i++)
    {
      Serial.println(data[i]);
    }
    #endif
    Serial.flush();
  }
}

void moveStuff()
{
  if(!sameTargets(data))
  {
      setTargets(data);
  }
  moveSegments();
}

void loop() 
{
  readData();
  processData();
}
