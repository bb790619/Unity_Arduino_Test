using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

//自己寫的
public class BtnControl : MonoBehaviour
{
    //欄位變數
    #region
    SerialPort SP = new SerialPort("COM3", 9600); //指定連接埠、鮑率並實例化SerialPort

    Text[] text;    //放置Text的物件
    int TextNum = 3; //Text的數量
    float TextNow = 0; //顯示目前Text顯示的數字(文字)
    int[] Result;    //紀錄Text的數字(文字)
    bool VictoryState = false; //true時才判斷LED狀態

    int Btn = 0;       //測試用，按鍵的狀態
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //初始化，Text抓取UI的"左中右"的Text
        text = new Text[TextNum];
        Result = new int[TextNum];
        for (int i = 0; i < TextNum; i++)
        {
            text[i] = GameObject.Find("UI").transform.GetChild(i).GetComponent<Text>();
            Result[i] = 0;
        }

        //開啟連接
        SP.Open();
        SP.ReadTimeout = 10;
        if (SP.IsOpen) print("SerialPort開啟連接");

    }

    // Update is called once per frame
    void Update()
    {

        if (SP.ReadLine() != null) print("Arduino的值:" + SP.ReadLine());
        if (SP.IsOpen) AdruinoRead(SP.ReadLine());
        ArduinoWrite();
        
        //測試用
        /*
        ArduinoWrite();
        AdruinoRead(Btn.ToString()); */
    }


    /// <summary>
    /// 讀取Adruino的值
    /// </summary>
    /// <param name="state"></param>
    public void AdruinoRead(string state)
    {
        /*
        讀取Adruinok的值
        初始為 0 => 歸0
        按一下變1=>全部數字移動
        按一下變2=>左停止
        按一下變3=>中停止
        按一下變4=>右停止
        */
        //數字由0開始增加，因為只要顯示0-9，之後會使用%取餘數來處理
        if (TextNow < 10) TextNow += Time.deltaTime * 5;
        else TextNow = 0;

        switch (state)
        {
            case "0": //初始化，歸0
                for (int i = 0; i < 3; i++) text[i].text = "0";
                TextNow = 0;
                VictoryState = false;
                break;
            case "1":
                for (int i = 0; i < 3; i++) text[i].text = ((int)TextNow % 10).ToString("D1"); //這樣就不會跳出10
                break;
            case "2":
                for (int i = 1; i < 3; i++) text[i].text = ((int)TextNow % 10).ToString("D1"); //這樣就不會跳出10，停止左
                break;
            case "3":
                for (int i = 2; i < 3; i++) text[i].text = ((int)TextNow % 10).ToString("D1"); //這樣就不會跳出10，再停止中
                break;
            case "4":
                for (int i = 0; i < 3; i++) Result[i] = int.Parse(text[i].text);  //再停止右，並記錄結果
                VictoryState = true; //代表結束了，開始判斷結果
                break;
        }

    }

    /// <summary>
    /// 將結果傳回Adruino
    /// </summary>
    public void ArduinoWrite()
    {
        /*
         如果3個數字一樣就回傳3，讓燈恆亮
         如果2個數字一樣就回傳2，讓燈閃爍
         其餘狀態回傳1，讓燈恆暗
        */
        if (SP.IsOpen)
        {
            if (VictoryState == false) //未回傳結果時
            {
                SP.Write("0");
                print("未回傳結果");
            }
            else if (Result[0] == Result[1] && Result[0] == Result[2] && VictoryState == true) //3個數字一樣
            {
                SP.Write("3");
                print("數字都一樣，恆亮");
            }
            else if (Result[0] != Result[1] && Result[0] != Result[2] && Result[1]!=Result[2] && VictoryState == true) //3個數字不一樣
            {
                SP.Write("1");
                print("數字都不一樣，恆暗");
            }
            else if (VictoryState == true) //只有2個數字一樣
            {
                SP.Write("2");
                print("只有2個數字一樣，閃爍");
            }
        }
    }

    /// <summary>
    /// 關閉Unity時，關閉序列埠
    /// </summary>
    private void OnApplicationQuit()
    {
        SP.Close();
        print("關閉連接");
    }



    /// <summary>
    /// 測試用，用Button測試
    /// </summary>
    public void ButtonTest()
    {
        if (Btn >= 4) Btn = 0;
        else Btn++;
        print(Btn);
    }

}
