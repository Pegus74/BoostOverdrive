// Assets/Editor/GameViewPresetManager.cs
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class GameViewPresetManager : EditorWindow
{
    private GameViewPresetConfig config;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Game View Presets/Manage Presets")]
    public static void ShowWindow()
    {
        GetWindow<GameViewPresetManager>("Game View Presets");
    }

    private void OnEnable()
    {
        // Загружаем или создаем конфиг
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

        // Отображаем текущий конфиг
        var serializedObject = new SerializedObject(config);
        var presetsProperty = serializedObject.FindProperty("presets");
        EditorGUILayout.PropertyField(presetsProperty, true);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        // Кнопки действий
        if (GUILayout.Button("Create All Presets (Auto)"))
        {
            CreateAllPresets();
        }

        if (GUILayout.Button("Open Game View"))
        {
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("If auto-creation fails, use manual instructions to add presets through Game View UI.", MessageType.Info);

        EditorGUILayout.EndScrollView();
    }

    public void CreateAllPresets()
    {
        if (config == null) return;

        int successCount = 0;
        foreach (var preset in config.presets)
        {
            if (TryCreatePreset(preset))
            {
                successCount++;
            }
        }

        if (successCount == config.presets.Count)
        {
            EditorUtility.DisplayDialog("Success", $"All {successCount} presets created successfully!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Partial Success",
                $"Created {successCount} out of {config.presets.Count} presets. Check console for details.", "OK");
        }
    }

    private bool TryCreatePreset(GameViewPresetConfig.Preset preset)
    {
        try
        {
            var assembly = typeof(Editor).Assembly;
            var gameViewType = assembly.GetType("UnityEditor.GameView");
            var gameView = EditorWindow.GetWindow(gameViewType);

            if (gameView == null)
            {
                Debug.LogWarning($"Failed to get GameView window for preset: {preset.displayName}");
                return false;
            }

            // Пробуем разные методы создания
            return TryCreatePresetReflection(preset);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create preset {preset.displayName}: {e.Message}");
            return false;
        }
    }

    private bool TryCreatePresetReflection(GameViewPresetConfig.Preset preset)
    {
        try
        {
            var assembly = typeof(Editor).Assembly;

            // Получаем GameViewSizes через ScriptableSingleton
            var gameViewSizesType = assembly.GetType("UnityEditor.GameViewSizes");
            var singletonType = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizesType);
            var instanceProperty = singletonType.GetProperty("instance");
            var instance = instanceProperty.GetValue(null);

            if (instance == null) return false;

            // Получаем группу
            var getGroupMethod = gameViewSizesType.GetMethod("GetGroup");
            var group = getGroupMethod.Invoke(instance, new object[] { preset.groupType });

            // Создаем GameViewSize
            var gameViewSizeType = assembly.GetType("UnityEditor.GameViewSize");
            var constructor = gameViewSizeType.GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(string) });
            var newSize = constructor.Invoke(new object[] { 1, preset.width, preset.height, preset.displayName });

            // Добавляем в группу
            var addMethod = group.GetType().GetMethod("AddCustomSize");
            addMethod.Invoke(group, new object[] { newSize });

            Debug.Log($"✅ Created preset: {preset.displayName}");
            return true;
        }
        catch
        {
            return false;
        }
    }

  
}