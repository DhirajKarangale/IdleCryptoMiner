using UnityEngine;
using System.Collections;

public class Menu : ScreenBase
{
    private bool isFocous;
    private bool isProcessing;

    private void OnApplicationFocus(bool focus)
    {
        isFocous = focus;
    }

    private IEnumerator IEInviteFriends()
    {
        string shareSubject = "This game is awesome and also gives little bit knowladge about crypto mining";
        string shareMessage = "Nice game about crypto mining have a try and also share if you like \nHeres the game link if you want : https://play.google.com/store/apps/details?id=com.playhobo.idlecryptominer";
        isProcessing = true;
        if (!Application.isEditor)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            //put text and subject extra
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

            //call createChooser method of activity class
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Make your Friend a Crypto Miner");
            currentActivity.Call("startActivity", chooser);
        }

        yield return new WaitUntil(() => isFocous);
        isProcessing = false;
    }

    public void InviteFriends()
    {
        if (!isProcessing)
        {
            StartCoroutine(IEInviteFriends());
            ActiveButton(false);
        }
    }

    public void LinkButton(string link)
    {
        Application.OpenURL(link);
        ActiveButton(false);
    }

    public void SoundButton()
    {
        SoundManager.instance.ActiveButton(true);
        ActiveButton(false);
    }
}
