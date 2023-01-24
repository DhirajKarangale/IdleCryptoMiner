using UnityEngine;

[CreateAssetMenu(fileName = "NFTDB", menuName = "NFT DB")]
public class NFTDB : ScriptableObject
{
    public NFTData[] nfts;

    public int itemCount
    {
        get { return nfts.Length; }
    }

    public NFTData GetItem(int index)
    {
        return nfts[index];
    }

    public void PurchasedItem(int index)
    {
        nfts[index].isPurchased = true;
    }
}