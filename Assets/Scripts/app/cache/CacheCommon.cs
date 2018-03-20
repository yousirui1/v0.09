using System;
using System.Collections.Generic;

namespace tpgm
{
    public class CacheCommon<T_CacheItem> where T_CacheItem : CacheItem
    {
        class LruComparer : IComparer<T_CacheItem>
        {
            public int Compare(T_CacheItem x, T_CacheItem y)
            {
                if (null == y)
                {
                    return -1;
                }

                long delta = y.m_lastMarkMs - x.m_lastMarkMs;

                delta = Math.Max(-1, Math.Min(delta, 1));
                //Log.d<CacheItem>(m_keyInDict + ", " + other.m_keyInDict + "; delta: " + delta);

                return (int) delta;
            }
        }

        public CacheCommon(IComparer<T_CacheItem> comparer)
        {
            if (null == comparer)
            {
                m_comparer = new LruComparer();
            }
        }

        internal void printUseMemSize()
        {
            Log.d<TexCache>("nowSize: " + Utils.bytesToReadableUnit(m_nowBytes));
        }

        //#刷新标记信息;
        internal void increaseMarkData(CacheItem citem)
        {
			long now = TimeUtils.utcNowMs();
            citem.m_lastMarkMs = now;
            citem.increaseMarkCnt(now);
        }

        //#返回dictKey是否不存在;
        internal bool unmarkPageUse(string instanceID, string dictKey)
        {
            T_CacheItem citem;
            if (m_dict.TryGetValue(dictKey, out citem))
            {
                if (citem.m_retainPages.Contains(instanceID))
                {
                    citem.m_retainPages.Remove(instanceID);
                }
                else
                {
                    //#其它页面mark过, 但这个页面没mark过;
                    //mark的时候可能失败的;
                    //throw new ArgumentException("markPageUse not called: " + pageID);
                }

                return false;
            }
            else
            {
                //#没有任何页面mark过; 可能是资源不存在, 此时不会被mark;

                return true;
            }
        }

        internal void findNoPageRetainItems()
        {
            foreach (var entry in m_dict)
            {
                T_CacheItem citem = entry.Value;
                if (citem.noPageRetain())
                {
                    m_tmpList.Add(citem);
                }
            }

            if (m_tmpList.Count > 0)
            {
                //#越在后面就是最近越没有被使用过;
                if (null == m_comparer)
                {
                    m_tmpList.Sort();
                }
                else
                {
                    m_tmpList.Sort(m_comparer);
                }
            }
        }

        internal void printAllEntriesLruOrder()
        {
            foreach (var entry in m_dict)
            {
                T_CacheItem citem = entry.Value;
                m_tmpList.Add(citem);
            }

            if (m_tmpList.Count > 0)
            {
                m_tmpList.Sort();
            }

            for (int i = 0; i < m_tmpList.Count; i++)
            {
                CacheItem item = m_tmpList[i];
                Log.d<TexCache>(item.m_path + ", " + item.m_lastMarkMs + ", rt: " + item.m_retainPages.Count);
            }
            m_tmpList.Clear();
        }

        public Dictionary<string, T_CacheItem> m_dict = new Dictionary<string, T_CacheItem>();
        public List<T_CacheItem> m_tmpList = new List<T_CacheItem>();

        //TODO: 最近需要的内存占用本身比较大, 则调大gcBytes; 最近一段时间内都比较小, 则调小gcBytes;
        //#贴图最大的可在内存中缓存这么多; 
        public long m_gcBytes = 10 * 1024 * 1024;

        //#当前贴图使用掉的内存大小;
        public long m_nowBytes;

        //#上次回收的时间;
        public long m_lastGcMs;
        //#gc的最少时间间隔;
        public long m_minGcInterval = 10 * 1000;

        IComparer<T_CacheItem> m_comparer;
    }
}

