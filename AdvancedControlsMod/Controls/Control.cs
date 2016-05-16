using System;
using UnityEngine;
using LenchScripter;
using LenchScripter.Blocks;
using AdvancedControls.Axes;

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

        internal string min;
        internal string cen;
        internal string max;

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

        public virtual void Load(BlockInfo blockInfo)
        {
            Axis = blockInfo.BlockData.ReadString("ac-control" + Name + "-axis");
            Min = blockInfo.BlockData.ReadFloat("ac-control" + Name + "-min");
            if (!PositiveOnly)
                Center = blockInfo.BlockData.ReadFloat("ac-control-" + Name + "-center");
            Max = blockInfo.BlockData.ReadFloat("ac-control-" + Name + "-max");
            Enabled = true;
        }

        public virtual void Save(BlockInfo blockInfo)
        {
            blockInfo.BlockData.Write("ac-control-" + Name + "-axis", Axis);
            blockInfo.BlockData.Write("ac-control-" + Name + "-min", Min);
            if (!PositiveOnly)
                blockInfo.BlockData.Write("ac-control-" + Name + "-center", Center);
            blockInfo.BlockData.Write("ac-control-" + Name + "-max", Max);
        }
    }
}
