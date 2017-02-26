using UnityEngine;

namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    ///     Handler for the Spring and Rope blocks.
    /// </summary>
    public class Spring : Block
    {
        private readonly SpringCode _sc;

        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Spring(BlockBehaviour bb) : base(bb)
        {
            _sc = bb.GetComponent<SpringCode>();
        }

        /// <summary>
        ///     Analog input for the spring and rope blocks.
        ///     Positive for contracting, negative for unwinding.
        /// </summary>
        public float Input
        {
            set { SetInput(value); }
        }

        /// <summary>
        ///     Controls the spring / winch. Contracts if rate is positive and unwinds if negative.
        ///     Springs cannot be unwound.
        /// </summary>
        /// <param name="rate">Rate of movement.</param>
        private void SetInput(float rate = 1)
        {
            if (Mathf.Abs(rate) < 0.02) return;
            if (_sc.winchMode)
                if (rate > 0)
                    _sc.WinchContract(rate);
                else
                    _sc.WinchUnwind(-rate);
            else
                _sc.Contract(rate);
        }

        /// <summary>
        ///     Contracts the spring.
        /// </summary>
        public void Contract(float rate = 1)
        {
            _sc.Contract(rate);
        }

        /// <summary>
        ///     Winds the winch.
        /// </summary>
        public void Wind(float rate = 1)
        {
            _sc.WinchContract(rate);
        }

        /// <summary>
        ///     Unwinds the winch.
        /// </summary>
        public void Unwind(float rate = 1)
        {
            _sc.WinchUnwind(rate);
        }
    }
}