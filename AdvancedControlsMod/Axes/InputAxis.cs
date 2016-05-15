namespace AdvancedControls.Axes
{
    public abstract class InputAxis
    {
        public virtual string Name { get; set; } = "new axis";
        public virtual float InputValue { get; } = 0;
        public virtual float OutputValue { get; set; } = 0;

        protected UI.AxisEditor editor;

        public InputAxis(string name)
        {
            Name = name;
            ADVControls.Instance.OnUpdate += Update;
            ADVControls.Instance.OnInitialisation += Initialise;
        }

        public abstract void Initialise();
        public abstract void Update();

        public virtual UI.AxisEditor GetEditor()
        {
            return editor;
        }

        public abstract InputAxis Clone();
        public abstract void Load(MachineInfo machineInfo);
        public abstract void Save(MachineInfo machineInfo);
    }
}
