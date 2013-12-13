#include <Servo.h>

Servo base;
Servo horizontal1;
Servo vertical1;
Servo horizontal2;
Servo vertical2;
Servo horizontal3;
Servo pinza;

void moveServo(Servo servo, int newangle){
  int stepDelay = 7;
  int oldangle = servo.read();
  if(oldangle<newangle){
      for(int i = oldangle;  i < newangle; i++){
      servo.write(i);
      delay(stepDelay);
    }
  }
  else{
    for(int i = oldangle; i > newangle; i--){
      servo.write(i);
      delay(stepDelay);
    }
  }
}

void moveSegments(int angbase, int anghorizontal1, int angvertical1, int anghorizontal2, int angvertical2, int anghorizontal3, int angpinza){
  moveServo(base, angbase);
  moveServo(horizontal1, anghorizontal1);
  moveServo(vertical1, 180-angvertical1);
  moveServo(horizontal2, 180-anghorizontal2);
  moveServo(vertical2, angvertical2);
  moveServo(horizontal3, anghorizontal3);
  moveServo(pinza, angpinza);
  
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
}
void loop() {
  moveSegments(90, 50, 90, 10, 90, 30, 160);
  moveSegments(90, 50, 90, 10, 90, 30, 50);
  moveSegments(90, 90, 90, 30, 90, 30, 50);
  moveSegments(0, 90, 90, 30, 90, 30, 160);
}
