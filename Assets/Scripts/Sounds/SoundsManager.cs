using UnityEngine;

public sealed class SoundsManager : MonoBehaviour {
    static SoundsManager _instance;
    public static SoundsManager Instance {
        get {
            if ( !_instance ) {
                var go = new GameObject("[SoundsManager]");
                _instance = go.AddComponent<SoundsManager>();
                _instance.Init();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    SoundsDictionary _dictionary;

    void Init() {
        _dictionary = Resources.Load<SoundsDictionary>("SoundsDictionary");
        if ( !_dictionary ) {
            Debug.LogError("Can't load SoundsDictionary from Resources");
        }
    }

    public AudioClip GetClip(string key) {
        return _dictionary.GetSound(key);
    }
}
