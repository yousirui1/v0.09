using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using tpgm;
using SimpleJson;

/**************************************
*FileName: UIHeadImgItem.cs
*User: ysr 
*Data: 2018/1/2
*Describe: 图标item逻辑处理和显示
**************************************/

public class UIHeadImgItem : MonoBehaviour 
{
	public int data = 0;

	public void Refresh(int  id)
	{
		this.data = id;
		this.transform.Find("img_head").GetComponent<Image>().sprite = TextureManage.getInstance().LoadAtlasSprite("RawImages/Public/Atlases/Icon/General_icon","General_icon_"+id);

	}



} 
