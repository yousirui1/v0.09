using UnityEngine;
using System.Collections;

//#运行期间需要保留下来的上下文，比如：用到的一些模块等；
//#好像spring那种di，也有叫ApplicationContext的东西；
using System;
using Pomelo.DotNetClient;


namespace tpgm
{
    public class SavedContext
    {
        
        public static string s_externalPath = "";


        public const string APK_HASH_NAME = "apk_hash_baidu";


		//url = url基地址 + 函数名
		public static string s_basicUrl = "http://121.40.149.87:3000";
		//public static string s_basicUrl = "http://192.168.52.1:3000";

		public static string s_valUrl = "http://121.40.149.87/magician/update.zip";

		//长连接客户端
        public static PomeloClient s_client;
        //#在和game服务器交互了;
        public static bool s_gameServerConnected;

        public static TexCache s_texCache;

        public static ValLoader s_valLoader = new ValLoader();

		//数值表缓存
        public static ValTableCache s_valTableCache;

		//音频缓存
        public static AudioCache s_audioCache = new AudioCache();

        public static Audio2D s_audio2D;

	

        public static Config s_config = new Config();

        //public static LayerManager s_layerMgrImpl;

		//文件名
        public static void setup(string channel)
        {
            if (!String.IsNullOrEmpty(s_externalPath))
            {
                return;
            }

            //GA.StartWithAppKeyAndChannelId("5860b1ef8f4a9d079d0014be" , "App Store");

            //调试时开启日志 发布时设置为false
            //GA.SetLogEnabled(true);

            initExternalPath(channel);

            s_texCache = TexCache.create();
            s_valTableCache = new ValTableCache(s_valLoader);
            s_audio2D = Audio2D.create(MainLooper.instance(), s_audioCache);
            AudioManager.setupAudio(s_audio2D);
        }

		//初始化设置数据目录  game_Data
        static void initExternalPath(string channel)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
        s_externalPath = "/sdcard/fsdzz/";
		s_externalPath = Application.persistentDataPath;
            #elif UNITY_IPHONE && !UNITY_EDITOR
        s_externalPath = "/sdcard/fsdzz/";
            #elif UNITY_STANDALONE_WIN || UNITY_EDITOR
			s_externalPath = Application.dataPath;
            int idx = s_externalPath.LastIndexOf("game_Data");
            if (-1 != idx)
            {
                s_externalPath = s_externalPath.Substring(0, idx);
            }
            #endif

            if (!s_externalPath.EndsWith("/"))
            {
                s_externalPath += "/";
            }

			s_externalPath += channel + "/";
        }

        //#if UNITY_ANDROID && !UNITY_EDITOR
        //    public static string s_externalPath = "/sdcard/demoUnity;
        //#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        //    public static string s_externalPath = Application.persistentDataPath;
        //#endif
        
		//获取game_Data数据路径
        public static string getExternalPath(string subPath)
        {
            if (subPath.StartsWith("/"))
            {
                subPath = subPath.Substring(1);
            }

            return s_externalPath + subPath;
        }

        public static string getApiUrl(string api)
        {
            if (!api.StartsWith("/"))
            {
                throw new ArgumentException(api + " not start with /");
            }

            return s_basicUrl + api;
        }

      
    }
}

