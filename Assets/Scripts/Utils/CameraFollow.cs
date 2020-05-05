using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public float     ZOffset;


    private void LateUpdate() {
        if (!Target)
        {
            return;
        }
        transform.position = new Vector3(Target.position.x, transform.position.y, Target.position.z - ZOffset);
    }
}
