using UnityEngine;
using TMPro;

public class RepareManager : ScreenBase
{
    [SerializeField] TMP_Text txtRepareCount;
    [SerializeField] internal TMP_Text txtFiat;

    [SerializeField] RepareUI uiAlgo;
    [SerializeField] RepareUI uiGraphicCard;
    [SerializeField] RepareUI uiCoolingDevice;

    internal SoundManager soundManager;
    internal CurrencyManager currencyManager;
    private Room room;

    private void Start()
    {
        soundManager = SoundManager.Instance;
        currencyManager = GameManager.instance.currencyManager;

        Room.OnRepareButton += OnRepareButton;
        currencyManager.OnFiatChanged += OnFiatChange;
    }

    private void OnDestroy()
    {
        Room.OnRepareButton -= OnRepareButton;
    }

    private void OnDisable()
    {
        Room.OnRepareButton -= OnRepareButton;
    }

    private void OnFiatChange(long amount)
    {
        txtFiat.text = currencyManager.fiat.ToCurrency();
        if (room)
        {
            txtRepareCount.text = $"{room.repareAlgo.itemToRepare + room.repareGraphicCard.itemToRepare + room.repareCoolingDevice.itemToRepare} Items for Repare";
            txtFiat.text = currencyManager.fiat.ToCurrency();
            uiAlgo.SetData(room.repareAlgo);
            uiGraphicCard.SetData(room.repareGraphicCard);
            uiCoolingDevice.SetData(room.repareCoolingDevice);
        }
    }

    private void OnRepareButton(Room room)
    {
        this.room = room;
        txtRepareCount.text = $"{room.repareAlgo.itemToRepare + room.repareGraphicCard.itemToRepare + room.repareCoolingDevice.itemToRepare} Items for Repare";
        txtFiat.text = currencyManager.fiat.ToCurrency();
        uiAlgo.SetData(room.repareAlgo);
        uiGraphicCard.SetData(room.repareGraphicCard);
        uiCoolingDevice.SetData(room.repareCoolingDevice);
        ActiveButton(true);
    }

    public void Repared()
    {
        if (!room) return;
        txtRepareCount.text = $"{room.repareAlgo.itemToRepare + room.repareGraphicCard.itemToRepare + room.repareCoolingDevice.itemToRepare} Items for Repare";
        OnRepareButton(room);
    }
}
