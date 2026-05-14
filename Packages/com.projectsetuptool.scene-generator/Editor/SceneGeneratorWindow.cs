using UnityEngine;
using UnityEditor;

namespace SceneGenerator
{
    public class SceneGeneratorWindow : EditorWindow
    {
        SceneConfig config;
        SerializedObject serializedConfig;
        Vector2 scroll;

        bool showLighting     = true;
        bool showCamera       = true;
        bool showLayersTags   = true;
        bool showPrefabs      = true;
        bool showPanels       = true;
        bool showFolders      = true;

        [MenuItem("Tools/Scene Generator")]
        public static void Open()
        {
            var w = GetWindow<SceneGeneratorWindow>("Scene Generator");
            w.minSize = new Vector2(420, 600);
        }

        void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Scene Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            var picked = (SceneConfig)EditorGUILayout.ObjectField("Scene Config", config, typeof(SceneConfig), false);
            if (picked != config)
            {
                config = picked;
                serializedConfig = config != null ? new SerializedObject(config) : null;
            }

            if (GUILayout.Button("New Config", GUILayout.Width(100)))
                CreateNewConfig();

            if (config == null)
            {
                EditorGUILayout.Space(8);
                EditorGUILayout.HelpBox("Select or create a Scene Config to begin.", MessageType.Info);
                return;
            }

            EditorGUILayout.Space(8);
            serializedConfig.Update();
            scroll = EditorGUILayout.BeginScrollView(scroll);

            // ── Identity ─────────────────────────────────────────────────
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("sceneType"));
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("sceneName"));
            EditorGUILayout.PropertyField(serializedConfig.FindProperty("scenePath"));
            EditorGUILayout.Space(6);

            // ── Lighting ─────────────────────────────────────────────────
            showLighting = EditorGUILayout.Foldout(showLighting, "Lighting", true);
            if (showLighting)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedConfig.FindProperty("overrideLighting"));
                if (config.overrideLighting)
                {
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("ambientColor"));
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("directionalLightColor"));
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("directionalLightIntensity"));
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("directionalLightRotationX"));
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("directionalLightRotationY"));
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("shadowType"));
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("skyboxMaterial"));
                }
                EditorGUI.indentLevel--;
            }

            // ── Camera ───────────────────────────────────────────────────
            showCamera = EditorGUILayout.Foldout(showCamera, "Camera", true);
            if (showCamera)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedConfig.FindProperty("overrideCamera"));
                if (config.overrideCamera)
                {
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("fieldOfView"));
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("nearClipPlane"));
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("farClipPlane"));
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("cameraBackground"));
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("clearFlags"));
                }
                EditorGUI.indentLevel--;
            }

            // ── Layers & Tags ─────────────────────────────────────────────
            showLayersTags = EditorGUILayout.Foldout(showLayersTags, "Layers & Tags", true);
            if (showLayersTags)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedConfig.FindProperty("setupLayersAndTags"));
                if (config.setupLayersAndTags)
                {
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("customLayers"), true);
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("customTags"), true);
                }
                EditorGUI.indentLevel--;
            }

            // ── Prefabs ───────────────────────────────────────────────────
            showPrefabs = EditorGUILayout.Foldout(showPrefabs, "Prefabs & Post-Processing", true);
            if (showPrefabs)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedConfig.FindProperty("prefabsToSpawn"), true);
                EditorGUILayout.PropertyField(serializedConfig.FindProperty("postProcessingVolumePrefab"));
                EditorGUI.indentLevel--;
            }

            // ── UI Panels (UI scene types only) ───────────────────────────
            if (config.sceneType == SceneType.UIMenu || config.sceneType == SceneType.LoadingScreen)
            {
                showPanels = EditorGUILayout.Foldout(showPanels, "UI Panels", true);
                if (showPanels)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedConfig.FindProperty("panels"), true);
                    EditorGUI.indentLevel--;
                }
            }

            // ── Folder Structure ──────────────────────────────────────────
            showFolders = EditorGUILayout.Foldout(showFolders, "Folder Structure", true);
            if (showFolders)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedConfig.FindProperty("createFolderStructure"));
                EditorGUILayout.PropertyField(serializedConfig.FindProperty("additionalFolders"), true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndScrollView();
            serializedConfig.ApplyModifiedProperties();

            // ── Generate button ───────────────────────────────────────────
            EditorGUILayout.Space(12);
            var prev = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.35f, 0.75f, 0.35f);
            if (GUILayout.Button("Generate Scene", GUILayout.Height(42)))
                TryGenerate();
            GUI.backgroundColor = prev;
            EditorGUILayout.Space(8);
        }

        void TryGenerate()
        {
            if (config == null) return;

            bool ok = EditorUtility.DisplayDialog(
                "Generate Scene",
                $"Create scene '{config.sceneName}' at '{config.scenePath}'?\n\nThis may modify project layers and tags.",
                "Generate", "Cancel");

            if (!ok) return;

            SceneGenerator.Generate(config);
            EditorUtility.DisplayDialog("Scene Generator", $"Scene '{config.sceneName}' created.", "OK");
        }

        void CreateNewConfig()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "New Scene Config", "NewSceneConfig", "asset", "Choose save location");

            if (string.IsNullOrEmpty(path)) return;

            var asset = CreateInstance<SceneConfig>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            config = asset;
            serializedConfig = new SerializedObject(config);
        }
    }
}
