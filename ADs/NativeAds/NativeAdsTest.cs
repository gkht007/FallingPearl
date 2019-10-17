using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeAdsTest : MonoBehaviour
{
	public void Init()
	{
		AllSDKManager.Init();
	}

	public void Banner()
	{ NativeAds.Instance.ShowBanner(); }
	public void Interstitial()
	{ NativeAds.Instance.ShowInterstitial(); }
	public void RewardedVideo()
	{ NativeAds.Instance.ShowRewardedVideo(); }

	public void CheckAdTypeTest()
	{
		//AllSDKManager.GetSDK<AdSdk>().GetAdSdk<NativeAds>().Test();
	}
}
