using PlayFab;
using UnityEngine;
using System.Collections;
using PlayFab.ClientModels;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class Lobby : MonoBehaviour
{
    private const string titleId = "55B2F";

    [SerializeField] GiftsDB db;
    [SerializeField] Sprite[] sprites;

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        for (int i = 0; i < 100; i++)
        {
            db.gifts[i].sprite = sprites[i];
        }

        InitGoogle();
        StartCoroutine(IECheckInternet());
    }

    private IEnumerator IECheckInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Offline();
            yield return new WaitForSecondsRealtime(2);
            StartCoroutine(IECheckInternet());
        }
        else
        {
            Message.instance.Show("Online", Color.green);
            // if (PlayerPrefs.HasKey("GoogleToken")) AutoLoginGoogle();
            // else ButtonGoogle();

            ButtonGoogle();
        }
    }

    private void LoginTimeOut()
    {
        CancelInvoke();
        StopAllCoroutines();

        PlayerPrefs.DeleteKey("GoogleToken");
        PlayerPrefs.DeleteKey("FBToken");
        Loading.instance.Active(false);
    }

    private void InitGoogle()
    {
        // PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //     .AddOauthScope("profile")
        //     .RequestServerAuthCode(false)
        //     .Build();
        // PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    private void Offline()
    {
        Message.instance.Show("You are Offline", Color.red);
        Loading.instance.Active(false);
    }

    private void LoadGame()
    {
        Loading.instance.LoadLevel(1);
    }

    private void AutoLoginGoogle()
    {
        ButtonGoogle();
    }


    private void PlayfabGoogle(string token)
    {
        PlayFabClientAPI.LoginWithGooglePlayGamesServices(new LoginWithGooglePlayGamesServicesRequest
        {
            TitleId = titleId,
            ServerAuthCode = token,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
        }, GoogleLoginSucess, PlafabLoginFail
        );
    }

    private void GoogleLoginSucess(LoginResult result)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.Authenticate((SignInStatus result) =>
            {
                if (result == SignInStatus.Success)
                {
                    PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                    {
                        // PlayfabGoogle(code);
                        PlayerPrefs.SetString("GoogleToken", code);
                        UpdateGoogleData();
                    });
                }
            });

            // PlayerPrefs.SetString("GoogleToken", PlayGamesPlatform.Instance.GetServerAuthCode());
            // UpdateGoogleData();
        }
        PlayerPrefs.SetString("PlayfabId", result.PlayFabId);
        PlayerPrefs.SetInt("LoggedIn", 1);
        LoadGame();
    }

    private void PlafabLoginFail(PlayFabError error)
    {
        StopAllCoroutines();
        PlayerPrefs.DeleteKey("FBToken");
        PlayerPrefs.DeleteKey("GoogleToken");

        StartCoroutine(IECheckInternet());
        Message.instance.Show("Login Failed, Try again: " + error.ErrorMessage, Color.red);
    }

    private void UpdateGoogleData()
    {
        SetName(PlayGamesPlatform.Instance.GetUserDisplayName());
        SetPic(PlayGamesPlatform.Instance.GetUserImageUrl());
    }

    private void SetName(string userName)
    {
        userName = ClampName(userName);
        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = userName, };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnSucessUpdatePlayerData => { PlayerPrefs.SetString("PlayerName", userName); }, OnError);
    }

    private string ClampName(string name)
    {
        if (name.Length <= 6) return name + "    ";
        else if (name.Length >= 25) return name.Substring(0, 25);
        return name;
    }

    private void SetPic(string url)
    {
        if (string.IsNullOrEmpty(url)) return;
        PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest()
        { ImageUrl = url }, OnSuccess => { }, OnFailed => { });
    }

    private void OnError(PlayFabError error)
    {
        Offline();
    }


    public void ButtonGoogle()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Message.instance.Show("You are offline", Color.red);
            Loading.instance.Active(false);
            return;
        }

        // if (PlayGamesPlatform.Instance.IsAuthenticated()) PlayGamesPlatform.Instance.SignOut();
        // else InitGoogle();

        CancelInvoke();
        Invoke(nameof(LoginTimeOut), 10);

        // PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        // PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (SignInStatus result) =>
        // {
        //     if (result == SignInStatus.Success) PlayfabGoogle(PlayGamesPlatform.Instance.GetServerAuthCode());
        // });

        Debug.Log("====================== Button Google");
        PlayGamesPlatform.Instance.Authenticate((SignInStatus result) =>
        {
            Debug.Log("====================== Authentacting");
            if (result == SignInStatus.Success)
            {
                Debug.Log("======================= Auth Success");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Debug.Log("======================= Auth Code: " + code);
                    PlayfabGoogle(code);
                });
            }
            else
            {
                Debug.Log("====================== Auth failed");
            }
            Debug.Log("====================== Auth Status: " + result);
        });
    }

    public void GuestButton()
    {
        Message.instance.Show("Playing Offline", Color.white);
        PlayerPrefs.SetInt("LoggedIn", 0);
        LoadGame();
    }
}