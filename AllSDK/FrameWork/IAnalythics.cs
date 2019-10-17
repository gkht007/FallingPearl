using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IAnalythics
{
    string Name { get; }
    /// <summary>
    /// 记录是否观看过广告
    /// </summary>
    bool IsLookAd { get; set; }

    /// <summary>
    /// 记录是否使用过提示
    /// </summary>
    bool IsUseHint { get; set; }

    void Disable();
    /// <summary>
    /// 初始化
    /// </summary>
    void Init();

	/// <summary>
	/// 进入填色
	/// </summary>
	/// <param name="_name">图片名称</param>
	/// /// <param name="_where">进入位置</param>
	void Event_Level_Enter(string _name,string _where);
	/// <summary>
	/// 离开填色
	/// </summary>
	/// <param name="_name">图片名称</param> 
	/// <param name="_pass">是否通过(1/0)</param>
	/// <param name="_leaveCause">离开原因(win,failed,backHome,reTry)</param>
	void Event_Level_Leave(string _name, int _pass, string _leaveCause = "");

    /// <summary>
    /// 按钮点击埋点
    /// </summary>
    /// <param name="_page">页面</param>
    /// <param name="_buttonName">按钮名</param>
    void Event_Button(string _page, string _buttonName);

    /// <summary>
    /// 内购开始埋点(此方法必须与Event_IAP_End函数成对使用,否则不会发送到友盟)
    /// </summary>
    /// <param name="_name">内购项目名</param>
    void Event_IAP_Start(string _name);


    /// <summary>
    /// 内购结束埋点(此方法必须与Event_IAP_Start函数成对使用,否则不会发送到友盟)
    /// 注：成功使用此方法
    /// </summary> 
    void Event_IAP_End();
    /// <summary>
    /// 内购结束埋点(此方法必须与Event_IAP_Start函数成对使用,否则不会发送到友盟)
    ///  注：失败使用此方法
    /// </summary>
    /// <param name="_failed">失败原因</param>
    void Event_IAP_End(string _failed = "");

    /// <summary>
    /// 广告播放开始埋点(此方法必须与Event_Ad_End函数成对使用,否则不会发送到友盟)
    /// </summary>
    /// <param name="_adTeam">广告商</param>
    /// <param name="_AdPlayType">广告类型</param>
    /// <param name="_clear">是否清空缓存</param>
    void Event_Ad_Start(string _adTeam, AdPlayType _AdPlayType,bool _clear=true);

    /// <summary>
    /// 广告播放结束埋点(此方法必须与Event_Ad_Start函数成对使用,否则不会发送到友盟)
    /// 注：成功使用此方法
    /// </summary> 
    void Event_Ad_End(string _adTeam,AdPlayType _AdPlayType);

    /// <summary>
    /// 广告播放结束埋点(此方法必须与Event_IAP_Start函数成对使用,否则不会发送到友盟)
    ///  注：失败使用此方法
    /// </summary>
    /// <param name="_failed">失败原因</param>
    void Event_Ad_End(string _adTeam, AdPlayType _AdPlayType,string _failed = "");

    /// <summary>
    /// 服务器接口失败埋点
    /// </summary>
    /// <param name="_api">方法名</param>
    /// <param name="_failed">失败原因</param>
    void Event_ServerAPI(string _api, string _failed = "未知原因");

	/// <summary>
	/// 原生广告点击和展示埋点
	/// </summary>
	/// <param name="_behavior">广告行为（展示，点击）</param>
	/// <param name="_bannerName">图片名</param>
	/// <param name="_jumpUrl">跳转链接</param>
	void Event_NativeAd(string _adTeam, string _behavior, string _bannerName, string _jumpUrl);
}
