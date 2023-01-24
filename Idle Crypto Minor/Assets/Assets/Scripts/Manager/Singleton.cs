using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T Instance;
    public static T instance
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<T>();
                if (Instance == null)
                {
                    GameObject newGO = new GameObject();
                    Instance = newGO.AddComponent<T>();
                }
            }

            return Instance;
        }
    }

    protected virtual void Awake()
    {
        Instance = this as T;
    }
}