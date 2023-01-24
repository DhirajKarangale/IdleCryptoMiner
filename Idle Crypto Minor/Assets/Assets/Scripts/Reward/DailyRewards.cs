using System;
using UnityEngine;
using System.Collections;

public class DailyRewards : RewardBase
{
    [SerializeField] GameObject obj;
    [SerializeField] GameObject objCollectButton;
    [SerializeField] GameObject obj2XButton;
    [SerializeField] GameObject objMsg;
    [SerializeField] GameObject objNotification;
    [SerializeField] RewardItem[] rewardItems;
    public bool isActive
    {
        get { return obj.activeInHierarchy; }
    }

    private RoomManager roomManager;
    private CurrencyManager currencyManager;
    private XReward xReward;
    private SoundManager soundManager;

    private int[] rewardAmount;
    private int strikeDays;
    private bool isRewardReady = false;
    private double nextReawrdDelay = 86400;

    private const string saveStrikeDays = "StrikeDays";
    private const string saveCollectTime = "CollectTime";

    public void Start()
    {
        roomManager = GameManager.instance.roomManager;
        currencyManager = GameManager.instance.currencyManager;
        xReward = GameManager.instance.xReward;
        soundManager = SoundManager.instance;

        SetRewardAmount();
        CheckStrikeDays();
    }

    private IEnumerator IECheckReward()
    {
        while (true)
        {
            if (!isRewardReady)
            {
                DateTime currTime = DateTime.Now;
                DateTime rewardCollectedTime = DateTime.Parse(PlayerPrefs.GetString(saveCollectTime, currTime.ToString()));

                double timeDiffernce = (currTime - rewardCollectedTime).TotalSeconds;
                if (timeDiffernce > nextReawrdDelay)
                {
                    RewardStatus(true);
                    yield break;
                }
                else RewardStatus(false);

            }
            yield return new WaitForSeconds(5);
        }
    }

    private void SetRewardAmount()
    {
        rewardAmount = new int[6];

        for (int i = 0; i < 6; i++)
        {
            rewardAmount[i] = ((i + 1) % 7) * 50 + roomManager.roomCount * 100 + 50 * ((i + 1) / 7);
        }
    }

    private void CheckStrikeDays()
    {
        if (!PlayerPrefs.HasKey(saveCollectTime))
        {
            strikeDays = 0;
            RewardStatus(true);
        }
        else
        {
            DateTime currTime = DateTime.Now;
            DateTime rewardCollectedTime = DateTime.Parse(PlayerPrefs.GetString(saveCollectTime, currTime.ToString()));

            double timeDiffernce = (currTime - rewardCollectedTime).TotalSeconds;
            if (timeDiffernce > 2 * nextReawrdDelay)
            {
                strikeDays = 0;
                RewardStatus(true);
                Message.instance.Show("Strike Ends", Color.red);
            }
            else
            {
                strikeDays = PlayerPrefs.GetInt(saveStrikeDays);
                if (strikeDays > (rewardAmount.Length + 1)) strikeDays = 0;
                RewardStatus(false);
                StopAllCoroutines();
                StartCoroutine(IECheckReward());
            }
        }
    }

    private void UpdateRewardItems()
    {
        for (int i = 0; i < 6; i++)
        {
            rewardItems[i].SetData(strikeDays > (i), rewardAmount[i], i + 1);
        }
    }

    private void RewardStatus(bool isAvailable)
    {
        isRewardReady = isAvailable;
        objNotification.SetActive(isAvailable);
        objCollectButton.SetActive(isAvailable);
        obj2XButton.SetActive(isAvailable);
        objMsg.SetActive(!isAvailable);
        UpdateRewardItems();
    }

    public void CollectButton()
    {
        strikeDays = (strikeDays + 1) % (rewardAmount.Length + 1);
        if (strikeDays <= 1) strikeDays = 1;
        currencyManager.ChangeFiat(rewardAmount[strikeDays - 1], objCollectButton.transform.position, Vector3.zero);
        Message.instance.Show("Reward Collected", Color.green);
        PlayerPrefs.SetString(saveCollectTime, DateTime.Now.ToString());
        PlayerPrefs.SetInt(saveStrikeDays, strikeDays);
        RewardStatus(false);
        ActiveButton(false);
    }

    public void XRewardButton()
    {
        ShowAd(this.GetInstanceID());
    }

    public override void GetReward(int scriptId)
    {
        if (scriptId == this.GetInstanceID())
        {
            if (strikeDays >= rewardAmount.Length) strikeDays = rewardAmount.Length;
            else if (strikeDays <= 0) strikeDays = 1;
            currencyManager.ChangeFiat(rewardAmount[strikeDays - 1] * 2, objCollectButton.transform.position, Vector3.zero);
            Message.instance.Show("Got 2X Reward", Color.green);
            PlayerPrefs.SetString(saveCollectTime, DateTime.Now.ToString());
            PlayerPrefs.SetInt(saveStrikeDays, strikeDays + 1);
            RewardStatus(false);
            ActiveButton(false);
        }
    }

    public override void RewardCancled()
    {
        base.RewardCancled();
        CollectButton();
    }

    public void ActiveButton(bool isActive)
    {
        soundManager.PlaySound(soundManager.clipTap);
        obj.SetActive(isActive);
    }
}
