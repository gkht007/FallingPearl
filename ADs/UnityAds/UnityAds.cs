using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;

public class UnityAds :MonoBehaviour, IAD
{
#if UNITY_EDITOR
    const string gameId = "3251270";
#elif UNITY_ANDROID
    const string gameId = "3251270";
#elif UNITY_IOS
    const string gameId = "3251271";
#endif
    bool testMode = true;
    string placementId_banner = "banner";
    string placementId_video = "video";
    string placementId_rewardedVideo = "rewardedVideo";
    static UnityAds instance;

	public static IAD Instance
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
    public string Name
    {
        get { return "UnityAds"; }
    }

    public void Init()
    {       
        Monetization.Initialize(gameId, testMode);
    }
    public void Disable()
    {
       
    }
	/// <summary>
	/// 隐藏横幅广告
	/// 注：要调用此方法请用调用协同的方法来调用(目前对中国区的支持不太友好).
	/// 例:StartCoroutine(ShowBanner())
	/// </summary>
	/// <returns></returns>
	IEnumerator HideUnityBanner()
	{
		UnityEngine.Advertisements.Advertisement.Banner.Hide(false);
		yield return new WaitForSeconds(0.5f);
	}
	/// <summary>
	/// 展示横幅广告
	/// 注：要调用此方法请用调用协同的方法来调用(目前对中国区的支持不太友好).
	/// 例:StartCoroutine(ShowBanner())
	/// </summary>
	/// <returns></returns>
	IEnumerator LoadBanner()
    {

        while (!UnityEngine.Advertisements.Advertisement.IsReady(placementId_banner))
        {
            yield return new WaitForSeconds(0.5f);
        }
        UnityEngine.Advertisements.Advertisement.Banner.Show(placementId_banner);

    }
    /// <summary>
    /// 展示视频广告
    /// 注：要调用此方法请用调用协同的方法来调用.
    /// 例:StartCoroutine(ShowVideo())
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadVideo()
    {

        while (!Monetization.IsReady(placementId_video))
        {
            yield return new WaitForSeconds(0.25f);
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(placementId_video) as ShowAdPlacementContent;

        if (ad != null)
        {
            ad.Show(VideoFinished);
        }
    }

    /// <summary>
    /// 展示视频奖励广告
    /// 注：要调用此方法请用调用协同的方法来调用.
    /// 例:StartCoroutine(ShowRewardedVideo())
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadRewardedVideo()
    {

        while (!Monetization.IsReady(placementId_rewardedVideo))
        {
            yield return null;
        }

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(placementId_rewardedVideo) as ShowAdPlacementContent;

        if (ad != null)
        {
            ad.Show(RewardedFinished);
        }
    }
    /// <summary>
    /// 插页广告回调
    /// </summary>
    /// <param name="result"></param>
    void VideoFinished(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Failed:
                Debug.Log(Name + " Interstitial播放失败");
                AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,false, AdPlayType.Interstitial,"播放失败");
                break;
            case ShowResult.Skipped:
				Debug.Log(Name + " Interstitial跳过播放");
                AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,true, AdPlayType.Interstitial,"跳过播放");
                break;
            case ShowResult.Finished:
				Debug.Log(Name + " Interstitial播放成功");
                AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,true, AdPlayType.Interstitial);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 视频奖励广告回调
    /// </summary>
    /// <param name="result"></param>
    void RewardedFinished(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
			Debug.Log(Name + " RewardedVideo播放成功");
            AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,true, AdPlayType.RewardedVideo);

        }
        else
            if (result == ShowResult.Failed)
        {
            Debug.Log(Name + " RewardedVideo播放失败");
           AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,false, AdPlayType.RewardedVideo,"播放失败");
        }
        else
            if (result == ShowResult.Skipped)
        {
			Debug.Log(Name + " RewardedVideo跳过播放");
            AllSDKManager.GetSDK<AdSdk>().IsAdDone(Name,true, AdPlayType.RewardedVideo,"跳过播放");
        }
    }

    public void ShowBanner()
    {
		//StartCoroutine(LoadBanner());
		//AllSDKManager.GetSDK<AdSdk>().GetAdSdk<NativeAds>().ShowBanner();
    }

    public void ShowInterstitial()
    {
        StartCoroutine(LoadVideo());
    }

    public void ShowRewardedVideo()
    {
        StartCoroutine(LoadRewardedVideo());
    }
	public void HideBanner()
	{
		StartCoroutine(HideUnityBanner());
	}
}
