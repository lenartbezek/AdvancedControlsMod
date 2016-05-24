using System;
using UnityEngine;
using LenchScripter;

namespace AdvancedControls.Axes
{
    public class CustomAxis : InputAxis
    {
        private const string DefaultInitialisationCode =
@"time = 0";
        private const string DefaultUpdateCode =
@"time = time + Time.deltaTime
axis_value = Mathf.Sin(time)
axis_value";

        public override string Name { get; set; } = "new custom axis";
        private bool initialised = false;

        public string InitialisationCode { get; set; }
        public string UpdateCode { get; set; }
        public bool GlobalScope { get; set; }

        public string Error { get; set; }

        private PythonEnvironment python;
        private Func<object> update;
        private Func<object> init;

        public CustomAxis(string name, string init = DefaultInitialisationCode, string update = DefaultUpdateCode, bool global = false) : base(name)
        {
            Type = AxisType.Custom;
            InitialisationCode = init;
            UpdateCode = update;
            GlobalScope = global;
            editor = new UI.CustomAxisEditor(this);
        }

        public override void Update()
        {
            if (!PythonEnvironment.Loaded || Error != null || !ADVControls.Instance.IsSimulating) return;
            if (initialised)
            {
                try
                {
                    OutputValue = Mathf.Clamp((float)update.Invoke(), -1, 1);
                }
                catch (Exception e)
                {
                    Error = PythonEnvironment.FormatException(e.InnerException);
                }
            }
            else
            {
                Initialise();
            }
        }

        public override void Initialise()
        {
            initialised = false;
            if (!PythonEnvironment.Loaded || !ADVControls.Instance.IsSimulating) return;
            Error = null;
            if (GlobalScope && PythonEnvironment.Enabled)
                python = PythonEnvironment.ScripterEnvironment;
            else
                python = new PythonEnvironment();
            if (python == null) return;
            try
            {
                init = python.Compile(InitialisationCode);
                update = python.Compile(UpdateCode);

                init.Invoke();
            }
            catch (Exception e)
            {
                Error = PythonEnvironment.FormatException(e.InnerException);
                return;
            }
            initialised = true;
        }

        public override InputAxis Clone()
        {
            return new CustomAxis(Name, InitialisationCode, UpdateCode, GlobalScope);
        }

        public override void Load(MachineInfo machineInfo)
        {
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-init"))
                InitialisationCode = machineInfo.MachineData.ReadString("ac-axis-" + Name + "-init");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-update"))
                UpdateCode = machineInfo.MachineData.ReadString("ac-axis-" + Name + "-update");
            if (machineInfo.MachineData.HasKey("ac-axis-" + Name + "-global"))
                GlobalScope = machineInfo.MachineData.ReadBool("ac-axis-" + Name + "-global");
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("ac-axis-" + Name + "-type", "custom");
            machineInfo.MachineData.Write("ac-axis-" + Name + "-init", InitialisationCode);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-update", UpdateCode);
            machineInfo.MachineData.Write("ac-axis-" + Name + "-global", GlobalScope);
        }
    }
}
