using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class StoreItemHint : IStoreItem
{

    public StoreItemHint(string _id, ProductType _productType, string _itemName, int _hint)
    {
        id = _id;
        productType = _productType;
        itemName = _itemName;
        hint = _hint;

    }
    public string id;
    public int hint;
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
    /// 购买提示点回调
    /// </summary>
    /// <param name="failed">失败原因</param>
    public void OnBuy(string failed)
    {
        if (failed == "")
        {

        }
        else
        {
            AllSDKManager.SDKDebug("提示点购买失败:" + failed);
        }
        UnityIAP.buyItemCompleteEvent -= OnBuy;
    }

    /// <summary>
    /// 恢复购买回调
    /// </summary>
    public void OnReStore()
    { }

}
