using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class LoadingController : MonoBehaviour
{


	//[DllImport("__Internal")]
	//private static extern void _GetVerdorid();

	public void GetVerdorid(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			Debug.Log("IDFA is NULL");
			return;
		}
		else
		{
			Debug.Log("IDFA is " + str);
			//DeveceIdGet.setUUID(str);			
		}

	}


}
public class DeveceIdGet
{
	public static string UUID = "";
	public static void initUUID()
	{
#if UNITY_IPHONE && !UNITY_EDITOR
          // DeveceIdGet._GetIDFA();
#else
		UUID = SystemInfo.deviceUniqueIdentifier;
		Debug.Log("###Utilities getUUID SystemInfo.deviceUniqueIdentifier ### " + UUID);

#endif

}
#if UNITY_IPHONE
	//[DllImport("__Internal")]
	//private static extern void _GetIDFA();

#endif
	//public static string getUUID()
	//{
	//	//test test用户
	//	//  return "123456784";
	//	Debug.Log("###Utilities getUUID UUID ### " + UUID);
	//	return UUID;
	//}
	//public static void setUUID(string uuid)
	//{
	//	Debug.Log("###Utilities setUUID uuid ### " + uuid);
	//	UUID = uuid;
	//}
}