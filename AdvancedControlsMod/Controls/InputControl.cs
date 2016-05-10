using System;
using LenchScripter.Blocks;
using UnityEngine;

namespace AdvancedControls.Controls
{
    public class InputControl : Control
    {
        private Cog cog;
        private Steering steering;
        private Spring spring;

        public override Block Block
        {
            get
            {
                if (cog != null) return cog;
                if (steering != null) return steering;
                if (spring != null) return spring;
                return null;
            }
            set
            {
                cog = value as Cog;
                steering = value as Steering;
                spring = value as Spring;
            }
        }

        public override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            cog?.SetInput(value);
            steering?.SetInput(value);
            spring?.SetInput(value);
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
