using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections;

public class InstallStatus : MonoBehaviour
{
    [SerializeField] NetWorth netWorthAd;
    public TextMeshProUGUI[] videoDoneText;
    public Sprite adBtnSprite;
    public Sprite doneBtnSprite;
    public Image[] adImage = new Image[5];

    [SerializeField] GameObject homeBG;
    [SerializeField] GameObject manageBalanceLock;
    [SerializeField] GameObject manageBalanceUnlock;
    public TextMeshProUGUI adCountText;
    //public TextMeshProUGUI DayCountText;
    public TextMeshProUGUI unlockTimeText;
    public TextMeshProUGUI remainingDaysText;
    public Button[] buttons = new Button[5];
    private bool isFirstTime;
    private DateTime unlockTime;
    private bool isObjectUnlocked;
    private TimeSpan remainingTime;
    private int adCount = 5;
    private int remainingDaysCount = 7;

    private string buttonInteractionsKey = "buttonInteractions";
    private int buttonInteractionCount = 0;

    public float checkInterval = 60f; // How often to check for the end of the day in seconds
    private DateTime lastCheckTime; // The last time the end of the day was checked




    void Start()
    {
        //PlayerPrefs.DeleteKey("isFirstTime");

        isFirstTime = (PlayerPrefs.GetInt("isFirstTime", 1) == 1);
        isObjectUnlocked = (PlayerPrefs.GetInt("isUnlocked", 0) == 1);
        if (isFirstTime)
        {

            Debug.Log("First time installation");
            PlayerPrefs.SetInt("isUnlocked", 0);
            isObjectUnlocked = (PlayerPrefs.GetInt("isUnlocked", 0) == 1);
            PlayerPrefs.SetInt("isFirstTime", 0);
            unlockTime = DateTime.Now.AddDays(7);
            remainingTime = unlockTime - DateTime.Now;
            SetUnlockTimeText();
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("App was previously installed");
            string unlockTimeString = PlayerPrefs.GetString("unlockTime", "");
            if (DateTime.TryParse(unlockTimeString, out unlockTime))
            {
                Debug.Log("Unlock time: " + unlockTime);
                remainingTime = unlockTime - DateTime.Now;

                SetUnlockTimeText();
                if (PlayerPrefs.HasKey(buttonInteractionsKey))
                {
                    string buttonInteractions = PlayerPrefs.GetString(buttonInteractionsKey);
                    char[] delimiter = { ',' };
                    string[] buttonInteractionArray = buttonInteractions.Split(delimiter);
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        buttons[i].interactable = (buttonInteractionArray[i] == "1") ? false : true;
                        buttonInteractionCount += (buttonInteractionArray[i] == "1") ? 1 : 0;
                        adCount = 5;
                        adCount -= buttonInteractionCount;
                        SetUnlockTimeText();
                        if (buttons[i].interactable == false)
                        {
                            adImage[i].sprite = doneBtnSprite;
                            videoDoneText[i].text = "Done";
                        }
                        else
                        {
                            adImage[i].sprite = adBtnSprite;
                            videoDoneText[i].text = "Video " + (i + 1);
                        }


                    }
                }
                else
                {
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        buttons[i].interactable = true;
                    }
                }
            }
            else
            {
                Debug.LogError("Invalid unlock time: " + unlockTimeString);
                unlockTime = DateTime.Now.AddDays(7);
                remainingTime = unlockTime - DateTime.Now;
                SetUnlockTimeText();
            }
        }


    }

    void Update()
    {

        if (DateTime.Now.Subtract(lastCheckTime).TotalSeconds >= checkInterval)
        {
            lastCheckTime = DateTime.Now;
            if (DateTime.Now.TimeOfDay >= new TimeSpan(23, 59, 59))
            {
                ShowDayEndMessage();
            }
        }
        // Check if all buttons have been interacted with
        if (buttonInteractionCount == buttons.Length)
        {
            unlockTime = unlockTime.AddDays(-1);
            remainingTime = unlockTime - DateTime.Now;
            SetUnlockTimeText();
            adImage[4].sprite = doneBtnSprite;
            videoDoneText[4].text = "Done";

            StartCoroutine(AllButtonPressed());
            buttonInteractionCount = 0;
            adCount = 5;
            adCount -= buttonInteractionCount;
            SetUnlockTimeText();
            PlayerPrefs.DeleteKey(buttonInteractionsKey);
        }


        remainingTime = unlockTime - DateTime.Now;
        if (!isObjectUnlocked && remainingTime.TotalSeconds <= 0)
        {
            UnlockObject();
        }
        // SetUnlockTimeText();
    }


    void OnApplicationQuit()
    {
        //PlayerPrefs.SetString("unlockTime", unlockTime.ToString());
        if (!isObjectUnlocked)
        {
            // Debug.Log("Quit game");
            PlayerPrefs.SetString("unlockTime", unlockTime.ToString());
            PlayerPrefs.Save();
        }
    }

    void UnlockObject()
    {
        Debug.Log("Object unlocked!");
        PlayerPrefs.SetInt("isUnlocked", 1);
        PlayerPrefs.Save();
        isObjectUnlocked = true;

    }

    void SetUnlockTimeText()
    {

        if (remainingTime.TotalSeconds > 0 && !isObjectUnlocked)
        {
            string unlockDateString = unlockTime.ToString("dd") + GetDaySuffix(unlockTime.Day) + unlockTime.ToString(" MMMM yyyy");
            int days = remainingTime.Days;

            string suffix = GetDaySuffix(days);
            string remainingTimeString = string.Format("{0} days remaining for unlock", days);
            unlockTimeText.text = unlockDateString;
            remainingDaysText.text = remainingTimeString;

            adCount = 5;
            adCount -= buttonInteractionCount;

            Debug.Log("adCount" + adCount);
            Debug.Log("interaction" + buttonInteractionCount);
            string remainingDay = string.Format("Watch {0} videos to reduce 1 unlock day({1}/7)", adCount, (remainingDaysCount - days));
            adCountText.text = remainingDay;
        }
        else
        {
            //Debug.Log("unlocked");
            manageBalanceLock.SetActive(false);
            manageBalanceUnlock.SetActive(true);
        }
    }

    string GetDaySuffix(int day)
    {
        switch (day % 10)
        {
            case 1:
                return "st";
            case 2:
                return "nd";
            case 3:
                return "rd";
            default:
                return "th";
        }
    }

    public void ReduceOneDay(int buttonIndex)
    {
        if (!isObjectUnlocked && remainingTime.TotalSeconds > 0)
        {
            // Set the interactable property of the button that was interacted with to false
            buttons[buttonIndex].interactable = false;
            buttonInteractionCount++;
            SetUnlockTimeText();
            netWorthAd.ShowAd(buttonIndex);
            // Save the updated button interactions to PlayerPrefs
            string buttonInteractions = "";
            for (int i = 0; i < buttons.Length; i++)
            {
                buttonInteractions += (buttons[i].interactable) ? "0" : "1";
                if (i < buttons.Length - 1)
                {
                    buttonInteractions += ",";
                }
            }
            PlayerPrefs.SetString(buttonInteractionsKey, buttonInteractions);
            PlayerPrefs.Save();

            if (buttonInteractionCount != 5)
            {
                StartCoroutine(AdShowUnlock(buttonIndex));
            }
        }
    }

    public void ManageButton()
    {
        if (isObjectUnlocked)
        {
            homeBG.SetActive(false);
            manageBalanceLock.SetActive(false);
            manageBalanceUnlock.SetActive(true);
        }
        else
        {
            homeBG.SetActive(false);
            manageBalanceLock.SetActive(true);
            manageBalanceUnlock.SetActive(false);
        }
    }

    IEnumerator AllButtonPressed()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
            adImage[i].sprite = adBtnSprite;
            videoDoneText[i].text = "Video " + (i + 1);

        }

    }
    IEnumerator AdShowUnlock(int buttonIndex)
    {
        yield return new WaitForSeconds(1f);
        adImage[buttonIndex].sprite = doneBtnSprite;
        videoDoneText[buttonIndex].text = "Done";
    }

    void ShowDayEndMessage()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
            adImage[i].sprite = adBtnSprite;
            videoDoneText[i].text = "Video " + (i + 1);
        }
        buttonInteractionCount = 0;
        PlayerPrefs.DeleteKey(buttonInteractionsKey);

    }

}

