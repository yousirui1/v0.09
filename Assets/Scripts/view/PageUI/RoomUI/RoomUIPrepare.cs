using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using DG.Tweening;
using tpgm;
using Pomelo.DotNetClient;
using SimpleJson;
using System;



public class RoomUIPrepare : UIPage
{
	private const string TAG = "RoomUIPrepare";

	private GameObject itemObj = null;
	private GameObject listObj = null;

	private List<UIFriendItem> friendItems = new List<UIFriendItem>();

	//当前item
	private UIFriendItem currentItem = null;	

	private TabControl tabControl = null;

	private List<TabIndex> tablist = new  List<TabIndex> ();

	Controller controller;

	private GameObject userObj0;
	private GameObject userObj1;
	private GameObject userObj2;

	private bool isActive_btn = false;

	private bool isActive_img = false;

	//是否有邀请了其他玩家
	private int other_count = 0;


	public RoomUIPrepare() : base (UIType.Normal, UIMode.HideOther, UICollider.None )
	{
		//布局预制体
		uiPath = "prefabs/ui/RoomUI/RoomUIPrepare";

	}

	//刷新
	public override void Refresh()
	{
		controller = new Controller (this);
		controller.onPomeloEvent_EnterRoom ();

		//listObj.transform.localScale = Vector3.zero;
		//listObj.transform.DOScale(new Vector3(1, 1, 1), 0.5f);


	}

	public override void Awake(GameObject go)
	{
		Init ();
		//InitToast ();
		tabControl = this.transform.Find ("tabcontrol").GetComponent<TabControl> () as TabControl;

		tablist.Add (new TabIndex(0,"好友", null));
		tablist.Add (new TabIndex(1,"最近", null));
		tablist.Add (new TabIndex(2,"附近", null));

		for (int i = 0; i < tablist.Count; i++) {
			tabControl.CreateTab (tablist[i].id, tablist[i].tabname, tablist[i].panelPath);
		}

		//listObj = this.transform.Find ().gameObject;
		//itemObj = this.transform.Find ().gameObject;
		//itemObj.SetActive (false);

		this.transform.Find("user0").GetComponent<Button>().onClick.AddListener(() =>
			{
				controller.onPomeloEvent_Invite("7886be03be8c4b48904eb37a98c8bcd1");
				Active_btn(userObj0);
			});

		this.transform.Find("user1").GetComponent<Button>().onClick.AddListener(() =>
			{
				Active_btn(userObj1);
			});

		this.transform.Find("user2").GetComponent<Button>().onClick.AddListener(() =>
			{
				Active_btn(userObj2);
			});
		

		this.transform.Find("btn_start").GetComponent<Button>().onClick.AddListener(() =>
			{
				UIPage.ShowPage<RoomUIMatch>();
			});

		this.transform.Find("btn_back").GetComponent<Button>().onClick.AddListener(() =>
			{
				ClosePage();
			});
	}

	private void Init()
	{
		userObj0 = this.transform.Find ("user0").gameObject;
		userObj1 = this.transform.Find ("user1").gameObject;
		userObj2 = this.transform.Find ("user2").gameObject;


		userObj0.transform.Find ("btn_detail").gameObject.SetActive (false);
		userObj0.transform.Find ("btn_kicking").gameObject.SetActive (false);

		userObj1.transform.Find ("btn_detail").gameObject.SetActive (false);
		userObj1.transform.Find ("btn_kicking").gameObject.SetActive (false);

		userObj2.transform.Find ("btn_detail").gameObject.SetActive (false);
		userObj2.transform.Find ("btn_kicking").gameObject.SetActive (false);

		userObj1.transform.Find ("img_prepare").gameObject.SetActive (false);
		userObj2.transform.Find ("img_prepare").gameObject.SetActive (false);
	
	
	}
	private void Active_btn(GameObject gameObj)
	{
		isActive_btn = !isActive_btn;
		gameObj.transform.Find ("btn_detail").gameObject.SetActive (isActive_btn);
		gameObj.transform.Find ("btn_kicking").gameObject.SetActive (isActive_btn);
	}

	private void Active_img(GameObject gameObj)
	{
		isActive_img = !isActive_img;
		gameObj.transform.Find ("img_prepare").gameObject.SetActive (isActive_img);
		gameObj.transform.Find ("img_prepare").gameObject.SetActive (isActive_img);
	}

	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		//code
		valCache.markPageUseOrThrow<ValCode>(m_pageID, ConstsVal.val_code);
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{
		//code
		valCache.unmarkPageUse(m_pageID, ConstsVal.val_code);
	}
	#if false
	//隐藏
	public override void Hide()
	{
		for(int i = 0; i< friendItems.Count ;i++)
		{
			GameObject.Destroy(friendItems[i].gameObject);
		}
		friendItems.Clear();

		this.gameObject.SetActive(false);
	}
	#endif


	private void CreateFriendItem(UDFriend.Friend friend)
	{
		GameObject go = GameObject.Instantiate(itemObj) as GameObject;
		go.transform.SetParent(itemObj.transform.parent);
		go.transform.localScale = Vector3.one;
		go.SetActive(true);

		UIFriendItem item = go.AddComponent<UIFriendItem>();
		item.Refresh(friend);
		friendItems.Add(item);	

		//add click btn listener
		go.AddComponent<Button>().onClick.AddListener(OnClickFriendItem);

	}

	//邀请好友
	private void OnClickFriendItem()
	{
		//获取好友信息
		UIFriendItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UIFriendItem>();
		//发送给服务端	
		//Client.Send
	}

	private void SetPlayerInfo(string uid ,string name, int headpath)
	{
		if (uid == SavedData.s_instance.m_user.m_uid) {
			userObj0.transform.Find ("tx_username").GetComponent<Text> ().text = name;
			userObj0.transform.Find ("img_user").GetComponent<Image> ().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+headpath);
		} else {
			if (other_count == 0) {
				userObj1.transform.Find ("tx_username").GetComponent<Text> ().text = name;
				userObj1.transform.Find ("img_user").GetComponent<Image> ().sprite = TextureManage.getInstance ().LoadAtlasSprite ("images/ui/icon/General_icon", "General_icon_" + headpath);
			} else if (other_count == 1) {
				userObj2.transform.Find ("tx_username").GetComponent<Text> ().text = name;
				userObj2.transform.Find ("img_user").GetComponent<Image> ().sprite = TextureManage.getInstance ().LoadAtlasSprite ("images/ui/icon/General_icon", "General_icon_" + headpath);
			} else {
				Debug.Log ("玩家数异常");
			}
			other_count++;
		}

	}



	public const int MSG_POMELO_ROOMADD = 1;
	public const int MSG_POMELO_INVITE = 2;


	private bool isLoadLevel = false;

	protected override void onHandleMsg(HandlerMessage msg)
	{
		switch (msg.m_what) {

		case MSG_POMELO_ROOMADD:
			{
				JsonObject data = (JsonObject)msg.m_dataObj;
				object uid = null;
				object name = null;
				object head = null;
				if (data.TryGetValue ("uid", out uid)
					&& data.TryGetValue ("name", out name)
					&& data.TryGetValue ("head", out head)) {
					SetPlayerInfo (uid.ToString (), name.ToString (), Convert.ToInt32 (head));

				}
			}
			break;
			
		default :
			{

			}
			break;
		}
	}


	class Controller : BaseController<RoomUIPrepare>
	{

		private MainLooper m_initedLooper;

		PomeloClient m_pClient;
		RoomUIPrepare m_page;
		//code

		public Controller(RoomUIPrepare iview):base(null)
		{
			m_initedLooper = MainLooper.instance();
			InitNetEvent();
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


		//发送加入房间请求
		public void onPomeloEvent_EnterRoom()
		{
			if (SavedContext.s_client != null) {
				SavedContext.s_client.request ("area.gloryHandler.enterRoom" ,(data) => {
					if(null != data)
					{
						System.Object roomNum = null;
						if (data.TryGetValue("roomNum", out roomNum))
						{
							SavedData.s_instance.m_roomNum = roomNum.ToString();
						}
					}
				});
			} else {
				Debug.LogError ("pClient null");
			}
		}

		//邀请好友
		public void onPomeloEvent_Invite(string uid)
		{
			if (SavedContext.s_client != null) {
				JsonObject jsMsg = new JsonObject ();
				jsMsg["roomNum"] = SavedData.s_instance.m_roomNum;
				jsMsg["uid"] = uid;
				SavedContext.s_client.request ("area.gloryHandler.invite",jsMsg, (data) => {
					Debug.Log(data);
				});
			} else {
				Debug.LogError ("pClient null");
			}
		}


		//其他玩家准备 1:准备 2:取消准备
		public void onPomeloEvent_Prepare(int type)
		{
			if (SavedContext.s_client != null) {
				JsonObject jsMsg = new JsonObject ();
				jsMsg["roomNum"] = SavedData.s_instance.m_roomNum;
				jsMsg["type"] = type;
				SavedContext.s_client.request ("area.gloryHandler.prepare",jsMsg, (data) => {
					Debug.Log(data);
				});
			} else {
				Debug.LogError ("pClient null");
			}
		}




		//房间里聊天
		public void onPomeloEvent_Send(string content)
		{
			if (SavedContext.s_client != null) {
				JsonObject jsMsg = new JsonObject ();
				jsMsg["roomNum"] = SavedData.s_instance.m_roomNum;
				//jsMsg["scope"] = SavedData.s_instance.m_roomNum;
				jsMsg["content"] = content;
				SavedContext.s_client.request ("area.gloryHandler.send",jsMsg, (data) => {
					Debug.Log(data);
				});
			} else {
				Debug.LogError ("pClient null");
			}
		}



		//离开房间
		public void onPomeloEvent_LeaveRoom()
		{
			if (SavedContext.s_client != null) {
				JsonObject jsMsg = new JsonObject ();
				jsMsg["roomNum"] = SavedData.s_instance.m_roomNum;
				SavedContext.s_client.request ("area.gloryHandler.leaveRoom",jsMsg, (data) => {
					Debug.Log(data);
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
				//房间新玩家加入
				pClient.on("roomAdd", (data) =>{
					HandlerMessage msg = MainLooper.obtainMessage(m_page.handleMsgDispatch, MSG_POMELO_ROOMADD);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});
					

				#if false
				pClient.on("match", (data) =>{
					HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_MATCH);
					Debug.Log(data);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on("gloryAdd", (data) =>{
					HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_GLORYADD);
					Debug.Log(data);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on("load", (data) =>{
					HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_LOAD);
					Debug.Log(data);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on("playerInfo", (data) =>{
					//HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_PLAYERINFO);
					//msg.m_dataObj = data;
					//m_initedLooper.sendMessage(msg);
				});


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
				#endif

			}

	}


	}
}
		

	#if false
	//friendList.transform.localScale = Vector3.zero;	
	//friendList.transform.DOScale(new Vector3(1, 1, 1), 0.5f);

	//Get Friend Data
	//查看前一个页面有没有传入参data没有则初始化GameData	
	//UDFriend.friends friendData = this.data != null ? this.data as UDFriend : GameData.Instance.playerFriend;

	if (ConectData.Instance.friedns == null) {
	//ConectData.Instance.friedns = GameData.Instance.playerFriend.friends;
	} else {
	for (int i = 0; i < ConectData.Instance.friedns.Count; i++) {
	CreateFriendItem (ConectData.Instance.friedns [i]);
	}
	}


	#endif

	//Init(UIValue.room_btnID, UIValue.room_inputID, UIValue.room_txID, UIValue.room_imgID);

	//向服务器创建房间
	//ClientMgr.Instance().CreatRoom();

	//m_controller = new Controller (this);
	//m_controller.onPomeloEvent_EnterRoom ();

//friendList = this.transform.Find("tabcontrol/Panels/panel0").gameObject;

//friendItem = this.transform.Find("tabcontrol/Panels/panel0/Viewport/Content/item").gameObject;
//friendItem.SetActive(false);
//this.gameObject.transform.Find("tabcontrol/Tabs/tab"+i+"/img_type").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+i);