using Lench.AdvancedControls.Axes;
using spaar.ModLoader.UI;
using System;
using UnityEngine;

namespace Lench.AdvancedControls.UI
{
    internal class ChainAxisEditor : AxisEditor
    {
        internal ChainAxisEditor(InputAxis axis)
        {
            Axis = axis as ChainAxis;
        }

        private ChainAxis Axis;

        internal string error;

        private AxisSelector popup;

        public void Open()
        {

        }

        public void Close()
        {
            GameObject.Destroy(popup);
        }

        public void DrawAxis(Rect windowRect)
        {
            // Draw graphs
            Rect graphRect = new Rect(
                GUI.skin.window.padding.left,
                GUI.skin.window.padding.top + 36,
                windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right,
                20);

            Rect leftGraphRect = new Rect(
                GUI.skin.window.padding.left,
                GUI.skin.window.padding.top + 100,
                (windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right) / 2 - 4,
                20);

            Rect rightGraphRect = new Rect(
                GUI.skin.window.padding.left + leftGraphRect.width + 8,
                GUI.skin.window.padding.top + 100,
                (windowRect.width - GUI.skin.window.padding.left - GUI.skin.window.padding.right) / 2 - 4,
                20);

            Util.DrawRect(graphRect, Color.gray);
            Util.FillRect(new Rect(
                    graphRect.x + graphRect.width / 2,
                    graphRect.y,
                    1,
                    graphRect.height),
                Color.gray);
            Util.DrawRect(leftGraphRect, Color.gray);
            Util.FillRect(new Rect(
                    leftGraphRect.x + leftGraphRect.width / 2,
                    leftGraphRect.y,
                    1,
                    leftGraphRect.height),
                Color.gray);
            Util.DrawRect(rightGraphRect, Color.gray);
            Util.FillRect(new Rect(
                    rightGraphRect.x + rightGraphRect.width / 2,
                    rightGraphRect.y,
                    1,
                    rightGraphRect.height),
                Color.gray);

            var axis_a = AxisManager.Get(Axis.SubAxis1);
            var axis_b = AxisManager.Get(Axis.SubAxis2);
            float a = axis_a != null ? axis_a.OutputValue : 0;
            float b = axis_b != null ? axis_b.OutputValue : 0;

            // Draw axis value
            GUILayout.Label("  <color=#808080><b>" + Axis.OutputValue.ToString("0.00") + "</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft },
                GUILayout.Height(20));

            // Draw method select
            int i = (int)Axis.Method;
            int num_methods = Enum.GetValues(typeof(ChainAxis.ChainMethod)).Length;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<", Elements.Buttons.Default, GUILayout.Width(30)))
                i--;
            if (i < 0) i += num_methods;

            GUILayout.Label(Enum.GetNames(typeof(ChainAxis.ChainMethod))[i], new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter });

            if (GUILayout.Button(">", Elements.Buttons.Default, GUILayout.Width(30)))
                i++;
            if (i == num_methods) i = 0;

            Axis.Method = (ChainAxis.ChainMethod)i;

            GUILayout.EndHorizontal();

            // Draw sub axis values
            GUILayout.BeginHorizontal(GUILayout.Height(20));

            GUILayout.Label("  <color=#808080><b>" + (axis_a == null ? "" : axis_a.Status == AxisStatus.OK ? a.ToString("0.00") : InputAxis.GetStatusString(axis_a.Status)) + "</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft },
                GUILayout.MinWidth(leftGraphRect.width),
                GUILayout.Height(20));

            GUILayout.Label("  <color=#808080><b>" + (axis_b == null ? "" : axis_b.Status == AxisStatus.OK ? b.ToString("0.00") : InputAxis.GetStatusString(axis_a.Status)) + "</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft, margin = new RectOffset(8, 0, 0, 0) },
                GUILayout.MinWidth(rightGraphRect.width),
                GUILayout.Height(20));

            GUILayout.EndHorizontal();

            // Draw yellow lines
            if (Axis.Status == AxisStatus.OK)
                Util.FillRect(new Rect(
                                        graphRect.x + graphRect.width / 2 + graphRect.width / 2 * Axis.OutputValue,
                                        graphRect.y,
                                        1,
                                        graphRect.height),
                                Color.yellow);

            if (axis_a != null && Axis.Status == AxisStatus.OK)
                Util.FillRect(new Rect(
                                      leftGraphRect.x + leftGraphRect.width / 2 + leftGraphRect.width / 2 * a,
                                      leftGraphRect.y,
                                      1,
                                      leftGraphRect.height),
                             Color.yellow);

            if (axis_b != null && Axis.Status == AxisStatus.OK)
                Util.FillRect(new Rect(
                                  rightGraphRect.x + rightGraphRect.width / 2 + rightGraphRect.width / 2 * b,
                                  rightGraphRect.y,
                                  1,
                                  rightGraphRect.height),
                         Color.yellow);

            // Draw axis select buttons
            GUILayout.BeginHorizontal();

            var buttonRect = GUILayoutUtility.GetRect(new GUIContent(" "), spaar.ModLoader.UI.Elements.Buttons.Default, GUILayout.MaxWidth(leftGraphRect.width));
            if (Axis.SubAxis1 == null)
            {
                if (GUI.Button(buttonRect, "Select Input Axis", spaar.ModLoader.UI.Elements.Buttons.Disabled))
                {
                    error = null;
                    var callback = new SelectAxisDelegate((InputAxis axis) =>
                    {
                        try
                        {
                            Axis.SubAxis1 = axis.Name;
                        }
                        catch (InvalidOperationException e)
                        {
                            error = "<color=#FFFF00><b>Chain cycle error</b></color>\n" + e.Message;
                        }
                    });
                    if (popup == null)
                        popup = AxisSelector.Open(callback, true);
                    else
                        popup.Callback = callback;
                    popup.windowRect.x = windowRect.x + buttonRect.x - 8;
                    popup.windowRect.y = windowRect.y + buttonRect.y - 8;
                }
            }
            else
            {
                if (GUI.Button(buttonRect, Axis.SubAxis1, axis_a != null ? axis_a.Saveable ? spaar.ModLoader.UI.Elements.Buttons.Default : spaar.ModLoader.UI.Elements.Buttons.Disabled : spaar.ModLoader.UI.Elements.Buttons.Red))
                {
                    error = null;
                    var callback = new SelectAxisDelegate((InputAxis axis) =>
                    {
                        try
                        {
                            Axis.SubAxis1 = axis.Name;
                        }
                        catch (InvalidOperationException e)
                        {
                            error = "<color=#FFFF00><b>Chain cycle error</b></color>\n" + e.Message;
                        }
                    });
                    if (popup == null)
                        popup = AxisSelector.Open(callback, true);
                    else
                        popup.Callback = callback;
                    popup.windowRect.x = windowRect.x + buttonRect.x - 8;
                    popup.windowRect.y = windowRect.y + buttonRect.y - 8;
                }
            }

            if (Axis.SubAxis2 == null)
            {
                if (GUILayout.Button("Select Input Axis", spaar.ModLoader.UI.Elements.Buttons.Disabled, GUILayout.MaxWidth(rightGraphRect.width)))
                {
                    error = null;
                    var callback = new SelectAxisDelegate((InputAxis axis) =>
                    {
                        try
                        {
                            Axis.SubAxis2 = axis.Name;
                        }
                        catch (InvalidOperationException e)
                        {
                            error = "<color=#FFFF00><b>Chain cycle error</b></color>\n" + e.Message;
                        }
                    });
                    if (popup == null)
                        popup = AxisSelector.Open(callback, true);
                    else
                        popup.Callback = callback;
                    popup.windowRect.x = windowRect.x + buttonRect.x - 8;
                    popup.windowRect.y = windowRect.y + buttonRect.y - 8;
                }
            }
            else
            {
                if (GUILayout.Button(Axis.SubAxis2, axis_b != null ? axis_b.Saveable ? spaar.ModLoader.UI.Elements.Buttons.Default : spaar.ModLoader.UI.Elements.Buttons.Disabled : spaar.ModLoader.UI.Elements.Buttons.Red,
                    GUILayout.MaxWidth(rightGraphRect.width)))
                {
                    error = null;
                    var callback = new SelectAxisDelegate((InputAxis axis) =>
                    {
                        try
                        {
                            Axis.SubAxis2 = axis.Name;
                        }
                        catch (InvalidOperationException e)
                        {
                            error = "<color=#FFFF00><b>Chain cycle error</b></color>\n" + e.Message;
                        }
                    });
                    if (popup == null)
                        popup = AxisSelector.Open(callback, true);
                    else
                        popup.Callback = callback;
                    popup.windowRect.x = windowRect.x + buttonRect.x - 8;
                    popup.windowRect.y = windowRect.y + buttonRect.y - 8;
                }
            }

            GUILayout.EndHorizontal();

            // Check for mouse exit
            if (popup != null && !popup.ContainsMouse)
                GameObject.Destroy(popup);
        }

        public string GetHelpURL()
        {
            return "https://github.com/lench4991/AdvancedControlsMod/wiki/Chain-Axis";
        }

        public string GetNote()
        {
            return null;
        }

        public string GetError()
        {
            return error;
        }
    }
}

