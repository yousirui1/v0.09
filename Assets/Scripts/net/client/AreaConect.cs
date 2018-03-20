using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomelo.DotNetClient;
using SimpleJson;
using System.Threading;
using UnityEngine.SceneManagement;
using System;
using tpgm;

public class AreaConect : MonoBehaviour {

	private static AreaConect instance = null;

	private List<string> UserNameList = new List<string>();

	EventController eventController;

	private MainLooper m_initedLooper;
	
	public static AreaConect Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
	
		eventController = GameObject.Find("EventController").GetComponent<EventController>() as EventController;

		m_initedLooper = MainLooper.instance();

		if(null == m_initedLooper)
		{
			 throw new NullReferenceException("MainLooper not inited");
		}

		//注册网络事件监听
		InitNetEvent();
	
		SavedData.s_instance.m_userlist = UserNameList;

		onPomeloEvent_EnterRoom ();
	}	

	void FixedUpdate()
	{
		onPomeloEvent_Move (eventController.ev_Input ());
	}	


	//发送加入房间请求
	public void onPomeloEvent_EnterRoom()
	{
		if (SavedContext.s_client != null) {
			SavedContext.s_client.request ("area.gloryHandler.enterRoom", (data) => {
				if(null != data)
				{
					System.Object roomNum = null;
					if (data.TryGetValue("roomNum", out roomNum))
					{
						SavedData.s_instance.m_roomNum = roomNum.ToString();
						onPomeloEvent_Match();
					}
				}
			});
		} else {
			Debug.LogError ("pClient null");
		}


	}



	//发送匹配请求
	public void onPomeloEvent_Match()
	{
		if (SavedContext.s_client != null) {
			JsonObject jsMsg = new JsonObject ();
			jsMsg ["roomNum"] = SavedData.s_instance.m_roomNum;
			SavedContext.s_client.request ("area.gloryHandler.match", jsMsg, (data) => {
                Debug.Log(data);
			});
		} else {
			Debug.LogError ("pClient null");
		}
	}

	//发送匹配请求
	public void onPomeloEvent_Move(PlayerVal entite)
	{
		if (SavedContext.s_client != null ) {
			if (entite != null) {
				
				JsonObject jsMsg = new JsonObject ();
                //sDebug.Log(SavedData.s_instance.m_roomNum);
                jsMsg["roomNum"] = SavedData.s_instance.m_roomNum;
               
                jsMsg ["x"] = entite.x;
				jsMsg ["y"] = entite.y;
				jsMsg ["d"] = entite.d;
				jsMsg ["v"] = entite.v;

				jsMsg ["sp"] = entite.sp;
				jsMsg ["hp"] = entite.hp;
				jsMsg ["score"] = entite.score;
				jsMsg ["lev"] = entite.lev;

				jsMsg ["item"] = 0;
				jsMsg ["magicStage"] = 0;
				//Debug.Log (entite.skill);
				jsMsg ["skill"] = entite.skill;
				SavedContext.s_client.request ("area.playHandler.move", jsMsg, (data) => {

				});
				entite.skill = 0;
			}
		} else {
			Debug.LogError ("pClient null");
		}
	}


	//玩家死亡请求接口
	public void onPomeloEvent_Dead(string stObj)
	{
		if (SavedContext.s_client != null ) {
			JsonObject jsMsg = (JsonObject)SimpleJson.SimpleJson.DeserializeObject (stObj);
			Debug.Log (jsMsg);
				SavedContext.s_client.request ("area.playHandler.dead", jsMsg, (data) => {
					Debug.Log(data);
				});
			}
		else {
			Debug.LogError ("pClient null");
		}
	}


	//玩家复活请求接口
	public void onPomeloEvent_Revive()
	{
		if (SavedContext.s_client != null ) {
			JsonObject jsMsg = new JsonObject ();
			jsMsg["roomNum"] = SavedData.s_instance.m_roomNum;
			SavedContext.s_client.request ("area.playHandler.revive", jsMsg, (data) => {
				Debug.Log(data);
			});
		}
		else {
			Debug.LogError ("pClient null");
		}
	}



#if false
	//发送匹配请求
	public void onPomeloEvent_Dead()
	{
		if (SavedContext.s_client != null ) {
			SavedContext.s_client.request ("area.playHandler.dead", jsMsg, (data) => {
					
			});

		} 
		else {
			Debug.LogError ("pClient null");
		}
	}


	//发送匹配请求
	public void onPomeloEvent_Dead()
	{
		if (SavedContext.s_client != null ) {
			SavedContext.s_client.request ("area.playHandler.dead", jsMsg, (data) => {

			});

		} 
		else {
			Debug.LogError ("pClient null");
		}
	}

#endif

	//注册网络事件
	void InitNetEvent()
	{
		PomeloClient pClient = SavedContext.s_client;
		if (pClient != null)
		{

			pClient.on("match", (data) =>{
                HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_MATCH);
                Debug.Log(data);
                msg.m_dataObj = data;
                m_initedLooper.sendMessage(msg);
            });

			pClient.on("gloryAdd", (data) =>{
				HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_GLORYADD);
				Debug.Log(data);
				msg.m_dataObj = data;
				m_initedLooper.sendMessage(msg);
			});

			pClient.on("load", (data) =>{
				HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_LOAD);
				Debug.Log(data);
				msg.m_dataObj = data;
				m_initedLooper.sendMessage(msg);
			});

			pClient.on("playerInfo", (data) =>{
				//HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_PLAYERINFO);
				//msg.m_dataObj = data;
				//m_initedLooper.sendMessage(msg);
			});


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

		
		}

	}

    private const int MSG_POMELO_MATCH = 1;
	private const int MSG_POMELO_GLORYADD = 2;
    private const int MSG_POMELO_LOAD = 3;
	private const int MSG_POMELO_PLAYERINFO = 4;
	private const int MSG_POMELO_MOVEINFO = 5;
	private const int MSG_POMELO_PLAYDATA = 6;


	public void handleMessage(HandlerMessage msg)
	{   
		switch (msg.m_what)
		{
           case MSG_POMELO_MATCH:
               {
                    JsonObject data = (JsonObject)msg.m_dataObj;
                    System.Object match = null;
                    if (data.TryGetValue("match", out match))
                    {
                        if (Convert.ToInt32(match) == 1)
                        {
                            //Debug.Log(Convert.ToInt32(match));
                        }
						if (Convert.ToInt32 (match) == 2) 
						{
							System.Object roomNum = null;
							if (data.TryGetValue ("roomNum", out roomNum)) {
							SavedData.s_instance.m_roomNum = roomNum.ToString ();
							}
						}
                    }
                
                }
                break;


		case MSG_POMELO_GLORYADD:
			{   

				JsonObject data = (JsonObject)msg.m_dataObj;
				Debug.Log (data);
				GloryAddBuf buf = SimpleJson.SimpleJson.DeserializeObject<GloryAddBuf> (data.ToString());
				Debug.Log (buf.newUser.Count);
				eventController.ev_InitPlayer (buf.newUser);
			}   
			break;

            case MSG_POMELO_LOAD:
			{   
				
				JsonObject data = (JsonObject)msg.m_dataObj;
				MapVal mapVal = SimpleJson.SimpleJson.DeserializeObject<MapVal> (data.ToString());
				eventController.InitMap (mapVal.map, mapVal.magicStage);
			}   
			break;

		case MSG_POMELO_PLAYERINFO:
			{   

				JsonObject data = (JsonObject)msg.m_dataObj;
				Debug.Log (data);
				PlayerInfoBuf buf = SimpleJson.SimpleJson.DeserializeObject<PlayerInfoBuf> (data.ToString());
				//eventController.ev_InitPlayer (buf.playerInfo);
			}   
			break;


		case MSG_POMELO_MOVEINFO:
			{   

				JsonObject data = (JsonObject)msg.m_dataObj;

				FrameBuf buf = SimpleJson.SimpleJson.DeserializeObject<FrameBuf> (data.ToString());
				eventController.ev_Output (buf);
			}   
			break;

		case MSG_POMELO_PLAYDATA:
			{   

				JsonObject data = (JsonObject)msg.m_dataObj;
				Debug.Log (data);
				PlayData buf = SimpleJson.SimpleJson.DeserializeObject<PlayData> (data.ToString());
				eventController.ev_OutTip (buf);
			}   
			break;

		}   
	}




}






