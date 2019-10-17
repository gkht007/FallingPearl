using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAD 
{
    string Name { get; }
    void Init();
    void ShowBanner();
    void ShowInterstitial();
    void ShowRewardedVideo();
    void Disable();
    void HideBanner();
}
