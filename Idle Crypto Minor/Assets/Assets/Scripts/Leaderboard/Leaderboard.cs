using PlayFab;
using UnityEngine;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class Leaderboard : ScreenBase
{
    [SerializeField] GameObject objLoading;
    [SerializeField] Sprite spriteRank1;
    [SerializeField] Sprite spriteRank2;
    [SerializeField] Sprite spriteRank3;
    [SerializeField] LeaderboardStat playerStat;
    [SerializeField] LeaderboardStat[] allStats;
    private SoundManager soundManager;

    private int score;
    private string playerId;
    private bool isLoggedIn;

    private void Start()
    {
        soundManager = SoundManager.instance;
        isLoggedIn = PlayerPrefs.GetInt("LoggedIn", 0) == 1;
        playerId = PlayerPrefs.GetString("PlayfabId", "");

        if (PlayerPrefs.GetInt("NFTPurchased", 0) == 1)
        {
            ChangeScore(20);
            Send();
        }
        PlayerPrefs.SetInt("NFTPurchased", 0);

        GetPlayerStat();
    }

    private void DesableStats()
    {
        foreach (LeaderboardStat stat in allStats)
        {
            stat.gameObject.SetActive(false);
        }
    }

    private LeaderboardData GetData(int rank, string name, int score, string avatarUrl)
    {
        LeaderboardData data;

        data.name = name;
        data.score = score.ToString();
        data.avatarUrl = avatarUrl;

        switch (rank)
        {
            case 1:
                data.spriteRank = spriteRank1;
                data.rank = "";
                break;
            case 2:
                data.spriteRank = spriteRank2;
                data.rank = "";
                break;
            case 3:
                data.spriteRank = spriteRank3;
                data.rank = "";
                break;
            default:
                data.spriteRank = null;
                data.rank = rank.ToString();
                break;
        }

        return data;
    }

    public void Send()
    {
        if (!isLoggedIn) return;

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "Score",
                        Value = score
                    }
                }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate => { }, OnError => { });
    }


    private void GetPlayerStat()
    {
        if (!isLoggedIn) return;

        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "Score",
            MaxResultsCount = 1,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnPlayerStatSucess, OnGetLeaderboardFail =>
        {
            GetPlayerStat();
            return;
        });
    }

    private void OnPlayerStatSucess(GetLeaderboardAroundPlayerResult result)
    {
        var item = result.Leaderboard[0];
        if (item.PlayFabId == playerId)
        {
            score = item.StatValue;
            playerStat.SetData(GetData(item.Position + 1, item.DisplayName, item.StatValue, item.Profile.AvatarUrl));
        }
    }


    private void GetAllPlayerStat()
    {
        if (!isLoggedIn) return;

        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 50,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }

        };
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSucess, OnGetLeaderboardFail =>
        {
            Message.instance.Show("Error in getting Leaderboard Try Again ...", Color.red);
            ActiveButton(false);
            return;
        });
    }

    private void OnGetLeaderboardSucess(GetLeaderboardResult result)
    {
        DesableStats();
        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            var item = result.Leaderboard[i];

            if (item.StatValue == 0) return;
            allStats[i].gameObject.SetActive(true);
            allStats[i].SetData(GetData(item.Position + 1, item.DisplayName, item.StatValue, item.Profile.AvatarUrl));
        }
        objLoading.SetActive(false);
    }




    public void ChangeScore(int score)
    {
        this.score += score;
    }

    public override void ActiveButton(bool isActive)
    {
        soundManager.PlaySound(soundManager.clipTap);
        if (!isLoggedIn)
        {
            Message.instance.Show("Not Logged in...", Color.red);
            return;
        }
        if (isActive)
        {
            GetPlayerStat();
            GetAllPlayerStat();
        }
        objLoading.SetActive(isActive);
        obj.SetActive(isActive);
    }
}
