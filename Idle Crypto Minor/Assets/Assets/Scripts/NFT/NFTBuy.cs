using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NFTBuy : MonoBehaviour
{
    [SerializeField] Material matGrayScale;
    [SerializeField] internal GameObject obj;
    [SerializeField] NFTManager nftManager;
    [SerializeField] GameObject objScrollView;
    [SerializeField] GameObject objOwn;
    [SerializeField] GameObject objPurchaseTag;

    [SerializeField] Image image;
    [SerializeField] Image imgPurchaseCoin;
    [SerializeField] Button buttonPurchase;
    [SerializeField] TMP_Text txtCost;
    [SerializeField] TMP_Text txtName;
    [SerializeField] TMP_Text txtBackground;
    [SerializeField] TMP_Text txtClothes;
    [SerializeField] TMP_Text txtEyes;
    [SerializeField] TMP_Text txtHat;
    [SerializeField] TMP_Text txtMounth;

    private SoundManager soundManager;
    private NFTData nftData;
    private CurrencyManager currencyManager;
    private int index;

    private void OnEnable()
    {
        soundManager = SoundManager.instance;
        currencyManager = GameManager.instance.currencyManager;
        NFT.OnNFTButton += SetData;
    }

    private void OnDestroy()
    {
        NFT.OnNFTButton -= SetData;
    }

    private void OnDisable()
    {
        NFT.OnNFTButton -= SetData;
    }

    private void SetData(int index)
    {
        soundManager.PlaySound(soundManager.clipTap);

        this.index = index;
        nftData = nftManager.nftDB.GetItem(index);

        image.sprite = nftData.image;
        txtCost.text = nftData.cost.ToString();
        txtName.text = nftData.name;

        txtBackground.text = nftData.background;
        txtClothes.text = nftData.clothes;
        txtEyes.text = nftData.eyes;
        txtHat.text = nftData.hat;
        txtMounth.text = nftData.mouth;

        objOwn.SetActive(nftData.isPurchased);
        objPurchaseTag.SetActive(nftData.isPurchased);
        buttonPurchase.gameObject.SetActive(!nftData.isPurchased);

        if (nftData.cost > currencyManager.fiat)
        {
            buttonPurchase.interactable = false;
            buttonPurchase.image.material = matGrayScale;
            imgPurchaseCoin.material = matGrayScale;
        }
        else
        {
            buttonPurchase.interactable = true;
            buttonPurchase.image.material = null;
            imgPurchaseCoin.material = null;
        }

        Debug.Log("NFT Object");
        obj.SetActive(true);
        objScrollView.SetActive(false);
    }

    public void PurchaseButton()
    {
        soundManager.PlaySound(soundManager.clipHardware);
        currencyManager.ChangeFiat(-nftData.cost, nftManager.txtFiat.transform.position, buttonPurchase.transform.position);
        nftData.isPurchased = true;
        //nftManager.nftDB.PurchasedItem(index);
        nftData.isPurchased = true;
        nftManager.UpdateNFTStatus();
        ActiveButton();
    }

    public void ActiveButton()
    {
        soundManager.PlaySound(soundManager.clipTap);

        index = -1;
        nftData = null;
        objScrollView.SetActive(true);
        obj.SetActive(false);
        buttonPurchase.gameObject.SetActive(false);
    }
}
