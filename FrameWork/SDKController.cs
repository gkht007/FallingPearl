using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// 场景中的托管类
/// 作用：
///     1.初始化SDK
///     2.还没想好。。。
/// </summary>

public class SDKController : MonoBehaviour
{
    public static SDKController instance;
    bool isEnable = true;

    string text = "启用广告";
    int index = 0;

    public float horizontal = 0;
    public float vertical = 50;
    public float width = 200;
    public float height = 50;
    public bool isDisplay = true;
    public GameObject adv;
    
    private void Start()
    {
        instance = this;
        name = "SDKController";
        DontDestroyOnLoad(this.gameObject);
        // Config.setUUID();//获取并设置设备ID
        MyUtili.MyUtilities.initUUID();
         //针对UnityAds做的特殊初始化处理(=======Start
         //它为啥和别的Ad初始化不一样？
         //因为unity的广告调用要开他娘的携程,
         //开携程就要继承mono,
         //继承mono就他娘的不能用new,
         //这就是下面这几行蛋疼的代码在这里的原因
         GameObject unityAds = new GameObject();
        unityAds.name = "UnityAds";
        unityAds.AddComponent<UnityAds>();
        DontDestroyOnLoad(unityAds);
		//================End)

		////对于AppsFlyerManager的初始化处理
		//GameObject AppsFlyerManager = new GameObject();
		//AppsFlyerManager.name = "AppsFlyerManager";
		//AppsFlyerManager.AddComponent<AppsFlyerManager>();
		//DontDestroyOnLoad(AppsFlyerManager);

		//对于NativeAnalytics的初始化处理
		GameObject NativeAnalytics = new GameObject();
		NativeAnalytics.name = "NativeAnalytics";
		NativeAnalytics.AddComponent<NativeAnalytics>();
		DontDestroyOnLoad(NativeAnalytics);

        //对于原生广告的初始化处理
        //GameObject nativeAds = new GameObject();
        //nativeAds.name = "NativeAds";
        //nativeAds.AddComponent<NativeAds>();
        //DontDestroyOnLoad(nativeAds);
        AllSDKManager.Init();
        MyUtili.MyUtilities.initUUID();
        AllSDKManager.GetSDK<AdSdk>().IsEnable = Config.isRemovedAD() ? false : true;
        //AllSDKManager.GetSDK<AdSdk>().IsEnable = true;
        AllSDKManager.SDKDebug(Config.isRemovedAD().ToString());




        AdShowControl.GameContinueCountNumber = 2;
        AdShowControl.CompleteRetryCountNumber = 2;
        AdShowControl.CompleteGameCountNumber = 3;

    }

    /// <summary>
    /// Test
    /// </summary>
#if UNITY_EDITOR
    string currentScene;
    string adTypeText;
    private void OnGUI()
    {
        if (isDisplay)
        {

            GUI.color = Color.white;
            GUI.skin.label.fontSize = 40;
            GUI.skin.button.fontSize = 30;

            if (GUI.Button(new Rect(horizontal, vertical, width, height), text))
            {
                index++;
                if (index % 2 == 0)
                {
                    text = "启用广告";
                    isEnable = true;
                }
                else
                {
                    text = "禁用广告";
                    isEnable = false;
                }

               // AllSDKManager.GetSDK<AdSdk>().IsEnable = isEnable;
            }



            if (GUI.Button(new Rect(0, 100, 200, 100), "原生广告"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("NativeAdsTest");
            }
            if (GUI.Button(new Rect(0, 200, 200, 100), "Google广告"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GoogleMobAdsTest");
            }
            if (GUI.Button(new Rect(0, 300, 200, 100), "IronSource广告"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("IronsourceAdsTest");
            }
            if (GUI.Button(new Rect(0, 400, 200, 100), "Unity广告"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("UnityAdsTest");
            }
            if (GUI.Button(new Rect(0, 500, 200, 100), "All"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("AdsTest");
            }

            GUI.color = Color.yellow;
            GUI.Label(new Rect(0, 600, 200, 100), currentScene);

            currentScene = "当前场景：" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            GUI.Label(new Rect(0, 700, 200, 100), adTypeText);

           // adTypeText = "当前策略：" + AllSDKManager.GetSDK<AdSdk>().AdType.ToString();
        }
    }



#endif

}
