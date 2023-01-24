using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] internal GameObject obj;
    [SerializeField] Transform circle;
    [SerializeField] float rotateSpeed;

    public static Loading instance = null;
    public static Loading Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private IEnumerator IERotateCircle()
    {
        while (true)
        {
            circle.Rotate(0, 0, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }


    public void LoadLevel(int level)
    {
        Active(true);
        SceneManager.LoadScene(level);
        // SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void Active(bool isActive)
    {
        obj.SetActive(isActive);
        StopAllCoroutines();
        if (isActive) StartCoroutine(IERotateCircle());
    }

}
