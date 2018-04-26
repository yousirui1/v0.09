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
		public  RespThirdMove()
		{

		}

		public List<PlayerVal> moveData = new List<PlayerVal>();
		public string utMs = "";   //服务器发送的时间
		public int frame = 0;  //逻辑帧
	}





	//玩家类用于服务器传输数据
	public class MoveData
	{
		public string uid = "";
		public float x = 0;
		public float y = 0;
		public float v = 0;     //速度
		public int sp = 0;
		public int hp = 0;
		public int score = 0;  //玩家所获得积分
		public int magicStage = 0;
		public int item = 0;
		public int lev = 0;
		public int skill = 0;  //技能 0,1,2 对应3种状态

		public float dx = 0;	 
		public float dy = 0;	

	}

	public class PlayerVal
	{

		public string uid = "";
		public int x;
		public int y;
		public int v;     //速度
		public int sp;
		public int hp;
		public int score;  //玩家所获得积分
		public int magicStage;
		public int item;
		public int lev;
		public int skill;  //技能 0,1,2 对应3种状态

		public int dx;	 
		public int dy;	 

		public float fdx;
		public float fdy;


		public int sx = 0;
		public int sy = 0;
		public int sdx = 0;
		public int sdy = 0;
		public int sv = 0;

		public float old_dx = 0;
		public float old_dy = 0;
	}


}

