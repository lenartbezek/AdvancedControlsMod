using UnityEngine;
using LenchScripter.Blocks;
using System;

namespace AdvancedControls.Controls
{

    public class PositionControl : Control
    {
        private Piston piston;

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

            set
            {
                base.Min = Mathf.Clamp(value, 0, 1);
            }
        }

        public override float Max
        {
            get { return base.Max; }

            set
            {
                base.Max = Mathf.Clamp(value, 0, 1);
            }
        }

        public override void Apply(float value)
        {
            piston?.SetPosition(Mathf.Lerp(Min, Max, value));
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
