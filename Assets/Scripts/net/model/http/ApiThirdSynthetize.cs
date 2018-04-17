using System;

/**************************************
*FileName: ApiThirdSynthetize.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 魔法师界面合成
**************************************/


namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdSynthetize
	{
		public ReqThirdSynthetize()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID  = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = ""; 

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public int m_gsid ;


	}

	[ProtoBuf.ProtoContract]
	public class RespThirdSynthetize
	{
		public RespThirdSynthetize()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";


		[ProtoBuf.ProtoMember(3, IsRequired = false)]
		public int m_achieve;


	}


}
