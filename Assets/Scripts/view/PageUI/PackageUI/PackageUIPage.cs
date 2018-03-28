using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;

public class PackageUIPage : UIPage
{
	private const string TAG = "PackageUIPage";
	
	private GameObject Item = null;
	private GameObject List = null;
	
	private List<UIPackageItem> Items = new List<UIPackageItem>();
	
	//当前item
	private UIPackageItem  currentItem = null;
	
	private TabControl tabControl = null;
	
	private List<TabIndex> tablist = new List<TabIndex>();

		


	public PackageUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/PackageUI/PackageUIMain";
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
		
		tabControl = this.transform.Find("tabcontrol").GetComponent<TabControl>() as TabControl;

		tablist.Add(new TabIndex(0, "魔法师", null));
		tablist.Add(new TabIndex(1, "魔宠", null));
		tablist.Add(new TabIndex(2, "技能", null));
		tablist.Add(new TabIndex(3, "喷漆", null));
		tablist.Add(new TabIndex(4, "魔法书", null));
		tablist.Add(new TabIndex(5, "天赋书", null));
		tablist.Add(new TabIndex(6, "魔法师", null));

		for(int i = 0; i<tablist.Count; i++)
		{
			tabControl.CreateTab(tablist[i].id, tablist[i].tabname, tablist[i].panelPath);
			initTab(i);
		}

		this.gameObject.transform.Find("btn_back").GetComponent<Button>().onClick.AddListener(() =>
		{
			// 返回
			ClosePage();
		});
	}


	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

	}


	private void initTab(int tab)
	{
		List = this.transform.Find("tabcontrol").gameObject;
		Item = this.transform.Find("tabcontrol/Panels/panel"+tab+"/Viewport/Content/item").gameObject;
		Item.SetActive(false);

		ValTableCache valCache = getValTableCache();
		Dictionary<int, ValStore> valDict = valCache.getValDictInPageScopeOrThrow<ValStore>(m_pageID, ConstsVal.val_store);

		for(int i = 0; i<valDict.Count; i++)
		{
			ValStore val = ValUtils.getValByKeyOrThrow(valDict, i);
			if(val.classify == tab)
				CreateItem(val);
		}

	}


	private void CreateItem(ValStore val)
	{
		GameObject go = GameObject.Instantiate(Item) as GameObject;
		go.transform.SetParent(Item.transform.parent);
		go.transform.localScale = Vector3.one;
		go.SetActive(true);
		
		//添加事件处理脚本
		UIPackageItem item = go.AddComponent<UIPackageItem>();
		item.Refresh(val);
		Items.Add(item);
		
		//添加按钮点击事件监听
		go.AddComponent<Button>().onClick.AddListener(OnClickItem);
	}

	private void OnClickItem()
	{
		UIPackageItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UIPackageItem>();
		UIPage.ShowPage<PublicUINotice>(item.data);
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

#if false


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;

public class StoreUIPage : UIPage
{
private const string TAG = "StoreUIPage";
private long AwadTime = 0;

private GameObject Item = null;
private GameObject List = null;


private List<UIStoreItem> Items = new List<UIStoreItem>();

//当前item	
private UIStoreItem currentItem = null;

private TabControl tabControl = null;

private List<TabIndex> tablist = new List<TabIndex>();



public StoreUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
{
//布局预制体
uiPath = "Prefabs/UI/StoreUI/StoreUIMain";
}
public override void Refresh()
{

}


//秒定时器
IEnumerator Timer() {
while (true) {
yield return new WaitForSeconds(1.0f);
//Refresh ();
}
}



public override void Awake(GameObject go)
{


tabControl = this.transform.Find("tabcontrol").GetComponent<TabControl>() as TabControl;

tablist.Add(new TabIndex(0, "推荐", null));	
tablist.Add(new TabIndex(1, "魔法石", null));	
tablist.Add(new TabIndex(2, "魔法币", null));	
tablist.Add(new TabIndex(3, "特卖", null));	

for(int i =0 ; i<tablist.Count; i++)
{
tabControl.CreateTab(tablist[i].id, tablist[i].tabname, tablist[i].panelPath);
}


checkTab(0);

this.gameObject.transform.Find("btn_back").GetComponent<Button>().onClick.AddListener(() =>
{
ClosePage();
});

this.gameObject.transform.Find("tabcontrol/Tabs/tab0").GetComponent<Button>().onClick.AddListener(() =>
{
checkTab(0);
});

this.gameObject.transform.Find("tabcontrol/Tabs/tab1").GetComponent<Button>().onClick.AddListener(() =>
{
Debug.Log("魔法石");
checkTab(1);
});

this.gameObject.transform.Find("tabcontrol/Tabs/tab2").GetComponent<Button>().onClick.AddListener(() =>
{
checkTab(2);
});

this.gameObject.transform.Find("tabcontrol/Tabs/tab3").GetComponent<Button>().onClick.AddListener(() =>
{
checkTab(3);
});

}




protected override void loadRes(TexCache texCache, ValTableCache valCache)
{
valCache.markPageUseOrThrow<ValGlobal>(m_pageID, ConstsVal.val_global);
valCache.markPageUseOrThrow<ValStore>(m_pageID, ConstsVal.val_store);
}

protected override void unloadRes(TexCache texCache, ValTableCache valCache)
{
valCache.unmarkPageUse(m_pageID, ConstsVal.val_global);
valCache.unmarkPageUse(m_pageID, ConstsVal.val_store);
}



private void checkTab(int tab)
{
List = this.transform.Find("tabcontrol").gameObject;
Item = this.transform.Find("tabcontrol/Panels/panel"+tab+"/Viewport/Content/item").gameObject;
Item.SetActive(false);

ValTableCache valCache = getValTableCache();
Dictionary<int, ValStore> valDict = valCache.getValDictInPageScopeOrThrow<ValStore>(m_pageID, ConstsVal.val_store);

for (int i = 1; i <= valDict.Count; i++) {
ValStore val = ValUtils.getValByKeyOrThrow(valDict, i);
//Debug.Log (""+val.classify);
if(val.classify == tab)
CreateItem(val);	
}

}


private void CreateItem(ValStore val)
{
Debug.Log ("CreateItem"+val.classify);
GameObject go = GameObject.Instantiate(Item) as GameObject;
go.transform.SetParent(Item.transform.parent);
go.transform.localScale = Vector3.one;
go.SetActive(true);

//添加事件处理脚本
UIStoreItem item = go.AddComponent<UIStoreItem>();
item.Refresh(val);
Items.Add(item);

//添加按钮点击事件监听
go.AddComponent<Button>().onClick.AddListener(OnClickItem);
}

//tablist item 点击事件
private void OnClickItem()
{
UIStoreItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UIStoreItem>();
Debug.Log (item.data.id);
UIPage.ShowPage<PublicUIPayPage>(item.data);
}


}

#endif
