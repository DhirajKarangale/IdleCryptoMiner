using UnityEngine;
using GoogleMobileAds.Api;

public class Interstitial : MonoBehaviour
{
    private string interId = "ca-app-pub-2251287037980958/2098236996";
    private InterstitialAd interstitialAd;


    private void Start()
    {
        LoadAd(3);
    }

    private void LoadAd(float delay)
    {
        CancelInvoke();
        Invoke(nameof(LoadInterstitial), delay);
    }

    private void LoadInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        InterstitialAd.Load(interId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                LoadAd(1);
                return;
            }

            interstitialAd = ad;
        });
    }

    private void InterstitialShow()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            LoadAd(1);
        }
        else
        {
            Message.instance.Show("Ad not available", Color.white);
            LoadAd(0);
        }
    }

    public void ShowAd()
    {
        InterstitialShow();
    }
}
