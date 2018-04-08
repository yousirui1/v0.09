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
	public class ReqThirdEnterRoom
	{
		public ReqThirdEnterRoom()
		{
		}
			
	}
		
	public class RespThirdEnterRoom
	{
		public RespThirdEnterRoom()
		{
		}

		public int code;
		public string roomNum = "";
		public List<FriendArr> friendArr;
		public string utcMs = "";

	}

	public class FriendArr
	{
		public FriendArr()
		{
		}


		public string nickname = "";
		public string uid = "";
		public int gender;
		public int head;
		public int level;
		public int status;
	}

}

