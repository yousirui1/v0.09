using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**************************************
*FileName: ValSignInAdd.cs
*User: ysr 
*Data: 2018/1/30
*Describe: 7天签到载入json文件对应的类
**************************************/

namespace tpgm
{
	[Serializable]
	public class ValSignInAdd : BaseVal
	{
		public string day = "";
		public string reward = ""; //奖励编号
		public string icon = "";   //奖励图标
	}
}
