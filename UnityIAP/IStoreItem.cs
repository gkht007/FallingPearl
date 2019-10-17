using UnityEngine.Purchasing;

public interface IStoreItem
{
    /// <summary>
    /// 商品id (wasay.watercup.xxxx)
    /// </summary>
    string id { get; }
    /// <summary>
    /// 商品名 (游戏中的商店里展示或者策划起的名字)
    /// </summary>
    string itemName { get; }
    /// <summary>
    /// 商品类型  （消耗品/非消耗品）
    /// </summary>
    ProductType productType { get; }
    /// <summary>
    /// 当购买完成的回调
    /// </summary>
    /// <param name="failed"></param>
    void OnBuy(string failed);
    /// <summary>
    /// 恢复购买完成
    /// </summary>
    void OnReStore();
}
