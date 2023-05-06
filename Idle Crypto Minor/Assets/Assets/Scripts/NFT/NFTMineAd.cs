using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFTMineAd : RewardBase
{
    [SerializeField] NFTNotMined nftNotMined;

    public override void ShowAd(int scriptId)
    {
        base.ShowAd(this.GetInstanceID());
    }

    public override void GetReward(int scriptId)
    {
        if (scriptId == this.GetInstanceID())
        {
            nftNotMined.MineAd();
        }
    }
}
