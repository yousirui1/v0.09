using System;
using System.Collections.Generic;



namespace tpgm
{

	public class ReqThirdGloryAdd
	{
		public ReqThirdGloryAdd()
		{
		}
	}


	public class RespThirdGloryAdd
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

}
