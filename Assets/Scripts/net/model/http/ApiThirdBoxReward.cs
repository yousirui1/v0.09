using System;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 宝箱奖励领取数据定义类
**************************************/

namespace tpgm
{
	[ProtoBuf.ProtoContract]
	public class ReqThirdBoxReward
	{
		public ReqThirdBoxReward()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired=true)]
		public int m_isRetry;

		[ProtoBuf.ProtoMember(2, IsRequired=true)]
		public string m_checkID = "";

		[ProtoBuf.ProtoMember(4, IsRequired=true)]
		public string m_token = "";

	}

	[ProtoBuf.ProtoContract]
	public class RespThirdBoxReward
	{
		public RespThirdBoxReward()
		{
		}

		[ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

		[ProtoBuf.ProtoMember(2, IsRequired = true)]
		public string m_utcMs = "";  

		[ProtoBuf.ProtoMember(2, IsRequired = true)]   
		public string m_reward = "";  						//奖励

		[ProtoBuf.ProtoMember(2, IsRequired = true)]    
		public string m_boxTime = "";     					//宝箱开始时间

	}
}

