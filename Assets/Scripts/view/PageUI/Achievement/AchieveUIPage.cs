using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using tpgm.UI;
using tpgm;
using LitJson;
using System.Runtime.Serialization;

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

	private  GameObject Desc  =null;
	
	private List<UIAchievementItem> Items = new List<UIAchievementItem>();
	
	//当前item
	private UIAchievementItem  currentItem = null;
	
	private TabControl tabControl = null;
	
	private List<TabIndex> tablist = new List<TabIndex>();

	private int achievement_id = 0;

	private List<JsonAchieve> list_achieve; 

	Controller m_controller;
	public AchievementUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/AchieveUI/AchieveUIPage";
	}
	public override void Refresh()
	{
		m_controller.reqThirdAchieve (false);

		this.gameObject.transform.Find ("cp/tx_cp").GetComponent<Text> ().text = "" + SavedData.s_instance.m_user.m_talent;
	}



	//秒定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);
		}
	}



	public override void Awake(GameObject go)
	{
		
		init ();

		m_controller = new Controller (this);

		this.gameObject.transform.Find("desc/btn_get").GetComponent<Button>().onClick.AddListener(() =>
		{
				m_controller.reqThirdAchieveWard(false);
		});

		this.gameObject.transform.Find("desc/btn_next").GetComponent<Button>().onClick.AddListener(() =>
		{
				RefreshDesc(true);
		});

		this.gameObject.transform.Find("desc/btn_front").GetComponent<Button>().onClick.AddListener(() =>
		{
				RefreshDesc(false);
		});
		

		this.gameObject.transform.Find("btn_close").GetComponent<Button>().onClick.AddListener(() =>
		{
			// 返回
			ClosePage();
		});
	}


	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		valCache.markPageUseOrThrow<ValGlobal>(m_pageID, ConstsVal.val_global);
		valCache.markPageUseOrThrow<ValAchieve>(m_pageID, ConstsVal.val_achieve);
		//code
		valCache.markPageUseOrThrow<ValCode>(m_pageID, ConstsVal.val_code);
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{
		valCache.unmarkPageUse(m_pageID, ConstsVal.val_global);
		valCache.unmarkPageUse(m_pageID, ConstsVal.val_achieve);
		valCache.unmarkPageUse(m_pageID, ConstsVal.val_code);
	}


	private void init()
	{
		int type = 0;

		Desc = this.transform.Find("desc").gameObject;

		Item = this.transform.Find("bg_panel/panel/Viewport/Content/item").gameObject;
		Item.SetActive(false);

		ValTableCache valCache = getValTableCache();
		Dictionary<int, ValAchieve> valDict = valCache.getValDictInPageScopeOrThrow<ValAchieve>(m_pageID, ConstsVal.val_achieve);
		for (int i = 1; i <= valDict.Count; i++) {
			
			ValAchieve val = ValUtils.getValByKeyOrThrow(valDict, i);

			if (type != val.type) {
				type = val.type;
				CreateItem(val);	
			}

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
		//当前点击的id
		achievement_id = item.data.id;
		ShowDesc (item);
	}

	private void ShowDesc(UIAchievementItem item)
	{
		currentItem = item;
		//Desc.SetActive(true);  //-520 72
		Desc.transform.localPosition = new Vector3(500f, Desc.transform.localPosition.y,Desc.transform.localPosition.z);
	
		Desc.GetComponent<RectTransform>().DOAnchorPos(new Vector2(426,2f), 0.25f, true);
		RefreshDesc(item);

	}

	private void RefreshDesc(UIAchievementItem item)
	{
		Desc.transform.Find ("bg_progress/tx_progress").GetComponent<Text> ().text = "0" + "/"  + item.data.count;

		Desc.transform.Find ("bg_progress/img_progress").GetComponent<Image> ().fillAmount = 0.0f;

		Desc.transform.Find ("img_desc").GetComponent<Image> ().sprite = ResourceMgr.Instance().Load<Sprite>("images/ui/icon/"+item.data.icon, false);
		Desc.transform.Find ("tx_title").GetComponent<Text> ().text = item.data.name;

		Desc.transform.Find ("bg_state/tx_condition").GetComponent<Text> ().text =""+ item.data.condition;
		Desc.transform.Find ("bg_state/tx_award").GetComponent<Text> ().text =""+ item.data.reward;


		for (int i = 0; i < list_achieve.Count; i++) {
			if(list_achieve [i].type == item.data.type)
			{
				if (list_achieve [i].finish != null) {
					string[] arryfinish =  list_achieve [i].finish.Split(',');
					achievement_id = Convert.ToInt32(arryfinish [arryfinish.Length - 1]) + 1;

					ValTableCache valCache = getValTableCache();
					Dictionary<int, ValAchieve> valDict = valCache.getValDictInPageScopeOrThrow<ValAchieve>(m_pageID, ConstsVal.val_achieve);
					ValAchieve val = ValUtils.getValByKeyOrThrow(valDict, achievement_id);

					Desc.transform.Find ("tx_title").GetComponent<Text> ().text = val.name;

					Desc.transform.Find ("bg_state/tx_condition").GetComponent<Text> ().text =""+ val.condition;
					Desc.transform.Find ("bg_state/tx_award").GetComponent<Text> ().text =""+ val.reward;
					Desc.transform.Find ("bg_progress/tx_progress").GetComponent<Text> ().text = "" + list_achieve [i].hold + "/"  + val.count;
				} 
				Desc.transform.Find ("bg_progress/tx_progress").GetComponent<Text> ().text = "" + list_achieve [i].hold + "/"  + item.data.count;
				if (list_achieve [i].hold != 0) {
					Desc.transform.Find ("bg_progress/img_progress").GetComponent<Image> ().fillAmount = (float)list_achieve [i].hold / (float)item.data.count;
					Debug.Log ((float)list_achieve [i].hold / (float)item.data.count);
				} else {
					Desc.transform.Find ("bg_progress/img_progress").GetComponent<Image> ().fillAmount = 0.0f;
				}
				break;
			}

		}

	}

	private void RefreshDesc(bool isNext)
	{
		ValTableCache valCache = getValTableCache();
		Dictionary<int, ValAchieve> valDict = valCache.getValDictInPageScopeOrThrow<ValAchieve>(m_pageID, ConstsVal.val_achieve);


		if(isNext)
		{
			if ((achievement_id +1) > valDict.Count) {
				achievement_id = valDict.Count + 1;
			} else {
				achievement_id++;
			}

		}
		else
		{
			if ((achievement_id-1) < 1) {
				achievement_id = 1;
			} else {
				achievement_id--;
			}

		}

		ValAchieve val = ValUtils.getValByKeyOrThrow(valDict, achievement_id);

		Desc.transform.Find ("tx_title").GetComponent<Text> ().text = val.name;

		Desc.transform.Find ("bg_state/tx_condition").GetComponent<Text> ().text =""+ val.condition;
		Desc.transform.Find ("bg_state/tx_award").GetComponent<Text> ().text =""+ val.reward;

		for (int i = 0; i < list_achieve.Count; i++) {
			if (list_achieve [i].type == val.type) {
				Desc.transform.Find ("bg_progress/tx_progress").GetComponent<Text> ().text = "" + list_achieve [i].hold + "/" + val.count;
			} else {
				Desc.transform.Find ("bg_progress/tx_progress").GetComponent<Text> ().text = "0"  + "/" + val.count;
			}
		}

	}

	class Controller : BaseController<AchievementUIPage>,NetHttp.INetCallback
	{
		NetHttp m_netHttp;


		AchievementUIPage m_page;

		public Controller(AchievementUIPage iview):base(null)
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
		private const int REQ_THIRD_ACHIEVE = 26;

		public void reqThirdAchieve(bool isRetry)
		{

			ReqThirdAchieve paramsValObj;
			string checkID;
			string api = "/achieve";

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdAchieve> ();
				paramsValObj.m_isRetry = 1;

				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdAchieve();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
			}

			string url = SavedContext.getApiUrl(api);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_ACHIEVE, checkID);

		}

		private const int REQ_THIRD_ACHIEVEWARD = 27;

		//领取奖励
		public void reqThirdAchieveWard(bool isRetry)
		{

			ReqThirdAchieveWard paramsValObj;
			string checkID;
			string api = "/achievereward";

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdAchieveWard> ();
				paramsValObj.m_isRetry = 1;
				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdAchieveWard();

				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_sid = m_page.achievement_id;
			}

			string url = SavedContext.getApiUrl(api);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_ACHIEVEWARD, checkID);
		}

		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			switch (data.m_reqTag) {
			case REQ_THIRD_ACHIEVE:
				{
					RespThirdAchieve resp = Utils.bytesToObject<RespThirdAchieve> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							try
							{
								//List<JsonAchieve> list = SimpleJson.SimpleJson.DeserializeObject<List<JsonAchieve>>(resp.m_achieve.ToString());
								List<JsonAchieve> list = JsonMapper.ToObject<List<JsonAchieve>>(resp.m_achieve.ToString());
								m_page.list_achieve = list;
							}

							catch (SerializationException ex) 
            				{   
                				//直接显示: 游戏数据损坏, 请重新启动游戏;
                				Log.w<ValUtils>(ex.Message);
                				Debug.Log("SerializationException"+ex.Message);
                				//tellOnTableLoadErr();
            				}   
            				catch (Exception ex) 
            				{   
                				Debug.Log("Exception"+ ex.Message + ", " + ex.GetType().FullName);
             
                				//tellOnTableLoadErr();
            				}   							

							Debug.Log (resp.m_achieve);
						
						}
						break;

					default:
						{
							Debug.Log (resp.m_code);
							ValTableCache valCache = m_page.getValTableCache ();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode> (m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow (valDict, resp.m_code);
							m_page.toast.showToast (val.text);
						}
						break;



					}
				}
				break;

			case REQ_THIRD_ACHIEVEWARD:
				{
					RespThirdAchieveWard resp = Utils.bytesToObject<RespThirdAchieveWard> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							
							//JsonAchieve js_Achieve = SimpleJson.SimpleJson.DeserializeObject<JsonAchieve>(resp.m_achieve.ToString());
							m_page.toast.showToast ("领取成功");
							Debug.Log("领取成功");
							m_page.RefreshDesc(true);
						}
						break;

					default:
						{
							Debug.Log (resp.m_code);
							ValTableCache valCache = m_page.getValTableCache ();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode> (m_page.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow (valDict, resp.m_code);
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
