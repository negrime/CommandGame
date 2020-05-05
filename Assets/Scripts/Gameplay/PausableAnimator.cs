using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class PausableAnimator : MonoBehaviour {
    Animator _animator;

    void Start() {
        _animator = GetComponent<Animator>();
        PauseController.OnPauseChanged += OnPauseChanged;
    }

    void OnDestroy() {
        PauseController.OnPauseChanged -= OnPauseChanged;
    }

    void OnPauseChanged(bool isPaused) {
        _animator.speed = isPaused ? 0f : 1f;
    }
}
