using System.Collections;
using UnityEngine;

public class NativeAdAnalytics : SingletionMono<NativeAdAnalytics>, IAnalythics
{
    string gameName = "FallingPearl";


    string time = System.DateTime.Now.ToString("yyyy-MM-dd");
    public string Name
    {
        get
        {
            return "NativeAdAnalytics";
        }
    }

    public bool IsLookAd { get; set; }
    public bool IsUseHint { get; set; }


    public void Init()
    {
        AllSDKManager.SDKDebug("NativeAdAnalytics初始化", "Blue");
    }
    public void Disable()
    {

    }
    IEnumerator UploadData(string data)
    {
        AllSDKManager.SDKDebug("uploaddata：" + Config.AdServer + "/dada2?" + data, "Blue");
        WWW upload = new WWW(Config.AdServer + "/dada2?" + data);
        yield return upload;
        if (!string.IsNullOrEmpty(upload.error))
        {
            AllSDKManager.SDKDebug("上报失败error:" + upload.error, "Blue");
        }

    }
    public void Event_Ad_End(string _adTeam, AdPlayType _AdPlayType)
    {

    }

    public void Event_Ad_End(string _adTeam, AdPlayType _AdPlayType, string _failed = "")
    {

    }

    public void Event_Ad_Start(string _adTeam, AdPlayType _AdPlayType, bool _clear = true)
    {

    }

    public void Event_Button(string _page, string _buttonName)
    {
        
    }

    public void Event_IAP_End()
    {

    }

    public void Event_IAP_End(string _failed = "")
    {

    }

    public void Event_IAP_Start(string _name)
    {

    }

    public void Event_Level_Enter(string _level, int _hourseHp)
    {

    }

    public void Event_Level_Leave(string _level, int _pass, string _source, int _star, int _hourseHp, string _leaveCause = "", float _brushLife = 1)
    {

    }

    public void Event_NativeAd(string _adTeam, string _behavior, string _bannerName, string _jumpUrl)
    {
        int jiansuo = -1;
        if (_behavior == "click")
            jiansuo = 1;
        else
            if (_behavior == "show")
            jiansuo = 2;
        _bannerName = _bannerName.Split('.')[0];
        string data = "name=" + _bannerName + "&" + "addr=" + _jumpUrl + "&" + "game=" + gameName + "&" + "platform=" + Config.Platform + "&" + "country=" + Config.Country + "&" + "time=" + time + "&" + "jiansuo=" + jiansuo;
        //data = "name=test&addr=1&game=1&phone=2&nation=1&time=1&jiansuo=1";
        StartCoroutine(UploadData(data));
        //HttpServerHelper.UpLoadBury(data);
    }

    public void Event_ServerAPI(string _api, string _failed = "未知原因")
    {

    }

    public void Event_Level_Enter(string _name, string _where)
    {
        throw new System.NotImplementedException();
    }

    public void Event_Level_Leave(string _name, int _pass, string _leaveCause = "")
    {
        throw new System.NotImplementedException();
    }
}
