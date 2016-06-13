namespace AdvancedControls.Axes
{
    public enum AxisType
    {
        Placeholder = 0,
        Controller = 1,
        Inertial = 2,
        Standard = 3,
        Custom = 4,
        Chain = 5
    }

    public abstract class InputAxis
    {
        public AxisType Type = AxisType.Placeholder;
        public virtual string Name { get; set; } = "new axis";
        public virtual float InputValue { get; } = 0;
        public virtual float OutputValue { get; set; } = 0;
        public virtual bool Connected { get; } = true;
        public virtual bool Saveable { get; } = true;

        protected UI.AxisEditor editor;

        public InputAxis(string name)
        {
            Name = name;
            ACM.Instance.OnUpdate += Update;
            ACM.Instance.OnInitialisation += Initialise;
        }

        public abstract void Initialise();
        public abstract void Update();

        public virtual UI.AxisEditor GetEditor()
        {
            return editor;
        }

        public abstract InputAxis Clone();
        public abstract void Load();
        public abstract void Save();
        public abstract void Delete();
    }
}
