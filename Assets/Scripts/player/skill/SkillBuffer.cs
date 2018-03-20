using UnityEngine;
using System.Collections;
using tpgm;
using System;

/**************************************
*FileName: SkillAoe.cs
*User: ysr 
*Data: 2017/3/15
*Describe: 范围性技能跟随人物
**************************************/

public class SkillBuffer : MonoBehaviour {


	public int hp { get; set; } //回血

	public string uid { get; set;} //技能所有者

	public int skillID { get; set;} //技能所有者

	//技能当前时间
	private float instantiateTime = 0.0f;

	//技能持续总时间时间
	private float durationTime = 0.0f;

	EventController eventController;

	private int skillCount = 0;

	void Start()
	{
		instantiateTime = Time.time;
	}

	public void init(int hp, float time, int skillID, string uid, EventController eventController )
	{
		//技能施法者
		this.uid = uid;
		//伤害最大值
		this.hp = hp;
		this.skillID = skillID;
		this.eventController = eventController;
		this.skillID = skillID;

		this.durationTime = time;

		//单人buffer
		Destroy(this.gameObject, time);

	}




	//计算攻击力
	private float GetAtt()
	{
		float att = 100 - (Time.time - instantiateTime) * 40;
		if(att < 1)
			att = 1;
		return att;
	}


	private void OnTriggerEnter2D(Collider2D collider)
	{
		
	
	}

	void OnTriggerStay2D(Collider2D collider)
	{
		Debug.Log ("Buffer"+collider.name);
		//单人buffer 执行一次
		if (collider.name == uid && skillCount == 0) {
			collider.GetComponent<ATKAndDamage> ().TakeBuffer (2, durationTime);
			skillCount++;
		}
	}



	#if false

	void OnTriggerStay2D(Collider2D collider)
	{
		if (collider.tag == Tags.player && collider.name != uid)
		{
			//Debug.Log ();
			//判断是否一个队
			//if(eventController.IsSameCamp(other.name, uid))
			{
				//other.GetComponent<ATKAndDamage> ().TakeDamage (10, 0);
				//Destroy(this.gameObject);
			}

			//collider.GetComponent<ATKAndDamage> ().TakeDamage (-5);

			switch(skillID)
			{
			case 8:
				{
					if (Time.time - instantiateTime >0.2) {
						instantiateTime = Time.time;
						collider.GetComponent<ATKAndDamage> ().TakeDamage (2, uid ,eventController);
					}
				}
				break;
			case 14: 
				{
					if ( this.durationTime == Time.time - instantiateTime) {
						collider.GetComponent<ATKAndDamage> ().TakeBuffer (1, this.durationTime);
					}

				}
				break;

			}



		}
	}
	void OnTriggerExit2D(Collider2D collider)
	{


	}
	#endif

}
