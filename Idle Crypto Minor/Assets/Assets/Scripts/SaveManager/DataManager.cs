using System;
using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public NFTDB nftDB;
    public GiftsDB giftsDB;

    public long fiat;
    public long netWorth;
    public double btc;
    public double hashSpeed;
    public int mapLevel;
    public string closeTime;
    public float addRoomRemainTime;
    public List<RoomData> roomDatas;
    public bool[] nfts;
    public bool[] gifts;

    public static Action<DataManager> OnDataReceived;
    public static Action<DataManager> OnTimeReceived;

    private Map map;
    private NetWorth netWorthObj;
    private RoomManager roomManager;
    private CurrencyManager currencyManager;

    private void Start()
    {
        netWorthObj = GameManager.instance.netWorth;
        currencyManager = GameManager.instance.currencyManager;
        roomManager = GameManager.instance.roomManager;
        map = GameManager.instance.map;
        nfts = new bool[nftDB.itemCount];
        gifts = new bool[giftsDB.itemCount];
    }


    private void SetRoomData(DataItems dataItems)
    {
        if (dataItems == null)
        {
            RoomData roomData = new RoomData();
            roomData.graphicCard = 1;
            roomData.algorithm = 1;
            roomData.coolingDevice = 1;
            roomData.cable = 1;

            roomData.repareAlgorithm = new double[6] { 0, 0, 0, 0, 0, 0 };
            roomData.repareGraphicCard = new double[6] { 0, 0, 0, 0, 0, 0 };
            roomData.repareCoolingDevice = new double[6] { 0, 0, 0, 0, 0, 0 };

            roomDatas.Add(roomData);

            return;
        }

        if (dataItems.roomData[0].repareAlgorithm == null)
        {
            for (int i = 0; i < dataItems.roomData.Count; i++)
            {
                RoomData roomData = new RoomData();
                roomData.graphicCard = dataItems.roomData[i].graphicCard;
                roomData.algorithm = dataItems.roomData[i].algorithm;
                roomData.coolingDevice = dataItems.roomData[i].coolingDevice;
                roomData.cable = dataItems.roomData[i].cable;

                roomData.repareAlgorithm = new double[6] { 0, 0, 0, 0, 0, 0 };
                roomData.repareGraphicCard = new double[6] { 0, 0, 0, 0, 0, 0 };
                roomData.repareCoolingDevice = new double[6] { 0, 0, 0, 0, 0, 0 };

                roomDatas.Add(roomData);
            }

            return;
        }

        roomDatas = dataItems.roomData;
    }

    private void GetRoomsData()
    {
        roomDatas.Clear();
        roomDatas = new List<RoomData>();
        for (int i = 0; i < roomManager.roomCount; i++)
        {
            RoomData roomData = new RoomData();
            roomData.graphicCard = roomManager.GetGraphicCardLevel(i);
            roomData.algorithm = roomManager.GetAlgoLevel(i);
            roomData.coolingDevice = roomManager.GetCoolingDeviceLevel(i);
            roomData.cable = roomManager.GetCableLevel(i);

            roomData.repareAlgorithm = new double[6];
            roomData.repareGraphicCard = new double[6];
            roomData.repareCoolingDevice = new double[6];

            roomData.repareAlgorithm = roomManager.GetRepareAlgoData(i);
            roomData.repareGraphicCard = roomManager.GetRepareGCData(i);
            roomData.repareCoolingDevice = roomManager.GetRepareCDData(i);

            roomDatas.Add(roomData);
        }
    }


    private void GetNFTData()
    {
        for (int i = 0; i < nftDB.itemCount; i++)
        {
            nfts[i] = nftDB.nfts[i].isPurchased;
        }
    }

    private void SetNFTData()
    {
        for (int i = 0; i < nftDB.itemCount; i++)
        {
            nftDB.nfts[i].isPurchased = nfts[i];
        }
    }


    private void GetGiftsData()
    {
        if (gifts == null)
        {
            gifts = new bool[giftsDB.itemCount];
            return;
        }
        for (int i = 0; i < giftsDB.itemCount; i++)
        {
            gifts[i] = giftsDB.IsPurchased(i);
        }
    }

    private void SetGiftData()
    {
        if (gifts == null)
        {
            gifts = new bool[giftsDB.itemCount];
            return;
        }
        for (int i = 0; i < gifts.Length; i++)
        {
            giftsDB.PurchasedItem(i, gifts[i]);
        }
    }



    public void ResetRoomsData()
    {
        roomDatas.Clear();
        GetRoomsData();
    }

    public void SetData()
    {
        fiat = PlayerPrefs.GetInt("LoggedIn", 0) == 1 ? 2500 : 0;
        netWorth = 0;
        btc = 0;
        hashSpeed = 0;
        mapLevel = 1;
        // closeTime = DateTime.Now.ToString();
        // addRoomRemainTime = 0;
        SetRoomData(null);
        SetNFTData();
        SetGiftData();

        OnDataReceived?.Invoke(this);
    }

    public void SetData(DataItems dataItems)
    {
        fiat = dataItems.fiat;
        netWorth = dataItems.netWorth;
        btc = dataItems.btc;
        hashSpeed = dataItems.hashSpeed;
        mapLevel = dataItems.map;
        SetRoomData(dataItems);
        nfts = dataItems.nfts;
        gifts = dataItems.gifts;
        SetNFTData();
        SetGiftData();

        OnDataReceived?.Invoke(this);
    }

    public DataItems GetData()
    {
        fiat = currencyManager.fiat;
        netWorth = netWorthObj.netWorth;
        btc = currencyManager.btc;
        hashSpeed = currencyManager.hashSpeed;
        mapLevel = map.currIsland;
        // closeTime = DateTime.Now.ToString();
        // addRoomRemainTime = roomManager.currTime;

        GetRoomsData();
        GetNFTData();
        GetGiftsData();

        return new DataItems(fiat, btc, netWorth, hashSpeed, mapLevel, roomDatas, nfts, gifts);
    }




    public void SetTime(string closeTime, string addRoomRemainTime)
    {
        if (string.IsNullOrEmpty(closeTime)) closeTime = DateTime.Now.ToString();
        this.closeTime = closeTime;

        if (string.IsNullOrEmpty(addRoomRemainTime)) addRoomRemainTime = "0";
        this.addRoomRemainTime = float.Parse(addRoomRemainTime);
        if (this.addRoomRemainTime <= 0) this.addRoomRemainTime = 0;

        OnTimeReceived?.Invoke(this);
    }

    public void SetTime()
    {
        this.closeTime = PlayerPrefs.GetString("CloseTime", DateTime.Now.ToString());
        this.addRoomRemainTime = PlayerPrefs.GetFloat("AddRoomRemainTime", 0);

        OnTimeReceived?.Invoke(this);
    }
}
