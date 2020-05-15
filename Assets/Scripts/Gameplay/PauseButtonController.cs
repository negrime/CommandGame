using UnityEngine;
using UnityEngine.UI;

public sealed class PauseButtonController : MonoBehaviour 
{
    public Button Button;

    void Start() 
    {
        Button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick() {
        PauseController.IsPaused = true;
    }
}
