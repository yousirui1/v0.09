using System;


/**************************************
*FileName: HandlerMessage.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 线程同步Message数据类型
**************************************/
namespace tpgm
{
	public delegate void MessageHandler(HandlerMessage msg);

    public class MessageHandlerProxy
    {
        public MessageHandlerProxy(MessageHandler h)
        {
            m_messageHandler = h;
        }

		public void handleMessage(HandlerMessage msg)
        {
            if (null != m_messageHandler)
            {
                m_messageHandler(msg);
            }
        }

        public void onObjectDestroy()
        {
            m_messageHandler = null;
        }

        MessageHandler m_messageHandler;
    }

    public class HandlerMessage
    {
        //#什么时候触发;
        internal long m_when = 0;

        public long m_createMs;

        public MessageHandler m_handler;

		public Action<HandlerMessage> m_toExecute;

        //########## 消息数据 begin;

        //#消息类型;
        public int m_what;

        public int m_dataInt;

        public string m_dataStr = "";

        public object m_dataObj;

        //########## 消息数据 end;

        public void resetForRecycle()
        {
            m_when = 0;
            m_createMs = 0;
            m_handler = null;
            m_toExecute = null;

            m_what = 0;
            m_dataInt = 0;
            m_dataStr = "";
            m_dataObj = null;
        }
    }
}

