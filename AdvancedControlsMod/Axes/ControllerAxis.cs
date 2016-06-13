using AdvancedControls.Input;
using System;
using UnityEngine;

namespace AdvancedControls.Axes
{
    public class ControllerAxis : InputAxis
    {
        public override string Name { get; set; } = "new controller axis";
        public float Sensitivity { get; set; }
        public float Curvature { get; set; }
        public float Deadzone { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public bool Invert { get; set; }
        public bool Smooth { get; set; }
        public int AxisID { get; set; }
        public Guid ControllerGUID
        {
            get
            {
                return guid;
            }
            set
            {
                guid = value;
                controller = Controller.Get(value);
            }
        }

        public override bool Connected { get { return controller != null && controller.Connected; } }
        public override bool Saveable { get { return AdvancedControlsMod.EventManager.SDL_Initialized && Controller.NumDevices > 0; } }

        private Guid guid;
        private Controller controller;

        public ControllerAxis(string name) : base(name)
        {
            Type = AxisType.Controller;
            AxisID = 0;
            ControllerGUID = Controller.NumDevices > 0 ? Controller.DeviceList[0] : new Guid();
            Sensitivity = 1;
            Curvature = 1;
            Deadzone = 0;
            Invert = false;
            Smooth = false;
            editor = new UI.ControllerAxisEditor(this);

            AdvancedControlsMod.EventManager.OnDeviceAdded += (SDL.SDL_Event e) => this.controller = Controller.Get(guid);
            AdvancedControlsMod.EventManager.OnDeviceRemoved += (SDL.SDL_Event e) => this.controller = Controller.Get(guid);
        }

        public ControllerAxis(string name, Guid controller,
            int axis = 0, float sensitivity = 1, float curvature = 1, float deadzone = 0, float off_x = 0, float off_y = 0,
            bool invert = false, bool smooth = false) : base(name)
        {
            Type = AxisType.Controller;
            AxisID = axis;
            ControllerGUID = controller;
            Sensitivity = sensitivity;
            Curvature = curvature;
            Deadzone = deadzone;
            Invert = invert;
            Smooth = smooth;
            OffsetX = off_x;
            OffsetY = off_y;
            editor = new UI.ControllerAxisEditor(this);

            AdvancedControlsMod.EventManager.OnDeviceAdded += (SDL.SDL_Event e) => this.controller = Controller.Get(guid);
            AdvancedControlsMod.EventManager.OnDeviceRemoved += (SDL.SDL_Event e) => this.controller = Controller.Get(guid);
        }

        public struct Param
        {
            public int axis;
            public Guid controller;
            public float sens;
            public float curv;
            public float dead;
            public bool inv;
            public float off_x;
            public float off_y;
        }

        public Param Parameters
        {
            get
            {
                return new Param()
                {
                    axis = AxisID,
                    controller = ControllerGUID,
                    sens = Sensitivity,
                    curv = Curvature,
                    dead = Deadzone,
                    inv = Invert,
                    off_x = OffsetX,
                    off_y = OffsetY
                };
            }
        }

        public override float InputValue
        {
            get
            {
                if (controller == null)
                    return 0;
                if (Smooth)
                    return controller.GetAxisSmooth(AxisID);
                else
                    return controller.GetAxis(AxisID);
            }
        }

        public override float OutputValue
        {
            get
            {
                return Process(InputValue);
            }
        }

        public float Process(float input)
        {
            input += OffsetX;
            float value;
            if (Mathf.Abs(input) < Deadzone)
                return 0 - OffsetY;
            else
                value = input > 0 ? input - Deadzone : input + Deadzone;
            value *= Sensitivity * (Invert ? -1 : 1);
            value = value > 0 ? Mathf.Pow(value, Curvature) : -Mathf.Pow(-value, Curvature);
            return Mathf.Clamp(value - OffsetY, -1, 1);
        }

        public override InputAxis Clone()
        {
            return new ControllerAxis(Name, ControllerGUID, AxisID, Sensitivity, Curvature, Deadzone, OffsetX, OffsetY, Invert, Smooth);
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

        public override void Update() { }
    }
}
