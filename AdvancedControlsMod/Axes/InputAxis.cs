using System;
using Lench.AdvancedControls.UI;

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
    /// Abstract class that defines the input axis frame.
    /// </summary>
    public abstract class InputAxis : IEquatable<InputAxis>
    {
        /// <summary>
        /// Unique name of the axis.
        /// </summary>
        public virtual string Name { get; internal set; }

        /// <summary>
        /// Raw input value.
        /// </summary>
        public virtual float InputValue { get; } = 0;

        /// <summary>
        /// Output value that is later processed by controls.
        /// Axis only gives correct output value if status equals AxisStatus.OK
        /// </summary>
        public virtual float OutputValue { get; protected set; } = 0;

        /// <summary>
        /// Is the associated device connected.
        /// </summary>
        public virtual bool Connected { get; } = true;

        /// <summary>
        /// Can axis be saved.
        /// </summary>
        public virtual bool Saveable { get; } = true;

        /// <summary>
        /// Current status of the axis.
        /// Axis only gives correct output value if the status equals AxisStatus.OK
        /// </summary>
        public virtual AxisStatus Status { get; } = AxisStatus.OK;

        /// <summary>
        /// Type of the axis.
        /// </summary>
        public AxisType Type { get; protected set; }

        internal IAxisEditor Editor;

        /// <summary>
        /// Initializes a new axis with given name.
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
        /// Disposes axis and unsubscribes it from update events.
        /// </summary>
        public void Dispose()
        {
            Mod.OnUpdate -= Update;
            Block.OnInitialisation -= Initialise;
        }

        /// <summary>
        /// Initializes axis. Called on simulation start by OnInitialisation event.
        /// Intended to reset or initialize variables.
        /// </summary>
        protected abstract void Initialise();

        /// <summary>
        /// Updates axis values. Called on every frame.
        /// </summary>
        protected abstract void Update();

        internal virtual IAxisEditor GetEditor()
        {
            return Editor;
        }

        internal abstract InputAxis Clone();
        internal abstract void Load();
        internal abstract void Load(MachineInfo machineInfo);
        internal abstract void Save();
        internal abstract void Save(MachineInfo machineInfo);
        internal abstract void Delete();

        /// <summary>
        /// Returns string representation of the axis status.
        /// </summary>
        public string GetStatusString()
        {
            return GetStatusString(Status);
        }

        /// <summary>
        /// Returns string representation of a given status enumerator.
        /// </summary>
        public static string GetStatusString(AxisStatus status)
        {
            switch (status)
            {
                case AxisStatus.OK:
                    return Strings.AxisStatus_Ok;
                case AxisStatus.NotFound:
                    return Strings.AxisStatus_NotFound;
                case AxisStatus.Unavailable:
                    return Strings.AxisStatus_NotAvailable;
                case AxisStatus.Disconnected:
                    return Strings.AxisStatus_NotConnected;
                case AxisStatus.NotRunning:
                    return Strings.AxisStatus_NotRunning;
                case AxisStatus.Error:
                    return Strings.AxisStatus_Error;
                case AxisStatus.NoLink:
                    return Strings.AxisStatus_NoLink;
                default:
                    return Strings.AxisStatus_Unknown;
            }
        }

        /// <summary>
        /// Compares axis tuning parameters.
        /// </summary>
        /// <param name="other">Axis to compare with.</param>
        /// <returns>Returns true if axes are identical.</returns>
        public abstract bool Equals(InputAxis other);
    }
}
