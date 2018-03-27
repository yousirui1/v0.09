using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using ProtoBuf;
using System.IO;
using System;
using Pomelo.DotNetClient;
using System.Threading;
using UnityEngine.UI;
using tpgm;

/**************************************
*FileName: GlobalConect.cs
*User: ysr 
*Data: 2017/12/19
*Describe: 全局监听服务端数据
**************************************/

public class GlobalConect : MonoBehaviour
{
	private const string TAG = "GlobalConect";

	private MainLooper m_initedLooper;


	void Start()
	{
		m_initedLooper = MainLooper.instance ();

		if (null == m_initedLooper)
		{
			throw new NullReferenceException("MainLooper not inited");
		}
		InitNetEvent ();
	}

	// Update is called once per frame
	void Update()
	{
	}
		



	//获取connector服务器数据
	private void Entry()
	{
		JsonObject jsMsg = new JsonObject ();
		jsMsg["uid"] = SavedData.s_instance.m_user.m_uid;
		SavedContext.s_client.request ("connector.entryHandler.entry", jsMsg, (data) => {
			
		});
	}


	//注册网络事件
	void InitNetEvent()
	{
		if (SavedContext.s_client != null) {
			SavedContext.s_client.on("invite", (data) =>{
				HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_INVITE);
				msg.m_dataObj = data;
				m_initedLooper.sendMessage(msg);
				Debug.Log(data);
			});
		}
	}


	private const int MSG_POMELO_INVITE = 1;

	public void handleMessage(HandlerMessage msg)
	{
		switch (msg.m_what)
		{
		case MSG_POMELO_INVITE:
			{
				JsonObject data = (JsonObject) msg.m_dataObj;
				Debug.Log (data);
			}
			break;

		}
	}
}
