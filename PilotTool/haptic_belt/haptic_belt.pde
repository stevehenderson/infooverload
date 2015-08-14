
void setup() {
  pinMode(4, OUTPUT);  // declare LED as output
  pinMode(5, OUTPUT);  // declare LED as output
  pinMode(6, OUTPUT);  // declare LED as output
  pinMode(7, OUTPUT);  // declare LED as output
  pinMode(8, OUTPUT);  // declare LED as output
  pinMode(9, OUTPUT);  // declare LED as output
  pinMode(10, OUTPUT);  // declare LED as output
  pinMode(11, OUTPUT);  // declare LED as output
  pinMode(12, OUTPUT);  // declare LED as output
  pinMode(13, OUTPUT);  // declare LED as output 
  clearAll();
  Serial.begin(9600);
}

void clearAll() {
  for(int i=4; i <=13;i++) {
      digitalWrite(i, HIGH);  // turn BUZZERS OFF (HIGH IS OFF)
  }  
}



void loop(){  
  
  //Serial.print("Looping...\n");
  // remote control
  if (Serial.available() > 0) {

    
    int b = Serial.read();
    Serial.print("READ!!" + b);
    clearAll();
    if (b == 'e') digitalWrite(4, LOW);
    if (b == 'r') digitalWrite(5, LOW);
    if (b == 't') digitalWrite(6, LOW);
    if (b == 'y') digitalWrite(7, LOW);
    if (b == 'u') digitalWrite(8, LOW);
    if (b == 'i') digitalWrite(9, LOW);
    if (b == 'o') digitalWrite(10, LOW);
    if (b == 'p') digitalWrite(11, LOW);
    if (b == 'q') digitalWrite(12, LOW);
    if (b == 'w') digitalWrite(13, LOW);
    Serial.flush();
  }
  delay(1000);                  // wait for a second
  

}

