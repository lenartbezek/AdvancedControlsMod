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
        /// <summary>
        ///     BlockBehaviour object of this handler.
        /// </summary>
        protected readonly BlockBehaviour Bb;

        /// <summary>
        ///     BlockLoaders BlockScript object.
        /// </summary>
        protected readonly MonoBehaviour Bs;

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
        ///     Creates a Block handler.
        /// </summary>
        /// <param name="bb">BlockBehaviour object.</param>
        protected Block(BlockBehaviour bb)
        {
            Bb = bb;
            Bs = bb.GetComponent<BlockScript>();

            Sliders = new SliderWrapper(Bb);
            Toggles = new ToggleWrapper(Bb);
            Keys = new KeyWrapper(Bb);
            Colors = new ColorWrapper(Bb);

            OnUpdate += Update;
            OnLateUpdate += LateUpdate;
            OnFixedUpdate += FixedUpdate;
        }

        /// <summary>
        ///     Unsubscribes the block handler from update events.
        /// </summary>
        internal void Dispose()
        {
            OnUpdate -= Update;
            OnLateUpdate -= LateUpdate;
            OnFixedUpdate -= FixedUpdate;
        }

        internal BlockBehaviour BlockBehaviour => Bb;

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
        public string ID { get; private set; }

        /// <summary>
        ///     Blocks GUID.
        ///     Only set when Block handler is created at simulation start.
        ///     Cannot be changed.
        /// </summary>
        public Guid GUID { get; private set; }

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
        ///     Returns the block's rotation in the form of it's Euler angles.
        /// </summary>
        /// <returns>UnityEngine.Vector3 vector.</returns>
        public virtual Vector3 Rotation => Bb.transform.eulerAngles;

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