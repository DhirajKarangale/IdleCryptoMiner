using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NFT : MonoBehaviour
{
    private NFTData nftData;
    [SerializeField] Material matGrayScale;

    [SerializeField] Image imgBG;
    [SerializeField] Image imgCoin;
    [SerializeField] Image imgCharacter;
    [SerializeField] TMP_Text txtCost;
    [SerializeField] TMP_Text txtName;

    public static Action<int> OnNFTButton;

    public void SetData(NFTData nftData)
    {
        this.nftData = nftData;

        imgCharacter.sprite = nftData.image;
        txtName.text = nftData.name;
        Status(nftData.isPurchased);
    }

    public void Status(bool isPurchased)
    {
        if (isPurchased)
        {
            imgBG.material = null;
            imgCoin.material = null;
            imgCharacter.material = null;

            txtCost.text = "Purchased";
        }
        else
        {
            imgBG.material = matGrayScale;
            imgCoin.material = matGrayScale;
            imgCharacter.material = matGrayScale;

            txtCost.text = nftData.cost.ToString();
        }
    }

    public void NFTButton()
    {
        OnNFTButton?.Invoke(nftData.index);
    }
}
