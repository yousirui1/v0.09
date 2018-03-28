using UnityEngine;
using System.Collections;
using System.IO;
using System;
using ProtoBuf;
using System.Collections.Generic;
using tpgm;

/**************************************
*FileName: AppUtils.cs
*User: ysr 
*Data: 2018/1/24
*Describe: App信息类
**************************************/


namespace tpgm
{
    public class AppUtils
    {

        private static readonly string TAG = typeof(AppUtils).FullName;

        public static string s_verName = "1.0.0";
        public static int s_verCode = 1;

        public const float Canvas_Width = 1280.0f;
        public const float Canvas_Height = 720.0f;

        public const bool Dev_Mode = false;



        //#当分辨率变小或变大时, 触摸时得到的screenPos也会错位, 这个用来修正的;
        public static Vector2 screenPosToCanvasPos(Vector2 screenPos)
        {
            Vector2 outPos = Vector2.zero;
            outPos.x = screenPos.x * Canvas_Width / Screen.width;
            outPos.y = screenPos.y * Canvas_Height / Screen.height;

            return outPos;
        }

        //获取checkID 用于重连
        public static string apiCheckID(string apiName)
        {
			return apiName + "_" + Utils.uuid();
        }



        /// 获取显示在启动界面的版本标识字符串;
        public static string GetVerStr()
        {
            try
            {   
                //版本
                Version verObj = parseVersionFileOrThrow();
                string retStr = verObj.m_verName;
                //资源版本
                if (verObj.m_resVer.Length > 0)
                {
                    retStr += "-";
                    retStr += verObj.m_resVer; 
                }

                //版本类型
                Config configObj = SavedContext.s_config;

                if (0 != configObj.m_serverType)
                {
                    string[] serverNames = { "正式", "测试外网", "测试内网" };
                    retStr += "-";
                    retStr += serverNames[configObj.m_serverType];
                }

                if (configObj.m_devLog)
                {
                    retStr += "-devLog";
                }

                return retStr;
            }
            catch (IOException ex)
            {

            }

            return "";
        }

        //解析版本文件
        static Version parseVersionFileOrThrow()
        {
            TextAsset txtFile = Resources.Load<TextAsset>("game_Data/data/Version");
            if (null == txtFile)
            {
                throw new IOException("Version File not exists");
            }

            Version ret = new Version();
            //计数  3个字段都成功才返回成功
            int remainItemsToParse = 3;
            //读文件
            using (StringReader sr = new StringReader(txtFile.text))
            {
                string line;

                while (null != (line = sr.ReadLine()))
                {
                    //去掉字符串左侧和右侧的空格
                    line = line.Trim();
                    //#开头的内容跳过  
                    if (line.Length <= 0 || line.StartsWith("#"))
                    {
                        continue;
                    }
                    //查找字符串= 失败返回-1
                    int idx = line.IndexOf("=");
                    if (-1 != idx)
                    {
                        //按照长度查找beginIndex endIndex
                        string key = line.Substring(0, idx);
                        string value = line.Substring(idx + 1);

                        switch (key)
                        {
                        case "verName":
                        {
                            ret.m_verName = value;
                            remainItemsToParse--;
                        }
                        break;

                        case "verCode":
                        {
                            try
                            {
                                int i = int.Parse(value);
                                ret.m_verCode = i;
                                remainItemsToParse--;
                            }
                            catch (FormatException ex)
                            {
                            }
                        }
                        break;

                        case "resVer":
                        {
                            ret.m_resVer = value;
                            remainItemsToParse--;
                        }
                        break;

                        }


					}
                    else
                    {
                        Log.w<AppUtils>("unknown line: " + line);
                    }
                }
            }
            //3个字段都解析成功则返回
            if (0 == remainItemsToParse)
            {
				return ret;
            }
            ret.m_verName = "unknown";

            return ret; //#没有完全解析成功;
        }

        //解析配置文件
        public static void parseConfig(string configText, Config outConfig)
        {
            using (StringReader sr = new StringReader(configText))
            {
                string line;

                while (null != (line = sr.ReadLine()))
                {
                    line = line.Trim();

                    if (line.Length <= 0 || line.StartsWith("#"))
                    {
                        continue;
                    }

                    int idx = line.IndexOf("=");
                    if (-1 != idx)
                    {
                        string key = line.Substring(0, idx);
                        string value = line.Substring(idx + 1);

                        Log.d<AppUtils>(key + ": " + value);

                        switch (key)
                        {
                        case "server":
                        {
                            try
                            {
                                int i = int.Parse(value);

                                if (i >= 0 && i <= 2)
                                {
                                    //外网测试，内网测试，正式版
                                    outConfig.m_serverType = i;
                                }
                            }
                            catch (FormatException ex)
                            {
                            }
                        }
                        break;

                        //设备打印存放的地址
                        case "devLog":
                        {
                            try
                            {
                                int i = int.Parse(value);
                                outConfig.m_devLog = (i != 0);
                            }
                            catch (FormatException ex)
                            {
                            }
                        }
                        break;

                        case "fps":
                        {
                            try
                            {
                                int i = int.Parse(value);
                                outConfig.m_fps = (i != 0);
                            }
                            catch (FormatException ex)
                            {
                            }
                        }
                        break;

                        case "uid":
                        {
                            outConfig.m_uid = value;
                        }
                        break;

                        //帮助的版本
                        case "noGuide":
                        {
                            try
                            {
                                int i = int.Parse(value);
                                outConfig.m_noGuide = (i != 0);
                            }
                            catch (FormatException ex)
                            {
                            }
                        }
                        break;

                        case "noUpdate":
                        {
                            try
                            {
                                int i = int.Parse(value);
                                outConfig.m_noUpdate = (i != 0);
                            }
                            catch (FormatException ex)
                            {
                            }
                        }
                        break;
                        
                        //热更新
                        case "noApkUpdate":
                        {
                            try
                            {
                                int i = int.Parse(value);
                                outConfig.m_noApkUpdate = (i != 0);
                            }
                            catch (FormatException ex)
                            {
                            }
                        }
                        break;

                        default:
                        {
                            Log.w<AppUtils>("unknown key: " + key + ", line: " + line);
                        }
                        break;

                        }
                    }
                    else
                    {
                        Log.w<AppUtils>("unknown line: " + line);
                    }
                }
            }
        }

       
    }
}
