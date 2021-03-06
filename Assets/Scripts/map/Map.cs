using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SimpleJson;
using System;
using UnityEngine.UI;
using System.IO;
using System.Text;
using tpgm.UI;
using tpgm;
using LitJson;


public class Map : MonoBehaviour
{
	private List<GameObject> playerObjs = new List<GameObject> ();
	
	private GameObject playerObj = null;	
	
	//固定建筑
	private GameObject backgroupObj; 

	private GameObject itemMgrObj;

	private GameObject skillMgrObj;
	private GameObject skillPosObj;

	private GameObject playerMgrObj;

	public const string m_gameID = "1";

	ValTableCache valCache = SavedContext.s_valTableCache;

	private EventController eventController;

	private MainLooper m_initedLooper = MainLooper.instance();

	private	Shadow shadow; 		//影子算法

	void Awake()
	{
		valCache.markPageUseOrThrow<ValRoleBattle> (m_gameID, ConstsVal.val_role_battle);
		valCache.markPageUseOrThrow<ValNum> (m_gameID, ConstsVal.val_num);

		eventController = GameObject.Find ("EventController").GetComponent<EventController>() as EventController;
	}

	void Start()
	{
		shadow = new Shadow ();
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



	void OnDestroy()
	{
		valCache.unmarkPageUse (m_gameID, ConstsVal.val_role_battle);
		valCache.unmarkPageUse (m_gameID, ConstsVal.val_num);
		valCache.unmarkPageUse (m_gameID, ConstsVal.val_global);
	
		Destroy (this);
	}


	//初始化资源,动态资源,技能水晶
	public IEnumerator InitMap(string valFileName, List<int> skill_list,GameObject canvasObj,LoadingUIPage ivew)
	{
		List<ItemJs> items = null;
		string path = "";
		string text = "";

		HandlerMessage msg = null;

		valCache.markPageUseOrThrow<ValGlobal> (m_gameID, ConstsVal.val_global);

		if (valFileName.Contains ("map1")) {
			 ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map1", false);
		} else {
			backgroupObj = ResourceMgr.Instance ().CreateGameObject ("prefabs/maps/map2", false);

			GameObject map_part0 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/icon_100020", false);
			map_part0.name = "map_part0";
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_1);
			m_initedLooper.sendMessage(msg);
			yield return 0;

			GameObject map_part1 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/icon_100033", false);
			map_part1.name = "map_part1";
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_2);
			m_initedLooper.sendMessage(msg);
			yield return 0;

			GameObject map_part2 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/icon_100034", false);
			map_part2.name = "map_part2";
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_3);
			m_initedLooper.sendMessage(msg);
			yield return 0;

			//GameObject map_part3 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/icon_100035", false);
			//map_part3.name = "map_part3";
			//yield return 0;
			GameObject map_part4 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/icon_100036", false);
			map_part4.name = "map_part4";
			yield return 0;

			GameObject map_part5 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/part_5", false);
			map_part5.name = "map_part5";
			for (int i = 0; i <= 6; i++) {
				GameObject map_part5_new = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/part_5/part_5_"+i, false);
				map_part5.name = "map_part5_"+i;
				map_part5_new.transform.parent = map_part5.transform;
				yield return 0;
			}
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_4);
			m_initedLooper.sendMessage(msg);
			map_part5.transform.parent = backgroupObj.transform;

			GameObject map_part6 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/icon_100038", false);
			map_part6.name = "map_part6";
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_5);
			m_initedLooper.sendMessage(msg);
			yield return 0;

			GameObject map_part7 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/skillPos", false);
			map_part7.name = "skillPos";
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_9);
			m_initedLooper.sendMessage(msg);
			yield return 0;

			map_part0.transform.parent = backgroupObj.transform;
			map_part1.transform.parent = backgroupObj.transform;
			map_part2.transform.parent = backgroupObj.transform;
			//map_part3.transform.parent = backgroupObj.transform;
			map_part4.transform.parent = backgroupObj.transform;

			map_part6.transform.parent = backgroupObj.transform;
			map_part7.transform.parent = backgroupObj.transform;
		}

		backgroupObj.transform.parent = this.gameObject.transform;
		backgroupObj.transform.localPosition = Vector3.zero;
		backgroupObj.transform.localScale =  Vector3.one;
		backgroupObj.name = "map";

		try
		{
			path = SavedContext.getExternalPath("data/" + valFileName + ".json");
			text = File.ReadAllText(path, Encoding.UTF8);
			//items = SimpleJson.SimpleJson.DeserializeObject<List<ItemJs>>(text);
			items = JsonMapper.ToObject<List<ItemJs>>(text);

		}
		catch (IOException ex)
		{
			Debug.Log (ex);
		}

		itemMgrObj = this.gameObject.transform.Find ("ItemManage").gameObject;

		ItemVal item = new ItemVal(5, 10, 1);
		for(int i = 0; i<items.Count; i++)	
		{
			GameObject newObj = ResourceMgr.Instance().CreateGameObject("prefabs/rewards/icon_100002",true);
			newObj.transform.parent = itemMgrObj.transform;
			newObj.transform.localPosition = new Vector3(items[i].X, items[i].Y, 0);
			newObj.transform.localScale = 100 * Vector3.one;
			//设置水晶的数值
			newObj.AddComponent<Item>().init(item.score, item.rehp, 0,0,item.exp, 0);
			#if false
			if(items.Count % 100 == 0)
			{
				yield return 0;
			}
			#endif

		}
		yield return 0;

		Dictionary<int, ValGlobal> valDict = valCache.getValDictInPageScopeOrThrow<ValGlobal>(m_gameID, ConstsVal.val_global);

		skillMgrObj = this.gameObject.transform.Find ("SkillManage").gameObject;
		skillPosObj = this.gameObject.transform.Find ("map/skillPos").gameObject;

		for(int i = 0; i<skillPosObj.transform.childCount; i++)	
		{
			if (skill_list [i] != 0)
			{
				ValGlobal val = ValUtils.getValByKeyOrThrow(valDict, skill_list [i]);

				GameObject newObj = ResourceMgr.Instance().CreateGameObject("prefabs/icon/"+val.icon, true);
				newObj.transform.parent = skillMgrObj.transform;
				newObj.transform.position = skillPosObj.transform.GetChild (i).transform.position;
				newObj.transform.localScale = 100 * Vector3.one;
				//立即道具
				if (val.type == 5) {
					string[] sArray=val.add.Split(',');
					switch(val.id)
					{
					//面包
					case 100041:
						{
							newObj.AddComponent<Item>().init(0,Convert.ToInt32(sArray[0]),0,0,0,0);
						}
						break;
						//加速鞋
					case 100111:
						{
							newObj.AddComponent<Item>().init(0,0,0,Convert.ToInt32(sArray[0]),0,0);
						}
						break;
						//啤酒
					case 100112:
						{
							newObj.AddComponent<Item>().init(0,0,Convert.ToInt32(sArray[0]),0,0,0);
						}
						break;
					}
				}
				//技能
				else if (val.type == 6)
				{
					newObj.AddComponent<Item>().init(0,0,0,0,0,val.id);
				}
			
			}
			#if false
			if(skillPosObj.transform.childCount% 10 == 0)
			{
				yield return 0;
			}
			#endif
		
		}



		playerMgrObj = this.gameObject.transform.Find ("PlayerManage").gameObject;

		ivew.Hide ();
		UIRoot.Instance.gameObject.SetActive (false);
		canvasObj.SetActive (true);
		yield return 0;

	}






		
	//更新资源
	public void UpdateResouce()
	{


	}

	public GameObject GetPlayerObj(int id)
	{
		return playerObjs [id];
	}


	public GameObject GetPlayerObj(string uid)
	{
		for (int i = 0; i < playerMgrObj.transform.childCount; i++) {
			if (playerMgrObj.transform.GetChild (i).name == uid) {
				return playerMgrObj.transform.GetChild (i).gameObject;
			}
		}
		return null;
	}


	//添加一个玩家
	public void AddPlayerObj(int i, string name, float x, float y)
	{
		GameObject newObj = ResourceMgr.Instance().CreateGameObject("prefabs/heros/roles/role1", true);
		newObj.transform.parent = playerMgrObj.transform;
		newObj.name = name;
		newObj.transform.localScale = Vector3.one;
		Debug.Log (x + ":" +y);
		newObj.transform.localPosition = new Vector3 (x, y, 0);
		playerObjs.Insert (i, newObj);
	
		int other_group1 = eventController.GetCamp (SavedData.s_instance.m_user.m_uid);

		if (other_group1++ > 3) {
			other_group1 = 1;
		}

		int other_group2 = other_group1++;

		if (other_group2 > 3) {
			other_group2 = 1;
		}

		if(other_group1 == eventController.GetCamp(name))
		{
			
			newObj.transform.Find ("Info_bar/img_hp").GetComponent<Image> ().sprite = ResourceMgr.Instance ().Load<Sprite> ("images/heros/roles/status/group_2", false);
		}
		else if(other_group2 == eventController.GetCamp(name))
		{
			newObj.transform.Find ("Info_bar/img_hp").GetComponent<Image> ().sprite = ResourceMgr.Instance ().Load<Sprite> ("images/heros/roles/status/group_3", false);
		}
			

		RespThirdUserData data = null;
		if (SavedData.s_instance.m_userCache.TryGetValue (name, out data)) {
			newObj.transform.Find ("tx_nickname").GetComponent<Text> ().text = data.nickname;
		} else {
			Debug.Log ("m_userCache is null");
		}
	}


	
	//删除一个玩家
	public void DelPlayerObj(int i)
	{
		playerObjs.Remove (playerObjs[i]);
		Destroy(playerObjs[i]);
	}
	
	//结束以后清空玩家
	public void ClearPlayerObj()
	{
		for(int i = 0; i< playerObjs.Count ; i++)
		{
			Destroy(playerObjs[i]);
		}
		playerObjs.Clear ();
	}

	private int m_state = 0;
	//
	public void OnEvent(int id, PlayerVal playerVal)
	{
		if(null != playerObjs[id])
		{

			iTween.MoveTo(playerObjs[id], iTween.Hash("x", (int)playerVal.x,
				"y", (int)playerVal.y,
                                           "z", 0,
                                           "time", 1.0,
                                           "islocal", true
             )); 
		
		

			if (System.Math.Abs(playerVal.fdx) >= 0.5 || System.Math.Abs(playerVal.fdy) >= 0.5) {

				if(playerVal.skill == 500)
				{
					shadow.craft_flash (playerVal,1,true);
				}


				//保存当前最后的面向
				playerVal.old_dx = playerVal.fdx;
				playerVal.old_dy = playerVal.fdy;

				//释放技能
				if(playerVal.skill != 0)
				{   
					skillMgrObj.GetComponent<SkillManage> ().onFire (playerVal.skill, 1, playerVal.fdx , playerVal.fdy, playerObjs [id], playerVal.uid);
				}

			} 
			else {
				if(playerVal.skill == 500)
				{
					shadow.craft_flash (playerVal,1 ,false);
				}

				//释放技能
				if(playerVal.skill != 0)
				{   
					skillMgrObj.GetComponent<SkillManage> ().onFire (playerVal.skill, 1, playerVal.old_dx, playerVal.old_dy,playerObjs [id], playerVal.uid);
				}
			}
			//( int d,  int skill,GameObject playerObj)
			skillMgrObj.GetComponent<SkillManage> ().onAnimator (playerVal.fdx,playerVal.fdy, playerVal.skill, playerObjs [id]);
		}
	}

	private void OnMove(GameObject playObj, Vector3 oriVec, Vector3 endVec)
	{
		playObj.transform.Translate (endVec - oriVec);
	}



}
