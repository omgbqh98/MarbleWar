// PauseSystem.cs
using UnityEngine;
using System;

/// <summary>
/// Hệ thống pause dạng stack.
/// Gọi PauseSystem.PushPause() để pause (nhiều nguồn có thể push).
/// Gọi PauseSystem.PopPause() để resume (chỉ resume khi tất cả nguồn đã pop).
/// </summary>
public static class PauseSystem
{
    // Số lượng request pause hiện tại
    private static int _pauseCount = 0;

    // Lưu trạng thái time trước khi pause lần đầu
    private static float _prevTimeScale = 1f;
    private static float _prevFixedDelta = 0.02f;

    // Tùy chọn: pause audio khi pause
    public static bool PauseAudioOnPause { get; set; } = true;

    // Sự kiện khi trạng thái pause thay đổi (isPaused)
    public static event Action<bool> OnPauseStateChanged;

    /// <summary>
    /// Truyền thêm một request pause. Nếu đây là request pause đầu tiên thì game sẽ dừng (Time.timeScale = 0).
    /// </summary>
    public static void PushPause()
    {
        if (_pauseCount == 0)
        {
            // Lưu trạng thái hiện tại
            _prevTimeScale = Time.timeScale;
            _prevFixedDelta = Time.fixedDeltaTime;

            // Pause
            Time.timeScale = 0f;

            // Note: fixedDeltaTime không nên set = 0 vì có thể ảnh hưởng hệ thống physics khi resume.
            // Ta giữ nguyên previous fixed delta (chỉ lưu để restore), không cần thay đổi.
            // Nếu bạn có custom behaviour, có thể điều chỉnh ở đây.

            if (PauseAudioOnPause)
                AudioListener.pause = true;

            OnPauseStateChanged?.Invoke(true);
        }

        _pauseCount = Mathf.Max(0, _pauseCount) + 1;
    }

    /// <summary>
    /// Bỏ một request pause. Nếu đây là request cuối cùng, game sẽ resume về trạng thái trước đó.
    /// Gọi cân bằng số lần PushPause() trước đó.
    /// </summary>
    public static void PopPause()
    {
        if (_pauseCount <= 0)
        {
            // Không có pause, ignore
            _pauseCount = 0;
            return;
        }

        _pauseCount--;

        if (_pauseCount == 0)
        {
            // Restore trạng thái trước khi pause
            Time.timeScale = _prevTimeScale;
            Time.fixedDeltaTime = _prevFixedDelta;

            if (PauseAudioOnPause)
                AudioListener.pause = false;

            OnPauseStateChanged?.Invoke(false);
        }
    }

    /// <summary>
    /// Trả về true nếu đang ở trạng thái pause (có ít nhất 1 request).
    /// </summary>
    public static bool IsPaused()
    {
        return _pauseCount > 0;
    }

    /// <summary>
    /// Reset toàn bộ trạng thái pause (cẩn trọng khi dùng).
    /// </summary>
    public static void ForceResume()
    {
        _pauseCount = 0;
        Time.timeScale = _prevTimeScale;
        Time.fixedDeltaTime = _prevFixedDelta;
        if (PauseAudioOnPause) AudioListener.pause = false;
        OnPauseStateChanged?.Invoke(false);
    }

    /// <summary>
    /// Lấy current pause count (debug).
    /// </summary>
    public static int PauseCount => _pauseCount;
}
