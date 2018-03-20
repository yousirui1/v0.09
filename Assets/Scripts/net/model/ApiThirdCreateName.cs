using System;

/**************************************
*FileName: ApiThirdcreateName.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 修改昵称数据定义类
**************************************/

namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdCreateName
	{
		public ReqThirdCreateName()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public string m_name;

	}

	[ProtoBuf.ProtoContract]
	public class RespThirdCreateName
	{
		public RespThirdCreateName()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";

	}
}
