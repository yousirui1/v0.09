using UnityEngine;
using System.Collections;

//#开发期间的调试日志;(debug)
//#外发包的错误日志;(warn, error)
//#外发包的调试日志;(info)
using UnityEngine.UI;
//using log4net;
using System.IO;
using System;


public class Log
{
    //#常见的错误日志;

    //#文件相关的: 文件的创建、读取、删除、移动等的失败；
    //#数据库相关的：数据库创建、open, insert, delete, update, select的失败；
    //#网络相关的：请求失败、请求超时；
    //#服务端相关的：数据格式不对；

    static void TryInitLogConfig()
    {
        if (!s_inited)
        {
            s_inited = true;

//            try
//            {
//                TextAsset text = Resources.Load("Data/log4net") as TextAsset;
//                MemoryStream ms = new MemoryStream();
//                ms.Write(text.bytes, 0, text.bytes.Length);
//                ms.Seek(0, SeekOrigin.Begin);
//                log4net.Config.XmlConfigurator.Configure(ms);
//            }
//            catch (Exception ex)
//            {
//                Log.d<Log>("err: " + ex.Message + ", " + ex.GetType().FullName);
//            }
        }
    }

    public static void d<T_Logger>(string text)
    {
        TryInitLogConfig();

        //if (SavedContext.s_config.m_devLog)
        {
            string tag = typeof(T_Logger).FullName;
            //Debug.Log(tag + ": " + text);

            //LogManager.GetLogger(tag).Debug(text);
        }
    }

    public static void i<T_Logger>(string text)
    {
        TryInitLogConfig();

        string tag = typeof(T_Logger).FullName;
        Debug.Log(tag + ": " + text);

        //LogManager.GetLogger(tag).Info(text);
    }

    public static void w<T_Logger>(string text)
    {
        TryInitLogConfig();

        string tag = typeof(T_Logger).FullName;
        Debug.LogWarning(tag + ": " + text);

        //LogManager.GetLogger(tag).Warn(text);
    }

    public static void e<T_Logger>(string text)
    {
        TryInitLogConfig();

        string tag = typeof(T_Logger).FullName;
        Debug.LogWarning(tag + ": " + text);

        //LogManager.GetLogger(tag).Error(text);
    }

    public static bool s_inited;
}
