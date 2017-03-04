using System;
using System.Collections;
using System.Collections.Generic;
using Lench.AdvancedControls.Blocks;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable UnusedMember.Local
// ReSharper disable PossibleNullReferenceException

namespace Lench.AdvancedControls
{
    /// <summary>
    ///     Block handlers API of the scripting mod.
    /// </summary>
    public partial class Block
    {
        /// <summary>
        ///     Event invoked when simulation block handlers are initialised.
        ///     Use this instead of Game.OnSimulation if you're relying on block handlers.
        /// </summary>
        public static event Action OnInitialisation;

        private static event Action OnUpdate;

        private static event Action OnLateUpdate;

        private static event Action OnFixedUpdate;

        /// <summary>
        ///     Creates a Block handler from a BlockBehaviour object.
        ///     Same BlockBehaviour will return same Block instance.
        /// </summary>
        public static Block Create(BlockBehaviour bb)
        {
            if (CreatedBlocks.ContainsKey(bb))
                return CreatedBlocks[bb];
            var block = TypeMap.ContainsKey(bb.GetBlockID())
                ? (Block)Activator.CreateInstance(TypeMap[bb.GetBlockID()], new object[] { bb })
                : new Block(bb);
            CreatedBlocks[bb] = block;
            return block;
        }

        private static readonly Dictionary<BlockBehaviour, Block> CreatedBlocks = new Dictionary<BlockBehaviour, Block>();

        /// <summary>
        ///     Map of all Block IDs to Block Types.
        ///     Looked up when creating Block handlers.
        ///     Blocks with ID's that are not present here will get assigned the base Block handler.
        /// </summary>
        public static readonly Dictionary<int, Type> TypeMap = new Dictionary<int, Type>
        {
            {(int) BlockType.Cannon, typeof(Cannon)},
            {(int) BlockType.ShrapnelCannon, typeof(Cannon)},
            {(int) BlockType.CogMediumPowered, typeof(Cog)},
            {(int) BlockType.Wheel, typeof(Cog)},
            {(int) BlockType.LargeWheel, typeof(Cog)},
            {(int) BlockType.Drill, typeof(Cog)},
            {(int) BlockType.Crossbow, typeof(Crossbow)},
            {(int) BlockType.Decoupler, typeof(Decoupler)},
            {(int) BlockType.Flamethrower, typeof(Flamethrower)},
            {(int) BlockType.FlyingBlock, typeof(FlyingSpiral)},
            {(int) BlockType.Grabber, typeof(Grabber)},
            {(int) BlockType.Grenade, typeof(Grenade)},
            {(int) BlockType.Piston, typeof(Piston)},
            {(int) BlockType.Rocket, typeof(Rocket)},
            {(int) BlockType.Spring, typeof(Spring)},
            {(int) BlockType.RopeWinch, typeof(Spring)},
            {(int) BlockType.SteeringHinge, typeof(Steering)},
            {(int) BlockType.SteeringBlock, typeof(Steering)},
            {(int) BlockType.Vacuum, typeof(Vacuum)},
            {(int) BlockType.WaterCannon, typeof(WaterCannon)},
            {410, typeof(Automatron)},
            {790, typeof(VectorThruster)}
        };

        private static BlockControllerComponent _component;

        // ReSharper disable once ClassNeverInstantiated.Local
        private class BlockControllerComponent : MonoBehaviour
        {
            private void Update()
            {
                OnUpdate?.Invoke();
            }

            private void LateUpdate()
            {
                OnLateUpdate?.Invoke();
            }

            private void FixedUpdate()
            {
                OnFixedUpdate?.Invoke();
            }
        }

        private static IEnumerator WaitAndInitialize()
        {
            while (!StatMaster.isSimulating || ReferenceMaster.SimulationBlocks.Count < ReferenceMaster.BuildingBlocks.Count)
                yield return null;
            OnInitialisation?.Invoke();
        }

        private static void Destroy()
        {
            Object.Destroy(_component);
        }

        /// <summary>
        ///     Creates or destroys block handlers.
        /// </summary>
        public static void HandleSimulationToggle(bool simulating)
        {
            if (simulating)
            {
                _component = Mod.Controller.AddComponent<BlockControllerComponent>();
                _component.StartCoroutine(WaitAndInitialize());
            }
            else
            {
                Destroy();
            } 
        }
    }
}