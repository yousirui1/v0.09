using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;


public class LoadingUIPage : UIPage
{

	private const string TAG = "LoadingUIPage";

	Coroutine coroutine;

	private EventController eventController;


	private MainLooper m_initedLooper;

	private Image img_loadbar = null;

	public LoadingUIPage() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
    {
        //布局预制体
		uiPath = "Prefabs/UI/LoadUI/LoadingUIPage";
    }

	//页面重载入
	public override void Refresh()
	{
		loaditem (this);
	}


    public override void Awake(GameObject go)
    {
		m_initedLooper = getMainLooper ();
		img_loadbar = this.gameObject.transform.Find ("img_loading/img_bar").GetComponent<Image> ();

    }

	void loaditem(LoadingUIPage iview)
	{
		//初始化Canvas
		GameRoot root = GameRoot.Instance;

		eventController = new GameObject ("EventController").AddComponent<EventController> () as EventController;
		eventController.InitObj (root.gameObject);

		#if false
		RespThirdLoad buf = SimpleJson.SimpleJson.DeserializeObject<RespThirdLoad> );
		SavedData.s_instance.m_map.file = buf.map;
		SavedData.s_instance.m_map.skill_list = buf.magicStage;
		#endif

		eventController.InitMap (SavedData.s_instance.m_map.file, SavedData.s_instance.m_map.skill_list, iview);
	}

	public const int MSG_LOAD_PART_1 = 1;
	public const int MSG_LOAD_PART_2 = 2;
	public const int MSG_LOAD_PART_3 = 3;
	public const int MSG_LOAD_PART_4 = 4;
	public const int MSG_LOAD_PART_5 = 5;
	public const int MSG_LOAD_PART_6 = 6;
	public const int MSG_LOAD_PART_7 = 7;
	public const int MSG_LOAD_PART_8 = 8;
	public const int MSG_LOAD_PART_9 = 9;
	public const int MSG_LOAD_PART_10 = 10;

	public void handleMessage(HandlerMessage msg)
	{
		switch (msg.m_what) {

		case MSG_LOAD_PART_1:
			{
				img_loadbar.fillAmount = 0.1f;
			}
			break;

		case MSG_LOAD_PART_2:
			{

				img_loadbar.fillAmount = 0.2f;
			}
			break;

		case MSG_LOAD_PART_3:
			{

				img_loadbar.fillAmount = 0.3f;
			}
			break;
		case MSG_LOAD_PART_4:
			{

				img_loadbar.fillAmount = 0.4f;
			}
			break;
		case MSG_LOAD_PART_5:
			{

				img_loadbar.fillAmount = 0.5f;
			}
			break;
		case MSG_LOAD_PART_6:
			{

				img_loadbar.fillAmount = 0.6f;
			}
			break;

		case MSG_LOAD_PART_7:
			{
				img_loadbar.fillAmount = 0.7f;
			}
			break;

		case MSG_LOAD_PART_8:
			{
				img_loadbar.fillAmount = 0.8f;
			}
			break;

		case MSG_LOAD_PART_9:
			{
				img_loadbar.fillAmount = 0.95f;
			}
			break;

		case MSG_LOAD_PART_10:
			{
				img_loadbar.fillAmount = 1.0f;
			}
			break;

		default :
			{

			}
			break;
		}

	}



	public const int MSG_Load_OVER = 1;

	protected override void onHandleMsg(HandlerMessage msg)
	{
		switch (msg.m_what) {

		case MSG_Load_OVER:
			{
				Debug.Log ("LoadOver");

			}
			break;

	
		default :
			{

			}
			break;
		}
	}




	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

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

		public virtual void onHttpOk(DataNeedOnResponse data, ResponseData respData)
		{
			Debug.Log ("onHttpOk");
			switch (data.m_reqTag) {

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
