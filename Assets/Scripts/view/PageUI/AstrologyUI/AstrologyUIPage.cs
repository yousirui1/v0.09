using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;


/**************************************
*FileName: AstrologyUIPage.cs
*User: ysr 
*Data: 2018/1/30
*Describe: 占星界面
**************************************/

public class AstrologyUIPage : UIPage
{
	private const string TAG = "AstrologyUIPage";

	private long AwadTime = 0;
	public AstrologyUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/AstrologyUI/AstrologyUIMain";
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

	Controller m_controller;

	public override void Awake(GameObject go)
	{
		m_controller = new Controller(this);
		
		this.gameObject.transform.Find("roll/btn_roll").GetComponent<Button>().onClick.AddListener (() =>
		{
			m_controller.reqThirdAstrology(false, 1);
		});
	
		this.gameObject.transform.Find("ag_roll/btn_roll").GetComponent<Button>().onClick.AddListener (() =>
		{
			m_controller.reqThirdAstrology(false, 2);
		});
			

		this.gameObject.transform.Find("btn_back").GetComponent<Button>().onClick.AddListener(() =>
			{
				// 返回
				ClosePage();
			});
	}




	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
		//全局表
		valCache.markPageUseOrThrow<ValGlobal>(m_pageID, ConstsVal.val_global);

		//code
		valCache.markPageUseOrThrow<ValCode>(m_pageID, ConstsVal.val_code);

		//占星
		valCache.markPageUseOrThrow<ValAstrology>(m_pageID, ConstsVal.val_astrology);
		
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{
		valCache.unmarkPageUse (m_pageID, ConstsVal.val_global);

		valCache.unmarkPageUse(m_pageID, ConstsVal.val_code);

		valCache.unmarkPageUse (m_pageID, ConstsVal.val_astrology);

	}

	class Controller : BaseController<AstrologyUIPage>, NetHttp.INetCallback
	{
		NetHttp m_netHttp;
		AstrologyUIPage m_astrology;

		public Controller(AstrologyUIPage iview):base(null)
		{
			m_netHttp = new NetHttp();
			m_netHttp.setPageNetCallback(this);
			m_astrology = iview;
		}

		public void onDestroy()
		{
			m_netHttp.setPageNetCallback (null);
		}

		//用于标识是那个接口用于处理接受函数
		private const int REQ_THIRD_ASTROLOGY = 12;

		public void reqThirdAstrology(bool isRetry, int type)
		{

			ReqThirdAstrology paramsValObj;
			string checkID;

			string api = "/astrology";

			AppUtils.apiCheckID (api);

			if (isRetry) {
				paramsValObj = m_netHttp.peekTopReqParamsValObj<ReqThirdAstrology> ();
				paramsValObj.m_isRetry = 1;

				checkID = paramsValObj.m_checkID;

			} else {
				checkID = AppUtils.apiCheckID(api);
				paramsValObj = new ReqThirdAstrology();
				//重连
				paramsValObj.m_checkID = checkID;
				paramsValObj.m_isRetry = 0;
				paramsValObj.m_token = SavedData.s_instance.m_user.m_token;
				paramsValObj.m_type = type;  //普通占星,大师占星

			}

			string url = SavedContext.getApiUrl(api);
			Debug.Log (url);
			m_netHttp.postParamsValAsync(url,paramsValObj, REQ_THIRD_ASTROLOGY, checkID);

		}





		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			Debug.Log ("onHttpOk");
			switch (data.m_reqTag) {
			case REQ_THIRD_ASTROLOGY:
				{
					RespThirdAstrology resp = Utils.bytesToObject<RespThirdAstrology> (respData.m_protobufBytes);
					switch (resp.m_code) {
					case 200:
						{
							UIPage.ShowPage<PublicUINotice> (resp.m_materials);
						}
						break;

					default:
						{
							ValTableCache valCache = m_astrology.getValTableCache();
							Dictionary<int, ValCode> valDict = valCache.getValDictInPageScopeOrThrow<ValCode>(m_astrology.m_pageID, ConstsVal.val_code);
							ValCode val = ValUtils.getValByKeyOrThrow(valDict, resp.m_code);
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
