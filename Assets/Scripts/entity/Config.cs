using UnityEngine;
using System.Collections;

/**************************************
*FileName: Config.cs
*User: ysr 
*Data: 2018/1/24
*Describe: 配置数据类
**************************************/

//#配置数据，包括根据不同渠道进行的配置数据等；
public class Config
{
    //#外网出现问题时，用于显示开发日志来确定闪退位置的；
    public bool m_devLog = true;

    ////#服务器地址；外网正式服务器、测试服务器还是外网测试服务器；
    public int m_serverType = 0;

    //#根据渠道配置的登录sdk类型；
    public string m_loginSdkType = "";

    //#根据渠道配置的支付sdk类型；
    public string m_paySdkType = "";

    //#显示fps;
    public bool m_fps;

    //#关闭apk更新;
    public bool m_noApkUpdate;

    //#关闭增量更新;
    public bool m_noUpdate;

    //#关闭引导;
    public bool m_noGuide;

    //#固定帐号
    public string m_uid = "";
}
