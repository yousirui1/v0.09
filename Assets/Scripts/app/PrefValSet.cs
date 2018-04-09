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

		private const string KEY_UID = "uid";

		private const string KEY_PASSWD = "passwd";

		private const string KEY_BTN_HID = "btn_hid";

	


		public static void saveUid(string uid)
		{
			PlayerPrefs.SetString(PREF_NAME + KEY_UID, uid);
			PlayerPrefs.Save();
		}


		public static void savePasswd(string passwd)
		{
			PlayerPrefs.SetString(PREF_NAME + KEY_PASSWD, passwd);
			PlayerPrefs.Save();
		}



		public static void saveButtonHid(int iActive)
		{
			PlayerPrefs.SetInt(PREF_NAME + KEY_BTN_HID, iActive);
			PlayerPrefs.Save();
		}


		public static bool isLogin()
		{
			if (getUid ().Equals (string.Empty))
				return false;
			else
				return true;
		}

		public static string getUid()
		{
			string ret = PlayerPrefs.GetString(PREF_NAME + KEY_UID, "");
			return ret;
		}

		public static string getPasswd()
		{
			string ret = PlayerPrefs.GetString(PREF_NAME + KEY_PASSWD, "");
			return ret;
		}

	

		public static int getButtonHid()
		{
			int ret = PlayerPrefs.GetInt(PREF_NAME + KEY_BTN_HID);
			return ret;
		}

	
	
			

		public static void clear()
		{
			PlayerPrefs.DeleteAll();
		}

		private PrefValSet()
		{

		}
	}	


}
