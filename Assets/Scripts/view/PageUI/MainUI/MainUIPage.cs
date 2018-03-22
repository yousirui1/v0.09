using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;

public class MainUIPage : UIPage
{
	
	private const string TAG = "MainUIPage";

	private long m_Time = 0;

	Coroutine coroutine;


    public MainUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
    {
        //布局预制体
		uiPath = "Prefabs/UI/MainUI/MainUIPage";
    }
   

	//页面重载入
	public override void Refresh()
	{
		//定时器
		coroutine = UIRoot.Instance.StartCoroutine(Timer());
		m_controller.reqThirdGetData (false);
	}

	//定时刷新
	private void AtRefresh()
	{
		//检查是否要刷新
		if (SavedData.s_instance.m_box.checkNeedReload ()||SavedData.s_instance.m_signIn.checkNeedReload()
			|| SavedData.s_instance.m_email.checkNeedReload() ||SavedData.s_instance.m_astrology.checkNeedReload()) {
			//m_controller.reqThirdGetData(false);
		}

		this.transform.Find("bg_username/btn_head").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+SavedData.s_instance.m_user.m_head);
		this.transform.Find ("bg_username/tx_nickname").GetComponent<Text> ().text = SavedData.s_instance.m_user.m_nickname;
		this.transform.Find ("bg_username/tx_level").GetComponent<Text> ().text = ""+SavedData.s_instance.m_user.m_level+"级";
	}



	//秒定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);

			AtRefresh ();
		}
	}

	private bool isActive;
	private GameObject btn_set = null;
	private GameObject btn_email = null;
	private GameObject btn_activity = null;
	private GameObject btn_check =  null;
	private GameObject btn_rank = null;

	//小红点
	private GameObject himt_friends = null;
	private GameObject himt_store = null;
	private GameObject himt_hid = null;

	Controller m_controller;	

    public override void Awake(GameObject go)
    {
		

		m_controller = new Controller(this);


		//初始化
		Init();
		//InitToast ();

		this.gameObject.transform.Find("bg_username/btn_head").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 个人信息
				//UIPage.ShowPage<InfoUIPage>();
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<PublicUINotice>("未完成");
			});
		

		this.gameObject.transform.Find("btn_bigpack").GetComponent<Button>().onClick.AddListener(() =>
			{
				UIRoot.Instance.StopCoroutine(coroutine);
				// 大礼包
				UIPage.ShowPage<PublicUINotice>("大礼包未完成，敬请期待");
			});

		this.gameObject.transform.Find("btn_firstpay").GetComponent<Button>().onClick.AddListener(() =>
			{
				UIRoot.Instance.StopCoroutine(coroutine);
				// 首充
				UIPage.ShowPage<PublicUINotice>("首充包未完成，敬请期待");

			});

		this.gameObject.transform.Find("btn_enchanter").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 背包
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<PublicUINotice>("首充包未完成，敬请期待");
			});
				

		this.gameObject.transform.Find("btn_task").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 任务
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<PublicUITaskPage>();

			});

		this.gameObject.transform.Find("btn_achievement").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 成就
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<AchievementUIPage>();
			});

		this.gameObject.transform.Find("btn_astrology").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 占星
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<AstrologyUIPage>();

			});
		
		this.gameObject.transform.Find("btn_friends").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 好友
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<ChatUIPage>();
			});
		

		this.gameObject.transform.Find("btn_store").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 商城
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<StoreUIPage>();
			});
		
		//隐藏按钮
		this.gameObject.transform.Find("btn_hid").GetComponent<Button>().onClick.AddListener(() =>
			{
				isActive = !isActive;
				Active_btn(isActive);
				int iActive = isActive == false ? 0 : 1;
				PlayerPrefs.SetInt("hid", iActive);
			});
		
		this.gameObject.transform.Find("btn_set").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 设置界面
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<PublicUISetPage>();
			});

		this.gameObject.transform.Find("btn_email").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 邮件
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<PublicUIEmailPage>();
			});

		this.gameObject.transform.Find("btn_activity").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 今日活动
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<PublicUIActivityPage>();
			});

		this.gameObject.transform.Find("btn_check").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 签到
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<PublicUISignInPage>();
			});
				
		
		this.gameObject.transform.Find("btn_rank").GetComponent<Button>().onClick.AddListener(() =>
		{
				//排行榜
				UIRoot.Instance.StopCoroutine(coroutine);
		});

	

		this.gameObject.transform.Find("btn_copperbox").GetComponent<Button>().onClick.AddListener(() =>
		{
				//免费宝箱
				UIRoot.Instance.StopCoroutine(coroutine);
		});

		this.gameObject.transform.Find("btn_goldbox").GetComponent<Button>().onClick.AddListener(() =>
			{
				//星星宝箱
				UIRoot.Instance.StopCoroutine(coroutine);
			});


		this.gameObject.transform.Find("btn_card/bar/btn_card0").GetComponent<Button>().onClick.AddListener(() =>
			{
				//荣耀对决
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<RoomUIPrepare>();
			});

		this.gameObject.transform.Find("btn_card/bar/btn_card1").GetComponent<Button>().onClick.AddListener(() =>
			{
				//荣耀对决
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<RoomUIPrepare>();
			});
		this.gameObject.transform.Find("btn_card/bar/btn_card2").GetComponent<Button>().onClick.AddListener(() =>
			{
				//荣耀对决
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<RoomUIPrepare>();
			});
		this.gameObject.transform.Find("btn_card/bar/btn_card3").GetComponent<Button>().onClick.AddListener(() =>
			{
				//荣耀对决
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<RoomUIPrepare>();
			});
		this.gameObject.transform.Find("btn_card/bar/btn_card4").GetComponent<Button>().onClick.AddListener(() =>
			{
				//荣耀对决
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<RoomUIPrepare>();
			});


		this.gameObject.transform.Find("btn_modequick").GetComponent<Button>().onClick.AddListener(() =>
		{
				//荣耀对决
				UIRoot.Instance.StopCoroutine(coroutine);
				UIPage.ShowPage<RoomUIPrepare>();
		});

    }

	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		//code
		valCache.markPageUseOrThrow<ValCode>(m_pageID, ConstsVal.val_code);
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{
		/*if (resp.m_boxData != 0) {
			SavedData.s_instance.m_box.reloadOk ();
			//JsonTh js_singnindata = SimpleJson.SimpleJson.DeserializeObject<JsonThirdSignInData> (resp.m_signInData);
			Debug.Log(resp.m_boxData);
		}

		if (!resp.m_signInData.Equals (string.Empty)) {
			SavedData.s_instance.m_box.reloadOk ();
			JsonThirdSignInData js_singnindata = SimpleJson.SimpleJson.DeserializeObject<JsonThirdSignInData> (resp.m_signInData);

		}

		if (resp.m_emailData == 1) {
			SavedData.s_instance.m_email.reloadOk ();
		}
		
		//code
		valCache.unmarkPageUse(m_pageID, ConstsVal.val_code);*/
	}


	private void Init()
	{
		#if false
		//初始化GameObject
		btn_set = GameObject.Find("btn_set") as GameObject;
		btn_email = GameObject.Find("btn_email") as GameObject;
		btn_activity = GameObject.Find("btn_activity") as GameObject;
		btn_check = GameObject.Find("btn_check") as GameObject;
		btn_rank = GameObject.Find("btn_rank") as GameObject;

		//himt_friends = GameObject.Find("btn_friends/img_hint") as GameObject;
		himt_store = GameObject.Find("btn_store/img_hint") as GameObject;
		himt_hid = GameObject.Find("btn_hid/img_hint") as GameObject;

		//himt_friends.SetActive (false);
		himt_store.SetActive (false);
		himt_hid.SetActive (false);
	
		int iActive = PlayerPrefs.GetInt("hid");
		//Debug.Log (iActive);
		isActive = iActive == 0 ? false : true;
		Active_btn(isActive);
		#endif
	}

	private void Active_btn(bool isActive)
	{

		btn_set.SetActive(isActive);
		btn_email.SetActive(isActive);
		btn_activity.SetActive(isActive);
		btn_check.SetActive(isActive);
		btn_rank.SetActive(isActive);
	}

	public const int MSG_HIMT_FRIENDS = 1;
	public const int MSG_HIMT_STORE = 2;
	public const int MSG_HIMT_HID = 3;

	private void Himt_msg(int type)
	{

		switch (type) {
		case MSG_HIMT_FRIENDS:
			{
				himt_friends.SetActive (false);
			}
			break;

		case MSG_HIMT_STORE:
			{
				himt_store.SetActive (false);
			}
			break;

		case MSG_HIMT_HID:
			{
				himt_hid.SetActive (false);
			}
			break;
		}
	}

	class Controller : BaseController<MainUIPage>,NetHttp.INetCallback
	{
		NetHttp m_netHttp;

		private MainLooper m_initedLooper;

		MainUIPage m_main;
		public Controller(MainUIPage iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_initedLooper = MainLooper.instance();
			m_main = iview;
		}

		public void onDestroy()
		{
			m_netHttp.setPageNetCallback (null);

			//可能在login页面没有登录
			if (null != SavedContext.s_client) {
				SavedContext.s_client.NetWorkStateChangedEvent -= (state) => {
					Debug.Log (""+state);
				};
			}

		}
			

		//获取用户数据
		private const int REQ_THIRD_GETDATA = 3;

		//获取玩家信息fa
		public void reqThirdGetData(bool isRetry)
		{
			//Debug.Log ("reqThirdGetData");
			ReqThirdGetData paramsValObj;
			string checkID;

			string api = "/getdata";

			//AppUtils.apiCheckID (api);
			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdGetData> ();
				paramsValObj.m_isRetry = 1;
				checkID = paramsValObj.m_checkID;


			} else {
				checkID = AppUtils.apiCheckID(api);
				GPSVal gps = new GPSVal();
				InfoUtil.GetGPS(gps);
				paramsValObj = new ReqThirdGetData();
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_checkID = checkID;
				//SavedData.s_instance.m_user.m_token = "a55e257875aa400c3af73526da2982dff223efa7cbb925037e892828906729c4e6d5e5746b163e500d3beb0079ac296f";
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_user = 1;
				paramsValObj.m_box = 1;//SavedData.s_instance.m_box.checkNeedReload() ? 1 : 0;

				paramsValObj.m_email = 0;//SavedData.s_instance.m_email.checkNeedReload() ? 1 : 0;
				paramsValObj.m_astrology = 0;//SavedData.s_instance.m_astrology.checkNeedReload() ? 1 : 0;

				paramsValObj.m_signIn = 0;// SavedData.s_instance.m_signIn.checkNeedReload() ? 1 : 0;
				paramsValObj.m_Lng = gps.longitude;
				paramsValObj.m_Lat = gps.latitude;
			}
			string url = SavedContext.getApiUrl(api);
			m_netHttp.postParamsValAsync(url, paramsValObj, REQ_THIRD_GETDATA,checkID);

		}

		//获取用户数据
		private const int REQ_THIRD_BOXREWARD = 4;

		//领取宝箱
		public void reqThirdBoxReward(bool isRetry,int user , int box, int friend, int signIn)
		{

			ReqThirdGetData paramsValObj;
			string checkID;

			string api = "/boxreward";

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
				Debug.Log (SavedData.s_instance.m_user.m_uid);

			}

			string url = SavedContext.getApiUrl(api);
			m_netHttp.postParamsValAsync(url, paramsValObj, REQ_THIRD_GETDATA,checkID);

		}



		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			Debug.Log ("onHttpOk");
			switch (data.m_reqTag) {
			case REQ_THIRD_GETDATA:
				{
					RespThirdGetData resp = Utils.bytesToObject<RespThirdGetData> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							if (!resp.m_userData.Equals (string.Empty)) {
								JsonThirdUserData js_userdata = SimpleJson.SimpleJson.DeserializeObject<JsonThirdUserData> (resp.m_userData);
								SavedData.s_instance.m_user.m_head = js_userdata.head;
								SavedData.s_instance.m_user.m_nickname = js_userdata.nickname;
								SavedData.s_instance.m_user.m_level = js_userdata.level;
								SavedData.s_instance.m_user.m_fans = js_userdata.fans;
								SavedData.s_instance.m_user.m_follow = js_userdata.follow;
								SavedData.s_instance.m_user.m_like = js_userdata.like;
								SavedData.s_instance.m_user.m_signature = js_userdata.signature;

							}
							if (resp.m_boxData != 0) {
								SavedData.s_instance.m_box.reloadOk ();
								Debug.Log(resp.m_boxData);
								Debug.Log(resp.m_utcMs);
							}
							#if false
							if (!resp.m_signInData.Equals (string.Empty)) {
								SavedData.s_instance.m_box.reloadOk ();
								JsonThirdSignInData js_singnindata = SimpleJson.SimpleJson.DeserializeObject<JsonThirdSignInData> (resp.m_signInData);

							}
							#endif
							if (resp.m_emailData == 1) {
								SavedData.s_instance.m_email.reloadOk ();
							}
								


						}
						break;
					default:
						{
							Debug.Log (resp.m_code);
							ValTableCache valCache = m_main.getValTableCache ();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode> (m_main.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow (valDict, resp.m_code);
							UIPage.ShowPage<PublicUINotice> (val.text);
						}
						break;

					}
				}
				break;
			case REQ_THIRD_BOXREWARD:
				{
					/*Debug.Log(rewardBuf.reward.gold);

					ItemAwad awad = new ItemAwad();
					awad.id = 0;
					awad.num = 100;

					UIPage.ShowPage<PublicUIAwadPage>(awad);
					ConectData.Instance.userInfo.boxData = rewardBuf.boxTime +30000;
					ConectData.Instance.NewTime =rewardBuf.utcMs;*/

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



