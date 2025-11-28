using UnityEngine;
using UnityEngine.UI;

public class SelectedUpgradeItem : MonoBehaviour
{
    [Header("UI")]
    public Image icon;

    [Header("Star visuals")]
    public Image[] starImages;      // kéo vào 5 Image (từ trái qua phải)
    public Sprite starOn;          // sprite sao sáng
    public Sprite starOff;         // sprite sao mờ

    [Header("Options")]
    public bool useNativeIconSize = true;

    public static object Instance { get; internal set; }

    public void SetData(UpgradeSO up)
    {
        if (up == null) return;
        if (icon != null) icon.sprite = up.icon;
    }

    // set số sao (0..maxStars)
    public void SetStars(int starCount)
    {
        if (starImages == null || starImages.Length == 0) return;

        for (int i = 0; i < starImages.Length; i++)
        {
            var img = starImages[i];
            if (img == null) continue;

            if (starCount > i) // ví dụ starCount=3 => i=0,1,2 là on
            {
                if (starOn != null) img.sprite = starOn;
                img.enabled = true;
            }
            else
            {
                if (starOff != null) img.sprite = starOff;
                img.enabled = true; // bật để giữ layout (nếu muốn ẩn, set false)
            }
        }
    }
}
