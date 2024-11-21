using PlayFab;
using UnityEngine;
using Facebook.Unity;
using System.Collections;
using PlayFab.ClientModels;
using System.Collections.Generic;
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
        Loading.instance.Active(false);
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
            if (PlayerPrefs.HasKey("GoogleToken")) AutoLoginGoogle();
            else Loading.instance.Active(false);
        }
    }

    private void LoginTimeOut()
    {
        CancelInvoke();
        StopAllCoroutines();

        PlayerPrefs.DeleteKey("GoogleToken");
        PlayerPrefs.DeleteKey("FBToken");

        StartCoroutine(IECheckInternet());
    }

    private void InitGoogle()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .AddOauthScope("profile")
            .RequestServerAuthCode(false)
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
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
        Loading.instance.Active(false);

        PlayFabClientAPI.LoginWithGooglePlayGamesServices(new LoginWithGooglePlayGamesServicesRequest
        {
            TitleId = titleId,
            ServerAuthCode = token,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        }, GoogleLoginSucess, PlafabLoginFail
        );
    }

    private void GoogleLoginSucess(PlayFab.ClientModels.LoginResult result)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayerPrefs.SetString("GoogleToken", PlayGamesPlatform.Instance.GetServerAuthCode());
            UpdateGoogleData();
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
        {
            ImageUrl = url
        }, OnSuccess => { }, OnFailed => { });
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

        if (PlayGamesPlatform.Instance.IsAuthenticated()) PlayGamesPlatform.Instance.SignOut();
        else InitGoogle();

        CancelInvoke();
        Invoke(nameof(LoginTimeOut), 10);

        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (SignInStatus result) =>
        {
            if (result == SignInStatus.Success) PlayfabGoogle(PlayGamesPlatform.Instance.GetServerAuthCode());
        });
    }


    public void GuestButton()
    {
        Message.instance.Show("Playing Offline", Color.white);
        PlayerPrefs.SetInt("LoggedIn", 0);
        LoadGame();
    }









    // private void LoginWithPlayfab(string token)
    // {
    //     Invoke("Offline", 10);
    //     PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest
    //     {
    //         TitleId = titleId,
    //         AccessToken = token,
    //         CreateAccount = true,
    //         InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
    //         {
    //             GetPlayerProfile = true
    //         }
    //     }, PlayfabLoginSucess, PlafabLoginFail);
    // }

    // private void PlayfabLoginSucess(PlayFab.ClientModels.LoginResult result)
    // {
    //     if (isGuestLogin) return;

    //     if (Application.platform == RuntimePlatform.Android)
    //     {
    //         if (FB.IsLoggedIn)
    //         {
    //             PlayerPrefs.SetString("FBToken", AccessToken.CurrentAccessToken.TokenString);
    //             UpdatePlayerData();
    //         }
    //         else FBLogin();
    //     }

    //     // Message.instance.Show("LoggedIn in FB", Color.green);
    //     PlayerPrefs.SetString("PlayfabId", result.PlayFabId);
    //     PlayerPrefs.SetInt("LoggedIn", 1);
    //     LoadGame();
    // }

    // private void PlafabLoginFail(PlayFabError error)
    // {
    //     // Message.instance.Show("FB LoggedIn Fail", Color.red);
    //     Loading.instance.Active(false);
    //     StartCoroutine(IEAutoLogin());
    // }

    // private void FBLogin()
    // {
    //     var perms = new List<string>() { "email", "gaming_user_picture" };
    //     FB.LogInWithReadPermissions(perms, OnFBLogin);
    // }

    // private void OnFBLogin(ILoginResult result)
    // {
    //     if (FB.IsLoggedIn)
    //     {
    //         PlayerPrefs.SetString("FBToken", AccessToken.CurrentAccessToken.TokenString);
    //         UpdatePlayerData();
    //     }
    //     else
    //     {
    //         DefaultScreen();
    //     }
    // }


    // private void UpdatePlayerData()
    // {
    //     FB.API("me?fields=name", HttpMethod.GET, GetFBName);
    //     FB.API("/me/picture?redirect=false", HttpMethod.GET, GetFBPic);
    // }

    // private void GetFBPic(IGraphResult result)
    // {
    //     if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
    //     {
    //         IDictionary data = result.ResultDictionary["data"] as IDictionary;
    //         string url = data["url"] as string;

    //         if (string.IsNullOrEmpty(url)) return;
    //         PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest()
    //         {
    //             ImageUrl = url
    //         }, OnSuccess => { }, OnFailed => { });
    //     }
    // }

    // private void GetFBName(IGraphResult result)
    // {
    //     string fbName = result.ResultDictionary["name"].ToString();
    //     fbName = ClampName(fbName);
    //     var request = new UpdateUserTitleDisplayNameRequest
    //     {
    //         DisplayName = fbName,
    //     };
    //     PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnSucessUpdatePlayerData => { }, OnError);
    // }

    // private string ClampName(string name)
    // {
    //     if (name.Length <= 6) return name + "    ";
    //     else if (name.Length >= 25) return name.Substring(0, 25);
    //     return name;
    // }

    // private void OnError(PlayFabError error)
    // {
    //     // Debug.Log("Playfab Error : " + error);
    // }

    // public void TryAgainButton()
    // {
    //     StartCoroutine(IEAutoLogin());
    // }

    // private void PlayfabGoogle(string token)
    // {
    //     // LoadingScreen.instance.Active();

    //     // PlayFabClientAPI.loginwith
    //     PlayFabClientAPI.LoginWithGooglePlayGamesServices(new PlayFab.ClientModels.LoginWithGooglePlayGamesServicesRequest
    //     {
    //         TitleId = titleId,
    //         ServerAuthCode = token,
    //         CreateAccount = true,
    //         InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
    //         {
    //             GetPlayerProfile = true
    //         }
    //     }, GoogleLoginSucess, PlafabLoginFail
    //     );
    // }

    // private void GoogleLoginSucess(PlayFab.ClientModels.LoginResult result)
    // {
    //     if (PlayGamesPlatform.Instance.IsAuthenticated())
    //     {
    //         PlayerPrefs.SetString("GoogleToken", PlayGamesPlatform.Instance.GetServerAuthCode());
    //         // PlayerPrefs.SetString("GoogleToken", result.SessionTicket);
    //         // UpdateGoogleData();
    //     }
    //     PlayerPrefs.SetString("PlayfabId", result.PlayFabId);
    //     PlayerPrefs.SetInt("LoggedIn", 1);
    //     LoadGame();
    // }

    // public void PlayButton()
    // {
    //     // FB.LogInWithReadPermissions(new List<string> { "email", "gaming_user_picture" }, Res =>
    //     // {
    //     //     LoginWithPlayfab(AccessToken.CurrentAccessToken.TokenString);
    //     // });

    //     // if (!txtStatus.text.Equals("Online"))
    //     // {
    //     //     Msg.instance.DisplayMsg("You are offline", Color.red);
    //     //     DefaultScreen();
    //     //     LoadingScreen.instance.Disable();
    //     //     return;
    //     // }

    //     if (PlayGamesPlatform.Instance.IsAuthenticated()) PlayGamesPlatform.Instance.SignOut();
    //     // else InitGoogle();

    //     // CancelInvoke();
    //     // Invoke(nameof(LoginTimeOut), 10);

    //     PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (SignInStatus result) =>
    //     {
    //         if (result == SignInStatus.Success) PlayfabGoogle(PlayGamesPlatform.Instance.GetServerAuthCode());
    //     });
    // }
}