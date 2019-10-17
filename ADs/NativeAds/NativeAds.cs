using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


#region Data
/// <summary>
/// Json数据映射
/// </summary>
public class NativeAdData
{

    public string next = "10";//下一张图片播放间隔
    public string check = "30";//json刷新间隔

    public List<NativeAdAssetsData> nativeAssetsDataList = new List<NativeAdAssetsData>();
}
[Serializable]
public class NativeAdAssetsData
{
    public string adPlayType;
    public string nickName;
    public string assetUrl;//图片地址
    public string textureName;//图片名
    public string jumpUrl;//跳转链接
}

/// <summary>
/// 加载到的数据存储类
/// </summary>
public class NativeAdAssetsDataWWW
{
    public string adPlayType;
    public string nickName;
    public Texture2D texture;
    public string jumpUrl;

    List<NativeAdAssetsDataWWW> nativeAdAssetsDataWWWList;

    public void AddAssets(NativeAdAssetsDataWWW _nativeAdAssetsDataWWW)
    {
        if (nativeAdAssetsDataWWWList == null)
            nativeAdAssetsDataWWWList = new List<NativeAdAssetsDataWWW>();

        nativeAdAssetsDataWWWList.Add(_nativeAdAssetsDataWWW);
    }

    public List<NativeAdAssetsDataWWW> GetAssets()
    {
        return nativeAdAssetsDataWWWList;
    }

    public NativeAdAssetsDataWWW GetAssets(int _index)
    {
        if (nativeAdAssetsDataWWWList == null || nativeAdAssetsDataWWWList.Count == 0)
            return null;
        else
        if (_index < nativeAdAssetsDataWWWList.Count)
            return nativeAdAssetsDataWWWList[_index];
        return null;
    }
}
#endregion


public class NativeAds : SingletionMono<NativeAds>, IAD
{

    string jsonUrl = Config.AdServer + "/a?topic=FallingPearl&";
    //string jsonUrl = Application.streamingAssetsPath + "/Resources/NativeAd/Json/NativeAd.json";
    string assetUrl = Application.streamingAssetsPath + "/Resources/Texture/";

    int version;//用途：校验策划是否在玩家游戏中变更了数据

    static bool isJsonReady = false;//json配置是否加载完成
    static bool isBannerReady = false;//banner资源是否加载完成
    int currentIndex = 0;

    bool isBannerOn = false; //是否正在展示banner
    bool isRequestBanner = false;//是否继续展示下一张banner
    bool isRequestJsonData = false;//是否检查json变更
    bool isClick = true;//是否为有效点击  ，展示一次点击一次为有效点击，没有新的展示之前 后续均为无效点击

    public Texture currentBanner;
    public string currentJumpUrl;


    public float timer = 0;


    public RawImage bannerImage;
    public string Name
    {
        get
        {
            return "NativeAds";
        }
    }

    #region Data
    NativeAdData nativeAdData;
    NativeAdData NativeAdData
    {
        get
        {
            if (nativeAdData == null)
                nativeAdData = new NativeAdData();
            return nativeAdData;
        }
        set
        {
            nativeAdData = value;
        }
    }
    NativeAdAssetsDataWWW nativeAdAssetsDataWWW;
    NativeAdAssetsDataWWW NativeAdAssetsDataWWW
    {
        get
        {
            if (nativeAdAssetsDataWWW == null)
                nativeAdAssetsDataWWW = new NativeAdAssetsDataWWW();
            return nativeAdAssetsDataWWW;
        }
        set
        {
            nativeAdAssetsDataWWW = value;
        }
    }


    CountryData CountryData = new CountryData();
    #endregion
    void OnGUI()
    {
        //GUIStyle s = new GUIStyle();
        //s.fontSize = 100;
        //GUI.Label(new Rect(700, 1500, 500, 500), timer+"\n"+currentBanner.name+"\n"+bannerImage.texture.name+"\n"+currentIndex+"\n"+"ShowBanner="+showbannercount+"\n"+"BannerPlay="+bannerplaycount+"\n"+"Awake="+awakeCount);
    }
    int awakeCount = 0;
    private void Awake()
    {
        awakeCount++;
        bannerImage.gameObject.SetActive(false);
        StartCoroutine(RequestJsonData());
    }
    public void Init()
    {
        timer = 0;
        isRequestBanner = false;
        isBannerOn = false;
    }
    public void Disable()
    {
        timer = 0;
        HideBanner();
        isRequestBanner = false;
    }
    void Update()
    {       
        if (isRequestBanner && isBannerOn)
        {
            timer += Time.deltaTime;
            if (timer >= float.Parse(NativeAdData.next))
            {
              
                ShowBanner();
            }
        }
    }
    void LoadBanner()
    {

        if (isJsonReady)
            StartCoroutine(RequestBannerData());
        else
        {
            Debug.Log("load banner false");
        }
    }
    int showbannercount = 0;
    int bannerplaycount = 0;
    public void ShowBanner()
    {
        showbannercount++;
        isRequestBanner = true;

        if (isBannerReady)
        {
            bannerplaycount++;
            SetBannerInfo();
        }
    }

    /// <summary>
    /// 向服务器请求json配置
    /// </summary>
    /// <returns></returns>
    IEnumerator RequestJsonData()
    {
        while (true)
        {
            WWW jsonWWW = new WWW(jsonUrl + "country=" + Config.Country + "&" + "platform=" + Config.Platform);
            AllSDKManager.SDKDebug(jsonUrl + "country=" + Config.Country + "&" + "platform=" + Config.Platform, "red");
            yield return jsonWWW;
            if (string.IsNullOrEmpty(jsonWWW.error))
            {
                AllSDKManager.SDKDebug("jsonWWW:" + jsonWWW.text, "white");
                try
                {
                    string[] dataArr = jsonWWW.text.Split('*');
                    int _version = int.Parse(dataArr[0].Split(':')[1]);//获取服务器最新版本号
                    AllSDKManager.SDKDebug("_version:" + _version, "white");
                    AllSDKManager.SDKDebug("version:" + version, "white");
                    if (version < _version)//如果策划在玩家游戏时变更数据，则刷新，否则继续使用上次读取的数据
                    {
                        version = _version;//更新客户端版本号
                        Debug.Log("<color=blue>" + dataArr[1] + "</color>");
                        string json = dataArr[1];

                        AllSDKManager.SDKDebug("json:" + json, "white");
                        NativeAdData = JsonUtility.FromJson<NativeAdData>(json);//数据更新
                        isJsonReady = true;
                        LoadBanner();

                    }
                    else
                    {
                        AllSDKManager.SDKDebug("数据没有变更", "white");
                        isJsonReady = true;
                    }
                }
                catch (Exception e)
                {

                    isJsonReady = false;
                    AllSDKManager.SDKDebug("服务器发送的json数据不对");
                }


            }
            else
            {
                isJsonReady = false;
                AllSDKManager.SDKDebug(jsonWWW.error, "white");
            }
            yield return new WaitForSeconds(int.Parse(NativeAdData.check));//每隔30秒向服务器校验一次json配置的改变
        }

    }


    /// <summary>
    /// 向服务器请求图片资源或者ab包
    /// </summary>
    /// <returns></returns>
    IEnumerator RequestBannerData()
    {

        AllSDKManager.SDKDebug("开始请求原生banner", "cyan");


        for (int i = 0; i < NativeAdData.nativeAssetsDataList.Count; i++)
        {
            string adPlayType = NativeAdData.nativeAssetsDataList[i].adPlayType;
            string nickName = NativeAdData.nativeAssetsDataList[i].nickName;
            string assetUrl = NativeAdData.nativeAssetsDataList[i].assetUrl;
            string assetName = NativeAdData.nativeAssetsDataList[i].textureName;
            string jumpUrl = NativeAdData.nativeAssetsDataList[i].jumpUrl;
            AllSDKManager.SDKDebug("URL:" + assetUrl + assetName + jumpUrl, "cyan");
            WWW www = new WWW(assetUrl + assetName);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                NativeAdAssetsDataWWW nativeAdAssetsDataWWW = new NativeAdAssetsDataWWW();
                nativeAdAssetsDataWWW.adPlayType = adPlayType;
                nativeAdAssetsDataWWW.texture = www.texture;
                nativeAdAssetsDataWWW.nickName = nickName;
                nativeAdAssetsDataWWW.texture.name = assetName;
                Debug.Log(www.texture.name);
                nativeAdAssetsDataWWW.jumpUrl = jumpUrl;

                NativeAdAssetsDataWWW.AddAssets(nativeAdAssetsDataWWW);

                OnLoadBanner("");
            }
            else
            {
                OnLoadBanner(www.error);
            }
        }
        AllSDKManager.SDKDebug("原生banner全部加载完等待显示", "cyan");

        isBannerReady = true;

    }



    public void ShowInterstitial()
    {
        AllSDKManager.SDKDebug(Name + ":原生插页还在等她的远方");
    }

    public void ShowRewardedVideo()
    {
        AllSDKManager.SDKDebug(Name + ":原生视频激励还在等他的诗意");
    }

    public void ClickBanner()
    {
        if (NativeAdAssetsDataWWW.GetAssets(currentIndex) != null)
        {
            string jumpUrl = NativeAdAssetsDataWWW.GetAssets(currentIndex).jumpUrl;
            currentJumpUrl = jumpUrl;
        }

        if (isClick)
        {
            AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_NativeAd(Name, "click", currentBanner.name, currentJumpUrl);
            isClick = false;
        }
        AllSDKManager.SDKDebug("点击了原生Banner", "cyan");
        Application.OpenURL(currentJumpUrl);
    }


    void OnLoadBanner(string error = "")
    {
        if (error == "")
        {
            AllSDKManager.SDKDebug(Name + ":原生banner加载完成", "cyan");

        }
        else
        {
            AllSDKManager.SDKDebug(Name + ":原生banner加载失败 失败原因:" + error, "cyan");
        }
    }



    public void HideBanner()
    {
        if (isRequestBanner && isBannerOn)
        {
            bannerImage.gameObject.SetActive(false);
            isRequestBanner = false;
            isBannerOn = false;
        }

    }

    void DisplayBanner()
    {
        timer = 0;
        isClick = true;
        bannerImage.gameObject.SetActive(true);
        isBannerOn = true;
        AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_NativeAd(Name, "show", currentBanner.name, currentJumpUrl);
        AllSDKManager.SDKDebug("++++++ texture.name;" + NativeAdAssetsDataWWW.GetAssets(currentIndex).texture.name, "cyan");
    }
    void SetBannerInfo()
    {       
        string nickName = "";
        if (NativeAdAssetsDataWWW.GetAssets() != null && isBannerReady)
        {
            currentIndex = NativeAdAssetsDataWWW.GetAssets().Count;
            currentIndex = UnityEngine.Random.Range(0, currentIndex);
            bannerImage.texture = NativeAdAssetsDataWWW.GetAssets(currentIndex).texture;
            currentJumpUrl = NativeAdAssetsDataWWW.GetAssets(currentIndex).jumpUrl;
            nickName = nativeAdAssetsDataWWW.GetAssets(currentIndex).nickName;
            currentBanner = bannerImage.texture;
            DisplayBanner();
        }
    }

}
