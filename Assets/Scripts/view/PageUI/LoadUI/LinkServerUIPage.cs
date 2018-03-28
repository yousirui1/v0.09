using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using Pomelo.DotNetClient;
using SimpleJson;
using System;
using tpgm;


public class LinkServerUIPage : UIPage
{

	private const string TAG = "LoadUIPage";

	Controller controller;
	public LinkServerUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/LoadUI/LinkServerUIPage";

	}

	public override void Awake(GameObject go)
	{
		controller = new Controller (this);
		controller.onPomeloEvent_Login ();
	}





	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

	}

	public const int MSG_POMELO_LINKOK = 1;
	public const int MSG_POMELO_LINKERR = 2;



	protected override void onHandleMsg(HandlerMessage msg)
	{
		switch (msg.m_what) {
		case MSG_POMELO_LINKOK:
			{
				//全局监听服务端数据
				new GameObject ("GlobalNet").AddComponent<GlobalConect> ();
				UIPage.ShowPage<MainUIPage> ();

			}
			break;
		case MSG_POMELO_LINKERR:
			{

			}
			break;
		
		default :
			{

			}
			break;
		}
	}

	class Controller : BaseController<LinkServerUIPage>
	{
		private MainLooper m_initedLooper;
		LinkServerUIPage m_page;

		public Controller(LinkServerUIPage iview):base(null)
		{
			m_initedLooper = MainLooper.instance();
			InitNetEvent ();
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
			Debug.Log ("onPomeloEvent_Login");
			if (null == SavedContext.s_client)
			{

				if (null == SavedContext.s_client)
				{
					SavedContext.s_client = new PomeloClient();
				}
				SavedContext.s_client.NetWorkStateChangedEvent += (state) =>
				{
					Debug.Log(state);
					//长连接状态改变，多是断连
					//onPomeloEvent_State(state);
				};
				SavedContext.s_client.initClient(SavedData.s_instance.s_clientUrl, SavedData.s_instance.s_clientPort, () =>
					{
						SavedContext.s_client.connect(null, (data1) =>
							{
								JsonObject jsMsg = new JsonObject();
								jsMsg["uid"] = SavedData.s_instance.m_user.m_uid;
								//Debug.Log(SavedData.s_instance.m_user.m_uid);
								SavedContext.s_client.request("gate.gateHandler.queryEntry", jsMsg, onPomeloEvent_Request);
							});
					});
			}
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
			jsMsg["uid"] = SavedData.s_instance.m_user.m_uid;
			SavedContext.s_client.request ("connector.entryHandler.entry", jsMsg, (data) => {
				HandlerMessage msg = MainLooper.obtainMessage(m_page.handleMsgDispatch, MSG_POMELO_LINKOK);
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


		//注册网络事件
		void InitNetEvent()
		{
			if (SavedContext.s_client != null) {
				#if false
				SavedContext.s_client.on("invite", (data) =>{
					//HandlerMessage msg = MainLooper.obtainMessage(m_page.handleMsgDispatch, MSG_POMELO_ROOMADD);
					//msg.m_dataObj = data;
					//m_initedLooper.sendMessage(msg);
					Debug.Log(data);
				});
				#endif

			}
		}

			
	}

}
