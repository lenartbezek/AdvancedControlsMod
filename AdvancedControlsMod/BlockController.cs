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
        ///     A list containing all initialized Block handler objects.
        /// </summary>
        public static readonly IList<Block> Blocks = new List<Block>();

        /// <summary>
        ///     Event invoked when simulation block handlers are initialised.
        ///     Use this instead of Game.OnSimulation if you're relying on block handlers.
        /// </summary>
        public static event Action OnInitialisation;

        private static event Action OnUpdate;

        private static event Action OnLateUpdate;

        private static event Action OnFixedUpdate;

        /// <summary>
        ///     Map of all Block IDs to Block Types.
        ///     Looked up when creating Block handlers.
        ///     Blocks with ID's that are not present here
        ///     will get assigned the base Block handler.
        /// </summary>
        public static readonly Dictionary<int, Type> Types = new Dictionary<int, Type>
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

        /// <summary>
        ///     Returns True if block handlers are initialised.
        /// </summary>
        public static bool Initialised { get; private set; }

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

        private static Block Create(BlockBehaviour bb)
        {
            return Types.ContainsKey(bb.GetBlockID())
                ? (Block) Activator.CreateInstance(Types[bb.GetBlockID()], new object[] {bb})
                : new Block(bb);
        }

        private static void Initialize()
        {
            Blocks.Clear();
            var typeCount = new Dictionary<int, int>();
            foreach (var bb in ReferenceMaster.SimulationBlocks)
            {
                var block = Create(bb);
                block.GUID = bb.BuildingBlock.Guid;

                if (typeCount.ContainsKey(block.Type))
                    typeCount[block.Type] += 1;
                else
                    typeCount[block.Type] = 1;
                block.ID = $"{block.Name} {typeCount[block.Type]}";

                Blocks.Add(block);
            }

            OnInitialisation?.Invoke();
        }

        private static IEnumerator WaitAndInitialize()
        {
            while (!StatMaster.isSimulating || ReferenceMaster.SimulationBlocks.Count < ReferenceMaster.BuildingBlocks.Count)
                yield return null;
            Initialize();
        }

        private static void Destroy()
        {
            foreach (var block in Blocks)
                block.Dispose();
            Blocks.Clear();
            Initialised = false;

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