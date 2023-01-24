using UnityEngine;

public class NetWorthItem : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text txtCount;
    [SerializeField] UnityEngine.UI.Image img;

    public void SetData(Sprite sprite, int count)
    {
        txtCount.gameObject.SetActive(count > 0);
        txtCount.text = count.ToString();
        img.sprite = sprite;
    }
}
