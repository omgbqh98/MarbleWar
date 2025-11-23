using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHighlighter : MonoBehaviour
{
    public Button targetButton;          // kéo chính Button vào
    public Image background;             // ảnh nền của nút (Image)
    public Color highlightColor = new Color(1f, 0.9f, 0.2f, 1f);
    public float scaleUp = 1.08f;        // độ phóng to nhẹ
    public float pulseTime = 0.15f;      // thời gian 1 nửa nhịp
    public int pulses = 3;               // số lần nhấp nháy

    Color _origColor;
    Vector3 _origScale;
    Coroutine _co;

    void Awake()
    {
        if (!targetButton) targetButton = GetComponent<Button>();
        if (!background) background = GetComponent<Image>();
        _origScale = transform.localScale;
        _origColor = background ? background.color : Color.white;
    }

    public void PulseOnce()
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(CoPulse(pulses));
    }

    public void StopAndReset()
    {
        if (_co != null) StopCoroutine(_co);
        transform.localScale = _origScale;
        if (background) background.color = _origColor;
    }

    IEnumerator CoPulse(int pulseCount)
    {
        for (int i = 0; i < pulseCount; i++)
        {
            // lên
            float t = 0f;
            while (t < pulseTime)
            {
                t += Time.unscaledDeltaTime;
                float k = t / pulseTime;
                transform.localScale = Vector3.Lerp(_origScale, _origScale * scaleUp, k);
                if (background) background.color = Color.Lerp(_origColor, highlightColor, k);
                yield return null;
            }
            // xuống
            t = 0f;
            while (t < pulseTime)
            {
                t += Time.unscaledDeltaTime;
                float k = t / pulseTime;
                transform.localScale = Vector3.Lerp(_origScale * scaleUp, _origScale, k);
                if (background) background.color = Color.Lerp(highlightColor, _origColor, k);
                yield return null;
            }
        }
        _co = null;
    }
}
