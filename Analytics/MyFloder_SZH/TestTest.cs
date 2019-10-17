using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTest : MonoBehaviour
{

    void Start()
    {

    }


    void Update()
    {
      
    }

    public void sendMessage111111111111()
    {
        Debug.Log("666");
        AllSDKManager.GetSDK<AllAnalyticsSdk>().GetAnalythicsSdk<NativeAnalytics>().Event_Button("TestPage", "Quit");
        AllSDKManager.GetSDK<AllAnalyticsSdk>().GetAnalythicsSdk<NativeAnalytics>().Test();
    }
}
