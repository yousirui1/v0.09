using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using System;
using UnityEngine.UI;
using System.IO;
using System.Text;
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

	ValTableCache valCache;

	void Start()
	{
		valCache = SavedContext.s_valTableCache;
		valCache.markPageUseOrThrow<ValGlobal> (m_gameID, ConstsVal.val_global);
		valCache.markPageUseOrThrow<ValRoleBattle> (m_gameID, ConstsVal.val_role_battle);
		valCache.markPageUseOrThrow<ValNum> (m_gameID, ConstsVal.val_num);
	
		InitMap (SavedData.s_instance.m_map.file, SavedData.s_instance.m_map.skill_list);
	}


	//初始化资源,动态资源,技能水晶
	public void InitMap(string valFileName, List<int> skill_list)
	{
		if (valFileName.Contains ("map1")) {
			backgroupObj = ResourceMgr.Instance ().CreateGameObject ("prefabs/maps/map1", false);
		} else {
			backgroupObj = ResourceMgr.Instance ().CreateGameObject ("prefabs/maps/map2", false);
		}

		backgroupObj.transform.parent = this.gameObject.transform;
		backgroupObj.transform.localPosition = Vector3.zero;
		backgroupObj.transform.localScale =  Vector3.one;
		backgroupObj.name = "map";
		string path = "";
		string text = "";
		try
		{
			path = SavedContext.getExternalPath("data/" + valFileName + ".json");
			text = File.ReadAllText(path, Encoding.UTF8);
		}
		catch (IOException ex)
		{
			Debug.Log (ex);
		}

		List<ItemJs> items = SimpleJson.SimpleJson.DeserializeObject<List<ItemJs>>(text);

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
		}

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
		}

		playerMgrObj = this.gameObject.transform.Find ("PlayerManage").gameObject;

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
