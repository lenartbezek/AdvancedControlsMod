using AdvancedControls.Axes;
using spaar.ModLoader.UI;
using System;
using UnityEngine;

namespace AdvancedControls.UI
{
    internal class ChainAxisEditor : AxisEditor
    {
        internal ChainAxisEditor(InputAxis axis)
        {
            Axis = axis as ChainAxis;
        }

        private ChainAxis Axis;

        internal string help =
@"With chain axis you can link two other axes
and combine them in a single linear axis.

You can also link another chain axis and
design more complex inputs.";
        internal string note;
        internal string error;

        private SelectAxisWindow popup;

        public void Open()
        {

        }

        public void Close()
        {
            UnityEngine.GameObject.Destroy(popup);
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
            Util.DrawRect(leftGraphRect, Color.gray);
            Util.DrawRect(rightGraphRect, Color.gray);

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

            GUILayout.Label("  <color=#808080><b>" + a.ToString("0.00") + "</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft },
                GUILayout.MinWidth(leftGraphRect.width),
                GUILayout.Height(20));

            GUILayout.Label("  <color=#808080><b>" + b.ToString("0.00") + "</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft, margin = new RectOffset(8, 0, 0, 0) },
                GUILayout.MinWidth(rightGraphRect.width),
                GUILayout.Height(20));

            GUILayout.EndHorizontal();

            // Draw yellow lines
            Util.FillRect(new Rect(
                                  graphRect.x + graphRect.width / 2 + graphRect.width / 2 * Axis.OutputValue,
                                  graphRect.y,
                                  1,
                                  graphRect.height),
                         Color.yellow);

            Util.FillRect(new Rect(
                                  leftGraphRect.x + leftGraphRect.width / 2 + leftGraphRect.width / 2 * a,
                                  leftGraphRect.y,
                                  1,
                                  leftGraphRect.height),
                         Color.yellow);

            Util.FillRect(new Rect(
                                  rightGraphRect.x + rightGraphRect.width / 2 + rightGraphRect.width / 2 * b,
                                  rightGraphRect.y,
                                  1,
                                  rightGraphRect.height),
                         Color.yellow);

            // Draw axis select buttons
            GUILayout.BeginHorizontal();

            if (Axis.SubAxis1 == null)
            {
                if (GUILayout.Button("Select Input Axis", Elements.Buttons.Disabled, GUILayout.MaxWidth(leftGraphRect.width)))
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
                    UnityEngine.GameObject.Destroy(popup);
                    popup = SelectAxisWindow.Open(callback, true);
                }
            }
            else
            {
                if (GUILayout.Button(Axis.SubAxis1, axis_a != null ? axis_a.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled : Elements.Buttons.Red,
                    GUILayout.MaxWidth(leftGraphRect.width)))
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
                    UnityEngine.GameObject.Destroy(popup);
                    popup = SelectAxisWindow.Open(callback, true);
                }
            }

            if (Axis.SubAxis2 == null)
            {
                if (GUILayout.Button("Select Input Axis", Elements.Buttons.Disabled, GUILayout.MaxWidth(rightGraphRect.width)))
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
                    UnityEngine.GameObject.Destroy(popup);
                    popup = SelectAxisWindow.Open(callback, true);
                }
            }
            else
            {
                if (GUILayout.Button(Axis.SubAxis2, axis_b != null ? axis_b.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled : Elements.Buttons.Red,
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
                    UnityEngine.GameObject.Destroy(popup);
                    popup = SelectAxisWindow.Open(callback, true);
                }
            }

            GUILayout.EndHorizontal();

            // Popup position
            if (popup != null)
            {
                popup.windowRect.x = windowRect.x;
                popup.windowRect.y = windowRect.y + windowRect.height;
            }
        }

        public string GetHelp()
        {
            return help;
        }

        public string GetNote()
        {
            return note;
        }

        public string GetError()
        {
            return error;
        }
    }
}

