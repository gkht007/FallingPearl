using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
namespace MyUtili
{
    public class MyUtilities
    {
        private static string UUID="";
        public static void initUUID()
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            //MyUtili.MyUtilities._GetIDFA();
#else
            Debug.Log("###Utilities getUUID SystemInfo.deviceUniqueIdentifier ### " + SystemInfo.deviceUniqueIdentifier);
            UUID = SystemInfo.deviceUniqueIdentifier;
#endif

        }
        public static string getUUID()
        {
            //test test用户
            //return "12345678433333333333333334";
            Debug.Log("###Utilities getUUID UUID ### " + UUID);
            return UUID;
        }
        public static void setUUID(string uuid)
        {
            Debug.Log("###Utilities setUUID uuid ### " + uuid);
            UUID = uuid;
        }
#if UNITY_IOS
        //[DllImport("__Internal")]
        //private static extern void _GetIDFA();

#endif
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public static string GetRandomUUID()
        {
            return Guid.NewGuid().ToString("N");
        }

        //天朝各应用商店的PackageName
        //包名    商店
        //com.android.vending   Google Play
        //com.tencent.android.qqdownloader  应用宝
        //com.qihoo.appstore    360手机助手
        //com.baidu.appsearch   百度手机助
        //com.xiaomi.market 小米应用商店
        //com.wandoujia.phoenix2    豌豆荚
        //com.huawei.appmarket  华为应用市场
        //com.taobao.appcenter  淘宝手机助手
        //com.hiapk.marketpho   安卓市场
        //cn.goapk.market   安智市场
        public static void OnRateToGoogle()
        {
            RateToOther("com.wasay.watercup", "com.android.vending");
            //RateToOther("com.google.android.apps.maps", "com.android.vending");
        }
        public static void OnRateToHuawei()
        {
            RateToOther("com.google.android.apps.maps", "com.huawei.appmarket");
        }
        public static void RateToOther(string appPkg, string marketPkg)
        {
            if (!Application.isEditor)
            {
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "market://details?id=" + appPkg);
                intentObject.Call<AndroidJavaObject>("setData", uriObject);
                intentObject.Call<AndroidJavaObject>("setPackage", marketPkg);
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intentObject);
            }
        }

    }
   

}
