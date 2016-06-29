using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedControls.Axes
{
    public class ChainAxis : InputAxis
    {
        public override AxisType Type { get { return AxisType.Chain; } }

        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
                var sub1 = AxisManager.Get(SubAxis1) as ChainAxis;
                var sub2 = AxisManager.Get(SubAxis2) as ChainAxis;
                bool error = false;
                string error_message = "Renaming this axis formed a cycle in the chain.\n";
                if (sub1 != null && sub1.CheckCycle(new List<string>() { Name }))
                {
                    error = true;
                    error_message += "'" + SubAxis1 + "' has been unlinked. ";
                    SubAxis1 = null;
                }
                if (sub2 != null && sub2.CheckCycle(new List<string>() { Name }))
                {
                    error = true;
                    error_message += "'" + SubAxis2 + "' has been unlinked. ";
                    SubAxis2 = null;
                }
                if (error)
                {
                    (editor as UI.ChainAxisEditor).error = "<color=#FFFF00><b>Chain cycle error</b></color>\n" + error_message;
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
                    throw new InvalidOperationException("'" + value + "' is already in the axis chain.\nLinking it here would create a cycle.");
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
                    throw new InvalidOperationException("'" + value + "' is already in the axis chain.\nLinking it here would create a cycle.");
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

        public ChainAxis(string name) : base(name)
        {
            base.Name = "new chain axis";
            SubAxis1 = null;
            SubAxis2 = null;
            Method = ChainMethod.Sum;
            editor = new UI.ChainAxisEditor(this);
        }

        public override string Status
        {
            get
            {
                if (sub_axis1 == null && sub_axis2 == null) return "NO LINK";
                return "OK";
            }
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

        internal override InputAxis Clone()
        {
            var clone = new ChainAxis(Name);
            clone.Method = Method;
            clone.SubAxis1 = SubAxis1;
            clone.SubAxis2 = SubAxis2;
            return clone;
        }

        internal override void Load()
        {
            if (spaar.ModLoader.Configuration.DoesKeyExist("axis-" + Name + "-method"))
                Method = (ChainMethod)Enum.Parse(typeof(ChainMethod), spaar.ModLoader.Configuration.GetString("axis-" + Name + "-method", "Sum"));
            if (spaar.ModLoader.Configuration.DoesKeyExist("axis-" + Name + "-subaxis1"))
                SubAxis1 = spaar.ModLoader.Configuration.GetString("axis-" + Name + "-subaxis1", null);
            if (spaar.ModLoader.Configuration.DoesKeyExist("axis-" + Name + "-subaxis2"))
                SubAxis2 = spaar.ModLoader.Configuration.GetString("axis-" + Name + "-subaxis2", null);
        }

        internal override void Save()
        {
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-type", Type.ToString());
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-method", Method.ToString());
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-subaxis1", SubAxis1);
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-subaxis2", SubAxis2);
        }

        internal override void Delete()
        {
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-type");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-method");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-subaxis1");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-subaxis2");
            Dispose();
        }

        protected override void Initialise() { }
        protected override void Update(){ }
    }
}
