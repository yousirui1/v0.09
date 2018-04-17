using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using tpgm;
using SimpleJson;

/**************************************
*FileName: UIPackageItem.cs
*User: ysr 
*Data: 2018/2/5
*Describe: 法师item逻辑处理和显示
**************************************/

public class UIPackageItem : MonoBehaviour 
{
	public ValEnchanter data = null;

	public string icon = "";


	public void Refresh(ValEnchanter val,string icon)
	{
		this.data = val;
		//Debug.Log (val.icon);
		this.icon = icon;

		if (val.type == 17) {
			//签名版
		} else {
			//Debug.Log (val.type);
			this.transform.Find ("img_item").GetComponent<Image> ().sprite = ResourceMgr.Instance ().Load<Sprite> ("images/icon/"+icon, false);
			this.transform.Find ("bg_progress/img_progress").GetComponent<Image> ().fillAmount = 0.0f;
		}
	}



} 
