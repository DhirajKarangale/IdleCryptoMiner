using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADS : MonoBehaviour
{
    string bannerAdUnitId = "5eff39d21dfc354d"; // Retrieve the ID from your account
        Color backgroundColor = new Color(0x3C / 255f, 0x50 / 255f, 0x6F / 255f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            
            // AppLovin SDK is initialized, start loading ads
            // Banners are automatically sized to 320�50 on phones and 728�90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional
            //MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
             MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, backgroundColor);
            MaxSdk.ShowBanner(bannerAdUnitId);
        };

        MaxSdk.SetSdkKey("PDb4ao8r3T2UIx1skVgJn4_nUGgFdmIiz2oylI8TkF94fImXfuPG0MwMlK4lDcvMTtoKH5td6nngXW94F_HgTn");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
    }
}
