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

	private GameObject friendItem = null;
	private GameObject friendList = null;

	private List<UIFriendItem> friendItems = new List<UIFriendItem>();

	//当前item
	private UIFriendItem currentItem = null;	

	private TabControl tabControl = null;

	private List<TabIndex> tablist = new  List<TabIndex> ();

	Controller m_controller;

	private GameObject userObj0;
	private GameObject userObj1;
	private GameObject userObj2;

	private bool isActive_btn = false;

	private bool isActive_img = false;


	public RoomUIPrepare() : base (UIType.Normal, UIMode.HideOther, UICollider.None )
	{
		//布局预制体
		uiPath = "prefabs/ui/RoomUI/RoomUIPrepare";

	}

	public override void Awake(GameObject go)
	{
		//Init(UIValue.room_btnID, UIValue.room_inputID, UIValue.room_txID, UIValue.room_imgID);

		//向服务器创建房间
		//ClientMgr.Instance().CreatRoom();

		//m_controller = new Controller (this);
		//m_controller.onPomeloEvent_EnterRoom ();

		Init ();

		//InitToast ();

		tabControl = this.transform.Find ("tabcontrol").GetComponent<TabControl> () as TabControl;

		tablist.Add (new TabIndex(0,"好友", null));
		tablist.Add (new TabIndex(1,"最近", null));
		tablist.Add (new TabIndex(2,"附近", null));

		for (int i = 0; i < tablist.Count; i++) {
			tabControl.CreateTab (tablist[i].id, tablist[i].tabname, tablist[i].panelPath);
			//this.gameObject.transform.Find("tabcontrol/Tabs/tab"+i+"/img_type").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+i);

		}


		//friendList = this.transform.Find("tabcontrol/Panels/panel0").gameObject;

		//friendItem = this.transform.Find("tabcontrol/Panels/panel0/Viewport/Content/item").gameObject;
		//friendItem.SetActive(false);

		this.transform.Find("user0").GetComponent<Button>().onClick.AddListener(() =>
			{
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
				//m_controller.onPomeloEvent_Match();
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


	//刷新
	public override void Refresh()
	{
		//friendList.transform.localScale = Vector3.zero;	
		//friendList.transform.DOScale(new Vector3(1, 1, 1), 0.5f);

		//Get Friend Data
		//查看前一个页面有没有传入参data没有则初始化GameData	
		//UDFriend.friends friendData = this.data != null ? this.data as UDFriend : GameData.Instance.playerFriend;

		#if false
		if (ConectData.Instance.friedns == null) {
		//ConectData.Instance.friedns = GameData.Instance.playerFriend.friends;
		} else {
		for (int i = 0; i < ConectData.Instance.friedns.Count; i++) {
		CreateFriendItem (ConectData.Instance.friedns [i]);
		}
		}
		#endif

	}


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



	private void CreateFriendItem(UDFriend.Friend friend)
	{
		GameObject go = GameObject.Instantiate(friendItem) as GameObject;
		go.transform.SetParent(friendItem.transform.parent);
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

	private void SetPlayerInfo(string uid ,string name, string headpath)
	{
		Debug.Log ("SetPlayerInfo");
		if (uid == SavedData.s_instance.m_user.m_uid) {
			userObj0.transform.Find ("tx_username").GetComponent<Text> ().text = name;
			userObj0.transform.Find ("img_user").GetComponent<Image> ().sprite = TextureManage.getInstance().LoadAtlasSprite("imges/ui/icon/General_icon","General_icon_"+headpath);
		} else {

		}
	}



	public const int MSG_POMELO_READY = 1;
	public const int MSG_POMELO_INVITE = 2;
	public const int MSG_POMELO_ONCHAT = 3;
	public const int MSG_POMELO_MATCH = 4;
	public const int MSG_POMELO_ROOMADD = 5;
	public const int MSG_POMELO_GLORYADD = 6;
	public const int MSG_POMELO_PALYERINFO = 7;

	private bool isLoadLevel = false;

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
				JsonObject data = (JsonObject)msg.m_dataObj;
				object match;
				if (data.TryGetValue ("match", out match)) {
					if (Convert.ToInt32 (match) == 2 && !isLoadLevel) {
						isLoadLevel = true;
						Application.LoadLevel("Game");
					}
				}
			}
			break;

		case MSG_POMELO_ROOMADD:
			{
				Debug.Log ("MSG_POMELO_ROOMADD");
				JsonObject data = (JsonObject)msg.m_dataObj;
				object uid;
				object name;
				object head;
				if (data.TryGetValue ("uid", out uid)
					&& data.TryGetValue ("name", out name) 
					&& data.TryGetValue ("head", out head) ) {
					SetPlayerInfo (uid.ToString(), name.ToString(), head.ToString());
				}
			}
			break;

		case MSG_POMELO_GLORYADD:
			{
				JsonObject data = (JsonObject)msg.m_dataObj;
				UDGroup.StartPlayerBuf buf = null;
				//buf = SimpleJson.SimpleJson.DeserializeObject<UDGroup.StartPlayerBuf>(jsMsg.ToString());
				//ConectData.Instance.start_groups = new List<UDGroup.Group>(buf.newUser);
			}
			break;

		case MSG_POMELO_PALYERINFO:
			{
				JsonObject data = (JsonObject)msg.m_dataObj;
				//ConectData.Instance.playerInfo = jsMsg;
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
		RoomUIPrepare m_roomuimain;
		//code

		public Controller(RoomUIPrepare iview):base(null)
		{
			m_initedLooper = MainLooper.instance();
			InitNetEvent();
			m_roomuimain = iview;
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

						/*JObject jsMsg = JObject.Parse(data.ToString());
						if(jsMsg.Property("friendArr")!=null)
						{
							buf = JsonConvert.DeserializeObject<UDFriend.FriendBuf>(data.ToString());
							ConectData.Instance.roomNum = buf.roomNum;
							ConectData.Instance.friedns = new List<UDFriend.Friend>(buf.friendArr);
						}
						else
						{
							Debug.Log("null friends");
							object roomNum = new object();
							object code = new object();
							object utcMs = new object();
							if (data.TryGetValue("roomNum", out roomNum) &&
								data.TryGetValue("code", out code) &&
								data.TryGetValue("utcMs", out utcMs))
							{
								Debug.Log(""+roomNum.ToString());
								ConectData.Instance.roomNum = roomNum.ToString();
							}
						}*/
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
				jsMsg ["roomNum"] = "";
				SavedContext.s_client.request ("area.gloryHandler.match", jsMsg, (data) => {
					Debug.Log("onPomeloEvent_Match" + data);
					object code;
					if (data.TryGetValue ("code", out code)) {
						if (Convert.ToInt32 (code) == 200 ) {
							
						}
						else
						{
							ValTableCache valCache = m_roomuimain.getValTableCache();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode>(m_roomuimain.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow(valDict, Convert.ToInt32 (code));
							Debug.Log(val.text);
							m_roomuimain.toast.showToast (val.text);
						}
					}

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
					HandlerMessage msg = MainLooper.obtainMessage(m_roomuimain.handleMsgDispatch, MSG_POMELO_READY);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on ("invite", (data) => {
					HandlerMessage msg = MainLooper.obtainMessage(m_roomuimain.handleMsgDispatch, MSG_POMELO_INVITE);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});


				pClient.on ("onChat", (data) => {
					HandlerMessage msg = MainLooper.obtainMessage(m_roomuimain.handleMsgDispatch, MSG_POMELO_ONCHAT);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on ("match", (data) => {
					Debug.Log("match"+ data);
					HandlerMessage msg = MainLooper.obtainMessage(m_roomuimain.handleMsgDispatch, MSG_POMELO_MATCH);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on ("roomAdd", (data) => {
					Debug.Log("roomAdd"+ data);
					HandlerMessage msg = MainLooper.obtainMessage(m_roomuimain.handleMsgDispatch, MSG_POMELO_ROOMADD);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on ("gloryAdd", (data) => {
					Debug.Log("gloryAdd"+data);
					HandlerMessage msg = MainLooper.obtainMessage(m_roomuimain.handleMsgDispatch, MSG_POMELO_GLORYADD);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				pClient.on ("playerInfo", (data) => {
					Debug.Log("playerInfo"+data);
					HandlerMessage msg = MainLooper.obtainMessage(m_roomuimain.handleMsgDispatch, MSG_POMELO_PALYERINFO);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});
			}
		}




	}


}

