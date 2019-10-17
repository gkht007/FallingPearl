using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityIAPtest : MonoBehaviour
{
	public void OnClickBuy()
	{
		AllSDKManager.GetSDK<UnityIAP>().OnPurchaseClicked();
	}
    public void OnClickBuy2()
    {
        AllSDKManager.GetSDK<UnityIAP>().OnPurchaseClicked2();
    }
}