using System;
using System.Reflection;

namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    ///     Handler for all wheel and cog blocks.
    /// </summary>
    public class Cog : Block
    {
        private static readonly FieldInfo InputFieldInfo = typeof(CogMotorControllerHinge).GetField("input",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly CogMotorControllerHinge _cmc;

        private float _desiredInput;
        private bool _setInputFlag;

        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Cog(BlockBehaviour bb) : base(bb)
        {
            _cmc = bb.GetComponent<CogMotorControllerHinge>();
        }

        /// <summary>
        ///     Analog input for the cog block.
        ///     0 for no rotation, 1 for normal speed rotation.
        /// </summary>
        public float Input
        {
            get { return (float) InputFieldInfo.GetValue(_cmc); }
            set { SetInput(value); }
        }

        private void SetInput(float value)
        {
            if (float.IsNaN(value))
                throw new ArgumentException("Value is not a number (NaN).");
            _desiredInput = value;
            _setInputFlag = true;
        }

        /// <summary>
        ///     Sets the desired input value to be read at the next FixedUpdate of the BlockBehaviour script.
        /// </summary>
        protected override void LateUpdate()
        {
            if (!_setInputFlag) return;

            InputFieldInfo.SetValue(_cmc, _desiredInput);
            _setInputFlag = false;
        }
    }
}