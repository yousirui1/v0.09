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

	EventController eventController = null;

	bool isGameOver = false;

	void Awake()
	{

	}

	void Start()
	{
		eventController = GameObject.Find("EventController").GetComponent<EventController>() as EventController;
		GlobalConect.Instance.SetEventController (eventController);
	}	

	void OnDestroy()
	{
		Destroy (this);
	}


	void FixedUpdate()
	{
		if(!isGameOver)
		{
			onPomeloEvent_Move (eventController.ev_Input ());
		}

	}	

	public void setGameOver()
	{
		isGameOver = true;
	}

	//发送匹配请求
	public void onPomeloEvent_Move(PlayerVal entite)
	{
		if (SavedContext.s_client != null ) {
			if (entite != null) {
				JsonObject jsMsg = new JsonObject ();
				jsMsg["roomNum"] = SavedData.s_instance.m_gameRoom;
				jsMsg ["x"] = entite.x;
				jsMsg ["y"] = entite.y;
				jsMsg ["dx"] = entite.dx;
				jsMsg ["dy"] = entite.dy;
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
			jsMsg["roomNum"] = SavedData.s_instance.m_gameRoom;
			SavedContext.s_client.request ("area.playHandler.revive", jsMsg, (data) => {
				Debug.Log(data);
			});
		}
		else {
			Debug.LogError ("pClient null");
		}
	}



}






