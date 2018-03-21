using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using tpgm.UI;
using tpgm;
using DG.Tweening;
using SimpleJson;
using UnityEngine.SceneManagement;
using Pomelo.DotNetClient;
using System;

public class RoomUIMatch : UIPage
{
	private const string TAG = "RoomUIMatch";
	Coroutine coroutine;

	GameObject tiemObj = null;
	public RoomUIMatch() : base(UIType.PopUp, UIMode.DoNothing, UICollider.WithBg)
	{
		//布局预制体
		uiPath = "prefabs/ui/RoomUI/RoomUIMatch";

	}

	//秒定时器
	IEnumerator Timer() {
		while (true) {
			yield return new WaitForSeconds(1.0f);
			AtRefresh ();
		}
	}

	public override void Awake(GameObject go)
	{
		tiemObj = this.transform.Find("content/tx_time").gameObject;
		this.gameObject.transform.Find("content/btn_close").GetComponent<Button>().onClick.AddListener(() =>
			{
				Hide();
				UIRoot.Instance.StopCoroutine(coroutine);

			});


	}

	public override void Refresh()
	{
		//定时器
		coroutine = UIRoot.Instance.StartCoroutine(Timer());
	
	}


	private void AtRefresh()
	{
		count++;
		time++;
		tiemObj.GetComponent<Text> ().text = ""+time;
	}


	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

	}
		


	private int rad =0;
	private int yellow = 0;
	private int green = 0;

	int count =0;
	int time =0 ;

	//
	private void AddNewUserItem(string uid, int imgID,int player_group)
	{

		switch (player_group) {
		case 1:
			{
				Debug.Log ("rad");
				GameObject item = this.gameObject.transform.Find ("content/player_groups").transform.GetChild (rad).gameObject;
				item.GetComponent<Image> ().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+imgID);
				item.name = uid;
				rad++;
				break;
			}
		case 2:
			{
				Debug.Log ("yellow" +(yellow+3));
				GameObject item = this.gameObject.transform.Find ("content/player_groups").transform.GetChild (yellow+3).gameObject;
				item.name = uid;
				item.GetComponent<Image> ().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+imgID);
				yellow++;
				break;
			}
		case 3:
			{
				Debug.Log ("green");
				GameObject item = this.gameObject.transform.Find ("content/player_groups").transform.GetChild (green+6).gameObject;
				item.name = uid;
				item.GetComponent<Image> ().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+imgID);
				green++;
				break;
			}
		}
	}


	class Controller : BaseController<RoomUIMatch>
	{

		private MainLooper m_initedLooper;

		PomeloClient m_pClient;
		RoomUIMatch m_page;
		//code

		public Controller(RoomUIMatch iview):base(null)
		{
			m_initedLooper = MainLooper.instance();
			InitNetEvent();
			m_page = iview;
		}

		public void onDestroy()
		{

			//可能在login页面没有登录
			if (null != SavedContext.s_client) {
				SavedContext.s_client.NetWorkStateChangedEvent -= (state) => {
					Debug.Log (""+state);
				};
			}

		}


		//发送匹配请求
		public void onPomeloEvent_Match()
		{
			if (SavedContext.s_client != null) {
				JsonObject jsMsg = new JsonObject ();
				jsMsg ["roomNum"] = SavedData.s_instance.m_roomNum;
				SavedContext.s_client.request ("area.gloryHandler.match", jsMsg, (data) => {
					Debug.Log(data);
				});
			} else {
				Debug.LogError ("pClient null");
			}
		}


		//注册网络事件
		void InitNetEvent()
		{
			PomeloClient pClient = SavedContext.s_client;
			if (pClient != null)
			{

				pClient.on("match", (data) =>{
					#if false
					HandlerMessage msg = MainLooper.obtainMessage(handleMessage, MSG_POMELO_MATCH);
					Debug.Log(data);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
					#endif
				});

			

			}

		}

	}
}

#if false
if (ConectData.Instance.start_groups != null) {
GameObject item;
foreach (UDGroup.Group player in ConectData.Instance.start_groups) {
bool isFind = false;
for (int i = 0; i < 9; i++) {
item = this.gameObject.transform.Find ("content/player_groups").transform.GetChild (i).gameObject;
//Debug.Log ("count"+ConectData.Instance.start_groups.Count);
//Debug.Log (item.name);
//Debug.Log (player.uid);
if (item.name == player.uid) {
//Debug.Log ("isFind");
isFind = true;	
break;
}
}
if (!isFind) {
//Debug.Log ("is not Find");
//Debug.Log (player.group);
AddNewUserItem (player.uid, player.head, int.Parse (player.group.Substring (player.group.Length - 1)));


}
}
}
if (time == 60) {
UIRoot.Instance.StopCoroutine(coroutine);
//SceneManager.LoadScene("Game");
//Application.LoadLevel("Game");
}

#endif