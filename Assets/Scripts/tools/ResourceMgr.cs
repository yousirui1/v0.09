using UnityEngine;
using System.Collections;
/**************************************
*FileName: ResourceMgr.cs
*User: ysr 
*Data: 2018/3/2
*Describe: 资源载入封装函数重复资源放入缓存 
*			cache = true
**************************************/

public class ResourceMgr : MonoBehaviour
{
	#region 初始化
	private static ResourceMgr m_instance;
	
	//获取资源加载实例
	public static ResourceMgr Instance()
	{
		if(m_instance == null)
		{
			m_instance = new GameObject("ResourceMgr").AddComponent<ResourceMgr>();	
		}
		return m_instance;
	}


	private ResourceMgr()
	{
		hashtable = new Hashtable();
	}

	#endregion

	//资源缓存容器
	private Hashtable hashtable;
	
	
	//Load 资源
	public T Load<T>(string path, bool cache) where T : UnityEngine.Object
	{
		if(hashtable.Contains(path))
		{
			return hashtable[path] as T;
		}

		//Debug.Log(string.Format("Load asset from resouce folder, path:{0}, cacje:{1}",path,cache));
		T assetObj = Resources.Load<T>(path);
		if(assetObj == null)
		{
			Debug.LogWarning("Resouce中找不到资源:" + path);

		}	
		
		if(cache)
		{
			hashtable.Add(path, assetObj);			
			//Debug.Log("Asset对象被缓存 path =" + path);

		}
		return assetObj;	

	}


	
	//创建Resouce中GameObject对象 bool 是否重复使用
	public GameObject CreateGameObject(string path, bool cache)
	{
		UnityEngine.GameObject assetObj = Load<GameObject>(path, cache);
		GameObject newObj = UnityEngine.Object.Instantiate(assetObj) as GameObject;
        if (newObj == null)
		{
			Debug.LogWarning("从Resource创建对象失败:"+path);
		}		
		return newObj;
	
	}

}
