using System;
using Lench.AdvancedControls.Blocks;
using Lench.AdvancedControls.UI;
using UnityEngine;
// ReSharper disable UseNullPropagation

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Input control creates analog input for otherwise one or two key actions.
    /// </summary>
    public class InputControl : Control
    {
        private Cog _cog;
        private Steering _steering;
        private Spring _spring;

        /// <summary>
        /// Input control key is 'INPUT'.
        /// </summary>
        public override string Key => "INPUT";

        /// <summary>
        /// Localized display name of the control.
        /// </summary>
        public override string Name => Strings.ControlName_Input;

        /// <summary>
        /// Creates an input control for a block with given GUID.
        /// </summary>
        /// <param name="guid">GUID of the block.</param>
        public InputControl(Guid guid) : base(guid) {}

        /// <summary>
        /// Control's block handler.
        /// Angle control accepts Lench.AdvancedControls.Blocks.Steering, Cog and Spring handlers.
        /// </summary>
        public override Block Block
        {
            get
            {
                if (_cog != null) return _cog;
                if (_steering != null) return _steering;
                if (_spring != null) return _spring;
                return null;
            }
            protected set
            {
                _cog = value as Cog;
                _steering = value as Steering;
                _spring = value as Spring;
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
            _cog?.SetInput(value);
            _steering?.SetInput(value);
            _spring?.SetInput(value);
        }

        /// <summary>
        /// Clears Left and Right keys for steering.
        /// Clears Contract key for springs.
        /// Cog input is overriden without clearing keys.
        /// </summary>
        protected override void ClearKeys()
        {
            if (_steering != null)
            {
                Block.ClearKeys("LEFT");
                Block.ClearKeys("RIGHT");
            }
            if (_spring != null)
            {
                Block.ClearKeys("CONTRACT");
            }
        }

        internal override Control Clone()
        {
            var clone = new InputControl(BlockGUID)
            {
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
