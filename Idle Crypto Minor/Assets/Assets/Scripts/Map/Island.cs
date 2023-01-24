using UnityEngine;
using UnityEngine.UI;

public class Island : MonoBehaviour
{
    [SerializeField] Material grascale;
    [SerializeField] Image imgIsland;
    [SerializeField] Image imgLine;

    public void Lock()
    {
        imgIsland.material = grascale;
        imgLine.color = new Color(1, 1, 1, 0.5f);
    }

    public void UnLock()
    {
        imgIsland.material = null;
        imgLine.color = Color.white;
    }
}
