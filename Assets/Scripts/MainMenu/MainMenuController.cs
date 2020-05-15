using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenuController : MonoBehaviour
{
    public Button PlayButton;
    public Button ExitButton;

    void Start() 
    {
        PlayButton.onClick.AddListener(OnPlayClick);
        ExitButton.onClick.AddListener(OnExitClick);
    }

    void OnPlayClick()
    {
        SceneManager.LoadScene(1);
    }

    void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
