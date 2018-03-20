using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;
using Pomelo.DotNetClient;
using SimpleJson;
using DG.Tweening;
using System;

public class ChatUIPage : UIPage
{

	private const string TAG = "ChatUIPage";

	GameObject Item = null;
	GameObject List = null;

	GameObject Desc = null;

	//当前选择的item
	UIEmailItem currentItem = null;

	List<UIEmailItem> Items = new List<UIEmailItem>();


	private TabControl tabControlMain = null;

	private TabControl tabControlMsg = null;

	private List<TabIndex> tablistMain = new List<TabIndex>();

	private List<TabIndex> tablistMsg = new List<TabIndex>();


	Controller m_controller;

    public ChatUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
    {
        //布局预制体
		uiPath = "Prefabs/UI/ChatUI/ChatUIPage";

    }

    public override void Awake(GameObject go)
    {
		m_controller = new Controller (this);

		//m_controller.reqThirdEmails (false);

		tabControlMain = this.transform.Find("tabcontrol").GetComponent<TabControl>() as TabControl;

		tablistMain.Add(new TabIndex(0, "好友", Paths.chat_panel));	
		tablistMain.Add(new TabIndex(1, "粉丝", Paths.chat_panel));	
		tablistMain.Add(new TabIndex(2, "添加", Paths.chat_panel));	

		for(int i =0 ; i<tablistMain.Count; i++)
		{
			tabControlMain.CreateTab(tablistMain[i].id, tablistMain[i].tabname, tablistMain[i].panelPath);
			//设置tab图标
			this.gameObject.transform.Find("tabcontrol/Tabs/tab"+i+"/img_type").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+i);

		}


		/*tabControlMsg = this.transform.Find("tabcontrol").GetComponent<TabControl>() as TabControl;

		tablistMsg.Add(new TabIndex(0, "好友", PathObj.info_panel));	
		tablistMsg.Add(new TabIndex(1, "粉丝", PathObj.info_panel));	
		tablistMsg.Add(new TabIndex(2, "添加", PathObj.info_panel));	

		for(int i =0 ; i<tablistMsg.Count; i++)
		{
			tabControlMsg.CreateTab(tablistMsg[i].id, tablistMsg[i].tabname, tablistMsg[i].panelPath);
			//设置tab图标
			initTab(i);
		}*/


		//聊天面板
		Desc = this.gameObject.transform.Find("group_msg").gameObject;
		Desc.SetActive (false);


		this.gameObject.transform.Find("tabcontrol/Tabs/tab0").GetComponent<Button>().onClick.AddListener(() =>
			{
				checkTab(0);
			});

		this.gameObject.transform.Find("tabcontrol/Tabs/tab1").GetComponent<Button>().onClick.AddListener(() =>
			{
				checkTab(1);
			});

		this.gameObject.transform.Find("tabcontrol/Tabs/tab2").GetComponent<Button>().onClick.AddListener(() =>
			{
				checkTab(2);
			});


		this.gameObject.transform.Find("btn_isMessage").GetComponent<Button>().onClick.AddListener(() =>
			{
				Desc.SetActive (true);
			});
		


		Desc.transform.Find ("content/btn_isActive").GetComponent<Button> ().onClick.AddListener (() => {
			Desc.SetActive (false);
		});

		this.gameObject.transform.Find("btn_back").GetComponent<Button>().onClick.AddListener(() =>
			{
				ClosePage();
			});
		/*
		this.gameObject.transform.Find("tabcontrol/Tabs/tab3").GetComponent<Button>().onClick.AddListener(() =>
			{
				checkTab(3);
			});


		this.gameObject.transform.Find("content/btn_back").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 隐藏
				Hide();
			});

		this.gameObject.transform.Find("content/btn_back").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 隐藏
				Hide();
			});
		*/
        
    }

	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

	}

	private void initTab(int tab)
	{
		List = this.transform.Find("tabcontrol").gameObject;
		Item = this.transform.Find("tabcontrol/Panels/panel"+tab+"/Viewport/Content/item").gameObject;
		Item.SetActive(false);

		ValTableCache valCache = getValTableCache();
		Dictionary<int, ValStore> valDict = valCache.getValDictInPageScopeOrThrow<ValStore>(m_pageID, ConstsVal.val_store);

		for (int i = 1; i <= valDict.Count; i++) {
			ValStore val = ValUtils.getValByKeyOrThrow(valDict, i);
			//Debug.Log (""+val.classify);
			//if(val.classify == tab)
				//sCreateItem(val);	
		}

	}

	//更新Tab
	private void checkTab(int tab)
	{
		/*List = this.transform.Find("tabcontrol").gameObject;
		Item = this.transform.Find("tabcontrol/Panels/panel"+tab+"/Viewport/Content/item").gameObject;
		Item.SetActive(false);

		ValTableCache valCache = getValTableCache();
		Dictionary<int, ValStore> valDict = valCache.getValDictInPageScopeOrThrow<ValStore>(m_pageID, ConstsVal.val_store);

		for (int i = 1; i <= valDict.Count; i++) {
			ValStore val = ValUtils.getValByKeyOrThrow(valDict, i);
			//Debug.Log (""+val.classify);
			if(val.classify == tab)
				CreateItem(val);	
		}*/

	}

	private void CreateItem(List<JsonThirdEmail> email_list)
	{
		List = this.transform.Find("content/bg_email/panel").gameObject;
		Item = this.transform.Find("content/bg_email/panel/Viewport/Content/item").gameObject;
		Item.SetActive(false);

		for (int i = 0; i < email_list.Count; i++) {
			GameObject go = GameObject.Instantiate(Item) as GameObject;
			go.transform.SetParent(Item.transform.parent);
			go.transform.localScale = Vector3.one;
			go.SetActive(true);

			UIEmailItem item = go.AddComponent<UIEmailItem>();
			item.Refresh(email_list[i]);
			Items.Add(item);

			go.AddComponent<Button>().onClick.AddListener(OnClickItem);
		}
	}

	//邮件详情隐藏
	public override void Hide()
	{
		for(int i =0; i<Items.Count; i++)
		{
			GameObject.Destroy(Items[i].gameObject);
		}
		Items.Clear();

		this.gameObject.SetActive(false);

	}

	private void OnClickItem()
	{
		UIEmailItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UIEmailItem>();

		ShowDesc(item);
	}

	//显示详细信息
	private void ShowDesc(UIEmailItem item)
	{
		currentItem = item;
		Desc.SetActive(true);
		Desc.transform.localPosition = new Vector3(300f, Desc.transform.localPosition.y,Desc.transform.localPosition.z);


		Desc.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 0.25f, true);
		RefreshDesc(item);


	}

	//更新详细信息
	private void RefreshDesc(UIEmailItem item)
	{
		Desc.transform.Find("tx_title").GetComponent<Text>().text = item.data.title;	
		Desc.transform.Find("tx_desc").GetComponent<Text>().text = item.data.content;	


		Desc.transform.Find ("btn_draw").GetComponent<Button> ().onClick.AddListener (() => {

		});

	}		


	public const int MSG_POMELO_READY = 1;
	public const int MSG_POMELO_INVITE = 2;
	public const int MSG_POMELO_ONCHAT = 3;
	public const int MSG_POMELO_MATCH = 4;
	public const int MSG_POMELO_ROOMADD = 5;
	public const int MSG_POMELO_GLORYADD = 6;
	public const int MSG_POMELO_PALYERINFO = 7;

	protected override void onHandleMsg(HandlerMessage msg)
	{
		switch (msg.m_what) {
		case MSG_POMELO_READY:
			{

			}
			break;

		case MSG_POMELO_INVITE:
			{

			}
			break;

		case MSG_POMELO_ONCHAT:
			{

			}
			break;

		case MSG_POMELO_MATCH:
			{
				JsonObject jsMsg = (JsonObject)msg.m_dataObj;
				object match;
				if (jsMsg.TryGetValue ("match", out match)) {
					if (Convert.ToInt32 (match) == 2) {
						Application.LoadLevel("Game");
					}
				}
			}
			break;

		case MSG_POMELO_ROOMADD:
			{

			}
			break;

		case MSG_POMELO_GLORYADD:
			{
				JsonObject jsMsg = (JsonObject)msg.m_dataObj;
				UDGroup.StartPlayerBuf buf = null;
				buf = SimpleJson.SimpleJson.DeserializeObject<UDGroup.StartPlayerBuf>(jsMsg.ToString());
				//SavedData.s_instance.start_groups = new List<UDGroup.Group>(buf.newUser);
			}
			break;

		case MSG_POMELO_PALYERINFO:
			{
				JsonObject jsMsg = (JsonObject)msg.m_dataObj;
				//ConectData.Instance.playerInfo = jsMsg;
			}
			break;
		default :
			{

			}
			break;
		}
	}



	class Controller : BaseController<ChatUIPage>
	{

		private MainLooper m_initedLooper;

		PomeloClient m_pClient;
		ChatUIPage m_chat;

		public Controller(ChatUIPage iview):base(null)
		{
			m_initedLooper = MainLooper.instance();
			InitNetEvent();
			m_chat = iview;
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


		//发送加入房间请求
		public void onPomeloEvent_EnterRoom()
		{
			if (SavedContext.s_client != null) {
				SavedContext.s_client.request ("area.gloryHandler.enterRoom", (data) => {
					//onMatch();
					UDFriend.FriendBuf buf = null;
					Debug.Log("onPomeloEvent_EnterRoom"+data);
					if(null != data)
					{
						buf = SimpleJson.SimpleJson.DeserializeObject<UDFriend.FriendBuf>(data.ToString());
						//ConectData.Instance.roomNum = buf.roomNum;
					}
				});
			} else {
				Debug.LogError ("pClient null");
			}


		}

		//发送准备请求
		public void onPomeloEvent_Prepare()
		{

			if (SavedContext.s_client != null) {
				JsonObject jsMsg = new JsonObject();
				jsMsg["roomNum"] = "";   //房间号
				jsMsg["type"] = 1;       //1：准备，2：取消准备
				SavedContext.s_client.request ("area.gloryHandler.prepare", jsMsg, (data) => {
					Debug.Log ("onPomeloEvent_Prepare" + data);
					UDFriend.FriendBuf buf = null;

					if(null != data)
					{
						/*buf = SimpleJson.SimpleJson.DeserializeObject<UDFriend.FriendBuf>(data.ToString());
					ConectData.Instance.roomNum = buf.roomNum;
					Debug.Log(""+buf.roomNum);*/
						//ConectData.Instance.friedns = new List<UDFriend.Friend>(buf.friendArr);
						//Debug.Log(""+buf.friendArr.Length);
						//ConectData.Instance.Channel = buf.frienaArr =UDfriends;
						//eventObj.onove(buf);
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
				//jsMsg ["roomNum"] = ConectData.Instance.roomNum;
				SavedContext.s_client.request ("area.gloryHandler.match", jsMsg, (data) => {
					Debug.Log("onPomeloEvent_Match" + data);
				});
			} else {
				Debug.LogError ("pClient null");
			}
		}


		//注册网络事件
		void InitNetEvent()
		{
			PomeloClient pClient = SavedContext.s_client;
			if (pClient != null)
			{

				pClient.on ("ready", (data) => {

					HandlerMessage msg = MainLooper.obtainMessage(m_chat.handleMsgDispatch, MSG_POMELO_READY);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on ("invite", (data) => {
					HandlerMessage msg = MainLooper.obtainMessage(m_chat.handleMsgDispatch, MSG_POMELO_INVITE);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});


				pClient.on ("onChat", (data) => {
					HandlerMessage msg = MainLooper.obtainMessage(m_chat.handleMsgDispatch, MSG_POMELO_ONCHAT);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on ("match", (data) => {
					Debug.Log("match"+data);
					HandlerMessage msg = MainLooper.obtainMessage(m_chat.handleMsgDispatch, MSG_POMELO_MATCH);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on ("roomAdd", (data) => {
					Debug.Log("roomAdd"+data);
					HandlerMessage msg = MainLooper.obtainMessage(m_chat.handleMsgDispatch, MSG_POMELO_ROOMADD);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on ("gloryAdd", (data) => {
					Debug.Log("gloryAdd"+data);
					HandlerMessage msg = MainLooper.obtainMessage(m_chat.handleMsgDispatch, MSG_POMELO_GLORYADD);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on ("playerInfo", (data) => {
					Debug.Log("playerInfo"+data);
					HandlerMessage msg = MainLooper.obtainMessage(m_chat.handleMsgDispatch, MSG_POMELO_PALYERINFO);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});
			}
		}




	}


}
