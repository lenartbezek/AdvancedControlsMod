namespace AdvancedControls.Axes
{
    public abstract class Axis
    {
        public virtual string Name { get; set; } = "new axis";
        public virtual float InputValue { get; } = 0;
        public virtual float OutputValue { get; set; } = 0;

        public Axis()
        {
            ADVControls.Instance.OnUpdate += Update;
            ADVControls.Instance.OnInitialisation += Initialise;
        }

        public abstract void Initialise();
        public abstract void Update();
    }
}
