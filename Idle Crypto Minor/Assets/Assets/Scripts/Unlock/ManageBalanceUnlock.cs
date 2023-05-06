using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ManageBalanceUnlock : MonoBehaviour {

    public Sprite adBtnSprite;
    public Sprite doneBtnSprite;
    public Image[] adImage=new Image[5];
    [SerializeField]GameObject manageBalanceLock;
    [SerializeField]GameObject manageBalanceUnlock;
    public TextMeshProUGUI adCountText;
   
    public TextMeshProUGUI unlockTimeText;
    public TextMeshProUGUI remainingDaysText;
    public Button[] reduceOneDayButtons = new Button[5];

    private bool isFirstTime;
    private DateTime unlockTime;
    private bool isObjectUnlocked = false;
    private TimeSpan remainingTime;
    private int buttonPressCount = 0;

    private int adCount=5;
    private int remainingDaysCount=7;

    void Start () {

        
        isFirstTime = PlayerPrefs.GetInt("isFirstTime", 1) == 1;
        if (isFirstTime) {
            Debug.Log("First time installation");
            PlayerPrefs.SetInt("isFirstTime", 0);
            unlockTime = DateTime.Now.AddDays(7);
            manageBalanceLock.SetActive(true);
            manageBalanceUnlock.SetActive(false);
        } else {
            Debug.Log("App was previously installed");
            string unlockTimeString = PlayerPrefs.GetString("unlockTime", "");
            if (DateTime.TryParse(unlockTimeString, out unlockTime)) {
                Debug.Log("Unlock time: " + unlockTime);
            } else {
                Debug.LogError("Invalid unlock time: " + unlockTimeString);
                unlockTime = DateTime.Now.AddDays(7);
            }
        }

        remainingTime = unlockTime - DateTime.Now;
        SetUnlockTimeText();

        // Add listeners to the reduceOneDayButtons
        for (int i = 0; i < reduceOneDayButtons.Length; i++) {
            int buttonIndex = i;
            reduceOneDayButtons[i].onClick.AddListener(() => ReduceOneDay(buttonIndex));
        }

       
    }

    void Update() {
        remainingTime = unlockTime - DateTime.Now;
        if (!isObjectUnlocked && remainingTime.TotalSeconds <= 0) {
            UnlockObject();
        }
        SetUnlockTimeText();
    }

    void OnApplicationQuit() {
        if (!isObjectUnlocked) {
            PlayerPrefs.SetString("unlockTime", unlockTime.ToString());
        }
    }

    void UnlockObject() {
        Debug.Log("Object unlocked!");
        isObjectUnlocked = true;
    }

    void SetUnlockTimeText() {
        if (remainingTime.TotalSeconds > 0) {
            string unlockDateString = unlockTime.ToString("dd") + GetDaySuffix(unlockTime.Day) + unlockTime.ToString(" MMMM yyyy");
            int days = remainingTime.Days;
            string suffix = GetDaySuffix(days);
            string remainingTimeString = string.Format("{0} days remaining for unlock", days);
            unlockTimeText.text =  unlockDateString;
            remainingDaysText.text = remainingTimeString;
             string remainingDay = string.Format("Watch {0} videos to reduce 1 unlock day({1}/7)", adCount,(remainingDaysCount-days));
             adCountText.text=remainingDay;
            
        } else {
         
            manageBalanceLock.SetActive(false);
            manageBalanceUnlock.SetActive(true);
        }
    }

    string GetDaySuffix(int day) {
        switch (day % 10) {
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

    public void ReduceOneDay(int buttonIndex) {
        if (!isObjectUnlocked && remainingTime.TotalSeconds > 0) {
                buttonPressCount++;
                adCount=5;
                adCount-=buttonPressCount;
            if (buttonPressCount >= 5) {
                unlockTime = unlockTime.AddDays(-1);
                remainingTime = unlockTime - DateTime.Now;
                SetUnlockTimeText();
                buttonPressCount = 0;
                adCount=5;
                for (int i = 0; i < reduceOneDayButtons.Length; i++) {
                    reduceOneDayButtons[i].interactable = true;
                    adImage[i].sprite=adBtnSprite;
                }
            } else {
                reduceOneDayButtons[buttonIndex].interactable = false;
                adImage[buttonIndex].sprite=doneBtnSprite;
            }
        }
    }
}