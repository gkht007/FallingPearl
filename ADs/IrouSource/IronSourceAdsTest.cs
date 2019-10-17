using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSourceAdsTest : MonoBehaviour
{

    public void Init()
    {
		AllSDKManager.Init();
       
    }
    public void Banner()
    { AllSDKManager.GetSDK<AdSdk>().ShowBanner(); }
    public void Interstitial()
    { AllSDKManager.GetSDK<AdSdk>().ShowInterstitial(); }
    public void RewardedVideo()
    { AllSDKManager.GetSDK<AdSdk>().ShowRewardedVideo(); }

}
