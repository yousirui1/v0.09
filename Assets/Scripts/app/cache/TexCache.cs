using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Threading;

/**************************************
*FileName: TexCache.cs
*User: ysr 
*Data: 2018/2/2
*Describe: 贴图缓存, 主要确保内存中的贴图占用不超过一定的范围;
**************************************/

namespace tpgm
{
    //# 贴图缓存, 主要确保内存中的贴图占用不超过一定的范围;
    public class TexCache : MonoBehaviour
    {

        CacheCommon<TexCacheItem> m_cacheCommon = new CacheCommon<TexCacheItem>(null);

        //#pageID => 该页面mark了的资源列表;
        Dictionary<string, HashSet<string>> m_pageMarkedResPathList = new Dictionary<string, HashSet<string>>();
        StringBuilder m_tmpSB = new StringBuilder();

        int m_mainThreadID;
        List<AsyncLoadData> m_resLoadList = new List<AsyncLoadData>();


		class TexCacheItem : tpgm.CacheItem
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

            public UnityEngine.Object m_retainObject;
        }

        public static TexCache create()
        {
            GameObject go = new GameObject("TexCache");
            MonoBehaviour.DontDestroyOnLoad(go);
            TexCache texCache = go.AddComponent<TexCache>();

            return texCache;
        }

        public TexCache()
        {
            //m_gcBytes = (long) (Profiler.GetTotalReservedMemory() * 0.6f);
        }

        void Awake()
        {
            Log.d<TexCache>("id: " + Thread.CurrentThread.ManagedThreadId);
            Log.d<TexCache>("name: " + Thread.CurrentThread.Name);
            m_mainThreadID = Thread.CurrentThread.ManagedThreadId;
        }

        void Update()
        {
            update_AsyncLoad();
        }

        public void printUseMemSize()
        {
            m_cacheCommon.printUseMemSize();
        }

        //#TODO, 用markPageUse<T_ResType>这样会比较好, 那所有的unity资源都可以加进来;

        public void markPageUseTex(string instanceID, string texPath)
        {
            bool resExists;
            markPageUseRes<Texture2D>(instanceID, texPath, out resExists);
        }

        public void markPageUsePrefab(string instanceID,  string prefabPath)
        {
            bool resExists;
            markPageUseRes<GameObject>(instanceID, prefabPath, out resExists);
        }

        void markPageUseRes<T_ResType>(string instanceID,  string prefabPath) where T_ResType : UnityEngine.Object
        {
            bool resExists;
            markPageUseRes<T_ResType>(instanceID, prefabPath, out resExists);
        }

        //#增加mark计数;
        void markUseRes<T_ResType>(string texPath, out bool pResExists, out TexCacheItem pCitem) where T_ResType : UnityEngine.Object
        {
            pResExists = true;
            pCitem = null;

            TexCacheItem citem;
            if (m_cacheCommon.m_dict.TryGetValue(texPath, out citem))
            {
                if (null == citem.m_retainObject)
                {
                    T_ResType res = Resources.Load<T_ResType>(texPath);
                    if (null == res)
                    {
                        //#warn: will not be here;
                        pResExists = false;
                        return;
                    }

                    citem.m_retainObject = res;
                    m_cacheCommon.m_nowBytes += citem.m_memBytes;

                    printUseMemSize();
                }
            }
            else
            {
                citem = new TexCacheItem();
                m_cacheCommon.m_dict.Add(texPath, citem);
                citem.m_path = texPath;

                T_ResType res = Resources.Load<T_ResType>(texPath);
                if (null == res)
                {
                    //#不存在或加载失败则不mark;
                    Log.d<TexCache>(texPath + " not exists");
                    pResExists = false;
                    return;
                }
                citem.m_retainObject = res;

                long bytes = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(res);//t2d.GetRawTextureData().LongLength;
                citem.m_memBytes = bytes;
                m_cacheCommon.m_nowBytes += bytes;

                printUseMemSize();
            }
            pCitem = citem;

            m_cacheCommon.increaseMarkData(citem);
        }

        void markPageUseRes<T_ResType>(string instanceID, string texPath, out bool pResExists) where T_ResType : UnityEngine.Object
        {
            bool resExists;
            TexCacheItem citem;

            markUseRes<T_ResType>(texPath, out resExists, out citem);

            if (resExists)
            {
                if (citem.m_retainPages.Contains(instanceID))
                {
                    throw new ArgumentException(texPath + " has marked by this page: " + instanceID);
                }
                else
                {
                    citem.m_retainPages.Add(instanceID);
                }
            }

            pResExists = resExists;
            //Log.d<TexCache>(texPath + " rt1: " + item.m_pageRetainCount.Count);
        }

        public void unmarkPageUse(string instanceID, string texPath)
        {
            m_cacheCommon.unmarkPageUse(instanceID, texPath);
        }

//        public void unmarkAll()
//        {
//            foreach (var entry in m_dict)
//            {
//                CacheItem citem = entry.Value;
//                citem.m_pageRetainCount.Clear();
//            }
//        }

        public void printAllEntriesLruOrder()
        {
            m_cacheCommon.printAllEntriesLruOrder();
        }

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

        //#清除cacheItem持有的引用; 并返回清除了多少个cacheItem的引用;
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
                TexCacheItem citem = m_cacheCommon.m_tmpList[i];
                m_cacheCommon.m_nowBytes -= citem.m_memBytes;
                citem.releaseRetain();

                Log.d<TexCache>("quick res gc: " + citem.m_path);
            }
            m_cacheCommon.m_tmpList.Clear();
            Log.d<TexCache>("quick gcCount: " + gcCount);

            if (gcCount > 0)
            {
                AsyncOperation asy = Resources.UnloadUnusedAssets();
            }
        }

        //#清除cacheItem持有的引用; 并返回清除了多少个cacheItem的引用;
        public void fullGc()
        {
            m_cacheCommon.findNoPageRetainItems();

            int gcCount = 0;
            for (int i = m_cacheCommon.m_tmpList.Count - 1; i >= 0; i--)
            {
                gcCount++;
                TexCacheItem citem = m_cacheCommon.m_tmpList[i];
                m_cacheCommon.m_nowBytes -= citem.m_memBytes;
                citem.releaseRetain();

                Log.d<TexCache>("full res gc: " + citem.m_path);
            }
            m_cacheCommon.m_tmpList.Clear();
            Log.d<TexCache>("full gcCount: " + gcCount);

            if (gcCount > 0)
            {
                AsyncOperation asy = Resources.UnloadUnusedAssets();
            }
        }

        //************************************************** 获取普通的sprite begin;

        public Sprite getSprite(string texPath, string spName)
        {
            TexCacheItem item;
            if (m_cacheCommon.m_dict.TryGetValue(texPath, out item))
            {
                if (null == item.m_retainObject)
                {
                    throw new ArgumentException(texPath + " not loaded!");
                }
            }
            else
            {
                throw new ArgumentException(texPath + " not cached and not loaded!");
            }

            Sprite sp = Utils.loadSprite(texPath, spName);

            return sp;
        }

        //************************************************** 获取普通的sprite end;

        //************************************************** icon的加载 begin;

        //#TODO: 改为pageBeginDynamicUseRes, pageEndDynamicUseRes
        public void pageBeginUseIconGlobal(string instanceID)
        {
            HashSet<string> resListMarkedByPage;
            if (m_pageMarkedResPathList.TryGetValue(instanceID, out resListMarkedByPage))
            {
                //warn: 一个页面只能出现一次;
                throw new ArgumentException(instanceID + " has called: pageBeginUseIconGlobal!");
            }
            else
            {
                m_pageMarkedResPathList.Add(instanceID, new HashSet<string>());
            }
        }
            
        public void pageEndUseIconGlobal(string instanceID)
        {
            HashSet<string> resListMarkedByPage;
            if (m_pageMarkedResPathList.TryGetValue(instanceID, out resListMarkedByPage))
            {
                foreach (var item in resListMarkedByPage)
                {
                    unmarkPageUse(instanceID, item);
                }

                m_pageMarkedResPathList.Remove(instanceID);
            }
            else
            {
                //warn:
                throw new ArgumentException(instanceID + " not call: pageBeginUseIconGlobal!");
            }
        }

        Sprite getIconSprite(string instanceID, string iconName, int gsidHint)
        {
            string resPath;
            {
                int pageNum;
                if (gsidHint < 600000)
                {
                    throw new ArgumentException("gsid must >= 600000: " + gsidHint);
                }
                else if (600000 == gsidHint)
                {
                    pageNum = 1;
                }
                else
                {
                    pageNum = (gsidHint % 600001) / 36 + 1;
                }

                m_tmpSB.Append("game_res/Sprites/icon_global/tp_icon_global_");
                if (pageNum < 10)
                {
                    m_tmpSB.Append('0');
                }
                m_tmpSB.Append(pageNum);
                resPath = m_tmpSB.ToString();
                //Log.d<TexCache>("iconTexPath: " + iconTexPath);
                m_tmpSB.Length = 0;
            }

            HashSet<string> resListMarkedByPage;
            if (m_pageMarkedResPathList.TryGetValue(instanceID, out resListMarkedByPage))
            {
                bool hasMarked = resListMarkedByPage.Contains(resPath);
                if (hasMarked)
                {
                    //Log.d<TexCache>(iconTexPath + " has marked by: " + pageFullName);
                }
                else
                {
                    resListMarkedByPage.Add(resPath);
                    markPageUseTex(instanceID, resPath);
                }
            }
            else
            {
                throw new ArgumentException("pageBeginUseIconGlobal not called!");
            }

			long t2 = TimeUtils.utcNowMs();
            //Debug.Log("getIconSprite diffMs 1: " + (t2 - t1));

            Sprite sp = Utils.loadSprite(resPath, iconName);

            if (null != sp)
            {
                return sp;
            }

            //Log.d<TexCache>(iconName + " not found, use default");

            sp = getIconSprite(instanceID, 600000);

			t2 = TimeUtils.utcNowMs();
            //Debug.Log("getIconSprite diffMs 2: " + (t2 - t1));

            return sp;
        }

        public Sprite getIconSprite(string instanceID, string iconName)
        {
            int gsid = 600000;

            if (true) //#mock:
            {
                if (iconName.StartsWith("icon_"))
                {
                    //warn:
                }
                else
                {
                    string gsidStr = iconName.Substring(5);
                    Log.d<TexCache>("gsidStr: " + gsidStr);

                    if (!int.TryParse(gsidStr, out gsid))
                    {
                        //warn:
                    }
                }
            }

            return getIconSprite(instanceID, iconName, gsid);
        }

        //#TODO: 改为getGlobalIconSprite;
        public Sprite getIconSprite(string instanceID, int gsid)
        {
			long t1 = TimeUtils.utcNowMs();

            string iconName;
            {
                m_tmpSB.Append("icon_").Append(gsid);
                iconName = m_tmpSB.ToString();
                m_tmpSB.Length = 0;
            }

            return getIconSprite(instanceID, iconName, gsid);
        }


        //************************************************** icon的加载, 其它资源的动态加载 end;


        //************************************************** 其它资源的动态加载 begin;

        //#按需加载所需的资源, 如果资源不存在或加载失败, 则返回null;
        public T_ResType dynamicLoadOrNull<T_ResType>(string instanceID, string resPath) where T_ResType : UnityEngine.Object
        {
            //#也可以叫managedLoad, 托管方式的加载;

            HashSet<string> resListMarkedByPage;
            if (m_pageMarkedResPathList.TryGetValue(instanceID, out resListMarkedByPage))
            {
                bool hasMarked = resListMarkedByPage.Contains(resPath);
                if (hasMarked)
				{
                    //Log.d<TexCache>(iconTexPath + " has marked by: " + pageFullName);
                }
                else
                {
                    resListMarkedByPage.Add(resPath);
                    markPageUseRes<T_ResType>(instanceID, resPath);
                }
            }
            else
            {
                throw new ArgumentException("pageBeginUseIconGlobal not called!");
            }

            if (true) //mock:
            {
                TexCacheItem citem = null;
                if (m_cacheCommon.m_dict.TryGetValue(resPath, out citem))
                {
                    if (null == citem.m_retainObject)
                    {
                        //# load fail or res not exists
                    }

                    return (T_ResType)citem.m_retainObject;
                }
                else
                {
                    //#will not be here;
                    throw new ArgumentException(resPath + " no cache item");
                }

                //#will not be here;
                throw new ArgumentException(resPath + " fail");
            }
            else
            {
                TexCacheItem citem = m_cacheCommon.m_dict[resPath];
                return (T_ResType)citem.m_retainObject;
            }
        }

        //************************************************** 其它资源的动态加载 end;


        //************************************************** multiPack的加载 begin;

        public void markPageUseMultiPack(string instanceID, string multiPackTexPath, int startIDX)
        {
            int idx = multiPackTexPath.LastIndexOf('_');
            if (-1 == idx)
            {
                throw new ArgumentException("multiPackTexPath not end with _:" + multiPackTexPath);
            }

            multiPackTexPath = multiPackTexPath.Substring(0, idx + 1);

            int count = 0;
            string texPath;
            for (int i = startIDX; i < 100; i++)
            {
                texPath = string.Format("{0}{1:D2}", multiPackTexPath, i);

                bool resExists;
                markPageUseRes<Texture2D>(instanceID, texPath, out resExists);
                if (!resExists)
                {
                    Log.i<TexCache>(multiPackTexPath + ", mark multi: " + count);
                    break;
                }

                count++;
            }
        }


        public void unmarkPageUseMultiPack(string instanceID, string multiPackTexPath, int startIDX)
        {
            int idx = multiPackTexPath.LastIndexOf('_');
            if (-1 == idx)
            {
                throw new ArgumentException("multiPackTexPath not end with _:" + multiPackTexPath);
            }

            multiPackTexPath = multiPackTexPath.Substring(0, idx + 1);

            int count = 0;
            string texPath;
            for (int i = startIDX; i < 100; i++)
            {
                texPath = string.Format("{0}{1:D2}", multiPackTexPath, i);

                bool entryExists = m_cacheCommon.unmarkPageUse(instanceID, texPath);
                if (!entryExists)
                {
                    Log.i<TexCache>(multiPackTexPath + ", unmark multi: " + count);
                    break;
                }

                count++;
            }
        }

        //************************************************** multiPack的加载 end;

        //************************************************** 异步加载资源 begin;

        void update_AsyncLoad()
        {
            for (int i = m_resLoadList.Count - 1; i >= 0; i--)
            {
                AsyncLoadData loadReq = m_resLoadList[i];

                if (loadReq.m_loadHandler.isDone)
                {
                    m_resLoadList.RemoveAt(i);

                    UnityEngine.Object resObjOrNull = loadReq.m_loadHandler.asset;
                    if (null != resObjOrNull)
                    {
                        markUseRes(loadReq.m_resPath, resObjOrNull);
                    }
                    loadReq.m_proxy.m_onResLoadDone.Invoke(this, loadReq.m_resPath, resObjOrNull);
                }
            }

            if (m_resLoadList.Count <= 0)
            {
                gameObject.SetActive(false);
            }
        }

        //#托管资源; 
        void markUseRes(string resPath, UnityEngine.Object resObj)
        {
            TexCacheItem citem;
            if (m_cacheCommon.m_dict.TryGetValue(resPath, out citem))
            {
                if (null == citem.m_retainObject)
                {
                    citem.m_retainObject = resObj;
                    m_cacheCommon.m_nowBytes += citem.m_memBytes;

                    printUseMemSize();
                }
            }
            else
            {
                citem = new TexCacheItem();
                m_cacheCommon.m_dict.Add(resPath, citem);
                citem.m_path = resPath;
                citem.m_retainObject = resObj;

                long bytes = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(resObj);//t2d.GetRawTextureData().LongLength;
                citem.m_memBytes = bytes;
                m_cacheCommon.m_nowBytes += bytes;

                printUseMemSize();
            }

            //#托管资源仅仅是加载了, 要被页面使用了才mark;
            //#m_cacheCommon.increaseMarkData(citem);
        }

//        public T_ResType getResOutPageScope<T_ResType>(string resPath) where T_ResType : UnityEngine.Object
//        {
//            T_ResType obj = Resources.Load<T_ResType>(resPath);
//            if (null != obj)
//            {
//                markUseRes(resPath, obj);
//            }
//        }

        //#AsyncState_Data??
        class AsyncLoadData
        {
            public string m_resPath;
            public ResourceRequest m_loadHandler;
            public ResLoadDoneProxy m_proxy;
        }

        //#resObjOrNull为null时表示资源加载失败(资源损坏或资源不存在时就会加载失败);
        public delegate void OnResLoadDone(TexCache sender, string resPath, UnityEngine.Object resObjOrNull);

        public class ResLoadDoneProxy
        {
            public ResLoadDoneProxy(OnResLoadDone cb)
            {
                m_onResLoadDone = cb;
            }

            public void onObjectDestroy()
            {
                m_onResLoadDone = null;
            }

            public void loadDone(TexCache sender, string resPath, UnityEngine.Object res)
            {
                if (null != m_onResLoadDone)
                {
                    m_onResLoadDone.Invoke(sender, resPath, res);
                }
            }

            public OnResLoadDone m_onResLoadDone;
        }

        public void loadResAsync(string resPath, ResLoadDoneProxy proxy)
        {
            if (m_mainThreadID != Thread.CurrentThread.ManagedThreadId)
            {
                throw new ArgumentException("call this on mainThread!");
            }

            gameObject.SetActive(true);

            //TODO: 重复加载检测;

            //#拿到加载句柄;
            ResourceRequest loadHandler = Resources.LoadAsync(resPath);

            AsyncLoadData loadReq = new AsyncLoadData();
            loadReq.m_resPath = resPath;
            loadReq.m_loadHandler = loadHandler;
            loadReq.m_proxy = proxy;

            m_resLoadList.Add(loadReq);
        }

        //************************************************** 异步加载资源 end;

//        //#
//        public void logout()
//        {
//        }

       
    }
}

