using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;
using DG.Tweening;

/**************************************
*FileName: PublicUIActive.cs
*User: ysr 
*Data: 2018/1/16
*Describe: 活动页面 部分为web
**************************************/
public class PublicUIActivityPage : UIPage
{
	private const string TAG = "PublicUIActivityPage";

	private GameObject mItem = null;
	private GameObject mList = null;
	private GameObject mWeb = null;

	private WebView webView = null;

	private List<UIActivityItem> friendItems = new List<UIActivityItem>();

	//当前item
	private UIActivityItem currentItem = null;	

	private TabControl tabControl = null;

	private List<TabIndex> tablist = new  List<TabIndex> ();

	public PublicUIActivityPage() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
	{
		//布局预制体
		uiPath = "Prefabs/UI/PublicUI/PublicUIActivity";

	}

	public override void Awake(GameObject go)
	{


		tabControl = this.transform.Find ("content/tabcontrol").GetComponent<TabControl> () as TabControl;

		tablist.Add (new TabIndex(0,"好友",null));
		tablist.Add (new TabIndex(1,"最近",null));


		for (int i = 0; i < tablist.Count; i++) {
			tabControl.CreateTab (tablist[i].id, tablist[i].tabname, tablist[i].panelPath);
		}


		mList = this.transform.Find("content/tabcontrol/Panels/panel0").gameObject;

		mItem = this.transform.Find("content/tabcontrol/Panels/panel0/Viewport/Content/item").gameObject;
		mItem.SetActive(false);

		//web页面
		mWeb = this.transform.Find("content/web").gameObject;

		webView = mWeb.gameObject.AddComponent<WebView> ();
	

		//mDesc.transform.Find("btn_upgrade").GetComponent<Button>().onClick.AddListener(OnClickUpgrade);

		this.gameObject.transform.Find("content/btn_back").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 销毁web
				//webView.Destory ();
				//this.gameObject.Destroy(mWeb.GetComponent("WebView"));
				webView.isShow (false);
				Hide();
			});

	}
	//更新
	public override void Refresh()
	{
		mList.transform.localScale = Vector3.zero;	
		mList.transform.DOScale(new Vector3(1, 1, 1), 0.5f);

		//Get Friend Data
		//查看前一个页面有没有传入参data没有则初始化GameData	
		//UDFriend.friends friendData = this.data != null ? this.data as UDFriend : GameData.Instance.playerFriend;


		UDActivity activityData = ActivityTestData.Instance.playerActivity;


		if (activityData == null) {
			//ConectData.Instance.friedns = GameData.Instance.playerFriend.friends;
		} else {
			for (int i = 0; i < activityData.activitys.Count; i++) {
				CreateFriendItem (activityData.activitys[i]);
			}
		}

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

	#region this logic

	private void CreateFriendItem(UDActivity.Activity activity)
	{
		GameObject go = GameObject.Instantiate(mItem) as GameObject;
		go.transform.SetParent(mItem.transform.parent);
		go.transform.localScale = Vector3.one;
		go.SetActive(true);

		UIActivityItem item = go.AddComponent<UIActivityItem>();
		item.Refresh(activity);
		friendItems.Add(item);	

		//add click btn listener
		go.AddComponent<Button>().onClick.AddListener(OnClickFriendItem);

	}

	//邀请好友
	private void OnClickFriendItem()
	{
		//获取好友信息
		UIActivityItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UIActivityItem>();

		ShowDesc(item);
		Debug.Log (item.data.url);
	}

	//显示页面
	private void ShowDesc(UIActivityItem item)
	{
		currentItem = item;
		mWeb.transform.localPosition = new Vector3(300f, mWeb.transform.localPosition.y, mWeb.transform.localPosition.z);
		mWeb.GetComponent<RectTransform>().DOAnchorPos(new Vector2(184.5f, -48f), 0.25f, true);
		RefreshDesc(item);
	}

	//根据点击事件更新web
	private void RefreshDesc(UIActivityItem item)
	{
		webView.isShow (true);
		webView.SetMargins (440, 180, 70, 80);
		webView.LoadUrl(item.data.url);
	}

	//按键之后更新
	private void OnClickUpgrade()
	{
		/*currentItem.data.level++;
		currentItem.Refresh(currentItem.data);
		RefreshDesc(currentItem);*/
	}

	#endregion





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
