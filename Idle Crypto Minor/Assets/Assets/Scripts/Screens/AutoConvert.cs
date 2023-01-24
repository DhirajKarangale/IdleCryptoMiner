using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoConvert : RewardBase
{
    [SerializeField] Image imgFill;
    [SerializeField] TMPro.TMP_Text txtTimer;
    [SerializeField] GameObject circleBg;

    private float time = 120;
    private float rate = 10;
    private float remainDuration;
    private ConvertBTC convert;
    private bool isButtonActive;

    public void Start()
    {
        convert = GameManager.instance.convert;
        StopAllCoroutines();
        isButtonActive = true;
        txtTimer.text = "Auto Convert";
        circleBg.SetActive(false);
    }

    private void StartAutoConvert()
    {
        Message.instance.Show("Auto Converted Bitcoin Activited", Color.green);
        remainDuration = time;
        circleBg.SetActive(true);
        isButtonActive = false;
        StartCoroutine(IEAutoConvert());
        StartCoroutine(IEUpdateTimer());
    }

    private IEnumerator IEAutoConvert()
    {
        convert.ConvertButton();
        yield return new WaitForSecondsRealtime(rate);
        StartCoroutine(IEAutoConvert());
    }

    private IEnumerator IEUpdateTimer()
    {
        while (remainDuration >= 0)
        {
            txtTimer.text = $"{remainDuration / 60:00} : {remainDuration % 60:00}s";
            imgFill.fillAmount = Mathf.InverseLerp(0, time, remainDuration);
            remainDuration--;
            yield return new WaitForSecondsRealtime(1);
        }
        OnEnd();
    }

    private void OnEnd()
    {
        StopAllCoroutines();
        isButtonActive = true;
        circleBg.SetActive(false);
        txtTimer.text = "Auto Convert";
        Message.instance.Show("Auto Convert Finished", Color.white);
    }

    public void AutoConvertButton()
    {
        if (!isButtonActive)
        {
            Message.instance.Show("Auto Convert is Already Active", Color.white);
            return;
        }
        convert.ActiveButton(false);
        ShowAd(this.GetInstanceID());
    }

    public override void GetReward(int scriptId)
    {
        if (scriptId == this.GetInstanceID()) { StartAutoConvert(); }
    }
}
