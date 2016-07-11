using System;
using System.Collections.Generic;
using Lench.AdvancedControls.Axes;

namespace Lench.AdvancedControls.Controls
{
    public static class ControlManager
    {
        internal static Dictionary<Guid, List<Control>> Blocks = new Dictionary<Guid, List<Control>>();

        public static List<Control> GetBlockControls(int BlockID, Guid GUID)
        {
            if (Blocks.ContainsKey(GUID)) return Blocks[GUID];
            var controls = CreateBlockControls(BlockID, GUID);
            Blocks.Add(GUID, controls);
            return controls;
        }

        public static List<Control> GetBlockControls(BlockBehaviour block)
        {
            if (Blocks.ContainsKey(block.Guid)) return Blocks[block.Guid];
            var controls = CreateBlockControls(block.GetBlockID(), block.Guid);
            Blocks.Add(block.Guid, controls);
            return controls;
        }

        public static Control GetBlockControl(int BlockID, Guid GUID, string name)
        {
            foreach (Control c in GetBlockControls(BlockID, GUID))
                if (c.Name == name) return c;
            return null;
        }

        internal static void CopyBlockControls(Guid source_block, Guid destination_block)
        {
            if (!Blocks.ContainsKey(source_block) || !Blocks.ContainsKey(destination_block)) return;

            foreach (Control src in Blocks[source_block])
                foreach (Control tgt in Blocks[destination_block])
                    if (src.Name == tgt.Name)
                    {
                        tgt.Enabled = src.Enabled;
                        tgt.Axis = src.Axis;
                        tgt.Min = src.Min;
                        tgt.Center = src.Center;
                        tgt.Max = src.Max;
                    }
        }

        internal static List<Control> CreateBlockControls(int BlockID, Guid GUID)
        {
            if (BlockID == (int)BlockType.Wheel ||
                BlockID == (int)BlockType.LargeWheel ||
                BlockID == (int)BlockType.CogMediumPowered || 
                BlockID == (int)BlockType.Drill)
            {
                return new List<Control>()
                {
                    new InputControl(GUID),
                    new SliderControl(GUID){ Slider = "SPEED"}
                };
            }

            if (BlockID == (int)BlockType.Piston)
            {
                return new List<Control>()
                {
                    new PositionControl(GUID),
                    new SliderControl(GUID){ Slider = "SPEED"}
                };
            }

            if (BlockID == (int)BlockType.SteeringBlock ||
                BlockID == (int)BlockType.SteeringHinge)
            {
                return new List<Control>()
                {
                    new AngleControl(GUID),
                    new InputControl(GUID),
                    new SliderControl(GUID) { Slider = "ROTATION SPEED" }
                };
            }

            if (BlockID == (int)BlockType.Spring ||
                BlockID == (int)BlockType.RopeWinch)
            {
                return new List<Control>()
                {
                    new InputControl(GUID){ PositiveOnly = BlockID == (int)BlockType.Spring },
                    new SliderControl(GUID){ Slider = BlockID == (int)BlockType.Spring ? "STRENGTH" : "SPEED" }
                };
            }

            if (BlockID == (int)BlockType.Suspension)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "SPRING"}
                };
            }

            if (BlockID == (int)BlockType.SpinningBlock)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "SPEED"}
                };
            }

            if (BlockID == (int)BlockType.CircularSaw)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "SPEED" }
                };
            }
            
            if (BlockID == (int)BlockType.Flamethrower)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "RANGE", PositiveOnly = true }
                };
            }

            if (BlockID == (int)BlockType.WaterCannon)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "POWER", PositiveOnly = true }
                };
            }

            if (BlockID == 59) //Rocket
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "THRUST", PositiveOnly = true },
                    new SliderControl(GUID){ Slider = "FLIGHT DURATION", PositiveOnly = true },
                    new SliderControl(GUID){ Slider = "EXPLOSIVE CHARGE", PositiveOnly = true },
                };
            }

            if (BlockID == (int)BlockType.Balloon)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "BUOYANCY", PositiveOnly = true },
                    new SliderControl(GUID){ Slider = "STRING LENGTH", PositiveOnly = true }
                };
            }

            if (BlockID == (int)BlockType.Ballast)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "MASS", PositiveOnly = true }
                };
            }

            if (BlockID == (int)BlockType.CameraBlock)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "DISTANCE", PositiveOnly = true, Min = 40, Center = 60, Max = 80 },
                    new SliderControl(GUID){ Slider = "HEIGHT", Min = 0, Center = 30, Max = 60 },
                    new SliderControl(GUID){ Slider = "ROTATION", Min = -60, Center = 0, Max = 60 }
                };
            }

            return new List<Control>();
        }

        public static List<Control> GetActiveBlockControls(Guid GUID)
        {
            var list = new List<Control>();
            if (Blocks.ContainsKey(GUID))
            {
                foreach (Control c in Blocks[GUID])
                {
                    if (c.Enabled && AxisManager.Get(c.Axis) != null)
                    {
                        list.Add(c);
                    }
                }
            }
            return list;
        }

        internal static List<Control> GetActiveControls()
        {
            var list = new List<Control>();
            foreach(Guid guid in Blocks.Keys)
            {
                list.AddRange(GetActiveBlockControls(guid));
            }
            return list;
        }

    }
}
