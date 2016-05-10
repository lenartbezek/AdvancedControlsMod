using System;
using System.Collections.Generic;
using LenchScripter;
using LenchScripter.Blocks;

namespace AdvancedControls.Controls
{
    public abstract class Control
    {
        public virtual float Min { get; set; } = -1;
        public virtual float Max { get; set; } = 1;
        public virtual float Center { get; set; } = 0;
        public virtual bool Enabled { get; set; } = false;
        public virtual Axes.Axis Axis { get; set; }
        public virtual Block Block { get; set; }
        public virtual string BlockGUID { get; set; }

        public Control(string guid)
        {
            BlockGUID = guid;
            ADVControls.Instance.OnUpdate += Update;
            ADVControls.Instance.OnReset += Reset;
        }

        public virtual void Reset()
        {
            Block = BlockHandlers.GetBlock(BlockGUID);
        }

        public virtual void Update()
        {
            if(Enabled)
                Apply(Axis.Output);
        }

        public abstract void Apply(float value);
        public abstract void Draw();
    }

    public class ControlGroup : Control
    {
        public ControlGroup(string guid) : base(guid){}

        public Dictionary<string, Control> Controls { get; set; }
        public new string Enabled
        {
            get { return Enabled; }
            set
            {
                foreach (KeyValuePair<string, Control> c in Controls)
                {
                    c.Value.Enabled = c.Key == value;
                }
                Enabled = value;
            }
        }

        public override Block Block
        {
            get { return Block; }
            set
            {
                foreach(KeyValuePair<string, Control> c in Controls)
                {
                    c.Value.Block = Block;
                }
                Block = value;
            }
        }

        public override string BlockGUID
        {
            get { return BlockGUID; }
            set
            {
                foreach (KeyValuePair<string, Control> c in Controls)
                {
                    c.Value.BlockGUID = value;
                }
                BlockGUID = value;
            }
        }

        public override void Apply(float value){ }

        public override void Draw()
        {
            foreach (KeyValuePair<string, Control> c in Controls)
            {
                c.Value.Draw();
            }
        }
    }
}
