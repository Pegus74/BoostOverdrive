// Assets/Editor/GameViewPresetInitializer.cs
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GameViewPresetInitializer
{
    static GameViewPresetInitializer()
    {
        EditorApplication.delayCall += Initialize;
    }

    private static void Initialize()
    {
        if (!SessionState.GetBool("GAME_VIEW_PRESETS_INITIALIZED", false))
        {
            // Создаем конфиг если не существует
            var config = AssetDatabase.LoadAssetAtPath<GameViewPresetConfig>("Assets/Editor/GameViewPresetConfig.asset");
            if (config == null)
            {
                var newConfig = ScriptableObject.CreateInstance<GameViewPresetConfig>();
                AssetDatabase.CreateAsset(newConfig, "Assets/Editor/GameViewPresetConfig.asset");
                AssetDatabase.SaveAssets();
            }

            SessionState.SetBool("GAME_VIEW_PRESETS_INITIALIZED", true);
        }
    }
}