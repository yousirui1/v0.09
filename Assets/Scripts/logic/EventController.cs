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

	private FollowPlayer followPlayer; //跟随玩家脚本

	private CArrowLockAt cArrowLockAt; //箭头指示

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
	ValTableCache valCache = SavedContext.s_valTableCache;

	//缓存标识
	public const string m_gameID = "1";

	//上次接收的时间
	float lastRecvInfoTime = float.MinValue;
	float updateRecvInfoTime = float.MinValue;

	void Awake()
	{
	}

	// Use this for initialization
	void Start () {
		
	}

	public void InitObj(GameObject canvasObj)
	{
		shadow = new Shadow();
		this.canvasObj = canvasObj;
		this.canvasObj.SetActive (false);

	
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

		//cArrowLockAt = gameMenu.gameObject.AddComponent<CArrowLockAt> ();

		//UI相机渲染
		cameraObj = canvasObj.transform.Find ("UICamera").gameObject;
		cameraObj.transform.parent = gameMenu.transform;
		//localposition 父物体为坐标轴
		cameraObj.transform.localPosition = new Vector3(0,0,-100);
		cameraObj.transform.localScale = Vector3.one;

		m_msgHandlerProxy = new MessageHandlerProxy(handleMsg);
	}

	public void  InitMap(string valFileName, List<int> skill_list, LoadingUIPage ivew)
	{
		StartCoroutine(map.InitMap (valFileName, skill_list,canvasObj,ivew));

	}
		
	void OnDestroy()
	{
		valCache.unmarkPageUse(m_gameID, ConstsVal.val_magicup);

		SavedData.s_instance.m_userCache.Clear ();
		SavedData.s_instance.m_userrank.Clear ();
		SavedData.s_instance.m_userlist.Clear ();
		ev_ClearPlayer ();

		Destroy (this);
	}


	public void InitJoyControl()
	{
		joyControl = gameMenu.transform.Find ("GameUI/JoyControl").gameObject.GetComponent<JoyControl> ();
		areaConect = new GameObject("NetController").AddComponent<AreaConect> ();
		areaConect.transform.parent = canvasObj.transform;
	}

	public GameObject GetCanvas()
	{
		return canvasObj;
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
			players [id].dx = (int)(horizontal * 100);
			players [id].dy = (int)(vertical * 100);
			players [id].fdx = horizontal ;
			players [id].fdy = vertical;


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
						if(players [id].skill >= 14 && players [id].skill <=19)
						{
							SavedData.s_instance.m_isMove = false;
							Invoke ("OnFlashEnd",2.0f);
						}
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
			players [id].v = (int)gameObj.GetComponent<PlayerATKAndDamage> ().speed;
			players [id].hp =(int) gameObj.GetComponent<PlayerATKAndDamage> ().hp;
			players [id].sp =(int) gameObj.GetComponent<PlayerATKAndDamage> ().sp;
			players [id].lev = gameObj.GetComponent<PlayerATKAndDamage> ().level;
			players [id].score = gameObj.GetComponent<PlayerATKAndDamage> ().score;

			Vector2 vec = new Vector2 (players[id].dx,players[id].dy);

			//Debug.Log (players [id].x + ":" + players [id].y);

			if(players[id].skill == 500 )
			{
				SavedData.s_instance.m_isMove = false;
				Invoke ("OnFlashEnd",0.8f);
			}
				
			//发射射线判断障碍物
			RaycastHit2D hit = Physics2D.Raycast(gameObj.transform.position, vec, (players [id].v * 0.03f + 1.0f), 1<<LayerMask.NameToLayer("barrier"));
			//Debug.DrawLine (gameObj.transform.position, hit.point);

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
	public void ev_Output(List<PlayerVal> buf)
	{
		bool isFind = false;

		lastRecvInfoTime = Time.time;
		 

		for(int i = 0; i<buf.Count; i++)
		{
			//接收数据的时间间隔
			if (Time.time - updateRecvInfoTime > 1f) {
				//用户积分更新
				UpdateUserData (buf[i].uid,buf[i].score,false,false,false); 
				updateRecvInfoTime = Time.time;
			}
				
			//在用户列表查找
			if (SavedData.s_instance.m_userlist.Contains (buf[i].uid)) {
				//更新其他玩家的信息
				if (buf [i].uid != SavedData.s_instance.m_user.m_uid) {	
					if (buf [i].dx != 0 || buf [i].dy != 0) {
						players [i].old_dx = buf [i].dx;
						players [i].old_dy = buf [i].dy;
					}
					//保存接受到其他玩家的数据
					players [i].x = buf [i].x;
					players [i].y = buf [i].y;
					players [i].v = buf [i].v;
					players [i].dx = buf [i].dx;
					players [i].dy = buf [i].dy;
					players [i].sp = buf [i].sp;
					players [i].hp = buf [i].hp;

					players [i].fdx = players [i].dx / 100.0f;
					players [i].fdy = players [i].dy / 100.0f;

					players [i].skill = buf [i].skill;
					players [i].score = buf [i].score;
					players [i].lev = buf [i].lev;

					//接收数据的时间间隔
					if (Time.time - lastRecvInfoTime > 0.2f) {
						//延迟大于多少秒做延迟补偿
						Debug.Log (Time.time - lastRecvInfoTime);

						shadow.shadow_refresh (players [i]);
						shadow.shadow_move (players [i], 1);
						shadow.trace (1, players [i], 1);
					}


					map.GetPlayerObj (i).GetComponent<PlayerATKAndDamage> ().hp = buf [i].hp;
					map.GetPlayerObj (i).GetComponent<PlayerATKAndDamage> ().sp = buf [i].sp;
					map.GetPlayerObj (i).GetComponent<PlayerATKAndDamage> ().level = buf [i].lev;
					map.OnEvent (i, players [i]);

				}
			}else {
					//添加新玩家
					ev_AddPlayer (buf [i], i);
					map.AddPlayerObj (i, buf[i].uid, buf[i].x, buf[i].y);

					//判断是否是队友设置箭头指向队友
					if (IsSameCamp (SavedData.s_instance.m_user.m_uid, buf [i].uid )) {
						//gameMenu.SetArrowGroup (map.GetPlayerObj (i).transform);
					}
					SavedData.s_instance.m_userlist.Add(buf[i].uid);
					if (SavedData.s_instance.m_user.m_uid == buf[i].uid) {
						id = i;
						followPlayer.SetUid (map.GetPlayerObj (i));
						//gameMenu.SetArrowSelf (map.GetPlayerObj (i).transform);
					}
				}
			}
	}

		
	public Map GetMap()
	{
		return map;
	}

	#if false
	private PlayerVal MoveData2PlayerVal(MoveData data)
	{
		PlayerVal val = new PlayerVal ();
		val.x = data.x;
		return val;
	}
	#endif
		


		
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
		newplayer.dx = 0;
		newplayer.dy = 0;
		newplayer.v = 10;
	
		newplayer.sx = newplayer.x;
		newplayer.sy = newplayer.y;
		newplayer.sdx = newplayer.dx;
		newplayer.sdy = newplayer.dy;
		newplayer.sv = newplayer.v;
		//Debug.Log (i);
		players.Insert (i, newplayer);
		//players.Add(newplayer);

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


	public void ev_GameOver()
	{
		gameMenu.ActiveResultPanel ();
		areaConect.setGameOver ();
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
	
		map.GetPlayerObj (uid).GetComponent<PlayerATKAndDamage> ().isInvincible = true;
		map.GetPlayerObj (uid).GetComponent<PlayerATKAndDamage> ().invincible_buffer_time = 2.0f;
		#if false
		players [count].hp =(int) map.GetPlayerObj (uid).GetComponent<PlayerATKAndDamage> ().hp_Max;

		players [count].sp =(int) map.GetPlayerObj (uid).GetComponent<PlayerATKAndDamage> ().sp_Max;
		map.GetPlayerObj (uid).GetComponent<PlayerATKAndDamage> ().hp = players [count].hp;
		map.GetPlayerObj (uid).GetComponent<PlayerATKAndDamage> ().sp = players [count].sp;

		map.GetPlayerObj (uid).SetActive (true);
		#endif
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
