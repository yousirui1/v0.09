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
	public class ValAstrology : BaseVal
	{
		public int sid ;
		public int type;
		public string name = "";
		public int num1;
		public int num2;
		public int eWeight1;
		public int eWeight2;

	}
}
