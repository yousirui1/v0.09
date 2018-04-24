using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using Pomelo.DotNetClient;
using SimpleJson;
using System;
using tpgm;
using LitJson;
using System.Runtime.Serialization;


public class LinkServerUIPage : UIPage
{

	private const string TAG = "LoadUIPage";

	Controller controller;
	public LinkServerUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/LoadUI/LinkServerUIPage";
	}

	//页面重载入
	public override void Refresh()
	{
		controller.onPomeloEvent_Login ();
	}

	public override void Awake(GameObject go)
	{
		controller = new Controller (this);

	}

	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

	}

	public const int MSG_POMELO_LINKOK = 1;
	public const int MSG_POMELO_LINKERR = 2;
	public const int MSG_POMELO_INVITE = 3;
	public const int MSG_POMELO_RELINK = 4;


	protected override void onHandleMsg(HandlerMessage msg)
	{
		switch (msg.m_what) {
		case MSG_POMELO_LINKOK:
			{
				UIPage.ShowPage<MainUIPage> ();
			}
			break;
		case MSG_POMELO_LINKERR:
			{

			}
			break;
		
		case MSG_POMELO_INVITE:
			{
				JsonObject data = (JsonObject) msg.m_dataObj;
				Debug.Log (data);
			}
			break;

			//重连
		case MSG_POMELO_RELINK:
			{
				JsonObject data = (JsonObject) msg.m_dataObj;
				Debug.Log (data);
				try
				{
				//	JsonReLink reLink = SimpleJson.SimpleJson.DeserializeObject<JsonReLink> (data.ToString());
					JsonReLink reLink = JsonMapper.ToObject<JsonReLink> (data.ToString());
					if (reLink.code == 200) {
						SavedData.s_instance.m_map.file = reLink.map;
						SavedData.s_instance.m_map.skill_list = reLink.magicStage;
						SavedData.s_instance.m_roomNum = reLink.roomNum;


		
						foreach(Uids uids in reLink.uids)
						{
							if(uids.uid == SavedData.s_instance.m_user.m_uid)
							{

							}


							RespThirdUserData userdata = new RespThirdUserData ();
							userdata.nickname = uids.nickname;
							userdata.score = uids.score;
							userdata.kill = uids.attack[0];
							userdata.death = uids.attack[1];
							userdata.assit = uids.attack[2];
							userdata.group = uids.group;
							userdata.head = uids.head;

							if (!SavedData.s_instance.m_userCache.ContainsKey (uids.uid)) {
								SavedData.s_instance.m_userCache.Add (uids.uid, userdata);
								UserRank rank = new UserRank (uids.uid,uids.nickname,0);
								SavedData.s_instance.m_userrank.Add (rank);
							}
						}
						UIPage.ShowPage<LoadingUIPage> ();
					} else {
						Debug.Log("No ReLink");
					}
				}
				  catch (SerializationException ex) 
            	{   
                	//直接显示: 游戏数据损坏, 请重新启动游戏;
                	Log.w<ValUtils>(ex.Message);
                	Debug.Log("SerializationException" + ex.Message);
                	//tellOnTableLoadErr();
            	}   
            	catch (Exception ex) 
            	{   
                	Debug.Log("Exception"+ ex.Message + ", " + ex.GetType().FullName);
                	//Log.w<ValUtils>(ex.Messasoge + ", " + ex.GetType().FullName);
                	//tellOnTableLoadErr();
            	}   

			



			}
			break;
		default :
			{

			}
			break;
		}
	}


	public class JsonReLink
	{

		public int code;
		public string utcMs = "";
		public string roomNum = "";
		public int type;
		public List<int> magicStage;
		public List<int> shuijing;

		public string createTime = "";

		public List<Uids> uids;

		public string map = "";


	}

	public class Uids
	{
		public string uid = "";
		public string nickname = "";
		public int head ;
		public string group = "";
		public int skin;
		public int magicBook;
		public int attackSkin;
		public string genius = "";
		public int pet ;

		public int footprint;
		public int signBox;
		//public 
		public List<int> attack;

		public int d ;

		public int hp;

		public int v;

		public int sp;

		public int score;

		public int lev;

	}



	class Controller : BaseController<LinkServerUIPage>
	{
		private MainLooper m_initedLooper;
		LinkServerUIPage m_page;

		public Controller(LinkServerUIPage iview):base(null)
		{
			m_initedLooper = MainLooper.instance();
			m_page = iview;
		}

		public void onDestroy()
		{
			//可能在login页面没有登录
			if (null != SavedContext.s_client) {
				SavedContext.s_client.NetWorkStateChangedEvent -= (state) => {
					Debug.Log (""+state);
				};
			}

		}


		public void onPomeloEvent_Login()
		{
			if (null == SavedContext.s_client) {
				SavedContext.s_client = new PomeloClient ();
			}
			SavedContext.s_client.NetWorkStateChangedEvent += (state) =>
			{
				//Debug.Log(state);
					//长连接状态改变，多是断连
					//onPomeloEvent_State(state);
			};
			SavedContext.s_client.initClient(SavedData.s_instance.s_clientUrl, SavedData.s_instance.s_clientPort, () =>
			{
				SavedContext.s_client.connect(null, (data1) =>
				{
					JsonObject jsMsg = new JsonObject();
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
				HandlerMessage msg = MainLooper.obtainMessage(m_page.handleMsgDispatch, MSG_POMELO_LINKOK);
				msg.m_dataObj = data;
				m_initedLooper.sendMessage(msg);
				InitNetEvent();

				onPomeloEvent_ReLink();
			});
		}

		//重连
		private void onPomeloEvent_ReLink()
		{
			SavedContext.s_client.request ("area.reloadHandler.reload", (data) => {
				HandlerMessage msg = MainLooper.obtainMessage(m_page.handleMsgDispatch, MSG_POMELO_RELINK);
				//Debug.Log(data);
				msg.m_dataObj = data;
				m_initedLooper.sendMessage(msg);

			});
		}


		//长连接状态改变
		private void onPomeloEvent_State(NetWorkState state)
		{
			switch(state)
			{

			case NetWorkState.CLOSED:
				{
					Debug.Log (TAG +":" +"CLOSED");
					UIPage.ShowPage<LinkServerUIPage> ();
				}
				break;
			case NetWorkState.CONNECTING:
				{
					Debug.Log (TAG +":" +"CONNECTING");
					UIPage.ShowPage<LinkServerUIPage> ();
				}
				break;
			case NetWorkState.CONNECTED:
				{
					Debug.Log (TAG +":" +"CONNECTED");
					//m_page.Hide ();
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
					UIPage.ShowPage<LinkServerUIPage> ();
				}
				break;
			case NetWorkState.ERROR:
				{
					Debug.Log (TAG +":" +"ERROR");
		
				}
				break;
			}
		}
			

		//全局监听事件
		//注册网络事件
		void InitNetEvent()
		{
			PomeloClient pClient = SavedContext.s_client;
			if (pClient != null) {
				pClient.on("invite", (data) =>{
					HandlerMessage msg = MainLooper.obtainMessage(m_page.handleMsgDispatch, MSG_POMELO_INVITE);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
					Debug.Log(data);
				});

				pClient.on("start", (data) =>{
					Debug.Log(data);

				});

		


			}
		}

			
	}

}
