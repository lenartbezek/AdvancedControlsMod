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
        public override bool Saveable { get { return PythonEnvironment.Loaded; } }

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
            if (!PythonEnvironment.Loaded || Error != null || !ACM.Instance.IsSimulating) return;
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
                    if (e.InnerException != null)
                        Error = PythonEnvironment.FormatException(e.InnerException);
                    else
                        Error = PythonEnvironment.FormatException(e);
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
            if (!PythonEnvironment.Loaded || !ACM.Instance.IsSimulating) return;
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
                if (e.InnerException != null)
                    Error = PythonEnvironment.FormatException(e.InnerException);
                else
                    Error = PythonEnvironment.FormatException(e);
                return;
            }
            initialised = true;
        }

        public override InputAxis Clone()
        {
            return new CustomAxis(Name, InitialisationCode, UpdateCode, GlobalScope);
        }

        public override void Load()
        {
            if (spaar.ModLoader.Configuration.DoesKeyExist("axis-" + Name + "-init"))
                spaar.ModLoader.Configuration.SetString("axis-" + Name + "-init", InitialisationCode);
            if (spaar.ModLoader.Configuration.DoesKeyExist("axis-" + Name + "-update"))
                spaar.ModLoader.Configuration.SetString("axis-" + Name + "-update", UpdateCode);
            if (spaar.ModLoader.Configuration.DoesKeyExist("axis-" + Name + "-global"))
                spaar.ModLoader.Configuration.SetBool("axis-" + Name + "-global", GlobalScope);
        }

        public override void Save()
        {
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-type", Type.ToString());
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-init", InitialisationCode);
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-update", UpdateCode);
            spaar.ModLoader.Configuration.SetBool("axis-" + Name + "-global", GlobalScope);
        }

        public override void Delete()
        {
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-type");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-init");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-update");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-global");
        }
    }
}
