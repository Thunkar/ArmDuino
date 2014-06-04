//######################################
/* ArmDuino Firmware v1.0
/  Gregorio Juliana Quirós
/  07/03/1014
*///####################################

#include <Servo.h>

//Servo definitions
Servo base;
Servo horizontal1;
Servo vertical1;
Servo horizontal2;
Servo vertical2;
Servo horizontal3;
Servo gripper;
//Servo array
Servo servos[] = {base, horizontal1, vertical1, horizontal2, vertical2, horizontal3, gripper  };
//Instant positions of the servos
int positions[] = {90,90,90,90,90,90,170};
//Target position that servos have to reach
int targets[] = {90,90,90,90,90,90,170};
//Indicates wether the servos have reached their targets
boolean movementStatus[] = {true, true, true, true, true, true, true};
//Store the time each servo has to move a step
long stepTimer[7];
//Stores last time a servo performed a step
long lastSteps[7];
//Global time the arm has to complete the next target
long movementPeriod = 500000;
//Incoming data (formatted)
int data[8];
//Incoming data (raw)
char dataBytes[24];
//Boolean to check wether we were connected last loop or not
boolean iWasConnected = false;

#define DEBUG

/*
/ Here we read the data from the serial port and store it in the data[] array
*/

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
    Serial.flush(); // IMPORTANT! Clear the incoming data buffer
  }
}

/*
/ Formats the incoming data from the serial port and stores the information into the data[] array.
*/

void dataBytesFormatter(char dataBytes[]){
  int counter = 0;
  for(int i = 0; i < 24; i=i+3){
    data[counter] = dataBytes[i]*100 + dataBytes[i+1]*10 + dataBytes[i+2];
    counter++;
  }
}

/*
/ This method takes the data array and sets the target position for each servo. Also, it assigns each servo the time it has to move.
*/
void setTargets(int data[]){
  targets[0] = data[1];
  targets[1] = data[2];
  targets[2] = data[3];
  targets[3] = 180- data[4]; //Note that this servo is reversed by design
  targets[4] = data[5];
  targets[5] = data[6];
  targets[6] = data[7];
  for(int i = 0; i < 7; i++)
  {
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

/*
/ Checks if each servo has to move and moves it. Also, it stores the new position and sets the last time it moved to the present.
*/


void moveStep(int servo, int target){
  if(movementStatus[servo] == true) return; //We are already there? Done!
  if((micros()-lastSteps[servo])>=stepTimer[servo]) //Has enough time passed already?
  {
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

/*
/ Loop to move all the servos
*/

void moveSegments(){
    for(int i = 0; i < 6; i++)
    {
      moveStep(i, targets[i]);
    }
    servos[6].write(targets[6]); // Instantly move gripper
} 

/*
/ Checks if the arm is receiving its actual target as data.
*/

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

/*
/ Checks if the data we are going to process is the same that we have and in that case ignores it. If it's different, time to move
*/

void moveStuff()
{
  if(!sameTargets(data))
  {
      setTargets(data);
  }
  moveSegments();
}

/*
/ Process the incoming data to perform the apropriate actions
*/

void processData()
{
  byte initCode = data[0];
  switch(initCode)
  {
    case 200: // 200 means the incoming data is movement data
    {
      if(!iWasConnected) // if we weren't connected, we signal it with the gripper
      {
        gripper.write(90);
        delay(200);
        gripper.write(170);
      }
       digitalWrite(13, LOW);
       iWasConnected = true;
       moveStuff();
       break;
    }
    case 201: // 201 means disconnection
    {
      if(iWasConnected) // if we were connected, reset the servos
      {
        reset();
      }
      iWasConnected = false;
      break;
    }
  }
}


/*
/ Resets the arm to the waiting for connection state
*/

void reset()
{
    digitalWrite(13, HIGH);
    for(int i = 0; i < 6; i++) 
    {
      positions[i] = 90;
      targets[i] = 90;
    }
    positions[6] = 170;
    targets[6] = 170;
    gripper.write(170);
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
    // gripper signal to indicate we are ready to receive stuff
    delay(300);
    gripper.write(90);
    delay(200);
    gripper.write(170);
}


/*
/ Servo attachment and serial port initialization
*/
void setup(){
    pinMode(13, OUTPUT);
    digitalWrite(13, HIGH); //LED on means the arm is not connected
    gripper.attach(6);
    gripper.write(170);
    delay(500);
    base.attach(7);
    delay(500);
    vertical1.attach(5);
    delay(500);
    vertical2.attach(9);
    delay(500);
    horizontal1.attach(12);
    delay(500);
    horizontal2.attach(10);
    delay(500);
    horizontal3.attach(8);
    // gripper signal to indicate we are ready to receive stuff
    delay(300);
    gripper.write(90);
    delay(200);
    gripper.write(170);
    Serial.begin(115200);
}


void loop() 
{
  readData(); 
  processData();
}
