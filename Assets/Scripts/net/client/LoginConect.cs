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
*FileName: LoginConectr.cs
*User: ysr 
*Data: 2017/12/19
*Describe: Login连接gata和connector服务器和数据处理
**************************************/

public class LoginConect : MonoBehaviour
{
	/** reference object*/
	// for login

	//是否完成登录
	public bool isLogin = false;


	private PomeloClient m_pClient;


	private const string TAG = "LoginConect";


	private MainLooper m_initedLooper;


	// Use this for initialization
	void Start()
	{

		m_initedLooper = MainLooper.instance ();

		if (null == m_initedLooper)
		{
			throw new NullReferenceException("MainLooper not inited");
		}
	}

	// Update is called once per frame
	void Update()
	{
	}
		



	public void onLogin()
	{
		if (null == m_pClient)
		{

			if (null == SavedContext.s_client)
			{
				m_pClient = new PomeloClient();
				SavedContext.s_client = m_pClient;
			}
			else
			{
				m_pClient = SavedContext.s_client;
			}
			m_pClient.NetWorkStateChangedEvent += (state) =>
			{
				Debug.Log(state);
				//长连接状态改变，多是断连
				//onPomeloEvent_State(state);
			};
			m_pClient.initClient(SavedData.s_instance.s_clientUrl, SavedData.s_instance.s_clientPort, () =>
				{
					m_pClient.connect(null, (data1) =>
						{
							JsonObject jsMsg = new JsonObject();
							jsMsg["uid"] = SavedData.s_instance.m_user.m_uid;
							m_pClient.request("gate.gateHandler.queryEntry", jsMsg, onEntryRequest);
						});
				});
		}
	}

	//gata服务器返回的数据
	private void onEntryRequest(JsonObject result)
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
				m_pClient.disconnect();
				string host = (string)result["host"];
				int port = Convert.ToInt32(result["port"]);
				m_pClient.initClient(host, port, () =>
				{
						JsonObject jsMsg = new JsonObject();
						jsMsg["uid"] = SavedData.s_instance.m_user.m_uid;
						m_pClient.connect(jsMsg, data =>
						{
							Entry();
						});
				});
			}
		}
	}

	//获取connector服务器数据
	private void Entry()
	{
		JsonObject jsMsg = new JsonObject ();
		jsMsg["uid"] = SavedData.s_instance.m_user.m_uid;
		m_pClient.request ("connector.entryHandler.entry", jsMsg, (data) => {
			onSave (data);
		});
	}
		
	//存储登录数据
	void onSave(JsonObject userData)
	{
		Debug.Log ("onSave");
		//ConectData.Instance.UserName = UserName;
		//ConectData.Instance.Channel = Password;
		//存储了在线用户
		//PlayerData.Instance.LoginUserData = userData;
		SavedContext.s_gameServerConnected = true;
		HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_MESSAGE);
		m_initedLooper.sendMessage(msg);

		InitNetEvent ();
	}

	private const int MSG_POMELO_MESSAGE = 1;

	public void handleMessage(HandlerMessage msg)
	{
		switch (msg.m_what)
		{
		case MSG_POMELO_MESSAGE:
			{
				Debug.Log ("handleMessage");
				//this.gameObject.AddComponent<AreaConect> ();
			}
			break;

		}
	}


	//长连接状态改变
	private void onPomeloEvent_State(NetWorkState state)
	{
		switch(state)
		{

		case NetWorkState.CLOSED:
			{
				Debug.Log (TAG +":" +"CLOSED");
			}
			break;
		case NetWorkState.CONNECTING:
			{
				Debug.Log (TAG +":" +"CONNECTING");
			}
			break;
		case NetWorkState.CONNECTED:
			{
				Debug.Log (TAG +":" +"CONNECTED");
			}
			break;
		case NetWorkState.DISCONNECTED:
			{
				Debug.Log (TAG +":" +"DISCONNECTED");
			}
			break;
		case NetWorkState.TIMEOUT:
			{
				Debug.Log (TAG +":" +"TIMEOUT");
				SavedContext.s_gameServerConnected = false;
			}
			break;
		case NetWorkState.ERROR:
			{
				Debug.Log (TAG +":" +"ERROR");
				SavedContext.s_gameServerConnected = true;
			}
			break;
		}
	}


	//注册网络事件
	void InitNetEvent()
	{
		if (m_pClient != null) {
			m_pClient.on ("add", (data) => {
				//Debug.Log ("InitNetEvent add" + data);
				//Debug.Log("message"+data);
			});
		}
	}

}
