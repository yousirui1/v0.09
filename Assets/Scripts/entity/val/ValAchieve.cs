using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**************************************
*FileName: ValAstrology.cs
*User: ysr 
*Data: 2018/1/30
*Describe: 占星json文件对应的类
**************************************/

namespace tpgm
{
	[Serializable]
	public class ValAchieve : BaseVal
	{
		public string name ="";
		public string icon = "";
		public int type;
		public string condition = "";
		public int count;
		public string text = "";
		public string reward = "";
		public int dif;

	}
}
