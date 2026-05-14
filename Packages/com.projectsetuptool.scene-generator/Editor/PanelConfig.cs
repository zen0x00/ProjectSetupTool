using UnityEngine;
using System.Collections.Generic;

namespace SceneGenerator
{
    [System.Serializable]
    public class PanelConfig
    {
        public string name = "Panel";

        [Header("Layout")]
        public Vector2 anchorMin = Vector2.zero;
        public Vector2 anchorMax = Vector2.one;
        public Vector2 pivot = new Vector2(0.5f, 0.5f);
        public Vector2 anchoredPosition = Vector2.zero;
        public Vector2 sizeDelta = Vector2.zero;

        [Header("Appearance")]
        public bool hasBackground = true;
        public Color backgroundColor = new Color(0f, 0f, 0f, 0.8f);

        [Header("Elements")]
        public List<UIElementConfig> elements = new List<UIElementConfig>();
    }
}
