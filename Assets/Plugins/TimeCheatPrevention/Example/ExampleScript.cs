using UnityEngine;
using UnityEngine.UI;
using System;

public class ExampleScript : MonoBehaviour
{
	// Time labels
	public Text m_DeviceTimeLabel = null;
	public Text m_EstimatedTimeLabel = null;
	public GameObject m_UnsupportedPlatform = null;
	public GameObject m_TimeSource = null;

	// Check boxes
	public Toggle m_UsingDeviceTime = null;
	public Toggle m_UsingEstimatedTime = null;
	public Toggle m_UsingServerTime = null;

	// TimeStamp
	public Button m_SaveTimeStampButton = null;
	public Button m_LoadTimeStampButton = null;
	public Text m_TimeSinceTimeStampLabel = null;

	// Helper
	private float m_UpdateTimer = -1.0f;

	//////////////////////////////////////////////////////////////////////////

	private void Start()
	{
		InitTimeStampLogic();

		// Set whether this platform supports offline estimation or not (Internet time is always supported, but requires Internet)
		bool bSupported = TimeCheatPrevention.IsPlatformTimeEstimationSupported();
		m_UnsupportedPlatform.SetActive(!bSupported);
		m_TimeSource.SetActive(bSupported);

	}

	//////////////////////////////////////////////////////////////////////////

	private void OnApplicationFocus(bool focus)
	{
		// If we're coming back from a suspended state, update and get a new, cheat-free time
		if (focus)
			InitTimeStampLogic();
	}

	//////////////////////////////////////////////////////////////////////////

	private void InitTimeStampLogic()
	{
		// Init label
		m_TimeSinceTimeStampLabel.text = "";

		// Disable buttons - don't allow time stamp logic until we have a reliable time
		m_SaveTimeStampButton.interactable = false;
		m_LoadTimeStampButton.interactable = false;

		// Register for callback
		TimeCheatPrevention.OnCheatFreeTimeAvailable.AddListener(OnCheatFreeTimeAvailable);
	}

	//////////////////////////////////////////////////////////////////////////

	void OnCheatFreeTimeAvailable()
	{
		// Unregister from callback
		TimeCheatPrevention.OnCheatFreeTimeAvailable.RemoveListener(OnCheatFreeTimeAvailable);

		// Make buttons interactable
		m_SaveTimeStampButton.interactable = true;
		m_LoadTimeStampButton.interactable = PlayerPrefs.HasKey("TCP_DEMO_TIMESTAMP");
	}

	//////////////////////////////////////////////////////////////////////////

	private void Update()
	{
		// Don't update every frame, that's a waste of resources for a timer that
		// only changes once per second.
		m_UpdateTimer -= Time.deltaTime;
		if (m_UpdateTimer > 0.0f)
			return;
		m_UpdateTimer = 0.5f;

		// Show timer labels
		m_DeviceTimeLabel.text = DateTime.UtcNow.ToShortDateString() + "\n" + DateTime.UtcNow.ToLongTimeString();
		m_EstimatedTimeLabel.text = TimeCheatPrevention.GetDateTimeUTC().ToShortDateString() + "\n" + TimeCheatPrevention.GetDateTimeUTC().ToLongTimeString();

		// Set check box toggles
		m_UsingDeviceTime.isOn = TimeCheatPrevention.IsUsingLocalDeviceTime();
		m_UsingEstimatedTime.isOn = TimeCheatPrevention.IsUsingEstimatedTime();
		m_UsingServerTime.isOn = TimeCheatPrevention.HasServerTime();
	}

	//////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Button Callback from the scene
	/// </summary>
	public void OnSaveTimeStamp_Pressed()
	{
		PlayerPrefs.SetString("TCP_DEMO_TIMESTAMP", TimeCheatPrevention.GetTimeStamp());

		// Delete the content of the time passed-label, since it is now invalid
		m_TimeSinceTimeStampLabel.text = "";

		// Since we have a time stamp now, make this button interactable
		m_LoadTimeStampButton.interactable = true;
	}

	/// <summary>
	/// Button Callback from the scene
	/// </summary>
	public void OnLoadTimeStamp_Pressed()
	{
		// Nothing to do if there is no time stamp
		if (!PlayerPrefs.HasKey("TCP_DEMO_TIMESTAMP"))
		{
			Debug.LogWarning("No saved time stamp found.");

			m_LoadTimeStampButton.interactable = false;
			return;
		}

		// We have a saved timestamp, but is it an actual valid string?
		string timeStamp = PlayerPrefs.GetString("TCP_DEMO_TIMESTAMP", "");
		if (string.IsNullOrEmpty(timeStamp))
		{
			Debug.LogWarning("Saved time stamp invalid.");

			m_LoadTimeStampButton.interactable = false;
			PlayerPrefs.DeleteKey("TCP_DEMO_TIMESTAMP");
			return;
		}

		double secondsPassed = TimeCheatPrevention.GetSecondsSinceTimeStamp(ref timeStamp);

		m_TimeSinceTimeStampLabel.text = "Last Time Stamp:\n" + timeStamp + "\n\n" + secondsPassed.ToString("N0") + " seconds have passed";
	}

	//////////////////////////////////////////////////////////////////////////

}
