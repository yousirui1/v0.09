using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using tpgm.UI;
using tpgm;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/29
*Describe: 邮件页面
**************************************/

public class PublicUIEmailPage : UIPage
{
	private const string TAG = "ChatUIPage";
	
	GameObject Item = null;
	GameObject List = null;

	GameObject Desc = null;

	//当前选择的item
	UIEmailItem currentItem = null;

	List<UIEmailItem> Items = new List<UIEmailItem>();

	Controller m_controller;
	public PublicUIEmailPage() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
	{
		//布局预制体
		uiPath = "Prefabs/UI/PublicUI/PublicUIEmail";

	}

	public override void Awake(GameObject go)
	{
		m_controller = new Controller (this);

		m_controller.reqThirdEmails (false);


		Desc = this.gameObject.transform.Find("content/group_msg").gameObject;
		Desc.SetActive (false);

		this.gameObject.transform.Find("content/btn_back").GetComponent<Button>().onClick.AddListener(() =>
		{
			// 隐藏
			Hide();
		});
		

	}





	private void addItem(List<JsonThirdEmail> email_list)
	{
		List = this.transform.Find("content/bg_email/panel").gameObject;
		Item = this.transform.Find("content/bg_email/panel/Viewport/Content/item").gameObject;
		Item.SetActive(false);

		for (int i = 0; i < email_list.Count; i++) {
			GameObject go = GameObject.Instantiate(Item) as GameObject;
			go.transform.SetParent(Item.transform.parent);
			go.transform.localScale = Vector3.one;
			go.SetActive(true);

			UIEmailItem item = go.AddComponent<UIEmailItem>();
			item.Refresh(email_list[i]);
			Items.Add(item);
			
			go.AddComponent<Button>().onClick.AddListener(OnClickItem);
		}
	}

	//邮件详情隐藏
	public override void Hide()
	{
		for(int i =0; i<Items.Count; i++)
		{
			GameObject.Destroy(Items[i].gameObject);
		}
		Items.Clear();

		this.gameObject.SetActive(false);

	}

	private void OnClickItem()
	{
		UIEmailItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UIEmailItem>();

		ShowDesc(item);
	}

	//显示详细信息
	private void ShowDesc(UIEmailItem item)
	{
		currentItem = item;
		Desc.SetActive(true);
		Desc.transform.localPosition = new Vector3(300f, Desc.transform.localPosition.y,Desc.transform.localPosition.z);
	

		Desc.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 0.25f, true);
		RefreshDesc(item);
			

	}

	//更新详细信息
	private void RefreshDesc(UIEmailItem item)
	{
		Desc.transform.Find("tx_title").GetComponent<Text>().text = item.data.title;	
		Desc.transform.Find("tx_desc").GetComponent<Text>().text = item.data.content;	


		Desc.transform.Find ("btn_draw").GetComponent<Button> ().onClick.AddListener (() => {
			
		});

	}		
	

	

	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		

	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

	}

	class Controller : BaseController<PublicUIEmailPage>, NetHttp.INetCallback
	{
		NetHttp m_netHttp;
		PublicUIEmailPage m_email;
		public Controller(PublicUIEmailPage iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_email = iview;
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
		private const int REQ_THIRD_EMAILS = 9;

		public void reqThirdEmails(bool isRetry)
		{

			ReqThirdEmails paramsValObj;
			string checkID;

			string api = "/emails";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdEmails> ();
				paramsValObj.m_isRetry = 1;

				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdEmails();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;

			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_EMAILS, checkID);

		}





		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			Debug.Log ("onHttpOk");
			switch (data.m_reqTag) {
			case REQ_THIRD_EMAILS:
				{
					RespThirdEmails resp = Utils.bytesToObject<RespThirdEmails> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							List<JsonThirdEmail> email_list= SimpleJson.SimpleJson.DeserializeObject<List<JsonThirdEmail>> (resp.m_emailList);
							Debug.Log (resp.m_emailList);
							m_email.addItem (email_list);
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
			Debug.Log (TAG +":" +"onOtherErr"+data.m_url);

		}
	}

}
