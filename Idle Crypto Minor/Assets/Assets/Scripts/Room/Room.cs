using UnityEngine;
using System;

public class Room : MonoBehaviour
{
    [SerializeField] internal RepareItem repareAlgo;
    [SerializeField] internal RepareItem repareGraphicCard;
    [SerializeField] internal RepareItem repareCoolingDevice;

    [SerializeField] internal RoomItem algorithm;
    [SerializeField] internal RoomItem graphicCard;
    [SerializeField] internal RoomItem coolingDevice;
    [SerializeField] internal RoomItem cable;

    [SerializeField] GameObject objUpgradeButton;
    [SerializeField] GameObject objMxTick;

    internal bool isMax;
    internal bool isRepareDataSet;

    private RoomManager roomManager;
    private CurrencyManager currencyManager;

    public static Action<Room> OnUpgradeButton;
    public static Action<Room> OnRepareButton;

    private void Start()
    {
        isRepareDataSet = false;
        roomManager = GameManager.instance.roomManager;
        currencyManager = GameManager.instance.currencyManager;
        CheckMaxRoom();
        UpdateRoomItemsData();
    }

    private void UpdateRoomItemsData()
    {
        graphicCard.UpdateData(this);
        algorithm.UpdateData(this);
        coolingDevice.UpdateData(this);
        cable.UpdateData(this);
    }

    private void InvokeRepareData()
    {
        double[] btcs = new double[6] { 0, 0, 0, 0, 0, 0 };
        repareAlgo.SetData(btcs);
        repareGraphicCard.SetData(btcs);
        repareCoolingDevice.SetData(btcs);
    }

    private void UpgradeItem(RoomItem item, int level)
    {
        if (level <= 1)
        {
            item.level = 1;
            item.UpdateData(this);
            return;
        }

        item.UpdateData(this);
        for (int i = 0; i < level - 1; i++)
        {
            item.Upgrade();
        }
    }

    private void SetRepareData(RoomData roomData)
    {
        if (isRepareDataSet) return;
        repareAlgo.SetData(roomData.repareAlgorithm);
        repareGraphicCard.SetData(roomData.repareGraphicCard);
        repareCoolingDevice.SetData(roomData.repareCoolingDevice);
        isRepareDataSet = true;
    }


    public void InitializeRepareData()
    {
        Invoke("InvokeRepareData", 1);
    }

    public void CheckMaxRoom()
    {
        isMax = graphicCard.isMax && algorithm.isMax && cable.isMax && coolingDevice.isMax;
        objUpgradeButton.SetActive(!isMax);
        objMxTick.SetActive(isMax);
        if (isMax) roomManager.CheckMaxRooms();
    }

    public void SetItemLevel(int graphicCardLevel, int algorithmLevel, int coolingDeviceLevel, int cableLevel)
    {
        UpgradeItem(graphicCard, graphicCardLevel);
        UpgradeItem(algorithm, algorithmLevel);
        UpgradeItem(coolingDevice, coolingDeviceLevel);
        UpgradeItem(cable, cableLevel);
    }

    public void SetData(RoomData roomData)
    {
        SetItemLevel(roomData.graphicCard, roomData.algorithm, roomData.coolingDevice, roomData.cable);
        SetRepareData(roomData);
    }


    public void UpgradeButton()
    {
        if (isMax)
        {
            Message.instance.Show("This Room is Maxxed", Color.white);
            return;
        }
        OnUpgradeButton?.Invoke(this);
    }

    public void RepareButton()
    {
        if (repareAlgo.isRepareMode || repareGraphicCard.isRepareMode || repareCoolingDevice.isRepareMode)
        {
            OnRepareButton?.Invoke(this);
        }
        else
        {
            UpgradeButton();
        }
    }
}
