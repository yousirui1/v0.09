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
	public class ReqThirdAstrology
	{
		public ReqThirdAstrology()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public int m_type ;

	}

	[ProtoBuf.ProtoContract]
	public class RespThirdAstrology
	{
		public RespThirdAstrology()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  

		[ProtoBuf.ProtoMember(3, IsRequired = false)]   
		public string m_materials = "";  						//签到奖励 JsonObject

	}
		
}

