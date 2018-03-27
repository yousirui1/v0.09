using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using SimpleJson;
using tpgm.UI;
using tpgm;

public class CreateNameUIPage : UIPage
{
	private const string TAG = "CreateNameUIPage";

	Controller m_controller;



	private List<ValName> list;

	private string nickname ="";

	private bool isMan = true;

	public static string[] keyWords;

	Coroutine coroutine;

	public CreateNameUIPage() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
	{
		//布局预制体
		uiPath = "Prefabs/UI/LoginUI/CreateNameUIPage";

	}

	public override void Awake(GameObject go)
	{
		//定时器
		coroutine = UIRoot.Instance.StartCoroutine(Timer());

		m_controller = new Controller(this);

		toast.InitToast (this.gameObject);

		InitJsonFile ();
		this.gameObject.transform.Find("content/btn_random").GetComponent<Button>().onClick.AddListener(() =>
			{
				//随机姓名
				isMan = (this.gameObject.transform.Find ("content/Dropdown").GetComponent<Dropdown> ().value != 0 ? false : true); 

				if(isMan)
				nickname = list[new System.Random().Next(list.Count)].boy + list[new System.Random().Next(list.Count)].familyName;
				else
					nickname = list[new System.Random().Next(list.Count)].girl + list[new System.Random().Next(list.Count)].familyName;
				this.transform.Find("content/input_nickname").GetComponent<InputField>().text = nickname;
			});
		

	
		this.gameObject.transform.Find("content/btn_confim").GetComponent<Button>().onClick.AddListener(() =>
			{
				
				nickname = this.transform.Find("content/input_nickname").GetComponent<InputField>().text;
			
				bool isLegal = true;
				for(int i =0 ;i < keyWords.Length; i++)
				{
					if(keyWords[i].Contains(nickname))
					{
						isLegal = false;
						break;
					}
				}
				if(isLegal)
				{
					m_controller.reqThirdCreateName(false);
				}
				else
				{
					toast.showToast("名字不合法");
				}
			

			});

	}
	//秒定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);
			AtRefresh ();
		}
	}

	//页面重载入
	public override void Refresh()
	{

	}

	//定时刷新
	private void AtRefresh()
	{
		if (this.transform.Find("content/input_nickname").GetComponent<InputField>().text == "") {
			this.gameObject.transform.Find ("content/btn_confim").GetComponent<Button> ().enabled = false;
		} else {
			this.gameObject.transform.Find ("content/btn_confim").GetComponent<Button> ().enabled = true;
		}
	}


	void InitJsonFile()
	{

		string path_name = SavedContext.getExternalPath("data/" + "val_name.json");
		string text_name = File.ReadAllText(path_name, Encoding.UTF8);
		list = SimpleJson.SimpleJson.DeserializeObject<List<ValName>>(text_name);

		string path_hide = SavedContext.getExternalPath("data/" + "hide.json");
		string text_hide = File.ReadAllText(path_hide, Encoding.UTF8);
		keyWords = text_hide.Split('.');
	}

	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		//nickname
		//valCache.markPageUseOrThrow<ValCode>(m_pageID, ConstsVal.val_hide);
		//nickname
		//valCache.markPageUseOrThrow<ValName>(m_pageID, ConstsVal.val_name);
		//code
		valCache.markPageUseOrThrow<ValCode>(m_pageID, ConstsVal.val_code);
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{
		//nickname
		//valCache.unmarkPageUse(m_pageID, ConstsVal.val_code);
		//code
		valCache.unmarkPageUse(m_pageID, ConstsVal.val_code);
	}


	class Controller : BaseController<RegisterUIPage>,NetHttp.INetCallback
	{
		NetHttp m_netHttp;
		CreateNameUIPage m_page;

		public Controller(CreateNameUIPage iview):base(null)
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
		private const int REQ_THIRD_CREATENAME = 3;

		public void reqThirdCreateName(bool isRetry)
		{

			ReqThirdCreateName paramsValObj;
			string checkID;

			string api = "/createname";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdCreateName> ();
				paramsValObj.m_isRetry = 1;

				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID (api);
				paramsValObj = new ReqThirdCreateName ();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_name = m_page.nickname;
			}
			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_CREATENAME, checkID);
		}






		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			Debug.Log ("onHttpOk");
			switch (data.m_reqTag) {
			case REQ_THIRD_CREATENAME:
				{
					RespThirdCreateName resp = Utils.bytesToObject<RespThirdCreateName> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							Debug.Log ("" + resp.m_code);
							m_page.Hide ();
							UIPage.ShowPage<MainUIPage> ();
						}
						break;

					default:
						{
							Debug.Log ("" + resp.m_code);
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
			Debug.Log (TAG +":" +"onHttpErr");
		}

		public virtual void onOtherErr(DataNeedOnResponse data, int type)
		{
			Debug.Log (TAG +":" +"onOtherErr");

		}
	}

}
