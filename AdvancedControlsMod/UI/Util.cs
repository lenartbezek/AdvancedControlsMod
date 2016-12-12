using UnityEngine;
using spaar.ModLoader.UI;
using System.Text.RegularExpressions;
// ReSharper disable SpecifyACultureInStringConversionExplicitly
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Lench.AdvancedControls.UI
{
    internal static class Util
    {
        private static readonly Texture2D RectTexture = new Texture2D(1, 1);
        private static readonly GUIStyle RectStyle = new GUIStyle();
        private static readonly Color CurrentColor = new Color();

        internal static GUIStyle LabelStyle => new GUIStyle(Elements.Labels.Default)
        {
            richText = true,
            padding =
            {
                top = 6,
                left = 12
            }
        };

        internal static GUIStyle ToggleStyle => new GUIStyle(Elements.Toggle.Default)
        {
            margin =
            {
                top = 12,
                bottom = 12,
                left = 30
            }
        };

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

        internal static GUIStyle CompactWindowStyle => new GUIStyle
        {
            normal = Elements.Windows.Default.normal,
            border = new RectOffset(4, 4, 4, 4),
            padding = Elements.Settings.DefaultPadding,
            margin = Elements.Settings.DefaultMargin
        };

        internal static GUIStyle FullWindowStyle => Elements.Windows.Default;

        internal static GUIStyle InvisibleWindowStyle => new GUIStyle
        {
            normal = new GUIStyleState(),
            border = new RectOffset(4, 4, 4, 4),
            padding = Elements.Settings.DefaultPadding,
            margin = Elements.Settings.DefaultMargin
        };

        internal static void DrawEnabledBadge(bool enabled)
        {
            if (enabled)
            {
                GUILayout.Label("✓", Elements.InputFields.Default, GUILayout.Width(30));
            }
            else
            {
                GUIStyle style = new GUIStyle(Elements.InputFields.Default) {normal = {textColor = new Color(1, 0, 0)}};
                GUILayout.Label("✘", style, GUILayout.Width(30));
            }
        }

        internal static float DrawSlider(string label, float value, float min, float max, string oldText, out string newText)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, LabelStyle);
            newText = Regex.Replace(GUILayout.TextField(oldText,
                LabelStyle,
                GUILayout.Width(60)), @"[^0-9\-.]", "");
            GUILayout.EndHorizontal();

            float slider = GUILayout.HorizontalSlider(value, min, max);
            if (newText != oldText)
            {
                if (newText != "-" &&
                    !newText.EndsWith(".") &&
                    !newText.EndsWith(".0"))
                {
                    float.TryParse(newText.TrimEnd('-'), out value);
                    newText = value.ToString();
                }
            }
            else if (slider != value)
            {
                value = slider;
                newText = value.ToString("0.00");
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
                color);

            FillRect(new Rect(
                position.x,
                position.y + position.height - 1,
                position.width,
                1),
                color);

            FillRect(new Rect(
                position.x,
                position.y,
                1,
                position.height),
                color);

            FillRect(new Rect(
                position.x + position.width - 1,
                position.y,
                1,
                position.height),
                color);
        }

        internal static void FillRect(Rect position, Color color)
        {
            if (color != CurrentColor)
            {
                RectTexture.SetPixel(0, 0, color);
                RectTexture.Apply();

                RectStyle.normal.background = RectTexture;
            }

            GUI.Box(position, GUIContent.none, RectStyle);
        }
    }
}
