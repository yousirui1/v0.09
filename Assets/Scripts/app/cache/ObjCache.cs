using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJson;
using System;
using UnityEngine.UI;
using tpgm;


/**************************************
*FileName: LoadingLabel.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 动态载入效果
**************************************/


namespace tpgm
{
	public class ObjCache : MonoBehaviour
	{
		#if false
		private static readonly string TAG = typeof(Map).FullName;

		MainLooper m_initedLooper;
		
		//在对象释放时调用一下OnObjectDestroy
		MessageHandlerProxy m_msgHandlerProxy;
		

		public static GameObject create(MainLooper looper)
		{
			GameObject go = new GameObject("ObjCache");
			MonoBehaviour.DontDestroyOnLoad(go);
			Map map = go.AddComponent<Map>();
			map.setup(looper, objCache);
		}			

		class MsgData
		{

		}
		
		void handleMsg(HandlerMessage msg)
		{
			switch(msg.m_what)
			{

			}

		}


		//初始化
		void setup(MainLooper looper, ObjCache objCache)
		{
			if(looper == null)
			{
				throw new ArgumentException("MainLooper not setup, it's null");
			}

			m_initedLooper = looper;
			
		}

		//只在部分游戏使用的特有资源
		public GameObject useScopeObj(string objPath)
		{

		}
	
		//对缓存检查
		ObjCacheItem checkGameBeginUseCalled(string instanceID, string objPath)
		{
			ObjCacheItem citem;
			if(m_cacheCommon.m_dict.TryGetValue(audioPath, out citem))
			{
				//检查id的合法性
				if(!citem.m_retainPages.Contains(instanceID))
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

		//所以游戏场景都用到的资源
		public GameObject useGlobalObj(string objPath)
		{
			ObjCacheItem citem = getCacheItem(objPath);
			
			//获取当前时间纳秒
			long now = TimeUtils.utcNowMs();
			citem.increaseUseCnt(now);
			return citem.m_retainObject;	
		}


		//获得缓存
		ObjCacheItem getCacheItem(string objPath)
		{
			ObjCacheItem citem;
			
			if(!m_cacheCommon.m_dict.TryGetValue(objPath, out citem)
			{

			}

			if(null == citem.m_retainObject)
			{
				GameObject res = Resouces.Load<GameObject>(objPath);

				if(null == res)
				{
					return null;
				}
				else
				{
					citem.m_retainObject =res;
					
					if(citem.m_memBytes <= 0)
					{
						citem.m_memBytes = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(res);
					}
					m_cacheCommon.m_nowBytes += citem.m_memBytes;
				}
			}

			return citem;
		}

		//装载资源	
		public void markGameUse(string instanceID, string objPath)
		{
			ObjCacheItem citem = getCacheItem(objPath);
			
			if(null != citem)
			{
				m_cacheCommon.increaseMarkData(citem);
				if(citem.m_retainPages.Contains(instanceID))
				{
					throw new ArgumentException(objPath + " has marked by this page:" + instaneID);
				}
				else
				{
					citem.m_retainPages.Add(instanceID);	
				}
			}
			else
			{
				//warn : 加载失败;
			}
		}	

		//卸载资源
		public void unmarkGameUse(string instanceID, string objPath)
		{
			m_cacheCommon.unmarkPageUse(instanceID, objPath);
		}
	
		//检查
		public void checkDoQuickGc()
		{
			long now = TimeUtils.utcNowMs();
			long diffMs = now - m_cacheCommon.m_lastGcMs;
			if(diffMs >= m_cacheCommon.m_minGcInterval)
			{
				quickGc();
			
				m_cacheCommon.m_lastGcMs = now;
			}
		}

		public void quickGc()
		{
			if(m_cacheCommon.m_nowBytes <= m_cacheCommon.m_gcBytes)
			{
				return;
			}

			m_cacheCommon.findNoPageRetainItems();
			
			int gcCount = 0;
			for(int = m_cacheCommon.m_tmpList.Count - 1; i >= 0; i--)
			{
				if(m_cacheCommon.m_nowBytes <= m_cacheCommon.m_gcBytes)
				{
					break;
				}

				gcCount++;
				ObjCacheItem citem = m_cacheCommon.m_tmpList[i];
				m_cacheCommon.m_nowBytes -= citem.m_memBytes;
				citem.releaseRetain();
			}
			
			m_cacheCommon.m_tmpList.Clear();

			
			if(gcCount > 0)
			{
				AsyncOperation asy = Resouces.UnloadUnusedAssets();
			}

		}
	
		//缓存满了,清空
		public void fullGc()
		{
			m_cacheCommon.findNoPageRetainItems();
			
			int gcCount = 0;
			
			for(int i = m_cacheCommon.m_tmpList.Count - 1; i >= 0; i--)
			{

			}

			m_cacheCommon.m_tmpList.Clear();
		
			if(gcCount > 0)
			{
				AsyncOperation asy = Resource.UnloadUnusedAssets();
			}
		}
		#endif
	}
}

