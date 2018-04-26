using UnityEngine;
using System.Collections;
using tpgm;
using System;

/**************************************
*FileName: SkillBallistic.cs
*User: ysr 
*Data: 2018/3/15
*Describe: 弹道技能持续飞行
**************************************/

public class SkillBallistic  : MonoBehaviour {
 
	public int hp { get; set; } //伤害最大值

	public string uid { get; set;} //技能所有者

	public int skillID { get ; set;} //技能id

	private Vector3 skill_vec = new Vector3(0,0,0);

	private float instantiateTime = 0.0f;

	EventController eventController;

	void Start()
	{
		instantiateTime = Time.time;
	}

	void Update()
	{
		this.transform.localPosition += skill_vec;
	}

	public void init(string uid, ValMagic val , float dx ,float dy ,EventController eventController)
	{
		//技能施法者
		this.uid = uid;

		this.eventController = eventController;

		//伤害最大值
		this.hp = val.hurt;

		this.skillID = val.id;

		Vector3 dVec = new Vector3 (dx, dy, 0);
		Debug.Log (val.speed);
		skill_vec = dVec * val.speed;

		float angle = Vector3.Angle(new Vector3(1,0,0),dVec);

		if (dy < 0) {
			angle = -angle;
		}
		this.transform.Rotate(0,0,angle);

		Destroy(this.gameObject, val.duration/1000f);
	}

	#if false
	public void init(int hp, float time, float dx ,float dy ,  int skillID, string uid,EventController eventController)
	{
		//技能施法者
		this.uid = uid;
		//伤害最大值
		this.hp = hp;

		this.eventController = eventController;

		this.skillID = skillID;

		Vector3 dVec = new Vector3 (dx, dy, 0);
		skill_vec = dVec * 20.0f;

		float angle = Vector3.Angle(new Vector3(1,0,0),dVec);

		if (dy < 0) {
			angle = -angle;
		}
		this.transform.Rotate(0,0,angle);

		Destroy(this.gameObject, time);
	}
	#endif

	private void OnTriggerEnter2D(Collider2D other)
	{
		
		if (other.tag == Tags.player && other.name != uid)
		{
			//Debug.Log (other.name);
			//判断是否一个队
			if(eventController.IsSameCamp(other.name, uid))
			{
				other.GetComponent<ATKAndDamage> ().TakeDamage (GetAtt(),uid,eventController);
				Destroy (this.gameObject);
			}
		}

		if(other.gameObject.layer == Layers.barrier)
		{
			Destroy (this.gameObject);
		}
	}


	//计算伤害
	private float GetAtt()
	{
		float att = hp;
		if(skillID == 2)
			att = hp - (Time.time - instantiateTime) * 40;
		return att;
	}
}
