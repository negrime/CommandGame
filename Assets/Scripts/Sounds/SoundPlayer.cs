using UnityEngine;

public sealed class SoundPlayer : MonoBehaviour {
    const float BaseDistance = 50f;
    
    public AudioSource AudioSource;
    public float       OverrideDistance;

    public bool IsPlaying => AudioSource.isPlaying;

    void Reset() {
        AudioSource = GetComponent<AudioSource>();
        if ( !AudioSource ) {
            AudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start() {
        AudioSource.spatialBlend = 1f;
        AudioSource.rolloffMode  = AudioRolloffMode.Linear;
        AudioSource.minDistance  = 1f;
        AudioSource.maxDistance  = (OverrideDistance > 0f) ? OverrideDistance : BaseDistance;
    }

    public void PlayOneShot(string key, float volume = 1f) {
        PlayOneShot(SoundsManager.Instance.GetClip(key), volume);
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f) {
        if ( !clip ) {
            Debug.LogWarning("Clip is null");
            return;
        }
        if ( AudioSource.isPlaying ) {
            AudioSource.Stop();
            return;
        }
        AudioSource.PlayOneShot(clip, volume);
    }
}
