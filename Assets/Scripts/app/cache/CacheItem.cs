using System;
using System.Collections.Generic;

namespace tpgm
{
    public class CacheItem
    {
        public virtual void releaseRetain()
        {
        }

        public virtual bool noPageRetain()
        {
            return false;
        }

        public virtual void increaseMarkCnt(long now)
        {
//            citem.m_totalMarkCnt++;
//
//            if (now - citem.m_recentMarkMs >= 60 * 1000)
//            {
//                citem.m_recentMarkCnt = 0;
//            }
//            citem.m_recentMarkCnt++;
        }

        //#被多少页面引用着; 不能用HashSet, 因为它是不可重复的;
        public HashSet<string> m_retainPages = new HashSet<string>();

        //#上次被mark的时间;
        public long m_lastMarkMs;

//        //#强引用着的object;
//        public UnityEngine.Object m_retainObjectUnity;
//        //#c#版本的;
//        public object m_retainObject;

//        //#总共被mark的次数;
//        public int m_totalMarkCnt;
//
//        //#最近一段时间内的mark次数;
//        public int m_recentMarkCnt;
//        //#>=1min时, 重置一下m_recentMarkCnt;
//        public long m_recentMarkMs;

        public long m_memBytes;

        //#用来在m_dict中反向查找自己的;
        public string m_path = "";
    }
}

