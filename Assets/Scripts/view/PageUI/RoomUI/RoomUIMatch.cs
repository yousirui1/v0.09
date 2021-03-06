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
using LitJson;
using System.Runtime.Serialization;

public class RoomUIMatch : UIPage
{
	private const string TAG = "RoomUIMatch";
	Coroutine coroutine;

	GameObject tiemObj = null;

	Controller controller;

	private int play_count = 0;


	public RoomUIMatch() : base (UIType.Normal, UIMode.HideOther, UICollider.None )
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
		controller = new Controller (this);
		tiemObj = this.transform.Find("content/tx_time").gameObject;
		this.gameObject.transform.Find("content/btn_close").GetComponent<Button>().onClick.AddListener(() =>
			{
				SoundPlay.btnClick();
				controller.onPomeloEvent_LeaveRoom();
				ClosePage();
				UIRoot.Instance.StopCoroutine(coroutine);

			});


	}

	public override void Refresh()
	{
		ClearUserItem ();

		//定时器
		coroutine = UIRoot.Instance.StartCoroutine(Timer());
		controller.onPomeloEvent_Match ();
		ClearData ();

	}


	private void AtRefresh()
	{
		count++;
		time++;
		tiemObj.GetComponent<Text> ().text = ""+time;
		this.transform.Find ("content/tx_players").GetComponent<Text> ().text = play_count + "/9 已就绪";

	}


	protected override void loadRes(TexCache texCache, ValTableCache valCache)
	{
	}

	protected override void unloadRes(TexCache texCache, ValTableCache valCache)
	{

	}

	public const int MSG_POMELO_MATCH = 1;
	public const int MSG_POMELO_GLORYADD = 2;
	public const int MSG_POMELO_LOAD = 3;

	protected override void onHandleMsg(HandlerMessage msg)
	{
		switch (msg.m_what) {

		case MSG_POMELO_MATCH:
			{
				
				JsonObject data = (JsonObject)msg.m_dataObj;
				object match = null;
				object roomNum = null;
				if (data.TryGetValue ("match", out match) && data.TryGetValue ("roomNum", out roomNum)) {
					if (Convert.ToInt32 (match) == 2) {
						SavedData.s_instance.m_gameRoom = roomNum.ToString ();
						UIRoot.Instance.StopCoroutine (coroutine);
						ShowPage<LoadingUIPage> ();
					}
				}

			}
			break;

		case MSG_POMELO_GLORYADD:
			{

				JsonObject data = (JsonObject)msg.m_dataObj;
				//ssDebug.Log (data);
				try
				{
					//RespThirdGloryAdd buf = SimpleJson.SimpleJson.DeserializeObject<RespThirdGloryAdd> (data.ToString());
					RespThirdGloryAdd buf = JsonMapper.ToObject<RespThirdGloryAdd> (data.ToString());
					//建立玩家字典数据


					play_count++;
					foreach (NewUser player in buf.newUser) {
						//保存数据
						RespThirdUserData userdata = new RespThirdUserData ();
						userdata.nickname = player.nickname;
						userdata.score = 0;
						userdata.kill = 0;
						userdata.death = 0;
						userdata.assit = 0;
						userdata.group = player.group;
						userdata.head = player.head;

						if (!SavedData.s_instance.m_userCache.ContainsKey (player.uid)) {
							SavedData.s_instance.m_userCache.Add (player.uid, userdata);
							UserRank rank = new UserRank (player.uid,player.nickname,0);
							SavedData.s_instance.m_userrank.Add (rank);
						}

						bool isFind = false;
						for (int i = 0; i < 9; i++) {
							GameObject item = this.gameObject.transform.Find ("content/player_groups").transform.GetChild (i).gameObject;
							if (item.name == player.uid) {
								isFind = true;	
								break;
							}
						}
						if (!isFind) {
							AddNewUserItem (player.uid, player.head, int.Parse (player.group.Substring (player.group.Length - 1)));
						}
					}

				}
				  catch (SerializationException ex) 
            	{   
                	//直接显示: 游戏数据损坏, 请重新启动游戏;
                	Log.w<ValUtils>(ex.Message);
                	Debug.Log("SerializationException ysr"+ex.Message);
                	//tellOnTableLoadErr();
            	}   
            	catch (Exception ex) 
            	{   
                	Debug.Log("Exception ysr"+ ex.Message + ", " + ex.GetType().FullName);
                	//Log.w<ValUtils>(ex.Messasoge + ", " + ex.GetType().FullName);
                	//tellOnTableLoadErr();
            	}  

			}
			break;

		case MSG_POMELO_LOAD:
			{
				JsonObject data = (JsonObject)msg.m_dataObj;
				try
				{
					//RespThirdLoad buf = SimpleJson.SimpleJson.DeserializeObject<RespThirdLoad> (data.ToString());
					RespThirdLoad buf = JsonMapper.ToObject<RespThirdLoad> (data.ToString());
					SavedData.s_instance.m_map.file = buf.map;
					SavedData.s_instance.m_map.skill_list = buf.magicStage;
				}
				catch (SerializationException ex) 
            	{   
                	//直接显示: 游戏数据损坏, 请重新启动游戏;
                	Log.w<ValUtils>(ex.Message);
                	Debug.Log("SerializationException ysr"+ex.Message);
                	//tellOnTableLoadErr();
            	}   
            	catch (Exception ex) 
            	{   
               	 	Debug.Log("Exception ysr"+ ex.Message + ", " + ex.GetType().FullName);
                	//Log.w<ValUtils>(ex.Messasoge + ", " + ex.GetType().FullName);
                	//tellOnTableLoadErr();
            	}   


			}
			break;


		default :
			{

			}
			break;
		}
	}


	private int rad =0;
	private int yellow = 0;
	private int green = 0;

	int count =0;
	int time =0 ;


	private void ClearData()
	{
		rad = 0;
		yellow = 0;
		green = 0;
		count = 0;
		time = 0;
		play_count = 1;
	}
	//
	private void AddNewUserItem(string uid, int imgID,int player_group)
	{

		switch (player_group) {
		case 1:
			{
			//	Debug.Log ("rad");
				GameObject item = this.gameObject.transform.Find ("content/player_groups").transform.GetChild (rad).gameObject;
				item.GetComponent<Image> ().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+imgID);
				item.name = uid;
				rad++;
				break;
			}
		case 2:
			{
			//	Debug.Log ("yellow" +(yellow+3));
				GameObject item = this.gameObject.transform.Find ("content/player_groups").transform.GetChild (yellow+3).gameObject;
				item.name = uid;
				item.GetComponent<Image> ().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+imgID);
				yellow++;
				break;
			}
		case 3:
			{
				//Debug.Log ("green"+(green+6));
				GameObject item = this.gameObject.transform.Find ("content/player_groups").transform.GetChild (green+6).gameObject;
				item.name = uid;
				item.GetComponent<Image> ().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+imgID);
				green++;
				break;
			}
		}
	}

	private void ClearUserItem()
	{
		GameObject items = this.gameObject.transform.Find ("content/player_groups").gameObject;
		for (int i = 0; i < items.transform.childCount; i++) {
			items.transform.GetChild (i).GetComponent<Image> ().sprite = null;
			items.transform.GetChild (i).name = "img_player0" + i;
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
				});
			} else {
				Debug.LogError ("pClient null");
			}
		}


		//发送匹配请求
		public void onPomeloEvent_LeaveRoom()
		{
			if (SavedContext.s_client != null) {
				JsonObject jsMsg = new JsonObject ();
				jsMsg ["roomNum"] = SavedData.s_instance.m_roomNum;
				SavedContext.s_client.request ("area.gloryHandler.leaveRoom", jsMsg, (data) => {
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
				//服务器同一开始
				pClient.on("match", (data) =>{
					Debug.Log(data);
					HandlerMessage msg = MainLooper.obtainMessage(m_page.handleMsgDispatch, MSG_POMELO_MATCH);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});
				//所以玩家信息用来显示
				pClient.on("gloryAdd", (data) =>{
				//	Debug.Log(data);
					HandlerMessage msg = MainLooper.obtainMessage(m_page.handleMsgDispatch, MSG_POMELO_GLORYADD);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});
				//地图数据
				pClient.on("load", (data) =>{
					Debug.Log(data);
					HandlerMessage msg = MainLooper.obtainMessage(m_page.handleMsgDispatch, MSG_POMELO_LOAD);
					msg.m_dataObj = data;
					m_initedLooper.sendMessage(msg);
				});

				//地图数据
				pClient.on("playerInfo", (data) =>{
					Debug.Log(data);
				});
			

			}

		}

	}
}

#if false
if (ConectData.Instance.start_groups != null) {
GameObject item;

}
}
if (time == 60) {
UIRoot.Instance.StopCoroutine(coroutine);
//SceneManager.LoadScene("Game");
//Application.LoadLevel("Game");
}

#endif
