using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**************************************
*FileName: ValGlobal.cs
*User: ysr 
*Data: 2018/1/30
*Describe: 全局数值表载入json文件对应的类
**************************************/

namespace tpgm
{
	[Serializable]
	public class ValGlobal : BaseVal
	{
		public string icon = "";
		public string name = "";
		public int type;
		public int sid;
		public int start;
		public string add = "";
		public string rel = "";

	}

	public class val_global
	{
		public List<ValGlobal> m_list;

		ValTableCache m_initedValTableCache = SavedContext.s_valTableCache;

		public val_global()
		{
			m_list = m_initedValTableCache.getValListOutPageScopeOrThrow<ValGlobal> (ConstsVal.val_global);
		}

		public ValGlobal getObj(int id)
		{
			for (int i = 0; i < m_list.Count; i++) {
				if (m_list [i].id == id) {
					ValGlobal obj = m_list [i];
					return obj;
				}
			}
			return null;	
		}

		public string getIcon(int id)
		{
			for (int i = 0; i < m_list.Count; i++) {
				if (m_list [i].id == id) {
					ValGlobal obj = m_list [i];
					return obj.icon;
				}
			}
			return null;
		}



	}
}
