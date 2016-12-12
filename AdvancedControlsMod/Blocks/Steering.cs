using System;
using System.Reflection;
using UnityEngine;
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Lench.AdvancedControls.Blocks
{
    /// <summary>
    /// Handler for steering blocks; Steering and Steering Hinge.
    /// </summary>
    public class Steering : BlockHandler
    {
        private static readonly FieldInfo AngleyToBeField = typeof(SteeringWheel).GetField("angleyToBe", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo SpeedSliderField = typeof(SteeringWheel).GetField("speedSlider", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo LimitsSliderField = typeof(SteeringWheel).GetField("limitsSlider", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly SteeringWheel _sw;

        private readonly MSlider _speedSlider;
        private readonly MLimits _limitsSlider;

        private float _desiredInput;
        private bool _setInputFlag;

        private float _desiredAngle;
        private bool _setAngleFlag;

        /// <summary>
        /// Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        public Steering(BlockBehaviour bb) : base(bb)
        {
            _sw = bb.GetComponent<SteeringWheel>();
            _speedSlider = SpeedSliderField.GetValue(_sw) as MSlider;
            _limitsSlider = LimitsSliderField.GetValue(_sw) as MLimits;
        }

        /// <summary>
        /// Invokes the block's action.
        /// Throws ActionNotFoundException if the block does not poses such action.
        /// </summary>
        /// <param name="actionName">Display name of the action.</param>
        public override void Action(string actionName)
        {
            actionName = actionName.ToUpper();
            if (actionName == "LEFT")
            {
                SetInput(1);
                return;
            }
            if (actionName == "RIGHT")
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
            _desiredInput = value * (_sw.Flipped ? -1 : 1);
            _setInputFlag = true;
            _setAngleFlag = false;
        }

        /// <summary>
        /// Moves the joint to the desired angle.
        /// </summary>
        /// <param name="angle">Float value in degrees.</param>
        public void SetAngle(float angle)
        {
            angle /= convertToRadians;
            angle *= (_sw.Flipped ? -1 : 1);
            if (float.IsNaN(angle))
                throw new ArgumentException("Value is not a number (NaN).");
            if (_sw.allowLimits && _limitsSlider.IsActive)
            {
                if (_sw.Flipped)
                    _desiredAngle = Mathf.Clamp(angle, -_limitsSlider.Min, _limitsSlider.Max);
                else
                    _desiredAngle = Mathf.Clamp(angle, -_limitsSlider.Max, _limitsSlider.Min);
            }
            else
            {
                _desiredAngle = angle;
            }

            _setAngleFlag = true;
        }

        /// <summary>
        /// Returns the current angle of the joint.
        /// </summary>
        /// <returns>Float value in degrees or radians as specified.</returns>
        public float GetAngle()
        {
            return (float)AngleyToBeField.GetValue(_sw) * convertToRadians;
        }

        /// <summary>
        /// Handles the movement of the joint.
        /// </summary>
        protected override void LateUpdate()
        {
            if (_setAngleFlag)
            {
                float currentAngle = (float)AngleyToBeField.GetValue(_sw);
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

                    float speed = _desiredInput * 100f * _sw.targetAngleSpeed * _speedSlider.Value;

                    float currentAngle = (float)AngleyToBeField.GetValue(_sw);
                    float newAngle = currentAngle + speed * Time.deltaTime;

                    if (_sw.allowLimits && _limitsSlider.IsActive)
                    {
                        newAngle = _sw.Flipped 
                            ? Mathf.Clamp(newAngle, -_limitsSlider.Min, _limitsSlider.Max) 
                            : Mathf.Clamp(newAngle, -_limitsSlider.Max, _limitsSlider.Min);
                    }
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
