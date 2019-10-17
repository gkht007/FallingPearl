using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSourceAds : IAD
{
    //通过回调通知激励是否成功
    public delegate void RewardedVideoDone(bool yes);
    public static event RewardedVideoDone rewardedVideoDone;
    int userTotalCredits = 0;
    int userCredits = 0;
#if UNITY_EDITOR
    public static string appKey = "9c6507dd";
#elif UNITY_ANDROID
    public static string appKey = "9c6507dd";
#elif UNITY_IOS
    public static string appKey = "9c64ce55";
#endif

    public static string REWARDED_INSTANCE_ID = "0";

    public string Name
    {
        get { return "IronSourceAds"; }
    }

    public void Init()
    {
        IronSource.Agent.setUserId(SystemInfo.deviceUniqueIdentifier);
        IronSource.Agent.init(appKey);
        IronSource.Agent.init(appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.OFFERWALL, IronSourceAdUnits.BANNER);
        IronSource.Agent.initISDemandOnly(appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);

        RegistInterstitial();
        RegistRewardedVideo();
        RegistOfferWall();
        RegistBanner();

		Debug.Log(Name + " Interstitial开始加载");
        IronSource.Agent.loadInterstitial();
       
    }
    public void Disable()
    {
        //IronSource.Agent.hideBanner();
        IronSource.Agent.destroyBanner();
    }

	/// <summary>
	/// 停止横幅广告
	/// 说明：先执行加载，加载完后才会展示
	/// 注：加载时长会受网络环境影响
	/// </summary>
	public void HideBanner()
	{
		AllSDKManager.SDKDebug("删除横幅");
		IronSource.Agent.hideBanner();
		IronSource.Agent.destroyBanner();
	}
	/// <summary>
	/// 展示横幅广告
	/// 说明：先执行加载，加载完后才会展示
	/// 注：加载时长会受网络环境影响
	/// </summary>
	public void ShowBanner()
    {
		Debug.Log(Name + " Banner开始加载");
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }
    /// <summary>
    /// 展示插页广告
    /// 说明：先执行加载，加载完后才会展示
    /// 注：加载时长会受网络环境影响
    /// </summary>
    public void ShowInterstitial()
    {
        if (IronSource.Agent.isInterstitialReady())
        {
			Debug.Log(Name + " Interstitial加载成功 开始显示");
            IronSource.Agent.showInterstitial();
        }
        else
        {
            AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,false, AdPlayType.Interstitial, "加载失败");
        }
    }
    /// <summary>
    /// 展示奖励广告
    /// 说明：通过isRewardedVideoAvailable()发送请求，广告商反馈true后播放
    /// 注：加载时长会受网络环境影响
    /// </summary>
    public void ShowRewardedVideo()
    {

        if (IronSource.Agent.isRewardedVideoAvailable())
        {
			Debug.Log(Name + " RewardedVideo加载成功 开始显示");
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,false, AdPlayType.RewardedVideo, "播放失败");
        }
    }
    /// <summary>
    /// 展示OfferWall广告
    /// 说明：通过isOfferwallAvailable()发送请求，广告商反馈true后播放
    /// 注：加载时长会受网络环境影响
    /// </summary>
    public void ShowOfferWall()
    {

        if (IronSource.Agent.isOfferwallAvailable())
        {
            IronSource.Agent.showOfferwall();
        }
        else
        {
			Debug.Log("IronSource.Agent.isOfferwallAvailable - False");
        }

    }
    #region 广告事件注册
    void RegistBanner()
    {
        IronSourceEvents.onBannerAdLoadedEvent += LoadSuccessBanner;
        IronSourceEvents.onBannerAdLoadFailedEvent += LoadFailedBanner;
        IronSourceEvents.onBannerAdClickedEvent += ClickBanner;
        IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent += LeftAppBanner;
    }
    void RegistInterstitial()
    {
        IronSourceEvents.onInterstitialAdReadyEvent += LoadSuccessInterstitial;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += LoadFailedInterstitial;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += ShowSuccessInterstitial;
        IronSourceEvents.onInterstitialAdShowFailedEvent += ShowFailedInterstitial;
        IronSourceEvents.onInterstitialAdClickedEvent += ClickInterstitial;
        IronSourceEvents.onInterstitialAdOpenedEvent += StartInterstitial;
        IronSourceEvents.onInterstitialAdClosedEvent += CloseInterstitial;
    }  
    void RegistRewardedVideo()
    {
        IronSourceEvents.onRewardedVideoAdOpenedEvent += OpenRewardedVideo;
        IronSourceEvents.onRewardedVideoAdClosedEvent += CloseRewardedVideo;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += IsCanShowRewardedVideo;
        IronSourceEvents.onRewardedVideoAdStartedEvent += StartRewardedVideo;
        IronSourceEvents.onRewardedVideoAdEndedEvent += EndRewardedVideo;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += ReceiveRewardedVideo;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += ShowFailedRewardedVideo;
    }
    void RegistOfferWall()
    {
        IronSourceEvents.onOfferwallClosedEvent += OfferwallClosedEvent;
        IronSourceEvents.onOfferwallOpenedEvent += OfferwallOpenedEvent;
        IronSourceEvents.onOfferwallShowFailedEvent += OfferwallShowFailedEvent;
        IronSourceEvents.onOfferwallAdCreditedEvent += OfferwallAdCreditedEvent;
        IronSourceEvents.onGetOfferwallCreditsFailedEvent += GetOfferwallCreditsFailedEvent;
        IronSourceEvents.onOfferwallAvailableEvent += OfferwallAvailableEvent;
    }
    #endregion

    #region 广告事件接口实现
    void OnApplicationPause(bool isPaused)
    {
		Debug.Log(Name + "暂停：" + isPaused);
        IronSource.Agent.onApplicationPause(isPaused);
    }
    #region Banner
   /// <summary>
   /// 加载成功
   /// </summary>
    void LoadSuccessBanner()
    {
		Debug.Log(Name + " Banner加载成功");
       AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,true,AdPlayType.Banner);
    }
    /// <summary>
    /// 加载失败
    /// </summary>
    /// <param name="error"></param>
    void LoadFailedBanner(IronSourceError error)
    {
		Debug.Log(Name + " Banner加载失败 失败原因："+error.ToString());
        AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,false,AdPlayType.Banner,"加载失败");
    }
    /// <summary>
    /// 玩家点击广告
    /// </summary>
    void ClickBanner()
    {
		Debug.Log("玩家点击了:"+Name + " Banner");
    }
    /// <summary>
    /// 点开的广告开始展示
    /// </summary>
    void BannerAdScreenPresentedEvent()
    {
		Debug.Log(Name + " Banner点击后开始展示");
    }
    /// <summary>
    /// 点开的广告完成展示
    /// </summary>
    void BannerAdScreenDismissedEvent()
    {
		Debug.Log(Name + " Banner点击后完成展示");
    }
    /// <summary>
    /// 玩家离开应用
    /// </summary>
    void LeftAppBanner()
    {
		Debug.Log(Name+" Banner玩家离开应用");
    }
   
    #endregion
    #region interstitial
    /// <summary>
    /// 加载成功
    /// </summary>
    void LoadSuccessInterstitial()
    {
		Debug.Log(Name + " Interstitial加载成功 等待显示");
    }
    /// <summary>
    /// 加载失败
    /// </summary>
    /// <param name="error"></param>
    void LoadFailedInterstitial(IronSourceError error)
    {
		Debug.Log(Name + " Interstitial加载失败 失败原因:" + error.ToString());
       
    }
    /// <summary>
    /// 显示成功
    /// </summary>
    void ShowSuccessInterstitial()
    {
		Debug.Log(Name + " Interstitial显示成功 继续开始新的加载");
        AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,true, AdPlayType.Interstitial);
        IronSource.Agent.loadInterstitial();
    }
    /// <summary>
    /// 显示失败
    /// </summary>
    /// <param name="error"></param>
    void ShowFailedInterstitial(IronSourceError error)
    {
		Debug.Log(Name + " Interstitial显示失败 失败原因:" + error.ToString());
       
    }
    /// <summary>
    /// 玩家点击广告
    /// </summary>
    void ClickInterstitial()
    {
		Debug.Log("玩家点击了:" + Name + " Interstitial");
    }
    /// <summary>
    /// 播放插页
    /// </summary>
    void StartInterstitial()
    {
		Debug.Log(Name + " Interstitial真·播放");
    }
    /// <summary>
    /// 玩家关闭广告
    /// </summary>
    void CloseInterstitial()
    {
		Debug.Log("玩家关闭了:" + Name + " Interstitial");
    }
    #endregion
    #region rewardedVideo
    /// <summary>
    /// 加载完成与未完成
    /// </summary>
    /// <param name="canShowAd"></param>
    void IsCanShowRewardedVideo(bool canShowAd)
    {
        if (canShowAd)
        {
			Debug.Log(Name + " RewardedVideo加载成功可以显示");
        }
        else
        {
			Debug.Log(Name + " RewardedVideo加载未完成不可以显示");
        }

    }
    /// <summary>
    /// 广告铺满屏幕
    /// </summary>
    void OpenRewardedVideo()
    {
		Debug.Log(Name + " RewardedVideo铺满屏幕");
      
    }
    /// <summary>
    /// 广告开始播放
    /// </summary>
    void StartRewardedVideo()
    {
		Debug.Log(Name + " RewardedVideo真·播放");
       
    }
    /// <summary>
    /// 播放失败
    /// </summary>
    /// <param name="error"></param>
    void ShowFailedRewardedVideo(IronSourceError error)
    {

		Debug.Log(Name + " RewardedVideo播放失败 失败原因:  " + error.ToString());
      
    }
    /// <summary>
    /// 播放完成
    /// </summary>
    void EndRewardedVideo()
    {
		Debug.Log(Name + " RewardedVideo播放完成");
    }
    /// <summary>
    /// 玩家关闭广告
    /// </summary>
    void CloseRewardedVideo()
    {
		Debug.Log("玩家关闭了:" + Name + " RewardedVideo");

    }
    /// <summary>
    /// 接收奖励
    /// </summary>
    /// <param name="ssp"></param>
    void ReceiveRewardedVideo(IronSourcePlacement ssp)
    {

		Debug.Log(Name + " RewardedVideo播放完成 玩家接收奖励, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
        userTotalCredits = userTotalCredits + ssp.getRewardAmount();
        AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,true, AdPlayType.RewardedVideo);

    
    }



    #endregion
    #region Offerwall
    //当Offerwall成功加载用户时调用
    void OfferwallOpenedEvent()
    {
		Debug.Log("I got OfferwallOpenedEvent");
    }
    //在用户即将关闭后返回应用程序时调用
    void OfferwallClosedEvent()
    {
		Debug.Log("I got OfferwallClosedEvent");
    }
    //调用方法'showOfferWall'并且OfferWall无法加载时调用
    void OfferwallShowFailedEvent(IronSourceError error)
    {
		Debug.Log("I got OfferwallShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }
    //每次用户完成报价时调用,使用与“积分”的价值相对应的分数金额奖励用户参数。
    //dict  - 一个包含学分和总学分的字典。
    void OfferwallAdCreditedEvent(Dictionary<string, object> dict)
    {
		Debug.Log("I got OfferwallAdCreditedEvent, current credits = " + dict["credits"] + " totalCredits = " + dict["totalCredits"]);
    }
    //当'getOfferWallCredits'方法无法检索时调用用户的贷方余额信息。
    //  error -string对象，表示失败的原因。
    void GetOfferwallCreditsFailedEvent(IronSourceError error)
    {
		Debug.Log("I got GetOfferwallCreditsFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }
    //当Offerwall可用性状态发生变化时调用
    //  canShowOfferwal - 当Offerwall可用时，值将更改为YES
    //然后您可以通过调用showOfferwall（）来显示视频
    //当Offerwall不可用时，值将变为NO
    void OfferwallAvailableEvent(bool canShowOfferwal)
    {
        Debug.Log("I got OfferwallAvailableEvent, value = " + canShowOfferwal);
    }
    #endregion
    #endregion
}
