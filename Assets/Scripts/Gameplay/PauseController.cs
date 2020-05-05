using UnityEngine;

using System;

public static class PauseController {
    static bool _isPaused;

    public static event Action<bool> OnPauseChanged;

    public static bool IsPaused {
        get => _isPaused;
        set {
            if ( _isPaused == value ) {
                return;
            }
            _isPaused = value;
            OnPauseChanged?.Invoke(_isPaused);
        }
    }

    public static float CurDeltaTime => IsPaused ? 0f : Time.deltaTime;
}
