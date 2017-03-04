using System;
using UnityEngine;

// ReSharper disable SpecifyStringComparison

namespace Lench.AdvancedControls
{
    /// <summary>
    ///     Base class for all block handlers.
    /// </summary>
    public partial class Block
    {
        private BlockBehaviour _buildingClone;
        private BlockBehaviour _simulationClone;

        /// <summary>
        ///     BlockBehaviour object of this handler.
        /// </summary>
        protected BlockBehaviour Bb
        {
            get
            {
                if (StatMaster.isSimulating)
                    return _simulationClone = _simulationClone ?? _buildingClone.SimBlock;
                
                return _buildingClone = _buildingClone ?? _simulationClone.BuildingBlock;
            }
        }

        /// <summary>
        ///     BlockLoaders BlockScript object.
        /// </summary>
        protected MonoBehaviour Bs => Bb.GetComponent<BlockScript>();

        // Internal wrappers
        internal BlockBehaviour BlockBehaviour => Bb;
        internal MonoBehaviour BlockScript => Bs;

        /// <summary>
        ///     Wrapper for accessing slider values.
        /// </summary>
        public readonly SliderWrapper Sliders;

        /// <summary>
        ///     Wrapper for accessing toggle values.
        /// </summary>
        public readonly ToggleWrapper Toggles;

        /// <summary>
        ///     Wrapper for accessing key binds.
        /// </summary>
        public readonly KeyWrapper Keys;

        /// <summary>
        ///     Wrapper for accessing colour sliders.
        /// </summary>
        public readonly ColorWrapper Colors;

        /// <summary>
        ///     Creates a Block handler from a BlockBehaviour object.
        ///     BlockBehaviour can be building clone or simulation clone.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        protected Block(BlockBehaviour bb)
        {
            if (ReferenceEquals(bb, bb.BuildingBlock))
                _buildingClone = bb;
            else
                _simulationClone = bb;

            Sliders = new SliderWrapper(this);
            Toggles = new ToggleWrapper(this);
            Keys = new KeyWrapper(this);
            Colors = new ColorWrapper(this);

            OnUpdate += Update;
            OnLateUpdate += LateUpdate;
            OnFixedUpdate += FixedUpdate;
        }

        /// <summary>
        ///     Name of the block. References to MyBlockInfo.BlockName.
        /// </summary>
        public string Name
        {
            get { return Bb.GetComponent<MyBlockInfo>().blockName; }
            set { Bb.GetComponent<MyBlockInfo>().blockName = value; }
        }

        /// <summary>
        ///     Type of the block.
        /// </summary>
        public int Type => Bb.GetBlockID();

        /// <summary>
        ///     Custom block identifier.
        ///     Used to look up blocks in the machine.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        ///     Blocks GUID.
        ///     Only set when Block handler is created at simulation start.
        ///     Cannot be changed.
        /// </summary>
        public Guid GUID => (_buildingClone ?? _simulationClone.BuildingBlock).Guid;

        /// <summary>
        ///     Returns true if the block has RigidBody.
        /// </summary>
        /// <returns>Boolean value.</returns>
        public virtual bool Exists => Bb != null && Bb.GetComponent<Rigidbody>() != null;

        /// <summary>
        ///     Returns the block's forward vector.
        /// </summary>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public virtual Vector3 Forward => Bb.transform.forward;

        /// <summary>
        ///     Returns the block's up vector.
        /// </summary>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public virtual Vector3 Up => Bb.transform.up;

        /// <summary>
        ///     Returns the block's right vector.
        /// </summary>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public virtual Vector3 Right => Bb.transform.right;

        /// <summary>
        ///     Returns the position of the block.
        /// </summary>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public virtual Vector3 Position => Bb.transform.position;

        /// <summary>
        ///     Returns the block's rotation as Euler angles.
        /// </summary>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public virtual Vector3 Rotation => Bb.transform.eulerAngles;

        /// <summary>
        ///     Returns the block's rotation as a quaternion.
        /// </summary>
        /// <returns>UnityEngine.Quaternion vector.</returns>
        public virtual Quaternion Quaternion => Bb.transform.rotation;

        /// <summary>
        ///     Returns the velocity of the block in units per second.
        /// </summary>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public virtual Vector3 Velocity
        {
            get
            {
                var body = Bb.GetComponent<Rigidbody>();
                if (body != null)
                    return body.velocity;
                throw new InvalidOperationException("Block " + Name + " has no rigid body.");
            }
        }

        /// <summary>
        ///     Returns the mass of the block.
        /// </summary>
        /// <returns>Float value.</returns>
        public virtual float Mass
        {
            get
            {
                var body = Bb.GetComponent<Rigidbody>();
                if (body != null)
                    return body.mass;
                throw new InvalidOperationException("Block " + Name + " has no rigid body.");
            }
        }

        /// <summary>
        ///     Returns the center of mass of the block, relative to the block's position.
        /// </summary>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public virtual Vector3 Com
        {
            get
            {
                var body = Bb.GetComponent<Rigidbody>();
                if (body != null)
                    return body.centerOfMass;
                throw new InvalidOperationException("Block " + Name + " has no rigid body.");
            }
        }

        /// <summary>
        ///     Returns the block's angular velocity.
        /// </summary>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public virtual Vector3 AngularVelocity
        {
            get
            {
                var body = Bb.GetComponent<Rigidbody>();
                if (body == null) throw new InvalidOperationException("Block " + Name + " has no rigid body.");

                return body.angularVelocity;
            }
        }

        /// <summary>
        ///     Is called at every Update.
        /// </summary>
        protected virtual void Update()
        {
        }

        /// <summary>
        ///     Is called at every LateUpdate.
        /// </summary>
        protected virtual void LateUpdate()
        {
        }

        /// <summary>
        ///     Is called at every FixedUpdate.
        /// </summary>
        protected virtual void FixedUpdate()
        {
        }
    }
}