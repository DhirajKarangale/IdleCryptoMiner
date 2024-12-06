using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class RewardBase : MonoBehaviour
{
    [SerializeField] internal Button adButton;

    private string rewardedId = "ca-app-pub-2251287037980958/9785155328";
    private RewardedAd rewardedAd;
    private int scriptId;


    private void Awake()
    {
        scriptId = -1;
        LoadAd(3);
    }

    private void LoadAd(float delay)
    {
        CancelInvoke();
        Invoke(nameof(LoadRewarded), delay);
    }


    private void LoadRewarded()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();
        // adRequest.Keywords.Add("unity-admob-sample");

        RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                LoadAd(1);
                return;
            }

            rewardedAd = ad;
        });
    }

    public virtual void ShowAd(int id)
    {
        scriptId = -1;
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                scriptId = id;
                GetReward(scriptId);
                LoadAd(1);
            });
        }
        else
        {
            LoadAd(0);
            Message.instance.Show("Ad not available", Color.white);
        }
    }

    private void RegisterReloadHandler(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () => { LoadAd(0); };
        ad.OnAdFullScreenContentFailed += (AdError error) => { LoadAd(0); };
    }

    public virtual void GetReward(int id) { }

    public virtual void RewardCancled() { }
}