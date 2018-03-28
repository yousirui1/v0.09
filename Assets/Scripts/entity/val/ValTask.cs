using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**************************************
*FileName: ValTask.cs
*User: ysr 
*Data: 2018/1/30
*Describe: 任务json文件对应的类
**************************************/

namespace tpgm
{
	[Serializable]
	public class ValTask : BaseVal
	{
		public string name = "";
		public string reward = "";
		public int type;
		public int count;
		public string text = "";
	}
}
