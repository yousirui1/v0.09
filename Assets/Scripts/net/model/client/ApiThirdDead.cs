using System;
using System.Collections.Generic;

/**************************************
*FileName: ApiThirdEnterRoom.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 创建房间
**************************************/

namespace tpgm
{

	public class ReqThirdUserData
	{
		public  ReqThirdUserData()
		{

		}
	}

	public class RespThirdUserData
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
		public string m_nickname { get; set ;}
		public int m_score { get; set ;}

		public UserRank(string uid, string nickname, int score)
		{
			this.m_uid = uid;
			this.m_nickname = nickname;
			this.m_score = score;
		}
	}
}
