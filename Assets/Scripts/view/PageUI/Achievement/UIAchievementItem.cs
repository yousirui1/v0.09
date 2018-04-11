using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using tpgm;
using SimpleJson;

/**************************************
*FileName: UIAchievementItem.cs
*User: ysr 
*Data: 2018/1/31
*Describe: 成就item逻辑处理和显示
**************************************/

public class UIAchievementItem : MonoBehaviour 
{
	public ValAchieve data = null;

	GameObject diffObj0;
	GameObject diffObj1;
	GameObject diffObj2;



	public void Refresh(ValAchieve val)
	{
		this.data = val;

		diffObj0 = this.transform.Find ("bg_difficulty/img_difficulty0").gameObject;
		diffObj1 = this.transform.Find ("bg_difficulty/img_difficulty1").gameObject;
		diffObj2 = this.transform.Find ("bg_difficulty/img_difficulty2").gameObject;

		setDiff (val.dif);

		string[] sArray=val.name.Split('·');


		this.transform.Find ("tx_title").GetComponent<Text> ().text = sArray [0]; 
		this.transform.Find ("tx_desc").GetComponent<Text> ().text = val.text;


		this.transform.Find ("img_icon").GetComponent<Image> ().sprite = ResourceMgr.Instance().Load<Sprite>("images/ui/icon/"+val.icon, false);

	}

	//设置难度星级
	private void setDiff(int dif)
	{
		diffObj0.SetActive (false);
		diffObj1.SetActive (false);
		diffObj2.SetActive (false);

		switch (dif) {
		case 1:
			{
				diffObj0.SetActive (true);
			}
			break;

		case 2:
			{
				diffObj0.SetActive (true);
				diffObj1.SetActive (true);
			}
			break;

		case 3:
			{
				diffObj0.SetActive (true);
				diffObj1.SetActive (true);
				diffObj2.SetActive (true);
			}
			break;
		}
	}



} 
