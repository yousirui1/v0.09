using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;

public class RegisterUIPage : UIPage
{
	private const string TAG = "RegisterUIPage";
	Controller m_controller;

	private string m_uid = "";
	private string m_passwd = "";
	private string m_passwd2 = "";

    public RegisterUIPage() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
    {
        //布局预制体
		uiPath = "Prefabs/UI/LoginUI/RegisterUIPage";

    }

    public override void Awake(GameObject go)
    {

		m_controller = new Controller(this);

		toast.InitToast (this.gameObject);

        this.gameObject.transform.Find("content/btn_register").GetComponent<Button>().onClick.AddListener(() =>
        {
				m_uid = this.transform.Find("content/input_uid").GetComponent<InputField>().text;
				m_passwd = this.transform.Find ("content/input_passwd").GetComponent<InputField> ().text;
				m_passwd2 = this.transform.Find ("content/input_passwd2").GetComponent<InputField> ().text;
				m_controller.reqThirdRegister(false);
        });

		this.gameObject.transform.Find("content/btn_back").GetComponent<Button>().onClick.AddListener(() =>
		{
				UIPage.ShowPage<LoginUIPage>();
			Hide();
		});

		this.gameObject.transform.Find("content/btn_close").GetComponent<Button>().onClick.AddListener(() =>
		{
			Hide();
				UIPage.ShowPage<StartUIPage>();
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
		

	class Controller : BaseController<RegisterUIPage>,NetHttp.INetCallback
	{
		NetHttp m_netHttp;
		RegisterUIPage m_page;
	
		public Controller(RegisterUIPage iview):base(null)
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
		private const int REQ_THIRD_REGISTER = 2;

		public void reqThirdRegister(bool isRetry)
		{
			ReqThirdRegister paramsValObj;
			string checkID;

			string api = "/register";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdRegister> ();
				paramsValObj.m_isRetry = 1;

				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID (api);
				paramsValObj = new ReqThirdRegister ();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;   
				paramsValObj.m_type = 1;   	//1：注册，2：绑定mac地址
				paramsValObj.m_mac = InfoUtil.GetMac ();
				paramsValObj.m_account = m_page.m_uid;
				paramsValObj.m_password = m_page.m_passwd;

				string password2 = m_page.m_passwd2;

				if (paramsValObj.m_password != password2) {
					m_page.toast.showToast ("2次输入的密码不一致");
				} 
				else if (password2.Length < 6 || password2.Length > 12 ) {
					m_page.toast.showToast ("密码不符合规则");

				}else {
					//md5加密
					paramsValObj.m_password = Md5Util.GetMd5FromStr (paramsValObj.m_password);

					//保存数据
					PrefValSet.saveUid (paramsValObj.m_account);
					PrefValSet.savePasswd (paramsValObj.m_password);
				}
			}
			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_REGISTER, checkID);
					
		}



		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			Debug.Log ("onHttpOk");
			switch (data.m_reqTag) {
			case REQ_THIRD_REGISTER:
				{
					RespThirdRegister resp = Utils.bytesToObject<RespThirdRegister> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							m_page.Hide ();
							UIPage.ShowPage<LoginUIPage> ();
						}
						break;

					default:
						{
							ValTableCache valCache = m_page.getValTableCache();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode>(m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow(valDict, resp.m_code);
							m_page.toast.showToast (val.text);
						}
						break;
					}
				}
				break;
			}

		}

		public virtual void onHttpErr(DataNeedOnResponse data, int statusCode, string errMsg)
		{
			//Debug.Log (TAG +":" +"onHttpErr");
			m_page.toast.showToast ("onHttpErr");
		}

		public virtual void onOtherErr(DataNeedOnResponse data, int type)
		{
			//Debug.Log (TAG +":" +"onOtherErr");
			m_page.toast.showToast ("onOtherErr");

		}
	}

}
