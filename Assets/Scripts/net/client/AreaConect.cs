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

public class AreaConect : MonoBehaviour {

	private static AreaConect instance = null;

//	private List<string> UserNameList = new List<string>();

	EventController eventController;

	private MainLooper m_initedLooper;

	private bool isGameOver = false;
	
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
	
		//SavedData.s_instance.m_userlist = UserNameList;
	}	

	void OnDestroy()
	{
		Debug.Log (".........");
		SavedContext.s_client.disconnect ();
	}


	void FixedUpdate()
	{
		if(!isGameOver)
		{
			onPomeloEvent_Move (eventController.ev_Input ());
		}

	}	




	//发送匹配请求
	public void onPomeloEvent_Move(PlayerVal entite)
	{
		if (SavedContext.s_client != null ) {
			if (entite != null) {
				JsonObject jsMsg = new JsonObject ();
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

				jsMsg ["skill"] = entite.skill;
				//Debug.Log (jsMsg);
				SavedContext.s_client.request ("area.playHandler.move", jsMsg, (data) => {
				//	Debug.Log(data);
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
			try
			{
				//JsonObject jsMsg = (JsonObject)SimpleJson.SimpleJson.DeserializeObject (stObj);
				JsonObject jsMsg =JsonMapper.ToObject<JsonObject>(stObj);
				SavedContext.s_client.request ("area.playHandler.dead", jsMsg, (data) => {
					Debug.Log(data);
				});
	
			}
			catch (SerializationException ex) 
            {   
                //直接显示: 游戏数据损坏, 请重新启动游戏;
                Log.w<ValUtils>(ex.Message);
                Debug.Log("SerializationException ysr"+ex.Message);
                //tellOnTableLoadErr();
            }   
            catch (Exception ex) 
            {   
                Debug.Log("Exception ysr"+ ex.Message + ", " + ex.GetType().FullName);
                Log.w<ValUtils>(ex.Message + ", " + ex.GetType().FullName);
                //tellOnTableLoadErr();
            }   
		
			}
		else {
			Debug.LogError ("pClient null");
		}
	}


	//玩家复活请求接口
	public void onPomeloEvent_Revive()
	{
		Debug.Log ("onPomeloEvent_Revive");
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
				try{
					//Debug.Log (data);
					//FrameBuf buf = SimpleJson.SimpleJson.DeserializeObject<FrameBuf> (data.ToString());

					RespThirdMove buf = JsonMapper.ToObject<RespThirdMove> (data.ToString());

					eventController.ev_Output (buf.data);
				
					#if false
					RespThirdMove buf = JsonMapper.ToObject<RespThirdMove> (data.ToString());
					List<PlayerVal> val =  new List<PlayerVal>();
				
					Debug.Log(buf.data.Count);
			
					for(int i = 0; i< buf.data.Count ; i++)
					{
						PlayerVal pVal = new PlayerVal();
						pVal.uid = buf.data[i].uid;
						pVal.x = buf.data[i].x;
						pVal.y = buf.data[i].y;
						pVal.d = buf.data[i].d;
						pVal.v = buf.data[i].v;
						pVal.sp = buf.data[i].sp;
						pVal.hp = buf.data[i].hp;
						pVal.score = buf.data[i].score;
						pVal.magicStage = buf.data[i].magicStage;
						pVal.item = buf.data[i].item;
						pVal.lev = buf.data[i].lev;
						pVal.skill = buf.data[i].skill;

						val.Add(pVal);
					}
					if(val == null || eventController == null)
					{
						Debug.Log("val null ............");
					}
					else
					{

						eventController.ev_Output (val);
					}
					#endif
				}
				catch (SerializationException ex)
				{
					//直接显示: 游戏数据损坏, 请重新启动游戏;
					//Log.w<ValUtils>(ex.Message);
					Debug.LogError("SerializationException"+ex.Message);
					SavedContext.s_client.disconnect ();
				}
				catch (Exception ex)
				{
					Debug.Log (data.ToString ());
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
				isGameOver = true;
			}   
			break;

		}   
	}




}






