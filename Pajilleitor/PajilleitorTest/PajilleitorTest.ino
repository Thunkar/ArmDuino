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
      Serial.println(stepTimer[i]);
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
  Serial.begin(9600);
  moveSegments(90, 60, 120, 30, 150, 90, 160);
}
void loop() {
  moveSegments(90, 60, 120, 30, 150, 50, 140);
  moveSegments(70, 60, 120, 30, 150, 130, 140);
}
