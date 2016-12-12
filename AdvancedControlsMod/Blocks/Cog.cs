using System;
using System.Reflection;
// ReSharper disable RedundantArgumentDefaultValue

namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    /// Handler for all wheel and cog blocks.
    /// </summary>
    public class Cog : BlockHandler
    {
        private static readonly FieldInfo InputFieldInfo = typeof(CogMotorControllerHinge).GetField("input", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly CogMotorControllerHinge _cmc;

        private float _desiredInput;
        private bool _setInputFlag;

        /// <summary>
        /// Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Cog(BlockBehaviour bb) : base(bb)
        {
            _cmc = bb.GetComponent<CogMotorControllerHinge>();
        }

        /// <summary>
        /// Invokes the block's action.
        /// Throws ActionNotFoundException if the block does not poses such action.
        /// </summary>
        /// <param name="actionName">Display name of the action.</param>
        public override void Action(string actionName)
        {
            actionName = actionName.ToUpper();
            if (actionName == "FORWARDS")
            {
                SetInput(1);
                return;
            }
            if (actionName == "REVERSE")
            {
                SetInput(-1);
                return;
            }
            throw new ActionNotFoundException("Block " + BlockName + " has no " + actionName + " action.");
        }

        /// <summary>
        /// Sets the input value on the next LateUpdate.
        /// </summary>
        /// <param name="value">Value to be set.</param>
        public void SetInput(float value = 1)
        {
            if (float.IsNaN(value))
                throw new ArgumentException("Value is not a number (NaN).");
            _desiredInput = value;
            _setInputFlag = true;
        }

        /// <summary>
        /// Sets the desired input value to be read at the next FixedUpdate of the BlockBehaviour script.
        /// </summary>
        protected override void LateUpdate()
        {
            if (_setInputFlag)
            {
                InputFieldInfo.SetValue(_cmc, _desiredInput);
                _setInputFlag = false;
            }
        }
    }
}
