using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GetIDFA : MonoBehaviour
{

//#if UNITY_IPHONE

    public void onGetIDFA(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            Debug.Log("IDFA is NULL");
            return;
        }
        else
        {
            Debug.Log("IDFA is "+str);
            MyUtili.MyUtilities.setUUID(str);
            //LoadingController.instance.iOSReady();
        }

    }

//#endif
}
