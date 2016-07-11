using System;
using UnityEngine;
using Lench.Scripter.Blocks;

namespace Lench.AdvancedControls.Controls
{

    public class PositionControl : Control
    {
        public override string Name { get; set; } = "POSITION";

        private Piston piston;

        public PositionControl(Guid guid) : base(guid)
        {
            Min = 0;
            Center = 0.5f;
            Max = 1;
        }

        public override Block Block
        {
            get
            {
                if (piston != null) return piston;
                return null;
            }
            set
            {
                piston = value as Piston;
            }
        }

        public override float Min
        {
            get { return base.Min; }
            set { base.Min = Mathf.Clamp(value, 0, 1); }
        }

        public override float Center
        {
            get { return base.Center; }
            set { base.Center = Mathf.Clamp(value, 0, 1); }
        }

        public override float Max
        {
            get { return base.Max; }
            set { base.Max = Mathf.Clamp(value, 0, 1); }
        }

        protected override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            piston?.SetPosition(value);
        }

        internal override Control Clone()
        {
            var clone = new PositionControl(BlockGUID);
            clone.Name = Name;
            clone.Enabled = Enabled;
            clone.Axis = Axis;
            clone.Block = Block;
            clone.Min = Min;
            clone.Center = Center;
            clone.Max = Max;
            return clone;
        }
    }
}
