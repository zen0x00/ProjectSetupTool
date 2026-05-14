using UnityEngine;
using System.Collections.Generic;

namespace SceneGenerator
{
    public enum SceneType
    {
        Gameplay,
        UIMenu,
        LoadingScreen,
        Cutscene
    }

    [CreateAssetMenu(fileName = "NewSceneConfig", menuName = "Scene Generator/Scene Config")]
    public class SceneConfig : ScriptableObject
    {
        [Header("Identity")]
        public SceneType sceneType = SceneType.Gameplay;
        public string sceneName = "NewScene";
        public string scenePath = "Assets/Scenes";

        [Header("Lighting")]
        public bool overrideLighting = false;
        public Color ambientColor = new Color(0.2f, 0.2f, 0.2f);
        public Color directionalLightColor = Color.white;
        public float directionalLightIntensity = 1f;
        public float directionalLightRotationX = 50f;
        public float directionalLightRotationY = -30f;
        public LightShadows shadowType = LightShadows.Soft;
        public Material skyboxMaterial = null;

        [Header("Camera")]
        public bool overrideCamera = false;
        public float fieldOfView = 60f;
        public float nearClipPlane = 0.1f;
        public float farClipPlane = 1000f;
        public Color cameraBackground = new Color(0.1f, 0.1f, 0.1f, 1f);
        public CameraClearFlags clearFlags = CameraClearFlags.Skybox;

        [Header("Layers & Tags")]
        public bool setupLayersAndTags = true;
        public List<string> customLayers = new List<string>();
        public List<string> customTags = new List<string>();

        [Header("Prefabs")]
        public List<GameObject> prefabsToSpawn = new List<GameObject>();
        public GameObject postProcessingVolumePrefab = null;

        [Header("UI Panels")]
        public List<PanelConfig> panels = new List<PanelConfig>();

        [Header("Folder Structure")]
        public bool createFolderStructure = true;
        public List<string> additionalFolders = new List<string>();
    }
}
