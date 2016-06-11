using System;
using UnityEngine;

namespace AdvancedControls.Axes
{
    public class ChainAxis : InputAxis
    {
        public string SubAxis1 { get; set; }
        public string SubAxis2 { get; set; }
        public ChainMethod Method { get; set; }

        public enum ChainMethod
        {
            Sum = 0,
            Multiply = 1,
            Maximum = 2,
            Minimum = 3
        }

        public override float OutputValue {
            get
            {
                var axis_a = AxisManager.Get(SubAxis1);
                var axis_b = AxisManager.Get(SubAxis2);
                float a = axis_a != null ? axis_a.OutputValue : 0;
                float b = axis_b != null ? axis_b.OutputValue : 0;
                if (Method == ChainMethod.Sum)
                    return Mathf.Clamp(a + b, -1, 1);
                if (Method == ChainMethod.Multiply)
                    return Mathf.Clamp(a * b, -1, 1);
                if (Method == ChainMethod.Maximum)
                    return Mathf.Clamp(Mathf.Max(a, b), -1, 1);
                if (Method == ChainMethod.Minimum)
                    return Mathf.Clamp(Mathf.Min(a, b), -1, 1);
                return 0;
            }
        }

        public ChainAxis(string name, string a = null, string b = null, ChainMethod m = ChainMethod.Sum) : base(name)
        {
            SubAxis1 = a;
            SubAxis2 = b;
            Method = m;
            editor = new UI.ChainAxisEditor(this);
        }

        public override InputAxis Clone()
        {
            return new ChainAxis(Name, SubAxis1, SubAxis2, Method);
        }

        public override void Load(MachineInfo machineInfo)
        {
            throw new NotImplementedException();
        }

        public override void Save(MachineInfo machineInfo)
        {
            throw new NotImplementedException();
        }

        public override void Initialise() { }
        public override void Update(){ }
    }
}
