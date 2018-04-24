using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using tpgm;

public class WlanServer : MonoBehaviour
{
	//端口号
	int Port = 10000;

	//聊天信息
	string Message = "";



	//连接数量
	int concount = 15;
	bool usenat = true;

	//滚动视图位置
	Vector2 scrollPosition;

	private NetworkView networkView;

    private string js_map = "{\"map\":\"map2_4\",\"" +
            "magicStage\":[0,0,0,100111,0,100054,100111,100111,100049," +
            "100051,0,0,0,100049,0,100051,100041,100112,100052,100041," +
            "100049,100112,0,100052,100051,100111,100054,100050,100054," +
            "100050,0,0,100112,100041,100052,100041,0,0,100111,0,100111," +
            "0,100041,0,100054,100049,100041,0,0,100050,0,100052,100111," +
            "100112,0,100054,100051,100052,0,100049,100052,100112,100054," +
            "100041,100054,100049,100041,0,0,100112,100054,100041,0,0,100041,100111]}";



    private int conect_count = 0;
	//private FrameBuf buf = new FrameBuf ();
	private List<PlayerVal> buf_data = new List<PlayerVal>();

    private string moveInfoLog = "";

	private bool isInitServer = false;

    //ip uid
	private Dictionary<string ,string> user_list = new Dictionary<string, string>();

	//移动信息
	string moveInfo = "";

	void Start()
	{
		networkView = this.gameObject.AddComponent<NetworkView>();


    }

	void OnGUI()
	{
		//网络连接状态
		switch(Network.peerType)
		{
		//服务器未开启状态
		case NetworkPeerType.Disconnected:
			StartServer();
			break;
			//成功连接至服务器
		case NetworkPeerType.Server:
			OnServer ();
			break;
			//成功连接至客户端
		case NetworkPeerType.Client:
			break;
			//正在尝试连接
		case NetworkPeerType.Connecting:
			break;	
		}

	}

	void StartServer()
	{
		if(GUILayout.Button("创建本地服务器"))
		{
			//创建服务器,运行连接10台主机
			NetworkConnectionError error = Network.InitializeServer(concount, Port,usenat);
			Debug.Log("连接状态" + error);
		}

	}


	void OnServer()
	{

		isInitServer = true;

		GUILayout.Label("服务器创建完毕,等待客户端连接");

		//打印客户端的信息
		for(int i = 0; i< Network.connections.Length; i++)
		{
			GUILayout.Label("客户端的数量" +i);	
			GUILayout.Label("客户端的ip" + Network.connections[i].ipAddress);	
			GUILayout.Label("客户端的端口号" + Network.connections[i].port);
		}



		//断开服务器
		if(GUILayout.Button("断开服务器"))
		{
			Network.Disconnect();
			//重置聊天信息
			Message = "";
			moveInfo = "";	
			buf_data = new List<PlayerVal> ();
		}

        //创建一个滚动视图，用来显示聊天信息
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(1000), GUILayout.Height(600));

		//显示聊天消息
		//GUILayout.Box(Message);

		//显示玩家移动信息
		GUILayout.Box(moveInfo);
		GUILayout.EndScrollView();

	}


	void AddPlayer(string uid, string ip)
	{
		user_list.Add (ip, uid);
		PlayerVal playerVal = new PlayerVal ();
		playerVal.uid = uid;
        playerVal.x = 500;
        playerVal.y = 500;
        buf_data.Add (playerVal);
		St2Json ();
	}

	private void St2Json()
	{
		#if false
		buf.frame = 0;
		buf.time = 0;
		buf.data = buf_data;
        moveInfo = SimpleJson.SimpleJson.SerializeObject (buf);
		#endif
	}

	void FixedUpdate()
	{
		if (conect_count < Network.connections.Length) {
			//用户上线
			networkView.RPC("Request2ClientMap", RPCMode.Others, js_map);
			conect_count = Network.connections.Length;
		}
		else if(conect_count > Network.connections.Length)
		{
			//用户离线
			conect_count = Network.connections.Length;
            user_list.Clear();
			
		}
		if(isInitServer)
		{
			networkView.RPC("Request2ClientMove", RPCMode.All, moveInfo);
        	St2Json();
        	Debug.Log(moveInfo);
		}
	}

	[RPC]
	void Send2ServerLogin(string message, NetworkMessageInfo info)
	{
		AddPlayer (message,info.sender.ipAddress);
	}



	[RPC]
	void Send2ServerMove(string message, NetworkMessageInfo info)
	{
		//Debug.Log (message);
        RequestMove move = SimpleJson.SimpleJson.DeserializeObject<RequestMove>(message);
        Debug.Log(info.sender.ipAddress);
        string uid = "";
        if(user_list.TryGetValue(info.sender.ipAddress, out uid))
        {
            Debug.Log(uid);
            for(int i =0; i<buf_data.Count;i++)
            {
                Debug.Log(buf_data[i].uid);
                if (buf_data[i].uid == uid)
                {
                    buf_data[i].x = move.x;
                    buf_data[i].y = move.y;
                    buf_data[i].d = move.d;
                    buf_data[i].v = move.v;
                    buf_data[i].sp = move.sp;
                    buf_data[i].hp = move.hp;
                    buf_data[i].score = move.score;
                    buf_data[i].lev = move.lev;
                    buf_data[i].item = move.item;
                    buf_data[i].magicStage = move.magicStage;
                    buf_data[i].skill = move.skill;
                    break;
                }
            }
        }
	}




	[RPC]
	void Request2ClientMap(string message, NetworkMessageInfo info)
	{
		
	}




	[RPC]
	void Request2ClientMove(string message, NetworkMessageInfo info)
	{

	
	}


    public class RequestMove
    {
        public string roomNum = "";
        public int x;
        public int y;
        public int d;
        public int v;
        public int sp;
        public int hp;
        public int score;
        public int lev;
        public int item;
        public int magicStage;
        public int skill;

    }



}


#if false

//接受信息
[RPC]
void RequestMessage(string message, NetworkMessageInfo info)
{
Debug.Log(message);
Message += "\n" + "发送者" + info.sender + ":" + message;



}

//接受模型移动信息
[RPC]
void RequestMove(string message, NetworkMessageInfo info)
{
MoveInfo += "\n" + "移动者" +info.sender + "执行了" + message + "移动事件";

}


#endif