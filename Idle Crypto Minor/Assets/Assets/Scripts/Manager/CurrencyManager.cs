using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    internal long fiat;
    internal double btc;
    internal double hashSpeed;

    private const float satoshi = 0.00000001f;

    public Action<long> OnFiatChanged;
    public Action<double> OnBTCChanged;

    private NetWorth netWorth;
    private AnimateCoin animateCoin;
    private SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.instance;
        netWorth = GameManager.instance.netWorth;
        animateCoin = GameManager.instance.animateCoin;
        InvokeRepeating("IncreaseBTC", 2, 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            fiat += 1000;
            OnFiatChanged?.Invoke(fiat);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            fiat = 0;
            OnFiatChanged?.Invoke(fiat);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Hashing Speed : " + hashSpeed);
        }
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

    private void DataReceived(DataManager dataManager)
    {
        fiat = dataManager.fiat;
        btc = dataManager.btc;
        hashSpeed = dataManager.hashSpeed;

        OnFiatChanged?.Invoke(fiat);
        OnBTCChanged?.Invoke(btc);
    }

    private void IncreaseBTC()
    {
        ChangeBTC(hashSpeed);
    }

    public void ResetCurrency()
    {
        hashSpeed = 0;
    }

    public void ChangeFiat(long amount, Vector3 startPos, Vector3 endPos)
    {
        if (amount > 0)
        {
            soundManager.PlaySound(soundManager.clipCoin);
            netWorth.ChangeNetWorth(amount);
        }
        animateCoin.MoveFiat(amount, startPos, endPos);
        fiat += amount;
        OnFiatChanged?.Invoke(fiat);
    }

    public void ChangeBTC(double amount)
    {
        btc += amount;
        OnBTCChanged?.Invoke(btc);
    }

    public void ChangeHashSpeed(double amount)
    {
        if (Loading.instance.obj.activeInHierarchy) return;
        hashSpeed += amount;
    }
}
