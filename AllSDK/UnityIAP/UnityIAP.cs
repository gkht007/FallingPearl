using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public class Receipt
{

	public string Store;
	public string TransactionID;
	public string Payload;

	public Receipt()
	{
		Store = TransactionID = Payload = "";
	}

	public Receipt(string store, string transactionID, string payload)
	{
		Store = store;
		TransactionID = transactionID;
		Payload = payload;
	}
}

public class PayloadAndroid
{
	public string json;
	public string signature;

	public PayloadAndroid()
	{
		json = signature = "";
	}

	public PayloadAndroid(string _json, string _signature)
	{
		json = _json;
		signature = _signature;
	}
}

public class UnityIAP : MonoBehaviour, ISDK, IStoreListener
{

#if UNITY_ANDROID
	string buy_adRemove = "fallingpearl.removead";
    string buy_goldBrick = "fallingpearl.buygold";
#elif UNITY_IOS
   string buy_adRemove = "fallingpearl.removead2";
   string buy_goldBrick = "fallingpearl.buygold2";
#endif
    public delegate void BuyItemComplete(string failed);
    public static event BuyItemComplete buyItemCompleteEvent;
    string currentBuy = "";
    IStoreController controller;
	IExtensionProvider extensions;
	IAppleExtensions appleExtensions;
    ConfigurationBuilder builder;

    Dictionary<string, IStoreItem> buyItemDic = new Dictionary<string, IStoreItem>();//已购买商品列表，主要用于非消耗品恢复
    public string Name
	{
		get
		{
			return "UnityIAP";
		}
	}

	public UnityIAP()
	{

	}
	public void Disable()
	{

	}
    public void AddProduct(IStoreItem _storeItem)
    {
        builder.AddProduct(_storeItem.id, _storeItem.productType);
        buyItemDic.Add(_storeItem.id, _storeItem);
    }
    public void Init()
    {

        currentBuy = buy_adRemove;
        if (controller == null)
            InitIAP();

    }
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    /// <returns></returns>
    private bool IsInitialized()
    {
        return controller != null && extensions != null;
    }
    public void InitIAP()
	{
        if (IsInitialized())
            return;
        builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        AddProduct(new StoreItemAd(buy_adRemove, ProductType.NonConsumable, "removeAd"));

        builder.AddProduct(buy_goldBrick, ProductType.Consumable, new IDs
            {
                {buy_goldBrick, MacAppStore.Name},
                {buy_goldBrick, GooglePlay.Name},

            });
        UnityPurchasing.Initialize(this, builder);       
    }
	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="_controller"></param>
	/// <param name="_extensions"></param>
	public void OnInitialized(IStoreController _controller, IExtensionProvider _extensions)
	{
		controller = _controller;
		extensions = _extensions;
		appleExtensions = _extensions.GetExtension<IAppleExtensions>();
		// appleExtensions.RegisterPurchaseDeferredListener();  //购买延迟
		AllSDKManager.SDKDebug(Name + "初始化成功");
	}
	/// <summary>
	/// 初始化失败
	/// </summary>
	/// <param name="error"></param>
	public void OnInitializeFailed(InitializationFailureReason error)
	{
		AllSDKManager.SDKDebug(Name + "初始化失败:" + error);
	}
	/// <summary>
	/// 内购失败
	/// </summary>
	/// <param name="i"></param>
	/// <param name="p">失败原因</param>
	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
        string failed = "内购失败";
        AllSDKManager.SDKDebug(Name + "内购失败");
		switch (p)
		{
			case PurchaseFailureReason.PurchasingUnavailable:
				AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_End("系统购买功能不可用");
				break;
			case PurchaseFailureReason.ExistingPurchasePending:
				AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_End("在请求新购买时，购买已在进行中");
				break;
			case PurchaseFailureReason.ProductUnavailable:
				AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_End("该商品无法在商店购买");
				break;
			case PurchaseFailureReason.SignatureInvalid:
				AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_End("购买收据的签名验证失败");
				break;
			case PurchaseFailureReason.UserCancelled:
				AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_End("用户选择取消而不是继续购买");
				break;
			case PurchaseFailureReason.PaymentDeclined:
				AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_End("付款有问题");
				break;
			case PurchaseFailureReason.DuplicateTransaction:
				AllSDKManager.GetSDK<AdSdk>().IsEnable = false;
				AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_End("事务已成功完成时出现重复的事务错误");
				break;
			case PurchaseFailureReason.Unknown:
				AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_End("未知失败");
				break;
			default:
				break;
		}
        if (buyItemCompleteEvent != null)
        {
            buyItemCompleteEvent(failed);
        }
        AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_End(failed);
    }
	/// <summary>
	/// 内购完成
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
        Debug.Log(Name + "=======内购完成的函数执行Start");
        if (buyItemDic.ContainsKey(args.purchasedProduct.definition.storeSpecificId))//如果商品id匹配则执行其恢复购买
        {
            buyItemDic[args.purchasedProduct.definition.storeSpecificId].OnReStore();
            Debug.Log(Name + "price:" + args.purchasedProduct.metadata.localizedPriceString);
            Debug.Log(Name + args.purchasedProduct.metadata.localizedTitle + args.purchasedProduct.metadata.localizedDescription);

            //交易号
            Debug.Log(Name + "storeSpecificId:" + args.purchasedProduct.definition.storeSpecificId);

            //回执单  
            Debug.Log(Name + "receipt:" + args.purchasedProduct.receipt);

            //商品的id号
            Debug.Log(Name + "transactionID:" + args.purchasedProduct.transactionID);

            Debug.Log(Name + "成功了！！！！");
        }
        if (String.Equals(args.purchasedProduct.definition.id, buy_adRemove, StringComparison.Ordinal))
		{
            AllSDKManager.GetSDK<AdSdk>().IsEnable = false;
            //移除广告
            Config.RemovedAD();
            AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_End();
            Debug.Log("去除广告");
            var product = controller.products.WithID(buy_adRemove);
			string receipt = product.receipt;
			string currency = product.metadata.isoCurrencyCode;
			int amount = decimal.ToInt32(product.metadata.localizedPrice * 100);
#if UNITY_ANDROID
            Receipt receiptAndroid = JsonUtility.FromJson<Receipt>(receipt);
            PayloadAndroid receiptPayload = JsonUtility.FromJson<PayloadAndroid>(receiptAndroid.Payload);
            //AllSDKManager.GetSDK<AppsFlyerManager>().Event_BusinessGooglePlay(currency, amount, "Ad");
#endif
#if UNITY_IOS
            Receipt receiptiOS = JsonUtility.FromJson<Receipt>(receipt);
            string receiptPayload = receiptiOS.Payload;
			 //AppsFlyerManager.Instance.Event_BusinessAppStore(currency, amount, "Ad");
#endif
        }
        else if(String.Equals(args.purchasedProduct.definition.id, buy_goldBrick, StringComparison.Ordinal))
        {
            var product = controller.products.WithID(buy_goldBrick);
            string receipt = product.receipt;
            string currency = product.metadata.isoCurrencyCode;
            int amount = decimal.ToInt32(product.metadata.localizedPrice * 100);
#if UNITY_ANDROID
            Receipt receiptAndroid = JsonUtility.FromJson<Receipt>(receipt);
            PayloadAndroid receiptPayload = JsonUtility.FromJson<PayloadAndroid>(receiptAndroid.Payload);
            //AllSDKManager.GetSDK<AppsFlyerManager>().Event_BusinessGooglePlay(currency, amount, "Ad");
#endif
#if UNITY_IOS
            Receipt receiptiOS = JsonUtility.FromJson<Receipt>(receipt);
            string receiptPayload = receiptiOS.Payload;
			 //AppsFlyerManager.Instance.Event_BusinessAppStore(currency, amount, "Ad");
#endif
            Debug.Log("购买金币");
            if(PlayerPrefs.HasKey("goldBrick"))
            {
                Globle.goldBrickNum = PlayerPrefs.GetInt("goldBrick");
                PlayerPrefs.DeleteKey("goldBrick");       
            }
            Globle.goldBrickNum += 200;
            PlayerPrefs.SetInt("goldBrick", Globle.goldBrickNum);
        }
        return PurchaseProcessingResult.Complete;
	}
    /// <summary>
    /// 恢复购买
    /// </summary>
    public void RestoringTransctions()
    {

        Debug.Log(Name + "恢复购买开始执行");
        if (!IsInitialized())
        {
            Debug.Log(Name + "初始化未完成不能执行恢复购买");
            return;
        }
        appleExtensions = extensions.GetExtension<IAppleExtensions>();
        appleExtensions.RestoreTransactions(result =>
        {
            //用于UI显示
            if (result)
            {
                Debug.Log(Name + "恢复购买result:" + result + "成功");
            }
            else
            {
                Debug.Log(Name + "恢复购买result:" + result + "失败");

            }
        });
    }
    /// <summary>
    /// 购买去除广告
    /// </summary>
    public void OnPurchaseClicked()
	{
        currentBuy = buy_adRemove;
        buyItemCompleteEvent += buyItemDic[currentBuy].OnBuy;
        controller.InitiatePurchase(currentBuy);
		AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_Start("RemoveAd");
	}
    /// <summary>
    /// 购买金砖
    /// </summary>
    public void OnPurchaseClicked2()
    {
        controller.InitiatePurchase(buy_goldBrick);
        AllSDKManager.GetSDK<AllAnalyticsSdk>().Event_IAP_Start("BuyGoldBrick");
    }
}
