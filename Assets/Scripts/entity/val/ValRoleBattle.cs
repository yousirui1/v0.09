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
	public class ValRoleBattle : BaseVal
	{
		public int level;  	//等级
		public int res; 	//经验
		public int atk;		//普攻伤害
		public int hp;		//血上限
		public int speed;	//速度
		public int spirit;	//sp上限
	}
}
