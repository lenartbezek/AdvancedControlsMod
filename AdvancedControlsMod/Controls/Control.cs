using System;
using UnityEngine;
using Lench.AdvancedControls.Blocks;
using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.UI;
using spaar.ModLoader;
// ReSharper disable SpecifyACultureInStringConversionExplicitly
// ReSharper disable VirtualMemberCallInConstructor

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Control class that takes value from an input axis, interpolates it in given range and applies it to the block.
    /// </summary>
    public abstract class Control
    {
        /// <summary>
        /// Name of the control. Displayed in the control mapper.
        /// </summary>
        public virtual string Name { get; set; } = Strings.ControlName_Default;

        /// <summary>
        /// Control key for saving, loading and copying. Should be unique per block.
        /// </summary>
        public virtual string Key { get; set; } = "CONTROL";

        /// <summary>
        /// Is the control enabled and applies value to the block on each frame.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Are control intervals set to be positive only.
        /// </summary>
        public virtual bool PositiveOnly { get; set; } = false;

        /// <summary>
        /// Bound axis name.
        /// </summary>
        public string Axis { get; set; }

        /// <summary>
        /// BlockHandler object of the control's block.
        /// </summary>
        public virtual BlockHandler Block { get; protected set; }

        /// <summary>
        /// GUID of the block the control is bound to.
        /// </summary>
        public virtual Guid BlockGUID { get; set; }

        /// <summary>
        /// Minimum (Left) interval value. Applied when bound axis value is -1.
        /// </summary>
        public virtual float Min
        {
            get { return _min; }
            set
            {
                _min = PositiveOnly && value < 0 ? 0 : value;
                MinString = (Mathf.Round(_min * 100) / 100).ToString();
            }
        }

        /// <summary>
        /// Center interval value. Applied when bound axis value is 0.
        /// </summary>
        public virtual float Center
        {
            get { return _cen; }
            set
            {
                _cen = PositiveOnly && value < 0 ? 0 : value;
                CenString = (Mathf.Round(_cen * 100) / 100).ToString();
            }
        }

        /// <summary>
        /// Maximum (Right) interval value. Applied when bound axis value is +1.
        /// </summary>
        public virtual float Max
        {
            get { return _max; }
            set
            {
                _max = PositiveOnly && value < 0 ? 0 : value;
                MaxString = (Mathf.Round(_max * 100) / 100).ToString();
            }
        }

        private float _min = -1;
        private float _cen;
        private float _max = 1;

        internal string MinString;
        internal string CenString;
        internal string MaxString;

        /// <summary>
        /// Swaps the Min and Max control interval limits.
        /// </summary>
        public void Invert()
        {
            var tmp = _min;
            _min = _max;
            _max = tmp;
            var tmpString = MinString;
            MinString = MaxString;
            MaxString = tmpString;
        }

        /// <summary>
        /// Creates a control for a block with given GUID.
        /// </summary>
        /// <param name="guid">GUID of the block.</param>
        protected Control(Guid guid)
        {
            BlockGUID = guid;
            ACM.Instance.OnUpdate += Update;
            ACM.Instance.OnInitialisation += Initialise;

            MinString = (Mathf.Round(Min * 100) / 100).ToString();
            CenString = (Mathf.Round(Center * 100) / 100).ToString();
            MaxString = (Mathf.Round(Max * 100) / 100).ToString();
        }

        /// <summary>
        /// Is called on OnInitialisation to retrieve BlockHandler reference.
        /// If the control has no associated block, it is deleted.
        /// </summary>
        protected virtual void Initialise()
        {
            try
            {
                Block = BlockHandlerController.GetBlock(BlockGUID);

                var axis = AxisManager.Get(Axis);
                if (Enabled && Block != null && axis != null && axis.Status == AxisStatus.OK)
                {
                    ClearKeys();
                }
            }
            catch (BlockNotFoundException)
            {
                Block = null;
                ControlManager.Blocks.Remove(BlockGUID);
            }
        }

        /// <summary>
        /// Called on every frame to apply the value.
        /// </summary>
        protected virtual void Update()
        {
            if (!Game.IsSimulating) return;
            var axis = AxisManager.Get(Axis);
            if (Enabled && Block != null && axis != null && axis.Status == AxisStatus.OK)
            {
                Apply(axis.OutputValue);
            }
        }

        /// <summary>
        /// Applies the value to the block.
        /// </summary>
        /// <param name="value">Value to be applied.</param>
        protected abstract void Apply(float value);

        /// <summary>
        /// Clears keys to disable interference from vanilla controls.
        /// </summary>
        protected abstract void ClearKeys();

        internal abstract Control Clone();

        /// <summary>
        /// Loads the control from BlockInfo and enables it.
        /// </summary>
        /// <param name="blockInfo"></param>
        internal virtual void Load(BlockInfo blockInfo)
        {
            Axis = blockInfo.BlockData.ReadString("ac-control-" + Key + "-axis");
            if (blockInfo.BlockData.HasKey("ac-control-" + Key + "-min"))
                Min = blockInfo.BlockData.ReadFloat("ac-control-" + Key + "-min");
            if (blockInfo.BlockData.HasKey("ac-control-" + Key + "-center"))
                Center = blockInfo.BlockData.ReadFloat("ac-control-" + Key + "-center");
            if (blockInfo.BlockData.HasKey("ac-control-" + Key + "-max"))
                Max = blockInfo.BlockData.ReadFloat("ac-control-" + Key + "-max");
            Enabled = true;
        }

        /// <summary>
        /// Saves the control to BlockInfo.
        /// </summary>
        /// <param name="blockInfo"></param>
        internal virtual void Save(BlockInfo blockInfo)
        {
            blockInfo.BlockData.Write("ac-control-" + Key + "-axis", Axis);
            blockInfo.BlockData.Write("ac-control-" + Key + "-min", Min);
            blockInfo.BlockData.Write("ac-control-" + Key + "-center", Center);
            blockInfo.BlockData.Write("ac-control-" + Key + "-max", Max);
        }
    }
}
