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


	public void init(int hp, float time, int d ,int skillID, string uid,EventController eventController)
	{
		//技能施法者
		this.uid = uid;
		//伤害最大值
		this.hp = hp;

		this.eventController = eventController;

		this.skillID = skillID;

		switch (d) {
		case 0:
			{
				skill_vec = new Vector3(-20,0,0);
				this.transform.Rotate(0, 0, 180);
			}
			break;

		case 1:
			{
				skill_vec = new Vector3(-20,0,0);
				this.transform.Rotate(0, 0, 180);
			}
			break;


		case 2:
			{
				skill_vec = new Vector3(-10,10,0);
				this.transform.Rotate(0, 0, 135);
			}
			break;

		case 3:
			{
				skill_vec = new Vector3(0,20,0);
				this.transform.Rotate(0, 0, 90);
			}
			break;

		case 4:
			{
				skill_vec = new Vector3(10,10,0);
				this.transform.Rotate(0, 0, 45);
			}
			break;

		case 5:
			{
				skill_vec = new Vector3(20,0,0);
				this.transform.Rotate(0, 0, 0);
			}
			break;

		case 6:
			{
				skill_vec = new Vector3(10,-10,0);
				this.transform.Rotate(0, 0, -45);
			}
			break;

		case 7:
			{
				skill_vec = new Vector3(0,-20,0);
				this.transform.Rotate(0, 0, -90);
			}
			break;

		case 8:
			{
				skill_vec = new Vector3(-10,-10,0);
				this.transform.Rotate(0, 0, -135);
			}
			break;

		}
		Destroy(this.gameObject, time);
	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == Tags.player && other.name != uid)
		{
			//判断是否一个队
			//if(eventController.IsSameCamp(other.name, uid))
			{
				other.GetComponent<ATKAndDamage> ().TakeDamage (GetAtt(),uid,eventController);
				Destroy (this.gameObject);
			}
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
