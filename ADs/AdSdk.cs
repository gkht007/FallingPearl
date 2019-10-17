using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum AdPlayType
{
    Banner,
    Interstitial,
    RewardedVideo
}
public enum AdType
{
    None,//无策略
    Native,//原生
    Third,//第三方
}
public class AdData
{
    public Type type;
    public IAD Ad;
    public AdType adType;
}

/// <summary>
/// 广告管理类
/// 1.新的广告接入需要创建对应的Class并且继承IAD接口
/// 2.所有新的广告接入需在本Class中的Init函数和RegistAllAdSdk函数里注册
/// </summary>
public class AdSdk : SingletionMono<AdSdk>, ISDK, IAD
{
    public Timer _gameTimer;
    public delegate void IsAdRewardedDone(bool _isDone);
    public delegate void IsAdIntersitialDone(bool _isDone);
    /// <summary>
    /// 视频奖励回调
    /// </summary>
    public event IsAdRewardedDone isAdRewardedDone;//奖励视频
    public event IsAdIntersitialDone isAdInterstitialDone;//插页视频

    public static int GAME_RETRY = 2;//游戏界面点击重玩次数记录
    public static int GAMEFAIL_RESTART = 3;//游戏失败重新开始次数记录
    public static int BILLING_RETRY = 2;//结算界面点击重玩按钮次数记录
    public static int BILLING_NEXTLEVEL = 3;//结算界面点击下一关按钮次数记录
    public static int BILLING_LEVELBUTTON = 3;//结算界面点击关卡按钮次数记录

    public string RequestAdTypeUrl
    {
        get
        {
            return Config.AdServer + "/dada1?";
        }
    }

    public Dictionary<Type, AdData> adSdkDic;//所有广告Sdk
    public List<AdData> adSdkList;//辅助列表，用来从字典取值用(主要用来循环第三方广告的获取)
    int nextAdSdk = 0;
    bool isEnable = true;
    AdType adType;

    /// <summary>
    /// 广告播放计时策略
    /// </summary>
    float time = 0;
    float timer = 30;
    //bool isAdTime = false;//是否为广告时间，30秒之后点击需求给定的按钮触发广告，然后重新计时
    public string Name
    {
        get
        {
            return "AdSdk";
        }
    }

    public bool IsEnable
    {
        get
        {
            AllSDKManager.SDKDebug(Name + "获取广告状态:" + isEnable);
            return isEnable;
        }
        set
        {
            isEnable = value;
            AllSDKManager.SDKDebug(Name + "设置广告状态:" + value);
        }

    }
    /// <summary>
    /// 广告策略切换
    /// </summary>
    /// <param name="_adType"></param>
    public void SetAdType(AdType _adType)
    {
        if (_adType == adType && adType != AdType.None)//如果不是第一次执行广告策略并且跟上次的广告策略相比没有变化则不用进行初始化切换
            return;
        adType = _adType;

        AllSDKManager.SDKDebug(Name + " 策略切换至原生广告");
        foreach (AdData item in adSdkDic.Values)
        {
            if (item.adType == adType)
            {
                item.Ad.Init();
                AllSDKManager.SDKDebug(Name + "/" + item.Ad.Name + "初始化");
            }
            else
            {
                item.Ad.Disable();
                AllSDKManager.SDKDebug(Name + "/" + item.Ad.Name + "关闭");
            }
        }
        if (SceneManager.GetActiveScene().name != "Game")
        {
            ShowBanner();
        }
    }

    public AdType GetAdType()
    {
        return adType;
    }
    /// <summary>
    /// 初始化部分
    /// </summary>
    #region Init
    public void Awake()
    {
        StartCoroutine(RequestCountry());
        adSdkDic = new Dictionary<Type, AdData>();
        adSdkList = new List<AdData>();
        adType = AdType.None;
        time = 0;
        timer = 30;
        // isAdTime = false;
        RegistAllAdSdk();
        // DontDestroyOnLoad(this);
    }
    private void Update()
    {

        //if (isAdTime == false)//如果不是广告时间 则开始计时
        //{
        //    time += Time.deltaTime;
        //    if (time >= timer)//到达广告时间则更改为广告播放时间，再次点击按钮会播放广告
        //    {
        //        time = 0;
        //        isAdTime = true;
        //    }
        //}
    }
    public void Disable()
    {
        IsEnable = false;

    }
    public void Init()
    {
        foreach (AdData item in adSdkDic.Values)
        {
            item.Ad.Init();
            AllSDKManager.SDKDebug(Name + "/" + item.Ad.Name + "初始化");
        }
        StartCoroutine(RequestAdType());//开启广告策略检查
                                        // ShowBanner();
    }
    void RegistAllAdSdk()
    {
        //这里注册新添加的

        // RegistAdSdk(typeof(GoogleMobAds), new GoogleMobAds());
        RegistAdSdk(typeof(IronSourceAds), new IronSourceAds());
        //RegistAdSdk(typeof(ApplovinAds), ApplovinAds.Instance);
        RegistAdSdk(typeof(UnityAds), UnityAds.Instance);
        // RegistAdSdk(typeof(FaceBookAds), FaceBookAds.Instance);
        
        RegistAdSdk(typeof(NativeAds), NativeAds.Instance, AdType.Native);
        //RegistAdSdk(typeof(NativeAdsDemo), NativeAdsDemo.Instance, AdType.Native);
    }

    void RegistAdSdk(Type _type, IAD _iAd, AdType _adType = AdType.Third)
    {
        if (!adSdkDic.ContainsKey(_type))
        {
            AdData adData = new AdData();
            adData.type = _type;
            adData.Ad = _iAd;
            adData.adType = _adType;
            adSdkDic.Add(_type, adData);
            if (_adType == AdType.Third)
                adSdkList.Add(adData);
            AllSDKManager.SDKDebug(Name + "/" + adSdkDic[_type].Ad.Name + "注册成功");
        }
        else
        {
            AllSDKManager.SDKDebug(Name + " 已经注册过这个平台" + adSdkDic[_type].Ad.Name);
        }
    }
    #endregion

    /// <summary>
    /// 轮寻检查广告策略
    /// </summary>
    /// <returns></returns>
    IEnumerator RequestAdType()
    {
        while (true)
        {
            AllSDKManager.SDKDebug("广告轮训检测", "cyan");
            string gameName = "FallingPearl";
            WWW www = new WWW(RequestAdTypeUrl + "topic=" + gameName);

            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                AllSDKManager.SDKDebug("数据测试：" + www.text, "cyan");
                int adType = int.Parse(www.text);
                SetAdType((AdType)adType);//1.播自己的2播第三方的
                                          // SetAdType(AdType.Native);
            }
            else
            {
                AllSDKManager.SDKDebug("数据测试：" + www.error, "cyan");
            }
            yield return new WaitForSeconds(30);
        }

    }
    /// <summary>
    /// 获取当前的国家  省份  城市
    /// </summary>
    /// <returns></returns>
    IEnumerator RequestCountry()
    {
        while (true)
        {
            WWW www = new WWW(Config.CountryServer);
            AllSDKManager.SDKDebug("=================", "red");
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Config.Country = www.text;
                AllSDKManager.SDKDebug(www.text, "red");
            }
            else
            {
                AllSDKManager.SDKDebug(www.error, "red");

            }
            yield return new WaitForSeconds(10);
        }
    }
    /// <summary>
    /// 隐藏横幅
    /// </summary>
    public void HideBanner()
    {
        foreach (AdData item in adSdkDic.Values)
        {
            item.Ad.HideBanner();
            AllSDKManager.SDKDebug(Name + "/" + item.Ad.Name + "隐藏Banner");
        }
    }



    /// <summary>
    /// 展示横幅
    /// </summary>
    public void ShowBanner()
    {
        AllSDKManager.SDKDebug(Name + " 展示横幅");
        switch (adType)
        {
            case AdType.Native:
                AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_Ad_Start(NativeAds.Instance.Name, AdPlayType.Banner);
                NativeAds.Instance.ShowBanner();
                break;
            case AdType.Third:
                AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_Ad_Start(adSdkList[0].Ad.Name, AdPlayType.Banner);
                adSdkList[0].Ad.ShowBanner();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 展示视屏奖励
    /// </summary>
    public void ShowRewardedVideo()
    {
        AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_Ad_Start(adSdkList[0].Ad.Name, AdPlayType.RewardedVideo);
        adSdkList[0].Ad.ShowRewardedVideo();
        //GameParse();
        Debug.Log("==test==视频奖励广告开始播放: timeScale=" + Time.timeScale);
    }

    /// <summary>
    /// 展示插页
    /// </summary>
    public void ShowInterstitial()
    {
        if (IsEnable)//如果广告是开启的并且是播放广告的时间 再播放广告  
        {
            //  if (IsEnable && (GameManager.levelLoaded > 10))
            // {
            AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_Ad_Start(adSdkList[0].Ad.Name, AdPlayType.Interstitial);
            adSdkDic[typeof(IronSourceAds)].Ad.ShowInterstitial();
            //adSdkDic[typeof(ApplovinAds)].Ad.ShowInterstitial();
            //GameParse();
            //Time.timeScale = 0;
            Debug.Log("==test==插页广告开始播放: timeScale=" + Time.timeScale);
            // isAdTime = false;
        }
    }
    /// <summary>
    /// 广告播放情况
    /// </summary>
    /// <param name="_isSuccess">是否播放成功</param>
    /// <param name="_AdPlayType">广告类型(banner/interstitial/rewardedVideo)</param>
    /// <param name="_failed">如果失败，失败原因</param>
    public void IsAdDone(string _adTeam, bool _isSuccess, AdPlayType _AdPlayType, string _failed = "")
    {
        AllSDKManager.SDKDebug(Name + "_isSuccess:" + _isSuccess + "  AdPlayType:" + _AdPlayType + " _failed:" + _failed);
        if (_isSuccess)//如果播放成功了
        {
            AllSDKManager.SDKDebug(Name + "播放成功");
            if (_AdPlayType != AdPlayType.Banner)//不是banner的情况下才可以记录广告观看状态
            {
                AllSDKManager.GetSDK<AllAnalyticsSdk>().IsLookAd = true;
            }
            AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_Ad_End(_adTeam, _AdPlayType);

            nextAdSdk = 0;

            //如果是视频奖励广告并且玩家没有跳过则去拿奖励,如果跳过则按失败处理
            if (_AdPlayType == AdPlayType.RewardedVideo)
            {

                if (_failed == "")
                    GetRewarded();
                else
                {
                    if (isAdRewardedDone != null)
                    {
                        isAdRewardedDone(false);
                    }
                }
                GameResume();
                Debug.Log("==test==视频奖励播放失败: timeScale=" + Time.timeScale);
            }
            else
                if (_AdPlayType == AdPlayType.Interstitial)
            {
                if (_failed == "")
                    InterstitialDone();
                else
                {
                    if (isAdInterstitialDone != null)
                    {

                        isAdInterstitialDone(false);
                    }
                }
                GameResume();
                Debug.Log("==test==插页播放失败: timeScale=" + Time.timeScale);
            }

            return;
        }
        else//当前的广告商的广告播放失败
        {
            AllSDKManager.SDKDebug(Name + " ==失败== " + "_isSuccess:" + _isSuccess + "  AdPlayType:" + _AdPlayType + " _failed:" + _failed);
            AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_Ad_End(_adTeam, _AdPlayType, _failed);
            GameResume();
            Debug.Log("==test==视频奖励播放失败222222: timeScale=" + Time.timeScale);
        }

        nextAdSdk++;//移位下一个广告商
        if (nextAdSdk >= adSdkList.Count)//没有可供播放的广告商的失败情况
        {

            AllSDKManager.SDKDebug(Name + "no can used team");
            AllSDKManager.GetSDK<AllAnalyticsSdk>().IsLookAd = false;
            AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_Ad_End(_adTeam, _AdPlayType, "no");
            GameResume();
            nextAdSdk = 0;
            if (_AdPlayType == AdPlayType.RewardedVideo)//奖励视频通知不能获得奖励
            {
                if (isAdRewardedDone != null)
                {
                    GameResume();
                    Debug.Log("==test==视频奖励播放失败: timeScale=" + Time.timeScale);
                    isAdRewardedDone(false);
                }
            }
            else if (_AdPlayType == AdPlayType.Interstitial)//插页视频 失败要恢复游戏时间
            {
                if (isAdInterstitialDone != null)
                {
                    GameResume();
                    Debug.Log("==test==插页播放失败: timeScale=" + Time.timeScale);
                    isAdInterstitialDone(false);
                }
            }
            return;
        }

        ShowAdHandler(_AdPlayType);//如果上面那家广告商的广告没有成功投放 则取下一家的继续尝试播放
    }
    /// <summary>
    /// 获取奖励
    /// </summary>
    public void GetRewarded()
    {
        AllSDKManager.SDKDebug("==========Start=====" + Name + "获得视频奖励");

        if (isAdRewardedDone != null)//视屏广告播放完成
        {
            //Time.timeScale = 1;
            //Debug.Log("==test==视频奖励播放成功: timeScale=" + Time.timeScale);
            isAdRewardedDone(true);
        }

        AllSDKManager.SDKDebug("==========End=====" + Name + "获得视频奖励");
    }
    /// <summary>
    /// 插页关闭
    /// </summary>
    public void InterstitialDone()
    {
        AllSDKManager.SDKDebug("==========Start=====" + Name + "玩家关闭了插页视频");

        if (isAdInterstitialDone != null)//视屏广告播放完成
        {

            isAdInterstitialDone(true);
        }
        GameResume();
        Debug.Log("==test==插页播放成功: timeScale=" + Time.timeScale);
        AllSDKManager.SDKDebug("==========End=====" + Name + "玩家关闭了插页视频");
    }
    /// <summary>
    /// 获取下一个广告并播放
    /// </summary>
    /// <param name="_AdPlayType"></param>
    void ShowAdHandler(AdPlayType _AdPlayType)
    {
        IAD nextAd = adSdkList[nextAdSdk].Ad;
        AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_Ad_Start(nextAd.Name, _AdPlayType, false);

        switch (_AdPlayType)
        {
            case AdPlayType.Banner:
                nextAd.ShowBanner();
                break;
            case AdPlayType.Interstitial:
                Debug.Log("ShowAdHandler Interstitial ==test==插页播放成功: timeScale=" + Time.timeScale);
                nextAd.ShowInterstitial();
                break;
            case AdPlayType.RewardedVideo:
                Debug.Log("ShowAdHandler ShowRewardedVideo ==test==插页播放成功: timeScale=" + Time.timeScale);
                nextAd.ShowRewardedVideo();
                break;
            default:
                break;
        }

    }
    public void GameParse()
    {
        // Time.timeScale = 0;
        //_gameTimer = FindObjectOfType<Timer>();
        //_gameTimer.time_pause();
        //  Debug.Log("GameParse ==test== Time.timeScale " + Time.timeScale);
    }

    public void GameResume()
    {
        //_gameTimer = FindObjectOfType<Timer>();
        //_gameTimer.time_resume();
        //   Time.timeScale = 1;

        // Debug.Log("GameResume ==test== Time.timeScale " + Time.timeScale);
    }



}
