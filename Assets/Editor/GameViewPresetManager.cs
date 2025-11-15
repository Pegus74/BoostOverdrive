using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public class GameViewPresetManager : EditorWindow
{
    private GameViewPresetConfig config;
    private Vector2 scrollPosition;
    private string statusMessage = "";
    private MessageType statusType = MessageType.Info;

    [MenuItem("Tools/Game View Presets/Manage Presets")]
    public static void ShowWindow()
    {
        GetWindow<GameViewPresetManager>("Game View Presets");
    }

    private void OnEnable()
    {
        config = AssetDatabase.LoadAssetAtPath<GameViewPresetConfig>("Assets/Editor/GameViewPresetConfig.asset");
        if (config == null)
        {
            CreateDefaultConfig();
        }
    }

    private void CreateDefaultConfig()
    {
        config = CreateInstance<GameViewPresetConfig>();
        AssetDatabase.CreateAsset(config, "Assets/Editor/GameViewPresetConfig.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Game View Presets Configuration", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        var serializedObject = new SerializedObject(config);
        var presetsProperty = serializedObject.FindProperty("presets");
        EditorGUILayout.PropertyField(presetsProperty, true);
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.Space();
        if (GUILayout.Button("Create All Presets (Auto)"))
        {
            CreateAllPresets();
        }
        if (GUILayout.Button("Open Game View"))
        {
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
        }

        if (GUILayout.Button("Refresh Game View"))
        {
            RefreshGameView();
        }
        EditorGUILayout.Space();
        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.HelpBox(statusMessage, statusType);
        }

        EditorGUILayout.EndScrollView();
    }

    public void CreateAllPresets()
    {
        if (config == null) return;

        int successCount = 0;
        List<string> createdPresets = new List<string>();

        foreach (var preset in config.presets)
        {
            if (CreatePreset(preset))
            {
                successCount++;
                createdPresets.Add(preset.displayName);
            }
        }
        SaveGameViewSizes();

        if (successCount > 0)
        {
            statusMessage = $" Successfully created {successCount} presets: {string.Join(", ", createdPresets)}";
            statusType = MessageType.Info;
        }
        else
        {
            statusMessage = "Failed to create presets..";
            statusType = MessageType.Error;
        }

        Repaint();
    }

    private bool CreatePreset(GameViewPresetConfig.Preset preset)
    {
        try
        {
            var assembly = typeof(Editor).Assembly;
            var gameViewSizesType = assembly.GetType("UnityEditor.GameViewSizes");
            var singletonType = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizesType);
            var instanceProperty = singletonType.GetProperty("instance");
            var gameViewSizesInstance = instanceProperty.GetValue(null);

            if (gameViewSizesInstance == null)
            {
                Debug.LogError("Failed to get GameViewSizes instance");
                return false;
            }
            var currentGroupType = GetCurrentGameViewSizeGroupType();
            var getGroupMethod = gameViewSizesType.GetMethod("GetGroup");
            var group = getGroupMethod.Invoke(gameViewSizesInstance, new object[] { currentGroupType });

            if (group == null)
            {
                Debug.LogError($"Failed to get group for type: {currentGroupType}");
                return false;
            }
            if (PresetExists(group, preset.displayName, preset.width, preset.height))
            {
                Debug.Log($"Preset already exists: {preset.displayName}");
                return true;
            }
            var gameViewSizeType = assembly.GetType("UnityEditor.GameViewSize");
            var gameViewSizeTypeType = assembly.GetType("UnityEditor.GameViewSizeType");
            var fixedResolution = gameViewSizeTypeType.GetField("FixedResolution").GetValue(null);
            var constructor = gameViewSizeType.GetConstructor(new[] {
                gameViewSizeTypeType, typeof(int), typeof(int), typeof(string)
            });

            var newSize = constructor.Invoke(new object[] {
                fixedResolution, preset.width, preset.height, preset.displayName
            });
            var addCustomSizeMethod = group.GetType().GetMethod("AddCustomSize");
            addCustomSizeMethod.Invoke(group, new object[] { newSize });

            Debug.Log($" Created preset: {preset.displayName} ({preset.width}x{preset.height})");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create preset {preset.displayName}: {e}");
            return false;
        }
    }

    private bool PresetExists(object group, string name, int width, int height)
    {
        try
        {
            var getTotalCountMethod = group.GetType().GetMethod("GetTotalCount");
            int count = (int)getTotalCountMethod.Invoke(group, null);

            var assembly = typeof(Editor).Assembly;
            var gameViewSizeType = assembly.GetType("UnityEditor.GameViewSize");

            for (int i = 0; i < count; i++)
            {
                var getGameViewSizeMethod = group.GetType().GetMethod("GetGameViewSize");
                var size = getGameViewSizeMethod.Invoke(group, new object[] { i });

                var sizeName = (string)gameViewSizeType.GetField("baseText").GetValue(size);
                var sizeWidth = (int)gameViewSizeType.GetField("width").GetValue(size);
                var sizeHeight = (int)gameViewSizeType.GetField("height").GetValue(size);

                if (sizeName == name || (sizeWidth == width && sizeHeight == height))
                {
                    return true;
                }
            }
        }
        catch (System.Exception e)
        {
        }

        return false;
    }

    private object GetCurrentGameViewSizeGroupType()
    {
        var assembly = typeof(Editor).Assembly;
        var gameViewSizesType = assembly.GetType("UnityEditor.GameViewSizes");
        var singletonType = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizesType);
        var instanceProperty = singletonType.GetProperty("instance");
        var gameViewSizesInstance = instanceProperty.GetValue(null);
        var currentGroupProperty = gameViewSizesType.GetProperty("currentGroupType");
        return currentGroupProperty.GetValue(gameViewSizesInstance);
    }

    private void SaveGameViewSizes()
    {
        try
        {
            var assembly = typeof(Editor).Assembly;
            var gameViewSizesType = assembly.GetType("UnityEditor.GameViewSizes");
            var singletonType = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizesType);
            var instanceProperty = singletonType.GetProperty("instance");
            var gameViewSizesInstance = instanceProperty.GetValue(null);
            var saveToHDMethod = gameViewSizesType.GetMethod("SaveToHDD");
            saveToHDMethod.Invoke(gameViewSizesInstance, null);
        }
        catch (System.Exception e)
        {
        }
    }

    private void RefreshGameView()
    {
        var gameView = EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
        if (gameView != null)
        {
            gameView.Repaint();
        }
    }
}