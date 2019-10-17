using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 腾讯异常捕获SDK
/// </summary>
public class BuglyTT : ISDK
{
#if UNITY_EDITOR
    string appId = "cc0f73f3d8";
    string appKey = "451a364a-dc8b-41ce-91d0-2ba26c4f36cd";
    string channel = "UnityEditor";
#elif UNITY_ANDROID
     string appId = "cc0f73f3d8";
    string appKey = "451a364a-dc8b-41ce-91d0-2ba26c4f36cd";
     string channel = "googleplay";
#elif UNITY_IOS
      string appId = "ce6b8a3336";
    string appKey = "bb08bfc7-265e-43ad-8889-16ef7e032fed";
     string channel = "appstore";
#endif
    public string Name
    {
        get
        {
            return "BuglyTT";
        }
    }

    public void Disable()
    {
      
    }

    public void Init()
    {
        BuglyAgent.ConfigDebugMode(true);
        BuglyAgent.ConfigAutoReportLogLevel(LogSeverity.LogWarning);
        BuglyAgent.ConfigDefault(channel, "", "", 0);
        BuglyAgent.InitWithAppId(appId);//初始化


        // BuglyAgent.SetUserId(Config.UserId);

        BuglyAgent.EnableExceptionHandler();//启动异常上报
    }


}
