using System;
using UnityEngine;
using LenchScripter;
using spaar.ModLoader;

namespace AdvancedControls.Axes
{
    public class CustomAxis : InputAxis
    {
        public override string Name { get; set; } = "new custom axis";
        public override AxisType Type { get { return AxisType.Custom; } }

        private const string DefaultInitialisationCode =
@"time = 0";
        private const string DefaultUpdateCode =
@"time = time + Time.deltaTime
axis_value = Mathf.Sin(time)
axis_value";

        private bool initialised = false;

        public string InitialisationCode { get; set; }
        public string UpdateCode { get; set; }
        public bool GlobalScope { get; set; }
        public override bool Saveable { get { return PythonEnvironment.Loaded; } }

        public string Error { get; set; }

        private PythonEnvironment python;
        private Func<object> update;
        private Func<object> init;

        public CustomAxis(string name) : base(name)
        {
            InitialisationCode = DefaultInitialisationCode;
            UpdateCode = DefaultUpdateCode;
            GlobalScope = false;
            editor = new UI.CustomAxisEditor(this);
        }

        protected override void Update()
        {
            if (!PythonEnvironment.Loaded || Error != null || !Game.IsSimulating) return;
            if (initialised)
            {
                try
                {
                    var result = update.Invoke();
                    if (result == null)
                    {
                        Error = "Update code does not return a value.";
                    }
                    else if (result is float || result is int || result is double)
                    {
                        OutputValue = Mathf.Clamp(Convert.ToSingle(result), -1f, 1f);
                    }
                    else
                    {
                        Error = "Update code returns "+result.GetType()+"\ninstead of number.";
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException != null) e = e.InnerException;
                    Error = PythonEnvironment.FormatException(e);
                }
            }
        }

        protected override void Initialise()
        {
            init = null;
            update = null;

            initialised = false;
            if (!PythonEnvironment.Loaded || !Game.IsSimulating) return;
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
                if (e.InnerException != null) e = e.InnerException;
                Error = PythonEnvironment.FormatException(e);
                return;
            }
            initialised = true;
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
        }

        internal override void Save()
        {
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-type", Type.ToString());
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-init", InitialisationCode);
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-update", UpdateCode);
            spaar.ModLoader.Configuration.SetBool("axis-" + Name + "-global", GlobalScope);
        }

        internal override void Delete()
        {
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-type");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-init");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-update");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-global");
            Dispose();
        }
    }
}
