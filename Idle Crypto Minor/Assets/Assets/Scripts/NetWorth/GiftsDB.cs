using UnityEngine;

[CreateAssetMenu(fileName = "GiftsDB", menuName = "Gifts DB")]
public class GiftsDB : ScriptableObject
{
    [SerializeField] internal GiftData[] gifts;

    public int itemCount
    {
        get { return gifts.Length; }
    }

    public bool IsPurchased(int index)
    {
        return gifts[index].isPurchased;
    }

    public string Name(int index)
    {
        return gifts[index].name;
    }

    public long Cost(int index)
    {
        return gifts[index].cost;
    }

    public Sprite Image(int index)
    {
        return gifts[index].sprite;
    }

    public void PurchasedItem(int index, bool isPurchased)
    {
        gifts[index].isPurchased = isPurchased;
    }
}
