// UnityAdsHelper.cs - Written for Unity Ads Asset Store v1.2.1 
//   first written by Nikkolai Davenport <nikkolai@unity3d.com>
//	 modified by Aylin Lee <aylin@unity3d.com>
//
// Setup Instructions:
// 1. Attach this script to a new game object.
// 2. Enter game IDs into the fields provided.
// 3. Enable Development Build in Build Settings to 
//     enable test mode and show SDK debug levels.
// 
// Usage Guide:
//  Write a script and call UnityAdsHelper.ShowAd() to show an ad. 
//  Customize the HandleShowResults method to perform actions based 
//  on whether an ad was succesfully shown or not.
//
// Notes:
//  - Game IDs by platform are required to initialize Unity Ads.
//  - Test game IDs are optional. If not set while in test mode, 
//     test game IDs will default to platform game IDs.
//  - The various debug levels and test mode are only used when
//     Development Build is enabled in Build Settings.
//  - Test mode can be disabled while Development Build is set
//     by checking the option to disable it in the inspector.



using UnityEngine;
using System.Collections;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class UnityAdsHelper : MonoBehaviour {
	
	[System.Serializable]
	public struct GameInfo
	{  
		[SerializeField]
		private string _gameID;
		[SerializeField]
		private string _testGameID;
		
		public string GetGameID ()
		{
			return Debug.isDebugBuild && !string.IsNullOrEmpty(_testGameID) ? _testGameID : _gameID;
		}
	}
	public GameInfo iOS;
	public GameInfo android;
	
	// Development Build must be enabled in Build Settings
	//  in order to use test mode and to show debug levels.
	public bool disableTestMode;
	public bool showInfoLogs=false;
	public bool showDebugLogs=false;
	public bool showWarningLogs = false;
	public bool showErrorLogs = false;
	
	protected void Awake() 
	{
		#if UNITY_IOS || UNITY_ANDROID
		string gameID = null;
		
		#if UNITY_IOS
		gameID = iOS.GetGameID();
		#elif UNITY_ANDROID
		gameID = android.GetGameID();
		#endif
		
		if (string.IsNullOrEmpty(gameID))
		{
			Debug.LogError("A valid game ID is required to initialize Unity Ads.");
		}
		else
		{
			Advertisement.debugLevel = Advertisement.DebugLevel.None;	
			if (showInfoLogs) Advertisement.debugLevel    |= Advertisement.DebugLevel.Info;
			if (showDebugLogs) Advertisement.debugLevel   |= Advertisement.DebugLevel.Debug;
			if (showWarningLogs) Advertisement.debugLevel |= Advertisement.DebugLevel.Warning;
			if (showErrorLogs) Advertisement.debugLevel   |= Advertisement.DebugLevel.Error;
			
			bool enableTestMode = Debug.isDebugBuild && !disableTestMode; 
			Debug.Log(string.Format("Initializing Unity Ads for game ID {0} with test mode {1}...",
			                        gameID, enableTestMode ? "enabled" : "disabled"));
			
			Advertisement.Initialize(gameID,false);			
		}
		#else
		Debug.LogWarning("Unity Ads is not supported on the current build platform.");
		#endif
	}
	
	public static bool ShowAd (string zone = null)  
	{
		#if UNITY_IOS || UNITY_ANDROID
		if (string.IsNullOrEmpty(zone)) zone = null;
		
		if (!Advertisement.IsReady(zone))
		{
			Debug.LogWarning(string.Format("Unable to show ad. The ad placement zone ($0) is not ready.",
			                               zone == null ? "default" : zone));
			return false;
		}
		
		ShowOptions options = new ShowOptions();
		options.resultCallback = HandleShowResult;
		
		Advertisement.Show(zone,options);
		
		return true;
		#else
		Debug.LogError("Failed to show ad. Unity Ads is not supported on the current build platform.");
		return false;
		#endif
	}
	
	#if UNITY_IOS || UNITY_ANDROID
	private static void HandleShowResult (ShowResult result)   
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}
	#endif
	public void ShowTestAds()
	{
		ShowAd ();
	}
	
	
}