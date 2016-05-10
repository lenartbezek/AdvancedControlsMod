namespace AdvancedControls.Axes
{
    public abstract class Axis
    {
        public virtual string Name { get; set; } = "new axis";
        public virtual float Input { get; } = 0;
        public virtual float Output { get; set; } = 0;

        public Axis()
        {
            ADVControls.Instance.OnUpdate += Update;
            ADVControls.Instance.OnReset += Reset;
        }

        public abstract void Reset();
        public abstract void Update();
    }
}
