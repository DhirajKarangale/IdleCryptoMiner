using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NFTNotMined : MonoBehaviour
{
    [Header("NFT Data")]
    [SerializeField] Image image;
    [SerializeField] TMP_Text txtName;
    [SerializeField] TMP_Text txtBackground;
    [SerializeField] TMP_Text txtClothes;
    [SerializeField] TMP_Text txtEyes;
    [SerializeField] TMP_Text txtHat;
    [SerializeField] TMP_Text txtMounth;

    [Header("Mine")]
    [SerializeField] TMP_Text txtMine;
    [SerializeField] GameObject objLock;
    [SerializeField] Transform imgOverlay;

    [Header("Staking")]
    [SerializeField] Slider slider;
    [SerializeField] GameObject objStaking;
    [SerializeField] GameObject objRented;
    [SerializeField] GameObject objNotRented;
    [SerializeField] Transform rentButton;
    [SerializeField] TMP_Text txtStakCost;
    [SerializeField] TMP_Text txtRentStakCost;
    [SerializeField] TMP_Text txtStakTime;
    private const string SaveStackTime = "SAVESTACKTIME";

    [Header("Objects")]
    [SerializeField] GameObject objStats;
    [SerializeField] GameObject objButtons;
    [SerializeField] internal GameObject obj;
    [SerializeField] GameObject objMineBtns;
    [SerializeField] Button buttonPurchase;
    [SerializeField] TMP_Text txtCost;

    [Header("Refrences")]
    [SerializeField] NFTManager nftManager;

    private NFTData nftData;
    private SoundManager soundManager;
    private CurrencyManager currencyManager;


    private void OnEnable()
    {
        soundManager = SoundManager.instance;
        currencyManager = GameManager.instance.currencyManager;
        NFT.OnNFTButton += OnSetData;
    }

    private void OnDestroy()
    {
        NFT.OnNFTButton -= OnSetData;
    }

    private void OnDisable()
    {
        NFT.OnNFTButton -= OnSetData;
    }


    private void OnSetData(int index)
    {
        nftData = nftManager.nftDB.GetItem(index);
        if (nftData.stakingCost == 0) nftData.stakingCost = nftData.cost / 100;
        UpdateUI();
        obj.SetActive(true);
    }


    private void UpdateUI()
    {
        soundManager.PlaySound(soundManager.clipTap);

        image.sprite = nftData.image;
        txtName.text = nftData.name;
        txtBackground.text = nftData.background;
        txtClothes.text = nftData.clothes;
        txtEyes.text = nftData.eyes;
        txtHat.text = nftData.hat;
        txtMounth.text = nftData.mouth;

        txtCost.text = nftData.cost.ToString();
        buttonPurchase.gameObject.SetActive(nftData.mine <= 0 && !nftData.isPurchased);
        buttonPurchase.interactable = nftData.cost <= currencyManager.fiat;

        objMineBtns.SetActive(nftData.mine > 0);

        imgOverlay.localScale = new Vector3(1, nftData.mine / 100, 1);
        objLock.SetActive(nftData.mine > 0 && nftData.isPurchased);
        txtMine.text = nftData.mine.ToString("F1") + "%";
        txtMine.gameObject.SetActive(nftData.mine > 0);

        if (nftData.mine <= 0 && nftData.isPurchased)
        {
            objStaking.SetActive(true);
            objButtons.SetActive(false);
            objStats.SetActive(false);

            txtStakCost.text = nftData.stakingCost.ToString();
            txtRentStakCost.text = nftData.stakingCost.ToString();

            if (nftData.stackTime != 72000)
            {
                CheckStackTime();
                objRented.SetActive(true);
                objNotRented.SetActive(false);
            }
            else
            {
                if (nftData.stakingCost == 0) nftData.stakingCost = nftData.cost / 100;
                objRented.SetActive(false);
                objNotRented.SetActive(true);
            }
        }
        else
        {
            objStaking.SetActive(false);
            objButtons.SetActive(true);
            objStats.SetActive(true);
        }
    }

    private void StakingComplete()
    {
        if (nftData.stackTime <= 0)
        {
            nftData.stakingCost = nftData.cost / 100;
            objStaking.SetActive(true);
            objButtons.SetActive(false);
            objStats.SetActive(false);
            objRented.SetActive(false);
            objNotRented.SetActive(true);
            nftData.stackTime = 72000;
        }
    }

    private void SetStackTime()
    {
        int hr = TimeSpan.FromSeconds(nftData.stackTime).Hours;
        int min = TimeSpan.FromSeconds(nftData.stackTime).Minutes;

        txtStakTime.text = $"{hr}h {min}m";
        slider.value = (float)nftData.stackTime / 72000;
    }

    private void CheckStackTime()
    {
        DateTime currTime = DateTime.Now;
        DateTime rewardCollectedTime = DateTime.Parse(PlayerPrefs.GetString(SaveStackTime, currTime.ToString()));
        double timeDiffernce = (currTime - rewardCollectedTime).TotalSeconds;
        nftData.stackTime -= timeDiffernce;
        PlayerPrefs.SetString(SaveStackTime, DateTime.Now.ToString());

        SetStackTime();
        StakingComplete();
    }

    internal void MineAd()
    {
        if (nftData.stakingCost == 0) nftData.stakingCost = nftData.cost / 100;
        nftData.mine -= 10;
        UpdateUI();
        nftManager.UpdateNFTStatus();
    }

    internal void XStaking()
    {
        nftData.stakingCost *= 2;
        UpdateUI();
    }

    internal void ReduceStakTime()
    {
        nftData.stackTime -= 1800;

        int hr = TimeSpan.FromSeconds(nftData.stackTime).Hours;
        int min = TimeSpan.FromSeconds(nftData.stackTime).Minutes;

        txtStakTime.text = $"{hr}h {min}m";
        slider.value = (float)nftData.stackTime / 72000;

        SetStackTime();
        StakingComplete();
        UpdateUI();
    }

    internal void BackButton()
    {
        obj.SetActive(false);
        nftData = null;
    }

    public void ButtonMine()
    {
        if (nftData.stakingCost == 0) nftData.stakingCost = nftData.cost / 100;
        nftData.mine -= 0.5f;
        UpdateUI();
        nftManager.UpdateNFTStatus();
    }


    public void ButtonPurchase()
    {
        if (nftData.stakingCost == 0) nftData.stakingCost = nftData.cost / 100;
        soundManager.PlaySound(soundManager.clipHardware);
        currencyManager.ChangeFiat(-nftData.cost, nftManager.txtFiat.transform.position, buttonPurchase.transform.position);
        nftData.isPurchased = true;
        UpdateUI();
        nftManager.UpdateNFTStatus();
    }

    public void ButtonGetStak()
    {
        Message.instance.Show($"Got {nftData.stakingCost} coins", Color.green);
        currencyManager.ChangeFiat(nftData.stakingCost, rentButton.position, Vector3.zero);
        nftData.stackTime -= 2;
        objNotRented.SetActive(false);
        objRented.SetActive(true);
        CheckStackTime();
        PlayerPrefs.SetString(SaveStackTime, DateTime.Now.ToString());
        nftManager.UpdateNFTStatus();
    }
}
