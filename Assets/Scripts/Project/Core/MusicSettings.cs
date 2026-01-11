using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MusicSettings", menuName = "Audio/Music Settings")]
public class MusicSettings : ScriptableObject
{
    [System.Serializable]
    public class LevelMusicRange
    {
        public int startLevel = 1;
        public int endLevel = 4;
        public AudioClip musicClip;
    }

    public List<LevelMusicRange> levelMusicRanges = new List<LevelMusicRange>();
    public AudioClip defaultMusic;
    public AudioClip menuMusic;
}