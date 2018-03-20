using System;
using UnityEngine;

//#主线程的Looper;
using System.Collections.Generic;
using System.Threading;

/**************************************
*FileName: MainLooper.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 主线程异步调用使用主线程
**************************************/

namespace tpgm
{
    public class MainLooper : MonoBehaviour
    {


		private static MainLooper s_instance;

		//#正在被处理的message;
		private LinkedList<HandlerMessage> m_pendingRunMsgList = new LinkedList<HandlerMessage>();
		private List<HandlerMessage> m_tmpList = new List<HandlerMessage>();

		//#发送的message首先都是到这个里来;
		private List<HandlerMessage> m_isRunMsgList = new List<HandlerMessage>();

		private static Queue<HandlerMessage> s_msgPool = new Queue<HandlerMessage>();
		private static int s_maxMsgPoolSize = 10;

        //#创建一个全局的looper;
        public static void setup()
        {
            GameObject gameObj = new GameObject("GlobalMainLooper");
            s_instance = gameObj.AddComponent<MainLooper>();

            MonoBehaviour.DontDestroyOnLoad(gameObj);
        }

        public static void checkSetup()
        {
            if (null == s_instance)
            {
                setup();
            }
        }

        public static MainLooper instance()
        {
            return s_instance;
        }

        void Update()
        {
            try
            {
                Monitor.Enter(m_pendingRunMsgList);
				long now = TimeUtils.utcNowMs();

                foreach (var item in m_pendingRunMsgList)
                {
                    if (now >= item.m_when)
                    {
                        m_isRunMsgList.Add(item);
                        //m_pendingRunMsgList.Remove(item); //#遍历过程中删除, 会提示list modified
                    }
                }

                for (int i = 0, size = m_isRunMsgList.Count; i < size; i++)
                {
					HandlerMessage item = m_isRunMsgList[i];
                    m_pendingRunMsgList.Remove(item);
                }
            }
            finally
            {
                Monitor.Exit(m_pendingRunMsgList);
            }


            bool receiveQuitMsg = false;
            for (int i = 0, size = m_isRunMsgList.Count; i < size; i++)
            {
				HandlerMessage item = m_isRunMsgList[i];

				long startUtcMs = TimeUtils.utcNowMs();

                if (null != item.m_toExecute)
                {
                    item.m_toExecute.Invoke(item);
                }
                else if (null != item.m_handler)
                {
                    item.m_handler(item);
                }

				long endUtcMs = TimeUtils.utcNowMs();
                long diffUtcMs = endUtcMs - startUtcMs;
                if (diffUtcMs >= 5 * 1000)
                {
                    Log.w<MainLooper>("MainLooper: msg execute too long: " + diffUtcMs);
                }

                recycleMessage(item);
            }
            m_isRunMsgList.Clear();
        }

		void enqueueMessage(HandlerMessage msg)
        {
            if (true) //mock: dev
            {
                if (null == msg.m_handler && null == msg.m_toExecute)
                {
                    throw new ArgumentException("Message's m_handler or m_toExecute not set");
                }
                if (null != msg.m_handler && null != msg.m_toExecute)
                {
                    throw new ArgumentException("Message's m_handler and m_toExecute all set");
                }
            }

            //#从A线程向B线程发送消息;
            lock (m_pendingRunMsgList)
            {
                //#按照时间顺序排好序; 最后一个时间总是最晚的;
        
                var item = m_pendingRunMsgList.Last;
                int idx = m_pendingRunMsgList.Count;
                if (null != item)
                {
                    do
                    {
                        if (msg.m_when >= item.Value.m_when)
                        {
                            m_pendingRunMsgList.AddAfter(item, msg);
                            //Log.d<MainLooper>(idx + "后面添加");
                            break;
                        }
        
                        idx--;
                        item = item.Previous;
                    } while (null != item);
                }
                else
                {
                    //Log.d<MainLooper>("添加在最后");
                    m_pendingRunMsgList.AddLast(msg);
                }
        
                //测试添加后的排序对不对的;
                if (false)
                {
                    foreach (var _item in m_pendingRunMsgList)
                    {
                        Log.d<MainLooper>("what: " + _item.m_what + ", when: " + _item.m_when);
                    }
                }
            }
        }

		//线程直接发送广播消息
		public static HandlerMessage obtainMessage(MessageHandler handler, int what)
        {
			HandlerMessage msg = obtainMessage(what);
            msg.m_handler = handler;

            return msg;
        }

		//异步线程调用主线程
		public static HandlerMessage obtainMessage(Action<HandlerMessage> toExecute)
        {
			HandlerMessage msg = obtainMessage(0);
            msg.m_toExecute = toExecute;

            return msg;
        }

		//根据what取广播数据
		public static HandlerMessage obtainMessage(int what)
        {
            lock (s_msgPool)
            {
                int poolSize = s_msgPool.Count;

                //Log.d<MainLooper>("obtainMessage msgPool size: " + poolSize);

				HandlerMessage msg;
                if (poolSize > 0)
                {
                    msg = s_msgPool.Dequeue();
                }
                else
                {
					msg = new HandlerMessage();
                }

				msg.m_createMs = TimeUtils.utcNowMs();
                msg.m_what = what;
                return msg;
            }
        }


		static void recycleMessage(HandlerMessage msg)
        {
            lock (s_msgPool)
            {
                int poolSize = s_msgPool.Count;
        
                if (poolSize < s_maxMsgPoolSize)
                {
                    msg.resetForRecycle();
                    s_msgPool.Enqueue(msg);
        
                    poolSize++;
                }
        
                //log: test
               //Log.d<MainLooper>("recycleMessage msgPool size: " + poolSize);
            }
        }

        //************************************************** 消息处理 begin;
		//发送消息处理
        public void sendMessage(MessageHandler handler, int what)
        {
			HandlerMessage msg = obtainMessage(handler, what);
            //msg.m_handler = handler;

            enqueueMessage(msg);
        }
		//发送消息处理
		public void sendMessage(Action<HandlerMessage> toExecute)
        {
			HandlerMessage msg = obtainMessage(toExecute);

            enqueueMessage(msg);
        }
		//发送消息处理
		public void sendMessage(HandlerMessage msg)
        {
            enqueueMessage(msg);
        }
		//延迟发送消息处理,delayMs延迟时间
		public void postMessageDelay(HandlerMessage msg, long delayMs)
        {
			msg.m_when = TimeUtils.utcNowMs() + delayMs;
            enqueueMessage(msg);
        }

		//定时发送消息
		public void postMessageAt(HandlerMessage msg, long atMs)
        {
            msg.m_when = atMs;
            enqueueMessage(msg);
        }

		//移除消息处理
        public void removeMessages(MessageHandler handler)
        {
            lock (m_pendingRunMsgList)
            {
                int count = 0;
                foreach (var item in m_pendingRunMsgList)
                {
                    if (item.m_handler == handler)
                    {
                        count++;
                        m_tmpList.Add(item);
                    }
                }

                for (int i = 0, size = m_tmpList.Count; i < size; i++)
                {
					HandlerMessage msg = m_tmpList[i];
                    m_pendingRunMsgList.Remove(msg);

                    recycleMessage(msg);
                }

                m_tmpList.Clear();

                //#log: test
                //Log.d<MainLooper>("remove count: " + count);
            }
        }

		//移除消息处理函数
		public void removeMessagesByToExecute(Action<HandlerMessage> toExecute)
        {
            lock (m_pendingRunMsgList)
            {
                int count = 0;
                foreach (var item in m_pendingRunMsgList)
                {
                    if (item.m_toExecute == toExecute)
                    {
                        count++;
                        m_tmpList.Add(item);
                    }
                }

                for (int i = 0, size = m_tmpList.Count; i < size; i++)
                {
					HandlerMessage msg = m_tmpList[i];
                    m_pendingRunMsgList.Remove(msg);

                    recycleMessage(msg);
                }

                m_tmpList.Clear();

                //#log: test
                //Log.d<MainLooper>("remove count: " + count);
            }
        }

        //************************************************** 消息处理 end;

       
    }
}

