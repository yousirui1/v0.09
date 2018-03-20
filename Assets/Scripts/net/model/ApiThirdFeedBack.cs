using System;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 意见反馈数据定义类
**************************************/

namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdFeedBack
	{
		public ReqThirdFeedBack()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public string m_token = "";

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public string m_content = "";    			//反馈意见


	}

	[ProtoBuf.ProtoContract]
	public class RespThirdFeedBack
	{
		public RespThirdFeedBack()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;


		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  


	}
}

