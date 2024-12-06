using System;
using UnityEngine;
using GoogleMobileAds.Ump.Api;

public class ConsentController : MonoBehaviour
{
    public bool CanRequestAds => ConsentInformation.CanRequestAds();

    private void Start()
    {
        GatherConsent(OnConsentGathered);
    }

    public void GatherConsent(Action<string> onComplete)
    {
        var requestParameters = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
            ConsentDebugSettings = new ConsentDebugSettings { DebugGeography = DebugGeography.Disabled, }
        };

        ConsentInformation.Update(requestParameters, (FormError updateError) =>
        {
            if (updateError != null)
            {
                // Debug.LogError("Consent update error: " + updateError.Message);
                Message.instance.Show("Consent update error, " + updateError.Message, Color.red);
                onComplete?.Invoke(updateError.Message);
                return;
            }

            if (CanRequestAds)
            {
                onComplete?.Invoke(null);
                return;
            }

            ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
            {
                if (showError != null)
                {
                    // Debug.LogError("Consent form error: " + showError.Message);
                    Message.instance.Show("Consent form error, " + showError.Message, Color.red);
                    onComplete?.Invoke(showError.Message);
                }
                else
                {
                    onComplete?.Invoke(null);
                }
            });
        });
    }

    private void OnConsentGathered(string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            // Debug.LogError("Consent gathering failed with error: " + error);
        }
        else
        {
            // Message.instance.Show("Consent successfully gathered.", Color.green);
            // Debug.Log("Consent successfully gathered.");
        }
    }

    public void ResetConsentInformation()
    {
        ConsentInformation.Reset();
        // Debug.Log("Consent information reset.");
    }

    public void ShowPrivacyOptionsForm(Action<string> onComplete)
    {
        ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
        {
            if (showError != null)
            {
                // Form showing failed.
                if (onComplete != null)
                {
                    onComplete(showError.Message);
                }
            }
            // Form showing succeeded.
            else if (onComplete != null)
            {
                onComplete(null);
            }
        });
    }
}
