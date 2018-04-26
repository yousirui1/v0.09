using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using DG.Tweening;
using System.IO;
using System.Text;
using tpgm;


public class StartUIPage : UIPage
{
	private const string TAG = "StartUIPage";
	GameObject startPanelObj;

	private string m_uid = "";
	private string m_passwd = "";

	//http控制
	Controller m_controller;
	 
	EventController eventController;

	AsyncOperation async;

	Coroutine coroutine;

	private string js_map = "{\"map\":\"map2_4\",\"" +
		"magicStage\":[0,0,0,100111,0,100054,100111,100111,100049," +
		"100051,0,0,0,100049,0,100051,100041,100112,100052,100041," +
		"100049,100112,0,100052,100051,100111,100054,100050,100054," +
		"100050,0,0,100112,100041,100052,100041,0,0,100111,0,100111," +
		"0,100041,0,100054,100049,100041,0,0,100050,0,100052,100111," +
		"100112,0,100054,100051,100052,0,100049,100052,100112,100054," +
		"100041,100054,100049,100041,0,0,100112,100054,100041,0,0,100041,100111]}";



	public StartUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/LoginUI/StartUIPage";

	}

	public override void Awake(GameObject go)
	{
		startPanelObj = this.gameObject.transform.Find ("startPanel").gameObject;

		m_controller = new Controller(this);

		//初始化Toash
		toast.InitToast (this.gameObject);

	
		Effect (this.transform.Find("startPanel/bg_logo").gameObject);
		this.transform.Find("startPanel/btn_start").GetComponent<Button>().onClick.AddListener(() =>
		{
			if(PrefValSet.isLogin())
			{
				m_uid = PrefValSet.getUid();
				m_passwd = PrefValSet.getPasswd();
				m_controller.reqThirdLogin(false);
				
			}
			else
			{
				UIPage.ShowPage<LoginModeUIPage>();
				startPanelObj.SetActive(false);
			}
		});

		#if false
		this.gameObject.transform.Find("btn_server").GetComponent<Button>().onClick.AddListener(() =>
		{
				new GameObject("server").AddComponent<WlanServer>();

			});

		this.gameObject.transform.Find("btn_client").GetComponent<Button>().onClick.AddListener(() =>
		{
				new GameObject("client").AddComponent<WlanClient>();

		});

		#endif
	}



	


	void Effect(GameObject obj)
	{

		Sequence seq = DOTween.Sequence();
		//#先快, 后慢(快落到底的时候), 然后快速回弹;
		seq.Append(obj.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0.0f, 13.0f), 1.8f).SetRelative())
			.Append(obj.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0.0f, -13.0f), 2.0f).SetRelative())
					.SetDelay(0.1f)
					.SetLoops(-1);  //翻转位置
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

	//页面重载入
	public override void Refresh()
	{
		startPanelObj.SetActive (true);
	}


	//秒定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);
		}
	}


	class Controller : BaseController<StartUIPage>, NetHttp.INetCallback
	{
		NetHttp m_netHttp;
		StartUIPage m_page;


		public Controller(StartUIPage iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_page = iview;
		}

		public void onDestroy()
		{
			m_netHttp.setPageNetCallback (null);
		}

		//用于标识是那个接口用于处理接受函数
		private const int REQ_THIRD_LOGIN = 1;

			
		public void reqThirdLogin(bool isRetry)
		{

			ReqThirdLogin paramsValObj;
			string checkID;

			string api = "/login";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdLogin> ();
				paramsValObj.m_isRetry = 1;

				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdLogin();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_type = 2;

				paramsValObj.m_account = m_page.m_uid;
				paramsValObj.m_password = m_page.m_passwd;


			}

			string url = SavedContext.getApiUrl(api);

			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_LOGIN, checkID);

		}
			
		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			switch (data.m_reqTag) {
			case REQ_THIRD_LOGIN:
				{
					RespThirdLogin resp = Utils.bytesToObject<RespThirdLogin> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							Debug.Log ("登录成功");
							if (null == SavedData.s_instance) {
								SavedData.s_instance = new SavedData ();
							}
							User user = SavedData.s_instance.m_user;
							user.m_uid = resp.m_uid; 
							user.m_token = resp.m_token; 
							//Debug.Log (resp.m_token);
							if (resp.m_isFirst == 1) {
								UIPage.ShowPage<CreateNameUIPage> ();
							} else {
								UIPage.ShowPage<LinkServerUIPage> ();
							}


						}
						break;
					default:
						{
							ValTableCache valCache = m_page.getValTableCache();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode>(m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow(valDict, resp.m_code);
							m_page.startPanelObj.SetActive(false);
							UIPage.ShowPage<LoginModeUIPage> ();
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
			m_page.toast.showToast ("onHttpErr");

		}

		public virtual void onOtherErr(DataNeedOnResponse data, int type)
		{
			Debug.Log (TAG +":" +"onOtherErr");
			m_page.toast.showToast ("onOtherErr");
		}
	}
}
