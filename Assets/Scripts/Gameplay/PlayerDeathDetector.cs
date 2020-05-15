using System;

using UnityEngine;

public sealed class PlayerDeathDetector : MonoBehaviour {
    public event Action OnDeath;
    
    public void Lose() 
    {
        OnDeath?.Invoke();
    }
}
