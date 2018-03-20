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
	public class ValStore : BaseVal
	{
		public string name = "";
		public string icon = "";
		public string reward = "";
		public int pay_type;
		public int price;
		public int classify;
		public int buy_type;
		public string start = "";
		public int validity;
	}
}
