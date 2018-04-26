using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using tpgm;
using tpgm.UI;

/**************************************
*FileName: GameMenu.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 游戏界面按钮和菜单
**************************************/
using System;


public class GameMenu : MonoBehaviour
{
	GameObject gameUIObj = null;		
	GameObject deathPanelObj = null;  //死亡面板
	GameObject bastplayerObj = null;  //最佳
	GameObject killplayerObj = null;  //击杀

	GameObject broadcastObj = null;   //广播通知

	GameObject tiemObj = null;			//倒计时

	GameObject systemInfoObj = null;   //系统信息
	GameObject signalObj = null;   //系统信息

	GameObject killDataObj = null;		//击杀数量

	//游戏
	public const int STATE_GAME = 0;
	//菜单
	public const int STATE_MENU = 1;
	//设置
	public const int STATE_OPTION = 2;
	//帮助
	public const int STATE_HELP = 3;
	//返回大厅
	public const int STATE_EXIT = 4;

	//保存当前状态
	private int gameState = 0;	

	//Canvas尺寸
	private int height;
	private int width;


	public const int BTN_FIRE = 0;		//普攻
	public const int BTN_SKILL = 1;	//技能
	public const int BTN_FLASH = 2;	//闪现
	public const int BTN_SET = 3;		//设置
	public const int BTN_CHAT = 4;		//聊天
	public const int SWITCH_VOICE = 5;	//语音开关
	public const int BTN_VOICE = 6;	//按键语音
	public const int BTN_REVIVE = 7; //复活按钮


	//1:1血,2:2杀,3:3杀,4:4杀,5:5杀,6:6杀,7:超神,8:终结

	public const int BROADCAST_SKILL = 0;   //击杀
	public const int BROADCAST_DOMINATNG = 1;	//主宰比赛
	public const int BROADCAST_GODLIKE = 2;	//超神
	public const int BROADCAST_SHUTDOWN = 3;	//终结


	private bool isFire = false;
	private bool isFlash = false;
	private bool isSkill= false;

	//击杀Tip不同的类型
	GameObject itemObj0 = null;
	GameObject itemObj1 = null;
	GameObject itemObj2 = null;
	GameObject itemObj3 = null;


	GameObject itemRankObjself = null;
	GameObject RankObj = null;

	GameObject resultUIObj = null;
	GameObject result_oneObj = null;  //个人结算界面
	GameObject result_teamObj; //团队结算界面

	CArrowLockAt cArrowLockAt = null;


	//数值表缓存
	ValTableCache valCache = null;

	public const string m_gameID = "1";

	private int kill_cd = 0;

	Ping ping  = null;
	float delayTime = float.MinValue;

	EventController eventController = null;

	Map map = null;

	float StartTime = float.MinValue;

	void Awake()
	{
		
	}

	void Start()
	{

		Init ();

		InitUserList();
		onClickListener ();
		SendPing();
	}

	void Init()
	{
		StartTime = Time.time;

		valCache = SavedContext.s_valTableCache;

		gameUIObj = ResourceMgr.Instance ().CreateGameObject ("prefabs/gameUI/GameUI", false);
		gameUIObj.transform.parent = this.transform;
		gameUIObj.transform.localPosition = Vector3.zero;
		gameUIObj.transform.localScale = Vector3.one;
		gameUIObj.name = "GameUI";

		//死亡面板
		deathPanelObj = gameUIObj.transform.Find ("bg_death").gameObject;
		bastplayerObj = deathPanelObj.transform.Find ("bg_bestplayer").gameObject;
		killplayerObj = deathPanelObj.transform.Find ("bg_killer").gameObject;
		deathPanelObj.SetActive (false);

		broadcastObj = gameUIObj.transform.Find ("broadcast").gameObject;

		itemObj0 = broadcastObj.transform.Find ("item0").gameObject;
		itemObj1 = broadcastObj.transform.Find ("item1").gameObject;
		itemObj2 = broadcastObj.transform.Find ("item2").gameObject;
		itemObj3 = broadcastObj.transform.Find ("item3").gameObject;

		systemInfoObj = gameUIObj.transform.Find ("bg_systeminfo").gameObject;
		signalObj = systemInfoObj.transform.Find ("signal").gameObject;

		tiemObj = gameUIObj.transform.Find ("bg_time/tx_time").gameObject;

		killDataObj = gameUIObj.transform.Find ("bg_data").gameObject;

		for(int i = 0; i<broadcastObj.transform.childCount; i++)	
		{
			broadcastObj.transform.GetChild (i).gameObject.SetActive (false);
		}

		for(int i = 0; i<signalObj.transform.childCount; i++)	
		{
			signalObj.transform.GetChild (i).gameObject.SetActive (false);
		}

		eventController = GameObject.Find ("EventController").GetComponent<EventController> ();
		map = eventController.GetMap ();

		RankObj = gameUIObj.transform.Find ("bg_rank").gameObject;

		//结算界面
		resultUIObj = ResourceMgr.Instance ().CreateGameObject ("prefabs/gameUI/ResultUI", false);
		resultUIObj.transform.parent = this.transform;
		resultUIObj.transform.localPosition = Vector3.zero;
		resultUIObj.transform.localScale = Vector3.one;

		result_oneObj = resultUIObj.transform.Find ("panel_one").gameObject;
		result_teamObj = resultUIObj.transform.Find ("panel_team").gameObject;
		resultUIObj.SetActive (false);


		resultUIObj.transform.Find("btn_close").GetComponent<Button>().onClick.AddListener(() =>
			{

				Destroy(eventController.GetCanvas());
				Destroy(eventController.gameObject);


				UIRoot.Instance.gameObject.SetActive (true);
				UIRoot.Instance.CreateEventSystem();
				UIPage.ShowPage<RoomUIPrepare>();
				//回到房间
			});

		resultUIObj.transform.Find("btn_back").GetComponent<Button>().onClick.AddListener(() =>
			{
				Destroy(eventController.GetCanvas());
				Destroy(eventController.gameObject);

				UIRoot.Instance.gameObject.SetActive (true);
				UIRoot.Instance.CreateEventSystem();
				UIPage.ShowPage<MainUIPage>();
				//回到大厅
			});

		resultUIObj.transform.Find("btn_true").GetComponent<Button>().onClick.AddListener(() =>
			{
				//转发
			});

		resultUIObj.transform.Find("btn_save").GetComponent<Button>().onClick.AddListener(() =>
			{
				//保存视频
			});

		result_oneObj.transform.Find("btn_team").GetComponent<Button>().onClick.AddListener(() =>
			{
				CheckResultPanel(false);
			});

		//cArrowLockAt = gameUIObj.transform.Find ("bg_arrow").GetComponent<CArrowLockAt> ();

		height = Screen.height;
		width = Screen.width;

		SetSkillData (0,0,0);
		SetSystemInfoData ();


		eventController.InitJoyControl ();
	}


	void OnDestroy()
	{

	}


	public void SetArrowSelf(Transform selfTr)
	{
		//cArrowLockAt.SetSelf (selfTr);
	}

	bool isArrowGroup = false;
	public void SetArrowGroup(Transform groupTr)
	{
		if(!isArrowGroup)
		{
			//cArrowLockAt.SetGroup1 (groupTr);
			//isArrowGroup = true;
		}
		else
		{
			//cArrowLockAt.SetGroup2 (groupTr);
		}
	}
		

	public void SetArrowBast(Transform bastTr)
	{
		//cArrowLockAt.SetBast (bastTr);
	}
		
	//击杀提示
	public  void SetBroadcastData(RespThirdPlayData data)
	{

		for (int i = 0; i < data.attackNum.type.Count; i++) {
			switch (data.attackNum.type [i]) {
			case 6:
				{
					//主宰比赛
					GameObject itemObj = GameObject.Instantiate(itemObj1) as GameObject;
					itemObj.transform.SetParent(itemObj1.transform.parent);
					itemObj.transform.localScale = Vector3.one;
					itemObj.transform.transform.position = itemObj1.transform.position;

					itemObj.SetActive (true);
					itemObj.transform.Find("tx_kill").GetComponent<Text>().text = "" + data.attackNum.kill;
					Destroy (itemObj, 3.0f);
				}
				break;
			case 7:
				{
					//超神
					GameObject itemObj = GameObject.Instantiate(itemObj2) as GameObject;
					itemObj.transform.SetParent(itemObj2.transform.parent);
					itemObj.transform.localScale = Vector3.one;
					itemObj.transform.transform.position = itemObj2.transform.position;
					itemObj.SetActive (true);
					itemObj.transform.Find("tx_kill").GetComponent<Text>().text = "" + data.attackNum.kill;
					Destroy (itemObj, 3.0f);
				}
				break;
			case 8:
				{
					//终结
					GameObject itemObj = GameObject.Instantiate(itemObj3) as GameObject;
					itemObj.transform.SetParent(itemObj3.transform.parent);
					itemObj.transform.localScale = Vector3.one;
					itemObj.transform.transform.position = itemObj3.transform.position;
					itemObj.SetActive (true);
					itemObj.transform.Find("tx_kill").GetComponent<Text>().text = "" +data.attackNum.kill;
					itemObj.transform.Find("tx_death").GetComponent<Text>().text = "" + data.attackNum.dead;
					Destroy (itemObj, 3.0f);
				}
				break;

			default:
				{
					//0为除了1血以外的1杀 1 为1血
					//1-5杀

					GameObject itemObj = GameObject.Instantiate(itemObj0) as GameObject;
					itemObj.transform.SetParent(itemObj0.transform.parent);
					itemObj.transform.localScale = Vector3.one;
					itemObj.transform.transform.position =itemObj0.transform.position;
					itemObj.SetActive (true);
					//不是一血
					if (data.attackNum.type [i] != 1) {
						itemObj.transform.Find ("img_firstblood").gameObject.SetActive (false);
					}
					itemObj.transform.Find ("img_killcount").GetComponent<Image> ().sprite = ResourceMgr.Instance().Load<Sprite>("images/ui/tp_page_fight/kill_"+data.attackNum.type [i], true);
					itemObj.transform.Find ("tx_kill").GetComponent<Text> ().text = "" + data.attackNum.kill;
					itemObj.transform.Find ("tx_death").GetComponent<Text> ().text = "" + data.attackNum.dead;
					Destroy (itemObj, 3.0f);
				}
				break;

			}

		}
			
	}

	//击杀数量显示
	private void SetSkillData(int kill, int death, int assit)
	{
		killDataObj.transform.Find("tx_kill").GetComponent<Text>().text = ""+ kill;   	//击杀数
		killDataObj.transform.Find("tx_death").GetComponent<Text>().text = ""+ death;  	//死亡数
		killDataObj.transform.Find("tx_assit").GetComponent<Text>().text = ""+ assit;	//助攻数

	}


	void SendPing()
	{
		try
		{
			ping = new Ping(SavedData.s_instance.s_clientUrl);
		}
		catch (Exception ex) 
		{   
			Debug.Log("Exception "+ ex.Message + ", " + ex.GetType().FullName);
			//tellOnTableLoadErr();
		}   

	}
	//系统数据
	private void SetSystemInfoData()
	{
		int i = 0;
		systemInfoObj.transform.Find ("bg_battery/img_battery").GetComponent<Image> ().fillAmount = SystemInfo.batteryLevel;

		if (delayTime <= 60) {
			i = 3;
		} else if (delayTime <= 100) {
			i = 2;
		} else if (delayTime<= 140) {
			i = 1;
		} else {
			i = 0;
		}
		//信号图标
		signalObj.transform.Find ("img_signal" + i).gameObject.SetActive (true);

		systemInfoObj.transform.Find ("tx_signal").GetComponent<Text> ().text = delayTime.ToString() + "ms";

		string st_time = "";

		float time = Time.time - StartTime;
	

		if ((int)((300 - time) % 60)<10 && time<=300)
		{
			st_time = "0" + (int)((300 - time) / 60) + ":" + "0"+(int)((300 - time) % 60);
		}
		else if((int)((300 - time) % 60) >= 10 && time <= 300)
		{
			st_time = "0" + (int)((300 - time) / 60) + ":" + (int)((300 - time) % 60);
		}
		else if (time > 300)
		{
			st_time = "00:00";
			if(!isTure)
			{
				isTure = true;
				ActiveResultPanel ();
			}

		}

		tiemObj.GetComponent<Text> ().text = st_time;
		Invoke ("SetSystemInfoData", 1.0f);
	}

	bool isTure = false;


	public void SetSkillID(ValMagic val)
	{
		//icon
		gameUIObj.transform.Find ("btn_skill/img_skill").GetComponent<Image> ().sprite = ResourceMgr.Instance().Load<Sprite>("images/icon/" + val.icon, true);

		kill_cd = val.cd;
		//刷新cd 
		gameUIObj.transform.Find ("btn_skill/img_cd").GetComponent<Image> ().fillAmount = 0.0f;
	}


	//技能用完后
	public void ClearSkill()
	{
		gameUIObj.transform.Find ("btn_skill/img_skill").GetComponent<Image> ().sprite = ResourceMgr.Instance().Load<Sprite>("images/ui/tp_page_fight/fight_ban_jineng", true);
	}

	public void SetSkillCD(float cd)
	{
		gameUIObj.transform.Find ("btn_skill/img_cd").GetComponent<Image> ().fillAmount = cd;
	}

	public void SetFlashCD(float cd)
	{
		gameUIObj.transform.Find ("btn_flash/img_cd").GetComponent<Image> ().fillAmount = cd;
	}


	//激活死亡面板
	public void ActiveDeathPanel(string killer)
	{

		RespThirdUserData killerdata = null;
		RespThirdUserData bastdata = null;

		if (SavedData.s_instance.m_userCache.TryGetValue (SavedData.s_instance.m_userrank[0].m_uid, out bastdata)) {
			
		}

		if (SavedData.s_instance.m_userCache.TryGetValue (killer, out killerdata)) {

		}
		deathPanelObj.SetActive (true);
		deathPanelObj.transform.Find ("btn_relive").GetComponent<Button> ().onClick.AddListener (delegate {
			onClick (BTN_REVIVE);
		});
			
		bastplayerObj.transform.Find ("bg_userhead/img_userhead").GetComponent<Image> ().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+bastdata.head);
		bastplayerObj.transform.Find ("tx_userdata/tx_name").GetComponent<Text> ().text = "" + bastdata.nickname;
		bastplayerObj.transform.Find ("tx_userdata/tx_death").GetComponent<Text> ().text = "" + bastdata.death;
		bastplayerObj.transform.Find ("tx_userdata/tx_assit").GetComponent<Text> ().text = "" + bastdata.assit;

		killplayerObj.transform.Find ("bg_userhead/img_userhead").GetComponent<Image> ().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+killerdata.head);
		killplayerObj.transform.Find ("tx_userdata/tx_name").GetComponent<Text> ().text = "" + killerdata.nickname;
		killplayerObj.transform.Find ("tx_userdata/tx_death").GetComponent<Text> ().text = "" + killerdata.death;
		killplayerObj.transform.Find ("tx_userdata/tx_assit").GetComponent<Text> ().text = "" + killerdata.assit;

		deathPanelObj.transform.Find ("tx_message/tx_killer").GetComponent<Text> ().text = "" + killerdata.nickname;

	}
		

	//激活结算面板
	public void ActiveResultPanel()
	{
		if (gameUIObj == null || resultUIObj == null) {
			Debug.Log ("我也不知道怎么错了。。。。。。。。。");
		} else {
			gameUIObj.SetActive (false);
			resultUIObj.SetActive (true);
		}

		SavedData.s_instance.m_home = 1;


		RespThirdUserData self_data = null;
		//更新显示
		for(int i = 0; i<SavedData.s_instance.m_userrank.Count; i++) {
			RespThirdUserData data = null;
			if (SavedData.s_instance.m_userCache.TryGetValue (SavedData.s_instance.m_userrank[i].m_uid, out data)) {
				//SetResultTempData (i+1, data);
			}
			//是不是主角
			if (SavedData.s_instance.m_userrank [i].m_uid == SavedData.s_instance.m_user.m_uid) {
				self_data = data;
			}
		}

		#if false
		//头像
		result_oneObj.transform.Find("panel/panel_self/img_head").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+self_data.head);
		//KDA
		result_oneObj.transform.Find("panel/panel_self/tx_kda").GetComponent<Text>().text = ""+ self_data.kda;


		SetResultOneData (result_oneObj.transform.Find ("panel/panel_score").gameObject,true ,self_data.score, self_data.score);
		SetResultOneData (result_oneObj.transform.Find ("panel/panel_keepkill").gameObject,true , self_data.kill, self_data.kill);
		SetResultOneData (result_oneObj.transform.Find ("panel/panel_dead").gameObject, true ,self_data.death, self_data.death);
		SetResultOneData (result_oneObj.transform.Find ("panel/panel_kill").gameObject, true ,self_data.kill, self_data.kill);
		SetResultOneData (result_oneObj.transform.Find ("panel/panel_live").gameObject, true ,0, 0);
		SetResultOneData (result_oneObj.transform.Find ("panel/panel_time").gameObject, true ,0, 0);


		result_teamObj.transform.Find ("btn_one").GetComponent<Button> ().onClick.AddListener (() => {
			CheckResultPanel (true);
		});

		#endif
		//CheckResultPanel(true);

	}
	//设置个人页面数据
	public void SetResultOneData(GameObject parentObj,bool isasc,int count , int asc)
	{
		parentObj.transform.Find ("tx_count").GetComponent<Text> ().text = ""+count;
		if (isasc) {
			parentObj.transform.Find ("img_asc").gameObject.SetActive (true);
			parentObj.transform.Find ("img_desc").gameObject.SetActive (false);
			parentObj.transform.Find ("img_asc/tx_asc").GetComponent<Text> ().text = ""+asc;
		} else {
			parentObj.transform.Find ("img_asc").gameObject.SetActive (false);
			parentObj.transform.Find ("img_desc").gameObject.SetActive (true);
			parentObj.transform.Find ("img_desc/tx_desc").GetComponent<Text> ().text = ""+asc;
		}
	}

	//设置组队页面数据
	public void SetResultTempData(int no,RespThirdUserData data)
	{
		//Debug.Log ("SetResultTempData");
		GameObject itemObj = null;
		switch (no) {
		case 1:
			{
				itemObj = result_teamObj.transform.Find ("panel/Viewport/Contect/item0").gameObject;
			}
			break;
		case 2:
			{
				itemObj = result_teamObj.transform.Find ("panel/Viewport/Contect/item1").gameObject;
			}
			break;
		case 3:
			{
				itemObj = result_teamObj.transform.Find ("panel/Viewport/Contect/item2").gameObject;
			}
			break;
		default:
			{
				itemObj = result_teamObj.transform.Find ("panel/Viewport/Contect/item3").gameObject;
			}
			break;

		}

		//第四名及之后的名次
		if (no > 3) {
			GameObject go = GameObject.Instantiate(itemObj) as GameObject;
			go.transform.SetParent(itemObj.transform.parent);
			go.transform.localScale = Vector3.one;
			go.SetActive(true);
			go.transform.Find("tx_no").GetComponent<Text>().text = "" + no;
			go.transform.Find("img_head").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+data.head);
			go.transform.Find("tx_name").GetComponent<Text>().text = ""+data.nickname;
			go.transform.Find("tx_kill").GetComponent<Text>().text = ""+data.kill;
			go.transform.Find("tx_dead").GetComponent<Text>().text = ""+data.death;
			go.transform.Find("tx_assist").GetComponent<Text>().text = ""+data.assit;
			go.transform.Find("tx_score").GetComponent<Text>().text = ""+data.score;
			go.transform.Find("tx_kda").GetComponent<Text>().text = ""+data.kda;

		} else {
			//1-3名
			itemObj.transform.Find("img_head").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("images/ui/icon/General_icon","General_icon_"+data.head);
			itemObj.transform.Find("tx_name").GetComponent<Text>().text = ""+data.nickname;
			itemObj.transform.Find("tx_kill").GetComponent<Text>().text = ""+data.kill;
			itemObj.transform.Find("tx_dead").GetComponent<Text>().text = ""+data.death;
			itemObj.transform.Find("tx_assist").GetComponent<Text>().text = ""+data.assit;
			itemObj.transform.Find("tx_score").GetComponent<Text>().text = ""+data.score;
			itemObj.transform.Find("img_kda/tx_kda").GetComponent<Text>().text = ""+data.kda;
		}

	}

	//切换结算面板
	public void CheckResultPanel(bool isone)
	{
		if (!isone) {
			result_oneObj.SetActive (false);
			result_teamObj.SetActive (true);

		} else {
			result_oneObj.SetActive (true);
			result_teamObj.SetActive (false);
		}
	}




	void OnGUI()
	{
		switch(gameState)
		{
			case STATE_GAME:
			{

			}
			break;

			case STATE_MENU:
			{
				RenderMenu ();
			}
			break;

			case STATE_OPTION:
			{
				RenderOption ();
			}
			break;

			case STATE_HELP:
			{

			}
			break;

			case STATE_EXIT:
			{
				//Application.LoadLevel("menu");
				//ChatUIManager.Instance.OnLeav();
			}
			break;
		}	

		if (null != ping && ping.isDone)
		{
			delayTime = ping.time;
			ping.DestroyPing();
			ping = null;
			Invoke("SendPing", 1.0F);//每秒Ping一次
		}
	}

	//绘制主菜单界面
	void RenderMenu()
	{

		//游戏开始按钮
		if (GUI.Button(new Rect(width * 0.4f, height * 0.3f, width * 0.1f, height * 0.05f), "返回"))
		{
			gameState = STATE_GAME;
		}
		//游戏设置按钮
		if (GUI.Button(new Rect(width * 0.4f, height * 0.4f, width * 0.1f, height * 0.05f), "设置"))
		{
			gameState = STATE_OPTION;
		}

		//游戏帮助按钮
		if (GUI.Button(new Rect(width * 0.4f, height * 0.5f, width * 0.1f, height * 0.05f), "帮助"))
		{
			gameState = STATE_HELP;
		}

		//游戏退出按钮
		if (GUI.Button(new Rect(width * 0.4f, height * 0.6f, width * 0.1f, height * 0.05f), "退出"))
		{
			gameState = STATE_EXIT;
			//Application.LoadLevel("menu");

		}
	}

	//绘制游戏设置界面
	void RenderOption()
	{
		//GUI.skin = mySkin;
		//GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), textureBG);

		if (GUI.Button(new Rect(width * 0.4f, height * 0.3f, width * 0.1f, height * 0.05f), "music开关"))
		{
			Debug.Log("height" + height + "width" + width);
		}

		if (GUI.Button(new Rect(width * 0.4f, height * 0.4f, width * 0.1f, height * 0.05f), "fps显示"))
		{
			//isShowFPS = !isShowFPS;
		}

		if (GUI.Button(new Rect(width * 0.4f, height * 0.5f, width * 0.1f, height * 0.05f), "back"))
		{
			gameState = STATE_MENU;
		}
	}


	public void onClickListener()
	{
		
		gameUIObj.gameObject.transform.Find ("btn_fire").GetComponent<Button> ().onClick.AddListener (delegate {
			onClick (BTN_FIRE);
		});
		gameUIObj.gameObject.transform.Find("btn_skill").GetComponent<Button>().onClick.AddListener(delegate {
			onClick (BTN_SKILL);
		});
		gameUIObj.gameObject.transform.Find("btn_flash").GetComponent<Button>().onClick.AddListener(delegate {
			onClick (BTN_FLASH);
		});
		gameUIObj.gameObject.transform.Find("btn_set").GetComponent<Button>().onClick.AddListener(delegate {
			onClick (BTN_SET);
		});
		gameUIObj.gameObject.transform.Find("btn_chat").GetComponent<Button>().onClick.AddListener(delegate {
			onClick (BTN_CHAT);
		});
		gameUIObj.gameObject.transform.Find("switch_voice").GetComponent<Button>().onClick.AddListener(delegate {
			onClick (SWITCH_VOICE);
		});
		gameUIObj.gameObject.transform.Find("btn_voice").GetComponent<Button>().onClick.AddListener(delegate {
			onClick (BTN_VOICE);
		});

	}

	private void onClick(int id)
	{
		switch (id) {
		case BTN_FIRE:
			{
				isFire = true;
			}
			break;	
		case BTN_FLASH:
			{
				isFlash = true;
			}
			break;	
		case BTN_SKILL:
			{
				isSkill = true;
			}
			break;	
		
		case BTN_SET:
			{
				
				if(gameState != STATE_MENU)
				{
					gameState = STATE_MENU;
				}
				else
				{
					gameState = STATE_GAME;
				}
			}
			break;	
		case BTN_CHAT:
			{
				Debug.Log (5);
			}
			break;	
		case SWITCH_VOICE:
			{
				Debug.Log (6);
			}
			break;	

		case BTN_VOICE:
			{
				Debug.Log (7);
			}
			break;	

		case BTN_REVIVE:
			{
				deathPanelObj.SetActive (false);
				eventController.RePlayerRevive ();
			}
			break;	
			default:
			{

			}
			break;
		}
	}


	public bool InputFire()
	{
		bool isfire = isFire;
		isFire = false;
		return isfire;
	}


	public bool InputFlash()
	{
		bool isflash = isFlash;
		isFlash = false;
		return isflash;
	}

	public bool InputSkill()
	{
		bool isskill = isSkill;
		isSkill = false;
		return isskill;
	}
		


	//添加玩家数据
	public void InitUserList()
	{
		//更新显示
		for(int i = 0; i< SavedData.s_instance.m_userrank.Count; i++) {
			GameObject obj = RankObj.transform.Find ("item_"+i).gameObject;
			UIRankItem item = obj.AddComponent<UIRankItem>();
			//设置数据
			item.Refresh(SavedData.s_instance.m_userrank [i]);
		}

		#if false
		itemRankObjself =  RankObj.transform.Find ("item_self").gameObject;
		UIRankItem itemself = itemRankObjself.AddComponent<UIRankItem>();
		//设置数据
		itemself.Refresh(data);
		#endif
	}



	//更新排行榜
	public void UpdateUserList()
	{
		//更新数值
		foreach (RespThirdUserData data in SavedData.s_instance.m_userCache.Values)
		{
			UserRank rank = SavedData.s_instance.m_userrank.Find ((UserRank x) => x.m_nickname == data.nickname) as UserRank;
			rank.m_score = data.score;


		}

		//升序
		SavedData.s_instance.m_userrank.Sort(delegate(UserRank x, UserRank y)
			{
				if(y.m_score != x.m_score)
				{
					return y.m_score.CompareTo(x.m_score);
				}
				else
				{
					return y.m_score;
				}

			});
		
		//更新显示
		for(int i = 0; i<SavedData.s_instance.m_userrank.Count; i++) {
			#if false
			if(SavedData.s_instance.m_userrank[i].m_uid == SavedData.s_instance.m_user.m_uid)
			{
				SelfItem (SavedData.s_instance.m_userrank [i], i);
			}
			#endif
			SortItem (SavedData.s_instance.m_userrank [i], i);

		}

		if(bast_player != SavedData.s_instance.m_userrank [0].m_uid)
		{
			bast_player = SavedData.s_instance.m_userrank [0].m_uid;
			//SetArrowBast (map.GetPlayerObj (bast_player).transform);
		}
		//SetArrowBast (map.GetPlayerObj (SavedData.s_instance.m_userrank [0].m_uid).transform);
	}

	string bast_player = "";
	private void SortItem(UserRank data,int id)
	{
		GameObject obj = RankObj.transform.Find ("item_"+id).gameObject;
		UIRankItem item = obj.GetComponent<UIRankItem>();
		item.Refresh(data);
	}

	private void SelfItem(UserRank data,int id)
	{
		Debug.Log ("SelfItem........."+data.m_nickname + data.m_score);
		GameObject obj = RankObj.transform.Find ("item_self").gameObject;
		UIRankItem item = obj.GetComponent<UIRankItem>();
		item.Refresh(data);
	}
	

}


#if false

//根据名字修改界面图片
public void SetImg()
{
	Sprite[] sprites = Resources.LoadAll<Sprite>("");

	Transform[] ui;
	ui = GetComponentsInChildren<Transform>();
	foreach(Transform child in ui)
	{
		for(int i = 0; i<sprites.Length; i++)
		{
			if(child.name == sprites[i].name)
			{
				child.GetComponent<Image>().sprite = (Sprite)sprites[i];
			}
			//九宫格属性
			if(child.name == "list_img" || child.name == "info_img")
			{
				child.GetComponent<Image>().type = Image.Type.Sliced;
			}
		}
	}
}
#endif