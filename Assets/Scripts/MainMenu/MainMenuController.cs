using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenuController : MonoBehaviour
{
    public Button PlayButton;
    public Button ExitButton;

    public void OnPlayClick()
    {
        SceneManager.LoadScene(1);
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
