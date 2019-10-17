using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using MyUtili;
public class CountryData//服务器给的国家定位
{
    public string country;//国家
    public string province;//省份
    public string city;//城市
}
public class Config
{
    private static string UUID = "";
    //每次发布版本请检查该版本和config中的版本号一致
    public static int defConifgVersion = 118;
    public static string AdServer//广告后台服务器接口
    {
        get
        {
            //return "http://192.168.1.111:8080/guanggaoTools/tupian";

            return "https://api.magicalarea.com/mu_ad/guanggaoTools/tupian";
        }
    }
    //上报埋点数据需要
    public static string UpLoadBuryUrl
    {
        get
        {
            return "https://api.magicalarea.com/datareport";
        }
    }
    public static string CountryServer//获取国家和地区的接口    国家缩写  省   城市
    {
        get
        {
            return "https://api.magicalarea.com/ipchange";
        }
    }
    static CountryData countryData = new CountryData();
    public static string Country
    {
        set
        {
            countryData = JsonUtility.FromJson<CountryData>(value);
        }
        get
        {
            if (string.IsNullOrEmpty(countryData.country))
                return "none";
            return countryData.country;
        }

    }
    public static string Topic
    {
        get
        {
            return "FallingPearl";
        }
    }
    public static string GameId
    {
        get
        {
            return "0";
        }
    }
    public static string Version
    {
        get
        {
            return Application.version;
        }
    }
    public static string getVersion()
    {
        string ver = PlayerPrefs.GetString("config_version", Config.defConifgVersion.ToString());
        Debug.Log("### getVersion : ###" + ver);
        return ver;
    }
    public static void setVersion(string ver)
    {
        Debug.Log("### setVersion : ###" + ver);
        PlayerPrefs.SetString("config_version", ver);
    }

    public static string UserId
    {
        get
        {
            if (getUserID() == string.Empty || getUserID() == "")
            {
                PlayerPrefs.SetString("UserID", DeviceId + UnityEngine.Random.Range(1, 999).ToString());
                return PlayerPrefs.GetString("UserID");
                // return "-1";
            }
            return getUserID();
        }
    }
    public static string getUserID()
    {
        string userid = PlayerPrefs.GetString("UserID", "");
        Debug.Log("### getUserID  ###  " + userid);
        return userid;
    }
    public static void setUserID(string userid)
    {
        Debug.Log("### setUserID userid ### " + userid);

        PlayerPrefs.SetString("UserID", userid);
    }
    public static string DeviceId
    {
        get
        {
            if (MyUtilities.getUUID() == null || MyUtilities.getUUID() == string.Empty)
            {
                return "-1";
            }
            return MyUtilities.getUUID();
        }
    }

    public static string Platform
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_IPHONE
            return "iOS";
#elif UNITY_IOS
            return "IOS";
#endif
        }
    }
    public static bool isRemovedAD()
    {
        string count = PlayerPrefs.GetString("removead", "");
        if(count=="")
        {
            return false;
        }
        return true;
    }
    public static void RemovedAD()
    {
        PlayerPrefs.SetString("removead", "remove");
    }
}
