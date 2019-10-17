using System;
using System.Collections.Generic;

public class AllAnalyticsSdk : ISDK, IAnalythics
{

    public Dictionary<Type, IAnalythics> analythicsSdkDic;//所有分析SDK
    public string Name
    {
        get
        {
            return "AllAnalyticsSdk";
        }
    }
    bool isLookAd = false;
    bool isUseHint = false;
    public bool IsLookAd
    {
        get
        {
            return isLookAd;
        }
        set
        {
            foreach (Type item in analythicsSdkDic.Keys)
            {
                analythicsSdkDic[item].IsLookAd = value;
            }
            isLookAd = value;
        }
    }

    public bool IsUseHint
    {
        get
        {
            return isUseHint;
        }
        set
        {
            foreach (Type item in analythicsSdkDic.Keys)
            {
                analythicsSdkDic[item].IsUseHint = value;
            }
            isUseHint = value;
        }
    }
    public void Disable()
    {

    }

    #region Init
    public AllAnalyticsSdk()
    {
        analythicsSdkDic = new Dictionary<Type, IAnalythics>();
        RegistAllAdSdk();
    }
    public void Init()
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Init();
            AllSDKManager.SDKDebug(Name + " " + analythicsSdkDic[item].Name + "初始化");
        }
    }
    void RegistAllAdSdk()
    {
        //这里注册新添加的
        RegistAnalyticsSdk(typeof(UmengSdk), UmengSdk.Instance());
        //RegistAnalyticsSdk(typeof(AppsFlyerManager), AppsFlyerManager.Instance);
        RegistAnalyticsSdk(typeof(NativeAnalytics), NativeAnalytics.Instance);
        RegistAnalyticsSdk(typeof(NativeAdAnalytics), NativeAdAnalytics.Instance);
    }

    void RegistAnalyticsSdk(Type _analythicsType, IAnalythics _iAnalythics)
    {
        if (!analythicsSdkDic.ContainsKey(_analythicsType))
        {
            analythicsSdkDic.Add(_analythicsType, _iAnalythics);
            AllSDKManager.SDKDebug(Name + " " + analythicsSdkDic[_analythicsType].Name + "注册成功");
        }
        else
        {
            AllSDKManager.SDKDebug(Name + " 已经注册过这个平台" + _analythicsType.Name);
        }
    }



    #endregion
    /// <summary>
    /// 获取已接入的分析SDK
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetAnalythicsSdk<T>() where T : IAnalythics
    {
        return (T)analythicsSdkDic[typeof(T)];
    }

    public void Event_Ad_End(string _adTeam, AdPlayType _AdPlayType)
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_Ad_End(_adTeam, _AdPlayType);
        }
    }

    public void Event_Ad_End(string _adTeam, AdPlayType _AdPlayType, string _failed = "")
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_Ad_End(_adTeam, _AdPlayType, _failed);
        }
    }

    public void Event_Ad_Start(string _adTeam, AdPlayType _AdPlayType, bool _clear = true)
    {
        if (_AdPlayType == AdPlayType.Banner)
            _clear = true;
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_Ad_Start(_adTeam, _AdPlayType, _clear);
        }
    }

    public void Event_Button(string _page, string _buttonName)
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_Button(_page, _buttonName);
        }
    }

    public void Event_IAP_End()
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_IAP_End();
        }
    }

    public void Event_IAP_End(string _failed = "")
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_IAP_End(_failed);
        }
    }

    public void Event_IAP_Start(string _name)
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_IAP_Start(_name);
        }
    }

    public void Event_Level_Enter(string _name, string _where)
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_Level_Enter(_name, _where);
        }
    }

    public void Event_Level_Leave(string _name, int _pass, string _leaveCause = "")
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_Level_Leave(_name, _pass, _leaveCause);
        }
    }

    public void Event_ServerAPI(string _api, string _failed = "未知原因")
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_ServerAPI(_api, _failed);
        }
    }

    public void Event_NativeAd(string _adTeam, string _behavior, string _bannerName, string _jumpUrl)
    {
        foreach (Type item in analythicsSdkDic.Keys)
        {
            analythicsSdkDic[item].Event_NativeAd(_adTeam, _behavior, _bannerName, _jumpUrl);
        }
    }
}
