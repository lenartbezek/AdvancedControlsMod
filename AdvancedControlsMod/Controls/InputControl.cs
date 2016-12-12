using System;
using Lench.AdvancedControls.Blocks;
using UnityEngine;
// ReSharper disable UseNullPropagation

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
        public override BlockHandler Block
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
            value = value > 0 
                ? Mathf.Lerp(Center, Max, value) 
                : Mathf.Lerp(Center, Min, -value);
            cog?.SetInput(value);
            steering?.SetInput(value);
            spring?.SetInput(value);
        }

        /// <summary>
        /// Clears Left and Right keys for steering.
        /// Clears Contract key for springs.
        /// Cog input is overriden without clearing keys.
        /// </summary>
        protected override void ClearKeys()
        {
            if (steering != null)
            {
                Block.ClearKeys("LEFT");
                Block.ClearKeys("RIGHT");
            }
            if (spring != null)
            {
                Block.ClearKeys("CONTRACT");
            }
        }

        internal override Control Clone()
        {
            var clone = new InputControl(BlockGUID)
            {
                Name = Name,
                Enabled = Enabled,
                Axis = Axis,
                Block = Block,
                Min = Min,
                Center = Center,
                Max = Max
            };
            return clone;
        }
    }
}
