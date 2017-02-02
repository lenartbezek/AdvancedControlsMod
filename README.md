[![GitHub license](https://img.shields.io/github/license/lench4991/AdvancedControlsMod.svg)](https://github.com/lench4991/AdvancedControlsMod/blob/master/LICENSE.md)
[![GitHub release](https://img.shields.io/github/release/lench4991/AdvancedControlsMod.svg)](https://github.com/lench4991/AdvancedControlsMod/releases)
[![GitHub total downloads](https://img.shields.io/github/downloads/lench4991/AdvancedControlsMod/total.svg)](https://github.com/lench4991/AdvancedControlsMod/releases)

# Advanced Controls Mod

This mod allows you to use joysticks and controllers in Besiege using SDL2 engine. With highly customisable controls and input axes, you can control your machine in many new ways. Additionally it implements a Python 2.7 engine to allow you to custom script your input logic and exposes an API to [LenchScripterMod](https://github.com/lench4991/LenchScripterMod).

### Installation

You will need [Spaar's ModLoader](https://github.com/spaar/besiege-modloader) to use this mod.
To install, place AdvancedControlsMod.dll in Besiege_Data/Mods folder. All mod assets will be downloaded automatically when needed.

### How to use

When installed, the control mapper will pop up when you use the key mapping tool. It's designed as a framework with two layers:

* Input axes take input from joystick, controller, mouse, keys or custom script and return a value in interval from -1 to 1. These are machine independent and device bound. They are meant to be configured per device and used across machines.
* Input controls take the value from input axes and apply it to the block's function. Every control has it's own interval interval in which it applies values. Every control is bound to a machine block and is saved with the machine.

![gif](http://i.imgur.com/79naVly.gif)