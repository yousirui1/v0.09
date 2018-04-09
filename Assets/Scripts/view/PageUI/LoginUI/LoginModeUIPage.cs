using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using DG.Tweening;
using System.IO;
using System.Text;
using tpgm;


public class LoginModeUIPage : UIPage
{
	private const string TAG = "LoginModeUIPage";
	//http控制
	Controller m_controller;

	Coroutine coroutine;

	public LoginModeUIPage() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
	{
		//布局预制体
		uiPath = "Prefabs/UI/LoginUI/LoginModeUIPage";

	}

	public override void Awake(GameObject go)
	{

		m_controller = new Controller (this);

		this.transform.Find("content/btn_maclogin").GetComponent<Button>().onClick.AddListener(() =>
		{
			m_controller.reqThirdLogin (false);
			
		});

		this.transform.Find("content/btn_uidlogin").GetComponent<Button>().onClick.AddListener(() =>
		{
			Hide();
			UIPage.ShowPage<LoginUIPage>();
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

	}
		
	//秒定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);
		}
	}




	class Controller : BaseController<LoginModeUIPage>, NetHttp.INetCallback
	{
		NetHttp m_netHttp;
		LoginModeUIPage m_page;


		public Controller(LoginModeUIPage iview):base(null)
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
				paramsValObj.m_type = 1;
				paramsValObj.m_mac = InfoUtil.GetMac();

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
							if (null == SavedData.s_instance) {
								SavedData.s_instance = new SavedData ();
							}
							User user = SavedData.s_instance.m_user;
							user.m_uid = resp.m_uid; 
							user.m_token = resp.m_token; 
							Debug.Log (resp.m_token);
							if (resp.m_isFirst == 1) {
								UIPage.ShowPage<CreateNameUIPage> ();
							}
							UIPage.ShowPage<LinkServerUIPage> ();
							m_page.Hide();
						}
						break;
					default:
						{
							ValTableCache valCache = m_page.getValTableCache();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode>(m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow(valDict, resp.m_code);

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
