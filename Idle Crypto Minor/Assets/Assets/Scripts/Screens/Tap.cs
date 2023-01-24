using UnityEngine;

public class Tap : MonoBehaviour
{
    [SerializeField] Animator animatorGC;
    [SerializeField] Animator animatorTap;
    [SerializeField] Animator animatorAlgo;
    [SerializeField] AudioClip[] audioClips;

    private float perTapReward;
    private int userClicks;
    private int clickRange;

    private AnimateCoin animateCoin;
    private RoomManager roomManager;
    private SoundManager soundManager;
    private CurrencyManager currencyManager;

    private void Start()
    {
        userClicks = 0;
        perTapReward = 0.1f;
        clickRange = Random.Range(8, 20);

        soundManager = SoundManager.instance;
        animateCoin = GameManager.instance.animateCoin;
        roomManager = GameManager.instance.roomManager;
        currencyManager = GameManager.instance.currencyManager;
    }

    private void Reward()
    {
        soundManager.PlaySound(soundManager.clipCoin);

        double reward = (perTapReward * roomManager.roomCount * 0.00005f);
        animateCoin.MoveBTC(1, animatorAlgo.transform.position);
        currencyManager.ChangeBTC(reward);
    }

    private void RandomReward()
    {
        soundManager.PlaySound(audioClips[Random.Range(0, audioClips.Length)]);

        double reward = perTapReward * Random.Range(2, 15) * roomManager.roomCount * 0.00005f;
        userClicks = 0;
        clickRange = Random.Range(8, 20);
        animateCoin.MoveBTC(5, animatorAlgo.transform.position);
        currencyManager.ChangeBTC(reward);
    }

    public void TapButton()
    {


        animatorGC.Play("Play");
        animatorTap.Play("Play");
        animatorAlgo.Play("Play");

        userClicks++;
        if (userClicks >= clickRange) RandomReward();
        else Reward();
    }
}
