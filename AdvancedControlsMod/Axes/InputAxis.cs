using System;
using UnityEngine;

namespace Lench.AdvancedControls.Axes
{
#pragma warning disable CS1591
    public enum AxisType
    {
        Controller = 1,
        Inertial = 2, // deprecated
        Standard = 3, // deprecated
        Custom = 4,
        Chain = 5,
        Mouse = 6,
        Key = 7
    }

    public enum AxisStatus
    {
        OK = 0,
        NotFound = 1,
        Unavailable = 2,
        Disconnected = 3,
        NotRunning = 4,
        Error = 5,
        NoLink = 6
    }
#pragma warning restore CS1591

    /// <summary>
    ///     Abstract class that defines the input axis frame.
    /// </summary>
    [Serializable]
    public abstract class InputAxis : IEquatable<InputAxis>
    {
        /// <summary>
        ///     Initializes a new axis with given name.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        protected InputAxis(string name)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Name = name;
            Mod.OnUpdate += Update;
            Block.OnInitialisation += Initialise;
        }

        /// <summary>
        ///     Unique name of the axis.
        /// </summary>
        public virtual string Name { get; internal set; }

        /// <summary>
        ///     Raw input value.
        /// </summary>
        public virtual float InputValue { get; } = 0;

        /// <summary>
        ///     Output value that is later processed by controls.
        ///     Axis only gives correct output value if status equals AxisStatus.OK
        /// </summary>
        public virtual float OutputValue { get; protected set; } = 0;

        /// <summary>
        ///     Is the associated device connected.
        /// </summary>
        public virtual bool Connected { get; } = true;

        /// <summary>
        ///     Can axis be saved.
        /// </summary>
        public virtual bool Saveable { get; } = true;

        /// <summary>
        ///     Current status of the axis.
        ///     Axis only gives correct output value if the status equals AxisStatus.OK
        /// </summary>
        public virtual AxisStatus Status { get; } = AxisStatus.OK;

        /// <summary>
        ///     Type of the axis.
        /// </summary>
        public AxisType Type { get; protected set; }

        /// <summary>
        ///     Compares axis tuning parameters.
        /// </summary>
        /// <param name="other">Axis to compare with.</param>
        /// <returns>Returns true if axes are identical.</returns>
        public abstract bool Equals(InputAxis other);

        /// <summary>
        ///     Disposes axis and unsubscribes it from update events.
        /// </summary>
        public void Dispose()
        {
            Mod.OnUpdate -= Update;
            Block.OnInitialisation -= Initialise;
        }

        /// <summary>
        ///     Initializes axis. Called on simulation start by OnInitialisation event.
        ///     Intended to reset or initialize variables.
        /// </summary>
        protected abstract void Initialise();

        /// <summary>
        ///     Updates axis values. Called on every frame.
        /// </summary>
        protected abstract void Update();

        internal abstract InputAxis Clone();

        internal virtual void Load(string json)
        {
            JsonUtility
        }

        internal virtual string Save()
        {
            
        }

        internal virtual void Save(MachineInfo machineInfo)
        {
            
        }

        internal virtual void Delete()
        {
            Dispose();
        }
    }
}