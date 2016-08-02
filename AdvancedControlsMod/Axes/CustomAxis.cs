using UnityEngine;
using spaar.ModLoader;
using System;
using System.Linq;
using System.Collections.Generic;

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

        private bool initialised = false;

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
        /// Axis is saveable if Python engine is ready.
        /// </summary>
        public override bool Saveable { get { return PythonEnvironment.Loaded; } }

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
                if (!PythonEnvironment.Loaded) return AxisStatus.Unavailable;
                if (Error != null) return AxisStatus.Error;
                if (!Running) return AxisStatus.NotRunning;
                return AxisStatus.OK;
            }
        }

        /// <summary>
        /// Returns the exception that ocurred during Python code execution in Python style.
        /// </summary>
        public string Error { get; private set; }

        private PythonEnvironment python;
        private Func<object> update;
        private Func<object> init;

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
            editor = new UI.CustomAxisEditor(this);
        }

        /// <summary>
        /// Static constructor subscribes global scope initialisation to OnInitialisation event.
        /// This needs to be done so global scopes are only initialized once, instead each time for every axis.
        /// </summary>
        static CustomAxis()
        {
            ACM.Instance.OnInitialisation += InitGlobalScope;
        }

        private static void InitGlobalScope()
        {
            if (PythonEnvironment.Loaded)
                PythonEnvironment.ScripterEnvironment = new PythonEnvironment();
        }

        /// <summary>
        /// Executes Python code and updates output value.
        /// </summary>
        protected override void Update()
        {
            if (!PythonEnvironment.Loaded) return;
            if (!Running && initialised)
            {
                initialised = false;
                Running = false;
            }
            if (!Running) return;
            if (initialised)
            {
                try
                {   // Attempts to run the update code.
                    var result = update.Invoke();
                    if (result == null)
                    {
                        Error = "Update code does not return a value.";
                        Running = false;
                    }
                    else if (result is float || result is int || result is double)
                    {
                        OutputValue = Mathf.Clamp(Convert.ToSingle(result), -1f, 1f);
                    }
                    else
                    {
                        Error = "Update code returns "+result.GetType()+"\ninstead of number.";
                        Running = false;
                    }
                }
                catch (Exception e)
                {   // On raised exception, it displays it and stops execution.
                    if (e.InnerException != null) e = e.InnerException;
                    Error = PythonEnvironment.FormatException(e);
                    Running = false;
                    initialised = false;
                }
            }
            else
            {
                Initialise();
            }
        }

        /// <summary>
        /// Initializes Python environment and compiles code.
        /// </summary>
        protected override void Initialise()
        {
            if (!PythonEnvironment.Loaded) return;

            init = null;
            update = null;

            initialised = false;
            if (!Running && !Game.IsSimulating) return;
            Error = null;
            Running = false;
            if (GlobalScope)
            {
                if (PythonEnvironment.ScripterEnvironment == null)
                    InitGlobalScope();
                python = PythonEnvironment.ScripterEnvironment;
            }
            else
            {
                python = new PythonEnvironment();
            }
            try
            {   // Attempts to compile initialisation and update code.
                init = python.Compile(InitialisationCode);
                update = python.Compile(UpdateCode);

                // Executes initialisation code and checks it's scope for linked axes.
                init.Invoke();
                LinkAxes();
            }
            catch (Exception e)
            {
                if (e.InnerException != null) e = e.InnerException;
                Error = PythonEnvironment.FormatException(e);
                return;
            }
            Running = true;
            initialised = true;
        }

        /// <summary>
        /// Goes through all global names in scope and checks if they represent an InputAxis object.
        /// </summary>
        private void LinkAxes()
        {
            LinkedAxes.Clear();
            foreach (var name in python.GetVariableNames())
            {
                try
                {
                    var axis = python.GetVariable<InputAxis>(name);
                    if (axis != null)
                    {
                        LinkedAxes.Add(axis.Name);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        internal override InputAxis Clone()
        {
            var clone = new CustomAxis(Name);
            clone.InitialisationCode = InitialisationCode;
            clone.UpdateCode = UpdateCode;
            clone.GlobalScope = GlobalScope;
            return clone;
        }

        internal override void Load()
        {
            InitialisationCode = spaar.ModLoader.Configuration.GetString("axis-" + Name + "-init", InitialisationCode);
            UpdateCode = spaar.ModLoader.Configuration.GetString("axis-" + Name + "-update", UpdateCode);
            GlobalScope = spaar.ModLoader.Configuration.GetBool("axis-" + Name + "-global", GlobalScope);
            for (int i = 0; spaar.ModLoader.Configuration.DoesKeyExist("axis-" + Name + "-link" + i); i++)
                LinkedAxes.Add(spaar.ModLoader.Configuration.GetString("axis-" + Name + "-link" + i, " "));
        }

        internal override void Load(MachineInfo machineInfo)
        {
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-init"))
                InitialisationCode = machineInfo.MachineData.ReadString("axis-" + Name + "-init");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-update"))
                UpdateCode = machineInfo.MachineData.ReadString("axis-" + Name + "-update");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-global"))
                GlobalScope = machineInfo.MachineData.ReadBool("axis-" + Name + "-global");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-links"))
                LinkedAxes.Union<string>(machineInfo.MachineData.ReadStringArray("axis-" + Name + "-links"));
        }

        internal override void Save()
        {
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-type", Type.ToString());
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-init", InitialisationCode);
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-update", UpdateCode);
            spaar.ModLoader.Configuration.SetBool("axis-" + Name + "-global", GlobalScope);
            var list = LinkedAxes.ToArray<string>();
            for (int i = 0; i < list.Length; i++)
                spaar.ModLoader.Configuration.SetString("axis-" + Name + "-link"+i, list[i]);
        }

        internal override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("axis-" + Name + "-type", Type.ToString());
            machineInfo.MachineData.Write("axis-" + Name + "-init", InitialisationCode);
            machineInfo.MachineData.Write("axis-" + Name + "-update", UpdateCode);
            machineInfo.MachineData.Write("axis-" + Name + "-global", GlobalScope);
            machineInfo.MachineData.Write("axis-" + Name + "-links", LinkedAxes.ToArray<string>());
        }

        internal override void Delete()
        {
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-type");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-init");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-update");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-global");
            for (int i = 0; spaar.ModLoader.Configuration.DoesKeyExist("axis-" + Name + "-link" + i); i++)
                spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-link" + i);
            Dispose();
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
            return this.Name == cast.Name &&
                   this.InitialisationCode == cast.InitialisationCode &&
                   this.UpdateCode == cast.UpdateCode &&
                   this.GlobalScope == cast.GlobalScope;
        }
    }
}
