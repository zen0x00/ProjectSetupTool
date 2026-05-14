using UnityEngine;
using System.Collections.Generic;

namespace SceneGenerator
{
    public static class SceneTypeDefaults
    {
        public static void Apply(SceneConfig config)
        {
            switch (config.sceneType)
            {
                case SceneType.Gameplay:      ApplyGameplay(config);      break;
                case SceneType.UIMenu:        ApplyUIMenu(config);        break;
                case SceneType.LoadingScreen: ApplyLoadingScreen(config); break;
                case SceneType.Cutscene:      ApplyCutscene(config);      break;
            }
        }

        static void ApplyGameplay(SceneConfig c)
        {
            if (!c.overrideLighting)
            {
                c.directionalLightColor = Color.white;
                c.directionalLightIntensity = 1f;
                c.directionalLightRotationX = 50f;
                c.directionalLightRotationY = -30f;
                c.shadowType = LightShadows.Soft;
                c.ambientColor = new Color(0.2f, 0.2f, 0.2f);
            }

            if (!c.overrideCamera)
            {
                c.fieldOfView = 60f;
                c.nearClipPlane = 0.1f;
                c.farClipPlane = 1000f;
                c.clearFlags = CameraClearFlags.Skybox;
            }

            if (c.customLayers.Count == 0)
                c.customLayers = new List<string> { "Ground", "Player", "Enemy", "Projectile", "Interactable" };

            if (c.customTags.Count == 0)
                c.customTags = new List<string> { "Player", "Enemy", "Ground", "Collectible", "Checkpoint" };
        }

        static void ApplyUIMenu(SceneConfig c)
        {
            if (!c.overrideLighting)
            {
                c.directionalLightIntensity = 0f;
                c.ambientColor = new Color(0.5f, 0.5f, 0.5f);
            }

            if (!c.overrideCamera)
            {
                c.clearFlags = CameraClearFlags.SolidColor;
                c.cameraBackground = new Color(0.05f, 0.05f, 0.05f, 1f);
            }

            if (c.panels.Count == 0)
            {
                c.panels = new List<PanelConfig>
                {
                    new PanelConfig
                    {
                        name = "MainMenuPanel",
                        anchorPreset = AnchorPreset.Custom,
                        customAnchorMin = new Vector2(0.1f, 0.1f),
                        customAnchorMax = new Vector2(0.9f, 0.9f),
                        pivot = new Vector2(0.5f, 0.5f),
                        backgroundColor = new Color(0f, 0f, 0f, 0.85f),
                        elements = new List<UIElementConfig>
                        {
                            new UIElementConfig
                            {
                                name = "TitleText",
                                elementType = UIElementType.Text,
                                anchorPreset = AnchorPreset.Custom,
                                customAnchorMin = new Vector2(0.1f, 0.75f),
                                customAnchorMax = new Vector2(0.9f, 0.95f),
                                pivot = new Vector2(0.5f, 0.5f),
                                sizeDelta = Vector2.zero,
                                defaultText = "Game Title",
                                fontSize = 48,
                                textColor = Color.white
                            },
                            new UIElementConfig
                            {
                                name = "PlayButton",
                                elementType = UIElementType.Button,
                                anchorPreset = AnchorPreset.Custom,
                                customAnchorMin = new Vector2(0.3f, 0.50f),
                                customAnchorMax = new Vector2(0.7f, 0.65f),
                                pivot = new Vector2(0.5f, 0.5f),
                                sizeDelta = Vector2.zero,
                                defaultText = "Play",
                                fontSize = 24,
                                imageColor = new Color(0.2f, 0.6f, 0.2f, 1f),
                                textColor = Color.white
                            },
                            new UIElementConfig
                            {
                                name = "SettingsButton",
                                elementType = UIElementType.Button,
                                anchorPreset = AnchorPreset.Custom,
                                customAnchorMin = new Vector2(0.3f, 0.33f),
                                customAnchorMax = new Vector2(0.7f, 0.48f),
                                pivot = new Vector2(0.5f, 0.5f),
                                sizeDelta = Vector2.zero,
                                defaultText = "Settings",
                                fontSize = 24,
                                imageColor = new Color(0.2f, 0.3f, 0.6f, 1f),
                                textColor = Color.white
                            },
                            new UIElementConfig
                            {
                                name = "QuitButton",
                                elementType = UIElementType.Button,
                                anchorPreset = AnchorPreset.Custom,
                                customAnchorMin = new Vector2(0.3f, 0.16f),
                                customAnchorMax = new Vector2(0.7f, 0.31f),
                                pivot = new Vector2(0.5f, 0.5f),
                                sizeDelta = Vector2.zero,
                                defaultText = "Quit",
                                fontSize = 24,
                                imageColor = new Color(0.6f, 0.2f, 0.2f, 1f),
                                textColor = Color.white
                            }
                        }
                    }
                };
            }
        }

        static void ApplyLoadingScreen(SceneConfig c)
        {
            if (!c.overrideLighting)
                c.directionalLightIntensity = 0f;

            if (!c.overrideCamera)
            {
                c.clearFlags = CameraClearFlags.SolidColor;
                c.cameraBackground = Color.black;
            }

            if (c.panels.Count == 0)
            {
                c.panels = new List<PanelConfig>
                {
                    new PanelConfig
                    {
                        name = "LoadingPanel",
                        anchorPreset = AnchorPreset.StretchAll,
                        pivot = new Vector2(0.5f, 0.5f),
                        backgroundColor = Color.black,
                        elements = new List<UIElementConfig>
                        {
                            new UIElementConfig
                            {
                                name = "LoadingText",
                                elementType = UIElementType.Text,
                                anchorPreset = AnchorPreset.Custom,
                                customAnchorMin = new Vector2(0.1f, 0.55f),
                                customAnchorMax = new Vector2(0.9f, 0.70f),
                                pivot = new Vector2(0.5f, 0.5f),
                                sizeDelta = Vector2.zero,
                                defaultText = "Loading...",
                                fontSize = 32,
                                textColor = Color.white
                            },
                            new UIElementConfig
                            {
                                name = "ProgressBarBackground",
                                elementType = UIElementType.Image,
                                anchorPreset = AnchorPreset.Custom,
                                customAnchorMin = new Vector2(0.1f, 0.40f),
                                customAnchorMax = new Vector2(0.9f, 0.50f),
                                pivot = new Vector2(0.5f, 0.5f),
                                sizeDelta = Vector2.zero,
                                imageColor = new Color(0.2f, 0.2f, 0.2f, 1f)
                            },
                            new UIElementConfig
                            {
                                name = "ProgressBarFill",
                                elementType = UIElementType.Image,
                                anchorPreset = AnchorPreset.Custom,
                                customAnchorMin = new Vector2(0.1f, 0.40f),
                                customAnchorMax = new Vector2(0.1f, 0.50f),
                                pivot = new Vector2(0f, 0.5f),
                                sizeDelta = Vector2.zero,
                                imageColor = new Color(0.2f, 0.8f, 0.2f, 1f)
                            }
                        }
                    }
                };
            }
        }

        static void ApplyCutscene(SceneConfig c)
        {
            if (!c.overrideLighting)
            {
                c.directionalLightColor = new Color(1f, 0.95f, 0.85f);
                c.directionalLightIntensity = 1.2f;
                c.directionalLightRotationX = 35f;
                c.directionalLightRotationY = -60f;
                c.shadowType = LightShadows.Soft;
            }

            if (!c.overrideCamera)
            {
                c.fieldOfView = 35f;
                c.nearClipPlane = 0.1f;
                c.farClipPlane = 500f;
                c.clearFlags = CameraClearFlags.Skybox;
            }
        }
    }
}
