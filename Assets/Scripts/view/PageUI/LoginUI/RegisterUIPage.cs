using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;

public class RegisterUIPage : UIPage
{
	private const string TAG = "ChatUIPage";
	Controller m_controller;




    public RegisterUIPage() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
    {
        //布局预制体
		uiPath = "Prefabs/UI/LoginUI/RegisterUIPage";

    }

    public override void Awake(GameObject go)
    {

		m_controller = new Controller(this);

		InitToast ();

        this.gameObject.transform.Find("content/btn_register").GetComponent<Button>().onClick.AddListener(() =>
        {
         
				m_controller.reqThirdRegister(false);
				
        });

		this.gameObject.transform.Find("content/btn_close").GetComponent<Button>().onClick.AddListener(() =>
			{
				Hide();


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
		RegisterUIPage m_register;
	
		public Controller(RegisterUIPage iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_register = iview;
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
				paramsValObj.m_account = GameObject.Find ("content/input_user").GetComponent<InputField> ().text;
				paramsValObj.m_password = GameObject.Find ("content/input_passwd1").GetComponent<InputField> ().text;

				string password2 = GameObject.Find ("content/input_passwd2").GetComponent<InputField> ().text;

				if (paramsValObj.m_password != password2) {
					m_register.toast.showToast ("2次输入的密码不一致");
				
				} 
				else if (password2.Length <6 || password2.Length > 12 ) {
					m_register.toast.showToast ("密码长度不合法");
				}else {
					//md5加密
					paramsValObj.m_password = Md5Util.GetMd5FromStr (paramsValObj.m_password);
					//保存数据
					PlayerPrefs.SetString ("account", paramsValObj.m_account);
					PlayerPrefs.SetString ("password", paramsValObj.m_password);
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
							m_register.Hide ();
							UIPage.ShowPage<LoginUIPage> ();
						}
						break;

					default:
						{
							ValTableCache valCache = m_register.getValTableCache();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode>(m_register.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow(valDict, resp.m_code);
							//UIPage.ShowPage<PublicUINotice> (val.text);
							m_register.toast.showToast (val.text);
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
