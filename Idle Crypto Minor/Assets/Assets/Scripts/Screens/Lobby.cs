using PlayFab;
using UnityEngine;
using Facebook.Unity;
using System.Collections;
using PlayFab.ClientModels;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Lobby : MonoBehaviour
{
    [SerializeField] GameObject objFBButton;
    [SerializeField] GameObject objTryAgainButton;

    private const string titleId = "55B2F";

    private bool isLogginTried;
    private bool isGuestLogin;

    [SerializeField] GiftsDB db;
    [SerializeField] Sprite[] sprites;

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            db.gifts[i].sprite = sprites[i];
        }

        isGuestLogin = false;
        isLogginTried = false;
        DefaultScreen();
        StartCoroutine(IEAutoLogin());

        if (FB.IsInitialized) return;
        FB.Init(() => FB.ActivateApp());
    }

    private IEnumerator IEAutoLogin()
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            StopAllCoroutines();
            Offline();
        }
        else
        {
            // Message.instance.Show("Online", Color.green);
            if (!isLogginTried && PlayerPrefs.HasKey("FBToken")) AutoLogin();
            else DefaultScreen();
        }
    }

    private void Offline()
    {
        Message.instance.Show("You are Offline", Color.red);
        objFBButton.SetActive(false);
        objTryAgainButton.SetActive(true);
        Loading.instance.Active(false);
    }

    private void DefaultScreen()
    {
        objFBButton.SetActive(true);
        objTryAgainButton.SetActive(false);
        Loading.instance.Active(false);
    }

    private void AutoLogin()
    {
        Loading.instance.Active(true);
        LoginWithPlayfab(PlayerPrefs.GetString("FBToken"));
        isLogginTried = true;
        // Message.instance.Show("Logging in", Color.green);
    }

    private void LoginWithPlayfab(string token)
    {
        Invoke("Offline", 10);
        PlayFabClientAPI.LoginWithFacebook(new PlayFab.ClientModels.LoginWithFacebookRequest
        {
            TitleId = titleId,
            AccessToken = token,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        }, PlayfabLoginSucess, PlafabLoginFail);
    }

    private void PlayfabLoginSucess(PlayFab.ClientModels.LoginResult result)
    {
        if (isGuestLogin) return;

        if (Application.platform == RuntimePlatform.Android)
        {
            if (FB.IsLoggedIn)
            {
                PlayerPrefs.SetString("FBToken", AccessToken.CurrentAccessToken.TokenString);
                UpdatePlayerData();
            }
            else FBLogin();
        }

        // Message.instance.Show("LoggedIn in FB", Color.green);
        PlayerPrefs.SetString("PlayfabId", result.PlayFabId);
        PlayerPrefs.SetInt("LoggedIn", 1);
        LoadGame();
    }

    private void PlafabLoginFail(PlayFabError error)
    {
        // Message.instance.Show("FB LoggedIn Fail", Color.red);
        Loading.instance.Active(false);
        StartCoroutine(IEAutoLogin());
    }

    private void FBLogin()
    {
        var perms = new List<string>() { "email", "gaming_user_picture" };
        FB.LogInWithReadPermissions(perms, OnFBLogin);
    }

    private void OnFBLogin(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            PlayerPrefs.SetString("FBToken", AccessToken.CurrentAccessToken.TokenString);
            UpdatePlayerData();
        }
        else
        {
            DefaultScreen();
            Debug.Log("User cancelled login");
        }
    }


    private void UpdatePlayerData()
    {
        FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, GetFBName);
        FB.API("/me/picture?redirect=false", HttpMethod.GET, GetFBPic);
    }

    private void GetFBPic(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
        {
            IDictionary data = result.ResultDictionary["data"] as IDictionary;
            string url = data["url"] as string;

            if (string.IsNullOrEmpty(url)) return;
            PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest()
            {
                ImageUrl = url
            }, OnSuccess => { }, OnFailed => { });
        }
    }

    private void GetFBName(Facebook.Unity.IGraphResult result)
    {
        string fbName = result.ResultDictionary["name"].ToString();
        fbName = ClampName(fbName);
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = fbName,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnSucessUpdatePlayerData => { }, OnError);
    }

    private string ClampName(string name)
    {
        if (name.Length <= 6) return name + "    ";
        else if (name.Length >= 25) return name.Substring(0, 25);
        return name;
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log("Playfab Error : " + error);
    }

    private void LoadGame()
    {
        Loading.instance.LoadLevel(1);
    }


    public void GuestButton()
    {
        isGuestLogin = true;
        Message.instance.Show("Playing Offline", Color.white);
        PlayerPrefs.SetInt("LoggedIn", 0);
        LoadGame();
    }

    public void TryAgainButton()
    {
        StartCoroutine(IEAutoLogin());
    }

    public void PlayButton()
    {
        FB.LogInWithReadPermissions(new List<string> { "email", "gaming_user_picture" }, Res =>
        {
            LoginWithPlayfab(AccessToken.CurrentAccessToken.TokenString);
        });
    }
}
