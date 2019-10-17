using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class StoreItemGift : IStoreItem
{
    public StoreItemGift(string _id, ProductType _productType, string _itemName,int _hint, int _buyId, string _assetName)
    {
        id = _id;
        productType = _productType;
        itemName = _itemName;
        hint = _hint;
        buyId = _buyId;
        assetName = _assetName;
    }
    public string id;
    public string itemName;
    public int hint;
    public int buyId;
    public string assetName;
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
    /// 购买大礼包回调
    /// </summary>
    /// <param name="failed">失败原因</param>
    public void OnBuy(string failed)
    {
        if (failed == "")
        {
           
        }
        else
        {
            AllSDKManager.SDKDebug("大礼包购买失败:" + failed);
        }
        UnityIAP.buyItemCompleteEvent -= OnBuy;
    }
  
   
    /// <summary>
    /// 恢复购买回调
    /// </summary>
    public void OnReStore()
    {
        
    }
}
