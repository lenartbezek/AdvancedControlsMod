using UnityEngine;

namespace AdvancedControlsMod.UI
{
    public static class Util
    {

        private static Texture2D _staticRectTexture = new Texture2D(1, 1);
        private static GUIStyle _staticRectStyle = new GUIStyle();

        public static void DrawRect(Rect position, Color color)
        {
            _staticRectTexture.SetPixel(0, 0, color);
            _staticRectTexture.Apply();

            _staticRectStyle.normal.background = _staticRectTexture;

            GUI.Box(position, GUIContent.none, _staticRectStyle);
        }

        public static void DrawPixel(Vector2 point, Color color)
        {
            DrawRect(new Rect(point.x, point.y, 1, 1), color);
        }

    }
}
