using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using System;
using UnityEngine.UI;
using System.IO;
using System.Text;
using tpgm.UI;
using tpgm;


public class Map : MonoBehaviour
{
	
	//private GameObject[] playerObjs = new GameObject[SavedData.s_instance.m_playerMax];

	private List<GameObject> playerObjs = new List<GameObject> ();
	
	private GameObject playerObj;	
	
	//固定建筑
	private GameObject backgroupObj; 

	private GameObject itemMgrObj;

	private GameObject skillMgrObj;
	private GameObject skillPosObj;

	private GameObject playerMgrObj;

	public const string m_gameID = "1";

	ValTableCache valCache = SavedContext.s_valTableCache;


	private MainLooper m_initedLooper = MainLooper.instance();

	void Start()
	{
		valCache.markPageUseOrThrow<ValRoleBattle> (m_gameID, ConstsVal.val_role_battle);
		valCache.markPageUseOrThrow<ValNum> (m_gameID, ConstsVal.val_num);
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

	//初始化资源,动态资源,技能水晶
	public IEnumerator InitMap(string valFileName, List<int> skill_list,GameObject canvasObj,StartUIPage ivew)
	{

		//HandlerMessage msg = MainLooper.obtainMessage(handleMsgDispatch, MSG_Load_OVER);
		//msg.m_dataObj = data;
		//m_initedLooper.sendMessage(msg);
		Debug.Log ("InitMap");
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
			map_part0.name = "icon_100020";
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_1);
			m_initedLooper.sendMessage(msg);
			yield return 0;

			GameObject map_part1 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/icon_100033", false);
			map_part1.name = "icon_100033";
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_2);
			m_initedLooper.sendMessage(msg);
			yield return 0;

			GameObject map_part2 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/icon_100034", false);
			map_part2.name = "icon_100034";
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_3);
			m_initedLooper.sendMessage(msg);
			yield return 0;
			GameObject map_part3 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/icon_100035", false);
			map_part3.name = "icon_100035";
			yield return 0;
			GameObject map_part4 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/icon_100036", false);
			map_part4.name = "icon_100037";
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
			map_part6.name = "icon_100038100020";
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_5);
			m_initedLooper.sendMessage(msg);
			yield return 0;

			GameObject map_part7 = ResourceMgr.Instance ().CreateGameObject ("prefabs/map/map2/other/skillPos", false);
			map_part7.name = "skillPos";
			msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_6);
			m_initedLooper.sendMessage(msg);
			yield return 0;

			map_part0.transform.parent = backgroupObj.transform;
			map_part1.transform.parent = backgroupObj.transform;
			map_part2.transform.parent = backgroupObj.transform;
			map_part3.transform.parent = backgroupObj.transform;
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
			items = SimpleJson.SimpleJson.DeserializeObject<List<ItemJs>>(text);

		}
		catch (IOException ex)
		{
			Debug.Log (ex);
		}

		itemMgrObj = this.gameObject.transform.Find ("ItemManage").gameObject;

		ItemVal item = new ItemVal(5, 10, 1);
		Debug.Log (items.Count);
		for(int i = 0; i<items.Count; i++)	
		{
			GameObject newObj = ResourceMgr.Instance().CreateGameObject("prefabs/rewards/icon_100002",true);
			newObj.transform.parent = itemMgrObj.transform;
			newObj.transform.localPosition = new Vector3(items[i].X, items[i].Y, 0);
			newObj.transform.localScale = 100 * Vector3.one;
			//设置水晶的数值
			newObj.AddComponent<Item>().init(item.score, item.rehp, 0,0,item.exp, 0);
			if(items.Count % 100==0)
			{
				yield return 0;
			}

		}
		s
		msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_7);
		m_initedLooper.sendMessage(msg);

		Dictionary<int, ValGlobal> valDict = valCache.getValDictInPageScopeOrThrow<ValGlobal>(m_gameID, ConstsVal.val_global);

		skillMgrObj = this.gameObject.transform.Find ("SkillManage").gameObject;
		skillPosObj = this.gameObject.transform.Find ("map/skillPos").gameObject;
		Debug.Log (skillPosObj.transform.childCount);

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
			if(skillPosObj.transform.childCount% 10 == 0)
			{
				yield return 0;
			}
		}

		msg = MainLooper.obtainMessage(ivew.handleMessage, MSG_LOAD_PART_8);
		m_initedLooper.sendMessage(msg);

		Debug.Log ("Init End");
		playerMgrObj = this.gameObject.transform.Find ("PlayerManage").gameObject;

		UIRoot.Instance.gameObject.SetActive (false);
		canvasObj.SetActive (true);

		yield return 0;

	}





	void Destroy()
	{
		valCache.unmarkPageUse(m_gameID, ConstsVal.val_global);
		valCache.unmarkPageUse(m_gameID, ConstsVal.val_role_battle);
		valCache.unmarkPageUse(m_gameID, ConstsVal.val_num);
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
	public void AddPlayerObj(int i, string name, int x, int y)
	{
		GameObject newObj = ResourceMgr.Instance().CreateGameObject("prefabs/heros/roles/role1", true);
		newObj.transform.parent = playerMgrObj.transform;
		newObj.name = name;
		newObj.transform.localScale = Vector3.one;
		newObj.transform.localPosition = new Vector3 (x, y, 0);
		playerObjs.Insert (i, newObj);

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
		for(int i = 0; i< SavedData.s_instance.m_playerMax; i++)
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
			iTween.MoveTo(playerObjs[id], iTween.Hash("x", playerVal.x,
                                           "y", playerVal.y,
                                           "z", 0,
                                           "time", 1.0,
                                           "islocal", true
             )); 
			if (playerVal.d != 0) {
				//保存当前最后的面向
				playerVal.old_d = playerVal.d;

				//释放技能
				if(playerVal.skill != 0)
				{   
					skillMgrObj.GetComponent<SkillManage> ().onFire (playerVal.skill, 1, playerVal.d, playerObjs [id], playerVal.uid);
				}

			} 
			else {
				
				//释放技能
				if(playerVal.skill != 0)
				{   
					skillMgrObj.GetComponent<SkillManage> ().onFire (playerVal.skill, 1, playerVal.old_d, playerObjs [id], playerVal.uid);
				}
			}
			//( int d,  int skill,GameObject playerObj)
			skillMgrObj.GetComponent<SkillManage> ().onAnimator (playerVal.d, playerVal.skill, playerObjs [id]);
		
		}
	}


}
