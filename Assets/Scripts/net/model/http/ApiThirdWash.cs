using System;

/**************************************
*FileName: ApiThirdWash.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 魔法师界面洗点
**************************************/


namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdWash
	{
		public ReqThirdWash()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID  = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = ""; 

	}

	[ProtoBuf.ProtoContract]
	public class RespThirdWash
	{
		public RespThirdWash()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";


		[ProtoBuf.ProtoMember(3, IsRequired = false)]
		public int m_talent;		//天赋点


	}


}
