using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Pomelo.DotNetClient;
using SimpleJson;
using System.Threading;
using tpgm;

/**************************************
*FileName: ChatConect.cs
*User: ysr 
*Data: 2017/12/19
*Describe: 连接Chat服务器和数据处理
**************************************/

public class ChatConect : MonoBehaviour
{
	
#region Define
    private static ChatConect instance = null;

    //用户表
    private List<string> UserNameList = new List<string>();


	EventController eventController ;

	private MainLooper m_initedLooper;
   
    

    //private TaskExecutor mTaskExecutor = null;

    //单列
    public static ChatConect Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		instance = this;
	}

  
    // Use this for initialization
    void Start () 
	{
        //线程池不能find物体
        //ThreadPool.QueueUserWorkItem(InitNetEvent);
       // mTaskExecutor = GetComponent<TaskExecutor>();
       // if (mTaskExecutor == null)
         //   mTaskExecutor = gameObject.AddComponent<TaskExecutor>();

		eventController = GameObject.Find("EventController").GetComponent<EventController>() as EventController;
			

		m_initedLooper = MainLooper.instance ();

		if (null == m_initedLooper)
		{
			throw new NullReferenceException("MainLooper not inited");
		}

        //注册网络事件用于接收
        InitNetEvent();

        //创建用户表
        SavedData.s_instance.m_userlist= UserNameList;

        onJoin();

    }

    float timer = 0.1f;
    // Update is called once per frame
    void Update()
    {
        /*timer -= Time.deltaTime;
        if (timer <= 0)
        {
            onChat(eventObj.onInput());
            Debug.Log(string.Format("Timer1 is up !!! time=${0}", Time.time));
            timer = 0.1f;
        }*/
    }
    #endregion


    #region  Send

    //加入房间请求
    void onJoin()
    {
		if (null != SavedContext.s_client)
        {
			SavedContext.s_client.request("chat.roomHandler.join", (data) =>
            {

                Debug.Log("onJoin" + data);
               
            });
        }
        else
        {
            //Debug.LogError("client error");
        }
    }

   

    //定时检测输入发送数据
    void FixedUpdate()
    {
		//Debug.Log ("FixedUpdate");
		onChat(eventController.ev_Input());
    }


    void onChat(PlayerVal entite)
    {
        if (null != entite)
        {
            JsonObject message = new JsonObject();
            message["uid"] = entite.uid;
            message["x"] = entite.x;
            message["y"] = entite.y;
            message["d"] = entite.d;
            message["v"] = entite.v;
            message["skill"] = entite.skill;
            message["hp"] = entite.hp;
            message["score"] = entite.score;
			SavedContext.s_client.request("chat.roomHandler.move", message, (data) =>
            {
					Debug.Log(data);
            });
            entite.skill = 0;
        }
        else
        {
			Debug.Log("entine null");
        }
    }

    //程序退出时通知服务器
    void OnApplicationQuit()
    {
        if (null != SavedContext.s_client)
        {
			SavedContext.s_client.request("chat.roomHandler.leave", (data) =>
            {
                Debug.Log("OnApplicationQuit" + data);
            });
			SavedContext.s_client.disconnect();
        }
        else
        {
            //Debug.LogError("client error");
        }
    }

    #endregion

    #region Recv

    //注册网络事件
    void InitNetEvent()
    {

        Debug.Log("InitNetEvent");

		PomeloClient pClient = SavedContext.s_client;

        if (pClient != null)
        {
            pClient.on("add", (data) => {
                Debug.Log("InitNetEvent onAdd"+data);
                onUserAdd(data);

            });
 
            pClient.on("onLeave", (data) => {
                Debug.Log("InitNetEvent onLeave" + data);
                onUserLeave(data);
            });

            pClient.on("message", (data) =>
            {
				Debug.Log("message"+data);
				HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_MESSAGE);
				msg.m_dataObj = data;
				msg.m_handler = handleMessage;
				m_initedLooper.sendMessage(msg);
               

			
            });
        }
    }


    void onChatAdd(JsonObject jsonObj)
    {
     	//FrameBuf buf = null;
	   	if(null != jsonObj)
		{
			//buf = JsonConvert.DeserializeObject<FrameBuf>(jsonObj.ToString());
            //mTaskExecutor.ScheduleTask(new Task(new Action<FrameBuf>(eventObj.onMove), buf));
			//eventController.ev_Output(buf);
		}	
    }


    void onUserAdd(JsonObject data)
	{
     	Debug.Log("onUserAdd"+data);	 
	}
   


	void onUserLeave(JsonObject data)
	{
		System.Object user = null;
		if(data.TryGetValue("user", out user))
		{
			string userName =user.ToString();
            int count = 0;
			foreach (string name in SavedData.s_instance.m_userlist)
            {
                if(name == userName)
                {
                    break;
                }
                count++;
            }
			SavedData.s_instance.m_userlist.Remove(userName);
            //eventObj.delPlayer(count);

        }	
	}


	private const int MSG_POMELO_MESSAGE = 1;

	public void handleMessage(HandlerMessage msg)
	{
		switch (msg.m_what)
		{
			case MSG_POMELO_MESSAGE:
			{
				JsonObject data = (JsonObject)msg.m_dataObj;
				onChatAdd (data);

			}
			break;
		
		}
	}



    #endregion
}
