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

		
	Controller m_controller;



	public PackageUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/PackageUI/PackageUIMain";
	}
	public override void Refresh()
	{
		
		this.gameObject.transform.Find("btn_close").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 返回
				ClosePage();
			});

		this.gameObject.transform.Find("btn_close").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 返回
				ClosePage();
			});

		this.gameObject.transform.Find("btn_close").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 返回
				ClosePage();
			});
				
	}



	//秒定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);
		}
	}



	public override void Awake(GameObject go)
	{

		m_controller = new Controller (this);
		
		tabControl = this.transform.Find("tabcontrol").GetComponent<TabControl>() as TabControl;

		tablist.Add(new TabIndex(0, "魔法师", Paths.package_panel));
		tablist.Add(new TabIndex(1, "魔宠", Paths.package_panel));
		tablist.Add(new TabIndex(2, "技能", Paths.package_panel));
		tablist.Add(new TabIndex(3, "喷漆", Paths.package_panel));
		tablist.Add(new TabIndex(4, "签名", Paths.package_panel));
		tablist.Add(new TabIndex(5, "魔法书", Paths.package_panel));
		tablist.Add(new TabIndex(6, "天赋树", Paths.package_panel));


		for(int i = 0; i<tablist.Count; i++)
		{
			tabControl.CreateTab(tablist[i].id, tablist[i].tabname, tablist[i].panelPath);
			initTab(i);
		}

		this.gameObject.transform.Find("btn_close").GetComponent<Button>().onClick.AddListener(() =>
		{
			// 返回
			ClosePage();
		});

		this.gameObject.transform.Find("btn_help").GetComponent<Button>().onClick.AddListener(() =>
		{
			// 帮助
			//ClosePage();
		});
	}


	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		//code
		valCache.markPageUseOrThrow<ValCode>(m_pageID, ConstsVal.val_code);

		valCache.markPageUseOrThrow<ValEnchanter>(m_pageID, ConstsVal.val_enchanter);

		//valCache.markPageUseOrThrow<ValCode>(m_pageID, ConstsVal.val_talent);

		valCache.markPageUseOrThrow<ValGlobal>(m_pageID, ConstsVal.val_global);
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{
		//code
		valCache.unmarkPageUse(m_pageID, ConstsVal.val_code);

		valCache.unmarkPageUse(m_pageID, ConstsVal.val_enchanter);

		//valCache.unmarkPageUse(m_pageID, ConstsVal.val_talent);

		valCache.unmarkPageUse(m_pageID, ConstsVal.val_global);
	}


	//chushi
	private void initTab(int tab)
	{
		int type = 0;

		switch(tab)
		{
		case 0:
			{
				//法师
				type = 3;
			}
			break;

		case 1:
			{
				//魔宠
				type = 27;
			}
			break;

		case 2:
			{
				//技能
				type = 7;
			}
			break;
		case 3:
			{
				//喷漆
				type = 22;
			}
			break;
		case 4:
			{
				//签名板
				type = 17;
			}
			break;
		case 5:
			{
				//魔法书
				type = 4;
			}
			break;

		case 6:
			{
				List<TabIndex> skilltree_tablist = new List<TabIndex>();
				//天赋树特别
				TabControl skilltree_tabControl =  this.transform.Find("tabcontrol/Panels/panel6/tabcontrol").GetComponent<TabControl>() as TabControl;
				skilltree_tablist.Add(new TabIndex(0, "攻击天赋", Paths.package_skilltree_panel));
				skilltree_tablist.Add(new TabIndex(1, "防御天赋", Paths.package_skilltree_panel));
				skilltree_tablist.Add(new TabIndex(2, "敏捷天赋", Paths.package_skilltree_panel));
				skilltree_tablist.Add(new TabIndex(3, "道具天赋", Paths.package_skilltree_panel));

				for(int i = 0; i<skilltree_tablist.Count; i++)
				{
					skilltree_tabControl.CreateTab(skilltree_tablist[i].id, skilltree_tablist[i].tabname, skilltree_tablist[i].panelPath);
					initSkillTab(i);
				}
			}
			break;

		}

		if (tab != 6) {
			List = this.transform.Find("tabcontrol").gameObject;
			Item = this.transform.Find("tabcontrol/Panels/panel"+tab+"/Viewport/Content/item").gameObject;
			Item.SetActive(false);

			//UDPackage data = new UDPackage (1,"icon_100191" ,tab);

			ValTableCache valCache = getValTableCache();
			//Dictionary<int, ValEnchanter> valDict = valCache.getValDictInPageScopeOrThrow<ValEnchanter>(m_pageID, ConstsVal.val_enchanter);
			Dictionary<int, ValEnchanter> valDict = valCache.getValDictInPageScopeOrThrow<ValEnchanter> (m_pageID, ConstsVal.val_enchanter);

			for(int i = 1; i<valDict.Count; i++)
			{
				ValEnchanter val = ValUtils.getValByKeyOrThrow(valDict, i);
				if(type == val.type)
				CreateItem(val);
			}
		

			Debug.Log (valDict.Count);
		}

	}




	void initSkillTab(int tab)
	{
		GameObject btn_groupObj = this.transform.Find ("tabcontrol/Panels/panel6/tabcontrol/Panels/panel" + tab + "/btn_group").gameObject;
		for (int i = 0; i < btn_groupObj.transform.childCount; i++) {
			btn_groupObj.transform.GetChild(i).GetComponent<Button>().onClick.AddListener (delegate {
				//OnClickSkillTree(btn_groupObj.transform.GetChild(i).gameObject,btn_groupObj);
			});
		}

	}

	private void OnClickSkillTree(GameObject obj,GameObject panelObj)
	{
		//panelObj.
	}

	private void CreateItem(ValEnchanter val)
	{

		GameObject go = GameObject.Instantiate(Item) as GameObject;
		go.transform.SetParent(Item.transform.parent);
		go.transform.localScale = Vector3.one;
		go.SetActive(true);

		//添加事件处理脚本
		UIPackageItem item = go.AddComponent<UIPackageItem> ();
		item.Refresh(val);
		Items.Add(item);

		//添加按钮点击事件监听
		go.GetComponent<Button>().onClick.AddListener (delegate {
			OnClickItem (go);
		});

	}

	private void OnClickItem(GameObject go)
	{
		#if false
		go.GetComponent<UIPackageItem>("").GetComponent<Button>().onClick.AddListener(() =>
		{
				// 合成按钮
			m_controller.reqThirdLogin (false, go.GetComponent<UIPackageItem>().data.sid);
		});
		#endif
		//修改右边显示的点击状态

		//Show()

	}

	class Controller : BaseController<PackageUIPage>,NetHttp.INetCallback
	{
		NetHttp m_netHttp;

		private MainLooper m_initedLooper;

		PackageUIPage m_main;
		public Controller(PackageUIPage iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_initedLooper = MainLooper.instance();
			m_main = iview;
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

		public void reqThirdLogin(bool isRetry,string sid)
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
