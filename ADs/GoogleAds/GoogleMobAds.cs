using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleMobAds : IAD
{
    //  —————————广告测试专用adUnitId—————————————      
    //  |                                                              |      
    //  |               ====Android====                                |      
    //  |                                                              |      
    //  |                                                              ——————————————
    //  |   const string adUnitId_banner = "ca-app-pub-3940256099942544/6300978111";              |
    //  |   const string adUnitId_interstitial = "ca-app-pub-3940256099942544/1033173712";        |
    //  |   const string adUnitId_rewardedVideo = "ca-app-pub-3940256099942544/5224354917";       |
    //  |                                                              ——————————————
    //  |                                                              |
    //  |   Native Advanced ：ca-app-pub-3940256099942544/2247696110   |
    //  |                                                              |
    //  |               ====iOS====                                    |
    //  |                                                              |
    //  |                                                              ——————————————
    //  |   const string adUnitId_banner = "ca-app-pub-3940256099942544/2934735716";              |
    //  |   const string adUnitId_interstitial = "ca-app-pub-3940256099942544/4411468910";        |
    //  |   const string adUnitId_rewardedVideo = "ca-app-pub-3940256099942544/3986624511";       |
    //  |                                                              ——————————————
    //  |   Native Advanced : ca-app-pub-3940256099942544/3986624511   |
    //  |                                                              |
    //  ————————————————————————————————

    BannerView bannerView;
    InterstitialAd interstitial;
    RewardBasedVideoAd rewardBasedVideo;
   // bool isBannerLoaded = false;//banner是否加载完成
   // bool isOnceLoaded = true;//是否为第一次加载
#if UNITY_EDITOR
    const string testDeviceId = "AA1EAD36614E2FC4CD11B7DF0E2E20DA";//测试设备ID
    const string appId = "ca-app-pub-9836495462170700~2646426322";
    const string adUnitId_banner = "ca-app-pub-9836495462170700/9029847757";
    const string adUnitId_interstitial = "ca-app-pub-9836495462170700/4154949119";
    const string adUnitId_rewardedVideo = "ca-app-pub-9836495462170700/4047137905";


#elif UNITY_ANDROID
    const string testDeviceId = "AA1EAD36614E2FC4CD11B7DF0E2E20DA";//测试设备ID
    const string appId="ca-app-pub-9836495462170700~2646426322";
    const string adUnitId_banner = "ca-app-pub-3940256099942544/6300978111";           
    const string adUnitId_interstitial = "ca-app-pub-3940256099942544/1033173712";     
    const string adUnitId_rewardedVideo = "ca-app-pub-3940256099942544/5224354917";

#elif UNITY_IOS
    const string testDeviceId = "27084ba8015c161776507f70cb5fa3ee65eef467";//测试设备ID
    const string appId="ca-app-pub-9836495462170700~6949661086";
    const string adUnitId_banner = "ca-app-pub-3940256099942544/2934735716";              
    const string adUnitId_interstitial = "ca-app-pub-3940256099942544/4411468910";       
    const string adUnitId_rewardedVideo = "ca-app-pub-3940256099942544/3986624511";
   
#endif

	public string Name
    {
        get { return "GoogleMobAds"; }
    }
    
    public void Init()
    {
        MobileAds.Initialize(appId);

        LoadInterstitial();
        LoadRewardedVideo();

    }

    public void Disable()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
        }
    }
    /// <summary>
    /// 展示横幅广告
    /// 说明：先执行加载，加载完后才会展示
    /// 注：加载时长会受网络环境影响
    /// </summary>
    public void ShowBanner()
    {
        LoadBanner();    
    }
	/// <summary>
	/// 停止横幅广告
	/// 说明：先执行加载，加载完后才会展示
	/// 注：加载时长会受网络环境影响
	/// </summary>
	public void HideBanner()
	{
		if (bannerView != null)
		{
			bannerView.Hide();
		}
		if (bannerView != null)
			bannerView.Destroy();
	}

	/// <summary>
	/// 展示插页广告
	/// 说明：先执行加载，加载完后才会展示
	/// 注：加载时长会受网络环境影响
	/// </summary>
	public void ShowInterstitial()
    {
        if (interstitial.IsLoaded())
        {
            Debug.Log(Name + " Interstitial加载成功 开始展示");
            interstitial.Show();
        }
        else
        {            
            AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,false, AdPlayType.Interstitial, "加载失败");
        }
    }

    /// <summary>
    /// 展示视频奖励广告
    /// 说明：先执行加载，加载完后才会展示
    /// 注：加载时长会受网络环境影响
    /// </summary>
    public void ShowRewardedVideo()
    {
        if (rewardBasedVideo.IsLoaded())
        {
			Debug.Log(Name + " RewardedVideo加载成功 开始展示");
            rewardBasedVideo.Show();
        }
        else
        {
            AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,false, AdPlayType.RewardedVideo, "加载失败");
        }
    }


    #region Load 广告加载
    void LoadBanner()
    {
        if (bannerView != null)
            bannerView.Destroy();

        bannerView = new BannerView(adUnitId_banner, AdSize.Banner, AdPosition.Bottom);

        RegistBanner();

        AdRequest request = new AdRequest.Builder().AddTestDevice(testDeviceId).Build();

        bannerView.LoadAd(request);
    }
    void LoadInterstitial()
    {
        interstitial = new InterstitialAd(adUnitId_interstitial);
        RegistInterstitial();
        AdRequest request = new AdRequest.Builder().AddTestDevice(testDeviceId).Build();
        interstitial.LoadAd(request);
    }
    void LoadRewardedVideo()
    {

        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        RegistRewardedVideo();

        AdRequest request = new AdRequest.Builder().AddTestDevice(testDeviceId).Build();

        this.rewardBasedVideo.LoadAd(request, adUnitId_rewardedVideo);
    }

    #endregion

    #region Event 广告回调事件

    #region Regist 事件注册

    //banner
    void RegistBanner()
    {

        bannerView.OnAdLoaded +=LoadSuccessBanner;

        bannerView.OnAdFailedToLoad += LoadFailedBanner;

        bannerView.OnAdOpening += ClickOpenBanner;

        bannerView.OnAdClosed += ClickCloseBanner;

        bannerView.OnAdLeavingApplication += Banner_HandleOnAdLeavingApplication;
    }

    // interstitial
    void RegistInterstitial()
    {

        this.interstitial.OnAdLoaded += LoadSuccessInterstitial;

        this.interstitial.OnAdFailedToLoad += LoadFailedInterstitial;

        this.interstitial.OnAdOpening += ClickOpenInterstitial;

        this.interstitial.OnAdClosed += ClickCloseInterstitial;

        this.interstitial.OnAdLeavingApplication += Interstitial_HandleOnAdLeavingApplication;

    }

    //rewardedVideo
    void RegistRewardedVideo()
    {

        rewardBasedVideo.OnAdLoaded += LoadSuccessRewardedVideo;

        rewardBasedVideo.OnAdFailedToLoad += LoadFailedRewardedVideo;

        rewardBasedVideo.OnAdOpening += ClickOpenRewardedVideo;

        rewardBasedVideo.OnAdStarted += StartRewardedVideo;

        rewardBasedVideo.OnAdRewarded += ReceiveRewarded;

        rewardBasedVideo.OnAdClosed += ClickCloseRewardedVideo;

        rewardBasedVideo.OnAdLeavingApplication += RewardedVideo_HandleRewardBasedVideoLeftApplication;



    }
    #endregion

    #region Handle 回调处理

    #region banner
    /// <summary>
    /// 加载完成
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void LoadSuccessBanner(object sender, EventArgs args)
    {
		Debug.Log(Name + " Banner加载成功 开始显示");
        AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,true, AdPlayType.Banner);
        bannerView.Show();
      
      
    }
    /// <summary>
    /// 加载失败
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void LoadFailedBanner(object sender, AdFailedToLoadEventArgs args)
    {
		Debug.Log(Name + " Banner加载失败");
        AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,false, AdPlayType.Banner,"加载失败");
    }
    /// <summary>
    /// 玩家点击广告
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void ClickOpenBanner(object sender, EventArgs args)
    {
		Debug.Log("玩家点击了:"+Name + " Banner");
    
    }
   /// <summary>
   /// 玩家关闭广告
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="args"></param>
    void ClickCloseBanner(object sender, EventArgs args)
    {
		Debug.Log("玩家关闭了:" + Name + " Banner");
        LoadBanner();
      
    }
  
    void Banner_HandleOnAdLeavingApplication(object sender, EventArgs args)
    {

    }
    #endregion

    #region interstitial
    /// <summary>
    /// 加载成功
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void LoadSuccessInterstitial(object sender, EventArgs args)
    {
		Debug.Log(Name + " Interstitial加载成功 等待显示");
     
    }
    /// <summary>
    /// 加载失败
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void LoadFailedInterstitial(object sender, AdFailedToLoadEventArgs args)
    {
		Debug.Log(Name + " Interstitial加载失败");     
    }
    /// <summary>
    /// 展示广告
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void ClickOpenInterstitial(object sender, EventArgs args)
    {
		Debug.Log(Name + " Interstitial展示成功");
        AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,true, AdPlayType.Interstitial);
    }
    /// <summary>
    /// 玩家关闭了点开的广告
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void ClickCloseInterstitial(object sender, EventArgs args)
    {
		Debug.Log("玩家关闭了点开的:"+Name + " Interstitial");
    }

    void Interstitial_HandleOnAdLeavingApplication(object sender, EventArgs args)
    {

    }
    #endregion

    #region rewardedVideo
    /// <summary>
    /// 加载成功
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void  LoadSuccessRewardedVideo(object sender, EventArgs args)
    {
		Debug.Log(Name+" RewardedVideo加载完成 等待显示");
    }
    /// <summary>
    /// 加载失败
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void LoadFailedRewardedVideo(object sender, AdFailedToLoadEventArgs args)
    {
		Debug.Log(Name + " RewardedVideo加载失败 失败原因："+args.Message);       
    }
    /// <summary>
    /// 广告铺满屏幕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void ClickOpenRewardedVideo(object sender, EventArgs args)
    {
		Debug.Log(Name + " RewardedVideo铺满屏幕");
      
    }
    /// <summary>
    /// 广告开始播放
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void StartRewardedVideo(object sender, EventArgs args)
    {
		Debug.Log(Name + " RewardedVideo真·开始播放");
    }
    /// <summary>
    /// 玩家关闭点开的广告
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void ClickCloseRewardedVideo(object sender, EventArgs args)
    {
		Debug.Log("玩家关闭了点开的:"+Name + " RewardedVideo");
     
    }
    /// <summary>
    /// 观看完视频接收奖励
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void ReceiveRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
		Debug.Log("观看完广告:"+Name + " RewardedVideo得到奖励:type:" + type + " amount:" + amount);
        AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,true, AdPlayType.RewardedVideo);
    }

    void RewardedVideo_HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {

    }


    #endregion

    #endregion

    #endregion


}
