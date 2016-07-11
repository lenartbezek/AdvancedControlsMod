using Lench.AdvancedControls.Input;
using System;
using UnityEngine;

namespace Lench.AdvancedControls.Axes
{
    public class ControllerAxis : InputAxis
    {
        public override string Name { get; set; } = "new controller axis";
        public override AxisType Type { get { return AxisType.Controller; } }

        private float sensitivity;
        public float Sensitivity
        {
            get { return sensitivity; }
            set { changed |= value != sensitivity; sensitivity = value; }
        }

        private float curvature;
        public float Curvature
        {
            get { return curvature; }
            set { changed |= value != curvature; curvature = value; }
        }

        private float deadzone;
        public float Deadzone
        {
            get { return deadzone; }
            set { changed |= value != deadzone; deadzone = value; }
        }

        private float offx;
        public float OffsetX
        {
            get { return offx; }
            set { changed |= value != offx; offx = value; }
        }

        private float offy;
        public float OffsetY
        {
            get { return offy; }
            set { changed |= value != offy; offy = value; }
        }

        private bool invert;
        public bool Invert
        {
            get { return invert; }
            set { changed |= value != invert; invert = value; }
        }

        public bool Smooth { get; set; }

        public int Axis { get; set; }

        private Guid guid;
        private Controller controller;
        public Guid GUID
        {
            get{ return guid; }
            set
            {
                guid = value;
                controller = Controller.Get(value);
            }
        }

        private bool changed = true;
        internal bool Changed
        {
            get { bool tmp = changed; changed = false; return tmp; }
        }

        public override bool Connected { get { return controller != null && controller.Connected; } }
        public override bool Saveable { get { return ACM.Instance.EventManager.SDL_Initialized && Controller.NumDevices > 0; } }
        public override string Status
        {
            get
            {
                if (!ACM.Instance.EventManager.SDL_Initialized) return "NOT AVAILABLE";
                if (!Connected) return "DISCONNECTED";
                return "OK";
            }
        }

        public ControllerAxis(string name) : base(name)
        {
            Axis = 0;
            GUID = Controller.NumDevices > 0 ? Controller.DeviceList[0] : new Guid();
            Sensitivity = 1;
            Curvature = 1;
            Deadzone = 0;
            OffsetX = 0;
            OffsetY = 0;
            Invert = false;
            Smooth = false;
            editor = new UI.ControllerAxisEditor(this);

            ACM.Instance.EventManager.OnDeviceAdded += (SDL.SDL_Event e) => this.controller = Controller.Get(guid);
            ACM.Instance.EventManager.OnDeviceRemoved += (SDL.SDL_Event e) => this.controller = Controller.Get(guid);
        }

        public override float InputValue
        {
            get
            {
                if (controller == null)
                    return 0;
                return controller.GetAxis(Axis, Smooth);
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
                return 0 + OffsetY;
            else
                value = input > 0 ? input - Deadzone : input + Deadzone;
            value *= Sensitivity * (Invert ? -1 : 1);
            value = value > 0 ? Mathf.Pow(value, Curvature) : -Mathf.Pow(-value, Curvature);
            return Mathf.Clamp(value + OffsetY, -1, 1);
        }

        internal override InputAxis Clone()
        {
            var clone = new ControllerAxis(Name);
            clone.GUID = GUID;
            clone.Axis = Axis;
            clone.Sensitivity = Sensitivity;
            clone.Curvature = Curvature;
            clone.Deadzone = Deadzone;
            clone.OffsetX = OffsetX;
            clone.OffsetY = OffsetY;
            clone.Invert = Invert;
            clone.Smooth = Smooth;
            return clone;
        }

        internal override void Load()
        {
            GUID = new Guid(spaar.ModLoader.Configuration.GetString("axis-" + Name + "-controller", GUID.ToString()));
            Axis = spaar.ModLoader.Configuration.GetInt("axis-" + Name + "-axis", Axis);
            Sensitivity = spaar.ModLoader.Configuration.GetFloat("axis-" + Name + "-sensitivity", Sensitivity);
            Curvature = spaar.ModLoader.Configuration.GetFloat("axis-" + Name + "-curvature", Curvature);
            Deadzone = spaar.ModLoader.Configuration.GetFloat("axis-" + Name + "-deadzone", Deadzone);
            OffsetX = spaar.ModLoader.Configuration.GetFloat("axis-" + Name + "-offsetx", OffsetX);
            OffsetY = spaar.ModLoader.Configuration.GetFloat("axis-" + Name + "-offsety", OffsetY);
            Invert = spaar.ModLoader.Configuration.GetBool("axis-" + Name + "-invert", Invert);
            Smooth = spaar.ModLoader.Configuration.GetBool("axis-" + Name + "-smooth", Smooth);
        }

        internal override void Save()
        {
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-type", Type.ToString());
            spaar.ModLoader.Configuration.SetString("axis-" + Name + "-controller", GUID.ToString());
            spaar.ModLoader.Configuration.SetInt("axis-" + Name + "-axis", Axis);
            spaar.ModLoader.Configuration.SetFloat("axis-" + Name + "-sensitivity", Sensitivity);
            spaar.ModLoader.Configuration.SetFloat("axis-" + Name + "-curvature", Curvature);
            spaar.ModLoader.Configuration.SetFloat("axis-" + Name + "-deadzone", Deadzone);
            spaar.ModLoader.Configuration.SetFloat("axis-" + Name + "-offsetx", OffsetX);
            spaar.ModLoader.Configuration.SetFloat("axis-" + Name + "-offsety", OffsetY);
            spaar.ModLoader.Configuration.SetBool("axis-" + Name + "-invert", Invert);
            spaar.ModLoader.Configuration.SetBool("axis-" + Name + "-smooth", Smooth);
        }

        internal override void Delete()
        {
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-type");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-controller");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-axis");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-sensitivity");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-curvature");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-deadzone");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-offsetx");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-offsety");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-invert");
            spaar.ModLoader.Configuration.RemoveKey("axis-" + Name + "-smooth");
            Dispose();
        }

        protected override void Initialise() { }

        protected override void Update() { }
    }
}
