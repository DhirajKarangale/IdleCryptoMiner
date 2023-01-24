using TMPro;
using UnityEngine;

public class RepareUI : MonoBehaviour
{
    [SerializeField] GameObject objFiat;
    [SerializeField] GameObject objRepareAnim;
    [SerializeField] GameObject objPurchasable;
    [SerializeField] GameObject objNotPurchasable;
    [SerializeField] GameObject objComplete;
    [SerializeField] TMP_Text txtCost;
    [SerializeField] TMP_Text txtRemainFiat;
    [SerializeField] UnityEngine.UI.Slider slider;

    private long repareCost;
    private RepareItem repareItem;
    private RepareManager repareManager;

    public void SetData(RepareItem repareItem)
    {
        if (!repareManager) repareManager = GameManager.instance.repareManager;

        this.repareItem = repareItem;
        if (!repareItem.isRepareMode)
        {
            objFiat.SetActive(false);
            objRepareAnim.SetActive(false);
            objPurchasable.SetActive(false);
            objNotPurchasable.SetActive(false);
            objComplete.SetActive(true);
            return;
        }

        repareCost = repareItem.repareCost;
        objComplete.SetActive(false);
        objFiat.SetActive(true);
        objRepareAnim.SetActive(true);
        txtCost.text = repareCost.ToString();

        if (repareManager.currencyManager.fiat < repareCost)
        {
            objNotPurchasable.SetActive(true);
            objPurchasable.SetActive(false);
            txtRemainFiat.text = $"{(int)(repareCost - repareManager.currencyManager.fiat)} more to Repare";
            slider.value = ((float)repareManager.currencyManager.fiat / (float)repareCost);
        }
        else
        {
            objNotPurchasable.SetActive(false);
            objPurchasable.SetActive(true);
        }
    }

    public void ButtonRepare()
    {
        repareManager.soundManager.PlaySound(repareManager.soundManager.clipHardware);
        repareManager.currencyManager.ChangeFiat(-repareCost, repareManager.txtFiat.transform.position, txtCost.transform.position);
        repareItem.Repare();
        repareManager.Repared();
    }
}
