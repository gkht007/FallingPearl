using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用：所有SDK平台的管理类，调用IsEnable开启
/// 开发： 1.新的SDK平台需要新建相应SDK平台的Class并且继承ISDK
///        2.在RegistAllSdk()函数里注册新的SDK平台
///        
/// 注： 广告SDK请跳转AdSdk类去查看规范
/// </summary>
public class AllSDKManager
{
    static bool isDebug = true;
    /// <summary>
    /// SDK日志开关(默认开启)
    /// </summary>
	public static bool IsDebug
    {
        get
        {
            return isDebug;
        }
        set
        {
            isDebug = value;
        }
    }


    static void RegistAllSdk()
    {
        //Bugly注册
        RegisteSDK(typeof(BuglyTT), new BuglyTT());
        //注册例子
        RegisteSDK(typeof(AdSdk), AdSdk.Instance);
        //新的SDK在这里注册
        RegisteSDK(typeof(AllAnalyticsSdk), new AllAnalyticsSdk());
        RegisteSDK(typeof(UnityIAP), new UnityIAP());

    }

    static Dictionary<Type, ISDK> allSDKDic = new Dictionary<Type, ISDK>();

    /// <summary>
    /// 初始化所有SDK平台
    /// </summary>
    public static void Init()
    {
        RegistAllSdk();
        foreach (ISDK _values in allSDKDic.Values)
        {
            _values.Init();
            SDKDebug(_values.Name + "初始化");
        }

    }

    /// <summary>
    /// 注册SDK平台
    /// </summary>
    /// <param name="key"></param>
    /// <param name="iSdk"></param>
    /// <returns></returns>
    static bool RegisteSDK(Type key, ISDK iSdk)
    {
        if (!allSDKDic.ContainsKey(key))
        {

            allSDKDic.Add(key, iSdk);
            SDKDebug(allSDKDic[key].Name + "注册成功");
            return true;
        }
        return false;
    }

    bool UnRegistSDK(Type key)
    {
        if (allSDKDic.ContainsKey(key))
        {
            allSDKDic[key].Disable();
            allSDKDic.Remove(key);
            SDKDebug(allSDKDic[key].Name + "注销成功");
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取指定的已注册平台
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetSDK<T>() where T : ISDK
    {
        return (T)allSDKDic[typeof(T)];
    }

    /// <summary>
    /// 指定销毁已经注册的平台
    /// </summary>
    /// <param name="_key">AdEnum</param>
    public void DestroySDK(Type _key)
    {
        UnRegistSDK(_key);
    }

    /// <summary>
    /// 销毁所有已经注册的平台
    /// </summary>
    public void DestroyAllSDK()
    {
        foreach (Type key in allSDKDic.Keys)
        {
            DestroySDK(key);
        }
    }

    /// <summary>
    /// 平台日志
    /// </summary>
    /// <param name="logMsg"></param>
    public static void SDKDebug(string logMsg)
    {
        if (IsDebug)
            Debug.Log("SDK日志：" + logMsg + "\n");
    }
    public static void SDKDebug(string logMsg, string color = "green")
    {
        if (IsDebug)
            Debug.Log("<color=" + color + ">SDK日志：" + logMsg + "\n</color>");
    }
    /// <summary>
    /// 按钮点击播放广告回调
    /// </summary>
    /// <param name="logMsg"></param>	
    public static void AdClickEvent(Transform trans, int AdType)
    {
        switch (AdType)
        {
            case 1:
                GetSDK<AdSdk>().ShowBanner();
                break;
            case 2:
                GetSDK<AdSdk>().ShowInterstitial();
                break;
            case 3:
                GetSDK<AdSdk>().ShowRewardedVideo();
                break;
        }
    }

}



/// <summary>
/// 广告播放计数
/// </summary>
public class AdShowControl
{
    public static int GameContinueCountNumber;
    public static int CompleteRetryCountNumber;
    public static int CompleteGameCountNumber;

}







