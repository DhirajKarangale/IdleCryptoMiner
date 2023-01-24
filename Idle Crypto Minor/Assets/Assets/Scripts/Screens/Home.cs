using TMPro;
using UnityEngine;

public class Home : MonoBehaviour
{
    [SerializeField] TMP_Text txtFiat;
    [SerializeField] TMP_Text txtBTC;

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
        txtFiat.text = amount.ToCurrency();
    }

    private void UpdateBTC(double amount)
    {
        txtBTC.text = amount.ToString("F9");
    }
}
