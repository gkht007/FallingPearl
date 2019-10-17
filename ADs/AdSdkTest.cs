using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdSdkTest : MonoBehaviour
{

    public void Init()
    {
        AllSDKManager.GetSDK<AdSdk>().Init();
    }


    public void ShowBanner()
    {
        AllSDKManager.GetSDK<AdSdk>().ShowBanner();
    }
    public void ShowInterstitial()
    {
        AllSDKManager.GetSDK<AdSdk>().ShowInterstitial();
    }
    public void ShowRewardedVideo()
    {
        AllSDKManager.GetSDK<AdSdk>().ShowRewardedVideo();
    }

    public void ThridAd()
    {
        //AllSDKManager.GetSDK<AdSdk>().AdType = AdType.Third;
    }
    public void NativeAd()
    {
       // AllSDKManager.GetSDK<AdSdk>().AdType = AdType.Native;
    }
}
