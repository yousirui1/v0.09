using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using System;
using tpgm;
using System.Threading;
/**************************************
*FileName: SkillManage.cs
*User: ysr 
*Data: 2017/12/12
*Describe: 技能管理脚本Event触发
**************************************/
public class SkillManage : MonoBehaviour {
	
    string skilljson ;
  
    
	List<SkillVal> skills = new List<SkillVal>();
    int id = 0;

    GameObject bulletObj;

	GameObject skillanimation;

	public const int BTN_FIRE = 1;
	public const int BTN_FLASH = 2;
	public const int BTN_SKILL = 3;

	//数值表缓存
	ValTableCache valCache;

	public const string m_gameID = "2";

	private MainLooper m_initedLooper;

	EventController eventController;

    void Start()
    {
        //读入所以技能的数值表
		//valCache = SavedContext.s_valTableCache;
		//valCache.markPageUseOrThrow<ValMagicUp> (m_gameID, ConstsVal.val_magicup);

		m_initedLooper = MainLooper.instance();

		if(null == m_initedLooper)
		{
			throw new NullReferenceException("MainLooper not inited");
		}
		eventController = GameObject.Find("EventController").GetComponent<EventController> ();
      	
    }

	void Destroy()
	{
		//valCache.unmarkPageUse(m_gameID, ConstsVal.val_magicup);
	}

	void OnFlashEnd()
	{
		GameObject player = GameObject.Find (SavedData.s_instance.m_skillUid).gameObject;
		foreach (Transform child in player.transform)
		{
			child.gameObject.SetActive (true);
		}
	}

	IEnumerator WaitCallback(int time)  
	{  
		yield return new WaitForSeconds(time);  

	}  


    //释放技能 1 普攻 2 闪现
	public void onFire(int type, int level, int d, GameObject playerObj ,string uid)
    {
		if (type != 0) {
			if (type == 1) {
				//(-100, 0) (0,30)  (50, 0) (50, -50) (50, -100)   (0 -120)  (-100 -100)  (-100, -50)
				//普攻
				GameObject newbullet = ResourceMgr.Instance().CreateGameObject("prefabs/skills/skill_1",true);
				newbullet.transform.parent = playerObj.transform.parent;
				newbullet.transform.position = playerObj.transform.position;
				newbullet.transform.localScale = 100 * Vector3.one;
				newbullet.AddComponent<SkillBallistic>().init(-10, 3.0f,d, type,uid, eventController);

			} else if (type == 500) {


				SavedData.s_instance.m_skillUid = uid;
				foreach (Transform child in playerObj.transform)
				{
					child.gameObject.SetActive (false);
				}
				Invoke ("OnFlashEnd", 1);
			} else {
				string st = "";
				Transform portTra = findChild (playerObj, "port");
				GameObject newbullet;

				if (type >= 2 && type <= 7) {
					st = "2-6";			//龙息术
				}
				else if(type>=8 && type <=13)
				{
					st = "8-13";		//水神祝福
				}
				else if(type>=14 && type <=19)
				{
					st = "14-19";		//霜之哀伤
				}

				else if(type>=20 && type <=25)
				{
					st = "20-25";		//狂怒荆棘
				}

				else if(type>= 26&& type <=31)
				{
					st = "26-31";		//龙卷风
				}

				else if(type>= 32&& type <= 37)
				{
					st = "32-37";		//土遁术
				}
					
				switch (st) {
				case "2-6":
					{
						//龙息术
						newbullet = ResourceMgr.Instance().CreateGameObject("prefabs/skills/skill_2",true);
						newbullet.transform.parent = portTra;
						newbullet.transform.localPosition = portTra.localPosition;
						newbullet.transform.localScale = Vector3.one;
						newbullet.AddComponent<SkillBallistic>().init(-10, 3.0f,d, type,uid,eventController);
					}
					break;
				case "8-13":
					{
						//水神祝福
						newbullet = ResourceMgr.Instance().CreateGameObject("prefabs/skills/skill_8",true);
						newbullet.transform.parent = portTra;
						newbullet.transform.localPosition = new Vector3(portTra.localPosition.x -30 ,portTra.localPosition.x -40,portTra.localPosition.z);
						newbullet.transform.localScale = 100 *Vector3.one;
						newbullet.AddComponent<SkillAoe>().init(10, 0.75f, type, uid, eventController);//0.75
					}
					break;

				case "14-19":
					{
						//霜之哀伤
						newbullet = ResourceMgr.Instance().CreateGameObject("prefabs/skills/skill_14",true);
						newbullet.transform.parent = portTra;
						newbullet.transform.localPosition = portTra.localPosition;
						newbullet.transform.localScale = Vector3.one;
						newbullet.AddComponent<SkillAoe>().init(10, 2.0f, type,uid, eventController);
					}
					break;
				case "20-25":
					{
						//狂怒荆棘
						newbullet = ResourceMgr.Instance().CreateGameObject("prefabs/skills/skill_20",true);
						newbullet.transform.parent = playerObj.transform.parent;
						newbullet.transform.position = portTra.position;
						newbullet.transform.localScale = 100 * Vector3.one;
						//newbullet.AddComponent<Skill>().init(10,0,0 , 3.0f,d, uid);
						Destroy(newbullet, 1.2f);

					}
					break;
				case "26-31":
					{
						//龙卷风
						newbullet = ResourceMgr.Instance().CreateGameObject("prefabs/skills/skill_26",true);
						newbullet.transform.parent = portTra;
						newbullet.transform.localPosition = portTra.localPosition;
						newbullet.transform.localScale = Vector3.one;
						//newbullet.AddComponent<Skill>().init(10,0,0 , 3.0f,d, uid);
						Destroy (newbullet, 0.75f);
					}
					break;

				case "32-37":
					{
						//土遁术
						newbullet = ResourceMgr.Instance().CreateGameObject("prefabs/skills/skill_32",true);
						newbullet.transform.parent = portTra;
						newbullet.transform.localPosition = portTra.localPosition;
						newbullet.transform.localScale = 100 * Vector3.one;
						//newbullet.AddComponent<SkillBuffer>().init(0, 3.0f, type,uid, eventController);
						Destroy (newbullet, 3.0f);
						playerObj.GetComponent<ATKAndDamage> ().TakeBuffer (3, 3.0f);
					}
					break;
				}
	
			}
		}
	
	}

	//查找玩家身上的子物体,射击点和动画位置
	Transform findChild(GameObject playerObj, string name)
	{
		Transform[] tranchilds = playerObj.GetComponentsInChildren<Transform>();

		foreach(Transform child in tranchilds)
		{
			if(child.name == name)
			{
				return child;
			}
		}	
		return null;
	}

}