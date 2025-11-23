using UnityEngine;

public class ResetProgress : MonoBehaviour
{
    [ContextMenu("Reset PlayerPrefs")]
    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("✅ Tất cả dữ liệu đã được reset!");
    }
}
