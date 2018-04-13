using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;


public class LoadUIPage : UIPage
{

	private const string TAG = "LoadUIPage";

	Coroutine coroutine;

	private EventController eventController;

	AsyncOperation async;

	private MainLooper m_initedLooper;

    public LoadUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
    {
        //布局预制体
		uiPath = "Prefabs/UI/LoadUI/LoadUIPage";
    }


    public override void Awake(GameObject go)
    {
		m_initedLooper = getMainLooper ();

		coroutine = UIRoot.Instance.StartCoroutine(loadScene());
    }


	IEnumerator loaditem()
	{
		//初始化Canvas
		if (GameRoot.Instance == null)

		eventController = new GameObject ("EventController").AddComponent<EventController> ();
		eventController.InitObj ();

		HandlerMessage msg = MainLooper.obtainMessage(handleMsgDispatch, MSG_Load_OVER);
		msg.m_dataObj = data;
		m_initedLooper.sendMessage(msg);

		//eventController.InitMap (data.map, data.magicStage);
		//结束
	
		yield return 0;
	}

	IEnumerator loadScene()
	{
		//异步读取场景。
		//Globe.loadName 就是A场景中需要读取的C场景名称。
		async = Application.LoadLevelAsync("Game");

		//读取完毕后返回， 系统会自动进入C场景
		yield return async;

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
