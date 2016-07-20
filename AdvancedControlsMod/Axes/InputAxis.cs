namespace Lench.AdvancedControls.Axes
{
    public enum AxisType
    {
        Controller = 1,
        Inertial = 2,
        Standard = 3,
        Custom = 4,
        Chain = 5
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

    public abstract class InputAxis
    {

        public virtual string Name { get; set; } = "new axis";
        public virtual float InputValue { get; } = 0;
        public virtual float OutputValue { get; set; } = 0;
        public virtual bool Connected { get; } = true;
        public virtual bool Saveable { get; } = true;
        public virtual AxisStatus Status { get; } = AxisStatus.OK;
        public abstract AxisType Type { get; }

        internal UI.AxisEditor editor;

        public InputAxis(string name)
        {
            Name = name;
            ACM.Instance.OnUpdate += Update;
            ACM.Instance.OnInitialisation += Initialise;
        }

        internal void Dispose()
        {
            ACM.Instance.OnUpdate -= Update;
            ACM.Instance.OnInitialisation -= Initialise;
        }

        protected abstract void Initialise();
        protected abstract void Update();

        internal virtual UI.AxisEditor GetEditor()
        {
            return editor;
        }

        internal abstract InputAxis Clone();
        internal abstract void Load();
        internal abstract void Save();
        internal abstract void Delete();

        public string StatusString()
        {
            return GetStatusString(Status);
        }

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
    }
}
