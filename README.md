[![GitHub license](https://img.shields.io/github/license/lench4991/AdvancedControlsMod.svg)](https://github.com/lench4991/AdvancedControlsMod/blob/master/LICENSE.md)
[![GitHub release](https://img.shields.io/github/release/lench4991/AdvancedControlsMod.svg)](https://github.com/lench4991/AdvancedControlsMod/releases)
[![GitHub total downloads](https://img.shields.io/github/downloads/lench4991/AdvancedControlsMod/total.svg)](https://github.com/lench4991/AdvancedControlsMod/releases)

# Advanced Controls Mod
Besiege v0.27

This mod allows you to bind different input axes to block's controls.
With highly customisable controls and input axes, you can control your machine in many new ways.

## Functionality
Input axes include controller and joystick axes, one or two key axes and custom programmed axes.
With them you can control all sliders, cogs, wheels, springs, ropes, drills, steering hinges and steering blocks.

## Limitations
Each block can have multiple active controls at once, but note that there are some inherent limitations:
Steering blocks' angle control is overriden by input control if active.
Angle control cannot move the block past it's limits.
Input controls are ignored if the block is set to automatic.
Due to Unity input manager limitations, the mod does not differentiate between different controllers and only recognizes default vertical and horizontal axes.
These are also controlled by keyboard arrow keys, so I recommend binding your other controls to something else.
Hopefully I will be able to resolve these issues in a future update.

## Development
This mod is still in early stages and I have a lot of awesome stuff planed, so be sure to check this post from time to time.
Current future plans include separate input device and axis support, transfer to native Besiege user interface and more.

## Dependency on LenchScripterMod
Interfacing with blocks is done through LenchScripterMod API. This means you will need to install and enable my scripting mod as well.
If you do not want it executing scripts, you can simply toggle Lua off in the settings menu.
Latest version of the scripting mod is included with the release.

How to use
Simply use the key mapper tool and click on a block. Advanced controls window will appear beside it.
