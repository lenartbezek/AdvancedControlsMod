using System;
using UnityEngine;
using LenchScripter;
using LenchScripter.Blocks;
using AdvancedControls.Axes;
using spaar.ModLoader;

namespace AdvancedControls.Controls
{
    public abstract class Control
    {
        public virtual string Name { get; set; } = "Control";
        public virtual bool Enabled { get; set; } = false;
        public virtual bool PositiveOnly { get; set; } = false;
        public virtual string Axis { get; set; }
        public virtual Block Block { get; set; }
        public virtual Guid BlockGUID { get; set; }

        public virtual float Min
        {
            get { return min; }
            set
            {
                min = PositiveOnly && value < 0 ? 0 : value;
                min_string = (Mathf.Round(min * 100) / 100).ToString();
            }
        }

        public virtual float Center
        {
            get { return cen; }
            set
            {
                cen = PositiveOnly && value < 0 ? 0 : value;
                cen_string = (Mathf.Round(cen * 100) / 100).ToString();
            }
        }

        public virtual float Max
        {
            get { return max; }
            set
            {
                max = PositiveOnly && value < 0 ? 0 : value;
                max_string = (Mathf.Round(max * 100) / 100).ToString();
            }
        }

        private float min = -1;
        private float cen = 0;
        private float max = 1;

        internal string min_string;
        internal string cen_string;
        internal string max_string;

        public Control(Guid guid)
        {
            BlockGUID = guid;
            ACM.Instance.OnUpdate += Update;
            ACM.Instance.OnInitialisation += Initialise;

            min_string = (Mathf.Round(Min * 100) / 100).ToString();
            cen_string = (Mathf.Round(Center * 100) / 100).ToString();
            max_string = (Mathf.Round(Max * 100) / 100).ToString();
        }

        protected virtual void Initialise()
        {
            try
            {
                Block = BlockHandlers.GetBlock(BlockGUID);
            }
            catch (BlockNotFoundException)
            {
                Block = null;
                ControlManager.Blocks.Remove(BlockGUID);
            }
        }

        protected virtual void Update()
        {
            if (Game.IsSimulating)
            {
                var a = AxisManager.Get(Axis);
                if (Enabled && Block != null && a != null && a.Saveable)
                {
                    Apply(a.OutputValue);
                }
            }
        }

        protected abstract void Apply(float value);

        internal abstract Control Clone();

        internal virtual void Load(BlockInfo blockInfo)
        {
            Axis = blockInfo.BlockData.ReadString("ac-control-" + Name + "-axis");
            if (blockInfo.BlockData.HasKey("ac-control-" + Name + "-min"))
                Min = blockInfo.BlockData.ReadFloat("ac-control-" + Name + "-min");
            if (blockInfo.BlockData.HasKey("ac-control-" + Name + "-center"))
                Center = blockInfo.BlockData.ReadFloat("ac-control-" + Name + "-center");
            if (blockInfo.BlockData.HasKey("ac-control-" + Name + "-max"))
                Max = blockInfo.BlockData.ReadFloat("ac-control-" + Name + "-max");
            Enabled = true;
        }

        internal virtual void Save(BlockInfo blockInfo)
        {
            blockInfo.BlockData.Write("ac-control-" + Name + "-axis", Axis);
            blockInfo.BlockData.Write("ac-control-" + Name + "-min", Min);
            blockInfo.BlockData.Write("ac-control-" + Name + "-center", Center);
            blockInfo.BlockData.Write("ac-control-" + Name + "-max", Max);
        }
    }
}
