using UnityEngine;
using spaar.ModLoader.UI;
using System.Text.RegularExpressions;

namespace Lench.AdvancedControls.UI
{
    internal static class Util
    {
        private static Texture2D _rectTexture = new Texture2D(1, 1);
        private static GUIStyle _rectStyle = new GUIStyle();
        private static Color _currentColor = new Color();

        internal static GUIStyle LabelStyle
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

        internal static GUIStyle ToggleStyle
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

        internal static GUISkin Skin
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

        internal static GUIStyle CompactWindowStyle
        {
            get
            {
                return new GUIStyle
                {
                    normal = Elements.Windows.Default.normal,
                    border = new RectOffset(4, 4, 4, 4),
                    padding = Elements.Settings.DefaultPadding,
                    margin = Elements.Settings.DefaultMargin
                };
            }
        }

        internal static GUIStyle FullWindowStyle
        {
            get { return Elements.Windows.Default; }
        }

        internal static void DrawEnabledBadge(bool enabled)
        {
            if (enabled)
            {
                GUILayout.Label("✓", Elements.InputFields.Default, GUILayout.Width(30));
            }
            else
            {
                GUIStyle style = new GUIStyle(Elements.InputFields.Default);
                style.normal.textColor = new Color(1, 0, 0);
                GUILayout.Label("✘", style, GUILayout.Width(30));
            }
        }

        internal static float DrawSlider(string label, float value, float min, float max, string old_text, out string new_text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, LabelStyle);
            new_text = Regex.Replace(GUILayout.TextField(old_text,
                LabelStyle,
                GUILayout.Width(60)), @"[^0-9\-.]", "");
            GUILayout.EndHorizontal();

            float slider = GUILayout.HorizontalSlider(value, min, max);
            if (new_text != old_text)
            {
                if (new_text != "-" &&
                    !new_text.EndsWith(".") &&
                    !new_text.EndsWith(".0"))
                {
                    float.TryParse(new_text.TrimEnd('-'), out value);
                    new_text = value.ToString();
                }
            }
            else if (slider != value)
            {
                value = slider;
                new_text = value.ToString("0.00");
            }
                
            return value;
        }

        internal static void DrawRect(Rect position, Color color)
        {
            FillRect(new Rect(
                position.x,
                position.y,
                position.width,
                1),
                Color.gray);

            FillRect(new Rect(
                position.x,
                position.y + position.height - 1,
                position.width,
                1),
                Color.gray);

            FillRect(new Rect(
                position.x,
                position.y,
                1,
                position.height),
                Color.gray);

            FillRect(new Rect(
                position.x + position.width - 1,
                position.y,
                1,
                position.height),
                Color.gray);
        }

        internal static void FillRect(Rect position, Color color)
        {
            if (color != _currentColor)
            {
                _rectTexture.SetPixel(0, 0, color);
                _rectTexture.Apply();

                _rectStyle.normal.background = _rectTexture;
            }

            GUI.Box(position, GUIContent.none, _rectStyle);
        }
    }
}
