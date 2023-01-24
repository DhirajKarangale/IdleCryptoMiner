using System.Collections.Generic;

[System.Serializable]
public struct RoomData
{
    public int graphicCard;
    public int algorithm;
    public int coolingDevice;
    public int cable;

    public double[] repareGraphicCard;
    public double[] repareAlgorithm;
    public double[] repareCoolingDevice;

    // public RepareData repareData;
}

// [System.Serializable]
// public struct RepareData
// {
//     public double[] graphicCard;
//     public double[] algorithm;
//     public double[] coolingDevice;
// }

public class DataItems
{
    public int map;
    public long fiat;
    public double btc;
    public long netWorth;
    public double hashSpeed;
    // public string closeTime;
    // public float addRoomRemainTime;
    // public List<RepareData> repareData;
    public List<RoomData> roomData;
    public bool[] nfts;
    public bool[] gifts;

    public DataItems(long fiat, double btc, long netWorth, double hashSpeed, int map, List<RoomData> roomData, bool[] nfts, bool[] gifts)
    {
        this.fiat = fiat;
        this.btc = btc;
        this.netWorth = netWorth;
        this.hashSpeed = hashSpeed;
        this.map = map;
        // this.closeTime = closeTime;
        // this.addRoomRemainTime = addRoomRemainTime;
        // this.repareData = repareData;
        this.roomData = roomData;
        this.nfts = nfts;
        this.gifts = gifts;
    }
}
