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




	public void Refresh(ValEnchanter val)
	{
		this.data = val;
		//Debug.Log (val.icon);

		if (val.type == 17) {
			//签名版
		} else {
			Debug.Log (val.type);
			this.transform.Find ("img_item").GetComponent<Image> ().sprite = ResourceMgr.Instance ().Load<Sprite> ("images/ui/icon/icon_100192", false);
			this.transform.Find ("bg_progress/img_progress").GetComponent<Image> ().fillAmount = 0.0f;
		}
	}



} 
