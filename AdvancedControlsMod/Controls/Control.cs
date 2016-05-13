using System;
using UnityEngine;
using LenchScripter;
using LenchScripter.Blocks;
using AdvancedControls.UI;
using AdvancedControls.Axes;
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
        public virtual string Axis { get; set; }
        public virtual Block Block { get; set; }
        public virtual Guid BlockGUID { get; set; }

        private string min;
        private string cen;
        private string max;

        public Control(Guid guid)
        {
            BlockGUID = guid;
            ADVControls.Instance.OnUpdate += Update;
            ADVControls.Instance.OnInitialisation += Initialise;

            min = (Mathf.Round(Min * 100) / 100).ToString();
            cen = (Mathf.Round(Center * 100) / 100).ToString();
            max = (Mathf.Round(Max * 100) / 100).ToString();
        }

        public virtual void Initialise()
        {
            try
            {
                Block = BlockHandlers.GetBlock(BlockGUID);
            }
            catch (BlockNotFoundException)
            {
                ControlManager.Blocks.Remove(BlockGUID);
            }
        }

        public virtual void Update()
        {
            if (ADVControls.Instance.IsSimulating)
            {
                var a = AxisManager.Get(Axis);
                if (Enabled && Block != null && a != null)
                {
                    Apply(a.OutputValue);
                }
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
                    Enabled = true;
                    AdvancedControlsMod.AxisList.SelectAxis(this);
                }
            }
            else
            {
                var a = AxisManager.Get(Axis);
                if (GUILayout.Button(Axis, a != null ? Elements.Buttons.Default : Elements.Buttons.Red))
                {
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

        public virtual void Load(BlockInfo blockInfo)
        {
            Axis = blockInfo.BlockData.ReadString("AC-Control-" + Name + "-Axis");
            Min = blockInfo.BlockData.ReadFloat("AC-Control-" + Name + "-Min");
            if (!PositiveOnly)
                Center = blockInfo.BlockData.ReadFloat("AC-Control-" + Name + "-Center");
            Max = blockInfo.BlockData.ReadFloat("AC-Control-" + Name + "-Max");
            Enabled = true;
        }

        public virtual void Save(BlockInfo blockInfo)
        {
            blockInfo.BlockData.Write("AC-Control-" + Name + "-Axis", Axis);
            blockInfo.BlockData.Write("AC-Control-" + Name + "-Min", Min);
            if (!PositiveOnly)
                blockInfo.BlockData.Write("AC-Control-" + Name + "-Center", Center);
            blockInfo.BlockData.Write("AC-Control-" + Name + "-Max", Max);
        }
    }
}
