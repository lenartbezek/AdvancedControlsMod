using System;
using Lench.AdvancedControls.Blocks;
using UnityEngine;

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Input control creates analog input for otherwise one or two key actions.
    /// </summary>
    public class InputControl : Control
    {
        private Cog cog;
        private Steering steering;
        private Spring spring;

        /// <summary>
        /// Creates an input control for a block with given GUID.
        /// </summary>
        /// <param name="guid">GUID of the block.</param>
        public InputControl(Guid guid) : base(guid)
        {
            Name = "INPUT";
        }

        /// <summary>
        /// Control's block handler.
        /// Angle control accepts Lench.AdvancedControls.Blocks.Steering, Cog and Spring handlers.
        /// </summary>
        public override Block Block
        {
            get
            {
                if (cog != null) return cog;
                if (steering != null) return steering;
                if (spring != null) return spring;
                return null;
            }
            protected set
            {
                cog = value as Cog;
                steering = value as Steering;
                spring = value as Spring;
            }
        }

        /// <summary>
        /// Applies input to the block.
        /// </summary>
        /// <param name="value">Input value.</param>
        protected override void Apply(float value)
        {
            if (value > 0)
                value = Mathf.Lerp(Center, Max, value);
            else
                value = Mathf.Lerp(Center, Min, -value);
            cog?.SetInput(value);
            steering?.SetInput(value);
            spring?.SetInput(value);
        }

        internal override Control Clone()
        {
            var clone = new InputControl(BlockGUID);
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
