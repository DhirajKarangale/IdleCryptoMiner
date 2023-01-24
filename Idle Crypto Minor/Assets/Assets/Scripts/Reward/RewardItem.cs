using TMPro;
using UnityEngine;

public class RewardItem : MonoBehaviour
{
    [Header("Completed")]
    [SerializeField] GameObject objCompleted;
    [SerializeField] TMP_Text txtCompDay;
    [SerializeField] TMP_Text txtCompCoin;

    [Header("NotCompleted")]
    [SerializeField] GameObject objNotCompleted;
    [SerializeField] TMP_Text txtNotCompDay;
    [SerializeField] TMP_Text txtNotCompCoin;

    public void SetData(bool isCompleted, int coin, int day)
    {
        txtCompCoin.text = coin.ToString();
        txtCompDay.text = $"Day{day}";
        objCompleted.SetActive(isCompleted);

        txtNotCompCoin.text = coin.ToString();
        txtNotCompDay.text = $"Day{day}";
        objNotCompleted.SetActive(!isCompleted);
    }
}
