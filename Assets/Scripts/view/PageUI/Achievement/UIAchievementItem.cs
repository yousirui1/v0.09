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




	public void Refresh(ValAchieve val)
	{
		this.data = val;
		//this.transform.Find("item_bg").GetComponent<Image>().sprite = "累计"+ val.day+"日登录奖励";
		//this.transform.Find("img_type").GetComponent<Image>().sprite = "累计"+ val.day+"日登录奖励";
		//this.transform.Find ("tx_itemhead").GetComponent<Text> ().text = val.name;
		//this.transform.Find("img_materials").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+0);

		//this.transform.Find ("").GetComponent<Text> ().text = val.text;



	}



} 
