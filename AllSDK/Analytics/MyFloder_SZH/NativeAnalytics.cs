
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpLoadBuryData
{
    public string topic = Config.Topic;
    public string version = Config.Version;

    public string platform = Config.Platform;
    public string userId = Config.UserId;
    public string gameId = Config.GameId;
    public string deviceId = Config.DeviceId;
    public string uploadTime = System.DateTime.Now.ToString();
    public string cdnConfigVersion = Config.getVersion();
    public string action = "test";

    public string getJson()
    {
        return JsonUtility.ToJson(this).ToString();
    }
}
public class UpLoadBuryStateData : UpLoadBuryData
{
    public string startTime = System.DateTime.Now + "";
}
public class LevelStartBuryData : UpLoadBuryData
{
    public string startLevel;
    public string startTime;
}
public class LevelEndBuryData : UpLoadBuryData
{
    public string levelLeave;
    public string leaveCause;
    public string leaveTime;
    public string brushLife;
    public string pass;
    public string useTime;
    public string source;
    public string star;
    public string useHint;
    public string lookAd;

}
public class ButtonBuryData : UpLoadBuryData
{
    public string page;
    public string buttonName;
    public string triggerTime;
    public string currentLevel;
}
public class IAPBuryData : UpLoadBuryData
{
    public string buyName;
    public string startTime;
    public string buyState;
    public string endTime;
    public string failed;
}
public class ADBuryData : UpLoadBuryData
{
    public List<string> adTeam = new List<string>();
    public List<string> adPlayType = new List<string>();
    public List<string> playTime = new List<string>();
    public List<string> playResult = new List<string>();
    public string closeTime;
    public List<string> failed = new List<string>();
}
public class ADBuryData_Banner : ADBuryData
{ }
public class ServerIPABuryData : UpLoadBuryData
{
    public string time;
    public string function;
    public string failed;
}
public class NativeAdBuryData : UpLoadBuryData
{
    public string adTeam;
    public string adPlayType;
    public string behaviour;
    public string bannerName;
    public string jumpUrl;
}
public class TestBuryData : UpLoadBuryData
{
    public string testStr = "test -_|,*+\"/ @";
}

public class NativeAnalytics :MonoBehaviour, IAnalythics,ISDK
{

    public string Name
    {
        get
        {
            return "NativeAnalytics";
        }
    }

    public void Disable()
    {

    }

    public void Init()
    {
        UpLoadBuryStateData upLoadBuryStateData = new UpLoadBuryStateData();
        upLoadBuryStateData.action = initAnalythics;
        UpLoad(upLoadBuryStateData);
    }
    public void UpLoad(UpLoadBuryData upLoadBuryData)
    {
        HttpServerHelper.UpLoadBury(upLoadBuryData);
    }

    static NativeAnalytics instance;
    public static IAnalythics Instance
    {
        get
        {
            return instance;

        }

    }
    private void Awake()
    {
        instance = this;
    }
    #region action 
    const string initAnalythics = "====START NativeAnalythics====";
    const string buttonClick = "clickButton";
    const string levelStart = "levelStart";
    const string levelEnd = "levelEnd";
    const string IAP = "IAP";
    const string AdPlay = "AdPlay";
    const string serverIPA = "serverIPA";
    const string nativeAd_banner = "nativeAd_banner";
    const string test = "test";



    #endregion

    #region Data
    // Dictionary<string, ADBuryData> AdDic = new Dictionary<string, ADBuryData>();
    // List<ADBuryData> ADList = new List<ADBuryData>();
    ADBuryData adBuryData;
    ADBuryData_Banner adBuryData_Banner;
    Dictionary<string, IAPBuryData> IAPDic = new Dictionary<string, IAPBuryData>();

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


    /// <summary>
    /// 进入关卡埋点
    /// </summary>
    /// <param name="_level">关卡编号</param>
    public void Event_Level_Enter(string _level,string a)
    {

        timeStart = new TimeSpan(DateTime.Now.Ticks);

        LevelStartBuryData levelStartBuryData = new LevelStartBuryData();
        levelStartBuryData.action = levelStart;
        levelStartBuryData.startLevel = _level;
        levelStartBuryData.startTime = System.DateTime.Now + "";
        UpLoad(levelStartBuryData);

    }
    /// <summary>
    /// 离开关卡埋点
    /// </summary>
    /// <param name="_level">关卡编号</param>
    /// <param name="_brushLife">画笔剩余百分比</param>
    /// <param name="_pass">是否通过(1/0)</param>
    /// <param name="_source">得分</param>
    /// <param name="_star">获得星级(0,1,2,3)</param>
    /// <param name="_leaveCause">离开原因(win,failed,backHome,reTry)</param>
    public void Event_Level_Leave(string _level, int _pass, string _leaveCause = "")
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

        //上报

        LevelEndBuryData levelBuryData = new LevelEndBuryData();
        levelBuryData.action = levelEnd;
        levelBuryData.levelLeave = _level;
        levelBuryData.leaveTime = DateTime.Now + "";
        levelBuryData.pass = pass;
        levelBuryData.useTime = useTime;
        levelBuryData.useHint = useHint;
        levelBuryData.lookAd = lookAd;
        levelBuryData.leaveCause = _leaveCause;
        UpLoad(levelBuryData);

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
        //todo

        ButtonBuryData buttonBuryData = new ButtonBuryData();
        buttonBuryData.action = buttonClick;
        buttonBuryData.page = _page;
        buttonBuryData.buttonName = _buttonName;
        buttonBuryData.triggerTime = System.DateTime.Now + "";
        //if (_page == "Quit")
            // buttonBuryData.currentLevel = GameManager.LevelLoaded + "";
            UpLoad(buttonBuryData);

    }


    #region
    /// <summary>
    /// 内购开始埋点(此方法必须与Event_IAP_End函数成对使用,否则不会发送到友盟)
    /// </summary>
    /// <param name="_name">内购项目名</param>
    public void Event_IAP_Start(string _name)
    {

        //记录前清空缓存
        IAPDic.Clear();

        //开始记录埋点

        IAPBuryData iAPBuryData = new IAPBuryData();
        iAPBuryData.action = IAP;
        iAPBuryData.buyName = _name;
        iAPBuryData.startTime = System.DateTime.Now + "";

        IAPDic.Add(iAPBuryData.buyName, iAPBuryData);

    }


    /// <summary>
    /// 内购结束埋点(此方法必须与Event_IAP_Start函数成对使用,否则不会发送到友盟)
    /// 注：成功使用此方法
    /// </summary> 
    public void Event_IAP_End()
    {

        string key = "";

        if (IAPDic == null || IAPDic.Count <= 0)
            return;

        foreach (string _key in IAPDic.Keys)
        {
            key = _key;
        }

        IAPBuryData iAPBuryData = IAPDic[key];
        iAPBuryData.buyState = "1";
        iAPBuryData.endTime = System.DateTime.Now + "";
        iAPBuryData.failed = "no";
        UpLoad(iAPBuryData);

    }
    /// <summary>
    /// 内购结束埋点(此方法必须与Event_IAP_Start函数成对使用,否则不会发送到友盟)
    ///  注：失败使用此方法
    /// </summary>
    /// <param name="_failed">失败原因</param>
    public void Event_IAP_End(string _failed = "")
    {

        string key = "";

        if (IAPDic == null || IAPDic.Count <= 0)
            return;

        foreach (string _key in IAPDic.Keys)
        {
            key = _key;
        }
        IAPBuryData iAPBuryData = IAPDic[key];
        iAPBuryData.buyState = "0";
        iAPBuryData.endTime = System.DateTime.Now + "";
        iAPBuryData.failed = _failed;

        UpLoad(iAPBuryData);

    }


    /// <summary>
    /// 广告播放开始埋点(此方法必须与Event_Ad_End函数成对使用,否则不会发送到友盟)
    /// </summary>
    /// <param name="_adTeam">广告商</param>
    /// <param name="_AdPlayType">广告类型</param>
    public void Event_Ad_Start(string _adTeam, AdPlayType _AdPlayType, bool _clear)
    {
        if (_clear)
        {
            if (_AdPlayType == AdPlayType.Banner)
            {
                if (adBuryData_Banner != null && adBuryData_Banner.adTeam.Count > 0)
                {
                    UpLoad(adBuryData_Banner);
                }
                adBuryData_Banner = new ADBuryData_Banner();
                adBuryData_Banner.action = AdPlay;
            }
            else
            {
                adBuryData = new ADBuryData();
                adBuryData.action = AdPlay;
            }
        }
        if (_AdPlayType == AdPlayType.Banner)
            adBuryData_Banner.playTime.Add(System.DateTime.Now + "");
        else
            adBuryData.playTime.Add(System.DateTime.Now + "");
        //是否清空缓存
        //if (_clear)
        //{
        //    if (_AdPlayType == AdPlayType.Banner)
        //        adBuryData_Banner = new ADBuryData_Banner();
        //    else
        //        adBuryData = new ADBuryData();

        //    adBuryData.action = AdPlay;

        //}

        //if (_AdPlayType == AdPlayType.Banner)
        //{
        //    adBuryData_Banner.playTime.Add(System.DateTime.Now + "");
        //}
        //else
        //{
        //    adBuryData.playTime.Add(System.DateTime.Now + "");
        //    adBuryData.adTeam.Add(_adTeam);
        //    adBuryData.adPlayType.Add(_AdPlayType.ToString());
        //}

    }


    /// <summary>
    /// 广告播放结束埋点(此方法必须与Event_Ad_Start函数成对使用,否则不会发送到友盟)
    /// 注：成功使用此方法
    /// </summary> 
    public void Event_Ad_End(string _adTeam, AdPlayType _AdPlayType)
    {

        if (_AdPlayType == AdPlayType.Banner)
        {
            if (adBuryData_Banner.adTeam.Count > 3)
                adBuryData_Banner.playTime.Add(System.DateTime.Now + "");
            adBuryData_Banner.adTeam.Add(_adTeam);
            adBuryData_Banner.adPlayType.Add(AdPlayType.Banner.ToString());
            adBuryData_Banner.playResult.Add("1");
            adBuryData_Banner.failed.Add("");
            adBuryData_Banner.closeTime = System.DateTime.Now + "";
        }
        else
        {
            if (adBuryData.adTeam.Count > 3)
                adBuryData.playTime.Add(System.DateTime.Now + "");
            adBuryData.adTeam.Add(_adTeam);
            adBuryData.adPlayType.Add(_AdPlayType.ToString());
            adBuryData.playResult.Add("1");
            adBuryData.failed.Add("");
            adBuryData.closeTime = System.DateTime.Now + "";
            UpLoad(adBuryData);
        }

        //if (_AdPlayType == AdPlayType.Banner)
        //{
        //    adBuryData_Banner.adTeam.Add(_adTeam);
        //    adBuryData_Banner.adPlayType.Add(AdPlayType.Banner.ToString());
        //    adBuryData_Banner.playResult.Add("1");
        //    adBuryData_Banner.failed.Add("");
        //    adBuryData_Banner.closeTime = System.DateTime.Now + "";
        //    UpLoad(adBuryData_Banner);

        //}
        //else
        //{
        //    adBuryData.playResult.Add("1");
        //    adBuryData.closeTime = System.DateTime.Now + "";
        //    adBuryData.failed.Add("");
        //    UpLoad(adBuryData);
        //}
    }
    /// <summary>
    /// 广告播放结束埋点(此方法必须与Event_IAP_Start函数成对使用,否则不会发送到友盟)
    ///  注：失败使用此方法
    /// </summary>
    /// <param name="_failed">失败原因</param>
    public void Event_Ad_End(string _adTeam, AdPlayType _AdPlayType, string _failed = "")
    {
        if (_AdPlayType == AdPlayType.Banner)
        {
            //if (_failed == "no")
            //{
            //    UpLoad(adBuryData_Banner);
            //}
            //else
            //{
            if (adBuryData_Banner.adTeam.Count > 3)
                adBuryData_Banner.playTime.Add(System.DateTime.Now + "");
            adBuryData_Banner.adTeam.Add(_adTeam);
            adBuryData_Banner.adPlayType.Add(AdPlayType.Banner.ToString());
            adBuryData_Banner.playResult.Add("0");
            adBuryData_Banner.closeTime = "";
            adBuryData_Banner.failed.Add(_failed);
            //}
        }
        else
        {
            if (_failed == "no")
            {

                UpLoad(adBuryData);

            }
            else
            {
                if (adBuryData.adTeam.Count > 3)
                    adBuryData.playTime.Add(System.DateTime.Now + "");
                adBuryData.adTeam.Add(_adTeam);
                adBuryData.adPlayType.Add(_AdPlayType.ToString());
                adBuryData.playResult.Add("0");
                adBuryData.closeTime = "";
                adBuryData.failed.Add(_failed);
            }
        }
        //if (_AdPlayType == AdPlayType.Banner)
        //{
        //    adBuryData_Banner.adTeam.Add(_adTeam);
        //    adBuryData_Banner.adPlayType.Add(AdPlayType.Banner.ToString());
        //    adBuryData_Banner.playResult.Add("0");
        //    adBuryData_Banner.closeTime = "";
        //}
        //else
        //{
        //    adBuryData.playResult.Add("0");
        //    adBuryData.closeTime = "";
        //}
        //if (_failed == "no")
        //{
        //    if (_AdPlayType == AdPlayType.Banner)
        //    {
        //        adBuryData_Banner.failed.Add("no can used team");
        //        UpLoad(adBuryData_Banner);
        //    }
        //    else
        //    {
        //        adBuryData.failed.Add("no can used team");
        //        UpLoad(adBuryData);
        //    }
        //}
        //else
        //{
        //    if (_AdPlayType == AdPlayType.Banner)
        //        adBuryData_Banner.failed.Add(_failed);
        //    else
        //        adBuryData.failed.Add(_failed);
        //}



    }



    /// <summary>
    /// 服务器接口失败埋点
    /// </summary>
    /// <param name="_api">方法名</param>
    /// <param name="_failed">失败原因</param>
    public void Event_ServerAPI(string _api, string _failed = "未知原因")
    {
        ServerIPABuryData serverIPABuryData = new ServerIPABuryData();
        serverIPABuryData.action = serverIPA;
        serverIPABuryData.time = System.DateTime.Now + "";
        serverIPABuryData.function = _api;
        serverIPABuryData.failed = _failed;

        UpLoad(serverIPABuryData);
    }


    #region 携程的调用方式
    //IEnumerator PostRequest(string eventId, UpLoadBuryData upLoadBuryData)
    //{
    //    var uwr = new UnityWebRequest(Config.UpLoadBuryUrl, "POST");

    //    string data = upLoadBuryData.getJson();

    //    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
    //    uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
    //    uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //    uwr.SetRequestHeader("Content-Type", "application/json");

    //    //Send the request then wait here until it returns
    //    yield return uwr.SendWebRequest();

    //    if (uwr.isNetworkError)
    //    {
    //        Debug.Log("Error While Sending: " + uwr.error);
    //    }
    //    else
    //    {
    //        Debug.Log("Received: " + uwr.downloadHandler.text);
    //    }
    //}
    #endregion

    public void Test()
    {
        UpLoadBuryStateData upLoadBuryStateData = new UpLoadBuryStateData();
        upLoadBuryStateData.action = "testUploadStart";
        UpLoad(upLoadBuryStateData);

        TestBuryData testBuryData = new TestBuryData();
        testBuryData.action = "test";
        UpLoad(testBuryData);

        LevelStartBuryData levelBuryData = new LevelStartBuryData();
        levelBuryData.action = "testStartLevel";
        UpLoad(levelBuryData);

        ButtonBuryData buttonBuryData = new ButtonBuryData();
        buttonBuryData.action = "testButtonTrigger";
        UpLoad(buttonBuryData);

        ServerIPABuryData serverIPABuryData = new ServerIPABuryData();
        serverIPABuryData.action = "testServerIPA";
        UpLoad(serverIPABuryData);

        ADBuryData aDBuryData = new ADBuryData();
        aDBuryData.action = "testAdPlay";
        UpLoad(aDBuryData);
        // StartCoroutine(PostRequest(test,testBuryData));
    }

    public void Event_NativeAd(string _adTeam, string _behavior, string _bannerName, string _jumpUrl)
    {
        NativeAdBuryData nativeaAdBuryData = new NativeAdBuryData();
        nativeaAdBuryData.action = nativeAd_banner;
        nativeaAdBuryData.adTeam = _adTeam;
        nativeaAdBuryData.adPlayType = AdPlayType.Banner.ToString();
        nativeaAdBuryData.behaviour = _behavior;
        nativeaAdBuryData.bannerName = _bannerName;
        nativeaAdBuryData.jumpUrl = _jumpUrl;
        UpLoad(nativeaAdBuryData);
    }

    #endregion
}
