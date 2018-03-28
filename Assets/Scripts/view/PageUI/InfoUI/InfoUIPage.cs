using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using DG.Tweening;
using tpgm;


public class InfoUIPage : UIPage
{

	private const string TAG = "InfoUIPage";

	private GameObject friendItem = null;
	private GameObject friendList = null;

	private List<UIFriendItem> friendItems = new List<UIFriendItem>();

	//当前item
	private UIFriendItem currentItem = null;	

	private TabControl tabControl = null;

	private List<TabIndex> tablist = new  List<TabIndex> ();

	//昵称
	private string user_name = "";
	//签名
	private string user_signature = "";

	Controller m_controller;

	public InfoUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/InfoUI/InfoUIPage";

	}

	public override void Awake(GameObject go)
	{


		m_controller = new Controller (this);

		tabControl = this.transform.Find ("tabcontrol").GetComponent<TabControl> () as TabControl;

		tablist.Add (new TabIndex(0,"账号", Paths.info_panel));
		tablist.Add (new TabIndex(1,"段位", Paths.info_panel));
		tablist.Add (new TabIndex(2,"数据", Paths.info_panel));

		for (int i = 0; i < tablist.Count; i++) {
			tabControl.CreateTab (tablist[i].id, tablist[i].tabname, tablist[i].panelPath);
		}


		this.transform.Find("tabcontrol/Panels/panel0/btn_checkhead").GetComponent<Button>().onClick.AddListener(() =>
		{
				//切换头像
				UIPage.ShowPage<PublicUIHeadEdit>();
		});

		this.transform.Find("tabcontrol/Panels/panel0/input_name/btn_nameedit").GetComponent<Button>().onClick.AddListener(() =>
		{
				//修改昵称
				user_name = this.transform.Find("tabcontrol/Panels/panel0/input_name").GetComponent<InputField>().text;
				if(!user_name.Equals(string.Empty))
				{
					m_controller.reqThirdUpdateUser (false,1);
				}
		});

		this.transform.Find("tabcontrol/Panels/panel0/input_signature/btn_signature").GetComponent<Button>().onClick.AddListener(() =>
		{
				//修改签名
				user_signature = this.transform.Find("tabcontrol/Panels/panel0/input_signature").GetComponent<InputField>().text;
				if(!user_signature.Equals(string.Empty))
				{
					m_controller.reqThirdUpdateUser (false,2);
				}
		});

		this.transform.Find("tabcontrol/Panels/panel0/btn_binduser").GetComponent<Button>().onClick.AddListener(() =>
		{
				//
				ClosePage();
		});

		this.transform.Find("tabcontrol/Panels/panel0/btn_bingqq").GetComponent<Button>().onClick.AddListener(() =>
			{
				ClosePage();
			});
		
		this.transform.Find("tabcontrol/Panels/panel0/btn_bindweichat").GetComponent<Button>().onClick.AddListener(() =>
		{
				ClosePage();
		});
		
		this.transform.Find("tabcontrol/Panels/panel0/btn_quit").GetComponent<Button>().onClick.AddListener(() =>
			{
				Application.Quit();
			});
		this.transform.Find("btn_back").GetComponent<Button>().onClick.AddListener(() =>
			{
				ClosePage();
			});
	

	}



	public override void Refresh()
	{
		this.transform.Find("tabcontrol/Panels/panel0/img_head").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+SavedData.s_instance.m_user.m_head);
		this.transform.Find("tabcontrol/Panels/panel0/input_name").GetComponent<InputField>().text = SavedData.s_instance.m_user.m_nickname;
		this.transform.Find("tabcontrol/Panels/panel0/input_signature").GetComponent<InputField>().text = SavedData.s_instance.m_user.m_signature;
		this.transform.Find("tabcontrol/Panels/panel0/tx_follow/tx_count").GetComponent<Text>().text = ""+SavedData.s_instance.m_user.m_follow;
		this.transform.Find("tabcontrol/Panels/panel0/tx_fans/tx_count").GetComponent<Text>().text = ""+SavedData.s_instance.m_user.m_fans;
		this.transform.Find("tabcontrol/Panels/panel0/tx_like/tx_count").GetComponent<Text>().text = ""+SavedData.s_instance.m_user.m_like;

	}


	public void getUserData(JsonThirdUserData js_user)
	{

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

	public const int MSG_HTTP_GETDATA = 1;


	protected override void onHandleMsg(HandlerMessage msg)
	{
		switch (msg.m_what) {
		case MSG_HTTP_GETDATA:
			{
				JsonThirdUserData js_user = SimpleJson.SimpleJson.DeserializeObject<JsonThirdUserData> ((string)msg.m_dataObj);
				SavedData.s_instance.m_user.m_head = js_user.head;
				SavedData.s_instance.m_user.m_nickname = js_user.nickname;
				SavedData.s_instance.m_user.m_fans = js_user.fans;
				SavedData.s_instance.m_user.m_follow = js_user.follow;
				SavedData.s_instance.m_user.m_like = js_user.like;
				SavedData.s_instance.m_user.m_signature = js_user.signature;
				Refresh ();
			}
			break;

		default :
			{

			}
			break;
		}
	}

	class Controller : BaseController<InfoUIPage>,NetHttp.INetCallback
	{
		NetHttp m_netHttp;

		private MainLooper m_initedLooper;

		InfoUIPage m_info;
		public Controller(InfoUIPage iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_initedLooper = MainLooper.instance();
			m_info = iview;
		}


		public void onDestroy()
		{
			m_netHttp.setPageNetCallback (null);

		}

		//用于标识是那个接口用于处理接受函数
		private const int REQ_THIRD_UPDATEUSER = 5;

		public void reqThirdUpdateUser(bool isRetry,int type)
		{

			ReqThirdNewUser paramsValObj;
			string checkID;

			string api = "/newuser";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdNewUser> ();
				paramsValObj.m_isRetry = 1;
				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdNewUser();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_type = type;   //1修改昵称，2修改签名,3 替换头像
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_name = m_info.user_name;
				paramsValObj.m_signature = m_info.user_signature;

			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_UPDATEUSER, checkID);

		}




		//获取用户数据
		private const int REQ_THIRD_GETDATA = 3;

		//重连时不需要后面的参数
		public void reqThirdGetData(bool isRetry)
		{

			ReqThirdGetData paramsValObj;
			string checkID;

			string api = "/getdata";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdGetData> ();
				paramsValObj.m_isRetry = 1;
				checkID = paramsValObj.m_checkID;


			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdGetData();
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_user = 1;   //获取user信息
			}

			string url = SavedContext.getApiUrl(api);
			m_netHttp.postParamsValAsync(url, paramsValObj, REQ_THIRD_GETDATA,checkID);

		}

		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			
			switch (data.m_reqTag) {
			case REQ_THIRD_UPDATEUSER:
				{
					RespThirdNewUser resp = Utils.bytesToObject<RespThirdNewUser> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{

						}
						break;

					default:
						{
							Debug.Log (resp.m_code);
						}
						break;

					}
				}
				break;

			case REQ_THIRD_GETDATA:
				{
					RespThirdGetData resp = Utils.bytesToObject<RespThirdGetData>(respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							Debug.Log (resp.m_userData);
							HandlerMessage msg = MainLooper.obtainMessage(m_info.handleMsgDispatch, MSG_HTTP_GETDATA);
							msg.m_dataObj = resp.m_userData;
							m_initedLooper.sendMessage(msg);

						}
						break;

					default:
						{
							ValTableCache valCache = m_info.getValTableCache();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode>(m_info.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow(valDict, resp.m_code);
							UIPage.ShowPage<PublicUINotice> (val.text);
						}
						break;

					}
				}
				break;
			}

		}

		public virtual void onHttpErr(DataNeedOnResponse data, int statusCode, string errMsg)
		{
			Debug.Log (TAG +":" +"onHttpErr");
		}

		public virtual void onOtherErr(DataNeedOnResponse data, int type)
		{
			Debug.Log (TAG +":" +"onOtherErr");

		}
	}


}




