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
	public class ValNum : BaseVal
	{
		public string key = "";
		public int num; 	//经验
		public string text = "";		//普攻伤害

	}
}
