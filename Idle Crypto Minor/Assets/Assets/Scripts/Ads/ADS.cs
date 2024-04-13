using UnityEngine;
using GoogleMobileAds.Api;

public class ADS : MonoBehaviour
{
    // app id ca-app-pub-2251287037980958~8712634852

    private string bannerId = "ca-app-pub-2251287037980958/9234437083";
    private BannerView bannerView;
    private bool isBanner;


    private void Start()
    {
        isBanner = false;

        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus => { });
        // LoadBanner();
    }

    private void LoadBanner()
    {
        CreateBannerView();
        if (bannerView == null) CreateBannerView();

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        bannerView.LoadAd(adRequest);
        HideBanner();
    }

    private void CreateBannerView()
    {
        if (bannerView != null) DestroyBanner();
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }

    private void DestroyBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
            isBanner = false;
        }
    }

    internal void HideBanner()
    {
        if (!isBanner) return;
        isBanner = false;
        bannerView.Hide();
    }

    internal void ShowBanner()
    {
        if (isBanner) return;
        isBanner = true;
        bannerView.Show();
    }
}