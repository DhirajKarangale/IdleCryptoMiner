using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AnimateCoin : MonoBehaviour
{
    [SerializeField] Sprite fiatSprite;
    [SerializeField] Sprite btcSprite;
    [SerializeField] Image coinPrefab;
    [SerializeField] TMPro.TMP_Text txtCount;
    [SerializeField] GameObject objTextBG;
    [SerializeField] RectTransform coinTarget;
    [SerializeField] RectTransform btcTarget;

    private int mxCoins = 10;
    private float minDuration = 0.5f;
    private float maxDuration = 2.5f;
    private Vector3 targetPos;
    private Queue<Image> coinQueue;

    private void Start()
    {
        targetPos = coinTarget.position;
        coinQueue = new Queue<Image>();
        PrepareCoin();
    }

    private IEnumerator IEAnimateCoinTxt(long amount, Vector3 startPos, Vector3 endPos)
    {
        if (amount >= 0)
        {
            txtCount.text = $"+{amount}";
            txtCount.color = Color.green;
            objTextBG.transform.position = startPos;
        }
        else
        {
            txtCount.text = $"{amount}";
            txtCount.color = Color.red;
            objTextBG.transform.position = endPos;
        }

        objTextBG.transform.DOScale(1.1f, 0.3f).SetEase(Ease.Flash);

        yield return new WaitForSecondsRealtime(0.7f);

        objTextBG.transform.DOScale(0, 0.3f).SetEase(Ease.Flash);
    }

    private IEnumerator IEAnimateBTCTxt(long amount, Vector3 startPos)
    {
        txtCount.text = $"Big Reward";
        txtCount.color = Color.green;
        objTextBG.transform.position = startPos;

        objTextBG.transform.DOScale(1.1f, 0.3f).SetEase(Ease.Flash);

        yield return new WaitForSecondsRealtime(0.7f);

        objTextBG.transform.DOScale(0, 0.3f).SetEase(Ease.Flash);
    }

    private void PrepareCoin()
    {
        for (int i = 0; i < mxCoins; i++)
        {
            Image coin = Instantiate(coinPrefab);
            coin.transform.SetParent(this.transform);
            coin.transform.localScale = Vector3.one;
            coin.gameObject.SetActive(false);
            coinQueue.Enqueue(coin);
        }
    }

    private void AnimateCoins(int amount, Vector3 startPos, bool isCoin)
    {
        for (int i = 0; i < amount; i++)
        {
            if (coinQueue.Count > 0)
            {
                Image coin = coinQueue.Dequeue();
                coin.sprite = isCoin ? fiatSprite : btcSprite;
                coin.gameObject.SetActive(true);
                coin.transform.position = startPos;

                coin.transform.DOMove(targetPos, Random.Range(minDuration, maxDuration)).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    coin.gameObject.SetActive(false);
                    coinQueue.Enqueue(coin);
                });
            }
        }
    }

    public void MoveFiat(long amount, Vector3 startPos, Vector3 endPos)
    {
        if (endPos == Vector3.zero) targetPos = coinTarget.position;
        else targetPos = endPos;
        if (startPos == Vector3.zero) startPos = coinTarget.position;

        StartCoroutine(IEAnimateCoinTxt(amount, startPos, endPos));

        if (amount == 0) return;

        int coinAmount = Mathf.Clamp((int)Mathf.Abs(amount) / (mxCoins * 3), 2, mxCoins);
        AnimateCoins(coinAmount, startPos, true);
    }

    public void MoveBTC(int amount, Vector3 startPos)
    {
        targetPos = btcTarget.position;
        AnimateCoins(amount, startPos, false);
        if (amount >= 3) StartCoroutine(IEAnimateBTCTxt(amount, startPos));
    }
}
