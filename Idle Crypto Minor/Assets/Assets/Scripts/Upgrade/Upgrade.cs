using UnityEngine;

public class Upgrade : ScreenBase
{
    [SerializeField] internal TMPro.TMP_Text txtFiat;
    [SerializeField] UpgradeItem graphicCard;
    [SerializeField] UpgradeItem algorithm;
    [SerializeField] UpgradeItem coolingDevice;
    [SerializeField] UpgradeItem cable;

    private Room room;
    private Leaderboard leaderboard;
    private CurrencyManager currencyManager;

    private void OnEnable()
    {
        currencyManager = GameManager.instance.currencyManager;
        leaderboard = GameManager.instance.leaderboard;

        Room.OnUpgradeButton += OnUpgradeButton;
        UpgradeItem.OnItemUpgrade += UpdateItems;
        currencyManager.OnFiatChanged += UpdateFiat;
    }

    private void OnDestroy()
    {
        Room.OnUpgradeButton -= OnUpgradeButton;
        UpgradeItem.OnItemUpgrade -= UpdateItems;
        currencyManager.OnFiatChanged -= UpdateFiat;
    }

    private void OnDisable()
    {
        Room.OnUpgradeButton -= OnUpgradeButton;
        UpgradeItem.OnItemUpgrade -= UpdateItems;
        currencyManager.OnFiatChanged -= UpdateFiat;
    }

    private void UpdateFiat(long amount)
    {
        txtFiat.text = amount.ToCurrency();
    }

    private void OnUpgradeButton(Room room)
    {
        ActiveButton(true);
        this.room = room;
        UpdateItems();
    }

    private void UpdateItems()
    {
        graphicCard.SetData(room.graphicCard);
        algorithm.SetData(room.algorithm);
        coolingDevice.SetData(room.coolingDevice);
        cable.SetData(room.cable);
    }


    public override void ActiveButton(bool isActive)
    {
        if (room)
        {
            room.CheckMaxRoom();
            leaderboard.Send();
        }
        base.ActiveButton(isActive);
        room = null;
    }
}
