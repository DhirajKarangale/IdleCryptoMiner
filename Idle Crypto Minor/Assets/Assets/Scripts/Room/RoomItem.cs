using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomItem : MonoBehaviour
{
    [SerializeField] internal List<Animator> items;
    [SerializeField] internal Sprite[] sprites;
    [SerializeField] internal Transform content;
    [SerializeField] RepareItem repareItem;
    [SerializeField] int weight;
    private int traversingItem;
    internal int level;
    internal int spriteNo = 1;
    internal int latestItem = 1;
    internal bool isMax;
    internal bool isRepareMode;
    private float btcPrice = 20000f;

    internal long purchaseCoast;
    internal long repareCost;
    internal double hashSpeed;

    private int anim;

    private Room room;
    private RoomManager roomManager;
    private CurrencyManager currencyManager;

    private void Awake()
    {
        if (content) spriteNo = 0;
        else spriteNo = 1;
    }

    private void UpdateCount()
    {
        if (this.name.Contains("GraphicCards")) roomManager.itemCount[0]++;
        else if (this.name.Contains("Algo")) roomManager.itemCount[1]++;
        else if (this.name.Contains("CoolingDevice")) roomManager.itemCount[2]++;
        else if (this.name.Contains("Cable")) roomManager.itemCount[3]++;
    }

    public double GetHashSpeed(int index)
    {
        int itemlevel = level;
        if ((items.Count > 1) && ((index + 1) < latestItem) && (itemlevel > 1)) itemlevel--;

        long cost = (weight * 20 * itemlevel * (roomManager.rooms.IndexOf(room) + 1)) / 4;
        return (cost / (5000f * btcPrice));
    }

    public long GetRepareCost(int index)
    {
        int itemlevel = level;
        if ((items.Count > 1) && ((index + 1) < latestItem) && (itemlevel > 1)) itemlevel--;

        long cost = (weight * 20 * itemlevel * (roomManager.rooms.IndexOf(room) + 1)) / 4;
        return cost / 10;
    }

    public long GetPurchaseCost(int index)
    {
        int itemlevel = level;
        if ((items.Count > 1) && ((index + 1) < latestItem) && (itemlevel > 1)) itemlevel--;

        return (weight * 20 * itemlevel * (roomManager.rooms.IndexOf(room) + 1)) / 4;
    }

    public void RepareMode(int index, bool isHashSpeedChange = true)
    {
        isRepareMode = true;
        items[index].speed = 0;
        items[index].transform.GetChild(0).gameObject?.SetActive(true);
        if (isHashSpeedChange) currencyManager.ChangeHashSpeed(-GetHashSpeed(index));
    }

    public void ActiveMode(int index)
    {
        isRepareMode = false;
        items[index].speed = 1;
        items[index].transform.GetChild(0).gameObject?.SetActive(false);
        currencyManager.ChangeHashSpeed(GetHashSpeed(index));
    }

    public Sprite GetSprite()
    {
        return items[0].GetComponent<Image>().sprite;
    }

    public void UpdateData(Room room)
    {
        roomManager = GameManager.instance.roomManager;
        currencyManager = GameManager.instance.currencyManager;
        this.room = room;

        if (level <= 0) level = 1;

        purchaseCoast = (weight * 20 * level * (roomManager.rooms.IndexOf(room) + 1)) / 4;
        repareCost = purchaseCoast / 10;
        hashSpeed = purchaseCoast / (5000f * btcPrice);
        currencyManager.ChangeHashSpeed(hashSpeed);

        if (purchaseCoast > 0 && room.isRepareDataSet) repareItem?.UpdateList();
    }

    public void Upgrade()
    {
        if (isMax) return;

        if (content)
        {
            level++;

            if (items.Count < 6)
            {
                Animator newItem = Instantiate(items[0], Vector3.zero, Quaternion.identity);
                newItem.transform.SetParent(this.transform);
                newItem.name = $"GC{items.Count}";
                newItem.transform.localScale = Vector3.one;
                items.Add(newItem);
                latestItem = items.Count;
                traversingItem = 0;
                anim = 1;
                if (latestItem < 6) spriteNo = 0;
                else
                {
                    spriteNo = 1;
                    anim = 2;
                }
            }
            else
            {
                latestItem = traversingItem + 1;

                //Upgrade
                if ((latestItem == 1) && (anim < 10)) anim++;
                items[traversingItem].Play($"{items[0].name}{anim}");

                //Inc
                traversingItem++;
                if (traversingItem > 5) traversingItem = 0;

                //Sprite
                spriteNo = anim - 1;
                if ((latestItem == 6) && (spriteNo < (sprites.Length - 1))) spriteNo++;

                if ((latestItem == 6) && (anim == 10)) level = 60;
            }

            UpdateData(room);

            isMax = level >= 60;
            if (isMax)
            {
                latestItem = 6;
                return;
            }
        }
        else
        {
            level++;
            if (level < sprites.Length) spriteNo = level;
            items[0].Play($"{items[0].name}{level}");
            UpdateData(room);
            isMax = level >= 10;
            if (isMax) return;
        }

        UpdateCount();
    }
}
