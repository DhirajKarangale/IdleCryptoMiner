using UnityEngine;
using GoogleMobileAds.Api;

public class ADS : MonoBehaviour
{
    private string bannerId = "ca-app-pub-2251287037980958/9234437083";
    private BannerView bannerView;
    private bool isBanner;

    private static bool? _isInitialized;
    [SerializeField] private ConsentController _consentController;

    private void Start()
    {
        isBanner = false;

        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.SetiOSAppPauseOnBackground(true);
        if (_consentController.CanRequestAds) InitAds();
        InitConsent();
    }

    private void InitAds()
    {
        if (_isInitialized.HasValue) return;

        _isInitialized = false;

        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            if (initStatus == null)
            {
                _isInitialized = null;
                return;
            }

            _isInitialized = true;
            LoadBanner(); 
        });
    }

    private void InitConsent()
    {
        _consentController.GatherConsent((string error) =>
        {
            if (!string.IsNullOrEmpty(error)) Debug.LogError("Consent error: " + error);
            else InitAds();
        });
    }

    #region Banner

    private void LoadBanner()
    {
        CreateBannerView();
        if (bannerView == null) CreateBannerView();

        var adRequest = new AdRequest
        {
            Keywords = { "unity-admob-sample" }
        };

        bannerView.LoadAd(adRequest);
        // HideBanner();
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

    #endregion

    public void OpenAdInspector()
    {
        MobileAds.OpenAdInspector((AdInspectorError error) =>
        {
            if (error != null)
            {
                Debug.Log("Ad Inspector failed to open with error: " + error);
                return;
            }

            Debug.Log("Ad Inspector opened successfully.");
        });
    }

    public void ButtonConsent()
    {
        _consentController.ShowPrivacyOptionsForm((string error) =>
            {
                if (error != null)
                {
                    Debug.Log("Failed to show consent privacy form with error: " + error);
                    Message.instance.Show("Failed to show consent privacy form, " + error, Color.white);
                }
                else
                {
                    // Message.instance.Show("Privacy form opened successfully.", Color.white);
                    Debug.Log("Privacy form opened successfully.");
                }
            });
    }
}