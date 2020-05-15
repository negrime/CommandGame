using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Create SoundsDictionary")]
public class SoundsDictionary : ScriptableObject
{
    [Serializable]
    public sealed class SoundPack
    {
        public string          Name;
        public List<AudioClip> Clips;
    }
    
    public List<SoundPack> Sounds = new List<SoundPack>();

    public AudioClip GetSound(string key)
    {
       var result =  Sounds.FirstOrDefault(sound => sound.Name == key);
       return result?.Clips[Random.Range(0, result.Clips.Count)];
    }
}
