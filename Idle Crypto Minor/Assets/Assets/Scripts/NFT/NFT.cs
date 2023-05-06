using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NFT : MonoBehaviour
{
    private NFTData nftData;
    [SerializeField] Material matGrayScale;

    [SerializeField] Transform imgOverlay;
    [SerializeField] GameObject objLock;
    [SerializeField] Image imgBG;
    [SerializeField] Image imgCoin;
    [SerializeField] Image imgCharacter;
    [SerializeField] TMP_Text txtCost;
    [SerializeField] TMP_Text txtName;
    [SerializeField] TMP_Text txtMine;

    public static Action<int> OnNFTButton;

    public void SetData(NFTData nftData)
    {
        this.nftData = nftData;

        imgCharacter.sprite = nftData.image;

        txtName.text = nftData.name;
        UpdaetStatus();
    }

    public void UpdaetStatus()
    {
        if (nftData.mine > 0)
        {
            imgOverlay.localScale = new Vector3(1, nftData.mine / 100, 1);
            txtMine.text = nftData.mine.ToString("F1") + "%";
            txtMine.gameObject.SetActive(true);
            objLock.SetActive(true);
            txtCost.text = nftData.cost.ToString();
        }
        else
        {
            imgOverlay.gameObject.SetActive(false);
            txtMine.gameObject.SetActive(false);
            objLock.SetActive(false);

            if (nftData.isPurchased)
            {
                imgBG.material = null;
                imgCoin.material = null;
                imgCharacter.material = null;

                txtCost.text = (nftData.stackTime != 72000) ? "Rented" : "Purchased";
            }
            else
            {

                imgBG.material = matGrayScale;
                imgCoin.material = matGrayScale;
                imgCharacter.material = matGrayScale;

                txtCost.text = nftData.cost.ToString();
            }
        }
    }

    public void NFTButton()
    {
        OnNFTButton?.Invoke(nftData.index);
    }
}
