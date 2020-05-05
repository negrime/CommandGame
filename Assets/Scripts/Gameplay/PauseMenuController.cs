using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class PauseMenuController : MonoBehaviour {
    public Player      Player;
    public EnemyMinion Boss;
    [Space]
    public GameObject PauseRoot;
    public Button     PauseContinueButton;
    public Button     PauseMenuButton;
    public Button     PauseExitButton;
    [Space]
    public GameObject WinRoot;
    public Button     WinContinueButton;
    [Space]
    public GameObject LoseRoot;
    public Button     LoseMenuButton;

    bool _isManual;

    void Start() {
        Player.DeathDetector.OnDeath += OnPlayerDied;
        Boss.DeathDetector.OnDeath   += OnBossDied;

        PauseContinueButton.onClick.AddListener(OnContinueClick);
        PauseMenuButton.onClick.AddListener(OnGoToMenuClick);
        PauseExitButton.onClick.AddListener(OnExitClick);

        WinContinueButton.onClick.AddListener(OnWinContinueClick);

        LoseMenuButton.onClick.AddListener(OnLoseMenuClick);

        WinRoot.SetActive(false);
        LoseRoot.SetActive(false);
        PauseController.OnPauseChanged += OnPauseChanged;
        OnPauseChanged(PauseController.IsPaused);
    }

    void OnDestroy() {
        PauseController.OnPauseChanged -= OnPauseChanged;
    }

    void OnPlayerDied() {
        _isManual = true;
        PauseController.IsPaused = true;
        LoseRoot.SetActive(true);
    }

    void OnBossDied() {
        _isManual = true;
        PauseController.IsPaused = true;
        WinRoot.SetActive(true);
    }

    void OnPauseChanged(bool isPaused) {
        if ( _isManual ) {
            return;
        }
        PauseRoot.SetActive(isPaused);
    }

    void OnWinContinueClick() {
        PauseController.IsPaused = false;
        WinRoot.SetActive(false);
        _isManual = false;
    }

    void OnLoseMenuClick() {
        PauseController.IsPaused = false;
        LoseRoot.SetActive(false);
        _isManual = false;
        SceneManager.LoadScene("MainMenu");
    }

    void OnContinueClick() {
        PauseController.IsPaused = false;
    }

    void OnGoToMenuClick() {
        PauseController.IsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    void OnExitClick() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
