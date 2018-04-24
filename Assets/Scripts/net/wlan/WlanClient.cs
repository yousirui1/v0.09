using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using tpgm;
using tpgm.UI;

public class WlanClient : MonoBehaviour {
	
	//服务器ip
	string IP = "192.168.16.45";
	
	//端口号
	int Port = 10000;

	//输入信息
	string inputMessage = "请输入信息";

	//接收到的信息
	string Message = "";

	
	//滚动视图位置
	Vector2 scrollPosition;
		
	//移动速度
	float speed = 10.0f;
		
	float rotationSpeed = 100.0f;


	private NetworkView networkView;

	private bool isInitMap  = false;

	EventController eventController;

	private List<string> UserNameList = new List<string>();

	private bool isLogin = false;

	void Start() {
		networkView = this.gameObject.AddComponent<NetworkView>();
	

		SavedData.s_instance.m_userlist = UserNameList;

		Invoke ("OnLogin", 0.5f);
	}

	
	void OnGUI()
	{
		switch(Network.peerType)
		{
			//服务器未开启状态
			case NetworkPeerType.Disconnected:
				StartConnect();
				break;
			//成功连接至服务器
			case NetworkPeerType.Server:
				break;
			//客户端连接成功
			case NetworkPeerType.Client:
				OnClient ();
				break;
			//正在尝试连接
			case NetworkPeerType.Connecting:
				break;
		}
	}

	void FixedUpdate()
	{
		if(Network.isClient && isLogin)
		{
			//OnMove (eventController.ev_Input ());
		}
	}

	void StartConnect()
	{
		if(GUILayout.Button("加入游戏"))
		{
			NetworkConnectionError error = Network.Connect(IP, Port);
			Debug.Log(error);
		}

	}


	void OnClient()
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(200), GUILayout.Height(500));
		
		GUILayout.Box(Message);

		GUILayout.BeginHorizontal();

		inputMessage = GUILayout.TextArea(inputMessage);

		if(GUILayout.Button("发送消息"))
		{
			//networkView.RPC("RequestMessage", RPCMode.All, inputMessage);
			PlayerVal playerVal = new PlayerVal ();
			playerVal.x = 0;
			playerVal.y = 0;
			playerVal.d = 0;
			playerVal.v = 0;
			playerVal.hp = 0;
			playerVal.sp= 0;
			playerVal.score = 0;
			OnMove (playerVal);
		}
		
		GUILayout.EndHorizontal();
		

		if(GUILayout.Button("断开连接"))
		{
			Network.Disconnect();
			Message = "";
		}

	



		GUILayout.EndScrollView();

	}

	void OnLogin()
	{
		if (Network.isClient) {
			SavedData.s_instance.m_user.m_uid = InfoUtil.GetMac ();
			networkView.RPC ("Send2ServerLogin", RPCMode.All, SavedData.s_instance.m_user.m_uid);
			Invoke ("LoginSucced", 1.0f);

		} else {
			Invoke ("OnLogin", 0.5f);
		}
	
		
	}


	void LoginSucced()
	{
		isLogin = true;
	}


	public void OnMove(PlayerVal entite)
	{
		JsonObject jsMsg = new JsonObject ();
		jsMsg ["roomNum"] = "wlan_room";

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

		networkView.RPC("Send2ServerMove", RPCMode.All, jsMsg.ToString());
		entite.skill = 0;

	}


	[RPC]
	void Send2ServerLogin(string message, NetworkMessageInfo info)
	{

	}



	[RPC]
	void Send2ServerMove(string message, NetworkMessageInfo info)
	{
		
	}
		

	[RPC]
	void Request2ClientMap(string message, NetworkMessageInfo info)
	{
		if (!isInitMap) {
			isInitMap = true;
			RespThirdLoad mapVal = SimpleJson.SimpleJson.DeserializeObject<RespThirdLoad> (message);
			SavedData.s_instance.m_map.file = mapVal.map;
			SavedData.s_instance.m_map.skill_list = mapVal.magicStage;

			UIPage.ShowPage<LoadingUIPage> ();
			//eventController = GameObject.Find("EventController").GetComponent<EventController>() as EventController;
		}
	}




	[RPC]
	void Request2ClientMove(string message, NetworkMessageInfo info)
	{
		//FrameBuf buf = SimpleJson.SimpleJson.DeserializeObject<FrameBuf> (message);
		//Debug.Log (buf.data.Count);
		//if(buf != null)
		//eventController.ev_Output (buf);
	}





}


