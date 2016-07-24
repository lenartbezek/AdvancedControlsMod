using System;

namespace Lench.AdvancedControls.Axes
{

#pragma warning disable CS1591
    public enum AxisType
    {
        Controller = 1,
        Inertial = 2,
        Standard = 3,
        Custom = 4,
        Chain = 5,
        Mouse = 6
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
        public virtual string Name { get; internal set; } = "new axis";

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

        internal UI.AxisEditor editor;

        /// <summary>
        /// Initializes a new axis with given name.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        public InputAxis(string name)
        {
            Name = name;
            ACM.Instance.OnUpdate += Update;
            ACM.Instance.OnInitialisation += Initialise;
        }

        /// <summary>
        /// Disposes axis and unsubscribes it from update events.
        /// </summary>
        public void Dispose()
        {
            ACM.Instance.OnUpdate -= Update;
            ACM.Instance.OnInitialisation -= Initialise;
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

        internal virtual UI.AxisEditor GetEditor()
        {
            return editor;
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
                    return "OK";
                case AxisStatus.NotFound:
                    return "NOT FOUND";
                case AxisStatus.Unavailable:
                    return "NOT AVAILABLE";
                case AxisStatus.Disconnected:
                    return "NOT CONNECTED";
                case AxisStatus.NotRunning:
                    return "NOT RUNNING";
                case AxisStatus.Error:
                    return "ERROR";
                case AxisStatus.NoLink:
                    return "NO LINK";
                default:
                    return "UNKNOWN";
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
