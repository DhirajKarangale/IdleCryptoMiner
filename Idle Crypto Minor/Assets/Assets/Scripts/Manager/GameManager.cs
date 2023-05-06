using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    [Header("Refrences")]
    [SerializeField] internal Map map;
    [SerializeField] internal Menu menu;
    [SerializeField] internal AnimateCoin animateCoin;
    [SerializeField] internal ConvertBTC convert;
    [SerializeField] internal Upgrade upgrade;
    [SerializeField] internal XReward xReward;
    [SerializeField] internal NetWorth netWorth;
    [SerializeField] internal Leaderboard leaderboard;
    [SerializeField] internal RoomManager roomManager;
    [SerializeField] internal DataManager dataManager;
    [SerializeField] internal SaveManager saveManager;
    [SerializeField] internal DailyRewards dailyRewards;
    [SerializeField] internal OfflineEarnings offlineEarnings;
    [SerializeField] internal CurrencyManager currencyManager;
    [SerializeField] internal NFTManager nftManager;
    [SerializeField] internal RepareManager repareManager;
    [SerializeField] internal Wallet wallet;

    [SerializeField] GameObject objExit;
    private SoundManager soundManager;

    public bool isOtherPanelActive
    {
        get
        {
            return leaderboard.isActive || netWorth.isActive || netWorth.isExtraActive || dailyRewards.isActive || convert.isActive ||
        map.isActive || map.isExtraActive || menu.isActive || offlineEarnings.isActive || SoundManager.instance.isActive ||
        xReward.isActive || upgrade.isActive || nftManager.isActive || wallet.isActive;
        }
    }

    private void Start()
    {
        soundManager = SoundManager.instance;
        StartCoroutine(IECheckDataLoaded());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackButton();
        }
    }

    private IEnumerator IECheckDataLoaded()
    {
        while (true)
        {
            //Debug.Log("checking load"+roomManager.rooms.Count);

            yield return null;
            if (roomManager.rooms.Count >= 1)
            {
                Loading.instance.Active(false);
                yield break;
            }
        }
    }

    private void BackButton()
    {
        soundManager.PlaySound(soundManager.clipTap);

        if (objExit.activeInHierarchy)
        {
            objExit.SetActive(false);
        }
        else if (isOtherPanelActive)
        {
            leaderboard.ActiveButton(false);
            netWorth.Desable();
            dailyRewards.ActiveButton(false);
            convert.ActiveButton(false);
            map.Desable();
            menu.ActiveButton(false);
            offlineEarnings.ActiveButton(false);
            soundManager.ActiveButton(false);
            xReward.ActiveButton(false);
            upgrade.ActiveButton(false);
            nftManager.BackButton();
            wallet.ActiveButton(false);
        }
        else
        {
            objExit.SetActive(true);
        }
    }

    public void ExitButton()
    {
        soundManager.PlaySound(soundManager.clipTap);
        Application.Quit();
    }
}
