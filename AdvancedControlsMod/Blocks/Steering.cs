using System;
using System.Reflection;
using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    ///     Handler for steering blocks; Steering and Steering Hinge.
    /// </summary>
    public class Steering : Block
    {
        private static readonly FieldInfo AngleyToBeField = typeof(SteeringWheel).GetField("angleyToBe",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo SpeedSliderField = typeof(SteeringWheel).GetField("speedSlider",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo LimitsSliderField = typeof(SteeringWheel).GetField("limitsSlider",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly MLimits _limitsSlider;
        private readonly MSlider _speedSlider;
        private readonly SteeringWheel _sw;

        private float _desiredAngle;
        private float _desiredInput;
        private bool _setAngleFlag;
        private bool _setInputFlag;

        /// <summary>
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Steering(BlockBehaviour bb) : base(bb)
        {
            _sw = bb.GetComponent<SteeringWheel>();
            _speedSlider = SpeedSliderField.GetValue(_sw) as MSlider;
            _limitsSlider = LimitsSliderField.GetValue(_sw) as MLimits;
        }

        /// <summary>
        ///     Analog input for the steering block.
        ///     0 for no rotation, 1 for normal speed rotation.
        /// </summary>
        public float Input
        {
            set { SetInput(value); }
        }

        /// <summary>
        ///     Angle for the steering block in degrees.
        /// </summary>
        public float Angle
        {
            get { return GetAngle(); }
            set { SetAngle(value); }
        }

        private void SetInput(float value)
        {
            if (float.IsNaN(value))
                throw new ArgumentException("Value is not a number (NaN).");
            _desiredInput = value * (_sw.Flipped ? -1 : 1);
            _setInputFlag = true;
            _setAngleFlag = false;
        }

        private void SetAngle(float angle)
        {
            angle *= _sw.Flipped ? -1 : 1;
            if (float.IsNaN(angle))
                throw new ArgumentException("Value is not a number (NaN).");
            if (_sw.allowLimits && _limitsSlider.IsActive)
                _desiredAngle = _sw.Flipped
                    ? Mathf.Clamp(angle, -_limitsSlider.Min, _limitsSlider.Max)
                    : Mathf.Clamp(angle, -_limitsSlider.Max, _limitsSlider.Min);
            else
                _desiredAngle = angle;

            _setAngleFlag = true;
        }

        /// <summary>
        ///     Returns the current angle of the joint.
        /// </summary>
        /// <returns>Float value in degrees or radians as specified.</returns>
        private float GetAngle()
        {
            return (float) AngleyToBeField.GetValue(_sw);
        }

        /// <summary>
        ///     Handles the movement of the joint.
        /// </summary>
        protected override void LateUpdate()
        {
            if (_setAngleFlag)
            {
                var currentAngle = (float) AngleyToBeField.GetValue(_sw);
                if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, _desiredAngle)) < 0.1)
                {
                    _setAngleFlag = false;
                }
                else
                {
                    _desiredInput = Mathf.DeltaAngle(currentAngle, _desiredAngle) /
                                    (100f * _sw.targetAngleSpeed * _speedSlider.Value * Time.deltaTime);
                    _desiredInput = Mathf.Clamp(_desiredInput, -1, 1);
                    _setInputFlag = true;
                }
            }

            if (_setInputFlag)
            {
                if (_speedSlider.Value != 0)
                {
                    _sw.GetComponent<Rigidbody>()?.WakeUp();

                    var speed = _desiredInput * 100f * _sw.targetAngleSpeed * _speedSlider.Value;

                    var currentAngle = (float) AngleyToBeField.GetValue(_sw);
                    var newAngle = currentAngle + speed * Time.deltaTime;

                    if (_sw.allowLimits && _limitsSlider.IsActive)
                        newAngle = _sw.Flipped
                            ? Mathf.Clamp(newAngle, -_limitsSlider.Min, _limitsSlider.Max)
                            : Mathf.Clamp(newAngle, -_limitsSlider.Max, _limitsSlider.Min);
                    else if (newAngle > 180)
                        newAngle -= 360;
                    else if (newAngle < -180)
                        newAngle += 360;

                    AngleyToBeField.SetValue(_sw, newAngle);
                }
                _setInputFlag = false;
            }
        }
    }
}