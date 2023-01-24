using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] TMP_Text txtCount;

    private Upgrade upgrade;
    private RoomItem roomItem;
    private Leaderboard leaderboard;
    private CurrencyManager currencyManager;

    public static Action OnItemUpgrade;

    [Header("Purchasable")]
    [SerializeField] GameObject objPurchasable;
    [SerializeField] TMP_Text txtPurchasableFiat;

    [Header("Not Purchasable")]
    [SerializeField] GameObject objNotPurchasable;
    [SerializeField] TMP_Text txtNotPurchasableFiat;
    [SerializeField] TMP_Text txtNotPurchasableRemainFiat;
    [SerializeField] Slider slider;

    [Header("Completed")]
    [SerializeField] GameObject objCompleted;
    [SerializeField] GameObject objRepare;

    private SoundManager soundManager;

    private void OnEnable()
    {
        soundManager = SoundManager.instance;
        upgrade = GameManager.instance.upgrade;
        leaderboard = GameManager.instance.leaderboard;
        currencyManager = GameManager.instance.currencyManager;
    }

    private void ChangeScore()
    {
        switch (transform.name)
        {
            case "GraphicCard":
                leaderboard.ChangeScore(3);
                break;
            case "Algorithm":
                leaderboard.ChangeScore(4);
                break;
            case "CoolingDevice":
                leaderboard.ChangeScore(2);
                break;
            case "Cable":
                leaderboard.ChangeScore(1);
                break;
        }
    }

    private void UpdateStats()
    {
        OnItemUpgrade?.Invoke();
    }

    public void SetData(RoomItem roomItem)
    {
        this.roomItem = roomItem;

        if (roomItem.isRepareMode)
        {
            objPurchasable.SetActive(false);
            objNotPurchasable.SetActive(false);
            objCompleted.SetActive(false);
            objRepare.SetActive(true);
        }
        else if (roomItem.isMax)
        {
            objPurchasable.SetActive(false);
            objNotPurchasable.SetActive(false);
            objCompleted.SetActive(true);
            objRepare.SetActive(false);
        }
        else if (roomItem.purchaseCoast <= currencyManager.fiat)
        {
            txtPurchasableFiat.text = roomItem.purchaseCoast.ToString();

            objPurchasable.SetActive(true);
            objNotPurchasable.SetActive(false);
            objCompleted.SetActive(false);
            objRepare.SetActive(false);
        }
        else
        {
            txtNotPurchasableFiat.text = roomItem.purchaseCoast.ToString();
            txtNotPurchasableRemainFiat.text = $"{(int)roomItem.purchaseCoast - (int)currencyManager.fiat} more to Upgrade";
            slider.value = ((float)currencyManager.fiat / (float)roomItem.purchaseCoast);

            objPurchasable.SetActive(false);
            objNotPurchasable.SetActive(true);
            objCompleted.SetActive(false);
            objRepare.SetActive(false);
        }

        if (txtCount) txtCount.text = $"{roomItem.latestItem}/6";
        img.sprite = roomItem.sprites[roomItem.spriteNo];
    }

    public void UpgradeButton()
    {
        if (!roomItem) return;
        soundManager.PlaySound(soundManager.clipHardware);
        ChangeScore();
        currencyManager.ChangeFiat(-roomItem.purchaseCoast, upgrade.txtFiat.transform.position, txtPurchasableFiat.transform.position);
        roomItem.Upgrade();
        Invoke("UpdateStats", 0.1f);
    }
}
