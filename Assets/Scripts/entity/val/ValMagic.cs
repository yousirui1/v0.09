using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**************************************
*FileName: ValMagic.cs
*User: ysr 
*Data: 2018/1/30
*Describe: 技能json文件对应的类
**************************************/

namespace tpgm
{
	[Serializable]
	public class ValMagic : BaseVal
	{
		public string name = "";			//
		public int grade;					//等级
		public int aim;						//技能作用对象 0 敌方 1 友方 2  仅 自己
		public int center;			
		public int radius;					//碰撞半径
		public int speed;					//技能速度
		public int hurt_type;				//技能是否带buffer 0 1 
		public int hurt_interval;
		public int hurt;					//技能伤害
		public int cd;						//技能cd
		public int count;					//技能数量
		public int bookSid;					//匹配魔法书
		public int duration	;				//动画时间


		public int buff_type; 
		public int buff_time;        		//buffer 持续时间 
		public int buff_interval;			//buffer 间隔触发时间 
		public int buff_aim;				//buffer 0 敌方 1 友方 2  仅 自己
		public int buff_hurt;				//buffer 伤害
		public int sid;						//
		public string remark = "";			//技能介绍
		public string icon = "";			//icon

	}
}
