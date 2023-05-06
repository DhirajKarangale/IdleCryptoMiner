using PlayFab;
using System.IO;
using UnityEngine;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    private string saveCloudDB;
    private string saveLocalBD;
    private bool isCloudLoadTried;
    private bool isLoggedIn;
    private bool isCloudDataLoaded;

    private DataManager dataManager;

    private void Start()
    {
        dataManager = GameManager.instance.dataManager;
        saveLocalBD = Application.persistentDataPath + "TestData.DK";
        saveCloudDB = "TestData";
        isCloudLoadTried = false;
        isCloudDataLoaded = false;

        isLoggedIn = PlayerPrefs.GetInt("LoggedIn", 0) == 1;
        // File.Delete(saveLocalBD);

        Load();

        Application.quitting += OnQuit;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Menu)) Save();
    }





    private void OnDestroy()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }

    private void OnApplicationPause(bool isPause)
    {
        if (isPause)
        {
            Save();
        }
    }

    private void OnApplicationFocus(bool isFocus)
    {
        if (!isFocus)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnQuit()
    {
        Save();
    }



    public void Save()
    {
        DataItems dataItems = dataManager.GetData();
        SaveLocal(dataItems);
        SaveTime();
        if (isLoggedIn && isCloudDataLoaded) SaveCloud(dataItems);
    }

    private void SaveTime()
    {
        if (isLoggedIn)
        {
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
            {
                {"AddRoomRemainTime", GameManager.instance.roomManager.currTime.ToString()},
                {"CloseTime", System.DateTime.Now.ToString()},
            }
            };

            PlayFabClientAPI.UpdateUserData(request, OntimeSave => { }, OnError => { });
        }
        PlayerPrefs.SetFloat("AddRoomRemainTime", GameManager.instance.roomManager.currTime);
        PlayerPrefs.SetString("CloseTime", System.DateTime.Now.ToString());
    }

    private void LoadTime()
    {
        if (isLoggedIn) PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnTimeReceived, OnError => { });
        else dataManager.SetTime();
    }

    private void OnTimeReceived(GetUserDataResult result)
    {
        if ((result.Data != null) && result.Data.ContainsKey("AddRoomRemainTime") && result.Data.ContainsKey("CloseTime"))
        {
            dataManager.SetTime(result.Data["CloseTime"].Value, result.Data["AddRoomRemainTime"].Value);
        }
    }

    private void Load()
    {
        LoadTime();
        if (isLoggedIn) LoadCloudData();
        else LoadLocalData();
    }

    private void SaveLocal(DataItems dataItems)
    {
        File.WriteAllText(saveLocalBD, JsonUtility.ToJson(dataItems));
        // Debug.Log("Local Data Saved Sucessfully");
    }

    private void SaveCloud(DataItems dataItems)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {saveCloudDB,JsonUtility.ToJson(dataItems)}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSave => {/* Debug.Log("Cloud Data Saves Sucessfully"); */ }, OnError => { /* Debug.Log("Data Not Saves"); */ });
    }

    private void LoadLocalData()
    {
        if (File.Exists(saveLocalBD))
        {
            string fileContents = File.ReadAllText(saveLocalBD);
            DataItems dataItems = JsonUtility.FromJson<DataItems>(fileContents);
            dataManager.SetData(dataItems);
            isCloudDataLoaded = false;
           // Debug.Log("Local Data Loaded");
        }
        else
        {
            isCloudDataLoaded = true;
            dataManager.SetData();
        }
    }

    private void LoadCloudData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecived, OnCloudDataError =>
        {
            if (isCloudLoadTried)
            {
                LoadLocalData();
            }
            else
            {
                isCloudLoadTried = true;
                LoadCloudData();
            }
        });
    }

    private void OnDataRecived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey(saveCloudDB))
        {
            isCloudDataLoaded = true;
            // Debug.Log("Cloud Data load Sucessfully");
            DataItems dataItems = JsonUtility.FromJson<DataItems>(result.Data[saveCloudDB].Value);
            dataManager.SetData(dataItems);
        }
        else
        {
            LoadLocalData();
        }
    }
}
