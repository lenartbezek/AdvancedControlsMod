using System;
using System.Collections.Generic;
using System.Linq;
using Lench.AdvancedControls.Axes;
using Lench.AdvancedControls.Resources;

// ReSharper disable RedundantArgumentDefaultValue

namespace Lench.AdvancedControls.Controls
{
    /// <summary>
    /// Control manager handles all controls bound to the machine.
    /// </summary>
    public static class ControlManager
    {
        internal static Dictionary<Guid, List<Control>> Blocks = new Dictionary<Guid, List<Control>>();

        /// <summary>
        /// Gets controls for a block of type BlockID and given GUID.
        /// </summary>
        /// <param name="blockID">BlockType enumerator value.</param>
        /// <param name="guid">GUID of the block.</param>
        /// <returns>Returns a list of controls.</returns>
        public static List<Control> GetBlockControls(int blockID, Guid guid)
        {
            if (Blocks.ContainsKey(guid)) return Blocks[guid];
            var controls = CreateBlockControls(blockID, guid);
            Blocks.Add(guid, controls);
            return controls;
        }

        /// <summary>
        /// Gets controls for a given BlockBehaviour object.
        /// </summary>
        /// <param name="block">BlockBehaviour of the block.</param>
        /// <returns>Returns a list of controls.</returns>
        public static List<Control> GetBlockControls(BlockBehaviour block)
        {
            if (Blocks.ContainsKey(block.Guid)) return Blocks[block.Guid];
            var controls = CreateBlockControls(block.GetBlockID(), block.Guid);
            Blocks.Add(block.Guid, controls);
            return controls;
        }

        /// <summary>
        /// Returns a control with given name.
        /// If such a control is not found, returns null.
        /// </summary>
        /// <param name="blockID">BlockType enumerator value.</param>
        /// <param name="guid">GUID of the block.</param>
        /// <param name="name">Name of the control.</param>
        /// <returns>Returns a Control object.</returns>
        public static Control GetBlockControl(int blockID, Guid guid, string name)
        {
            return GetBlockControls(blockID, guid).FirstOrDefault(c => c.Key == name);
        }

        /// <summary>
        /// Copies the controls from a block with GUID source_block to a block with GUID destination_block.
        /// Ignores mismatching controls.
        /// </summary>
        /// <param name="sourceBlock">GUID of the source block.</param>
        /// <param name="destinationBlock">GUID of the target block.</param>
        public static void CopyBlockControls(Guid sourceBlock, Guid destinationBlock)
        {
            if (!Blocks.ContainsKey(sourceBlock) || !Blocks.ContainsKey(destinationBlock)) return;

            foreach (var src in Blocks[sourceBlock])
                foreach (var tgt in Blocks[destinationBlock])
                    if (src.Key == tgt.Key)
                    {
                        tgt.Enabled = src.Enabled;
                        tgt.Axis = src.Axis;
                        tgt.Min = src.Min;
                        tgt.Center = src.Center;
                        tgt.Max = src.Max;
                    }
        }

        private static List<Control> CreateBlockControls(int blockID, Guid guid)
        {
            switch (blockID)
            {
                case (int)BlockType.Wheel:
                case (int)BlockType.LargeWheel:
                case (int)BlockType.CogMediumPowered:
                case (int)BlockType.Drill:
                    return new List<Control>
                    {
                        new InputControl(guid),
                        new SliderControl(guid)
                        {
                            Slider = "SPEED",
                            Name = Strings.SliderName_Speed
                        }
                    };
                case (int)BlockType.Piston:
                    return new List<Control>
                    {
                        new PositionControl(guid),
                        new SliderControl(guid)
                        {
                            Slider = "SPEED",
                            Name = Strings.SliderName_Speed
                        }
                    };
                case (int)BlockType.SteeringBlock:
                case (int)BlockType.SteeringHinge:
                    return new List<Control>
                    {
                        new AngleControl(guid),
                        new InputControl(guid),
                        new SliderControl(guid)
                        {
                            Slider = "ROTATION SPEED",
                            Name = Strings.SliderName_RotationSpeed
                        }
                    };
                case (int)BlockType.Spring:
                    return new List<Control>
                    {
                        new InputControl(guid)
                        {
                            PositiveOnly = true,
                            Min = 0, Center = 1, Max = 2
                        },
                        new SliderControl(guid)
                        {
                            Slider = "STRENGTH",
                            Name = Strings.SliderName_Strength
                        }
                    };
                case (int)BlockType.RopeWinch:
                    return new List<Control>
                    {
                        new InputControl(guid),
                        new SliderControl(guid)
                        {
                            Slider = "SPEED",
                            Name = Strings.SliderName_Speed
                        }
                    };
                case (int)BlockType.Suspension:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "SPRING",
                            Name = Strings.SliderName_Spring
                        }
                    };
                case (int)BlockType.SpinningBlock:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "SPEED",
                            Name = Strings.SliderName_Speed
                        }
                    };
                case (int)BlockType.CircularSaw:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "SPEED",
                            Name = Strings.SliderName_Speed
                        }
                    };
                case (int)BlockType.Flamethrower:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "RANGE",
                            Name = Strings.SliderName_Range,
                            PositiveOnly = true,
                            Min = 0, Center = 1, Max = 2
                        }
                    };
                case (int)BlockType.FlyingBlock:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "FLYING SPEED",
                            Name = Strings.SliderName_FlyingSpeed,
                            PositiveOnly = true,
                            Min = 0, Center = 1, Max = 2
                        }
                    };
                case (int)BlockType.Vacuum:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "POWER",
                            Name = Strings.SliderName_Power,
                            PositiveOnly = true,
                            Min = 0, Center = 1, Max = 2
                        }
                    };
                case (int)BlockType.WaterCannon:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "POWER",
                            Name = Strings.SliderName_Power,
                            PositiveOnly = true,
                            Min = 0, Center = 1, Max = 2
                        }
                    };
                case 59:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "THRUST",
                            Name = Strings.SliderName_Thrust,
                            PositiveOnly = true,
                            Min = 0, Center = 1, Max = 2
                        },
                        new SliderControl(guid)
                        {
                            Slider = "FLIGHT DURATION",
                            Name = Strings.SliderName_FlightDuration,
                            PositiveOnly = true,
                            Min = 0, Center = 5, Max = 10
                        },
                        new SliderControl(guid){
                            Slider = "EXPLOSIVE CHARGE",
                            Name = Strings.SliderName_ExplosiveCharge,
                            PositiveOnly = true,
                            Min = 0, Center = 1, Max = 2
                        },
                    };
                case (int)BlockType.Balloon:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "BUOYANCY",
                            Name = Strings.SliderName_Buoyancy,
                            PositiveOnly = true,
                            Min = 0, Center = 1, Max = 2
                        },
                        new SliderControl(guid)
                        {
                            Slider = "STRING LENGTH",
                            Name = Strings.SliderName_StringLength,
                            PositiveOnly = true,
                            Min = 0, Center = 1, Max = 2
                        }
                    };
                case (int)BlockType.Ballast:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "MASS",
                            Name = Strings.SliderName_Mass,
                            PositiveOnly = true,
                            Min = 0, Center = 1, Max = 2
                        }
                    };
                case (int)BlockType.CameraBlock:
                    return new List<Control>
                    {
                        new SliderControl(guid)
                        {
                            Slider = "DISTANCE",
                            Name = Strings.SliderName_Distance,
                            PositiveOnly = true,
                            Min = 40, Center = 60, Max = 80
                        },
                        new SliderControl(guid)
                        {
                            Slider = "HEIGHT",
                            Name = Strings.SliderName_Height,
                            Min = 0, Center = 30, Max = 60
                        },
                        new SliderControl(guid)
                        {
                            Slider = "ROTATION",
                            Name = Strings.SliderName_Rotation,
                            Min = -60, Center = 0, Max = 60
                        }
                    };
                case 790:
                    return new List<Control>
                    {
                        new VectorControl(guid, Axis.X),
                        new VectorControl(guid, Axis.Y),
                        new VectorControl(guid, Axis.Z)
                        {
                            Min = -1.25f,
                            Max = 1.25f
                        },
                    };
            }

            return new List<Control>();
        }

        /// <summary>
        /// Returns a list of all active controls on a block.
        /// A control is active if it is enabled and has bound an axis.
        /// </summary>
        /// <param name="guid">GUID of the block.</param>
        /// <returns>Returns a list of controls.</returns>
        public static List<Control> GetActiveBlockControls(Guid guid)
        {
            var list = new List<Control>();
            if (!Blocks.ContainsKey(guid)) return list;

            list.AddRange(Blocks[guid].Where(c => c.Enabled && AxisManager.Get(c.Axis) != null));
            return list;
        }

        /// <summary>
        /// Returns a list of all active controls in the machine.
        /// </summary>
        /// <returns>Returns a list of controls.</returns>
        public static List<Control> GetActiveControls()
        {
            var list = new List<Control>();
            foreach(var guid in Blocks.Keys)
                list.AddRange(GetActiveBlockControls(guid));

            return list;
        }

    }
}
