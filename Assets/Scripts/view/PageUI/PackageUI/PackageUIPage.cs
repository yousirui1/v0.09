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

	private int itemId = 0;

	GameObject DescObj0 = null;
	GameObject DescObj1 = null;
	GameObject DescObj2 = null;
	GameObject DescObj3 = null;
	GameObject DescObj4 = null;
	GameObject DescObj5 = null;
	GameObject DescObj6 = null;




	public PackageUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/PackageUI/PackageUIMain";
	}
	public override void Refresh()
	{
		m_controller.reqThirdMage (false);

		//魔法石
		this.gameObject.transform.Find ("tx_stone").GetComponent<Text> ().text = "" + SavedData.s_instance.m_user.m_stone;
		//魔法币
		this.gameObject.transform.Find ("tx_gold").GetComponent<Text> ().text = "" +SavedData.s_instance.m_user.m_gold;
		//魔法圣杯
		this.gameObject.transform.Find ("tx_grail").GetComponent<Text> ().text = "" +SavedData.s_instance.m_user.m_grail;
	
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

		GameObject descObj = null;
		switch(tab)
		{
		case 0:
			{
				Debug.Log ("法师界面");
				//法师
				type = 3;
				DescObj0 = this.transform.Find ("tabcontrol/Panels/panel0/desc").gameObject;
				descObj = DescObj0;
			}
			break;

		case 1:
			{
				//魔宠
				type = 27;
				DescObj1 = this.transform.Find ("tabcontrol/Panels/panel1/desc").gameObject;
				descObj = DescObj1;
			}
			break;

		case 2:
			{
				//技能
				type = 7;
				DescObj2 = this.transform.Find ("tabcontrol/Panels/panel2/desc").gameObject;
				descObj = DescObj2;
			}
			break;
		case 3:
			{
				//喷漆
				type = 22;
				DescObj3 = this.transform.Find ("tabcontrol/Panels/panel3/desc").gameObject;
				descObj = DescObj3;
			}
			break;
		case 4:
			{
				//签名板
				type = 17;
				DescObj4 = this.transform.Find ("tabcontrol/Panels/panel4/desc").gameObject;
				descObj = DescObj4;

			}
			break;
		case 5:
			{
				//魔法书
				type = 4;
				DescObj5 = this.transform.Find ("tabcontrol/Panels/panel5/desc").gameObject;
				descObj = DescObj5;
			}
			break;

		case 6:
			{
				DescObj6 = this.transform.Find ("tabcontrol/Panels/panel6/desc").gameObject;  //天赋树没写
				DescObj6.SetActive (false);
				descObj = DescObj6;
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
		default:
			{
				descObj = DescObj0;
			}
			break;
		}

		if (tab != 6) {
			List = this.transform.Find("tabcontrol").gameObject;
			Item = this.transform.Find("tabcontrol/Panels/panel"+tab+"/Viewport/Content/item").gameObject;
			Item.SetActive(false);

			ValTableCache valCache = getValTableCache();
			Dictionary<int, ValEnchanter> valDict = valCache.getValDictInPageScopeOrThrow<ValEnchanter> (m_pageID, ConstsVal.val_enchanter);

			for(int i = 1; i<valDict.Count; i++)
			{
				ValEnchanter val = ValUtils.getValByKeyOrThrow(valDict, i);
				if (type == val.type) {
					CreateItem(val);
				}
			}
		}

		//监听按钮事件
		descObj.transform.Find("btn_ok").GetComponent<Button>().onClick.AddListener(() =>
		{
			m_controller.reqThirdSynthetize(false);
		});



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

		ValTableCache valCache = getValTableCache();
		//Debug.Log (val.sid);
		Dictionary<int, ValGlobal> valDict = valCache.getValDictInPageScopeOrThrow<ValGlobal> (m_pageID, ConstsVal.val_global);
		ValGlobal gval = ValUtils.getValByKeyOrThrow(valDict, val.sid);

		//添加事件处理脚本
		UIPackageItem item = go.AddComponent<UIPackageItem> ();
		item.Refresh(val, gval.icon);
		Items.Add(item);

		//添加按钮点击事件监听
		go.AddComponent<Button>().onClick.AddListener(OnClickItem);
	}

	private void OnClickItem()
	{
		Debug.Log ("OnClickItem");
		UIPackageItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UIPackageItem>();
		itemId = item.data.sid;
		RefreshDesc (item);
	}

	#if false
	//显示详细信息 交互动画
	private void ShowDesc(UIEmailItem item)
	{   
		currentItem = item;
		Desc.SetActive(true);
		Desc.transform.localPosition = new Vector3(300f, Desc.transform.localPosition.y,Desc.transform.localPosition.z);


		Desc.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 0.25f, true);
		RefreshDesc(item);
	}  
	#endif

	//更新详细信息
	private void RefreshDesc(UIPackageItem item)
	{   
		Debug.Log ("RefreshDesc");
		switch(item.data.type)
		{
		case 3:
			{
				//法师
				DescObj0.transform.Find ("tx_name").GetComponent<Text> ().text = "";
				DescObj0.transform.Find ("bg_progress/img_progress").GetComponent<Image> ().fillAmount = 0.1f;
				DescObj0.transform.Find ("bg_progress/tx_progress").GetComponent<Text> ().text = "100/100";
				DescObj0.transform.Find ("img_head").GetComponent<Image> ().sprite = ResourceMgr.Instance ().Load<Sprite> ("images/icon/"+item.icon, false);
				DescObj0.transform.Find ("img_desc/tx_desc").GetComponent<Text> ().text = item.data.text;
				DescObj0.transform.Find ("img_icon/tx_count").GetComponent<Text> ().text = "" + item.data.sum;
			}
			break;

		case 27:
			{
				//魔宠
				DescObj1.transform.Find ("tx_name").GetComponent<Text> ().text = "";
				DescObj1.transform.Find ("bg_progress/img_progress").GetComponent<Image> ().fillAmount = 0.1f;
				DescObj1.transform.Find ("bg_progress/tx_progress").GetComponent<Text> ().text = "100/100";
				DescObj1.transform.Find ("img_head").GetComponent<Image> ().sprite = ResourceMgr.Instance ().Load<Sprite> ("images/icon/"+item.icon, false);
				DescObj1.transform.Find ("img_desc/tx_desc").GetComponent<Text> ().text = item.data.text;
				DescObj1.transform.Find ("img_icon/tx_count").GetComponent<Text> ().text = "" + item.data.sum;
		
			}
			break;
	
		case 7:
			{
				//技能
				DescObj2.transform.Find ("tx_name").GetComponent<Text> ().text = "";
				DescObj2.transform.Find ("bg_progress/img_progress").GetComponent<Image> ().fillAmount = 0.1f;
				DescObj2.transform.Find ("bg_progress/tx_progress").GetComponent<Text> ().text = "100/100";
				DescObj2.transform.Find ("img_head").GetComponent<Image> ().sprite = ResourceMgr.Instance ().Load<Sprite> ("images/icon/"+item.icon, false);
				DescObj2.transform.Find ("img_desc/tx_desc").GetComponent<Text> ().text = item.data.text;
				DescObj2.transform.Find ("img_icon/tx_count").GetComponent<Text> ().text = "" + item.data.sum;
			}
			break;
		case 22:
			{
				//喷漆
				DescObj2.transform.Find ("bg_progress/img_progress").GetComponent<Image> ().fillAmount = 0.1f;
				DescObj2.transform.Find ("bg_progress/tx_progress").GetComponent<Text> ().text = "100/100";
				DescObj2.transform.Find ("img_head").GetComponent<Image> ().sprite = ResourceMgr.Instance ().Load<Sprite> ("images/icon/"+item.icon, false);
				DescObj2.transform.Find ("img_desc/tx_desc").GetComponent<Text> ().text = item.data.text;
				DescObj2.transform.Find ("img_icon/tx_count").GetComponent<Text> ().text = "" + item.data.sum;
			}
			break;
		case 17:
			{
				//签名板
				DescObj3.transform.Find ("img_head").GetComponent<Image> ().sprite = ResourceMgr.Instance ().Load<Sprite> ("images/icon/"+item.icon, false);
			}
			break;
		case 4:
			{
				//魔法书
				DescObj4.transform.Find ("tx_name").GetComponent<Text> ().text = "";
				DescObj4.transform.Find ("img_head").GetComponent<Image> ().sprite = ResourceMgr.Instance ().Load<Sprite> ("images/icon/"+item.icon, false);
				DescObj4.transform.Find ("img_desc/tx_desc").GetComponent<Text> ().text = item.data.text;
			}
			break;

		default:
			{
				//天赋树

			
			}
			break;
		}
	

	
	

	}





	class Controller : BaseController<PackageUIPage>,NetHttp.INetCallback
	{
		NetHttp m_netHttp;

		private MainLooper m_initedLooper;

		PackageUIPage m_page;
		public Controller(PackageUIPage iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_initedLooper = MainLooper.instance();
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

		//魔法师界面获取用户数据
		private const int REQ_THIRD_MAGE = 20;

		public void reqThirdMage(bool isRetry)
		{

			ReqThirdMage paramsValObj;
			string checkID;

			string api = "/mage";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdMage> ();
				paramsValObj.m_isRetry = 1;
				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdMage();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_MAGE, checkID);

		}


		//碎片合成
		private const int REQ_THIRD_SYNTHETIZE = 21;

		public void reqThirdSynthetize(bool isRetry)
		{

			ReqThirdSynthetize paramsValObj;
			string checkID;

			string api = "/synthetize";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdSynthetize> ();
				paramsValObj.m_isRetry = 1;

				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdSynthetize();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_gsid = m_page.itemId;
			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_SYNTHETIZE, checkID);

		}


		//装备物品
		private const int REQ_THIRD_EQUIPMENT = 22;

		public void reqThirdEquipment(bool isRetry)
		{

			ReqThirdEquipment paramsValObj;
			string checkID;

			string api = "/equipment";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdEquipment> ();
				paramsValObj.m_isRetry = 1;

				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdEquipment();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_gsid = m_page.itemId;
			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_EQUIPMENT, checkID);
		}


		//天赋点
		private const int REQ_THIRD_GENIUS = 23;

		public void reqThirdGenius(bool isRetry)
		{

			ReqThirdGenius paramsValObj;
			string checkID;

			string api = "/genius";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdGenius> ();
				paramsValObj.m_isRetry = 1;
				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdGenius();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_gsid = m_page.itemId;
			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_GENIUS, checkID);
		}




		//洗点
		private const int REQ_THIRD_WASH = 24;

		public void reqThirdWash(bool isRetry)
		{

			ReqThirdWash paramsValObj;
			string checkID;

			string api = "/wash";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdWash> ();
				paramsValObj.m_isRetry = 1;

				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdWash();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_WASH, checkID);
		}




		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			Debug.Log ("onHttpOk");
			switch (data.m_reqTag) {
			case REQ_THIRD_MAGE:
				{
					//获取用户数据
					RespThirdMage resp = Utils.bytesToObject<RespThirdMage> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							Debug.Log (resp.m_skin);
							Debug.Log (resp.m_pet);
							Debug.Log (resp.m_attackSkin);

						}
						break;

					default:
						{
							Debug.Log (resp.m_code);
							ValTableCache valCache = m_page.getValTableCache ();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode> (m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow (valDict, resp.m_code);
							Debug.Log (val.text);
						}
						break;



					}
				}
				break;


			case REQ_THIRD_SYNTHETIZE:
				{
					//碎片合成
					RespThirdSynthetize resp = Utils.bytesToObject<RespThirdSynthetize> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							Debug.Log ("碎片合成成功");

						}
						break;

					default:
						{
							Debug.Log (resp.m_code);
							ValTableCache valCache = m_page.getValTableCache ();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode> (m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow (valDict, resp.m_code);
							Debug.Log (val.text);
						}
						break;



					}
				}
				break;



			case REQ_THIRD_EQUIPMENT:
				{
					//装备
					RespThirdEquipment resp = Utils.bytesToObject<RespThirdEquipment> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							Debug.Log ("装备成功");
						}
						break;

					default:
						{
							Debug.Log (resp.m_code);
							ValTableCache valCache = m_page.getValTableCache ();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode> (m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow (valDict, resp.m_code);
							Debug.Log (val.text);
						}
						break;

					}
				}
				break;



			case REQ_THIRD_GENIUS:
				{
					//天赋点
					RespThirdGenius resp = Utils.bytesToObject<RespThirdGenius> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							Debug.Log ("天赋点加点成功");
						}
						break;

					default:
						{
							Debug.Log (resp.m_code);
							ValTableCache valCache = m_page.getValTableCache ();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode> (m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow (valDict, resp.m_code);
							Debug.Log (val.text);
						}
						break;

					}
				}
				break;


			case REQ_THIRD_WASH:
				{
					//洗点
					RespThirdWash resp = Utils.bytesToObject<RespThirdWash> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							Debug.Log ("洗点成功" +resp.m_talent);

						}
						break;

					default:
						{
							Debug.Log (resp.m_code);
							ValTableCache valCache = m_page.getValTableCache ();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode> (m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow (valDict, resp.m_code);
							Debug.Log (val.text);
							//m_page.toast.showToast (val.text);
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
