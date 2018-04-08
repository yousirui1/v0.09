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

	public class ReqThirdDead
	{
		public  ReqThirdDead()
		{

		}

		public string roomNum = "";
		public string kill = "";  
		public string dead = "";
		public List<string> assists;
	}

	public class RespThirdDead
	{
		public int code;
		public string utcMs ="";
	}


}
