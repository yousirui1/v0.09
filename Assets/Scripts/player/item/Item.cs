using UnityEngine;
using System.Collections;
using tpgm;


public class Item : MonoBehaviour
{
    public int score { get; set; } //得分
    public int rehp { get; set; } //回血
	public int resp { get; set; } //回蓝
	public int speed { get; set; } //加速
    public int exp  { get; set; } //经验
	public int skill  { get; set; } //技能

	EventController eventController;

	void Start()
	{
		eventController = GameObject.Find ("EventController").GetComponent<EventController> () as EventController;
	}


	public void init(int score, int rehp, int resp, int speed, int exp ,int skill)
	{
		this.score = score;
		this.rehp = rehp;
		this.resp = resp;
		this.speed = speed;
		this.exp = exp;
		this.skill = skill;
	}
		
    private void OnTriggerEnter2D(Collider2D other)
    {
		
        if (other.tag == Tags.player )
        {
			if (skill != 0) {
				//魔法瓶
				if (other.name == SavedData.s_instance.m_user.m_uid) {
					eventController.SetSkillID (skill);
				}
			} else {
				//及时道具
				other.GetComponent<ATKAndDamage> ().TakeItem (rehp, resp, speed ,score, exp);
				Destroy (this.gameObject);
			}
        }
    }



}