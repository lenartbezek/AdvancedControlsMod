using UnityEngine;
using spaar.ModLoader.UI;

namespace AdvancedControlsMod.UI
{
    public static class Util
    {

        private static Texture2D _rectTexture = new Texture2D(1, 1);
        private static GUIStyle _rectStyle = new GUIStyle();

        private static Color _currentColor = new Color();

        public static GUIStyle LabelStyle
        {
            get
            {
                var style = new GUIStyle(Elements.Labels.Default);
                style.richText = true;
                style.padding.top = 6;
                style.padding.left = 12;
                return style;
            }
        }

        public static GUIStyle ToggleStyle
        {
            get
            {
                var style = new GUIStyle(Elements.Toggle.Default);
                style.margin.top = 12;
                style.margin.bottom = 12;
                style.margin.left = 30;
                return style;
            }
        }

        public static GUIStyle CancelButton
        {
            get
            {
                var style = new GUIStyle(Elements.Buttons.Red);
                style.margin.top = 12;
                style.padding.top = 2;
                style.padding.bottom = 2;
                return style;
            }
        }

        public static GUISkin Skin
        {
            get
            {
                var skin = ModGUI.Skin;
                GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 0.7f);
                skin.window.padding.left = 8;
                skin.window.padding.right = 8;
                skin.window.padding.bottom = 8;
                return skin;
            }
        }

        public static void DrawRect(Rect position, Color color)
        {
            if (color != _currentColor)
            {
                _rectTexture.SetPixel(0, 0, color);
                _rectTexture.Apply();

                _rectStyle.normal.background = _rectTexture;
            }

            GUI.Box(position, GUIContent.none, _rectStyle);
        }

        public static void DrawPixel(Vector2 point, Color color)
        {
            DrawRect(new Rect(point.x, point.y, 1, 1), color);
        }

    }
}
