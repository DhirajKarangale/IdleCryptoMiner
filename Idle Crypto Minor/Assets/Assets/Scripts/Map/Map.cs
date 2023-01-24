using UnityEngine;

public class Map : ScreenBase
{
    [SerializeField] internal GameObject objIslandComplete;
    [SerializeField] Island[] islands;
    private RoomManager roomManager;
    internal int currIsland;
    private SoundManager soundManager;

    public bool isExtraActive
    {
        get { return objIslandComplete.activeInHierarchy; }
    }

    private void Start()
    {
        roomManager = GameManager.instance.roomManager;
        soundManager = SoundManager.instance;

        DataManager.OnDataReceived += DataReceived;
    }

    private void OnDestroy()
    {
        DataManager.OnDataReceived -= DataReceived;
    }

    private void OnDisable()
    {
        DataManager.OnDataReceived -= DataReceived;
    }

    private void DataReceived(DataManager dataManager)
    {
        currIsland = dataManager.mapLevel;
        if (currIsland <= 0) currIsland = 1;
        SetIsland();
    }

    private void SetIsland()
    {
        for (int i = 0; i < islands.Length; i++)
        {
            if (i + 1 <= currIsland)
            {
                islands[i].UnLock();
            }
            else
            {
                islands[i].Lock();
            }
        }
    }

    public void IslandComplete()
    {
        objIslandComplete.SetActive(true);
        currIsland++;
        SetIsland();
    }

    public void NextislandButton()
    {
        soundManager.PlaySound(soundManager.clipMeta);
        roomManager.ResetRooms();
        objIslandComplete.SetActive(false);
    }

    public void Desable()
    {
        objIslandComplete.SetActive(false);
        ActiveButton(false);
    }
}
