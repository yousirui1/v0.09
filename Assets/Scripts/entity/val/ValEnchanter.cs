using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**************************************
*FileName: ValStore.cs
*User: ysr 
*Data: 2018/1/30
*Describe: 商城json文件对应的类
**************************************/

namespace tpgm
{
	[Serializable]
	public class ValEnchanter : BaseVal
	{
		public string name = "";
		public int type;
		public string sid = "";
		public int sum;
		public string text = "";
		public int exp;
	}
}
