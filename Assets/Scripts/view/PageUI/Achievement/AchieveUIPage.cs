using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;

/**************************************
*FileName: AchievementUIPage.cs
*User: ysr 
*Data: 2018/2/5
*Describe: 成就界面
**************************************/

public class AchievementUIPage : UIPage
{
	private const string TAG = "UIAchievementItem";
	
	private GameObject Item = null;
	
	private List<UIAchievementItem> Items = new List<UIAchievementItem>();
	
	//当前item
	private UIAchievementItem  currentItem = null;
	
	private TabControl tabControl = null;
	
	private List<TabIndex> tablist = new List<TabIndex>();

		


	public AchievementUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/AchieveUI/AchieveUIPage";
	}
	public override void Refresh()
	{
		
	}



	//秒定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);
			Refresh ();
		}
	}



	public override void Awake(GameObject go)
	{
		
		init ();
		this.gameObject.transform.Find("btn_back").GetComponent<Button>().onClick.AddListener(() =>
		{
			// 返回
			ClosePage();
		});
	}


	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		valCache.markPageUseOrThrow<ValGlobal>(m_pageID, ConstsVal.val_global);
		valCache.markPageUseOrThrow<ValAchieve>(m_pageID, ConstsVal.val_achieve);
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{
		valCache.unmarkPageUse(m_pageID, ConstsVal.val_global);
		valCache.unmarkPageUse(m_pageID, ConstsVal.val_achieve);
	}


	private void init()
	{
		Item = this.transform.Find("bg_achieve/panel/Viewport/Content/item").gameObject;
		Item.SetActive(false);

		ValTableCache valCache = getValTableCache();
		Dictionary<int, ValAchieve> valDict = valCache.getValDictInPageScopeOrThrow<ValAchieve>(m_pageID, ConstsVal.val_achieve);
		for (int i = 1; i <= valDict.Count; i++) {
			ValAchieve val = ValUtils.getValByKeyOrThrow(valDict, i);
			CreateItem(val);	
		}
	}




	private void CreateItem(ValAchieve val)
	{
		GameObject go = GameObject.Instantiate(Item) as GameObject;
		go.transform.SetParent(Item.transform.parent);
		go.transform.localScale = Vector3.one;
		go.SetActive(true);
		
		//添加事件处理脚本
		UIAchievementItem item = go.AddComponent<UIAchievementItem>();
		item.Refresh(val);
		Items.Add(item);
		
		//添加按钮点击事件监听
		go.AddComponent<Button>().onClick.AddListener(OnClickItem);
	}


	private void OnClickItem()
	{
		UIAchievementItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UIAchievementItem>();
		//UIPage.ShowPage<>(item.data);
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
							#if false
							User user = SavedData.s_instance.m_user;
							user.m_uid = resp.m_uid; 
							user.m_token = resp.m_token; 
							LoginConect client = new GameObject ("Client").AddComponent<LoginConect> ();
							client.onLogin ();
							#endif
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
