using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public struct LeaderboardData
{
    public Sprite spriteRank;
    public string rank;
    public string name;
    public string score;
    public string avatarUrl;
}

public class LeaderboardStat : MonoBehaviour
{
    [SerializeField] Image imgRank;
    [SerializeField] RawImage imgPlayer;
    [SerializeField] TMP_Text txtRank;
    [SerializeField] TMP_Text txtScore;
    [SerializeField] Text txtName;

    private string lastAvatarUrl = "";

    private IEnumerator SetImage(string url)
    {
        if (string.IsNullOrEmpty(url)) yield break;

        Texture2D UserPicture = new Texture2D(32, 32);
        WWW www = new WWW(url);
        yield return www;

        www.LoadImageIntoTexture(UserPicture);
        www.Dispose();
        www = null;

        imgPlayer.texture = UserPicture;
        lastAvatarUrl = url;
    }

    public void SetData(LeaderboardData data)
    {
        if (data.spriteRank)
        {
            imgRank.enabled = true;
            imgRank.sprite = data.spriteRank;
            txtRank.gameObject.SetActive(false);
        }
        else
        {
            txtRank.text = data.rank;
            txtRank.gameObject.SetActive(true);
            imgRank.enabled = false;
        }

        txtName.text = data.name;
        txtScore.text = data.score;

        if (!lastAvatarUrl.Equals(data.avatarUrl) && this.gameObject.activeInHierarchy)
        {
            StartCoroutine(SetImage(data.avatarUrl));
        }

    }
}
