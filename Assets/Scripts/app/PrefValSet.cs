using System;
using UnityEngine;

/**************************************
*FileName: PrefValUpdate.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 本地持久数据用户设置
**************************************/


namespace tpgm
{
	public class PrefValSet
	{
		private const string PREF_NAME = "pref_val_";

		private const string KEY_LAST_MODIFIED = "last_modified";

		private const string KEY_ETAG = "etag";

	


		public static void saveButtonHid(string ifModifiedSince, string etag)
		{
			PlayerPrefs.SetString(PREF_NAME + KEY_LAST_MODIFIED, ifModifiedSince);
			PlayerPrefs.SetString(PREF_NAME + KEY_ETAG, etag);
			PlayerPrefs.Save();
		}


		public static string getButtonHid()
		{
			string ret = PlayerPrefs.GetString(PREF_NAME + KEY_ETAG, "");
			return ret;
		}

	

		#if false
		//语音设置
		public static string getButtonHid()
		{
			string ret = PlayerPrefs.GetString(PREF_NAME + KEY_ETAG, "");
			return ret;
		}


		public static void saveButtonHid(string ifModifiedSince, string etag)
		{
			PlayerPrefs.SetString(PREF_NAME + KEY_LAST_MODIFIED, ifModifiedSince);
			PlayerPrefs.SetString(PREF_NAME + KEY_ETAG, etag);
			PlayerPrefs.Save();
		}
		#endif
			

		public static void clear()
		{
			PlayerPrefs.DeleteAll();
		}

		private PrefValSet()
		{

		}
	}	


}
