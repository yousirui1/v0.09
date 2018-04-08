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
	public class ValMagic : BaseVal
	{
		public string name = "";
		public int grade;
		public int aim;
		public int center;
		public int radius;
		public int speed;
		public int dist;
		public int hurt_type;
		public int hurt_interval;
		public int hurt;
		public int delay_time;
		public int delay_interval;
		public int cd;
		public int count;
		public int bookSid;
		public int debuff_type;
		public int debuff_time;
		public int debuff_interval;
		public int debuff_hurt;
		public int buff_type;
		public int buff_time;
		public int buff_aim;
		public int buff_hurt;
		public int sid;
		public string remark = "";
		public string icon = "";

	}
}
