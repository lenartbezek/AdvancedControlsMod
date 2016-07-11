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

    public abstract class InputAxis
    {

        public virtual string Name { get; set; } = "new axis";
        public virtual float InputValue { get; } = 0;
        public virtual float OutputValue { get; set; } = 0;
        public virtual bool Connected { get; } = true;
        public virtual bool Saveable { get; } = true;
        public virtual string Status { get; } = "OK";
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
    }
}
