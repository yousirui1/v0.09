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

	public class ReqThirdPlayerInfo
	{
		public  ReqThirdPlayerInfo()
		{

		}
	}


	public class RespThirdPlayerInfo
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
}
