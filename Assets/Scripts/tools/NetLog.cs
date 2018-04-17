using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

/**************************************
*FileName: NetLog.cs
*User: ysr 
*Data: 2018/1/4
*Describe: 远程接收发送打印的log
**************************************/




public class NetLog : MonoBehaviour
{
	private static List<string> mLines = new List<string>();
	private static List<string> mWriteText = new List<string>();
	private string mPath;


	public int screenLogMaxCount = 8;
	public bool isInputLogOnScreen = false;
	public Color color = Color.red;


	public string ip = "192.168.16.45";
	public int point = 60000;

	private UdpClient udpClient;
	private IPEndPoint ipEndPoint;





    #region 初始化
    private static NetLog m_instance;

    //获取资源加载实例
    public static NetLog Instance()
    {
        if (m_instance == null)
        {
			m_instance = new GameObject("NetLog").AddComponent<NetLog>();
        }
        return m_instance;
    }


    #endregion



    void Awake()
	{
        ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), point);
		udpClient = new UdpClient();

		//Application.persistentDataPath 只有这个路径是既可以读又可以写
		mPath = Application.persistentDataPath + "/outLog.txt";
		//Debug.Log("path:" +mPath);

		//每次启动客户端删除之前保存的Log
		if(System.IO.File.Exists(mPath))
		{
			File.Delete(mPath);
		}

        //log监听
        Application.logMessageReceived += HandleLog;
        //Application.logMessageReceived -= HandleLog;   //移除监听

    }



    void Update()
	{
		//文件写入操作必须在主线程完成
		if(mWriteText.Count > 0)
		{
			string[] temp = mWriteText.ToArray();
			foreach(string t in temp)
			{

				using(StreamWriter writer = new StreamWriter(mPath, true, Encoding.UTF8))
				{

					writer.WriteLine(t);
				}
				mWriteText.Remove(t);
			}

		}

	}

	//通过udp发送的接收服务器上
	void HandleLog(string logString, string stackTrace, LogType type)
	{
		mWriteText.Add(logString);
		if(type == LogType.Log || type == LogType.Error  || type == LogType.Exception)
		{
			Log(logString);
            //Debug.Log(""+type);

		}

		try{
			byte[] bytes;
			bytes = Encoding.UTF8.GetBytes("1"+","+type+","+logString+ "\n"+ stackTrace);
			udpClient.Send(bytes, bytes.Length, ipEndPoint);
		}catch (System.Exception)
		{


		}

	}

	//把错误信息保存起来，用于输出
	public void Log(params object[] objs )
	{
		string text = "";
		for(int i =0; i< objs.Length; ++i)
		{
			if(i ==0 )
			{
				text += objs[i].ToString();	
			}
			else
			{
				text +="," +objs[i].ToString();
			}
		}
		if(Application.isPlaying)
		{
			if(mLines.Count > screenLogMaxCount)
			{
				mLines.RemoveAt(0);
			}
			mLines.Add(text);
		}	

	}

	void OnGUI()
	{
		if(isInputLogOnScreen)
		{
			GUI.color= color;
			int count = 0;
			for(int i = mLines.Count -1; i >= 0&& count < screenLogMaxCount; --i)
			{
				count++;
                GUILayout.Label(mLines[i]);

			}

		}
	}



}
