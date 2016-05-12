using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedControls.Controls
{
    public static class ControlManager
    {
        public static Dictionary<string, List<Control>> Blocks = new Dictionary<string, List<Control>>();
        
        public static List<Control> GetBlockControls(int BlockID, string GUID)
        {
            if (Blocks.ContainsKey(GUID)) return Blocks[GUID];
            var controls = CreateBlockControls(BlockID, GUID);
            Blocks.Add(GUID, controls);
            return controls;
        }

        public static List<Control> CreateBlockControls(int BlockID, string GUID)
        {
            if (BlockID == (int)BlockType.Wheel ||
                BlockID == (int)BlockType.LargeWheel ||
                BlockID == (int)BlockType.CogMediumPowered)
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
                    new ControlGroup(GUID)
                    {
                        Controls = new Dictionary<string, Control>()
                        {
                            { "Angle", new AngleControl(GUID) },
                            { "Input", new InputControl(GUID) }
                        },
                        Enabled = "Angle"
                    },
                    new SliderControl(GUID) { Slider = "SPEED" }
                };
            }

            if (BlockID == (int)BlockType.Spring ||
                BlockID == (int)BlockType.RopeWinch)
            {
                return new List<Control>()
                {
                    new InputControl(GUID){ PositiveOnly = BlockID == (int)BlockType.Spring },
                    new SliderControl(GUID){ Slider = BlockID == (int)BlockType.Spring ? "STRENGHT" : "SPEED" }
                };
            }

            if (BlockID == (int)BlockType.Suspension)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "SPRING", PositiveOnly = true }
                };
            }

            if (BlockID == (int)BlockType.CircularSaw)
            {
                return new List<Control>()
                {
                    new SliderControl(GUID){ Slider = "SPEED" }
                };
            }

            if (BlockID == (int)BlockType.Drill)
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
                    new SliderControl(GUID){ Slider = "SPEED", PositiveOnly = true }
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

            return new List<Control>();
        }

    }
}
