﻿using UnityEngine;
using spaar.ModLoader.UI;
using LenchScripter;

namespace AdvancedControlsMod.UI
{
    public class CustomAxisEdit : AxisEdit
    {
        public new string name { get { return "Edit Custom Axis window"; } }

        private new CustomAxis axis = new CustomAxis();

        protected override float DesiredWidth { get; } = 320;
        protected override float DesiredHeight { get; } = 466;

        private Vector2 initialisationScrollPosition = Vector2.zero;
        private Vector2 updateScrollPosition = Vector2.zero;

        public override void SaveAxis()
        {
            Visible = false;
            axis.Name = axisName;
            AdvancedControlsMod.AxisList.SaveAxis(axis);
        }

        public override void EditAxis(Axis axis)
        {
            Visible = true;
            this.axisName = axis.Name;
            this.axis = (axis as CustomAxis).Clone();
        }

        protected override void DoWindow(int id)
        {
            base.DoWindow(id);

            // Draw initialisation code text area
            GUILayout.Label("Initialisation code ",
                Util.LabelStyle);

            initialisationScrollPosition = GUILayout.BeginScrollView(initialisationScrollPosition,
                GUILayout.Height(100));
            axis.InitialisationCode = GUILayout.TextArea(axis.InitialisationCode);
            GUILayout.EndScrollView();

            // Draw update code text area
            GUILayout.Label("Update code ",
                Util.LabelStyle);

            updateScrollPosition = GUILayout.BeginScrollView(updateScrollPosition,
                GUILayout.Height(200));
            axis.UpdateCode = GUILayout.TextArea(axis.UpdateCode);
            GUILayout.EndScrollView();

            // Draw notes
            if (!Lua.Enabled)
                GUILayout.Label(
@"<color=#FFFF00>Note:</color>
Lua needs to be enabled in the settings menu.",
                    Util.LabelStyle);
        }
    }
}