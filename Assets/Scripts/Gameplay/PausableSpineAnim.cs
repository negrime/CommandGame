using UnityEngine;

using Spine.Unity;

[RequireComponent(typeof(SkeletonAnimation))]
public sealed class PausableSpineAnim : MonoBehaviour {
    SkeletonAnimation _skeletonAnimation;

    void Start() {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();

        PauseController.OnPauseChanged += OnPauseChanged;
    }

    void OnDestroy() {
        PauseController.OnPauseChanged -= OnPauseChanged;
    }

    void OnPauseChanged(bool isPaused) {
        _skeletonAnimation.timeScale = isPaused ? 0f : 1f;
    }
}
