using System;
using System.Collections.Generic;
using System.IO;

/**************************************
*FileName: ValTableCache.cs
*User: ysr 
*Data: 2018/2/2
*Describe: 数值表缓存
**************************************/


namespace tpgm
{
    public class ValTableCache
    {
		class ValCacheItem : tpgm.CacheItem
        {
            public override void releaseRetain()
            {
                base.releaseRetain();
                m_retainObject = null;
            }

            public override bool noPageRetain()
            {
                bool ret = (null != m_retainObject && m_retainPages.Count <= 0);
                return ret;
            }

            public object m_retainObject;
        }

        public ValTableCache(ValLoader loader)
        {
            m_initedValLoader = loader;
        }

        //************************************************** outPageScope begin;

        //#在非页面中获取数值表时; 如果数据获取失败, 则抛出DataDamageException(只获取, 不处理);
        public List<T_Val> getValListOutPageScopeOrThrow<T_Val>(string valFileName) where T_Val : BaseVal
        {
            ValCacheItem citem = getCacheItemOrThrow<T_Val>(valFileName);
            //return (List<T_Val>)citem.m_retainObject;

            return m_initedValLoader.getValList<T_Val>(valFileName);
        }

        public Dictionary<int, T_Val> getValDictOutPageScopeOrThrow<T_Val>(string valFileName) where T_Val : BaseVal
        {
            ValCacheItem citem = getCacheItemOrThrow<T_Val>(valFileName);

            return m_initedValLoader.getValDict<T_Val>(valFileName);
        }

//        public T_Val getValBySidOutPageScopeOrThrow<T_Val>(string instanceID, string valFileName, int sid)  where T_Val : BaseVal
//        {
//            CacheItem citem = getCacheItemOrThrow<T_Val>(valFileName);
//
//            Dictionary<int, T_Val> dict = m_initedValLoader.getValDict<T_Val>(valFileName);
//            return ValUtils.getValByKeyOrThrow(dict, sid);
//        }

        //************************************************** outPageScope end;

        //************************************************** pageScope begin;

        //#会抛出DataDamageException(只获取, 不处理); 获取的肯定是mark成功的数值表, 如果没有数据, 则是数据损坏造成的;
        public List<T_Val> getValListInPageScopeOrThrow<T_Val>(string instanceID, string valFileName) where T_Val : BaseVal
        {
            if (true) //mock: 开发时才启用;
            {
                checkPageBeginUseCalled(instanceID, valFileName);
            }

            return m_initedValLoader.getValList<T_Val>(valFileName);
        }

        void checkPageBeginUseCalled(string instanceID, string valFileName)
        {
            ValCacheItem citem;
            if (m_cacheCommon.m_dict.TryGetValue(valFileName, out citem))
            {
                if (!citem.m_retainPages.Contains(instanceID))
                {
                    throw new ArgumentException("markPageUse not called: " + instanceID);
                }
            }
            else
            {
                throw new ArgumentException("markPageUse not called: " + instanceID);
            }
        }

        public Dictionary<int, T_Val> getValDictInPageScopeOrThrow<T_Val>(string instanceID, string valFileName) where T_Val : BaseVal
        {
            if (true) //mock: 开发时才启用;
            {
                checkPageBeginUseCalled(instanceID, valFileName);
            }

            return m_initedValLoader.getValDict<T_Val>(valFileName);
        }

        //#如果想要的sid不存在, 则会抛出DataCorruptException, 那是产品把数据配错了;
        public T_Val getValBySidInPageScopeOrThrow<T_Val>(string instanceID, string valFileName, int sid)  where T_Val : BaseVal
        {
            if (true) //mock: 开发时才启用;
            {
                checkPageBeginUseCalled(instanceID, valFileName);
            }

            Dictionary<int, T_Val> dict = m_initedValLoader.getValDict<T_Val>(valFileName);
            //return ValUtils.getValByKeyOrThrow(dict, sid);  yousirui
			return null;
        }

        //************************************************** pageScope end;

        //************************************************** use begin;

        //#数值表出错时, 会抛出DataDamageException, 只需要捕获, 不要去处理;
        public void markPageUseOrThrow<T_Val>(string instanceID, string valFileName) where T_Val : BaseVal
        {
            ValCacheItem citem = getCacheItemOrThrow<T_Val>(valFileName);
            m_cacheCommon.increaseMarkData(citem);

            if (citem.m_retainPages.Contains(instanceID))
            {
                throw new ArgumentException(valFileName + " has marked by this page: " + instanceID);
            }
            else
            {
                citem.m_retainPages.Add(instanceID);
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ #数值表加载失败时, 返回null, 这里没有使用异常来自动跳转;
        //#数值表加载失败抛出不需要处理的异常;
        ValCacheItem getCacheItemOrThrow<T_Val>(string valFileName) where T_Val : BaseVal
        {
            ValCacheItem citem;

            if (!m_cacheCommon.m_dict.TryGetValue(valFileName, out citem))
            {
                citem = new ValCacheItem();
                m_cacheCommon.m_dict.Add(valFileName, citem);
                citem.m_path = valFileName;
            }

            if (null == citem.m_retainObject)
            {
                List<T_Val> valList = m_initedValLoader.loadValListOrThrow<T_Val>(valFileName);

                /*
                if (null == valList)
                {
                    //#不存在或加载失败则不mark;
                    //#warn: 加载失败: 全局范围内会弹出弹框;

                    return null;
                }
                else
                */
                {
                    citem.m_retainObject = valList;

                    if (citem.m_memBytes <= 0)
                    {
                        citem.m_memBytes = Utils.memoryBytes(valList);

                        Dictionary<int, T_Val> valDict = m_initedValLoader.getValDict<T_Val>(valFileName);
                        citem.m_memBytes += Utils.memoryBytes(valDict);
                    }

                    m_cacheCommon.m_nowBytes += citem.m_memBytes;
                }
            }

            //m_cacheCommon.updateCacheItemMark(citem); //#移到markXxx中;

            return citem;
        }

        public void unmarkPageUse(string instanceID, string valFileName)
        {
            m_cacheCommon.unmarkPageUse(instanceID, valFileName);
        }

        //************************************************** use end;

        //************************************************** gc begin;

        public void checkDoQuickGc()
        {
			long now = TimeUtils.utcNowMs();
            long diffMs = now - m_cacheCommon.m_lastGcMs;
            if (diffMs >= m_cacheCommon.m_minGcInterval)
            {
                quickGc();

                m_cacheCommon.m_lastGcMs = now;
            }
        }

        public void quickGc()
        {
            if (m_cacheCommon.m_nowBytes <= m_cacheCommon.m_gcBytes)
            {
                Log.i<TexCache>("no need to gc: now: " + Utils.bytesToReadableUnit(m_cacheCommon.m_nowBytes) + ", gc: " + Utils.bytesToReadableUnit(m_cacheCommon.m_gcBytes));
                return;
            }

            m_cacheCommon.findNoPageRetainItems();

            int gcCount = 0;
            for (int i = m_cacheCommon.m_tmpList.Count - 1; i >= 0; i--)
            {
                if (m_cacheCommon.m_nowBytes <= m_cacheCommon.m_gcBytes)
                {
                    break;
                }

                gcCount++;
                ValCacheItem citem = m_cacheCommon.m_tmpList[i];
                m_cacheCommon.m_nowBytes -= citem.m_memBytes;
                citem.releaseRetain();

                m_initedValLoader.unloadTable(citem.m_path);
            }
            m_cacheCommon.m_tmpList.Clear();
            Log.d<TexCache>("1 gcCount: " + gcCount);

            if (gcCount > 0)
            {
                System.GC.Collect();
            }
        }

        public void fullGc()
        {
            m_cacheCommon.findNoPageRetainItems();

            int gcCount = 0;
            for (int i = m_cacheCommon.m_tmpList.Count - 1; i >= 0; i--)
            {
                gcCount++;
                ValCacheItem citem = m_cacheCommon.m_tmpList[i];
                m_cacheCommon.m_nowBytes -= citem.m_memBytes;
                citem.releaseRetain();

                m_initedValLoader.unloadTable(citem.m_path);
            }
            m_cacheCommon.m_tmpList.Clear();
            Log.d<TexCache>("2 gcCount: " + gcCount);

            if (gcCount > 0)
            {
                System.GC.Collect();
            }
        }

        //************************************************** gc end;

        ValLoader m_initedValLoader;

        CacheCommon<ValCacheItem> m_cacheCommon = new CacheCommon<ValCacheItem>(null);
    }
}

