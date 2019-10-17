using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class StoreItemAd : IStoreItem
{
    public StoreItemAd(string _id, ProductType _productType, string _itemName)
    {
        id = _id;
        productType = _productType;
        itemName = _itemName;
    }
    public string id;
    public string itemName;
    public ProductType productType;

    string IStoreItem.id
    {
        get
        {
            return id;
        }

    }
    string IStoreItem.itemName
    {
        get
        {
            return itemName;
        }

    }
    ProductType IStoreItem.productType
    {
        get
        {
            return productType;
        }

    }
    /// <summary>
    ///  去广告购买完成回调
    /// </summary>
    /// <param name="failed">失败原因，没有即为成功</param>
    public void OnBuy(string failed)
    {
        Debug.Log("UnityIAP移除广告购买情况：" + failed);
        if (failed == "")
        {

        }
        else
        {
            AllSDKManager.SDKDebug("去广告失败回调:" + failed);
        }
        UnityIAP.buyItemCompleteEvent -= OnBuy;
    }
    /// <summary>
    /// 恢复购买成功回调
    /// </summary>
    public void OnReStore()
    {

    }
}
