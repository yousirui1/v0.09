using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;


public class LoadNoticeUIPage : UIPage
{

	private const string TAG = "LoadNoticeUIPage";

	Coroutine coroutine;

	private MainLooper m_initedLooper;

	private int fream = 0;


	public LoadNoticeUIPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
	{
		//布局预制体
		uiPath = "Prefabs/UI/LoadUI/LoadNoticeUIPage";
	}

	//页面重载入
	public override void Refresh()
	{
		fream = 0;
	}


	public override void Awake(GameObject go)
	{
		coroutine = UIRoot.Instance.StartCoroutine(Timer());
	}


    private void LoadNoticeEnd()
	{
		UIRoot.Instance.StopCoroutine (coroutine);
		UIPage.ShowPage<SplashUIPage> ();
	}

	//秒定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);
			if (fream == 5) {
				LoadNoticeEnd ();
			} else {
				fream++;
			}
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
