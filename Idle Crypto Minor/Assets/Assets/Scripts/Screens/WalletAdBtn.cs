using UnityEngine;

public class WalletAdBtn : RewardBase
{
    [SerializeField] Wallet wallet;
    [SerializeField] UnityEngine.UI.Image image;
    [SerializeField] TMPro.TMP_Text txt;
    internal bool isWatched;

    public void Watched(Sprite sprite, string text)
    {
        isWatched = true;
        image.sprite = sprite;
        txt.text = text;
    }

    public void UnWatched(Sprite sprite, string text)
    {
        isWatched = false;
        image.sprite = sprite;
        txt.text = text;
    }

    public void ButtonWatchAd()
    {
        if (isWatched) return;
        ShowAd(this.GetInstanceID());
    }

    public override void GetReward(int scriptId)
    {
        if (scriptId == this.GetInstanceID()) { wallet.Watched(); }
    }
}
