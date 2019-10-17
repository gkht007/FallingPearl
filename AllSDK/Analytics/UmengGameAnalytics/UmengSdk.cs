using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Umeng;
using System;
using MyUtili;
using System.Runtime.Remoting.Contexts;

public class UmengSdk : Singletion<UmengSdk>, IAnalythics
{
#if UNITY_EDITOR
    public const string appKey = "5d50ed16570df300280004b3";
    public const string channelId = "UMeng_Editor";
#elif UNITY_ANDROID
    public const string appKey = "5d50ed16570df300280004b3";
    public const string channelId = "google";
#elif UNITY_IOS
    public const string appKey = "5d50ee693fc1958b1f000618";
    public const string channelId = "appstore";
#endif


    #region EventId 
    const string button = "button";
    const string level = "level";
    const string test = "test";
    const string IAP = "IAP";
    const string Ad = "Ad";
    const string serverIPA = "serverIPA";
	#endregion
	string deveceId = "";
	#region Data
	Dictionary<string, string> AdDic;
    Dictionary<string, string> IAPDic;

    //用于计算关卡进入到离开的时间差
    TimeSpan timeStart;
    TimeSpan timeEnd;
    #endregion

    /// <summary>
    /// 记录是否观看过广告
    /// </summary>
    bool isLookAd = false;

    /// <summary>
    /// 记录是否使用过提示
    /// </summary>
    bool isUseHint = false;

    public string Name
    {
        get
        {
            return "Umeng";
        }
    }

    public bool IsLookAd
    {
        get
        {
            return isLookAd;
        }

        set
        {
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
            isUseHint = value;
        }
    }

    public void Disable()
    {

    }
    public UmengSdk()
    {
        AdDic = new Dictionary<string, string>();
        IAPDic = new Dictionary<string, string>();
    }
    public void Init()
    {
        GA.StartWithAppKeyAndChannelId(appKey, channelId);
        GA.SetLogEnabled(false);

        MyUtilities.initUUID();
        deveceId = MyUtilities.getUUID();
    }

    void SetEvent(string eventId, Dictionary<string, string> eventDic)
    {
        //AllSDKManager.SDKDebug("目前友盟暂时停用:" + eventId);
        AllSDKManager.SDKDebug("eventId:" + eventId);
        GA.Event(eventId, eventDic);
    }


	/// <summary>
	/// 进入图片埋点
	/// </summary>
	/// <param name="_name">图片名称</param>
	/// <param name="_where">图片位置</param>
	public void Event_Level_Enter(string _name,string _where)
    {
        Dictionary<string, string> levelEnterDic = new Dictionary<string, string>();

        timeStart = new TimeSpan(DateTime.Now.Ticks);

        levelEnterDic["Level_Enter"] =   "设备ID:" + deveceId +"从"+ _where+ " | 进入:第" + _name + "关" + " | 进入时间:" + System.DateTime.Now;
        SetEvent(level, levelEnterDic);
        AllSDKManager.SDKDebug(Name + "设备ID:" + deveceId +"从" + _where+ " | 进入:第" + _name + "关" + " | 进入时间:" + System.DateTime.Now);
    }
	/// <summary>
	/// 离开图片埋点
	/// </summary>
	/// <param name="_level">图片名称</param>
	/// <param name="_pass">是否完成(是/否)</param>
	/// <param name="_leaveCause">离开原因</param>
	public void Event_Level_Leave(string _name, int _pass, string _leaveCause = "")
    {
        //处理得到的数据
        string useHint = isUseHint ? "1" : "0";
        string lookAd = isLookAd ? "1" : "0";
        string pass = _pass.ToString();
        string useTime = "";
        timeEnd = new TimeSpan(DateTime.Now.Ticks);

        TimeSpan timeResult = timeStart.Subtract(timeEnd).Duration();

        if (timeResult.Days > 0)
            useTime += timeResult.Days + "d";
        if (timeResult.Hours > 0)
            useTime += timeResult.Hours + "h";
        if (timeResult.Minutes > 0)
            useTime += timeResult.Minutes + "m";
        useTime += timeResult.Seconds + "s";
        //发送到友盟
        Dictionary<string, string> levelLeaveDic = new Dictionary<string, string>();
        levelLeaveDic["Level_Leave"] = "设备ID:" + deveceId + " | 离开第:" + _name + "关" + " | 离开时间:" + DateTime.Now + " | 通过：" + pass + " | 用时：" + useTime +  " | 使用提示：" + useHint + " | 观看广告:" + lookAd + " | 离开原因:" + _leaveCause ;
        SetEvent(level, levelLeaveDic);
        AllSDKManager.SDKDebug(Name + "设备ID:" + deveceId + " | 离开第:" + _name + "关" + " | 离开时间:" + DateTime.Now + " | 通过：" + pass + " | 用时：" + useTime +  " | 使用提示：" + useHint + " | 观看广告:" + lookAd + " | 离开原因:" + _leaveCause );

        //用完后清零状态
        isLookAd = false;
        isUseHint = false;
    }
    /// <summary>
    /// 按钮点击埋点
    /// </summary>
    /// <param name="_page">页面</param>
    /// <param name="_buttonName">按钮名</param>
    public void Event_Button(string _page, string _buttonName)
    {
        Dictionary<string, string> buttonDic = new Dictionary<string, string>();
        buttonDic["Button_Click"] = "UserId:" + "设备ID:" + deveceId + " | 页面：" + _page + " | 按钮：" + _buttonName + " | 点击时间:" + System.DateTime.Now;
        SetEvent(button, buttonDic);
        AllSDKManager.SDKDebug(Name + "UserId:" + "设备ID:" + deveceId + " | 页面：" + _page + " | 按钮：" + _buttonName + " | 点击时间:" + System.DateTime.Now);
    }

    /// <summary>
    /// 内购开始埋点(此方法必须与Event_IAP_End函数成对使用,否则不会发送到友盟)
    /// </summary>
    /// <param name="_name">内购项目名</param>
    public void Event_IAP_Start(string _name)
    {

        //记录前清空缓存
        IAPDic.Clear();

        //开始记录埋点
        string buyMsg =  "设备ID:" + deveceId + " | 内购项目：" + _name + " | 开始时间:" + System.DateTime.Now;
        AllSDKManager.SDKDebug(Name +  "设备ID:" + deveceId + " | 内购项目：" + _name + " | 开始时间:" + System.DateTime.Now);
        IAPDic[_name] = buyMsg;

    }


    /// <summary>
    /// 内购结束埋点(此方法必须与Event_IAP_Start函数成对使用,否则不会发送到友盟)
    /// 注：成功使用此方法
    /// </summary> 
    public void Event_IAP_End()
    {
        string buyMsg = "";
        string key = "";

        if (IAPDic == null || IAPDic.Count <= 0)
            return;

        foreach (string _key in IAPDic.Keys)
        {
            key = _key;
        }

        buyMsg = IAPDic[key];

        buyMsg += " | 购买情况：成功" + " | 结束时间:" + System.DateTime.Now;
        AllSDKManager.SDKDebug(Name + " | 购买情况：成功" + " | 结束时间:" + System.DateTime.Now);

        IAPDic[key] = buyMsg;

        //记录完成发送到友盟后台
        if (IAPDic != null && IAPDic.Count > 0)
            SetEvent(IAP, IAPDic);

    }
    /// <summary>
    /// 内购结束埋点(此方法必须与Event_IAP_Start函数成对使用,否则不会发送到友盟)
    ///  注：失败使用此方法
    /// </summary>
    /// <param name="_failed">失败原因</param>
    public void Event_IAP_End(string _failed = "")
    {
        string buyMsg = "";
        string key = "";

        if (IAPDic == null || IAPDic.Count <= 0)
            return;

        foreach (string _key in IAPDic.Keys)
        {
            key = _key;
        }
        buyMsg = IAPDic[key];

        buyMsg += " | 购买情况：失败" + " | " + "失败原因:" + _failed + " | 结束时间:" + System.DateTime.Now;
        AllSDKManager.SDKDebug(Name + " | 购买情况：失败" + " | " + "失败原因:" + _failed + " | 结束时间:" + System.DateTime.Now);
        IAPDic[key] = buyMsg;

        //记录完成发送到友盟后台
        if (IAPDic != null && IAPDic.Count > 0)
            SetEvent(IAP, IAPDic);

    }


    /// <summary>
    /// 广告播放开始埋点(此方法必须与Event_Ad_End函数成对使用,否则不会发送到友盟)
    /// </summary>
    /// <param name="_adTeam">广告商</param>
    /// <param name="_AdPlayType">广告类型</param>
    public void Event_Ad_Start(string _adTeam, AdPlayType _AdPlayType,bool _clear)
    {

        string AdPlayType = _AdPlayType.ToString();
        //记录前清空缓存
        AdDic.Clear();

        //开始记录埋点
        string adPlayMsg = "设备ID:" + deveceId + " | 广告商:" + _adTeam + " | 广告类型:" + _AdPlayType + " | 播放时间:" + System.DateTime.Now;
        AllSDKManager.SDKDebug(Name +  "设备ID:" + deveceId + " | 广告商:" + _adTeam + " | 广告类型:" + _AdPlayType + " | 播放时间:" + System.DateTime.Now);
        AdDic[_adTeam] = adPlayMsg;

    }


    /// <summary>
    /// 广告播放结束埋点(此方法必须与Event_Ad_Start函数成对使用,否则不会发送到友盟)
    /// 注：成功使用此方法
    /// </summary> 
    public void Event_Ad_End(string _adTeam, AdPlayType _AdPlayType)
    {
        string adPlayMsg = "";
        string key = "";

        if (AdDic == null || AdDic.Count <= 0)
            return;

        foreach (string _key in AdDic.Keys)
        {
            key = _key;
        }

        adPlayMsg = AdDic[key];

        adPlayMsg += " | 播放结果：成功" + " | 关闭时间:" + System.DateTime.Now;
        AllSDKManager.SDKDebug(Name + adPlayMsg);

        AdDic[key] = adPlayMsg;

        //记录完成发送到友盟后台
        if (AdDic != null && AdDic.Count > 0)
            SetEvent(Ad, AdDic);

    }
    /// <summary>
    /// 广告播放结束埋点(此方法必须与Event_IAP_Start函数成对使用,否则不会发送到友盟)
    ///  注：失败使用此方法
    /// </summary>
    /// <param name="_failed">失败原因</param>
    public void Event_Ad_End(string _adTeam, AdPlayType _AdPlayType,string _failed = "")
    {
        string adPlayMsg = "";
        string key = "";

        if (AdDic == null || AdDic.Count <= 0)
            return;

        foreach (string _key in AdDic.Keys)
        {
            key = _key;
        }
        adPlayMsg = AdDic[key];
        if (_failed == "no")
        {
            adPlayMsg =  "设备ID:" + deveceId + " | 播放结果:失败 | 失败原因:没有用的商家";
        }
        else
        {
            adPlayMsg += " | 播放结果:失败 | 失败原因:" + _failed;


        }
        AllSDKManager.SDKDebug(Name + adPlayMsg);
        AdDic[key] = adPlayMsg;
        //记录完成发送到友盟后台
        if (AdDic != null && AdDic.Count > 0)
            SetEvent(Ad, AdDic);

    }



    /// <summary>
    /// 服务器接口失败埋点
    /// </summary>
    /// <param name="_api">方法名</param>
    /// <param name="_failed">失败原因</param>
    public void Event_ServerAPI(string _api, string _failed = "未知原因")
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic["HttpServerHelper"] = "设备ID：" + deveceId + " | 时间：" + System.DateTime.Now + " | 方法:" + _api + " | 原因:" + _failed;
        AllSDKManager.SDKDebug(Name +"设备ID：" + deveceId + " | 时间：" + System.DateTime.Now + " | 方法:" + _api + " | 原因:" + _failed);
        SetEvent(serverIPA, dic);
    }


    /// <summary>
    /// 埋点测试
    /// </summary>
    /// <param name="_level"></param>
    /// <param name="str"></param>
    /// <param name="str2"></param>
    public void Event_Test(string _level, string str, string str2)
    {
        GA.Event("测试", "测试数据:" + " 第一个参数：" + str + " 第二个参数：" + str2);
        Dictionary<string, string> levelAdDic = new Dictionary<string, string>();
        levelAdDic["Level_Test_Id"] = _level;
        levelAdDic["Level_Test_Str1"] = str;
        levelAdDic["Level_Test_Str2"] = str2;

        SetEvent(test,levelAdDic);
        AllSDKManager.SDKDebug(Name + "测试场景:" + _level + " 测试参数1：" + str + " 测试参数2:" + str2);
    }
	public void Event_NativeAd(string _adTeam, string _behavior, string _bannerName, string _jumpUrl)
	{

	}
}
