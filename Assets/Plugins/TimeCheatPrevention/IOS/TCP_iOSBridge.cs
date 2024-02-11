using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;


public class TCP_iOSBridge
{
#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern long GetElapsedTimeSinceLastBoot();
#endif


	//////////////////////////////////////////////////////////////////////////

	static public long GetTimeInSecSinceLastBoot()
	{
#if UNITY_IOS && !UNITY_EDITOR
		return GetElapsedTimeSinceLastBoot();
#endif
		return 0;
	}

	//////////////////////////////////////////////////////////////////////////

}
