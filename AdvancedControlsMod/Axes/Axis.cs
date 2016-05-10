namespace AdvancedControlsMod
{
    public abstract class Axis
    {
        public virtual string Name { get; set; } = "new axis";
        public virtual float Input { get; } = 0;
        public virtual float Output { get; set; } = 0;

        public Axis()
        {
            AdvancedControls.Instance.OnUpdate += Update;
            AdvancedControls.Instance.OnReset += Reset;
        }

        public abstract void Reset();
        public abstract void Update();
    }
}
