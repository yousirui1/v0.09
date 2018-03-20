using System;
using UnityEngine;

/**************************************
*FileName: PrefValUpdate.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 本地持久数据更新
**************************************/


namespace tpgm
{
	public class PrefValUpdate
	{
		private const string PREF_NAME = "pref_val_";
		
		private const string KEY_LAST_MODIFIED = "last_modified";

		private const string KEY_ETAG = "etag";
	
		public static string getIfModifiedSince()
		{
			string ret = PlayerPrefs.GetString(PREF_NAME + KEY_LAST_MODIFIED, "");
			return ret;
		}

		public static string getETag()
		{
			string ret = PlayerPrefs.GetString(PREF_NAME + KEY_ETAG, "");
			return ret;
		}


		public static void saveFileModifyData(string ifModifiedSince, string etag)
		{
			PlayerPrefs.SetString(PREF_NAME + KEY_LAST_MODIFIED, ifModifiedSince);
			PlayerPrefs.SetString(PREF_NAME + KEY_ETAG, etag);
			PlayerPrefs.Save();
		}

		public static void clear()
		{
			PlayerPrefs.DeleteAll();
		}

		private PrefValUpdate()
		{

		}
	}	


}
