using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pomelo.DotNetClient;
using System;
using SimpleJson;
using tpgm;

public class Test : MonoBehaviour {

	private PomeloClient pClient;

	void Start()
	{
		MainLooper.checkSetup ();

		if (null == SavedData.s_instance) {
			SavedData.s_instance = new SavedData ();
		}

		//设置数据存储目录
		SavedContext.setup ("tpgm");

		onPomeloEvent_Login ();
	}


	public void onPomeloEvent_Login()
	{
		if (null == SavedContext.s_client) {
			SavedContext.s_client = new PomeloClient ();
		}
		SavedContext.s_client.NetWorkStateChangedEvent += (state) =>
		{
			Debug.Log(state);
		};
		SavedContext.s_client.initClient(SavedData.s_instance.s_clientUrl, SavedData.s_instance.s_clientPort, () =>
			{
				SavedContext.s_client.connect(null, (data1) =>
					{
						JsonObject jsMsg = new JsonObject();
						SavedData.s_instance.m_user.m_uid = "106838593c644563a355144d6844933a";
						jsMsg["uid"] = SavedData.s_instance.m_user.m_uid;
						SavedContext.s_client.request("gate.gateHandler.queryEntry", jsMsg, onPomeloEvent_Request);
					});
			});

	}

	//gata服务器返回的数据
	private void onPomeloEvent_Request(JsonObject result)
	{
		System.Object code = null;
		if (result.TryGetValue("code", out code))
		{
			if (Convert.ToInt32(code) == 500)
			{
				return;
			}
			else
			{
				SavedContext.s_client.disconnect();
				string host = (string)result["host"];
				int port = Convert.ToInt32(result["port"]);
				SavedContext.s_client.initClient(host, port, () =>
					{
						JsonObject jsMsg = new JsonObject();
						jsMsg["uid"] = SavedData.s_instance.m_user.m_uid;
						SavedContext.s_client.connect(jsMsg, data =>
							{
								InitNetEvent();
								onPomeloEvent_Entry();

							});
					});
			}
		}
	}

	//获取connector服务器数据
	private void onPomeloEvent_Entry()
	{
		JsonObject jsMsg = new JsonObject ();
		jsMsg ["uid"] = SavedData.s_instance.m_user.m_uid;
		SavedContext.s_client.request ("connector.entryHandler.entry", jsMsg, (data) => {


			onPomeloEvent_EnterRoom();
		});
	}

	//发送加入房间请求
	public void onPomeloEvent_EnterRoom()
	{
		if (SavedContext.s_client != null) {
			SavedContext.s_client.request ("area.gloryHandler.enterRoom" ,(data) => {
				Debug.Log(data);
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
			});
		} else {
			Debug.LogError ("pClient null");
		}
	}

	void InitNetEvent()
	{
		PomeloClient pClient = SavedContext.s_client;
		if (pClient != null) {
			pClient.on("invite", (data) =>{
				Debug.Log(data);
			});

			pClient.on("start", (data) =>{
				Debug.Log(data);

			});

			pClient.on("cccc", (data) =>{
				Debug.Log(data);
			});

			pClient.on("gloryAdd", (data) =>{
				Debug.Log(data);
			});

			pClient.on("load", (data) =>{
				Debug.Log(data);
			});

			pClient.on("playerInfo", (data) =>{
				Debug.Log(data);

			});

			pClient.on("gameOver", (data) =>{
				Debug.Log(data);

			});

			pClient.on("roomAdd", (data) =>{
				Debug.Log(data);

			});
		}
	}

	


}
