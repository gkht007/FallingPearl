using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class StoreItemMotif : IStoreItem
{
    public StoreItemMotif(string _id, ProductType _productType, string _itemName, int _buyId, string _assetName)
    {
        id = _id;
        productType = _productType;
        itemName = _itemName;
        buyId = _buyId;
        assetName = _assetName;
    }
    public string id;
    public string itemName;
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
    /// 购买皮肤包回调
    /// </summary>
    /// <param name="failed">失败原因</param>
    public void OnBuy(string failed)
    {
        if (failed == "")
        {
         

        }
        else
        {
            AllSDKManager.SDKDebug("皮肤包购买失败:" + failed);
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
