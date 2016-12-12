using Lench.AdvancedControls.Input;
using System;
using UnityEngine;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Lench.AdvancedControls.Axes
{
    /// <summary>
    /// Controller axis takes input from hardware and processes it.
    /// </summary>
    public class ControllerAxis : InputAxis
    {
#pragma warning disable CS1591
        /* TUNING:
         * Following properties flip the changed flag on edit.
         * Changed flag is used for redrawing the graph.
         */
        public float Sensitivity
        {
            get { return _sensitivity; }
            set { _changed |= value != _sensitivity; _sensitivity = value; }
        }
        private float _sensitivity;

        public float Curvature
        {
            get { return _curvature; }
            set { _changed |= value != _curvature; _curvature = value; }
        }
        private float _curvature;

        public float Deadzone
        {
            get { return _deadzone; }
            set { _changed |= value != _deadzone; _deadzone = value; }
        }
        private float _deadzone;

        public float OffsetX
        {
            get { return _offx; }
            set { _changed |= value != _offx; _offx = value; }
        }
        private float _offx;

        public float OffsetY
        {
            get { return _offy; }
            set { _changed |= value != _offy; _offy = value; }
        }
        private float _offy;

        public bool Invert
        {
            get { return _invert; }
            set { _changed |= value != _invert; _invert = value; }
        }
        private bool _invert;

        public bool Smooth { get; set; }
#pragma warning restore CS1591

        /// <summary>
        /// Index of the device axis.
        /// </summary>
        public int Axis { get; set; }

        private Guid _guid;
        private Controller _controller;

        /// <summary>
        /// GUID of the controller.
        /// Just created axis returns zero value GUID.
        /// </summary>
        public Guid GUID
        {
            get { return _guid; }
            set
            {
                _guid = value;
                _controller = Controller.Get(value);
            }
        }

        private bool _changed = true;

        /// <summary>
        /// Is true if the axis tuning has been changed since the last call.
        /// </summary>
        public bool Changed
        {
            get { bool tmp = _changed; _changed = false; return tmp; }
        }

        /// <summary>
        /// Is the associated device currently connected.
        /// </summary>
        public override bool Connected => _controller != null && _controller.Connected && Axis < _controller.NumAxes;

        /// <summary>
        /// Axis is saveable if SDL engine is initialized and at least one device is connected.
        /// </summary>
        public override bool Saveable => DeviceManager.SdlInitialized && Controller.NumDevices > 0;

        /// <summary>
        /// Controller axis status distinguishes between Engine Unavailable, Device Disconnected and OK.
        /// </summary>
        public override AxisStatus Status
        {
            get
            {
                if (!DeviceManager.SdlInitialized) return AxisStatus.Unavailable;
                if (!Connected) return AxisStatus.Disconnected;
                return AxisStatus.OK;
            }
        }

        /// <summary>
        /// Creates a new controller axis with given name and default values.
        /// </summary>
        /// <param name="name">Name of the axis.</param>
        public ControllerAxis(string name) : base(name)
        {
            Type = AxisType.Controller;
            Axis = 0;
            GUID = Controller.NumDevices > 0 ? Controller.ControllerList[0].GUID : new Guid();
            Sensitivity = 1;
            Curvature = 1;
            Deadzone = 0;
            OffsetX = 0;
            OffsetY = 0;
            Invert = false;
            Smooth = false;
            Editor = new UI.ControllerAxisEditor(this);

            DeviceManager.OnDeviceAdded += (e) => _controller = Controller.Get(_guid);
            DeviceManager.OnDeviceRemoved += (e) => _controller = Controller.Get(_guid);
        }

        /// <summary>
        /// Raw input value.
        /// </summary>
        public override float InputValue
        {
            get
            {
                if (!Connected) return 0;
                return _controller.GetAxis(Axis, Smooth);
            }
        }

        /// <summary>
        /// Processed input value.
        /// </summary>
        public override float OutputValue
        {
            get
            {
                return Process(InputValue);
            }
        }

        /// <summary>
        /// Returns processed output value for given input value.
        /// Intended for drawing graph.
        /// </summary>
        /// <param name="input">Input value in range [-1, +1]</param>
        /// <returns>Output value in range [-1, +1]</returns>
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

        internal override void Load(MachineInfo machineInfo)
        {
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-controller"))
                GUID = new Guid(machineInfo.MachineData.ReadString("axis-" + Name + "-controller"));
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-axis"))
                Axis = machineInfo.MachineData.ReadInt("axis-" + Name + "-axis");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-sensitivity"))
                Sensitivity = machineInfo.MachineData.ReadFloat("axis-" + Name + "-sensitivity");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-curvature"))
                Curvature = machineInfo.MachineData.ReadFloat("axis-" + Name + "-curvature");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-deadzone"))
                Deadzone = machineInfo.MachineData.ReadFloat("axis-" + Name + "-deadzone");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-offsetx"))
                OffsetX = machineInfo.MachineData.ReadFloat("axis-" + Name + "-offsetx");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-offsety"))
                OffsetY = machineInfo.MachineData.ReadFloat("axis-" + Name + "-offsety");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-invert"))
                Invert = machineInfo.MachineData.ReadBool("axis-" + Name + "-invert");
            if (machineInfo.MachineData.HasKey("axis-" + Name + "-smooth"))
                Smooth = machineInfo.MachineData.ReadBool("axis-" + Name + "-smooth");
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

        internal override void Save(MachineInfo machineInfo)
        {
            machineInfo.MachineData.Write("axis-" + Name + "-type", Type.ToString());
            machineInfo.MachineData.Write("axis-" + Name + "-controller", GUID.ToString());
            machineInfo.MachineData.Write("axis-" + Name + "-axis", Axis);
            machineInfo.MachineData.Write("axis-" + Name + "-sensitivity", Sensitivity);
            machineInfo.MachineData.Write("axis-" + Name + "-curvature", Curvature);
            machineInfo.MachineData.Write("axis-" + Name + "-deadzone", Deadzone);
            machineInfo.MachineData.Write("axis-" + Name + "-offsetx", OffsetX);
            machineInfo.MachineData.Write("axis-" + Name + "-offsety", OffsetY);
            machineInfo.MachineData.Write("axis-" + Name + "-invert", Invert);
            machineInfo.MachineData.Write("axis-" + Name + "-smooth", Smooth);
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

        /// <summary>
        /// Controller axis requires no initialisation.
        /// </summary>
        protected override void Initialise() { }

        /// <summary>
        /// Controller axis requires no update.
        /// </summary>
        protected override void Update() { }

        /// <summary>
        /// Compares axis tuning parameters.
        /// </summary>
        /// <param name="other">Axis to compare with.</param>
        /// <returns>Returns true if axes are identical.</returns>
        public override bool Equals(InputAxis other)
        {
            var cast = other as ControllerAxis;
            if (cast == null) return false;
            return Name == cast.Name &&
                   GUID == cast.GUID &&
                   Axis == cast.Axis &&
                   Sensitivity == cast.Sensitivity &&
                   Curvature == cast.Curvature &&
                   Deadzone == cast.Deadzone &&
                   OffsetX == cast.OffsetX &&
                   OffsetY == cast.OffsetY &&
                   Invert == cast.Invert &&
                   Smooth == cast.Smooth;
        }
    }
}
