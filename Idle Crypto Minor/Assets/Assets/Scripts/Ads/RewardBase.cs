using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardBase : MonoBehaviour
{
    [SerializeField] internal Button adButton;

    private int scriptId;
    private const string adUnitId = "5163de406d51626b";
    private int retryAttempt;


    private void Awake()
    {
        this.scriptId = -1;
        InitializeAds();
        // SuscribingEvents();
    }

    private void OnEnable()
    {
        SuscribingEvents();
    }

    private void OnDisable()
    {
        UnSuscribeEvents();
    }

    private void OnDestroy()
    {
        UnSuscribeEvents();
    }


    private void InitializeAds()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
       {
           // AppLovin SDK is initialized, start loading ads
       };

        MaxSdk.SetSdkKey("PDb4ao8r3T2UIx1skVgJn4_nUGgFdmIiz2oylI8TkF94fImXfuPG0MwMlK4lDcvMTtoKH5td6nngXW94F_HgTn");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();

        // MaxSdk.ShowRewardedAd(adUnitId, "MY_REWARDED_PLACEMENT");
    }

    private void SuscribingEvents()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void UnSuscribeEvents()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedRewardEvent;
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(adUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        GetReward(scriptId);
        scriptId = -1;
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }





    public virtual void ShowAd(int id)
    {
        this.scriptId = -1;
        if (MaxSdk.IsRewardedAdReady(adUnitId))
        {
            this.scriptId = id;
            MaxSdk.ShowRewardedAd(adUnitId);
            retryAttempt = 0;
        }
        else
        {
            Message.instance.Show("Ads are not ready try again", Color.red);
        }
    }

    public virtual void GetReward(int scriptId)
    {
        Debug.Log("Reward Received in Ad");
    }

    public virtual void RewardCancled()
    {

    }
}