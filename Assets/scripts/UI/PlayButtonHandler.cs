using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayButtonHandler : MonoBehaviour
{
    public Button playButton;
    public TMP_Text warnText;                      // text cảnh báo ngay dưới nút Play
    public ButtonHighlighter upUnitHL;       // KÉO nút UpUnit (có script Highlighter) vào đây
    public string sceneName = "lv1";

    void Start()
    {
        warnText.gameObject.SetActive(false);
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(OnClickPlay);
    }

    void OnClickPlay()
    {
        int idx = RosterManager.Instance.FirstEmptySlot(); // -1 nếu đã đủ trong phạm vi slot đã mở
        if (idx != -1)
        {
            warnText.gameObject.SetActive(true);
            if (warnText) warnText.text = $"Bạn cần chọn unit cho Slot #{idx + 1} trước khi chơi.";
            if (upUnitHL) upUnitHL.PulseOnce();            // 🔔 highlight nút UpUnit

            return;
        }

        // đủ unit → vào màn
        if (warnText) warnText.text = "";
        SceneManager.LoadScene(sceneName);
    }

    void OnDisable()
    {
        if (warnText != null)
            warnText.gameObject.SetActive(false);
    }

}
