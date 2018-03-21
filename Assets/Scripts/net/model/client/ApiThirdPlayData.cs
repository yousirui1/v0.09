using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tpgm
{


	public class ReqThirdPlayData
	{
		public ReqThirdPlayData()
		{
		}
	}



	public class RespThirdPlayData
	{
		public AttackNum attackNum;
	}

	#if false
	public class AttackInfo
	{

	}
	#endif

	public class AttackNum
	{
		public string dead = "";
		public List<int> type;
		public string kill = "";
		public List<string> assists;
	}

}
