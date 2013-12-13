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
int targets[7];
boolean movementStatus[] = {true, true, true, true, true, true, true};
long stepTimer[7];
long lastSteps[7];
long movementPeriod = 500000;
int data[7];



void setFlagsToFalse() {
  for(int i = 0; i < 7; i++){
    movementStatus[i] = false; 
  }
}

void setTargets(int angbase, int anghorizontal1, int angvertical1, int anghorizontal2, int angvertical2, int anghorizontal3, int angpinza){
  targets[0] = angbase;
  targets[1] = anghorizontal1;
  targets[2] = angvertical1;
  targets[3] = 180- anghorizontal2;
  targets[4] = angvertical2;
  targets[5] = anghorizontal3;
  targets[6] = angpinza;
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

void moveSegments(int angbase, int anghorizontal1, int angvertical1, int anghorizontal2, int angvertical2, int anghorizontal3, int angpinza){
  setTargets(angbase, anghorizontal1, angvertical1, anghorizontal2, angvertical2, anghorizontal3, angpinza);
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
  while(!isItDoneYet()){
    for(int i = 0; i < 7; i++){
      moveStep(i, targets[i]);
    }
  } 
}


void setup(){
  base.attach(2);
  horizontal1.attach(3);
  vertical1.attach(4);
  horizontal2.attach(5);
  vertical2.attach(6);
  horizontal3.attach(7);
  pinza.attach(8);
  horizontal3.write(90);
  horizontal2.write(90);
  horizontal1.write(90);
  base.write(90);
  vertical1.write(90);
  vertical2.write(90);
  pinza.write(170);
  delay(3000);
  Serial.begin(115200);
}

void dataFormatter(String dataString){
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


void loop() {
  moveSegments(90, 50, 90, 10, 90, 30, 160);
  moveSegments(90, 50, 90, 10, 90, 30, 50);
  moveSegments(90, 90, 90, 30, 90, 50, 50);
  moveSegments(90, 90, 90, 30, 90, 30, 50);
  moveSegments(0, 90, 90, 30, 90, 30, 50);
  moveSegments(0, 90, 90, 30, 90, 30, 160);
}
