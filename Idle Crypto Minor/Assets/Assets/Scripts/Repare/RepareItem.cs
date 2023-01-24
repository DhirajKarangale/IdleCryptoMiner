using UnityEngine;
using System.Collections;

public class RepareItem : MonoBehaviour
{
    [SerializeField] RoomItem roomItem;
    internal int itemToRepare;
    internal long repareCost;
    internal RepareItemData[] items;
    // private double[] btcs = new double[6];

    public bool isRepareMode
    {
        get
        {
            foreach (RepareItemData data in items)
            {
                if (data.isRepareMode) return true;
            }

            return false;
        }
    }

    private IEnumerator IECheckRepareMode(int index)
    {
        while (true)
        {
            items[index].isChecking = true;
            items[index].btc += roomItem.GetHashSpeed(index);

            // items[index].isRepareMode = (items[index].btc * 20000) >= (1f); // Test
            items[index].isRepareMode = (items[index].btc * 20000) >= (roomItem.GetPurchaseCost(index) * 3);

            if (items[index].isRepareMode)
            {
                items[index].isChecking = false;
                roomItem.RepareMode(index, !items[index].isPreviouslyRepareMode);
                repareCost += roomItem.GetRepareCost(index);
                itemToRepare++;
                yield break;
            }

            yield return new WaitForSecondsRealtime(1);
        }
    }


    public void UpdateList()
    {
        for (int i = 0; i < roomItem.items.Count; i++)
        {
            if (!items[i].isChecking)
            {
                StartCoroutine(IECheckRepareMode(i));
            }
        }
    }

    public void SetData(double[] btcData)
    {
        itemToRepare = 0;
        items = new RepareItemData[6];
        for (int i = 0; i < 6; i++)
        {
            items[i] = new RepareItemData();
            items[i].btc = btcData[i];
            items[i].isChecking = false;
            items[i].isRepareMode = false;
            // items[i].isPreviouslyRepareMode = ((items[i].btc * 20000) >= (1)); // Test
            items[i].isPreviouslyRepareMode = (items[i].btc * 20000) >= (roomItem.GetPurchaseCost(i) * 3);
        }

        Invoke("UpdateList", 2);
    }

    public double[] GetBTC()
    {
        double[] btcs = new double[6] { 0, 0, 0, 0, 0, 0 };

        if (items == null) return btcs;
        // Debug.Log(transform.name + " Items Count : " + items.Length);
        for (int i = 0; i < 6; i++)
        {
            btcs[i] = items[i].btc;
        }

        return btcs;
    }

    public void Repare()
    {
        itemToRepare = 0;
        repareCost = 0;
        for (int i = 0; i < 6; i++)
        {
            items[i].isRepareMode = false;
            items[i].isChecking = false;
            items[i].btc = 0;
        }
        for (int i = 0; i < roomItem.items.Count; i++)
        {
            roomItem.ActiveMode(i);
        }
    }
}











[System.Serializable]
public class RepareItemData
{
    internal double btc;
    internal bool isRepareMode;
    internal bool isChecking;
    internal bool isPreviouslyRepareMode;
}