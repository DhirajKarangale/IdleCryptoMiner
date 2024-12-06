using UnityEngine;

public class NFTMineAd : RewardBase
{
    [SerializeField] NFTNotMined nftNotMined;

    public override void ShowAd(int scriptId)
    {
        base.ShowAd(GetInstanceID());
    }

    public override void GetReward(int scriptId)
    {
        if (scriptId == GetInstanceID()) nftNotMined.MineAd();
    }
}
