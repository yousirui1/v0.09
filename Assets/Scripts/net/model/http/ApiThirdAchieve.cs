using System;

/**************************************
*FileName: ApiThirdAstrology.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 占星数据定义类
**************************************/

namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdAchieve
	{
		public ReqThirdAchieve()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

	}

	[ProtoBuf.ProtoContract]
	public class RespThirdAchieve
	{
		public RespThirdAchieve()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  

		[ProtoBuf.ProtoMember(3, IsRequired = false)]   
		public string m_achieve = "";  						//魔法师使用
	}

	public class JsonAchieve
	{
		public int sid;
		public int type;		//成就类型
		public int hold;		//拥有数量
		public string finish = "";	//领取过的奖励id
	}

}

