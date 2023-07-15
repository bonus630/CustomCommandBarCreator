# Custom Shortcut Bar Builder for CorelDRAW Macros
This repository contains a tool for generating custom shortcut bars for macros in CorelDRAW. With this tool, you can create a personalized bar of macros and save it to an empty folder. Later, this folder needs to be copied to the CorelDRAW installation directory, specifically in the **addons folder**. This will allow the shortcut bar to be created using CorelDRAW's addon system.

Administrator privileges is required to use install function, install function rebuilds "DataSource" DLL, maybe is necessary after sharing your bar with a diferen version. 

Compatible versions
* X7.1
* X8
* 2017
* 2018
* 2019
* 2020
* 2021
* 2022
* And updates*

### About

**V1.0** Uses "DynamicCommand", macros is stored in bar folder and loaded in CorelDraw startup

**V2.0** Uses "DataSource", datasource will load e run the macros, macros is stored in bar folder but no extension, CorelDraw dont will load macros in startup

### Extra library
https://github.com/dblock/resourcelib

![PrintScreen 01](print.PNG)

## Advantages

1. Preservation of the custom bar: When restoring CorelDRAW to its factory default settings, our custom bar will not be lost. It will remain available for use even after the restoration.
2. Easy distribution: Sharing the custom shortcut bar is extremely convenient. Simply copy the folder containing the files and share it with interested users. They can then install it in their own addons folder.(V2.0) Maybe require rebuild datasouce using install button from application
3. Simplified removal: If you ever wish to remove the custom shortcut bar, you can easily do so by deleting the folder.

## Disadvantages

1. Macro loading timing: Macros are loaded when the shortcut bar is opened, and they do not benefit from CorelDRAW's macro delay loading system.(V1.0)
2. Duplicate macros in multiple bars: Macros that are repeated in multiple bars will be loaded multiple times according to the number of bars that contain them.
Additionally, this software provides a faster creation speed compared to CorelDRAW's native customization system.(V1.0)
