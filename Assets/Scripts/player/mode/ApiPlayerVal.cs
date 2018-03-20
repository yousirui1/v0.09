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


	public class PlayerInfoBuf
	{
		public List<PlayerInfo> playerInfo;
	}


	public class PlayerInfo
	{

		public string uid;
		public int skin;
		public int magicBook;
		public int attackSkin;
		public string genius;
		public int pet;
		public	Coordinate coordinate;

	}



	public class Coordinate
	{
		public int X;
		public int Y;
	}



	public class PlayData
	{
		
		public AttackNum attackNum;

	}


	public class AttackInfo
	{

	}


	public class AttackNum
	{
		public string dead = "";
		public List<int> type;
		public string kill = "";
		public List<string> assists;
	}



	public class GloryAddBuf
	{
		public List<NewUser> newUser;
	}

	public class NewUser
	{
		public string uid = "";
		public string nickname = "";
		public int head;
		public string group = "";
	}




	public class UserData
	{
		public string nickname = "";
		public int score;  //玩家所获得积分
		public int kill;	//击杀
		public int death;	//死亡	
		public int assit;	//助攻	
		public string group = "";	//组队
		public int head;  	//头像

		public float kda;	//(杀人+助攻)/死亡
	}

	public class UserRank
	{
		public string m_uid { get; set ;}
		public int m_score { get; set ;}

		public UserRank(string uid, int score)
		{
			this.m_uid = uid;
			this.m_score = score;
		}
	}
}

