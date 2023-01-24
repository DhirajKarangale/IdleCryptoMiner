using System;
using UnityEngine;

public class OfflineEarnings : RewardBase
{
    [SerializeField] GameObject obj;
    [SerializeField] TMPro.TMP_Text txtReward;
    public bool isActive
    {
        get { return obj.activeInHierarchy; }
    }

    private RoomManager roomManager;
    private CurrencyManager currencyManager;
    private XReward xReward;

    private long reward;
    private double timeDiff;
    private string lastTime;

    public void Start()
    {
        roomManager = GameManager.instance.roomManager;
        currencyManager = GameManager.instance.currencyManager;
        xReward = GameManager.instance.xReward;

        DataManager.OnTimeReceived += OnTimeReceived;
    }

    private void OnDisable()
    {
        DataManager.OnTimeReceived -= OnTimeReceived;
    }

    private void OnDestroy()
    {
        DataManager.OnTimeReceived -= OnTimeReceived;
    }

    private void OnTimeReceived(DataManager dataManager)
    {
        lastTime = dataManager.closeTime;
        Invoke("GetOfflineEarning", 1);
    }

    private void GetOfflineEarning()
    {
        if (string.IsNullOrEmpty(lastTime))
        {
            obj.SetActive(false);
        }
        else
        {
            DateTime currTime = DateTime.Now;
            timeDiff = (currTime - DateTime.Parse(lastTime)).TotalSeconds;
            timeDiff /= 60;
            if (timeDiff > 5)
            {
                reward = (long)UnityEngine.Random.Range((float)timeDiff * roomManager.roomCount, (float)timeDiff * roomManager.roomCount * 1.25f);
                if (reward <= 5) reward = 20;
                txtReward.text = reward.ToString("F0");
                obj.SetActive(true);
            }
        }
    }

    public void OkButton()
    {
        currencyManager.ChangeFiat(reward, txtReward.transform.position, Vector3.zero);
        Message.instance.Show("Reward Collected", Color.green);
        obj.SetActive(false);
    }

    public void XRewardButton()
    {
        ShowAd(this.GetInstanceID());
    }

    public override void GetReward(int scriptId)
    {
        if (scriptId == this.GetInstanceID())
        {
            currencyManager.ChangeFiat(reward * 2, txtReward.transform.position, Vector3.zero);
            Message.instance.Show("Got 2X Reward", Color.green);
            obj.SetActive(false);
        }
    }

    public override void RewardCancled()
    {
        base.RewardCancled();
        OkButton();
    }

    public void ActiveButton(bool isActive)
    {
        obj.SetActive(isActive);
    }
}
