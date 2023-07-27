# SIRIUS2 

**1. Descriptions**

 SuperEasy Library for Control Scanner and Laser

[![SIRIUS2 DEMO](http://img.youtube.com/vi/wTNeATLo2HQ/0.jpg)](https://youtu.be/wTNeATLo2HQ "SIRIUS2 DEMO")

[![SIRIUS2 DEMO](http://img.youtube.com/vi/3MlErOf4cc8/0.jpg)](https://youtu.be/3MlErOf4cc8 "SIRIUS2 DEMO")


----


**2. Features**
 
 - Support SCANLAB's RTC5, RTC6 and RTC6ethernet controllers.
 - Support SCANLAB's XL-SCAN (syncAxis) controllers.
 - Support 2D,3D scanner field correction.
 - Support powerful 4x4 matrix with stack operations.
 - Support processing unlimited list data by automatically.
 - Support MoF(Marking on the Fly), 2nd head, 3D, Ramp(Automatic Laser Control) and Sky writing.
 - Support measure and profile scanner trajectory with output signals by plotted graph.
 - Support many kinds of laser source control by frequency, duty cycle, analog, digital output signals.
 - Support many kinds of executable demo programs.


----


**3. What's major changes in Sirius2**

|                       |                         Sirius2                       |      Sirius(Old)       |
| :--------------------:|:------------------------------------------------------|:-----------------------|
| Matrix operation      |4x4 (3D)                                               |3x3 (2D)                |
| Camera                |Perspective                                            |Orthogonal              |
| Editor                |3D                                                     |2D                      |
| Render engine         |OpenTK (with shaders)                                  |SharpGL                 |
| Render speed          |Faster                                                 |Acceptable              |
| Customization         |Expandable                                             |Acceptable              |


----


**4. Modules**

 - spirallab.sirius2.dll
 - spirallab.sirius2.winforms.dll
    - Target frameworks: NET Framework 4.7.2
    - Target platforms: Windows (x64) only
 - dependencies
    - SCANLAB RTC5 (2022.11.11)
    - SCANLAB RTC6 v.1.15.4 (2023.01.23)
    - SCANLAB syncAXIS v.1.8.2 (2023.3.9)
    - OpenTK v3.3.3 (https://www.nuget.org/packages/OpenTK/3.3.3)
    - OpenTK.GLControl v3.3.3 (https://www.nuget.org/packages/OpenTK.GLControl/)


----


**5. How to use ?**

 - Demo 'init' console project for beginner
 - Demo 'editor_basic' winforms project for user interface
 - Demo 'editor_entity' winforms project for create entities
    - config "config.ini" for RTC5 or RTC6
    - config "config_syncaxis.ini" for XL-SCAN

 *The program running 30 mins at evalution copy mode !*

  
----


**6. Author**
 
 - 2023 Copyright to (c)SpiralLAB.
 - All rights reserved. 
 - Homepage: http://spirallab.co.kr
 - Email: hcchoi@spirallab.co.kr
 

----


**7. Version history**


DEVELOPER PREVIEW VERSION. IT WIIL BE CHANGED WITHOUT NOTIFICATION !


* 2023.7.28 v0.8.3
  - added) sirius text entity
    - cxf font
    - lff font
  - fixed) file open exception with curve entity

* 2023.7.26 v0.8.2
  - added) hatch with polygon
  - added) expand or shrink path for polyline 

* 2023.7.21 v0.8.1
  - added) new namespaces
    - spiralab.sirius2.rtc 
    - spiralab.sirius2.rtc.syncaxis
  - added) changeable camera look at position
  - added) encode and decode for unicode string 
  - fixed) hatch bug at some entities
  - fixed) unable to edit bug at propertygrid

* 2023.7.14 v0.8
  - added) preliminary document (doc\sirius2)
  - added) new namespace for marker, opengl
  - added) import dxf with text entity 
  - removed) path optimizer license
  - fixed) improved stability

* 2023.7.7 v0.7
  - added) datamatrix barcode entity
  - added) qrcode barcode entity
  - fixed) imagetext 
     - font style and alignment
     - fill mode
  - added) import file with preview
  - added) extract hatches 

* 2023.7.1 v0.6 (developer preview version) 
  - added) text entity 
  - added) write data, write data ext16 entity
  - added) ramp factor at line, polyline for automatic laser control by defined vector
  - added) (experimental) hard jump at points 
  - fixed) render very tiny arc entity
  - added) editor_ui demo

* 2023.6.26 v0.5
  - added) hatchable, reversable, textconvertible interface
  - added) hpgl(plt) entity
  - added) editor_marker demo

* 2023.6.16 v0.4
  - added) alignment 
  - added) size, rotate, alignment form
  - added) move camera at in/out
  - added) path optimizer
  - fixed) extract points cloud without traslation factor
  - fixed) bounding box with min/max
  - fixed) in/out 
  - fixed) rotate x,y with matrix (fixed. listArcTo)
  - fixed) texture with selection color

* 2023.6.7 v0.3
  - added) vertex and fragment shader has applied
  - added) extract points clouds from STL and convert 3D field correction from points cloud
  - added) entity Mof 
     - wait encoder 
  - added) entity image with texture
     -  raster operation 

* 2023.5.27 v0.2
   - added) render engine by OpenTK
   - added) editor demo
      - added) point, line, arc, polyline, spiral, group, stl entities
      - added) pen, measurement entities
      - added) marker (seperated marker rtc, syncaxis)
      - added) user controls (log, pen, editor, dio, rtc, laser, ...)
      - added) multi languages

* 2023.4.8 v0.1
   - first commit 
  

----


**8. Known Issues**

* Editor
  - Editing EntityBlock is not supported yet
  - Hatch inside of entity is not supported yet (extract hatch only)
  - Selection entities by rubber banding is not supported yet
  - Multi-languages are not supported yet
  
  