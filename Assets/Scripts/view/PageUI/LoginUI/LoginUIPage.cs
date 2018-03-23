using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using DG.Tweening;
using tpgm;


public class LoginUIPage : UIPage
{
	private const string TAG = "LoginUIPage";

	//http控制
	Controller m_controller;
	//登录方式 1 mac 2账号
	private int m_login_type = 0;

	private string m_uid = "";
	private string m_passwd = "";

	Coroutine coroutine;

	GameObject toastObj;

	public LoginUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/LoginUI/LoginUIPage";

	}

	public override void Awake(GameObject go)
	{
		//InitToast ();
	
		//定时器
		coroutine = UIRoot.Instance.StartCoroutine(Timer());

		toastObj = this.transform.Find("toast").gameObject;
		toastObj.SetActive (false);

		m_controller = new Controller(this);

		this.gameObject.transform.Find("btn_register").GetComponent<Button>().onClick.AddListener(() =>
			{
				//UIRoot.Instance.StopCoroutine(coroutine);
				// 注册
				UIPage.ShowPage<RegisterUIPage>();
			});

		this.gameObject.transform.Find("btn_login").GetComponent<Button>().onClick.AddListener(() =>
			{
				m_uid = this.transform.Find("input_user").GetComponent<InputField>().text;
				m_passwd = this.transform.Find ("input_passwd").GetComponent<InputField> ().text;
				m_login_type = 2;
				m_controller.reqThirdLogin (false);
			});

		this.gameObject.transform.Find("btn_maclogin").GetComponent<Button>().onClick.AddListener(() =>
			{
				m_login_type = 1;
				m_controller.reqThirdLogin (false);

			});

		//创建局域网服务器
		this.gameObject.transform.Find("btn_wlanserver").GetComponent<Button>().onClick.AddListener(() =>
			{
				SavedData.s_instance.m_mode = 2;
				Application.LoadLevel("Game");
			});

		//局域网客户端
		this.gameObject.transform.Find("btn_wlanclient").GetComponent<Button>().onClick.AddListener(() =>
			{
				SavedData.s_instance.m_mode = 3;
				Application.LoadLevel("Game");
			});




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
		//定时器
		//coroutine = UIRoot.Instance.StartCoroutine(Timer());
	}

	int count = 0;
	//秒定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);
		}
	}




	class Controller : BaseController<LoginUIPage>, NetHttp.INetCallback
	{
		NetHttp m_netHttp;
		LoginUIPage m_page;
	

		public Controller(LoginUIPage iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_page = iview;
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
				paramsValObj.m_type = m_page.m_login_type;
				paramsValObj.m_mac = InfoUtil.GetMac();
				paramsValObj.m_account = m_page.m_uid;
				paramsValObj.m_password = m_page.m_passwd;

				paramsValObj.m_password = Md5Util.GetMd5FromStr(paramsValObj.m_password);


				//保存数据
				PlayerPrefs.SetString("account", paramsValObj.m_account);
				PlayerPrefs.SetString("password", paramsValObj.m_password);
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
							//m_page.showToast ("登录成功");
							if (null == SavedData.s_instance) {
								SavedData.s_instance = new SavedData ();
							}
							User user = SavedData.s_instance.m_user;
							user.m_uid = resp.m_uid; 
							user.m_token = resp.m_token; 
							if (resp.m_isFirst == 1) {
								UIPage.ShowPage<CreateNameUIPage> ();
							}
							UIPage.ShowPage<LinkServerUIPage> ();


						}
						break;
					default:
						{
							ValTableCache valCache = m_page.getValTableCache();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode>(m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow(valDict, resp.m_code);
							//m_page.showToast (val.text);
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
	