using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lench.AdvancedControls.Axes
{
    /// <summary>
    /// Chain axis links two axes together and combines them in a new axis.
    /// </summary>
    public class ChainAxis : InputAxis
    {
        /// <summary>
        /// Name of the axis.
        /// Changing it checks for chain cycles and removes links if a cycle is detected.
        /// </summary>
        public override string Name
        {
            get
            {
                return base.Name;
            }

            internal set
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

        /// <summary>
        /// Name of the left linked axis.
        /// Changing it checks for a created cycle.
        /// </summary>
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
        private string sub_axis1;

        /// <summary>
        /// name of the right linked axis.
        /// Changing it checks for a created cycle.
        /// </summary>
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
        private string sub_axis2;

        /// <summary>
        /// The method that determines how the axes are combined into a single one.
        /// </summary>
        public ChainMethod Method { get; set; }

        /// <summary>
        /// Chain method enumerator.
        /// </summary>
        public enum ChainMethod
        {
            /// <summary>
            /// Sums both axis values.
            /// </summary>
            Sum = 0,
            /// <summary>
            /// Subtracts right axis value from the left axis value.
            /// </summary>
            Subtract = 1,
            /// <summary>
            /// Calculates average value.
            /// </summary>
            Average = 2,
            /// <summary>
            /// Multiplies both axis values.
            /// </summary>
            Multiply = 3,
            /// <summary>
            /// Takes the maximum value.
            /// </summary>
            Maximum = 4,
            /// <summary>
            /// Takes the minimum value.
            /// </summary>
            Minimum = 5
        }

        /// <summary>
        /// Output value based on linked axes values and the chaining method.
        /// </summary>
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

        /// <summary>
        /// Creates a new chain axis with given name and no links.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        public ChainAxis(string name) : base(name)
        {
            Type = AxisType.Chain;
            base.Name = name;
            SubAxis1 = null;
            SubAxis2 = null;
            Method = ChainMethod.Sum;
            editor = new UI.ChainAxisEditor(this);
        }

        /// <summary>
        /// Retrieves linked axes again.
        /// Intended to be called after loading.
        /// </summary>
        public void RefreshLinks()
        {
            SubAxis1 = sub_axis1;
            SubAxis2 = sub_axis2;
        }

        /// <summary>
        /// Chain axis status distinguishes between No Link and OK.
        /// </summary>
        public override AxisStatus Status
        {
            get
            {
                if (sub_axis1 == null && sub_axis2 == null) return AxisStatus.NoLink;
                return AxisStatus.OK;
            }
        }

        /* Recursively traverse the chain axis tree depth-first.
         * Keeps list of already visited names as path.
         * Duplicate in path means there is a cycle.
         */
        private bool CheckCycle(List<string> path)
        {
            // Check for duplicate
            if (path.Contains(Name))
                return true;
            var sub1 = AxisManager.Get(SubAxis1) as ChainAxis;
            var sub2 = AxisManager.Get(SubAxis2) as ChainAxis;
            bool cycle = false;
            // Traverse left node
            if (sub1 != null)
            {
                var new_path = new List<string>();
                new_path.AddRange(path);
                new_path.Add(Name);
                cycle |= sub1.CheckCycle(new_path);
            }
            // Traverse right node
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
            Method = (ChainMethod)Enum.Parse(typeof(ChainMethod), spaar.ModLoader.Configuration.GetString("axis-" + Name + "-method", "Sum"));
            SubAxis1 = spaar.ModLoader.Configuration.GetString("axis-" + Name + "-subaxis1", null);
            SubAxis2 = spaar.ModLoader.Configuration.GetString("axis-" + Name + "-subaxis2", null);
        }

        internal override void Load(MachineInfo machineInfo)
        {
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-method"))
                Method = (ChainMethod)Enum.Parse(typeof(ChainMethod), machineInfo.MachineData.ReadString("axis-" + Name + "-method"));
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-subaxis1"))
                SubAxis1 = machineInfo.MachineData.ReadString("axis-" + Name + "-subaxis1");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-subaxis2"))
                SubAxis2 = machineInfo.MachineData.ReadString("axis-" + Name + "-subaxis2");
        }

        internal override void Save()
        {
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-type", Type.ToString());
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-method", Method.ToString());
            if (SubAxis1 != null)
                spaar.ModLoader.Configuration.SetString("axis-" + Name + "-subaxis1", SubAxis1);
            else
                spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-subaxis1");
            if (SubAxis2 != null)
                spaar.ModLoader.Configuration.SetString("axis-" + Name + "-subaxis2", SubAxis2);
            else
                spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-subaxis2");
        }

        internal override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("axis-" + Name + "-type", Type.ToString());
            machineInfo.MachineData.Write("axis-" + Name + "-method", Method.ToString());
            if (SubAxis1 != null)
                machineInfo.MachineData.Write("axis-" + Name + "-subaxis1", SubAxis1);
            if (SubAxis2 != null)
                machineInfo.MachineData.Write("axis-" + Name + "-subaxis2", SubAxis2);
        }

        internal override void Delete()
        {
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-type");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-method");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-subaxis1");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-subaxis2");
            Dispose();
        }

        /// <summary>
        /// Chain axis requires no initialisation.
        /// </summary>
        protected override void Initialise() { }

        /// <summary>
        /// Chain axis requires no update.
        /// </summary>
        protected override void Update(){ }

        /// <summary>
        /// Compares axis tuning parameters.
        /// </summary>
        /// <param name="other">Axis to compare with.</param>
        /// <returns>Returns true if axes are identical.</returns>
        public override bool Equals(InputAxis other)
        {
            var cast = other as ChainAxis;
            if (cast == null) return false;
            return this.Name == cast.Name &&
                   this.Method == cast.Method &&
                   this.SubAxis1 == cast.SubAxis1 &&
                   this.SubAxis2 == cast.SubAxis2;
        }
    }
}
