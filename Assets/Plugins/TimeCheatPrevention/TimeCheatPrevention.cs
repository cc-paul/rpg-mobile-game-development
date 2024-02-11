using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Events;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

public class TimeCheatPrevention : MonoBehaviour
{
	[Header("Debug")]

	// Debug Output
	public bool m_DebugOutput = false;

	//////////////////////////////////////////////////////////////////////////

	[Header("Callback")]

	[Tooltip("When it is impossible to get a time estimate or a server time, the plugin " +
		"will retry getting a Internet based time this many seconds before triggering the " +
		"callback with the device time instead.")]
	[Range(1.0f, 10.0f)]
	public float m_CallbackTimeout = 3.0f;

	/// <summary>
	/// Callback that triggers as soon as there is either a server-based time,
	/// or a cheat-free estimate. If neither is available, the callback will trigger
	/// after a time threshold with regardless. This ensures that the callback will be called
	/// regardless.
	/// Register with:
	/// TimeCheatPrevention.OnCheatFreeTimeAvailable.AddListener(<YourFunctionName>);
	/// and unregister when you've received the callback.
	/// </summary>
	static public UnityEvent OnCheatFreeTimeAvailable = new UnityEvent();

	[Tooltip("Callback that triggers as soon as there is either a server-based time, " +
		"or a cheat-free estimate. If neither is available, the callback will trigger after a time " +
		"threshold with regardless. This ensures that the callback will be called regardless")]
	public UnityEvent m_OnCheatFreeTimeAvailable = new UnityEvent();

	// Internal helpers for callback triggering
	//////////////////////////////////////////////////////////////////////////
	//! Triggers the callback(s) next frame - this delay allows other scripts to subscribe to the event at game start
	private bool m_CallbackNeedsTriggering = false;
	//! TimeOut for when the callback should be triggered regardless
	private float m_CallbackTriggerTimeOut = -1.0f;

	//////////////////////////////////////////////////////////////////////////

	// Singleton instance, private, because all access functions are static
	static private TimeCheatPrevention Instance = null;

	// Server Time
	private DateTime m_ServerTimeStampUTC = new DateTime();
	private float m_ServerTimeLastUpdated = -1.0f; // How many seconds ago did we last receive a valid server time stamp
	private float m_ServerTimeRetryTimer = -1.0f;
	private float m_ServerTimeRetryTimerNext = 1.0f;
	private bool m_ServerTimeRequestRunning = false; //! Server request sent, waiting for response

	// Device Uptime
	private DateTime m_LastConfirmedTimeStampUTC = new DateTime();
	private float m_DeviceUpTimeDelta = -1.0f; // This is the time passed since the last confirmed time stamp, calculated from the delta between the current and saved device uptime counters
	private bool m_UpdateEstimationTimestampWithServerTimestamp;
	private bool m_UsingDeviceTime = true;

	private bool m_SkipFrame = true;
	private static List<string> sBackupServers = new List<string>() { "138.68.46.177", "45.79.36.123", "185.213.26.143", "204.11.201.12" };

	// Helper variables
	private Coroutine m_CoroutineHandle = null;
	private Thread m_Thread;

	//
	static private bool sUseThreads = true;

	//////////////////////////////////////////////////////////////////////////

	private void Awake()
	{
		// Singleton - one of these is enough
		if (Instance != null && Instance != this)
		{
			DestroyImmediate(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		InitializeTimeCheatPrevention();
	}

	//////////////////////////////////////////////////////////////////////////

	private void InitializeTimeCheatPrevention()
	{
		// Initialize quickly, in case the time is queried before estimation is complete
		m_ServerTimeLastUpdated = -1.0f;
		m_ServerTimeStampUTC = DateTime.UtcNow;
		m_UpdateEstimationTimestampWithServerTimestamp = false;
	}

	//////////////////////////////////////////////////////////////////////////

	private void Start()
	{
		RefreshCheatFreeTime();
	}

	//////////////////////////////////////////////////////////////////////////

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			// Refresh times (since we haven't been able to update it)
			Log("Refreshing cheat free time.");

			// Invalidate server time 
			m_ServerTimeLastUpdated = -1.0f;
			m_ServerTimeStampUTC = DateTime.UtcNow;

			// This will also retrigger the callback
			RefreshCheatFreeTime();
		}
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Updates both the locally estimated time and the server based time, and will trigger
	/// the callback once it has a cheat-free time (or the timeout is reached)
	/// </summary>
	private void RefreshCheatFreeTime()
	{
		// Start the timeout clock for the callback
		m_CallbackTriggerTimeOut = m_CallbackTimeout;

		// Load last valid time stamp and estimate from there
		UpdateEstimatedTime();

		// Start Query to server for Internet based time
		UpdateServerTime();

		// Skip one frame during interpolation
		m_SkipFrame = true;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// This function will try to calculate a cheat-free time without using the Internet.
	/// If successful, it will flag to trigger the callback next frame. If a reliable time
	/// cannot be estimated, the callback will not be triggered and instead wait for either
	/// an Internet-based time to become available, or for the timeout to be reached.
	/// </summary>
	private void UpdateEstimatedTime()
	{
		// Check if there is a chance for Internet. 
		// Note that just because a network appears reachable, it doesn't
		// mean that there actually is a valid connection. It could be that there
		// is a WiFi which requires a login, but auto-connects the device right away (leading 
		// any web requests to a landing page).
		// However, if there is definitely NO Internet reported, then we can fire the 
		// callback right after the estimation is complete, since there's no point in waiting.
		bool weAreOffline = (Application.internetReachability == NetworkReachability.NotReachable);


		// If there is no previously save time stamp - we have to initialize on first app start
		// The same is true if this platform doesn't support time estimation
		if (!IsPlatformTimeEstimationSupported())
		{
			Log("This platform doesn't support time estimation (only Android and iOS) - using device time instead");
			m_UsingDeviceTime = true;
			m_LastConfirmedTimeStampUTC = DateTime.UtcNow;
			m_DeviceUpTimeDelta = 0;

			// If there's no chance for a server based time, let's fire the callback right away
			if (weAreOffline)
				m_CallbackNeedsTriggering = true;
		}
		else if (!PlayerPrefs.HasKey("OS_UnbiasedTimeStamp"))
		{
			// Once, initialize with the current device time. 
			// Will be overridden as soon as we have an Internet based time.
			Log("No previously confirmed & saved time stamp. Initializing with system time.");
			UpdateEstimationTimeStamp(DateTime.UtcNow);
			m_UsingDeviceTime = true;

			// If there's no chance for a server based time, let's fire the callback right away
			if (weAreOffline)
				m_CallbackNeedsTriggering = true;
		}
		else
		{
			// Load estimated time
			string timeStamp = PlayerPrefs.GetString("OS_UnbiasedTimeStamp", "");
			m_LastConfirmedTimeStampUTC = DateTime.Parse(timeStamp);
			int prevUptime = PlayerPrefs.GetInt("OS_UnbiasedDeviceUpTime", 0);
			int currentUptime = (int)GetElapsedTimeSinceLastBoot();
			if (currentUptime < prevUptime)
			{
				// Device was rebooted - we need to re-init (which means trusting the system time)
				Log("Device was rebooted. Re-initializing with system time.");
				UpdateEstimationTimeStamp(DateTime.UtcNow);
				m_UsingDeviceTime = true;

				// If there's no chance for a server based time, let's fire the callback right away
				if (weAreOffline)
					m_CallbackNeedsTriggering = true;
			}
			else
			{
				m_DeviceUpTimeDelta = currentUptime - prevUptime;
				if (m_DebugOutput) // avoid the string concatenation if we can
					Log("Loading estimated offline timer. Seconds since boot: " + currentUptime.ToString("N0") + " before " + prevUptime.ToString("N0") + ". Last valid time stamp: " + m_LastConfirmedTimeStampUTC + " making it now: " + m_LastConfirmedTimeStampUTC.AddSeconds(m_DeviceUpTimeDelta).ToString());
				m_UsingDeviceTime = false;

				// We have a cheat-free estimated time, so mark the callback for triggering
				// If there's the chance of an Internet connection, instead of triggering right 
				// away next frame, delay the trigger for 1.5 seconds,
				// to give the Internet time a chance to be received. 
				// Server based timing will always be more accurate.
				if (weAreOffline)
					m_CallbackNeedsTriggering = true;
				else if (m_CallbackTriggerTimeOut > 1.5f)
					m_CallbackTriggerTimeOut = 1.5f;
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Saves a time stamp of the last confirmed time (usually Internet time), this is used as 
	/// a base time for calculation if there is no Internet time available in the future.
	/// </summary>
	/// <param name="dt"></param>
	private void UpdateEstimationTimeStamp(DateTime dt)
	{
		m_LastConfirmedTimeStampUTC = dt;
		m_DeviceUpTimeDelta = 0;

		// No need to save estimation data if this platform doesn't support it
		if (!IsPlatformTimeEstimationSupported())
			return;

		string timeStamp = "";
		try
		{
			timeStamp = dt.ToString();
		}
		catch (System.Exception ex)
		{
			// Building custom timestamp in case the automatic ToString conversion fail (there's a Unity bug where that sometimes happens)
			var now = dt;
			var hour = now.Hour;
			if (hour > 12) hour -= 12;
			timeStamp = now.Month.ToString("0") + "/" + now.Day.ToString("0") + "/" + now.Year + " " + hour.ToString("0") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00") + " " + ((now.Hour > 11) ? "PM" : "AM");
			if (m_DebugOutput) Log("Caught exception while updating timestamp: " + ex.ToString() + " --- Message: " + ex.Message);
		}

		int elapsedSecondsSinceBoot = (int)GetElapsedTimeSinceLastBoot();
		PlayerPrefs.SetString("OS_UnbiasedTimeStamp", timeStamp);
		PlayerPrefs.SetInt("OS_UnbiasedDeviceUpTime", elapsedSecondsSinceBoot);

		if (m_DebugOutput) // avoid unnecessary string operations
			Log("Saving unbiased time stamp for time estimation. Now: " + m_LastConfirmedTimeStampUTC.ToString() + " Seconds since last boot: " + elapsedSecondsSinceBoot.ToString("N0"));
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Helper function to get the number of seconds that have elapsed since the device was last booted.
	/// Only supported on Android and iOS. This value is used to estimate the time when there is no Internet connection.
	/// </summary>
	/// <returns></returns>
	private static long GetElapsedTimeSinceLastBoot()
	{
		if (!IsPlatformTimeEstimationSupported())
		{
			// Since we don't have a device that we can use for an estimate, let's use local time
			if (Instance != null)
			{
				Log("Unsupported platform, we're using device time");
				long timeDifference = (long)DateTime.UtcNow.Subtract(Instance.m_LastConfirmedTimeStampUTC).TotalSeconds;
				return timeDifference;
			}
			else
				return 0;
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaClass systemClock = new AndroidJavaClass("android.os.SystemClock");
			return (systemClock.CallStatic<long>("elapsedRealtime")) / 1000; // because this function returns milliseconds
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return TCP_iOSBridge.GetTimeInSecSinceLastBoot();
		}

		return 0;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Starts a new attempt to get the correct time from the Internet
	/// </summary>
	private void UpdateServerTime()
	{
		// Don't start another web request if we're still waiting to hear back form the last one
		if (m_ServerTimeRequestRunning)
			return;
		m_ServerTimeRequestRunning = true;

		// Use an increasing amount of wait time in between requests (up to a max wait time value)
		m_ServerTimeRetryTimer = m_ServerTimeRetryTimerNext;
		m_ServerTimeRetryTimerNext += 1.0f;
		if (m_ServerTimeRetryTimerNext > 10.0f) // max wait time: 10 seconds
			m_ServerTimeRetryTimerNext = 10.0f;

		// Get time from server (this takes a moment)
		{
			if (sUseThreads)
			{
				// Thread based version
				if (m_Thread != null && m_Thread.IsAlive)
					m_Thread.Abort();
				m_Thread = new Thread(GetServerTime_Thread);
				m_Thread.Start();
			}
			else
			{
				// Coroutine based version - blocking calls
				if (m_CoroutineHandle != null)
					StopCoroutine(m_CoroutineHandle); // Make sure this isn't running already (stalling from Internet reachability checks or similar)
				m_CoroutineHandle = StartCoroutine(GetServerTime());
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////

	private void GetServerTime_Thread(object obj)
	{
		const string ntpServer = "pool.ntp.org";
		IPAddress[] addresses = null;
		try
		{
			IPHostEntry iPHostEntry = Dns.GetHostEntry(ntpServer);
			if (iPHostEntry != null)
			{
				addresses = iPHostEntry.AddressList;
				if (m_DebugOutput)
					Log("Received " + addresses.Length + " time server addresses from pool.");
			}
		}
		catch (System.Exception ex)
		{
			if (m_DebugOutput)
				Debug.LogWarning("[Time Cheat Prevention] Exception occurred when resolving host entry for " + ntpServer + ". Will use fallback servers instead. Error message: " + ex.Message);
			addresses = null;
		}

		// Loop and try all servers until one responds
		if (addresses != null && addresses.Length > 0)
		{
			for (int i = 0; i < addresses.Length; i++)
			{
				IPAddress targetAddress = addresses[i];
				if (targetAddress == null)
					continue;

				if (m_DebugOutput)
					Log("Attempting server " + (i + 1).ToString("0") + ": " + targetAddress.ToString());

				if (TryGetServerTimeFromIP(targetAddress))
					return;
			} // ~for loop over all servers

			// None of the (usually 4) servers responded at all or correctly. Try one of the backup servers.
			if (m_DebugOutput)
				Log("Dynamic server resolution failed. Will use backup servers.");
		}
		else
		{
			Log("Attempting to use backup servers.");
		}


		// Reset waiting timers 
		m_ServerTimeRetryTimerNext = 1.0f;
		m_ServerTimeRetryTimer = 1.0f;
		m_ServerTimeLastUpdated = -1;

		for (int i = 0; i < 4; i++) // try a few different times to increase the chances
		{
			// Pick one at random to not overburden the load on one specific server
			string serverAddress = sBackupServers[UnityEngine.Random.Range(0, sBackupServers.Count)];
			IPAddress newAddress = IPAddress.Parse(serverAddress);
			if (TryGetServerTimeFromIP(newAddress))
				return;
		}

		Debug.LogWarning("[Time Cheat Prevention] Time could not be received from servers. None of the servers (including fallback servers) supplied a valid response. Will try again in a few seconds.");
		m_ServerTimeRequestRunning = false;
	}

	//////////////////////////////////////////////////////////////////////////

	IEnumerator GetServerTime()
	{
		const string ntpServer = "pool.ntp.org";

		// Wait until there is Internet
		while (Application.internetReachability == NetworkReachability.NotReachable)
			yield return null;

		IPAddress[] addresses = null;
		try
		{
			IPHostEntry iPHostEntry = Dns.GetHostEntry(ntpServer);
			if (iPHostEntry != null)
			{
				addresses = iPHostEntry.AddressList;
				if (m_DebugOutput)
					Log("Received " + addresses.Length + " time server addresses from pool.");
			}
		}
		catch (System.Exception ex)
		{
			if (m_DebugOutput)
				Debug.LogWarning("[Time Cheat Prevention] Exception occurred when resolving host entry for " + ntpServer + ". Will use fallback servers instead. Error message: " + ex.Message);
			addresses = null;
		}

		// Loop and try all servers until one responds
		if (addresses != null && addresses.Length > 0)
		{
			for (int i = 0; i < addresses.Length; i++)
			{
				IPAddress targetAddress = addresses[i];
				if (targetAddress == null)
					continue;

				if (m_DebugOutput)
					Log("Attempting server " + (i + 1).ToString("0") + ": " + targetAddress.ToString());

				if (TryGetServerTimeFromIP(targetAddress))
					yield break;
			} // ~for loop over all servers

			// None of the (usually 4) servers responded at all or correctly. Try one of the backup servers.
			if (m_DebugOutput)
				Log("Dynamic server resolution failed. Will use backup servers.");
		}
		else
		{
			Log("Attempting to use backup servers.");
		}


		// Reset waiting timers 
		m_ServerTimeRetryTimerNext = 1.0f;
		m_ServerTimeRetryTimer = 1.0f;
		m_ServerTimeLastUpdated = -1;

		for (int i = 0; i < 4; i++) // try a few different times to increase the chances
		{
			// Pick one at random to not overburden the load on one specific server
			string serverAddress = sBackupServers[UnityEngine.Random.Range(0, sBackupServers.Count)];
			IPAddress newAddress = IPAddress.Parse(serverAddress);
			if (TryGetServerTimeFromIP(newAddress))
				yield break;
		}

		Debug.LogWarning("[Time Cheat Prevention] Time could not be received from servers. None of the servers (including fallback servers) supplied a valid response. Will try again in a few seconds.");
		m_ServerTimeRequestRunning = false;
		yield break;
	}

	private bool TryGetServerTimeFromIP(IPAddress targetAddress)
	{
		var ntpData = new byte[48];
		ntpData[0] = 0x1B; // LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

		try
		{
			// Below is the version using System.Net sockets instead of Unity Web Requests (which would mean blocking calls)
			var ipEndPoint = new IPEndPoint(targetAddress, 123);
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Connect(ipEndPoint);
			socket.ReceiveTimeout = 3000;
			socket.Send(ntpData);
			socket.Receive(ntpData);
			socket.Close();

			if (ntpData == null || ntpData.Length < 48)
			{
				if (m_DebugOutput)
					Log("Server response too short, skipping server " + targetAddress.ToString());
				return false;
			}

			if (m_DebugOutput)
				Log("Response received, " + ntpData.Length + " bytes.");

			ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
			ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

			var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
			if (milliseconds < 1702125956388)
			{
				// Invalid result
				if (m_DebugOutput)
					Log("Invalid result (ms: " + milliseconds + "), skipping server " + targetAddress.ToString());
				return false;
			}

			var serverDateTimeUTC = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

			// Update local server timestamp and reset time offset, so it can be interpolated locally
			m_ServerTimeStampUTC = serverDateTimeUTC;
			m_ServerTimeLastUpdated = 0.0f;

			if (m_DebugOutput)
				Log("Received server time: " + m_ServerTimeStampUTC.ToLongDateString() + " " + m_ServerTimeStampUTC.ToLongTimeString() + " (ms: " + milliseconds.ToString("0") + ")");

			// Since we have a confirmed time now, update unbiased calculation locally
			m_UpdateEstimationTimestampWithServerTimestamp = true;
			m_UsingDeviceTime = false;

			// Reset waiting timers 
			m_ServerTimeRetryTimerNext = 1.0f;
			m_ServerTimeRetryTimer = 1.0f;

			// Mark callback for triggering next frame
			m_CallbackNeedsTriggering = true;

			m_ServerTimeRequestRunning = false;
			return true;
		}
		catch (System.Exception ex)
		{
			if (m_DebugOutput)
				Debug.LogWarning("[Time Cheat Prevention] Error while getting time from " + targetAddress.ToString() + ". Will try different server. Message: " + ex.Message);
			return false;
		}
	}

	//////////////////////////////////////////////////////////////////////////

	void Update()
	{
		// Handle Callback
		if (m_CallbackNeedsTriggering)
			TriggerOnCheatFreeTimeAvailableCallback();

		if (m_UpdateEstimationTimestampWithServerTimestamp)
		{
			m_UpdateEstimationTimestampWithServerTimestamp = false;
			UpdateEstimationTimeStamp(m_ServerTimeStampUTC);
		}

		// Handle frame skipping needed for estimation
		if (m_SkipFrame)
		{
			m_SkipFrame = false;
			return;
		}

		// Callback timeout
		if (m_CallbackTriggerTimeOut > 0.0f)
		{
			m_CallbackTriggerTimeOut -= Time.unscaledDeltaTime;
			if (m_CallbackTriggerTimeOut <= 0.0f)
				TriggerOnCheatFreeTimeAvailableCallback();
		}


		// Local estimation is always updated
		m_DeviceUpTimeDelta += Time.unscaledDeltaTime;

		// Server time is only updated if we actually have a server time
		if (m_ServerTimeLastUpdated >= 0)
		{
			// Server timestamp offset
			m_ServerTimeLastUpdated += Time.unscaledDeltaTime;
		}
		else
		{
			// Regularly try to contact time server until we get a server time
			m_ServerTimeRetryTimer -= Time.unscaledDeltaTime;

			if (m_ServerTimeRetryTimer <= 0.0f && !m_ServerTimeRequestRunning)
				UpdateServerTime();
		}
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Calls back all registered listeners to tell them it is now safe to use GetDateTimeUTC.
	/// Listeners can register either using the Prefab in the scene or by calling
	/// TimeCheatPrevention.OnCheatFreeTimeAvailable.AddListener(<YourFunctionName>);
	/// You must unregister after the callack, or you might receive the callback multiple times.
	/// </summary>
	private void TriggerOnCheatFreeTimeAvailableCallback()
	{
		m_CallbackNeedsTriggering = false;
		m_CallbackTriggerTimeOut = -1.0f;
		m_OnCheatFreeTimeAvailable.Invoke();
		OnCheatFreeTimeAvailable.Invoke();
	}

	//////////////////////////////////////////////////////////////////////////

	static private void Log(string message)
	{
		if (Instance == null || !Instance.m_DebugOutput)
			return;

		Debug.Log("[Time Cheat Prevention] " + message);
	}

	//////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Returns the time int UTC, either from server or locally estimated
	/// </summary>
	/// <returns></returns>
	static public DateTime GetDateTimeUTC()
	{
		if (Instance.m_ServerTimeLastUpdated < 0)
		{
			DateTime currentEstimate = Instance.m_LastConfirmedTimeStampUTC;
			if (Instance.m_DeviceUpTimeDelta > 0)
				currentEstimate = currentEstimate.AddSeconds(Instance.m_DeviceUpTimeDelta);

			return currentEstimate;
		}

		DateTime currentTime = Instance.m_ServerTimeStampUTC;
		currentTime = currentTime.AddSeconds(Instance.m_ServerTimeLastUpdated);

		return currentTime;
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Returns whether or not GetDateTimeUTC would return a server based time, or just an estimate.
	/// Note that you can call GetDateTimeUTC regardless, but if this returns false and there is an
	/// active Internet connection, maybe try waiting for a sec?
	/// </summary>
	static public bool HasServerTime()
	{
		if (Instance == null)
			return false;

		return (Instance.m_ServerTimeLastUpdated >= 0);
	}

	/// <summary>
	/// Returns true if there is no server time and the local time couldn't be estimated, probably due to a device reboot.
	/// Time might have been tampered with, cheat-free cannot be guaranteed.
	/// </summary>
	static public bool IsUsingLocalDeviceTime()
	{
		if (Instance == null)
			return true;

		if (HasServerTime())
			return false;

		return Instance.m_UsingDeviceTime;
	}

	/// <summary>
	/// Returns true if there is no time from the Internet, but we have a local, cheat-free estimate.
	/// </summary>
	/// <returns></returns>
	static public bool IsUsingEstimatedTime()
	{
		return !IsUsingLocalDeviceTime() && !HasServerTime() && IsPlatformTimeEstimationSupported();
	}

	//////////////////////////////////////////////////////////////////////////

	static public bool IsPlatformTimeEstimationSupported()
	{
#if UNITY_EDITOR
		return false;
#else
		if (Application.platform == RuntimePlatform.Android
			|| Application.platform == RuntimePlatform.IPhonePlayer)
			return true;

		return false;
#endif
	}

	//////////////////////////////////////////////////////////////////////////
	// Time Stamp Convenience functions
	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Returns the current timezone independent date/time as a string, which can be saved
	/// </summary>
	public static string GetTimeStamp()
	{
		// Don't log out spam on a platform that doesn't support estimation if we are offline (if online, fire away, because then we SHOULD have a proper server time)
		//if (IsUsingLocalDeviceTime() && IsPlatformTimeEstimationSupported())
		if (IsUsingLocalDeviceTime() && IsPlatformTimeEstimationSupported() && Application.internetReachability != NetworkReachability.NotReachable)
			Debug.LogWarning("[Time Cheat Prevention] Time Stamp is using device time. It might not be cheat-free.");

		DateTime utcNow = GetDateTimeUTC();
		try
		{
			// #Release Because of a bug in Unity, we're creating our own timestamp here, to avoid IndexOutOfRange exceptions
			// ORIGINAL Code (due to an implementation issue in Unity (?) this sometimes causes IndexOutOfRange exceptions)
			//string timeStamp = (utcNow.CompareTo(new DateTime(2000, 1, 1)) > 0) ? now.ToString() : DateTime.UtcNow.ToString();
			// Format: 6/15/2019 1:48:19 PM
			var now = (utcNow.CompareTo(new DateTime(2000, 1, 1)) > 0) ? utcNow : DateTime.UtcNow;
			var hour = now.Hour;
			if (hour > 12) hour -= 12;
			string timeStamp = now.Month.ToString("0") + "/" + now.Day.ToString("0") + "/" + now.Year + " " + hour.ToString("0") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00") + " " + ((now.Hour > 11) ? "PM" : "AM");

			Log("Creating timestamp: " + timeStamp);
			return timeStamp;
		}
		catch (System.Exception ex)
		{
			Debug.LogError("[Time Cheat Prevention] Caught exception in function 'GetTimeStamp()' when trying to convert DateTime variable to a string. Exception: " + ex.Message + ". now valid? (later than 2000) ? " + (utcNow.CompareTo(new DateTime(2000, 1, 1)) > 0) + " last server time update: " + Instance.m_ServerTimeLastUpdated.ToString("0.00"));
			// Trying to create our own custom timestamp instead
			// Format: 6/15/2019 1:48:19 PM
			var now = (utcNow.CompareTo(new DateTime(2000, 1, 1)) > 0) ? utcNow : DateTime.UtcNow;
			var hour = now.Hour;
			if (hour > 12) hour -= 12;
			string timeStamp = now.Month.ToString("0") + "/" + now.Day.ToString("0") + "/" + now.Year + " " + hour.ToString("0") + ":" + now.Minute.ToString("00") + ":" + now.Second.ToString("00") + " " + ((now.Hour > 11) ? "PM" : "AM");

			Log("Creating timestamp: " + timeStamp);
			return timeStamp;
		}
	}

	/// <summary>
	/// Reads the provided timestamp string and converts it back into a date/time.
	/// </summary>
	public static DateTime ParseTimeStamp(ref string timeStamp)
	{
		if (timeStamp == null || timeStamp.Length == 0)
		{
			Debug.LogError("[Time Cheat Prevention] Invalid TimeStamp provided (length = 0). Returning current date/time");
			return GetDateTimeUTC();
		}

		DateTime parsedDateTime = new DateTime();
		if (!DateTime.TryParse(timeStamp, out parsedDateTime))
		{
			Debug.LogError("[Time Cheat Prevention] Provided TimeStamp '" + timeStamp + "' cannot be parsed. Returning current date/time");
			return GetDateTimeUTC();
		}

		return parsedDateTime;
	}

	/// <summary>
	/// Returns the number of seconds since the given time stamp, capped to a maximum time span if desired.
	/// capToMaxTime - maximum number of seconds to return (for example 24 * 60 * 60 for 24 hours)
	/// capToMaxTime = -1 will be ignored, returned value is uncapped
	/// </summary>
	public static double GetSecondsSinceTimeStamp(ref string timeStamp, int capToMaxTime = -1)
	{
		DateTime timeStampDate = ParseTimeStamp(ref timeStamp);
		TimeSpan deltaTime = GetDateTimeUTC().Subtract(timeStampDate);

		// If more time has passed than we're interested in, cap the value
		if (capToMaxTime > 0 && deltaTime.TotalSeconds >= capToMaxTime)
			return capToMaxTime;

		return deltaTime.TotalSeconds;
	}

	//////////////////////////////////////////////////////////////////////////
}
