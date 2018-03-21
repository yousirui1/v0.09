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

	public class ReqThirdReward
	{
		public  ReqThirdReward()
		{

		}
		public string roomNum = "";
	}

	public class RespThirdReward
	{
		public int code;
		public string utcMs = "";
		public string reward = "";
	}

	public class PlayerData
	{
		public int gsid;
		public int num;
	}
}
