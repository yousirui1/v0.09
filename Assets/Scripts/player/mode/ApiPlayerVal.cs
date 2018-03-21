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

	//传输帧
	public class FrameBuf
	{
		public List<PlayerVal> data;
		public long time;   //服务器发送的时间
		public int frame;  //逻辑帧
	}

	//玩家类用于服务器传输数据
	public class PlayerVal
	{
		public string uid;
		public int x;
		public int y;
		public int d;	 
		public int v;     //速度
	  
		public int sp;
		public int hp;

		public int skill;  //技能 0,1,2 对应3种状态

		public int magicStage;
		public int score;  //玩家所获得积分
		public int item;
		public int lev;
		public string utcMs; //服务器接收到的请求值
	

		public int sx;
		public int sy;
		public int sd;
		public int sv;
	}



}

