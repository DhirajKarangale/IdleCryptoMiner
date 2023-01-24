using UnityEngine;

public class XReward : ScreenBase
{
    [SerializeField] TMPro.TMP_Text txtReward;
    private CurrencyManager currencyManager;
    private long reward;

    private void Start()
    {
        currencyManager = GameManager.instance.currencyManager;
    }

    private void AdSucess()
    {
        reward *= 2;
        txtReward.text = reward.ToString("F0");
        obj.SetActive(true);
    }

    private void AdFail()
    {
        txtReward.text = reward.ToString("F0");
        obj.SetActive(true);
    }

    public void Reward(long reward)
    {
        this.reward = reward;
        // Show Ad
        AdSucess();
    }

    public void ClaimButton()
    {
        currencyManager.ChangeFiat(reward, txtReward.transform.position, Vector3.zero);
        obj.SetActive(false);
        Message.instance.Show("2X Reward Collected", Color.green);
    }
}
