int Button=2;//按鈕接腳，第2為中斷腳
int LED=10;//LED接腳
bool BtnUp=true; //按下為false，按一下只執行一次的限制開關
int BtnState=0;  //一開始為0，按一下+1，只有0-4的結果

void setup() {
  // put your setup code here, to run once:
  pinMode(Button,INPUT); //按鈕為輸入接角
  digitalWrite(Button,BtnUp);
  pinMode(LED,OUTPUT);    //LED為輸出接角
  Serial.begin(9600);    //設定鮑率
  
  attachInterrupt(0,BtnClick, RISING); //中斷
}

void loop() {
  // put your main code here, to run repeatedly:
  BtnClick();
  Serial.println(BtnState);//按下按鈕，將結果傳給Unity
  UnityRead();
}

//讀取Uituy的結果
void UnityRead(){
  //如果為3代表值全部一樣，LED恆亮。如果為2，代表值2個一樣，LED閃爍。其餘狀態，LED恆暗
    if(Serial.available()){
    char Mode=Serial.read();   
    if (Mode=='3'){
         digitalWrite(LED,HIGH);
      }
    else if(Mode=='2'){
         digitalWrite(LED,HIGH);
         delay(300);
         digitalWrite(LED,LOW);
         delay(300);
      }
    else{
         digitalWrite(LED,LOW);
      }   
   }
  delay(10);
  }

//按鍵，防彈跳
void BtnClick()
{
    if(digitalRead(Button)!=HIGH && BtnUp==true){
    BtnUp=false;
    BtnAdd();
    }
  else if(digitalRead(Button)==HIGH && BtnUp!=true){
    BtnUp=true;
    }
  }

//按一下就+1，只會有0-4的結果
void BtnAdd(){
  if (BtnState>=4) BtnState=0;
  else BtnState++;
 }
