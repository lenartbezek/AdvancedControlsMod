using System;
using System.Linq;

namespace Lench.AdvancedControls
{
    public class Machine
    {
        private global::Machine _machine;

        public Machine(global::Machine machine)
        {
            _machine = machine;
        }

        public Block this[string id] => GetBlock(id);
        public Block this[BlockBehaviour bb] => GetBlock(bb);
        public Block this[Guid guid] => GetBlock(guid);

        public Block GetBlock(BlockBehaviour bb)
        {
            return Block.Blocks.FirstOrDefault(b => b.BlockBehaviour == bb);
        }

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

        public Block GetBlock(Guid guid)
        {
            return Block.Blocks.FirstOrDefault(b => b.GUID == guid);
        }


    }
}
