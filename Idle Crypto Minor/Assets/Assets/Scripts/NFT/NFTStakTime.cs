using UnityEngine;

public class NFTStakTime : RewardBase
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
            nftNotMined.ReduceStakTime();
        }
    }
}
