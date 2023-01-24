using UnityEngine;
using TMPro;

public class ConvertBTC : ScreenBase
{
    [SerializeField] TMP_Text txtFiat;
    [SerializeField] TMP_Text txtBTC;
    [SerializeField] TMP_Text txtConvertedFiat;
    [SerializeField] Transform convertButton;
    private CurrencyManager currencyManager;


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
        txtFiat.text = currencyManager.fiat.ToCurrency();
    }

    private void UpdateBTC(double amount)
    {
        txtBTC.text = currencyManager.btc.ToString("F9");
        txtConvertedFiat.text = "= " + GetFiat().ToString("F0");
    }

    private long GetFiat()
    {
        return (long)(currencyManager.btc * 20000);
    }

    public void ConvertButton()
    {
        currencyManager.ChangeFiat(GetFiat(), convertButton.position, Vector3.zero);
        currencyManager.ChangeBTC(-currencyManager.btc);
    }
}
