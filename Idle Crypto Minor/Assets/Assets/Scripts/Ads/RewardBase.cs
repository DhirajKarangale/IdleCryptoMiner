using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class RewardBase : MonoBehaviour
{
    [SerializeField] internal Button adButton;

    private string rewardedId = "ca-app-pub-2251287037980958/9785155328";
    private RewardedAd rewardedAd;
    private int scriptId;


    private void Start()
    {
        scriptId = -1;
        Invoke(nameof(LoadRewarded), 3);
    }


    private void LoadRewarded()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                return;
            }

            rewardedAd = ad;
        });
    }

    private void Reward()
    {
        GetReward(scriptId);
        Invoke(nameof(LoadRewarded), 2);
    }

    public virtual void ShowAd(int id)
    {
        scriptId = -1;
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                scriptId = id;
                Reward();
            });
        }
        else
        {
            Message.instance.Show("Ad not available", Color.white);
            LoadRewarded();
        }
    }

    public virtual void GetReward(int id)
    {

    }

    public virtual void RewardCancled()
    {

    }
}