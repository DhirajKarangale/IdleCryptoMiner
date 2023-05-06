using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class Wallet : ScreenBase
{
    [SerializeField] NetWorth netWorth;

    [Header("Text")]
    [SerializeField] TMP_Text txtFiat;
    [SerializeField] TMP_Text txtBTC;
    [SerializeField] TMP_Text txtOpenDate;
    [SerializeField] TMP_Text txtDaysLeft;
    [SerializeField] TMP_Text txtVideosLeft;

    [Header("AdButton")]
    [SerializeField] Sprite normal;
    [SerializeField] Sprite watched;
    [SerializeField] WalletAdBtn[] adButtons;

    private int daysLeft;
    private int adsRemain;

    private CurrencyManager currencyManager;

    private const string saveCloseTime = "SAVECLOSETIME";


    private void OnEnable()
    {
        currencyManager = GameManager.instance.currencyManager;
        currencyManager.OnFiatChanged += UpdateFiat;
        currencyManager.OnBTCChanged += UpdateBTC;
    }

    private void OnDestroy()
    {
        currencyManager.OnFiatChanged -= UpdateFiat;
        currencyManager.OnBTCChanged -= UpdateBTC;
    }

    private void OnDisable()
    {
        currencyManager.OnFiatChanged -= UpdateFiat;
        currencyManager.OnBTCChanged -= UpdateBTC;
    }



    private void UpdateFiat(long amount)
    {
        txtFiat.text = amount.ToCurrency();
    }

    private void UpdateBTC(double amount)
    {
        txtBTC.text = amount.ToString("F9");
    }




    private void GetData()
    {
        if (PlayerPrefs.HasKey(saveCloseTime))
        {
            adsRemain = PlayerPrefs.GetInt("AdsRemain", 5);
            daysLeft = PlayerPrefs.GetInt("DaysLeft", 7);

            DateTime currTime = DateTime.Now;
            DateTime rewardCollectedTime = DateTime.Parse(PlayerPrefs.GetString(saveCloseTime, currTime.ToString()));
            double timeDiffernce = (currTime - rewardCollectedTime).TotalSeconds;
            int days = (int)(timeDiffernce / 86400);
            if (days > 0)
            {
                PlayerPrefs.SetString(saveCloseTime, DateTime.Now.ToString());
                adsRemain = 5;
                daysLeft = (Mathf.Clamp(daysLeft - days, 0, 7));
            }
        }
        else
        {
            PlayerPrefs.SetString(saveCloseTime, DateTime.Now.ToString());
            daysLeft = 7;
            adsRemain = adButtons.Length;
        }
        
        UpdateUI();
    }

    private void SetData()
    {
        PlayerPrefs.SetInt("DaysLeft", daysLeft);
        PlayerPrefs.SetInt("AdsRemain", adsRemain);
    }


    private void UpdateUI()
    {
        txtOpenDate.text = DateTime.Now.AddDays(daysLeft).ToString("dd MMMM yyyy");
        txtDaysLeft.text = $"{daysLeft} days remaing for unlock";
        txtVideosLeft.text = $"Watch {adsRemain} videos to reduce 1 unlock day";

        for (int i = 0; i < adButtons.Length; i++)
        {
            if (i < 5 - adsRemain) adButtons[i].Watched(watched, "Done");
            else adButtons[i].UnWatched(normal, $"Video{i + 1}");
        }

        if (daysLeft <= 0) Complete();
    }

    private void Complete()
    {
        daysLeft = 7;
        adsRemain = adButtons.Length;
        UpdateUI();
        netWorth.ActiveButton(true);
    }


    internal void Watched()
    {
        adsRemain--;
        if (adsRemain <= 0)
        {
            daysLeft--;
            adsRemain = adButtons.Length;
        }
        UpdateUI();
        SetData();
    }

    public override void ActiveButton(bool isActive)
    {
        base.ActiveButton(isActive);
        GetData();
    }
}
