using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomelo.DotNetClient;
using LitJson;
using SimpleJson;
using System.Threading;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization;
using tpgm;

/**************************************
*FileName: GlobalConect.cs
*User: ysr 
*Data: 2018/4/25
*Describe: 全局监听单例函数,监听只能执行一次
*			所以用单例保存否则会异常
**************************************/


public class GlobalConect : MonoBehaviour {

	private static GlobalConect instance = null;


	EventController eventController = null;

	private MainLooper m_initedLooper = null;



	public static GlobalConect Instance
	{
		get { return instance; }
	}


	void Awake()
	{
		instance = this;
	}

	void Start()
	{

		m_initedLooper = MainLooper.instance();

		if(null == m_initedLooper)
		{
			throw new NullReferenceException("MainLooper not inited");
		}

		//注册网络事件监听
		InitNetEvent();
	}	

	public void SetEventController(EventController ev)
	{
		this.eventController = ev;
	}

	void OnDestroy()
	{
		Destroy (this);
	}

	//注册网络事件
	void InitNetEvent()
	{
		PomeloClient pClient = SavedContext.s_client;
		if (pClient != null)
		{

			pClient.on("moveInfo", (data) =>{
				//Debug.Log(data);
				HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_MOVEINFO);
				msg.m_dataObj = data;
				m_initedLooper.sendMessage(msg);
			});

			pClient.on("playData", (data) =>{
				//击杀等信息接收
				//Debug.Log("playData"+data);
				HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_PLAYDATA);
				msg.m_dataObj = data;
				m_initedLooper.sendMessage(msg);
			});

			pClient.on("gameOver", (data) =>{
				//击杀等信息接收
				Debug.Log(data);
				HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_GAMEOVER);
				m_initedLooper.sendMessage(msg);
			});

		}

	}


	private const int MSG_POMELO_PLAYERINFO = 1;
	private const int MSG_POMELO_MOVEINFO = 2;
	private const int MSG_POMELO_PLAYDATA = 3;
	private const int MSG_POMELO_GAMEOVER = 4;

	public void handleMessage(HandlerMessage msg)
	{   
		switch (msg.m_what)
		{
		case MSG_POMELO_PLAYERINFO:
			{   
				JsonObject data = (JsonObject)msg.m_dataObj;
				//Debug.Log (data);

				try{
					//RespThirdPlayerInfo buf = SimpleJson.SimpleJson.DeserializeObject<RespThirdPlayerInfo> (data.ToString());
					RespThirdPlayerInfo buf = JsonMapper.ToObject<RespThirdPlayerInfo> (data.ToString());
					//eventController.ev_InitPlayer (buf.playerInfo);
				}
				catch (SerializationException ex)
				{
					//直接显示: 游戏数据损坏, 请重新启动游戏;
					//Log.w<ValUtils>(ex.Message);
					Debug.LogError("SerializationException ysr"+ex.Message);
					SavedContext.s_client.disconnect ();
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception"+ ex.Message + ", " + ex.GetType().FullName);
					SavedContext.s_client.disconnect ();
				}

			}   
			break;


		case MSG_POMELO_MOVEINFO:
			{   
				JsonObject data = (JsonObject)msg.m_dataObj;
				//Debug.Log (data);
				try{
					//RespThirdMove buf = SimpleJson.SimpleJson.DeserializeObject<RespThirdMove> (data.ToString());
					RespThirdMove buf = JsonMapper.ToObject<RespThirdMove> (data.ToString());
					if(eventController != null)
					{
						eventController.ev_Output (buf.moveData);
					}

				}
				catch (SerializationException ex)
				{
					//直接显示: 游戏数据损坏, 请重新启动游戏;
					Debug.LogError("SerializationException"+ex.Message);
					SavedContext.s_client.disconnect ();
				}
				catch (Exception ex)
				{
					//会捕获异常不知道为什么！！！！！ 未解决
					Debug.LogError("Exception "+ ex.Message + ", " + ex.GetType().FullName);
					SavedContext.s_client.disconnect ();
				}
			}   
			break;

		case MSG_POMELO_PLAYDATA:
			{   
				JsonObject data = (JsonObject)msg.m_dataObj;
				//Debug.Log (data);

				try{
					//RespThirdPlayData buf = SimpleJson.SimpleJson.DeserializeObject<RespThirdPlayData> (data.ToString());
					RespThirdPlayData buf = JsonMapper.ToObject<RespThirdPlayData> (data.ToString());
					eventController.ev_OutTip (buf);
				}
				catch (SerializationException ex)
				{
					//直接显示: 游戏数据损坏, 请重新启动游戏;
					//Log.w<ValUtils>(ex.Message);
					Debug.LogError("SerializationException ysr"+ex.Message);
					SavedContext.s_client.disconnect ();
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception ysr"+ ex.Message + ", " + ex.GetType().FullName);
					SavedContext.s_client.disconnect ();
				}

			}   
			break;

		case MSG_POMELO_GAMEOVER:
			{   
				Debug.Log ("gameOver");

				eventController.ev_GameOver();
			}   
			break;

		}   
	}




}






