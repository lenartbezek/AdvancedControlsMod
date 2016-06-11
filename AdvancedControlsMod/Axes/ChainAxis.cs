using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedControls.Axes
{
    public class ChainAxis : InputAxis
    {
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
                var axis_a = AxisManager.Get(SubAxis1) as ChainAxis;
                var axis_b = AxisManager.Get(SubAxis2) as ChainAxis;
                bool error = false;
                string error_message = "Renaming this axis caused a cycle in the chain.\n";
                if (axis_a != null && axis_a.CheckCycle(new List<string>() { }))
                {
                    error = true;
                    error_message += "'" + SubAxis1 + "' has been removed.\n";
                    SubAxis1 = null;
                }
                if (axis_b != null && axis_b.CheckCycle(new List<string>() { }))
                {
                    error = true;
                    error_message += "'" + SubAxis2 + "' has been removed.\n";
                    SubAxis2 = null;
                }
                if (error)
                {
                    throw new InvalidOperationException(error_message);
                }
            }
        }

        private string sub_axis1;
        public string SubAxis1
        {
            get
            {
                return sub_axis1;
            }
            set
            {
                sub_axis1 = value;
                var axis = AxisManager.Get(value) as ChainAxis;
                if (axis != null && axis.CheckCycle(new List<string>() { }))
                {
                    sub_axis1 = null;
                    throw new InvalidOperationException("'" + value + "' is already in the axis chain.\nAdding it here would create a cycle.");
                }
            }
        }

        private string sub_axis2;
        public string SubAxis2
        {
            get
            {
                return sub_axis2;
            }
            set
            {
                sub_axis2 = value;
                var axis = AxisManager.Get(sub_axis2) as ChainAxis;
                if (axis != null && axis.CheckCycle(new List<string>() { }))
                {
                    sub_axis2 = null;
                    throw new InvalidOperationException("'" + value + "' is already in the axis chain.\nAdding it here would create a cycle.");
                }
            }
        }

        public ChainMethod Method { get; set; }

        public enum ChainMethod
        {
            Sum = 0,
            Subtract = 1,
            Average = 2,
            Multiply = 3,
            Maximum = 4,
            Minimum = 5
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
                if (Method == ChainMethod.Subtract)
                    return Mathf.Clamp(a - b, -1, 1);
                if (Method == ChainMethod.Average)
                    return Mathf.Clamp((a + b) / 2, -1, 1);
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

        private bool CheckCycle(List<string> path)
        {
            if (path.Contains(Name))
                return true;
            var sub1 = AxisManager.Get(SubAxis1) as ChainAxis;
            var sub2 = AxisManager.Get(SubAxis2) as ChainAxis;
            bool cycle = false;
            if (sub1 != null)
            {
                var new_path = new List<string>();
                new_path.AddRange(path);
                new_path.Add(Name);
                cycle |= sub1.CheckCycle(new_path);
            }
            if (sub2 != null)
            {
                var new_path = new List<string>();
                new_path.AddRange(path);
                new_path.Add(Name);
                cycle |= sub2.CheckCycle(new_path);
            }
            return cycle;
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
