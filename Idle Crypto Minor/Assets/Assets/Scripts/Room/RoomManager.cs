using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class RoomManager : RewardBase
{
    [SerializeField] Color colorDesable;
    [Header("UI")]
    [SerializeField] TMP_Text txtCost;
    [SerializeField] TMP_Text txtTimer;
    [SerializeField] TMP_Text txtNewRoom;
    [SerializeField] GameObject objLock;
    [SerializeField] Image imgCostBG;
    [SerializeField] Image imgCoin;
    [SerializeField] Button buttonAddRoom;
    [SerializeField] Slider sliderTimer;

    [Header("Objects")]
    [SerializeField] GameObject locked;
    [SerializeField] GameObject unlocked;
    [SerializeField] Room roomPrefab;
    [SerializeField] Transform roomContent;

    private int cost;
    private byte index;
    private float startTime;
    private bool isMax;
    [HideInInspector] public float currTime;
    internal List<Room> rooms;

    private Map map;
    private Leaderboard leaderboard;
    private SoundManager soundManager;
    private CurrencyManager currencyManager;
    private DataManager dataManager;
    private SaveManager saveManager;

    private DateTime timeLast;

    public int[] itemCount;
    private double[] repareData;

    private void Awake() {
         rooms = new List<Room>();
    }
    public void Start()
    {
        repareData = new double[6];
        currTime = 0;
        index = 0;
        //rooms = new List<Room>();
        itemCount = new int[4];
    }

    private void OnEnable()
    {
        map = GameManager.instance.map;
        soundManager = SoundManager.instance;
        leaderboard = GameManager.instance.leaderboard;
        dataManager = GameManager.instance.dataManager;
        saveManager = GameManager.instance.saveManager;
        currencyManager = GameManager.instance.currencyManager;

        currencyManager.OnFiatChanged += CheckRoomCost;
        DataManager.OnDataReceived += DataReceived;
        DataManager.OnTimeReceived += OnTimeReceived;
    }

    private void OnDestroy()
    {
        currencyManager.OnFiatChanged -= CheckRoomCost;
        DataManager.OnDataReceived -= DataReceived;
        DataManager.OnTimeReceived -= OnTimeReceived;
    }

    private void OnDisable()
    {
        currencyManager.OnFiatChanged -= CheckRoomCost;
        DataManager.OnDataReceived -= DataReceived;
        DataManager.OnTimeReceived -= OnTimeReceived;
    }

    private IEnumerator IECheckTime()
    {
        while (true)
        {
            currTime -= 1;
            TimeSpan remainTime = TimeSpan.FromSeconds(currTime);
            string currTimeStr = "";
            if (remainTime.Hours > 0) currTimeStr += remainTime.Hours + "H : ";
            if (remainTime.Minutes > 0) currTimeStr += remainTime.Minutes + "M : ";
            if (remainTime.Seconds > 0) currTimeStr += remainTime.Seconds + "S";
            txtTimer.text = currTimeStr;
            sliderTimer.value = (float)(currTime / startTime);

            if (currTime <= 0)
            {
                UnlockRoom();
                yield break;
            }

            yield return new WaitForSecondsRealtime(1);
        }
    }

    private void DataReceived(DataManager dataManager)
    {
        foreach (RoomData roomData in dataManager.roomDatas)
        {
           
            Room newRoom = Instantiate(roomPrefab, roomContent);
            newRoom.transform.SetSiblingIndex(index++);
            rooms.Add(newRoom);
            newRoom.SetData(roomData);

            cost = roomCount * 1500;
            txtCost.text = cost.ToString();
            CheckRoomCost(currencyManager.fiat);
        }

        UpdateRoomTimer();
         //Debug.Log("Data Received"+rooms.Count);
    }

    private void OnTimeReceived(DataManager dataManager)
    {
        currTime = dataManager.addRoomRemainTime;

        if (!DateTime.TryParse(dataManager.closeTime, out timeLast))
        {
            if (!DateTime.TryParse(PlayerPrefs.GetString("CloseTime", System.DateTime.Now.ToString()), out timeLast))
            {
                timeLast = DateTime.Now;
            }
        }

        UpdateRoomTimer();
        CheckLastTime();
    }

    private void CheckLastTime()
    {
        if (currTime > 0)
        {
            buttonAddRoom.interactable = true;
            DateTime timeCurr = DateTime.Now;

            currTime -= (float)((timeCurr - timeLast).TotalSeconds);

            LockRoom();
            StartCoroutine(IECheckTime());
        }
        else
        {
            UnlockRoom();
        }
    }

    private void UpdateRoomTimer()
    {
        startTime = roomCount * 1.5f * 3600;
        // PlayerPrefs.SetFloat("StartTime", startTime);
    }

    private void SetRoomTimer()
    {
        LockRoom();
        UpdateRoomTimer();
        currTime = startTime;
        StartCoroutine(IECheckTime());
    }

    private void UnlockRoom()
    {
        objLock.SetActive(false);
        unlocked.SetActive(true);
        locked.SetActive(false);
        CheckRoomCost(currencyManager.fiat);
    }

    private void LockRoom()
    {
        objLock.SetActive(false);
        unlocked.SetActive(false);
        locked.SetActive(true);
    }

    private void CheckRoomCost(long fiat)
    {
        objLock.SetActive(false);
        if (roomCount >= 6) MaxRoomButton();
        else if (fiat >= cost || currTime > 0) ActiveButton();
        else DeactiveButton();
    }

    private void ActiveButton()
    {
        objLock.SetActive(false);
        buttonAddRoom.interactable = true;
        imgCostBG.color = Color.white;
        imgCoin.color = Color.white;
        txtNewRoom.text = "Add New Room";
    }

    private void DeactiveButton()
    {
        imgCostBG.color = colorDesable;
        imgCoin.color = colorDesable;
        txtNewRoom.text = "Not Enough Money";
        objLock.SetActive(true);
        buttonAddRoom.interactable = currTime > 0;
    }

    private void MaxRoomButton()
    {
        UpdateRoomTimer();
        currTime = startTime;
        imgCostBG.color = Color.white;
        imgCoin.color = Color.white;
        txtCost.text = "Max Rooms";
        txtNewRoom.text = "Max Rooms";
        buttonAddRoom.interactable = false;
        unlocked.SetActive(true);
        locked.SetActive(false);
    }

    private void AddRoom()
    {
        Room newRoom = Instantiate(roomPrefab, roomContent);
        // newRoom.transform.SetSiblingIndex(index++);
        newRoom.transform.SetSiblingIndex(roomCount - 1);
        newRoom.InitializeRepareData();
        rooms.Add(newRoom);

        leaderboard.ChangeScore(8);
        leaderboard.Send();

        currencyManager.ChangeFiat(-cost, Vector3.zero, txtCost.transform.position);
        cost = roomCount * 5000;
        txtCost.text = cost.ToString();

        soundManager.PlaySound(soundManager.clipRoom);
        if (roomCount >= 6) MaxRoomButton();
        else SetRoomTimer();
        CheckRoomCost(currencyManager.fiat);
    }

    private void ReduceTimer()
    {
        ShowAd(this.GetInstanceID());
    }

    public override void GetReward(int scriptId)
    {
        if (scriptId == this.GetInstanceID())
        {
            currTime -= 1800; // 30 min
        }
    }

    public int roomCount
    {
        //get { return rooms.Count; }
         get
    {
        if (rooms == null)
        {
            rooms = new List<Room>();
        }
        return rooms.Count;
    }
    }

    public double[] GetRepareAlgoData(int index)
    {
        return rooms[index].repareAlgo.GetBTC();
    }

    public double[] GetRepareGCData(int index)
    {
        return rooms[index].repareGraphicCard.GetBTC();
    }

    public double[] GetRepareCDData(int index)
    {
        return rooms[index].repareCoolingDevice.GetBTC();
    }

    public int GetGraphicCardLevel(int index)
    {
        return rooms[index].graphicCard.level;
    }

    public int GetAlgoLevel(int index)
    {
        return rooms[index].algorithm.level;
    }

    public int GetCableLevel(int index)
    {
        return rooms[index].cable.level;
    }

    public int GetCoolingDeviceLevel(int index)
    {
        return rooms[index].coolingDevice.level;
    }

    public void CheckMaxRooms()
    {
        if (roomCount < 6) return;

        foreach (Room room in rooms)
        {
            isMax = room.isMax;
            if (!isMax) return;
        }

        if (isMax)
        {
            leaderboard.ChangeScore(16);
            leaderboard.Send();
            map.IslandComplete();
        }
    }

    public void ResetRooms()
    {
        foreach (Room room in rooms)
        {
            Destroy(room.gameObject);
        }
        currencyManager.ResetCurrency();
        rooms.Clear();

        Room newRoom = Instantiate(roomPrefab, roomContent);
        newRoom.transform.SetSiblingIndex(index++);
        newRoom.InitializeRepareData();
        rooms.Add(newRoom);
        cost = roomCount * 1500;
        txtCost.text = cost.ToString();
        CheckRoomCost(currencyManager.fiat);

        dataManager.ResetRoomsData();
        saveManager.Save();
    }

    public void RoomButton()
    {
        if (locked.activeInHierarchy) ReduceTimer();
        else AddRoom();
    }
}
