using System;
using UnityEngine;
using System.Collections.Generic;

/**************************************
*FileName: AudioCache.cs
*User: ysr 
*Data: 2018/2/2
*Describe: 音频缓存, 主要确保内存中的音频占用不超过一定的范围
**************************************/

namespace tpgm
{
    //# 音频缓存, 主要确保内存中的音频占用不超过一定的范围;
    public class AudioCache
    {
        CacheCommon<AudioCacheItem> m_cacheCommon = new CacheCommon<AudioCacheItem>(new LfuComparer());

        //#pageID => 该页面使用的音频;
        Dictionary<string, HashSet<string>> m_pageMarkedAudioDict = new Dictionary<string, HashSet<string>>();

		class AudioCacheItem : tpgm.CacheItem
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

            public void increaseUseCnt(long now)
            {
                m_totalUseCnt++;
                
                if (now - m_recentUseMs >= 60 * 1000)
                {
                    m_recentUseCnt = 0;
                }
                m_recentUseCnt++;
            }

            public AudioClip m_retainObject;

            public int m_totalUseCnt;

            public int m_recentUseCnt;
            public long m_recentUseMs;
        }

        class LfuComparer : IComparer<AudioCacheItem>
        {
            public int Compare(AudioCacheItem x, AudioCacheItem y)
            {
                if (null == y)
                {
                    return -1;
                }

                int delta = y.m_recentUseCnt - x.m_recentUseCnt;
                delta = Math.Max(-1, Math.Min(delta, 1));

                return delta;
            }
        }


        public AudioCache()
        {
			
        }

        //#一些只在当前页面使用的音频;
        public AudioClip usePageScopeAudio(string instanceID, string audioPath)
        {
            AudioCacheItem citem;
            if (true) //mock: 开发时才启用;
            {
                citem = checkPageBeginUseCalled(instanceID, audioPath);
            }
            else
            {
                m_cacheCommon.m_dict.TryGetValue(audioPath, out citem);
            }

			long now = TimeUtils.utcNowMs();
            citem.increaseUseCnt(now);
            return citem.m_retainObject;
        }

        AudioCacheItem checkPageBeginUseCalled(string instanceID, string audioPath)
        {
            AudioCacheItem citem;
            if (m_cacheCommon.m_dict.TryGetValue(audioPath, out citem))
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

            return citem;
        }

        //#一些在多个页面间用到的音效;
        public AudioClip useGlobalSound(string audioPath)
        {
            AudioCacheItem citem = getCacheItem(audioPath);

			long now = TimeUtils.utcNowMs();
            citem.increaseUseCnt(now);
            return citem.m_retainObject;
        }

        //************************************************** mark begin;

        AudioCacheItem getCacheItem(string audioPath)
        {
            AudioCacheItem citem;

            if (!m_cacheCommon.m_dict.TryGetValue(audioPath, out citem))
            {
                citem = new AudioCacheItem();
                m_cacheCommon.m_dict.Add(audioPath, citem);
                citem.m_path = audioPath;
            }

            if (null == citem.m_retainObject)
            {
                AudioClip res = Resources.Load<AudioClip>(audioPath);

                if (null == res)
                {
                    //#不存在或加载失败则不mark;
                    return null;
                }
                else
                {
                    citem.m_retainObject = res;

                    if (citem.m_memBytes <= 0)
                    {
                        citem.m_memBytes = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(res);
                    }

                    m_cacheCommon.m_nowBytes += citem.m_memBytes;
                }
            }

            //m_cacheCommon.updateCacheItemMark(citem); //#移到markXxx处;

            return citem;
        }

        public void markPageUse(string instanceID, string audioPath)
        {
            AudioCacheItem citem = getCacheItem(audioPath);

            if (null != citem)
            {
                m_cacheCommon.increaseMarkData(citem);

                if (citem.m_retainPages.Contains(instanceID))
                {
                    throw new ArgumentException(audioPath + " has marked by this page: " + instanceID);
                }
                else
                {
                    citem.m_retainPages.Add(instanceID);
                }
            }
            else
            {
                //warn: 加载失败;
            }
        }

        public void unmarkPageUse(string instanceID, string audioPath)
        {
            m_cacheCommon.unmarkPageUse(instanceID, audioPath);
        }

        //************************************************** mark end;

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
                Log.i<AudioCache>("no need to gc: now: " + Utils.bytesToReadableUnit(m_cacheCommon.m_nowBytes) + ", gc: " + Utils.bytesToReadableUnit(m_cacheCommon.m_gcBytes));
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
                AudioCacheItem citem = m_cacheCommon.m_tmpList[i];
                m_cacheCommon.m_nowBytes -= citem.m_memBytes;
                citem.releaseRetain();
            }
            m_cacheCommon.m_tmpList.Clear();
            Log.d<AudioCache>("1 gcCount: " + gcCount);

            if (gcCount > 0)
            {
                AsyncOperation asy = Resources.UnloadUnusedAssets();
            }
        }

        public void fullGc()
        {
            m_cacheCommon.findNoPageRetainItems();

            int gcCount = 0;
            for (int i = m_cacheCommon.m_tmpList.Count - 1; i >= 0; i--)
            {
                gcCount++;
                AudioCacheItem citem = m_cacheCommon.m_tmpList[i];
                m_cacheCommon.m_nowBytes -= citem.m_memBytes;
                citem.releaseRetain();
            }
            m_cacheCommon.m_tmpList.Clear();
            Log.d<AudioCache>("2 gcCount: " + gcCount);

            if (gcCount > 0)
            {
                AsyncOperation asy = Resources.UnloadUnusedAssets();
            }
        }

        //************************************************** gc end;

        //#
        //public void logout()
        //{
        //}



     

    }
}

