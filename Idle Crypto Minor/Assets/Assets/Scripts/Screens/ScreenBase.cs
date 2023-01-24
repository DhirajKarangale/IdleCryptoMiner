using UnityEngine;

public class ScreenBase : MonoBehaviour
{
    [SerializeField] protected GameObject obj;

    public bool isActive
    {
        get { return obj.activeInHierarchy; }
    }

    public virtual void ActiveButton(bool isActive)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.clipTap);
        obj.SetActive(isActive);
    }
}
