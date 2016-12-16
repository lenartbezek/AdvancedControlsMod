using System;
using Lench.AdvancedControls.Axes;
using spaar.ModLoader.UI;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable PossibleNullReferenceException

namespace Lench.AdvancedControls.UI
{
    internal class ChainAxisEditor : IAxisEditor
    {
        internal ChainAxisEditor(InputAxis axis)
        {
            _axis = axis as ChainAxis;
        }

        private readonly ChainAxis _axis;

        internal string Error;

        private AxisSelector _popup;

        public void Open()
        {

        }

        public void Close()
        {
            Object.Destroy(_popup);
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

            var axisA = AxisManager.Get(_axis.SubAxis1);
            var axisB = AxisManager.Get(_axis.SubAxis2);
            float a = axisA?.OutputValue ?? 0;
            float b = axisB?.OutputValue ?? 0;

            // Draw axis value
            GUILayout.Label($"  <color=#808080><b>{_axis.OutputValue:0.00}</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft },
                GUILayout.Height(20));

            // Draw method select
            int i = (int)_axis.Method;
            int numMethods = Enum.GetValues(typeof(ChainAxis.ChainMethod)).Length;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Strings.ButtonText_ArrowPrevious, Elements.Buttons.Default, GUILayout.Width(30)))
                i--;
            if (i < 0) i += numMethods;

            GUILayout.Label(Enum.GetNames(typeof(ChainAxis.ChainMethod))[i], new GUIStyle(Elements.InputFields.Default) { alignment = TextAnchor.MiddleCenter });

            if (GUILayout.Button(Strings.ButtonText_ArrowNext, Elements.Buttons.Default, GUILayout.Width(30)))
                i++;
            if (i == numMethods) i = 0;

            _axis.Method = (ChainAxis.ChainMethod)i;

            GUILayout.EndHorizontal();

            // Draw sub axis values
            GUILayout.BeginHorizontal(GUILayout.Height(20));

            GUILayout.Label(
                $"  <color=#808080><b>{(axisA == null ? string.Empty : axisA.Status == AxisStatus.OK ? a.ToString("0.00") : InputAxis.GetStatusString(axisA.Status))}</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft },
                GUILayout.MinWidth(leftGraphRect.width),
                GUILayout.Height(20));

            GUILayout.Label(
                $"  <color=#808080><b>{(axisB == null ? string.Empty : axisB.Status == AxisStatus.OK ? b.ToString("0.00") : InputAxis.GetStatusString(axisA.Status))}</b></color>",
                new GUIStyle(Elements.Labels.Default) { richText = true, alignment = TextAnchor.MiddleLeft, margin = new RectOffset(8, 0, 0, 0) },
                GUILayout.MinWidth(rightGraphRect.width),
                GUILayout.Height(20));

            GUILayout.EndHorizontal();

            // Draw yellow lines
            if (_axis.Status == AxisStatus.OK)
                Util.FillRect(new Rect(
                                        graphRect.x + graphRect.width / 2 + graphRect.width / 2 * _axis.OutputValue,
                                        graphRect.y,
                                        1,
                                        graphRect.height),
                                Color.yellow);

            if (axisA != null && _axis.Status == AxisStatus.OK)
                Util.FillRect(new Rect(
                                      leftGraphRect.x + leftGraphRect.width / 2 + leftGraphRect.width / 2 * a,
                                      leftGraphRect.y,
                                      1,
                                      leftGraphRect.height),
                             Color.yellow);

            if (axisB != null && _axis.Status == AxisStatus.OK)
                Util.FillRect(new Rect(
                                  rightGraphRect.x + rightGraphRect.width / 2 + rightGraphRect.width / 2 * b,
                                  rightGraphRect.y,
                                  1,
                                  rightGraphRect.height),
                         Color.yellow);

            // Draw axis select buttons
            GUILayout.BeginHorizontal();

            var buttonRect = GUILayoutUtility.GetRect(new GUIContent(" "), Elements.Buttons.Default, GUILayout.MaxWidth(leftGraphRect.width));
            if (_axis.SubAxis1 == null)
            {
                if (GUI.Button(buttonRect, Strings.ButtonText_SelectInputAxis, Elements.Buttons.Disabled))
                {
                    Error = null;
                    var callback = new SelectAxisDelegate(axis =>
                    {
                        try
                        {
                            _axis.SubAxis1 = axis.Name;
                        }
                        catch (InvalidOperationException e)
                        {
                            Error = Strings.ChainAxisEditor_Message_ChainCycleError + e.Message;
                        }
                    });
                    if (_popup == null)
                        _popup = AxisSelector.Open(callback, true);
                    else
                        _popup.Callback = callback;
                    _popup.WindowRect.x = windowRect.x + buttonRect.x - 8;
                    _popup.WindowRect.y = windowRect.y + buttonRect.y - 8;
                }
            }
            else
            {
                if (GUI.Button(buttonRect, _axis.SubAxis1, axisA != null ? axisA.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled : Elements.Buttons.Red))
                {
                    Error = null;
                    var callback = new SelectAxisDelegate(axis =>
                    {
                        try
                        {
                            _axis.SubAxis1 = axis.Name;
                        }
                        catch (InvalidOperationException e)
                        {
                            Error = Strings.ChainAxisEditor_Message_ChainCycleError + e.Message;
                        }
                    });
                    if (_popup == null)
                        _popup = AxisSelector.Open(callback, true);
                    else
                        _popup.Callback = callback;
                    _popup.WindowRect.x = windowRect.x + buttonRect.x - 8;
                    _popup.WindowRect.y = windowRect.y + buttonRect.y - 8;
                }
            }

            if (_axis.SubAxis2 == null)
            {
                if (GUILayout.Button(Strings.ButtonText_SelectInputAxis, Elements.Buttons.Disabled, GUILayout.MaxWidth(rightGraphRect.width)))
                {
                    Error = null;
                    var callback = new SelectAxisDelegate(axis =>
                    {
                        try
                        {
                            _axis.SubAxis2 = axis.Name;
                        }
                        catch (InvalidOperationException e)
                        {
                            Error = Strings.ChainAxisEditor_Message_ChainCycleError + e.Message;
                        }
                    });
                    if (_popup == null)
                        _popup = AxisSelector.Open(callback, true);
                    else
                        _popup.Callback = callback;
                    _popup.WindowRect.x = windowRect.x + buttonRect.x - 8;
                    _popup.WindowRect.y = windowRect.y + buttonRect.y - 8;
                }
            }
            else
            {
                if (GUILayout.Button(_axis.SubAxis2, axisB != null ? axisB.Saveable ? Elements.Buttons.Default : Elements.Buttons.Disabled : Elements.Buttons.Red,
                    GUILayout.MaxWidth(rightGraphRect.width)))
                {
                    Error = null;
                    var callback = new SelectAxisDelegate(axis =>
                    {
                        try
                        {
                            _axis.SubAxis2 = axis.Name;
                        }
                        catch (InvalidOperationException e)
                        {
                            Error = Strings.ChainAxisEditor_Message_ChainCycleError + e.Message;
                        }
                    });
                    if (_popup == null)
                        _popup = AxisSelector.Open(callback, true);
                    else
                        _popup.Callback = callback;
                    _popup.WindowRect.x = windowRect.x + buttonRect.x - 8;
                    _popup.WindowRect.y = windowRect.y + buttonRect.y - 8;
                }
            }

            GUILayout.EndHorizontal();

            // Check for mouse exit
            if (_popup != null && !_popup.ContainsMouse)
                Object.Destroy(_popup);
        }

        public string GetHelpURL()
        {
            return Strings.ChainAxisEditor_HelpURL;
        }

        public string GetNote()
        {
            return null;
        }

        public string GetError()
        {
            return Error;
        }
    }
}

