using UnityEngine;
using GoogleMobileAds.Api;

public class Interstitial : MonoBehaviour
{
    private string interId = "ca-app-pub-2251287037980958/2098236996";
    private InterstitialAd interstitialAd;


    private void Start()
    {
        LoadInterstitial();
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
            if (error != null || ad == null) return;

            interstitialAd = ad;
        });
    }

    private void InterstitialShow()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            Invoke(nameof(LoadInterstitial), 2);
        }
        else
        {
            LoadInterstitial();
        }
    }

    public void ShowAd()
    {
        InterstitialShow();
        Debug.Log("Show ads");
    }
}
