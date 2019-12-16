using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;


//參考別人的，因為使用Update會有延遲，所以使用Thread(但目前使用自己寫的Update也沒有延遲，所以先隱藏(保留))
public class NumControl : MonoBehaviour
{
    SerialPort SP;
    public string ComPort = "COM3";      //Arduino 序列埠
    Thread readThread;   //宣告執行緒
    public string readMessage;  //Arduino讀取到的資料
    bool isNewMessage;   //讀取判斷開關
    
    // Start is called before the first frame update
    void Start()
    {
        if (ComPort != "")
        {
            SP = new SerialPort(ComPort, 9600); //指定連接埠、鮑率並實例化SerialPort
            SP.ReadTimeout = 10;
        }
        try
        {
            SP.Open();
            readThread = new Thread(new ThreadStart(ArdrinoRead));
            readThread.Start();
            print("SerialPort開啟連接");
        }
        catch
        {
            print("SerialPort連接失敗");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isNewMessage) print(readMessage);
        isNewMessage = false;


    }


    void ArdrinoRead()
    {
        while (SP.IsOpen)
        {
            try
            {
                readMessage = SP.ReadLine(); //讀取SerialPort資料並裝入readMessage
                isNewMessage = true;
            }
            catch (System.Exception e)
            {
                print(e.Message);
            }
        }
    }

    public void ArduinoWrite(string message)
    {
        print(message);
        try
        {
            SP.Write(message);
        }
        catch (System.Exception e)
        {
            print(e.Message);
        }
    }

    /// <summary>
    /// 關閉Unity時，關閉序列埠
    /// </summary>
    private void OnApplicationQuit()
    {
        if (SP != null)
        {
            if (SP.IsOpen)
                SP.Close();
        }
    }
}
