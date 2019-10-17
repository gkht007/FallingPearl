using BestHTTP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class HttpServerHelper : SingletionMono<HttpServerHelper>
{
    /// <summary>
    /// 埋点上报
    /// </summary>
    /// <param name="upLoadBuryData">提交的数据</param>
    /// <returns></returns>
    public static bool UpLoadBury(UpLoadBuryData upLoadBuryData)
    {

        if (upLoadBuryData == null)
        {
            AllSDKManager.SDKDebug("==数据上报失败：没有数据上报===");
            return false;
        }
        HTTPRequest uploadRequest = new HTTPRequest(new Uri(Config.UpLoadBuryUrl), HTTPMethods.Post, OnUpLoadBury);

        //AllSDKManager.SDKDebug("==准备上报数据===url:" + Config.UpLoadBuryUrl);
        //AllSDKManager.SDKDebug("topic:" + upLoadBuryData.topic);
        //AllSDKManager.SDKDebug("platform:" + upLoadBuryData.platform);
        //AllSDKManager.SDKDebug("version:" + upLoadBuryData.version);
        //AllSDKManager.SDKDebug("userId:" + upLoadBuryData.userId);
        //AllSDKManager.SDKDebug("gameId:" + upLoadBuryData.gameId);
        //AllSDKManager.SDKDebug("deviceId:" + upLoadBuryData.deviceId);

        //AllSDKManager.SDKDebug("action:" + upLoadBuryData.action);

        uploadRequest.RawData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(upLoadBuryData));
        AllSDKManager.SDKDebug("NativeAnalytics:  "+"uploadJson:" + JsonUtility.ToJson(upLoadBuryData), "yellow");
        uploadRequest.Send();
        AllSDKManager.SDKDebug("NativeAnalytics:  "+"==数据上报成功===", "yellow");
        return true;
    }
    /// <summary>
    /// 上报情况回调
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    static void OnUpLoadBury(HTTPRequest request, HTTPResponse response)
    {
        AllSDKManager.SDKDebug("NativeAnalytics:  " + "==数据上报回调===", "yellow");
        AllSDKManager.SDKDebug("NativeAnalytics:  " + "State:" + request.State, "yellow");

        switch (request.State)
        {
            case HTTPRequestStates.Initial:
                break;
            case HTTPRequestStates.Queued:
                break;
            case HTTPRequestStates.Processing:
                break;
            case HTTPRequestStates.Finished:
                break;
            case HTTPRequestStates.Error:
                Debug.Log("Error:" + request.Exception.Message);
                break;
            case HTTPRequestStates.Aborted:
                break;
            case HTTPRequestStates.ConnectionTimedOut:
                break;
            case HTTPRequestStates.TimedOut:
                break;
            default:
                break;
        }
    }
}
