using UnityEngine;

namespace SceneGenerator
{
    public enum AnchorPreset
    {
        TopLeft,     TopCenter,    TopRight,    TopStretch,
        MiddleLeft,  MiddleCenter, MiddleRight, MiddleStretch,
        BottomLeft,  BottomCenter, BottomRight, BottomStretch,
        StretchLeft, StretchCenter,StretchRight,StretchAll,
        Custom
    }

    public static class AnchorPresetResolver
    {
        public static (Vector2 min, Vector2 max) Resolve(AnchorPreset p)
        {
            return p switch
            {
                AnchorPreset.TopLeft       => (new Vector2(0,    1   ), new Vector2(0,    1   )),
                AnchorPreset.TopCenter     => (new Vector2(0.5f, 1   ), new Vector2(0.5f, 1   )),
                AnchorPreset.TopRight      => (new Vector2(1,    1   ), new Vector2(1,    1   )),
                AnchorPreset.TopStretch    => (new Vector2(0,    1   ), new Vector2(1,    1   )),
                AnchorPreset.MiddleLeft    => (new Vector2(0,    0.5f), new Vector2(0,    0.5f)),
                AnchorPreset.MiddleCenter  => (new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                AnchorPreset.MiddleRight   => (new Vector2(1,    0.5f), new Vector2(1,    0.5f)),
                AnchorPreset.MiddleStretch => (new Vector2(0,    0.5f), new Vector2(1,    0.5f)),
                AnchorPreset.BottomLeft    => (new Vector2(0,    0   ), new Vector2(0,    0   )),
                AnchorPreset.BottomCenter  => (new Vector2(0.5f, 0   ), new Vector2(0.5f, 0   )),
                AnchorPreset.BottomRight   => (new Vector2(1,    0   ), new Vector2(1,    0   )),
                AnchorPreset.BottomStretch => (new Vector2(0,    0   ), new Vector2(1,    0   )),
                AnchorPreset.StretchLeft   => (new Vector2(0,    0   ), new Vector2(0,    1   )),
                AnchorPreset.StretchCenter => (new Vector2(0.5f, 0   ), new Vector2(0.5f, 1   )),
                AnchorPreset.StretchRight  => (new Vector2(1,    0   ), new Vector2(1,    1   )),
                AnchorPreset.StretchAll    => (new Vector2(0,    0   ), new Vector2(1,    1   )),
                _                          => (new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
            };
        }
    }
}
