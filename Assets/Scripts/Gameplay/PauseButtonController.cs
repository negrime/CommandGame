using UnityEngine;
using UnityEngine.UI;

public sealed class PauseButtonController : MonoBehaviour 
{
    public Button Button;

    public void OnButtonClick() 
    {
        PauseController.IsPaused = true;
    }
}
