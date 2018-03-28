using System;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 获取版本号数据定义类
**************************************/

namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdCheckVersion
	{
		public ReqThirdCheckVersion()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(3, IsRequired=true)]
		public int  m_platform;       				//平台号

	}

	[ProtoBuf.ProtoContract]
	public class RespThirdCheckVersion
	{
		public RespThirdCheckVersion()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;


		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_version = "";  


		[ProtoBuf.ProtoMember(3, IsRequired = true)]
		public string m_apkUrl = "";  


		[ProtoBuf.ProtoMember(4, IsRequired = true)]
		public string m_utcMs = "";  


	}
}

