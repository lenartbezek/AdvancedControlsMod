using System;
using System.Collections.Generic;
using System.Linq;
using spaar.ModLoader;
using UnityEngine;

// ReSharper disable PossibleNullReferenceException

namespace Lench.AdvancedControls.Axes
{
    /// <summary>
    /// Custom axis executes Python code to determine it's output value.
    /// </summary>
    public class CustomAxis : InputAxis
    {
        private const string DefaultInitialisationCode =
@"time = 0";
        private const string DefaultUpdateCode =
@"time = time + Time.deltaTime
axis_value = Mathf.Sin(time)
axis_value";

        private bool _initialised;

        /// <summary>
        /// Initialisation code is run once when the simulation is started.
        /// </summary>
        public string InitialisationCode { get; set; }

        /// <summary>
        /// Update code runs again on every frame and returns output value.
        /// </summary>
        public string UpdateCode { get; set; }

        /// <summary>
        /// Is axis set to run in global scope.
        /// </summary>
        public bool GlobalScope { get; set; }

        /// <summary>
        /// Is axis code currently running.
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// List of axes retrieved in initialisation code.
        /// </summary>
        public HashSet<string> LinkedAxes { get; private set; } = new HashSet<string>();

        /// <summary>
        /// Returns status of the code execution.
        /// </summary>
        public override AxisStatus Status
        {
            get
            {
                if (Error != null) return AxisStatus.Error;
                if (!Running) return AxisStatus.NotRunning;
                return AxisStatus.OK;
            }
        }

        /// <summary>
        /// Returns the exception that ocurred during Python code execution in Python style.
        /// </summary>
        public string Error { get; private set; }
        
        private Func<object> _update;
        private Func<object> _init;
        private Script _script;

        /// <summary>
        /// Reads input value and updates output value depending on pressed keys and settings.
        /// </summary>
        public CustomAxis(string name) : base(name)
        {
            Type = AxisType.Custom;
            InitialisationCode = DefaultInitialisationCode;
            UpdateCode = DefaultUpdateCode;
            GlobalScope = false;
            Running = false;

            Game.OnSimulationToggle += value => { if (!value) { Stop(); } };
        }

        /// <summary>
        /// Executes Python code and updates output value.
        /// </summary>
        protected override void Update()
        {
            if (!Running && _initialised)
            {
                Stop();
            }
            if (!Running) return;
            if (_initialised)
            {
                try
                {   // Attempts to run the update code.
                    var result = _update.Invoke();
                    if (result == null)
                    {
                        Error = "Does not return a value"; // TODO: Localize
                        Running = false;
                    }
                    else if (result is float || result is int || result is double)
                    {
                        OutputValue = Mathf.Clamp(Convert.ToSingle(result), -1f, 1f);
                    }
                    else
                    {
                        Error = string.Format("Returns wrong type", result.GetType()); // TODO: Localize
                        Running = false;
                    }
                }
                catch (Exception e)
                {   // On raised exception, it displays it and stops execution.
                    if (e.InnerException != null) e = e.InnerException;
                    Error = Script.FormatException(e);
                    Stop();
                }
            }
            else
            {
                // Initializes axis when starting through Running toggle.
                Initialise();
            }
        }

        /// <summary>
        /// Initializes Python environment and compiles code.
        /// Sets Running to true if successfull.
        /// </summary>
        protected override void Initialise()
        {
            _init = null;
            _update = null;

            _initialised = false;
            if (!Running && !Game.IsSimulating) return;

            Error = null;
            Running = false;

            _script = GlobalScope 
                ? Script.Global 
                : new Script();

            try
            {   
                // Attempts to compile initialisation and update code.
                _init = _script.Compile(InitialisationCode);
                _update = _script.Compile(UpdateCode);

                // Executes initialisation code and checks it's scope for linked axes.
                _init.Invoke();
                LinkAxes();
            }
            catch (Exception e)
            {
                if (e.InnerException != null) e = e.InnerException;
                Error = Script.FormatException(e);
                return;
            }

            Running = true;
            _initialised = true;
        }

        private void Stop()
        {
            Running = false;
            _initialised = false;
        }

        /// <summary>
        /// Goes through all global names in scope and checks if they represent an InputAxis object.
        /// </summary>
        private void LinkAxes()
        {
            LinkedAxes.Clear();
            foreach (var name in _script.GetVariableNames())
            {
                try
                {
                    var axis = _script.GetVariable<InputAxis>(name);
                    if (axis != null)
                    {
                        LinkedAxes.Add(axis.Name);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        internal override InputAxis Clone()
        {
            var clone = new CustomAxis(Name)
            {
                InitialisationCode = InitialisationCode,
                UpdateCode = UpdateCode,
                GlobalScope = GlobalScope
            };
            return clone;
        }

        /// <summary>
        /// Compares axis tuning parameters.
        /// </summary>
        /// <param name="other">Axis to compare with.</param>
        /// <returns>Returns true if axes are identical.</returns>
        public override bool Equals(InputAxis other)
        {
            var cast = other as CustomAxis;
            if (cast == null) return false;
            return Name == cast.Name &&
                   InitialisationCode == cast.InitialisationCode &&
                   UpdateCode == cast.UpdateCode &&
                   GlobalScope == cast.GlobalScope;
        }
    }
}
