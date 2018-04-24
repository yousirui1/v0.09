using System;
using System.Collections.Generic;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 宝箱奖励领取数据定义类
**************************************/

namespace tpgm
{

	//发送
	public class ReqThirdMove
	{
		public  ReqThirdMove()
		{

		}
	}

	//接收
	public class RespThirdMove
	{
		public List<PlayerVal> data;
		public string time = "";   //服务器发送的时间
		public int frame;  //逻辑帧
	}





	//玩家类用于服务器传输数据
	public class Data
	{
		public string uid = "";
		public int x;
		public int y;
		public int d;	 
		public int v;     //速度
	  
		public int sp;
		public int hp;

		public int score;  //玩家所获得积分

		public int magicStage;

		public int item;
		public int lev;
		public int skill;  //技能 0,1,2 对应3种状态

	}

	public class PlayerVal
	{

		public string uid = "";
		public int x;
		public int y;
		public int v;     //速度

		public int d;	 
		public int dx;	 
		public int dy;	 

		public int sp;
		public int hp;

		public int score;  //玩家所获得积分

		public int magicStage;

		public int item;
		public int lev;
		public int skill;  //技能 0,1,2 对应3种状态

		public int sx = 0;
		public int sy = 0;
		public int sd = 0;
		public int sv = 0;

		public int old_d = 0;
	}


}

