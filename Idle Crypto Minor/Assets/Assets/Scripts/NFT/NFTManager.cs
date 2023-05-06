using UnityEngine;

public class NFTManager : ScreenBase
{
    
    [SerializeField] internal TMPro.TMP_Text txtFiat;
    [SerializeField] internal NFTDB nftDB;
    //[SerializeField] NFTBuy nftBuy;
    [SerializeField] NFTNotMined nftNotMined;
    [SerializeField] NFT[] nftS;
    private CurrencyManager currencyManager;
    private ConvertBTC convert;

    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey("SaveNewNFTData"))
        {
            for (int i = 0; i < nftDB.itemCount; i++)
            {
                nftDB.nfts[i].mine = 100;
                nftDB.nfts[i].stackTime = 72000;
            }
            PlayerPrefs.SetInt("SaveNewNFTData", 1);

            // UnityEditor.EditorUtility.SetDirty(nftDB);
            // UnityEditor.AssetDatabase.SaveAssets();
        }

        currencyManager = GameManager.instance.currencyManager;
        convert = GameManager.instance.convert;

        currencyManager.OnFiatChanged += UpdateFiat;
        DataManager.OnDataReceived += DataReceived;
    }

    private void OnDestroy()
    {
        currencyManager.OnFiatChanged -= UpdateFiat;
        DataManager.OnDataReceived -= DataReceived;
    }

    private void OnDisable()
    {
        currencyManager.OnFiatChanged -= UpdateFiat;
        DataManager.OnDataReceived -= DataReceived;
    }

    private void DataReceived(DataManager dataManager)
    {
        UpdateNFT();
    }

    private void UpdateFiat(long amount)
    {
        txtFiat.text = amount.ToCurrency();
    }

    public void UpdateNFTStatus()
    {
        for (int i = 0; i < nftDB.itemCount; i++)
        {
            nftS[i].UpdaetStatus();
        }
    }

    private void UpdateNFT()
    {
        for (int i = 0; i < nftDB.itemCount; i++)
        {
            nftDB.nfts[i].index = i;
            nftS[i].SetData(nftDB.GetItem(i));
        }
    }

    public void FiatButton()
    {
        ActiveButton(false);
        convert.ActiveButton(true);
    }

    public void BackButton()
    {
        if (nftNotMined.obj.activeInHierarchy)
        {
            nftNotMined.BackButton();
        }
        else
        {
            ActiveButton(false);
        }
    }
}
