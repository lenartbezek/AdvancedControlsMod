﻿using System;
using UnityEngine;
using LenchScripter;

namespace AdvancedControls.Axes
{
    public class CustomAxis : Axis
    {
        private bool initialised = false;
        public CustomAxis() : base() { }

        public string InitialisationCode { get; set; } = @"time = 0";
        public string UpdateCode { get; set; } =
@"time = time + Time.deltaTime
axis_value = Mathf.Sin(time)
return axis_value";
        public Exception Exception { get; set; }

        public CustomAxis(string name = "new axis", string init = "", string update = "")
        {
            Name = name;
            InitialisationCode = init;
            UpdateCode = update;
        }

        public override void Update()
        {
            if (Lua.IsActive && initialised && Exception == null)
            {
                try
                {
                    var result = Lua.Evaluate(UpdateCode)[0] as double?;
                    OutputValue = Mathf.Clamp((float)result, -1, 1);
                }
                catch (Exception e)
                {
                    Exception = e;
                }
            }
            else if (Lua.IsActive && Exception == null)
            {
                Initialise();
            }
        }

        public override void Initialise()
        {
            initialised = false;
            if (Lua.IsActive)
            {
                Exception = null;
                try
                {
                    Lua.Evaluate(InitialisationCode);
                    initialised = true;
                }
                catch (Exception e)
                {
                    Exception = e;
                    initialised = false;
                }

            }
        }

        public CustomAxis Clone()
        {
            return new CustomAxis(Name, InitialisationCode, UpdateCode);
        }

        public override void Load(MachineInfo machineInfo)
        {
            InitialisationCode = @machineInfo.MachineData.ReadString("AC-Axis-" + Name + "-InitialisationCode");
            UpdateCode = @machineInfo.MachineData.ReadString("AC-Axis-" + Name + "-UpdateCode");
        }

        public override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-Type", "Custom");
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-InitialisationCode", InitialisationCode);
            machineInfo.MachineData.Write("AC-Axis-" + Name + "-UpdateCode", UpdateCode);
        }
    }
}