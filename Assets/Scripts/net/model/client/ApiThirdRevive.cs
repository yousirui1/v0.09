using System;
using System.Collections.Generic;

/**************************************
*FileName: ApiThirdRevive.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 复活
**************************************/

namespace tpgm
{

	public class ReqThirdRevive
	{
		public  ReqThirdRevive()
		{

		}
		public string roomNum = "";
	}

	public class RespThirdRevive
	{
		public int code;
		public string utcMs = "";
	}
}
