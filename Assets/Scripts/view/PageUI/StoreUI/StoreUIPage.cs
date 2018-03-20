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
			checkTab(i);
		}
	


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
