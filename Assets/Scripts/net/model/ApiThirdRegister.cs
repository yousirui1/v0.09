using System;

/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 注册数据定义类
**************************************/

namespace tpgm
{
    [ProtoBuf.ProtoContract]
    public class ReqThirdRegister
    {
		public ReqThirdRegister()
        {
        }

        [ProtoBuf.ProtoMember(1, IsRequired=true)]
        public int m_isRetry;

        [ProtoBuf.ProtoMember(2, IsRequired=true)]
        public string m_checkID = "";

        [ProtoBuf.ProtoMember(3, IsRequired=true)]
		public int  m_type;       				//1：注册，2：绑定mac地址

        [ProtoBuf.ProtoMember(4, IsRequired=false)]
		public string m_mac = "";

        [ProtoBuf.ProtoMember(5, IsRequired=true)]
		public string m_account = "";

		[ProtoBuf.ProtoMember(6, IsRequired=true)]
		public string m_password = "";
    }

    [ProtoBuf.ProtoContract]
	public class RespThirdRegister
    {
		public RespThirdRegister()
        {
        }

        [ProtoBuf.ProtoMember(1, IsRequired = true)]
		public int m_code;

        [ProtoBuf.ProtoMember(2, IsRequired = true)]
        public string m_utcMs = "";  
	
    }
}

