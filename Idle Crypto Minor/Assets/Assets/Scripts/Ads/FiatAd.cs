using UnityEngine;

public class FiatAd : RewardBase
{
    [SerializeField] Transform endPos;
    [SerializeField] ConvertBTC convert;
    private RoomManager roomManager;
    private CurrencyManager currencyManager;
    private Vector3 targetPos;

    public void Start()
    {
        roomManager = GameManager.instance.roomManager;
        currencyManager = GameManager.instance.currencyManager;
    }

    public override void ShowAd(int scriptId)
    {
        if (convert) convert.ActiveButton(false);
        base.ShowAd(GetInstanceID());
    }

    public override void GetReward(int scriptId)
    {
        if (scriptId == GetInstanceID())
        {
            int reward = roomManager.roomCount * Random.Range(95, 120);

            if (endPos == null) targetPos = Vector3.zero;
            else targetPos = endPos.position;
            Message.instance.Show($"Rewarded with {reward} coins", Color.green);
            currencyManager.ChangeFiat(reward, adButton.transform.position, targetPos);
        }
    }
}
