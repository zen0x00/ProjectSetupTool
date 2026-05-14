using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
#if TMP_PRESENT
using TMPro;
#endif

namespace SceneGenerator
{
    public static class SceneGenerator
    {
        public static void Generate(SceneConfig config)
        {
            SceneTypeDefaults.Apply(config);

            if (config.createFolderStructure)
                CreateFolderStructure(config);

            if (config.setupLayersAndTags)
                SetupLayersAndTags(config);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            switch (config.sceneType)
            {
                case SceneType.Gameplay:
                    BuildGameplay(config);
                    break;
                case SceneType.UIMenu:
                case SceneType.LoadingScreen:
                    BuildUI(config);
                    break;
                case SceneType.Cutscene:
                    BuildCutscene(config);
                    break;
            }

            Directory.CreateDirectory(config.scenePath);
            string fullPath = Path.Combine(config.scenePath, config.sceneName + ".unity");
            EditorSceneManager.SaveScene(scene, fullPath);
            AssetDatabase.Refresh();

            Debug.Log($"[SceneGenerator] Saved: {fullPath}");
        }

        // ── Folder structure ────────────────────────────────────────────────

        static void CreateFolderStructure(SceneConfig config)
        {
            string[] common = new[]
            {
                "Assets/Scenes",
                "Assets/Scripts/Runtime",
                "Assets/Scripts/Editor",
                "Assets/Prefabs",
                "Assets/Materials",
                "Assets/Textures",
                "Assets/Audio/Music",
                "Assets/Audio/SFX",
                "Assets/Animations",
                "Assets/UI",
                "Assets/ScriptableObjects",
                "Assets/Plugins"
            };
            foreach (var f in common) EnsureFolder(f);

            switch (config.sceneType)
            {
                case SceneType.Gameplay:
                    EnsureFolder("Assets/Scripts/Runtime/Player");
                    EnsureFolder("Assets/Scripts/Runtime/Enemy");
                    EnsureFolder("Assets/Scripts/Runtime/Systems");
                    EnsureFolder("Assets/Prefabs/Player");
                    EnsureFolder("Assets/Prefabs/Enemies");
                    EnsureFolder("Assets/Prefabs/Environment");
                    break;
                case SceneType.UIMenu:
                case SceneType.LoadingScreen:
                    EnsureFolder("Assets/Scripts/Runtime/UI");
                    EnsureFolder("Assets/UI/Fonts");
                    EnsureFolder("Assets/UI/Sprites");
                    break;
                case SceneType.Cutscene:
                    EnsureFolder("Assets/Animations/Cutscenes");
                    EnsureFolder("Assets/Timeline");
                    break;
            }

            foreach (var f in config.additionalFolders) EnsureFolder(f);
            AssetDatabase.Refresh();
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string leaf   = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }

        // ── Layers & Tags ───────────────────────────────────────────────────

        static void SetupLayersAndTags(SceneConfig config)
        {
            var tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            AddTags(tagManager.FindProperty("tags"), config.customTags);
            AddLayers(tagManager.FindProperty("layers"), config.customLayers);

            tagManager.ApplyModifiedProperties();
        }

        static void AddTags(SerializedProperty prop, List<string> tags)
        {
            foreach (string tag in tags)
            {
                for (int i = 0; i < prop.arraySize; i++)
                    if (prop.GetArrayElementAtIndex(i).stringValue == tag) goto next;

                prop.InsertArrayElementAtIndex(prop.arraySize);
                prop.GetArrayElementAtIndex(prop.arraySize - 1).stringValue = tag;
                next:;
            }
        }

        static void AddLayers(SerializedProperty prop, List<string> layers)
        {
            foreach (string layer in layers)
            {
                for (int i = 8; i < prop.arraySize; i++)
                    if (prop.GetArrayElementAtIndex(i).stringValue == layer) goto next;

                for (int i = 8; i < prop.arraySize; i++)
                {
                    if (string.IsNullOrEmpty(prop.GetArrayElementAtIndex(i).stringValue))
                    {
                        prop.GetArrayElementAtIndex(i).stringValue = layer;
                        break;
                    }
                }
                next:;
            }
        }

        // ── Scene builders ──────────────────────────────────────────────────

        static void BuildGameplay(SceneConfig config)
        {
            CreateLighting(config);
            CreateCamera(config);
            CreatePostProcessing(config);
            SpawnPrefabs(config);
        }

        static void BuildUI(SceneConfig config)
        {
            CreateCamera(config);

            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            var eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            foreach (var panel in config.panels)
                CreatePanel(panel, canvasGO.transform);
        }

        static void BuildCutscene(SceneConfig config)
        {
            CreateLighting(config);
            CreateCamera(config);
            CreatePostProcessing(config);

            var director = new GameObject("CutsceneDirector");
            director.AddComponent<UnityEngine.Playables.PlayableDirector>();
        }

        // ── Scene object helpers ────────────────────────────────────────────

        static void CreateCamera(SceneConfig config)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            go.transform.position = new Vector3(0f, 1f, -10f);

            var cam = go.AddComponent<Camera>();
            cam.fieldOfView    = config.fieldOfView;
            cam.nearClipPlane  = config.nearClipPlane;
            cam.farClipPlane   = config.farClipPlane;
            cam.backgroundColor = config.cameraBackground;
            cam.clearFlags     = config.clearFlags;

            go.AddComponent<AudioListener>();
        }

        static void CreateLighting(SceneConfig config)
        {
            var go = new GameObject("Directional Light");
            go.transform.rotation = Quaternion.Euler(config.directionalLightRotationX, config.directionalLightRotationY, 0f);

            var light = go.AddComponent<Light>();
            light.type      = LightType.Directional;
            light.color     = config.directionalLightColor;
            light.intensity = config.directionalLightIntensity;
            light.shadows   = config.shadowType;

            if (config.skyboxMaterial != null)
                RenderSettings.skybox = config.skyboxMaterial;

            RenderSettings.ambientLight = config.ambientColor;
            RenderSettings.ambientMode  = UnityEngine.Rendering.AmbientMode.Flat;
        }

        static void CreatePostProcessing(SceneConfig config)
        {
            if (config.postProcessingVolumePrefab == null) return;
            var go = PrefabUtility.InstantiatePrefab(config.postProcessingVolumePrefab) as GameObject;
            if (go != null) go.name = "PostProcessingVolume";
        }

        static void SpawnPrefabs(SceneConfig config)
        {
            foreach (var prefab in config.prefabsToSpawn)
            {
                if (prefab != null)
                    PrefabUtility.InstantiatePrefab(prefab);
            }
        }

        // ── UI helpers ──────────────────────────────────────────────────────

        static void ApplyAnchors(RectTransform rect, AnchorPreset preset, Vector2 customMin, Vector2 customMax)
        {
            if (preset == AnchorPreset.Custom)
            {
                rect.anchorMin = customMin;
                rect.anchorMax = customMax;
            }
            else
            {
                var (min, max) = AnchorPresetResolver.Resolve(preset);
                rect.anchorMin = min;
                rect.anchorMax = max;
            }
        }

        static void CreatePanel(PanelConfig cfg, Transform canvasTransform)
        {
            var go = new GameObject(cfg.name);
            go.transform.SetParent(canvasTransform, false);

            var rect = go.AddComponent<RectTransform>();
            ApplyAnchors(rect, cfg.anchorPreset, cfg.customAnchorMin, cfg.customAnchorMax);
            rect.pivot            = cfg.pivot;
            rect.anchoredPosition = cfg.anchoredPosition;
            rect.sizeDelta        = cfg.sizeDelta;

            if (cfg.hasBackground)
                go.AddComponent<Image>().color = cfg.backgroundColor;

            foreach (var element in cfg.elements)
                CreateUIElement(element, go.transform);
        }

        static void CreateUIElement(UIElementConfig cfg, Transform parent)
        {
            var go = new GameObject(cfg.name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            ApplyAnchors(rect, cfg.anchorPreset, cfg.customAnchorMin, cfg.customAnchorMax);
            rect.pivot            = cfg.pivot;
            rect.anchoredPosition = cfg.anchoredPosition;
            rect.sizeDelta        = cfg.sizeDelta;

            switch (cfg.elementType)
            {
                case UIElementType.Image:
                    go.AddComponent<Image>().color = cfg.imageColor;
                    break;

                case UIElementType.RawImage:
                    go.AddComponent<RawImage>().color = cfg.imageColor;
                    break;

                case UIElementType.Text:
                    AddText(go, cfg);
                    break;

                case UIElementType.Button:
                    go.AddComponent<Image>().color = cfg.imageColor;
                    go.AddComponent<Button>();
                    AddTextChild(go, cfg);
                    break;

                case UIElementType.Slider:
                    go.AddComponent<Slider>();
                    break;

                case UIElementType.Toggle:
                    go.AddComponent<Toggle>();
                    break;

                case UIElementType.InputField:
                    go.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.15f);
                    go.AddComponent<InputField>();
                    break;

                case UIElementType.ScrollView:
                    go.AddComponent<ScrollRect>();
                    break;

                case UIElementType.Dropdown:
                    go.AddComponent<Image>().color = cfg.imageColor;
                    go.AddComponent<Dropdown>();
                    break;
            }
        }

        static void AddText(GameObject go, UIElementConfig cfg)
        {
#if TMP_PRESENT
            if (cfg.useTextMeshPro)
            {
                var t = go.AddComponent<TextMeshProUGUI>();
                t.text      = cfg.defaultText;
                t.fontSize  = cfg.fontSize;
                t.color     = cfg.textColor;
                t.alignment = TextAlignmentOptions.Center;
                return;
            }
#endif
            var legacy = go.AddComponent<Text>();
            legacy.text      = cfg.defaultText;
            legacy.fontSize  = cfg.fontSize;
            legacy.color     = cfg.textColor;
            legacy.alignment = TextAnchor.MiddleCenter;
            legacy.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        static void AddTextChild(GameObject parent, UIElementConfig cfg)
        {
            var child = new GameObject("Text");
            child.transform.SetParent(parent.transform, false);

            var r = child.AddComponent<RectTransform>();
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.sizeDelta = Vector2.zero;

            AddText(child, cfg);
        }
    }
}
