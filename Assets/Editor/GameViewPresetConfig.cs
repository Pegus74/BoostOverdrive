// Assets/Editor/GameViewPresetConfig.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameViewPresetsConfig", menuName = "Editor/Game View Presets Config")]
public class GameViewPresetConfig : ScriptableObject
{
    public enum PresetGroupType
    {
        Standalone = 0,
        WebPlayer = 1,
        iOS = 4,
        Android = 5,
        PS4 = 6,
        XboxOne = 7,
        WindowsStore = 8
    }

    [Serializable]
    public class Preset
    {
        public string displayName;
        public int width;
        public int height;
        public PresetGroupType groupType = PresetGroupType.Standalone;
    }

    public List<Preset> presets = new List<Preset>
    {
        new Preset { displayName = "PC_720p", width = 1280, height = 720 },
        new Preset { displayName = "PC_UltraWide_1440p", width = 3440, height = 1440 },
        new Preset { displayName = "PC_UltraWide_1080p", width = 2560, height = 1080 },
        new Preset { displayName = "Mobile_Notch", width = 393, height = 852 },
        new Preset { displayName = "Mobile_Tall", width = 430, height = 932 },
        new Preset { displayName = "Tablet_iPad_Classic", width = 1024, height = 768 }
    };
}