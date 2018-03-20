using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using tpgm;

/**************************************
*FileName: GameMenu.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 游戏界面按钮和菜单
**************************************/


public class GameMenu : MonoBehaviour
{
	GameObject joyObj;			//遥感
	GameObject gameUIObj;		
	GameObject deathPanelObj;  //死亡面板

	GameObject broadcastObj;   //广播通知

	GameObject tiemObj;			//倒计时

	GameObject systemInfoObj;   //系统信息
	GameObject signalObj;   //系统信息

	GameObject killDataObj;		//击杀数量

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
	GameObject itemObj0;
	GameObject itemObj1;
	GameObject itemObj2;
	GameObject itemObj3;


	GameObject itemRankObjself;
	GameObject RankObj;


	//数值表缓存
	ValTableCache valCache;

	public const string m_gameID = "1";

	private int kill_cd = 0;

	Ping ping;
	float delayTime;

	EventController eventController;


	void Start()
	{
		Init ();
		Invoke ("InitUserList", 10);
		onClickListener ();
		SendPing();
	}

	void Init()
	{

		valCache = SavedContext.s_valTableCache;
		valCache.markPageUseOrThrow<ValMagic> (m_gameID, ConstsVal.val_magic);

		gameUIObj = ResourceMgr.Instance ().CreateGameObject ("prefabs/gameUI/GameUI", false);
		gameUIObj.transform.parent = this.transform;
		gameUIObj.transform.localPosition = Vector3.zero;
		gameUIObj.transform.localScale = Vector3.one;


		joyObj = gameUIObj.transform.Find ("JoyControl").gameObject;

		deathPanelObj = gameUIObj.transform.Find ("bg_death").gameObject;
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

		RankObj = gameUIObj.transform.Find ("bg_rank").gameObject;



		height = Screen.height;
		width = Screen.width;

		SetSkillData (0,0,0);
		SetSystemInfoData ();
	}

	void Destroy()
	{
		valCache.unmarkPageUse(m_gameID, ConstsVal.val_magic);
	}

	//击杀提示
	public  void SetBroadcastData(PlayData data)
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
		ping = new Ping(SavedData.s_instance.s_clientUrl);
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

		float time = Time.timeSinceLevelLoad;

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
		}
		tiemObj.GetComponent<Text> ().text = st_time;
		Invoke ("SetSystemInfoData", 1.0f);
	}



	public JoyControl GetJoyControl()
	{
		if(joyObj.GetComponent<JoyControl> () == null)
		joyObj.AddComponent<JoyControl> ();
		return joyObj.GetComponent<JoyControl> ();
	}

	public void SetSkillID(int id)
	{
		
		int magic_id = 0;
		Dictionary<int, ValMagic> valDict = valCache.getValDictInPageScopeOrThrow<ValMagic>(m_gameID, ConstsVal.val_magic);
		ValMagic val = ValUtils.getValByKeyOrThrow(valDict, id);

		Debug.Log (val.icon);
		//icon
		gameUIObj.transform.Find ("btn_skill/img_skill").GetComponent<Image> ().sprite = ResourceMgr.Instance().Load<Sprite>("images/icon/" + val.icon, true);

		kill_cd = val.cd;

		//刷新cd 
		gameUIObj.transform.Find ("btn_skill/img_cd").GetComponent<Image> ().fillAmount = 0.0f;
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
	public void SetDeathPanel()
	{
		deathPanelObj.SetActive (true);
		deathPanelObj.transform.Find ("").GetComponent<Button> ().onClick.AddListener (delegate {
			onClick (BTN_REVIVE);
		});

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
				Application.LoadLevel("menu");
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
				gameState = STATE_MENU;
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
				eventController.PlayerRevive ();
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
		for(int i = 0; i<SavedData.s_instance.m_userrank.Count; i++) {
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
		foreach (UserData data in SavedData.s_instance.m_userCache.Values)
		{
			UserRank rank = SavedData.s_instance.m_userrank.Find ((UserRank x) => x.m_uid == data.nickname) as UserRank;
			rank.m_score = data.score;
		}

		//升序
		SavedData.s_instance.m_userrank.Sort(delegate(UserRank x, UserRank y)
			{
				return y.m_score.CompareTo(x.m_score);
			});
		
		//更新显示
		for(int i = 0; i<SavedData.s_instance.m_userrank.Count; i++) {
			SortItem (SavedData.s_instance.m_userrank [i], i);
		}
	}

	private void SortItem(UserRank data,int id)
	{
		GameObject obj = RankObj.transform.Find ("item_"+id).gameObject;
		UIRankItem item = obj.GetComponent<UIRankItem>();
		Debug.Log (data.m_uid);
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