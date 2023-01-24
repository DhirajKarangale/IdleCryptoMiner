using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Boost : RewardBase
{
    [SerializeField] Image imgFill;
    [SerializeField] TMPro.TMP_Text txtTimer;

    private bool isButtonActive;
    private int time = 300;
    private int remainDuration;
    private CurrencyManager currencyManager;

    public void Start()
    {
        currencyManager = GameManager.instance.currencyManager;
        txtTimer.text = "Boost";
        imgFill.gameObject.SetActive(false);
        isButtonActive = true;
    }

    IEnumerator IEUpdateTimer()
    {
        while (remainDuration >= 0)
        {
            txtTimer.text = $"{remainDuration / 60:00} : {remainDuration % 60:00}s";
            imgFill.fillAmount = Mathf.InverseLerp(0, time, remainDuration);
            remainDuration--;
            isButtonActive = false;
            yield return new WaitForSeconds(1);
        }
        OnEnd();
    }

    private void OnEnd()
    {
        StopAllCoroutines();
        currencyManager.hashSpeed -= (currencyManager.hashSpeed / 100 * 2);
        isButtonActive = true;
        imgFill.gameObject.SetActive(false);
        txtTimer.text = "Boost";
        Time.timeScale = 1;
        Message.instance.Show("Boost Finished", Color.white);
    }

    private void BoostReward()
    {
        Message.instance.Show("Rewarded with Boost", Color.green);
        currencyManager.hashSpeed += (currencyManager.hashSpeed / 100 * 2);
        remainDuration = time;
        isButtonActive = false;
        imgFill.gameObject.SetActive(true);
        Time.timeScale = 2;
        StartCoroutine(IEUpdateTimer());
    }

    public void BoostButton()
    {
        if (!isButtonActive)
        {
            Message.instance.Show("Boost is Already Active", Color.white);
            return;
        }

        ShowAd(this.GetInstanceID());
    }

    public override void GetReward(int scriptId)
    {
        if (scriptId == this.GetInstanceID()) { BoostReward(); }
    }
}