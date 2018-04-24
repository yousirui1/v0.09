using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using tpgm;
using UnityEngine.UI;

/**************************************
*FileName: PlayerATKAndDamage.cs
*User: ysr 
*Data: 2017/12/12
*Describe: 玩家攻击和伤害计算脚步
**************************************/

public class PlayerATKAndDamage : ATKAndDamage{
	private GameObject roleObj = null;


	private int othergroup = 0;
	void Start()
	{
		roleObj = this.transform.Find ("role").gameObject;

	}
	//public string uid = "";
	public void Hide()
	{
		if (this.name != SavedData.s_instance.m_user.m_uid) {
			if(this.gameObject.activeInHierarchy)
			{
				this.gameObject.SetActive (false);
			}
		} else {
			roleObj.transform.GetComponent<SpriteRenderer> ().color = new Color (255, 255, 255, 0.43f);
		}

	}

	public void Show()
	{
		if(this.name != SavedData.s_instance.m_user.m_uid)
		{
			if(this.gameObject.activeInHierarchy)
			{
				this.gameObject.SetActive (true);
			}


		}
		else {
			roleObj.transform.GetComponent<SpriteRenderer> ().color = new Color (255, 255, 255, 1.0f);
		}
	}



}


#if false
void FixedUpdate()
{
switch (m_state) {
case 0:
{

}
break;

case 1:
{
m_sprite.transform.eulerAngles = new Vector3 (0, 0, 0);
m_frame++;
if (m_frame >= m_clips.Length)
m_frame = 0;
m_sprite.sprite = m_clips [m_frame];
}
break;

case 2:
{
m_sprite.transform.eulerAngles = new Vector3 (0, 180f, 0);
m_frame++;
if (m_frame >= m_clips.Length)
m_frame = 0;
m_sprite.sprite = m_clips [m_frame];
}
break;

}

}
#endif