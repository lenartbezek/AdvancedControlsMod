using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using spaar.ModLoader;
using UnityEngine;

namespace Lench.AdvancedControls
{
    /// <summary>
    ///     Wraps an API for accessing machine properties.
    /// </summary>
    public class Machine : IEnumerable<Block>
    {
        /// <summary>
        ///     Currently active machine.
        /// </summary>
        public static Machine Current
        {
            get
            {
                if (_currentMachine != null)
                    return _currentMachine;
                return _currentMachine = new Machine(global::Machine.Active());
            }
        }

        private readonly global::Machine _machine;
        private readonly List<Block> _blocks;
        private static Machine _currentMachine;

        /// <summary>
        ///     Creates a machine from it's game object.
        /// </summary>
        public Machine(global::Machine machine)
        {
            _machine = machine;
            _blocks = machine.BuildingBlocks.Select(Block.Create) as List<Block>;

            Game.OnBlockPlaced += AddBlock;
            Game.OnBlockRemoved += RemoveBlock;
        }

        private void AddBlock(Transform t)
        {
            var bb = t.GetComponent<BlockBehaviour>();
            if (bb) AddBlock(bb);
        }

        private void AddBlock(BlockBehaviour bb)
        {
            _blocks.Add(Block.Create(bb));
        }

        private void RemoveBlock()
        {
            _blocks.RemoveAll(b => !_machine.BuildingBlocks.Exists(bb => bb.Guid == b.GUID));
        }

#pragma warning disable 1591
        public Block this[string id] => GetBlock(id);
        public Block this[BlockBehaviour bb] => GetBlock(bb);
        public Block this[Guid guid] => GetBlock(guid);
#pragma warning restore 1591

        /// <summary>
        ///     Retrieve block by associated BlockBehaviour game object.
        /// </summary>
        /// <param name="bb"></param>
        /// <returns></returns>
        public Block GetBlock(BlockBehaviour bb)
        {
            return _blocks.FirstOrDefault(b => b.BlockBehaviour == bb);
        }

        /// <summary>
        ///     Tries to convert the string to block GUID and calls GetBlock(Guid guid)
        ///     Falls back to searching by ID. Returns the first match or null.
        /// </summary>
        /// <param name="id">GUID or ID string.</param>
        /// <returns>Block object or null if not found.</returns>
        public Block GetBlock(string id)
        {
            try
            {
                return GetBlock(new Guid(id));
            }
            catch
            {
                return _blocks.FirstOrDefault(b => b.ID.ToLower().StartsWith(id));
            }
        }

        /// <summary>
        ///     Retrieve block by GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Block GetBlock(Guid guid)
        {
            return _blocks.FirstOrDefault(b => b.GUID == guid);
        }

        /// <summary>
        ///     Allows iteration through blocks.
        /// </summary>
        public IEnumerator<Block> GetEnumerator() => _blocks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _blocks).GetEnumerator();
        }
    }
}
