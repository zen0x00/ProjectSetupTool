using UnityEngine;

namespace SceneGenerator
{
    public enum UIElementType
    {
        Text,
        Image,
        Button,
        Slider,
        Toggle,
        InputField,
        RawImage,
        ScrollView,
        Dropdown
    }

    [System.Serializable]
    public class UIElementConfig
    {
        public string name = "Element";
        public UIElementType elementType = UIElementType.Text;

        [Header("Layout")]
        public AnchorPreset anchorPreset = AnchorPreset.MiddleCenter;
        public Vector2 customAnchorMin = new Vector2(0.5f, 0.5f);
        public Vector2 customAnchorMax = new Vector2(0.5f, 0.5f);
        public Vector2 pivot = new Vector2(0.5f, 0.5f);
        public Vector2 anchoredPosition = Vector2.zero;
        public Vector2 sizeDelta = new Vector2(160f, 30f);

        [Header("Text")]
        public bool useTextMeshPro = true;
        public string defaultText = "Text";
        public int fontSize = 14;
        public Color textColor = Color.white;

        [Header("Image")]
        public Color imageColor = Color.white;
    }
}
