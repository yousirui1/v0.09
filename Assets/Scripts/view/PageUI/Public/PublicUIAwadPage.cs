using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;

public class PublicUIAwadPage : UIPage
{
	private const string TAG = "ChatUIPage";

    public PublicUIAwadPage() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
    {
        //布局预制体
		uiPath = "Prefabs/UI/PublicUI/PublicUIAwadPage";

    }

    public override void Awake(GameObject go)
    {
		#if false
        ItemAwad awad = (ItemAwad)this.data;

        this.gameObject.transform.Find("content/img_awad0").GetComponent<Image>().sprite = ResourceMgr.Instance().Load<Sprite>("Public/Item/Awad/awad" + awad.id, false);

        this.gameObject.transform.Find("content/tx_awad0").GetComponent<Text>().text = ""+awad.num;

        ConectData.Instance.userInfo.gold += awad.num;
        //child.GetComponent<SpriteRenderer>().sprite =

		#endif
        this.gameObject.transform.Find("content/btn_confim").GetComponent<Button>().onClick.AddListener(() =>
        {
            // 隐藏
            Hide();
        });


    }




	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

	}

	class Controller : NetHttp.INetCallback
	{
		NetHttp m_netHttp;


		public Controller()
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);

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

		public void reqThirdLogin(bool isRetry,int type)
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
				paramsValObj.m_type = type;
				paramsValObj.m_mac = InfoUtil.GetMac();
				paramsValObj.m_account = GameObject.Find("input_user").GetComponent<InputField>().text;
				paramsValObj.m_password = GameObject.Find ("input_passwd").GetComponent<InputField> ().text;

				paramsValObj.m_password = Md5Util.GetMd5FromStr(paramsValObj.m_password);

				//保存数据
				PlayerPrefs.SetString("account", paramsValObj.m_account);
				PlayerPrefs.SetString("password", paramsValObj.m_password);
			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_LOGIN, checkID);

		}





		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			Debug.Log ("onHttpOk");
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
							//user.m_uid = resp.m_uid; 
							//user.m_token = resp.m_token; 


							LoginConect client = new GameObject ("Client").AddComponent<LoginConect> ();
							client.onLogin ();
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
