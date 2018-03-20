using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;
using System;
using DG.Tweening;

/**************************************
*FileName: PublicUICheckPage.cs
*User: ysr 
*Data: 2018/1/30
*Describe: 签到页面
**************************************/

public class PublicUIHeadEdit : UIPage
{
	private const string TAG = "PublicUIHeadEdit";

	Controller m_controller;

	private GameObject Item = null;

	private GameObject List = null;

	private GameObject Desc = null;

	//当前选择的item
	UIHeadImgItem currentItem = null;

	List<UIHeadImgItem> Items = new List<UIHeadImgItem>();

	private int head_id = 0;


	public PublicUIHeadEdit() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
	{
		//布局预制体
		uiPath = "Prefabs/UI/PublicUI/PublicUIHeadEdit";

	}

	public override void Awake(GameObject go)
	{

		m_controller = new Controller (this);

		init ();

		Desc = this.transform.Find ("content/desc").gameObject;
		//Desc.SetActive (false);

		Desc.transform.Find("img_desc").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+SavedData.s_instance.m_user.m_head);
		Desc.transform.Find("tx_desc").GetComponent<Text>().text = "头像名";


		this.gameObject.transform.Find ("content/btn_cancel").GetComponent<Button> ().onClick.AddListener (() => {
			//取消保存
			Hide();
		});

		this.gameObject.transform.Find ("content/btn_confim").GetComponent<Button> ().onClick.AddListener (() => {
			//保存头像
			m_controller.reqThirdNewUser (false);
		});

	
		this.gameObject.transform.Find("content/btn_close").GetComponent<Button>().onClick.AddListener(() =>
		{
			// 隐藏
			Hide();
		});
	}



	private void init()
	{

		Item = this.transform.Find("content/panel/Viewport/Content/item").gameObject;
		List = this.transform.Find("content/panel").gameObject;
		Item.SetActive (false);

		for (int i = 0; i < 100; i++) {
			CreateItem(i);		
		}

	}


	public override void Refresh()
	{

	}

	private void CreateItem(int id)
	{
		GameObject go = GameObject.Instantiate(Item) as GameObject;
		go.transform.SetParent(Item.transform.parent);
		go.transform.localScale = Vector3.one;
		go.SetActive(true);

		UIHeadImgItem item = go.AddComponent<UIHeadImgItem>();
		item.Refresh(id);

		go.AddComponent<Button>().onClick.AddListener(OnClickItem);

	}

	private void OnClickItem()
	{
		UIHeadImgItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<UIHeadImgItem>();
		head_id = item.data;
		ShowDesc (item);
	}


	private void ShowDesc(UIHeadImgItem item)
	{
		currentItem = item;
		Desc.SetActive(true);
		Desc.transform.localPosition = new Vector3(300f, Desc.transform.localPosition.y,Desc.transform.localPosition.z);


		Desc.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), 0.25f, true);
		RefreshDesc(item);
	}

	private void RefreshDesc(UIHeadImgItem item)
	{
		Desc.transform.Find("img_desc").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+item.data);
		Desc.transform.Find("tx_desc").GetComponent<Text>().text = "头像名";

	}





	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		//valCache.markPageUseOrThrow<ValGlobal> (m_pageID, ConstsVal.val_global);

	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{
		//valCache.unmarkPageUse(m_pageID, ConstsVal.val_global);
	}


	private const int MSG_HTTP_OK = 1;
	private const int MSG_HTTP_ERR = 2;
	private const int MSG_NET_ERR = 3;

	public void handleMessage(HandlerMessage msg)
	{
		Log.d<NetHttp>("handleMessage");

		switch (msg.m_what) {
		case MSG_HTTP_OK:
			{

			}
			break;
		}
	}

	class Controller : BaseController<PublicUIHeadEdit>,NetHttp.INetCallback
	{
		NetHttp m_netHttp;


		PublicUIHeadEdit m_headEdit;
		public Controller(PublicUIHeadEdit iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_headEdit = iview;
		}


		public void onDestroy()
		{
			m_netHttp.setPageNetCallback (null);

		}

		//用于标识是那个接口用于处理接受函数
		private const int REQ_THIRD_UPDATEUSER = 5;

		public void reqThirdNewUser(bool isRetry)
		{

			ReqThirdNewUser paramsValObj;
			string checkID;

			string api = "/newuser";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdNewUser> ();
				paramsValObj.m_isRetry = 1;
				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdNewUser();
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_type = 3;      //1修改昵称，2修改签名,3 替换头像

				paramsValObj.m_head = m_headEdit.head_id;
			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_UPDATEUSER, checkID);

		}

	




		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			Debug.Log ("onHttpOk");
			switch (data.m_reqTag) {
			case REQ_THIRD_UPDATEUSER:
				{
					RespThirdNewUser resp = Utils.bytesToObject<RespThirdNewUser> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							SavedData.s_instance.m_user.m_head = m_headEdit.head_id;
							UIPage.ShowPage<InfoUIPage> ();
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
