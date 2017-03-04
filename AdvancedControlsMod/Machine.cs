using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lench.AdvancedControls
{
    /// <summary>
    ///     Wraps an API for accessing machine properties.
    /// </summary>
    public class Machine : IEnumerable<Block>, IEnumerator<Block>
    {
        private global::Machine _machine;

        /// <summary>
        ///     Creates a machine from it's game object.
        /// </summary>
        public Machine(global::Machine machine)
        {
            _machine = machine.;
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
            return Block.Blocks.FirstOrDefault(b => b.BlockBehaviour == bb);
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
                return Block.Blocks.FirstOrDefault(b => b.ID.ToLower().StartsWith(id));
            }
        }

        /// <summary>
        ///     Retrieve block by GUID.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Block GetBlock(Guid guid)
        {
            return Block.Blocks.FirstOrDefault(b => b.GUID == guid);
        }


#region IterationThroughBlocks
#pragma warning disable 1591

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<Block> GetEnumerator() => this;

        private int _enumeratorPosition;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _enumeratorPosition++;
            return _enumeratorPosition < Block.Blocks.Count;
        }

        public void Reset()
        {
            _enumeratorPosition = 0;
        }

        object IEnumerator.Current => Current;

        public Block Current => Block.Blocks[_enumeratorPosition];

#pragma warning restore 1591

        #endregion

    }
}
