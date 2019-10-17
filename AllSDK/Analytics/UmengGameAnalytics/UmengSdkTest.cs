using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmengSdkTest : MonoBehaviour
{
    public void Init()
    {
        AllSDKManager.GetSDK<AllAnalyticsSdk>().GetAnalythicsSdk<UmengSdk>().Init();
    }
    public void Event_Level_Enter()
    {
        AllSDKManager.GetSDK<AllAnalyticsSdk>().GetAnalythicsSdk<UmengSdk>().Event_Button("测试页面","进入关卡");
        //AllSDKManager.GetSDK<UmengSdk>().Event_Level_Enter("-1");
    }
    public void Event_Level_Leave()
    {
		AllSDKManager.GetSDK<AllAnalyticsSdk>().GetAnalythicsSdk<UmengSdk>().Event_Button("测试页面", "退出关卡");
		//AllSDKManager.GetSDK<UmengSdk>().Event_Button("测试页面", "离开关卡");
		//AllSDKManager.GetSDK<UmengSdk>().Event_Level_Leave("-1",1, "1000", 3);

	}
    //public void Event_Level_Source()
    //{
    //   AllSDKManager.GetSDK<UmengSdk>().Event_Button("关卡得分");

    //    AllSDKManager.GetSDK<UmengSdk>().Event_Level_Source(1.ToString(),100.ToString());
    //}
    //public void Event_Level_Pass()
    //{
    //    AllSDKManager.GetSDK<UmengSdk>().Event_Button("通关");

    //    AllSDKManager.GetSDK<UmengSdk>().Event_Level_Pass(1.ToString(),"通过");

    //}
    //public void Event_Level_Star()
    //{
    //   AllSDKManager.GetSDK<UmengSdk>().Event_Button("关卡得星");

    //    AllSDKManager.GetSDK<UmengSdk>().Event_Level_Star(1.ToString(), 3.ToString());
    //}

    //public void Event_Level_Hint()
    //{
    //    AllSDKManager.GetSDK<UmengSdk>().Event_Button("提示");
    //    AllSDKManager.GetSDK<UmengSdk>().Event_Level_Hint("-1");
    //}
    //public void Event_Level_Ad()
    //{
    //    AllSDKManager.GetSDK<UmengSdk>().Event_Level_Ad(1.ToString(),"没看广告");
    //    AllSDKManager.GetSDK<UmengSdk>().Event_Button("广告");


    //}
    public void Event_Test_Level()
    {
        AllSDKManager.GetSDK<AllAnalyticsSdk>().GetAnalythicsSdk<UmengSdk>().Event_Test("-1", "我是第一句话", "我是第二句话");
    }
    public void Event_Button()
    {
        AllSDKManager.GetSDK<AllAnalyticsSdk>().GetAnalythicsSdk<UmengSdk>().Event_Button("测试页面","测试按钮");
    }
}
