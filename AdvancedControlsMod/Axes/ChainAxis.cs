using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable PossibleNullReferenceException

namespace Lench.AdvancedControls.Axes
{
    /// <summary>
    /// Chain axis links two axes together and combines them in a new axis.
    /// </summary>
    [Serializable]
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
                var oldName = base.Name;
                base.Name = value;
                var sub1 = AxisManager.Get(SubAxis1) as ChainAxis;
                var sub2 = AxisManager.Get(SubAxis2) as ChainAxis;
                if ((sub1 != null && sub1.CheckCycle(new List<string> {Name})) ||
                    (sub2 != null && sub2.CheckCycle(new List<string> {Name})))
                {
                    base.Name = oldName;
                    throw new InvalidOperationException("Renaming this axis would create a cycle.");
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
                return _subAxis1;
            }
            set
            {
                _subAxis1 = value;
                if (AxisManager.Get(value) is ChainAxis axis && axis.CheckCycle(new List<string>()))
                {
                    _subAxis1 = null;
                    throw new InvalidOperationException("Linking this axis here would create a cycle.");
                }
            }
        }
        private string _subAxis1;

        /// <summary>
        /// name of the right linked axis.
        /// Changing it checks for a created cycle.
        /// </summary>
        public string SubAxis2
        {
            get
            {
                return _subAxis2;
            }
            set
            {
                _subAxis2 = value;
                if (AxisManager.Get(_subAxis2) is ChainAxis axis && axis.CheckCycle(new List<string>()))
                {
                    _subAxis2 = null;
                    throw new InvalidOperationException("Linking this axis here would create a cycle.");
                }
            }
        }
        private string _subAxis2;

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
                var axisA = AxisManager.Get(SubAxis1);
                var axisB = AxisManager.Get(SubAxis2);
                var a = axisA?.OutputValue ?? 0;
                var b = axisB?.OutputValue ?? 0;
                switch (Method)
                {
                    case ChainMethod.Sum:
                        return Mathf.Clamp(a + b, -1, 1);
                    case ChainMethod.Subtract:
                        return Mathf.Clamp(a - b, -1, 1);
                    case ChainMethod.Average:
                        return Mathf.Clamp((a + b) / 2, -1, 1);
                    case ChainMethod.Multiply:
                        return Mathf.Clamp(a * b, -1, 1);
                    case ChainMethod.Maximum:
                        return Mathf.Clamp(Mathf.Max(a, b), -1, 1);
                    case ChainMethod.Minimum:
                        return Mathf.Clamp(Mathf.Min(a, b), -1, 1);
                    default:
                        return 0;
                }
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
        }

        /// <summary>
        /// Retrieves linked axes again.
        /// Intended to be called after loading.
        /// </summary>
        public void RefreshLinks()
        {
            SubAxis1 = _subAxis1;
            SubAxis2 = _subAxis2;
        }

        /// <summary>
        /// Chain axis status distinguishes between No Link and OK.
        /// </summary>
        public override AxisStatus Status
        {
            get
            {
                if (_subAxis1 == null && _subAxis2 == null) return AxisStatus.NoLink;
                return AxisStatus.OK;
            }
        }

        /* Recursively traverse the chain axis tree depth-first.
         * Keeps list of already visited names as path.
         * Duplicate in path means there is a cycle.
         */
        private bool CheckCycle(ICollection<string> path)
        {
            // Check for duplicate
            if (path.Contains(Name))
                return true;
            var sub1 = AxisManager.Get(SubAxis1) as ChainAxis;
            var sub2 = AxisManager.Get(SubAxis2) as ChainAxis;
            var cycle = false;
            // Traverse left node
            if (sub1 != null)
            {
                var newPath = new List<string>();
                newPath.AddRange(path);
                newPath.Add(Name);
                cycle |= sub1.CheckCycle(newPath);
            }
            // Traverse right node
            if (sub2 != null)
            {
                var newPath = new List<string>();
                newPath.AddRange(path);
                newPath.Add(Name);
                cycle |= sub2.CheckCycle(newPath);
            }
            return cycle;
        }

        internal override InputAxis Clone()
        {
            var clone = new ChainAxis(Name)
            {
                Method = Method,
                SubAxis1 = SubAxis1,
                SubAxis2 = SubAxis2
            };
            return clone;
        }

        internal override void Delete()
        {
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
            return Name == cast.Name &&
                   Method == cast.Method &&
                   SubAxis1 == cast.SubAxis1 &&
                   SubAxis2 == cast.SubAxis2;
        }
    }
}
