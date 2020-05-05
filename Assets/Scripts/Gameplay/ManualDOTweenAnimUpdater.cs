using UnityEngine;

using DG.Tweening;

public sealed class ManualDOTweenAnimUpdater : MonoBehaviour {
    static ManualDOTweenAnimUpdater _instance;

    void Awake() {
        if ( _instance && (_instance != this) ) {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy() {
        if ( _instance && (_instance == this) ) {
            _instance = null;
        }
    }

    void Update() {
        DOTween.ManualUpdate(PauseController.CurDeltaTime, Time.deltaTime);
    }
}
