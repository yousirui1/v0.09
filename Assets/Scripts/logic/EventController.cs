using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using tpgm;

/**************************************
*FileName: EventController.cs
*User: ysr 
*Data: 2018/2/28
*Describe: 游戏事件处理主要数值计算
**************************************/

public class EventController : MonoBehaviour {
	
	private Map map;
	private GameObject joyObj;		//遥感
	private GameMenu gameMenu;	//菜单


	private SkillManage skillManage;
	private GameObject itemManage;
	private GameObject playerManage;

	private GameObject canvasObj;
	private GameObject cameraObj; //ui相机
	private GameObject touchObj;

	private FollowPlayer followPlayer;

	private JoyControl joyControl;
	
	private	Shadow shadow; 		//影子算法

	private AreaConect areaConect;
	
	private int id = 0;			//主角对应的玩家id

	// 0站立 1行走 2攻击1 3 攻击2 
	private int iAnimationState = 0;

	private int iFire = 0;      //普攻 
	private int iSkill = 0;     //技能
	private int iFlash = 0;     //闪现


	/*			 3			
	 * 		  2	   4	
	 * 		1		 5
	 * 		  8    6
	 *           7
	 */

	//人物移动方向  参考上图
	private int[,] direction = new int[9, 2] { {0,0 },{0, -1},{1, -1},{1, 0},{1, 1},
											   {0, 1},{-1, 1},{-1, 0},{-1, -1}};
	public const int BTN_FIRE = 1;
	public const int BTN_SKILL = 2;
	public const int BTN_FLASH = 3;


	//普攻
	public float lastFireTime = 0;
	//普攻的时间间隔
	private float fireInterval = 0.5f;

	//闪现的时间
	public float lastFlashTime = 0;
	//闪现时间间隔
	private float flashInterval = 3.0f;

	//技能的时间
	public float lastSkillTime = 0;
	//技能的时间间隔
	private float skillInterval = 3.0f;

	//所以玩家的数据
	private List<PlayerVal> players = new List<PlayerVal>();

	//数值表缓存
	ValTableCache valCache;

	//缓存标识
	public const string m_gameID = "1";

	//上次接收的时间
	float lastRecvInfoTime = float.MinValue;


	// Use this for initialization
	void Start () {
		shadow = new Shadow();
		canvasObj = GameObject.Find ("GameRoot").gameObject;

		valCache = SavedContext.s_valTableCache;
		valCache.markPageUseOrThrow<ValMagicUp> (m_gameID, ConstsVal.val_magicup);

		//地图
		map = new GameObject("Map").AddComponent<Map>();
		map.transform.parent = canvasObj.transform;
		map.transform.localPosition = new Vector3(0,0,-100);
		map.transform.localScale = Vector3.one;

		//技能
		skillManage = new GameObject("SkillManage").AddComponent<SkillManage>();
		skillManage.transform.parent = map.transform;
		skillManage.transform.localPosition = Vector3.zero;
		skillManage.transform.localScale = Vector3.one;

		//资源
		itemManage = new GameObject ("ItemManage");
		itemManage.transform.parent = map.transform;
		itemManage.transform.localPosition = Vector3.zero;
		itemManage.transform.localScale = Vector3.one;

		//玩家
		playerManage = new GameObject ("PlayerManage");
		playerManage.transform.parent = map.transform;
		playerManage.transform.localPosition = Vector3.zero;
		playerManage.transform.localScale = Vector3.one;

		//菜单
		gameMenu = new GameObject("GameUI").AddComponent<GameMenu>();
		gameMenu.transform.parent = canvasObj.transform;
		gameMenu.transform.localScale = Vector3.one;
		gameMenu.transform.localPosition = Vector3.zero;
		followPlayer = gameMenu.gameObject.AddComponent<FollowPlayer> ();

		//UI相机渲染
		cameraObj = canvasObj.transform.Find ("UICamera").gameObject;
		cameraObj.transform.parent = gameMenu.transform;
		//localposition 父物体为坐标轴
		cameraObj.transform.localPosition = new Vector3(0,0,-100);
		cameraObj.transform.localScale = Vector3.one;

		areaConect = GameObject.Find ("NetController").GetComponent<AreaConect> ();

		m_msgHandlerProxy = new MessageHandlerProxy(handleMsg);

		Invoke ("InitJoyControl", 0.5f);

	}
		
	void OnDestroy()
	{
		valCache.unmarkPageUse(m_gameID, ConstsVal.val_magicup);
	}


	void InitJoyControl()
	{
		joyControl = gameMenu.transform.Find ("GameUI/JoyControl").gameObject.GetComponent<JoyControl> ();
	}


	public void SetSkillID(int id)
	{
		int magic_id = 0;
		Dictionary<int, ValMagicUp> valDict = valCache.getValDictInPageScopeOrThrow<ValMagicUp>(m_gameID, ConstsVal.val_magicup);
		for (int i = 1; i <= valDict.Count; i++) {
			ValMagicUp val = ValUtils.getValByKeyOrThrow(valDict, i);
			if (val.bottle == id) {
				magic_id = val.magic_id;
				break;
			}
		}
			
		//技能升级
		if (magic_id == SavedData.s_instance.m_skillID) {
			if(SavedData.s_instance.m_skillLevel == 0)
			{
				magic_id += 1;
			}
			else if(SavedData.s_instance.m_skillLevel == 1)
			{
				magic_id += 2;
			}
			SavedData.s_instance.m_skillLevel++;

		} else {
			//替换掉当前技能
			SavedData.s_instance.m_skillID = magic_id;
			SavedData.s_instance.m_skillLevel = 0;
		}
		//刷新技能的使用次数
		SavedData.s_instance.m_skillCount = 3;
		gameMenu.SetSkillID (magic_id);
	}




	//主角输入	
	public PlayerVal ev_Input()
	{
		
		float vertical;	
		float horizontal;

		//遥感
		if (null == joyControl) {
			return null;
		}
		//
	
		//获取遥感数据
		if(joyControl.isMove)
		{
			vertical = joyControl.vertical;
			horizontal = joyControl.horizontal;
		}		
		//获取键盘数据
		else
		{
			vertical = Input.GetAxis("Vertical");
			horizontal = Input.GetAxis("Horizontal");
		}	
			
		if(id < players.Count && players[id] != null)
		{
			
			//获取移动方向
			for(int i =0; i <9 ;i++)
			{
				if(direction[i, 0] == vertical && direction[i, 1] == horizontal)
				{
					players[id].d = i;
					break;
				}
			}
			//普攻
			if(gameMenu.InputFire() || Input.GetKey(KeyCode.J))
			{
				if (Time.time - lastFireTime > fireInterval) {
					players[id].skill = 1;
					lastFireTime = Time.time;
				}
			}

			if (Time.time - lastFlashTime > flashInterval) {
				//闪现
				if(gameMenu.InputFlash() || Input.GetKey(KeyCode.K))
				{
					if (map.GetPlayerObj (id).GetComponent<PlayerATKAndDamage> ().useFlash (-50)) 
					{
						players[id].skill = 500;
						lastFlashTime = Time.time;
					}
				}
			}else {
				gameMenu.InputFlash ();
				gameMenu.SetFlashCD (1 - (Time.time - lastFlashTime) / flashInterval);
			}

			if (Time.time - lastSkillTime > skillInterval) {
				
				if (SavedData.s_instance.m_skillCount > 0) {
					//技能
					if(gameMenu.InputSkill() || Input.GetKey(KeyCode.L))
					{
						players [id].skill = SavedData.s_instance.m_skillID;
						SavedData.s_instance.m_skillCount--;
						lastSkillTime = Time.time;
					}
				} else {
					gameMenu.ClearSkill ();
				}


			}else {
				gameMenu.InputSkill ();
				gameMenu.SetSkillCD (1 -(Time.time - lastSkillTime) / skillInterval);
			}
				


			GameObject gameObj = map.GetPlayerObj (id);


			//获取玩家的数据
			players [id].v =(int) gameObj.GetComponent<PlayerATKAndDamage> ().speed;
			players [id].hp =(int) gameObj.GetComponent<PlayerATKAndDamage> ().hp;
			players [id].sp =(int) gameObj.GetComponent<PlayerATKAndDamage> ().sp;
			players [id].lev =(int) gameObj.GetComponent<PlayerATKAndDamage> ().level;
			players [id].score =(int) gameObj.GetComponent<PlayerATKAndDamage> ().score;

	
			Vector2 vec = new Vector2 (0,0);

			switch (players[id].d) {
			case 1:
				{
					vec = new Vector2 (-1.0f, 0);
				}
				break;
			case 2:
				{
					vec = new Vector2 (-1.0f,1.0f);
				}
				break;
			case 3:
				{
					vec = new Vector2 (0,1.0f);
				}
				break;
			case 4:
				{
					vec = new Vector2 (1.0f,1.0f);
				}
				break;
			case 5:
				{
					vec = new Vector2 (1.0f,0);
				}
				break;
			case 6:
				{
					vec = new Vector2 (1.0f,-1.0f);
				}
				break;
			case 7:
				{
					vec = new Vector2 (0,-1.0f);
				}
				break;
			case 8:
				{
					vec = new Vector2 (-1.0f,-1.0f);
				}
				break;
			}

			if(players[id].skill == 500 )
			{
				shadow.craft_flash (players [id], 1);
				SavedData.s_instance.m_isMove = false;
				Invoke ("OnFlashEnd",0.8f);
			}


			//发射射线判断障碍物
			RaycastHit2D hit = Physics2D.Raycast(gameObj.transform.position, vec, (players [id].v * 0.03f + 1.0f), 1<<LayerMask.NameToLayer("barrier"));

			if (hit.collider == null && SavedData.s_instance.m_isMove) {
				shadow.craft_move (players [id], 1);
			} else if (hit.collider != null && SavedData.s_instance.m_isMove){
				if ((hit.collider.transform.position.x - gameObj.transform.position.x) * vec.x < 0)
					shadow.craft_move (players [id], 1);
				if ((hit.collider.transform.position.y - gameObj.transform.position.y) * vec.y < 0)
					shadow.craft_move (players [id], 1);
			}
			//对事件处理
			map.OnEvent(id, players[id]);

			//返回通过网络发送给服务器
			return players[id];

		}
		return null;
	}

	void OnFlashEnd()
	{
		players [id].v = 20;
		SavedData.s_instance.m_isMove = true;

	}



	//处理服务端发送过来的数据
	public void ev_Output(FrameBuf buf)
	{
		bool isFind = false;

		lastRecvInfoTime = Time.time;

		for(int i = 0; i<buf.data.Count; i++)
		{

			if (buf.data [i].skill != 0) {
				Debug.Log (buf.data [i].skill);
			}
			//用户积分更新
			UpdateUserData (buf.data[i].uid,buf.data[i].score,false,false,false); 
			//在用户列表查找
			foreach(string name in SavedData.s_instance.m_userlist)
			{
				isFind = false;
				if(buf.data[i].uid == name)
				{
					isFind = true;
                    //判断是否为主角用户
                    //shadow.shadow_refresh(buf.data[i]);

					if (buf.data [i].uid != SavedData.s_instance.m_user.m_uid)
					{	
						if (buf.data[i].d != 0) {
							players [i].old_d = buf.data [i].d;
						}
						//保存接受到其他玩家的数据
						players [i].x = buf.data [i].x;
						players [i].y = buf.data [i].y;
						players [i].v = buf.data [i].v;
						players [i].d = buf.data [i].d;
						players [i].sp = buf.data [i].sp;
						players [i].hp = buf.data [i].hp;

						players [i].skill = buf.data [i].skill;
						players [i].score = buf.data [i].score;
						players [i].lev= buf.data [i].lev;

						//接收数据的时间间隔
						if (Time.time - lastRecvInfoTime > 0.2f) {
							//延迟大于多少秒做延迟补偿
							Debug.Log (Time.time - lastRecvInfoTime);

							shadow.shadow_refresh(players [i]);
							shadow.shadow_move (players [i], 1);
							shadow.trace (1, players [i], 1);
						}

						map.GetPlayerObj (i).GetComponent<PlayerATKAndDamage> ().hp = buf.data [i].hp;
						map.GetPlayerObj (i).GetComponent<PlayerATKAndDamage> ().sp = buf.data [i].sp;
						map.GetPlayerObj (i).GetComponent<PlayerATKAndDamage> ().level = buf.data [i].lev;
						map.OnEvent (i, players [i]);


					}
						break;
				}	
			}
			//没有在用户表中查找到用户则添加新用户
			if(!isFind)
			{
				ev_AddPlayer (buf.data [i], i);
				map.AddPlayerObj (i, buf.data [i].uid, buf.data[i].x, buf.data[i].y);
				SavedData.s_instance.m_userlist.Add(buf.data[i].uid);
				if (SavedData.s_instance.m_user.m_uid == buf.data[i].uid) {
					id = i;
					followPlayer.SetUid (SavedData.s_instance.m_user.m_uid);
				}
			}
		}
	}
		

	//建立玩家字典数据
	public void ev_InitPlayer(List<NewUser> newUser)
	{
		for (int i = 0; i < newUser.Count; i++) {
			RespThirdUserData data = new RespThirdUserData ();
			data.nickname = newUser [i].nickname;
			data.score = 0;
			data.kill = 0;
			data.death = 0;
			data.assit = 0;
			data.group = newUser [i].group;
			data.head = newUser [i].head;

			if (!SavedData.s_instance.m_userCache.ContainsKey (newUser [i].uid)) {
				SavedData.s_instance.m_userCache.Add (newUser [i].uid, data);
				UserRank rank = new UserRank (newUser [i].uid, newUser [i].nickname,0);
				SavedData.s_instance.m_userrank.Add (rank);
			}


		}


	}
		
	//更新玩家数据字典
	private void UpdateUserData(string uid, int score,bool iskill, bool isdeath, bool isassit)
	{
		RespThirdUserData data = null;
		if(SavedData.s_instance.m_userCache.TryGetValue(uid, out data))
		{
			if(iskill)
			{
				data.kill++;
			}

			if(isdeath)
			{
				data.death++;
			}

			if(isassit)
			{
				data.assit++;
			}
			if (score != 0) {
				data.score = score;
				gameMenu.UpdateUserList ();
			}
			if (data.death == 0) {
				data.kda = 0;
			} else {
				data.kda = (data.kill + data.assit) / data.death;
			}
		}
	}

	public void ev_AddPlayer(PlayerVal player, int i)
	{
		PlayerVal newplayer = new PlayerVal();
		newplayer.uid = player.uid;
		newplayer.x = player.x;
		newplayer.y = player.y;
		newplayer.d = 0;
		newplayer.v = 20;
		newplayer.sx = newplayer.x;
		newplayer.sy = newplayer.y;
		newplayer.sd = newplayer.d;
		newplayer.sv = newplayer.v;
		players.Insert (i, newplayer);
	}

	public void ev_DelPlayer(int i)
	{
		players.Remove (players [i]);
		map.DelPlayerObj(i);
	}

	//游戏结束后清空
	public void ev_ClearPlayer()
	{
		players.Clear ();
		map.ClearPlayerObj ();
	}

	//获取阵营,0标识错误;
	public int GetCamp(string uid)
	{
		RespThirdUserData data = null;
		if(SavedData.s_instance.m_userCache.TryGetValue(uid, out data))
		{
			return int.Parse (data.group.Substring (data.group.Length - 1));
		}
		return 0;

	}

	//是否同一阵营
	public bool IsSameCamp(string uid1, string uid2)
	{
		return GetCamp(uid1) == GetCamp(uid2);
	}

	//玩家死亡
	public void PlayerDead(Queue<string> queue_assit,string kill_user) 
	{
		KillData killData = new KillData ();
		List<string> list_assit = new List<string> ();
		foreach (string st in queue_assit) {
			if (st != kill_user) {
				list_assit.Add (st);
			}
		}

		killData.roomNum = SavedData.s_instance.m_roomNum;
		killData.kill = kill_user;
		killData.dead = SavedData.s_instance.m_user.m_uid;
		killData.assists = list_assit;

		string stObj = SimpleJson.SimpleJson.SerializeObject (killData);


		//打开死亡面板
		gameMenu.ActiveDeathPanel (kill_user);

		//发送给网络
		areaConect.onPomeloEvent_Dead (stObj);

	}

	public class AttackNum
	{
		public string dead = "";
		public List<int> type;
		public string kill = "";
		public List<string> assists;
	}


	public void ev_OutTip(RespThirdPlayData data)
	{

		if (null != data.attackNum) {
			//击杀数据存储在用户字典里
			UpdateUserData (data.attackNum.kill, 0, true, false, false); //击杀者
			UpdateUserData (data.attackNum.dead, 0, false, true, false); //死亡者

			for (int i = 0; i < data.attackNum.assists.Count; i++) {
				UpdateUserData (data.attackNum.assists [i], 0, false, false, true); //助攻
			}
			//击杀显示在页面上
			gameMenu.SetBroadcastData (data);
		}
		if (!data.revive.Equals (string.Empty)) {
			//Debug.Log (data.revive);
			PlayerRevive (data.revive);
		}

	}
	public MessageHandlerProxy m_msgHandlerProxy;


	//玩家请求复活
	public void RePlayerRevive() 
	{
		Debug.Log ("RePlayerRevive");
		areaConect.onPomeloEvent_Revive();
	}
	//玩家复活
	public void PlayerRevive(string uid) 
	{
		MainLooper looper = MainLooper.instance ();
		int count = 0;
		for (int i = 0; i < SavedData.s_instance.m_userlist.Count; i++) {
			if (uid == SavedData.s_instance.m_userlist [i])
				count = i;
		}
		map.GetPlayerObj (count).GetComponent<PlayerATKAndDamage> ().state = 3;
		map.GetPlayerObj (count).GetComponent<PlayerATKAndDamage> ().buffer_time = 2.0f;

		players [count].hp =(int) map.GetPlayerObj (count).GetComponent<PlayerATKAndDamage> ().hp_Max;

		players [count].sp =(int) map.GetPlayerObj (count).GetComponent<PlayerATKAndDamage> ().sp_Max;
		map.GetPlayerObj (count).GetComponent<PlayerATKAndDamage> ().hp = players [count].hp;
		map.GetPlayerObj (count).GetComponent<PlayerATKAndDamage> ().sp = players [count].sp;

		map.GetPlayerObj (count).SetActive (true);
		//复活之后频闪
		#if false
		GameObject roleObj = map.GetPlayerObj (count).transform.Find ("role").gameObject;

		HandlerMessage msg1 = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 1);
		msg1.m_dataObj = (object)roleObj;
		looper.postMessageDelay (msg1, 500);

		HandlerMessage msg2 = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 2);
		msg1.m_dataObj = (object)roleObj;
		looper.postMessageDelay (msg2, 1000);

		HandlerMessage msg3 = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 3);
		msg1.m_dataObj = (object)roleObj;
		looper.postMessageDelay (msg3, 1500);

		HandlerMessage msg4 = MainLooper.obtainMessage (m_msgHandlerProxy.handleMessage, 4);
		msg1.m_dataObj = (object)roleObj;
		looper.postMessageDelay (msg4, 2000);
		#endif


	}

	private void looper_CheckAlpha(int fream, object obj)
	{
		GameObject roleObj = (GameObject)obj;
		Debug.Log (roleObj.name);
		#if false
		if (fream % 2 == 0) {
			roleObj.GetComponent<SpriteRenderer> ().color = new Color (roleObj.GetComponent<SpriteRenderer> ().color.a,roleObj.GetComponent<SpriteRenderer> ().color.b,roleObj.GetComponent<SpriteRenderer> ().color.g,0);

		} else {
			roleObj.GetComponent<SpriteRenderer> ().color = new Color (roleObj.GetComponent<SpriteRenderer> ().color.a,roleObj.GetComponent<SpriteRenderer> ().color.b,roleObj.GetComponent<SpriteRenderer> ().color.g,1.0f);
		}
		#endif
	}

	void handleMsg(HandlerMessage msg)
	{
		looper_CheckAlpha (msg.m_what,msg.m_dataObj);

	}



	public class KillData
	{
		public string roomNum = "";
		public string kill = "";
		public string dead = "";
		public List<string> assists;
	
	}
}
