using UnityEngine;

using System;

public sealed class BossDeathDetector : MonoBehaviour {
    public event Action OnDeath;
    
    public void Win() {
        OnDeath?.Invoke();
    }
}
