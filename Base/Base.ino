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
long movementPeriod = 500000;
int data[7];
char dataBytes[21];

#define DEBUG


void setFlagsToFalse() {
  for(int i = 0; i < 7; i++){
    movementStatus[i] = false; 
  }
}

void setTargets(int data[]){
  targets[0] = data[0];
  targets[1] = data[1];
  targets[2] = data[2];
  targets[3] = 180- data[3];
  targets[4] = data[4];
  targets[5] = data[5];
  targets[6] = data[6];
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

void dataStringFormatter(String dataString){
  int counter = 0;
  for(int i = 0; i < 21; i = i+3){
    char data1 = dataString.charAt(i);
    char data2 = dataString.charAt(i+1);
    char data3 = dataString.charAt(i+2);
    String datast1(data1);
    String datast2(data2);
    String datast3(data3);
    String datast = datast1+datast2+datast3;
    data[counter] = datast.toInt();
    counter++;
  }
}


boolean sameTargets(int data[]){
  return
  targets[0] == data[0] &
  targets[1] == data[1] &
  targets[2] == data[2] &
  targets[3] == 180- data[3] &
  targets[4] == data[4] &
  targets[5] == data[5] &
  targets[6] == data[6];
}

void dataBytesFormatter(char dataBytes[]){
  int counter = 0;
  for(int i = 0; i < 21; i=i+3){
    data[counter] = dataBytes[i]*100 + dataBytes[i+1]*10 + dataBytes[i+2];
    counter++;
  }
}

boolean initialize(char initBytes[]){
  int condition = 7777;
  int data = initBytes[0]*1000 + initBytes[1]*100 + initBytes[2]*10 + initBytes[3];
  if(condition == data) return true;
  else return false;
}


void setup(){
  pinMode(13, OUTPUT);
  digitalWrite(13, HIGH);
  pinza.attach(8);
  pinza.write(170);
  delay(1000);
  horizontal3.attach(7);
  delay(1000);
  horizontal2.attach(5);
  delay(1000);
  horizontal1.attach(3);
  delay(1000);
  vertical1.attach(4);
  delay(1000);
  vertical2.attach(6);
  delay(1000);
  base.attach(2);
  Serial.begin(115200);
  char initBytes[4];
  while(!initialize(initBytes)){
    if(Serial.available()>3){
      Serial.readBytes(initBytes, 4);
    }
  Serial.flush();
  }
 digitalWrite(13, LOW);
 Serial.println("Connected");
}



void loop() {
  if(Serial.available()>20){
    byte nbytes = Serial.readBytes(dataBytes,21);
    dataBytesFormatter(dataBytes);
    #ifdef DEBUG
    Serial.print(nbytes);
    Serial.println(" bytes read");
    for(int i = 0; i < 7; i++){
      Serial.println(data[i]);
    }
    #endif
    Serial.flush();
  }
  if(!sameTargets(data))
  {
      setTargets(data);
  }
  moveSegments();
}
