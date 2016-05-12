using System;
using System.Collections.Generic;
using UnityEngine;
using LenchScripter;
using LenchScripter.Blocks;
using AdvancedControls.UI;
using spaar.ModLoader.UI;

namespace AdvancedControls.Controls
{
    public abstract class Control
    {
        public virtual string Name { get; set; } = "Control";
        public virtual float Min { get; set; } = -1;
        public virtual float Max { get; set; } = 1;
        public virtual float Center { get; set; } = 0;
        public virtual bool Enabled { get; set; } = false;
        public virtual bool PositiveOnly { get; set; } = false;
        public virtual Axes.Axis Axis { get; set; }
        public virtual Block Block { get; set; }
        public virtual string BlockGUID { get; set; }

        private string min;
        private string cen;
        private string max;

        public Control(string guid)
        {
            BlockGUID = guid;
            ADVControls.Instance.OnUpdate += Update;
            ADVControls.Instance.OnReset += Reset;
            min = (Mathf.Round(Min * 100) / 100).ToString();
            cen = (Mathf.Round(Center * 100) / 100).ToString();
            max = (Mathf.Round(Max * 100) / 100).ToString();
        }

        public virtual void Reset()
        {
            
        }

        public virtual void Update()
        {
            if (ADVControls.Instance.IsSimulating)
            {
                if (Block == null) Block = BlockHandlers.GetBlock(BlockGUID);
                if (Enabled && Axis != null)
                    Apply(Axis.Output);
            }
            else
            {
                Block = null;
            }
        }

        public abstract void Apply(float value);
        public virtual void Draw()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(Name, Elements.InputFields.Default, GUILayout.Width(120));

            if (Axis == null)
            {
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    AdvancedControlsMod.AxisList.SelectAxis(this);
                }
            }
            else
            {
                if (GUILayout.Button(Axis.Name))
                {
                    Enabled = true;
                    AdvancedControlsMod.AxisList.SelectAxis(this);
                }
                if (GUILayout.Button("×", Elements.Buttons.Red, GUILayout.Width(30)))
                {
                    Axis = null;
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Mininum");
                float min_parsed = Min;
                min = GUILayout.TextField(min);
                if (!min.EndsWith(".") && !min.EndsWith("-"))
                {
                    float.TryParse(min, out min_parsed);
                    Min = min_parsed;
                    min = (Mathf.Round(Min * 100) /100).ToString();
                }
                GUILayout.EndVertical();

                if (!PositiveOnly)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Center");
                    float cen_parsed = Center;
                    cen = GUILayout.TextField(cen);
                    if (!cen.EndsWith(".") && !cen.EndsWith("-"))
                    {
                        float.TryParse(cen, out cen_parsed);
                        Center = cen_parsed;
                        cen = (Mathf.Round(Center * 100) / 100).ToString();
                    }
                    GUILayout.EndVertical();
                }

                GUILayout.BeginVertical();
                GUILayout.Label("Maximum");
                float max_parsed = Max;
                max = GUILayout.TextField(max);
                if (!max.EndsWith(".") && !max.EndsWith("-"))
                {
                    float.TryParse(max, out max_parsed);
                    Max = max_parsed;
                    max = (Mathf.Round(Max * 100) / 100).ToString();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }

    public class ControlGroup : Control
    {
        public ControlGroup(string guid) : base(guid){}

        public Dictionary<string, Control> Controls { get; set; } = new Dictionary<string, Control>();

        private string enabled;
        public new string Enabled
        {
            get { return enabled; }
            set
            {
                foreach (KeyValuePair<string, Control> c in Controls)
                {
                    c.Value.Enabled = c.Key == value;
                }
                enabled = value;
            }
        }

        public override Block Block
        {
            get { return base.Block; }
            set
            {
                foreach(KeyValuePair<string, Control> c in Controls)
                {
                    c.Value.Block = Block;
                }
                base.Block = value;
            }
        }

        public override string BlockGUID
        {
            get { return base.BlockGUID; }
            set
            {
                foreach (KeyValuePair<string, Control> c in Controls)
                {
                    c.Value.BlockGUID = value;
                }
                base.BlockGUID = value;
            }
        }

        public override void Apply(float value){ }

        public override void Draw()
        {
            Control enabled = null;
            GUILayout.BeginHorizontal();
            foreach (KeyValuePair<string, Control> c in Controls)
            {
                if (GUILayout.Button(c.Value.Name, c.Value.Enabled ? Elements.Buttons.Default : Elements.Buttons.Disabled))
                {
                    Enabled = c.Key;
                }
                if (c.Value.Enabled) enabled = c.Value;
            }
            GUILayout.EndHorizontal();
            enabled?.Draw();
        }
    }
}
