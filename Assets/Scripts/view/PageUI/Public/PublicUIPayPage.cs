using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;

public class PublicUIPayPage : UIPage
{
	private const string TAG = "PublicUIPayPage";
	Controller m_controller;

	public PublicUIPayPage() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
	{
		//布局预制体
		uiPath = "Prefabs/UI/PublicUI/PublicUIPayPage";

	}

	public override void Awake(GameObject go)
	{
		ValStore val = (ValStore)this.data;



		m_controller = new Controller (this);

		this.transform.Find ("content/item/tx_itemhead").GetComponent<Text> ().text = val.name;
		this.transform.Find("content/item/img_materials").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+0);

		switch (val.buy_type) {
		case  0:  //立即购买
			{
				//string[] rewards = val.reward.Split (';');

				this.transform.Find ("content/item/tx_itemcount").GetComponent<Text> ().text = val.reward;

			}break;
		case 1: //延时购买
			{
				this.transform.Find ("content/item/tx_itemcount").GetComponent<Text> ().text = "延迟"+val.start+val.validity;
			}
			break;
		case 2: //限时购买
			{
				this.transform.Find ("content/item/tx_itemcount").GetComponent<Text> ().text ="限时"+ val.start+val.validity;
			}
			break;
		}

		//string[] rewards = val.reward.Split (':');
		this.transform.Find("content/item/tx_itemmoney").GetComponent<Text>().text = "￥"+val.price;
		//child.GetComponent<SpriteRenderer>().sprite =

		this.gameObject.transform.Find("content/btn_cancel").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 隐藏
				Hide();
			});
		
		this.gameObject.transform.Find("content/btn_pay").GetComponent<Button>().onClick.AddListener(() =>
			{
				m_controller.reqThirdGoodsBuy(false,val.id);
			});

	}

	//刷新
	public override void Refresh()
	{
		ValStore val = (ValStore)this.data;



		m_controller = new Controller (this);

		this.transform.Find ("content/item/tx_itemhead").GetComponent<Text> ().text = val.name;
		this.transform.Find("content/item/img_materials").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+0);

		switch (val.buy_type) {
		case  0:  //立即购买
			{
				//string[] rewards = val.reward.Split (';');

				this.transform.Find ("content/item/tx_itemcount").GetComponent<Text> ().text = val.reward;

			}break;
		case 1: //延时购买
			{
				this.transform.Find ("content/item/tx_itemcount").GetComponent<Text> ().text = "延迟"+val.start+val.validity;
			}
			break;
		case 2: //限时购买
			{
				this.transform.Find ("content/item/tx_itemcount").GetComponent<Text> ().text ="限时"+ val.start+val.validity;
			}
			break;
		}

		//string[] rewards = val.reward.Split (':');
		this.transform.Find("content/item/tx_itemmoney").GetComponent<Text>().text = "￥"+val.price;
		//child.GetComponent<SpriteRenderer>().sprite =

		this.gameObject.transform.Find("content/btn_cancel").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 隐藏
				Hide();
			});

		this.gameObject.transform.Find("content/btn_pay").GetComponent<Button>().onClick.AddListener(() =>
			{

				m_controller.reqThirdGoodsBuy(false,val.id);
			});

	}




	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		//code
		valCache.markPageUseOrThrow<ValCode>(m_pageID, ConstsVal.val_code);
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{
		//code
		valCache.markPageUseOrThrow<ValCode>(m_pageID, ConstsVal.val_code);
	}

	class Controller : BaseController<PublicUIPayPage>,NetHttp.INetCallback
	{
		NetHttp m_netHttp;

		PublicUIPayPage m_pay;
		public Controller(PublicUIPayPage iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_pay = iview;
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
		private const int REQ_THIRD_GOODSBUY = 13;

		public void reqThirdGoodsBuy(bool isRetry,int id)
		{

			ReqThirdGoodsBuy paramsValObj;
			string checkID;

			string api = "/goodsbuy";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdGoodsBuy> ();
				paramsValObj.m_isRetry = 1;

				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdGoodsBuy();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_goodsID = id;	
			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_GOODSBUY, checkID);

		}





		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			Debug.Log ("onHttpOk");
			switch (data.m_reqTag) {
			case REQ_THIRD_GOODSBUY:
				{
					RespThirdGoodsBuy resp = Utils.bytesToObject<RespThirdGoodsBuy> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							UIPage.ShowPage<StoreUIPage> ();
						}
						break;

					default:
						{
							ValTableCache valCache = m_pay.getValTableCache ();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode> (m_pay.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow (valDict, resp.m_code);
							UIPage.ShowPage<PublicUINotice> (val.text);
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
