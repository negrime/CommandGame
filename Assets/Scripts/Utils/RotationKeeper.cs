using UnityEngine;

public sealed class RotationKeeper : MonoBehaviour {
    public Vector3 LocalAngle;

    void Update() {
        transform.localEulerAngles = LocalAngle;
    }
}
