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
	public class ReqThirdAchieveWard
	{
		public ReqThirdAchieveWard()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public int m_sid;

	}

	[ProtoBuf.ProtoContract]
	public class RespThirdAchieveWard
	{
		public RespThirdAchieveWard()
		{
		}
		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  
	
	}
		

}

