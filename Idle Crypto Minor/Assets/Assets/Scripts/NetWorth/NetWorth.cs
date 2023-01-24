using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetWorth : RewardBase
{
    [SerializeField] GameObject obj;
    [SerializeField] GiftsDB giftsDB;
    [SerializeField] NFTDB nftDB;

    [Header("Home")]
    [SerializeField] Slider sliderHome;
    [SerializeField] Image imgHomeReward;
    [SerializeField] TMP_Text txtHomeNetWorth;
    [SerializeField] TMP_Text txtHomeNextReward;

    [Header("Unlocked")]
    [SerializeField] internal GameObject objUnlock;
    [SerializeField] TMP_Text txtUnlockName;
    [SerializeField] TMP_Text txtUnlockNetWorth;
    [SerializeField] Image imgUnlockReward;

    [Header("Items")]
    [SerializeField] TMP_Text txtGet;
    [SerializeField] ScrollRect scrollRect;

    [Header("Content")]
    [SerializeField] RectTransform hardware;
    [SerializeField] RectTransform nft;
    [SerializeField] RectTransform collection;

    [Header("Content Items")]
    [SerializeField] NetWorthItem[] hardwareItem;
    [SerializeField] NetWorthItem[] nftItem;
    [SerializeField] NetWorthItem[] collectionItem;

    [Header("NetWorth")]
    [SerializeField] Slider sliderNet;
    [SerializeField] Image imgNetReward;
    [SerializeField] TMP_Text txtNetNetWorth;
    [SerializeField] TMP_Text txtNetNextReward;
    [SerializeField] TMP_Text txtMetaVerse;
    [SerializeField] TMP_Text txtNFTCount;
    [SerializeField] TMP_Text txtGiftCount;
    [SerializeField] TMP_Text txtHardwareCount;

    internal long netWorth;
    private int nextGift;

    private Map map;
    private NFTManager nftManager;
    private Upgrade upgrade;
    private RoomManager roomManager;
    private GameManager gameManager;
    private SoundManager soundManager;
    private CurrencyManager currencyManager;

    public bool isExtraActive
    {
        get { return objUnlock.activeInHierarchy; }
    }

    public bool isActive
    {
        get { return obj.activeInHierarchy; }
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        soundManager = SoundManager.instance;
        currencyManager = gameManager.currencyManager;
        map = gameManager.map;
        upgrade = gameManager.upgrade;
        roomManager = gameManager.roomManager;
        nftManager = gameManager.nftManager;
    }

    private void OnEnable()
    {
        DataManager.OnDataReceived += DataReceived;
    }

    private void OnDestroy()
    {
        DataManager.OnDataReceived -= DataReceived;
    }

    private void OnDisable()
    {
        DataManager.OnDataReceived -= DataReceived;
    }

    private IEnumerator IEActiveUnlock()
    {
        while (!isExtraActive)
        {
            objUnlock.SetActive(!gameManager.isOtherPanelActive);
            yield return new WaitForSecondsRealtime(1);
        }
    }

    private void DataReceived(DataManager dataManager)
    {
        netWorth = dataManager.netWorth;
        NextGift();
        UpdateHomeData();
        SetNFTData();
    }

    private void NextGift()
    {
        for (int i = 0; i < giftsDB.itemCount; i++)
        {
            if (!giftsDB.IsPurchased(i))
            {
                nextGift = i;
                return;
            }
        }
    }

    private void UpdateHomeData()
    {
        sliderHome.value = (float)netWorth / (float)giftsDB.Cost(nextGift);
        imgHomeReward.sprite = giftsDB.Image(nextGift);
        txtHomeNetWorth.text = netWorth.ToString();

        if (netWorth < giftsDB.Cost(nextGift)) txtHomeNextReward.text = $"{giftsDB.Cost(nextGift) - netWorth} more for {giftsDB.Name(nextGift)}";
        else txtHomeNextReward.text = $"Your {giftsDB.Name(nextGift)} is Wating for you";
    }

    private void UpdateUnlockData()
    {
        txtUnlockName.text = giftsDB.Name(nextGift);
        txtUnlockNetWorth.text = netWorth.ToString();
        imgUnlockReward.sprite = giftsDB.Image(nextGift);
    }

    private void CheckUnlockGifts()
    {
        if (nextGift >= 99)
        {
            sliderHome.value = 1;
            imgHomeReward.sprite = giftsDB.Image(nextGift);
            txtHomeNetWorth.text = netWorth.ToString();
            txtHomeNextReward.text = "All Rewards Achieved";
            return;
        }

        if (netWorth >= giftsDB.Cost(nextGift))
        {
            UpdateUnlockData();
            StopAllCoroutines();
            StartCoroutine(IEActiveUnlock());
        }
    }

    private void SetContentData()
    {
        if (nextGift >= 99)
        {
            sliderNet.value = 1;
            imgNetReward.sprite = giftsDB.Image(nextGift);
            txtNetNetWorth.text = netWorth.ToString();
            txtNetNextReward.text = "All Rewards Achieved";
        }
        else
        {
            sliderNet.value = (float)netWorth / (float)giftsDB.Cost(nextGift);
            imgNetReward.sprite = giftsDB.Image(nextGift);
            txtNetNetWorth.text = netWorth.ToString();
            if (netWorth < giftsDB.Cost(nextGift)) txtNetNextReward.text = $"{giftsDB.Cost(nextGift) - netWorth} more for {giftsDB.Name(nextGift)}";
            else txtNetNextReward.text = $"Your {giftsDB.Name(nextGift)} is Wating for you";
        }

        txtMetaVerse.text = $"Metaverse Conquered    {map.currIsland}/12";

        SetHardwareData();
        SetGiftData();
        SetNFTData();
    }

    private void SetNFTData()
    {
        int itemCount = 0;
        for (int i = 0; i < nftDB.itemCount; i++)
        {
            if (nftDB.nfts[i].isPurchased)
            {
                nftItem[i].SetData(nftDB.GetItem(i).image, 0);
                nftItem[i].gameObject.SetActive(true);
                itemCount++;
            }
            else
            {
                nftItem[i].gameObject.SetActive(false);
            }
        }
        txtNFTCount.text = itemCount.ToString();
    }

    private void SetGiftData()
    {
        int itemCount = 0;
        for (int i = 0; i < giftsDB.itemCount; i++)
        {
            if (giftsDB.IsPurchased(i))
            {
                collectionItem[i].SetData(giftsDB.Image(i), 0);
                collectionItem[i].gameObject.SetActive(true);
                itemCount++;
            }
            else
            {
                collectionItem[i].gameObject.SetActive(false);
            }
        }
        txtGiftCount.text = itemCount.ToString();
    }

    private void SetHardwareData()
    {
        int itemCount = 0;
        for (int i = 0; i < 4; i++)
        {
            hardwareItem[i].gameObject.SetActive(roomManager.itemCount[i] > 0);
            itemCount += roomManager.itemCount[i];
        }
        hardwareItem[0].SetData(roomManager.rooms[0].graphicCard.GetSprite(), roomManager.itemCount[0]);
        hardwareItem[1].SetData(roomManager.rooms[0].algorithm.GetSprite(), roomManager.itemCount[1]);
        hardwareItem[2].SetData(roomManager.rooms[0].coolingDevice.GetSprite(), roomManager.itemCount[2]);
        hardwareItem[3].SetData(roomManager.rooms[0].cable.GetSprite(), roomManager.itemCount[3]);

        txtHardwareCount.text = itemCount.ToString();
    }

    public void ChangeNetWorth(long amount)
    {
        netWorth += amount;
        UpdateHomeData();
        CheckUnlockGifts();
    }

    public void HardwareButton()
    {
        soundManager.PlaySound(soundManager.clipTap);
        txtGet.text = "Get More Hardware";
        scrollRect.content = hardware;
        hardware.gameObject.SetActive(true);
        nft.gameObject.SetActive(false);
        collection.gameObject.SetActive(false);
    }

    public void NFTButton()
    {
        soundManager.PlaySound(soundManager.clipTap);
        txtGet.text = "Get More NFTs";
        scrollRect.content = nft;
        hardware.gameObject.SetActive(false);
        nft.gameObject.SetActive(true);
        collection.gameObject.SetActive(false);
    }

    public void CollectionButton()
    {
        soundManager.PlaySound(soundManager.clipTap);
        txtGet.text = "Play to Earn More";
        scrollRect.content = collection;
        hardware.gameObject.SetActive(false);
        nft.gameObject.SetActive(false);
        collection.gameObject.SetActive(true);
    }

    public void ClaimButton()
    {
        soundManager.PlaySound(soundManager.clipGift);
        giftsDB.PurchasedItem(nextGift, true);
        nextGift++;
        Invoke("UpdateHomeData", 1);
        objUnlock.SetActive(false);
        Invoke("CheckUnlockGifts", 1.5f);
    }

    public void GetButton()
    {
        if (nft.gameObject.activeInHierarchy)
        {
            nftManager.ActiveButton(true);
        }
        else if (hardware.gameObject.activeInHierarchy)
        {
            foreach (Room room in roomManager.rooms)
            {
                if (!room.isMax)
                {
                    room.UpgradeButton();
                    break;
                }
            }
        }
        Desable();
    }

    public void Desable()
    {
        objUnlock.SetActive(false);
        ActiveButton(false);
    }

    public void MapButton()
    {
        Desable();
        map.ActiveButton(true);
    }

    public void ActiveButton(bool isActive)
    {
        if (isActive) SetContentData();
        SoundManager.instance.PlaySound(SoundManager.instance.clipTap);
        obj.SetActive(isActive);
    }

    public override void GetReward(int scriptId)
    {
        if (scriptId == this.GetInstanceID())
        {
            int reward = roomManager.roomCount * Random.Range(95, 120);
            Message.instance.Show($"Rewarded with {reward} coins", Color.green);
            currencyManager.ChangeFiat(reward, adButton.transform.position, Vector3.zero);
        }
    }

    public override void ShowAd(int scriptId)
    {
        ClaimButton();
        base.ShowAd(this.GetInstanceID());
    }
}
