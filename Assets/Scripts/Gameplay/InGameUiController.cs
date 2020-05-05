using UnityEngine;

using TMPro;

public sealed class InGameUiController : MonoBehaviour {
    public PlayerMinionManager MinionManager;
    public TMP_Text            MinionsCountText;

    void Start() {
        MinionsCountText.text = "0";
    }

    void Update() {
        MinionsCountText.text = MinionManager.ActiveMinionsNum.ToString();
    }
}
