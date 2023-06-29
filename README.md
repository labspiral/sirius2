# SIRIUS2 

**1. Descriptions**

 Super easy library for Control Scanner and Laser

[![SIRIUS2 DEMO](http://img.youtube.com/vi/3Y3fzXr54gQ/0.jpg)](http://www.youtube.com/watch?v=3Y3fzXr54gQ "SIRIUS2 DEMO")

Link: [Youtube](http://www.youtube.com/watch?v=3Y3fzXr54gQ)


----


**2. Features**

 - More easy, simple and expandable architecture to create own laser application.
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


----


**4. Modules**

 - spirallab.sirius2.dll
    - Target frameworks: NET Framework 4.7.2, NET Standard 2.0, NET5 or higher 
    - Dependencies: NLog, Newtonsoft.Json, CalibrationLibrary, syncAXIS, RTC5DLL, RTC6DLL, ...

 - spirallab.sirius2.winforms.dll
    - Target frameworks: NET Framework 4.7.2
    - Dependencies: OpenTK, OpenTK.GLControl, ...


----


**5. How to use ?**

 - Target platforms
    - Windows (x64) only
 - Dll assembly(reference) files
    - spirallab.sirius2.dll 
	- spirallab.sirius2.winforms.dll

 *The program running 30 mins at evalution copy mode !*

  
----


**6. Author**
 
 - Email: hcchoi@spirallab.co.kr
 - Homepage: http://spirallab.co.kr
 - 2023 Copyright to (c)SpiralLAB. All rights reserved.
 

----


**7. Version history**

DEVELOPER PREVIEW VERSION! IT WIIL BE CHANGED WITHOUT NOTIFICATION BEFORE OFFICIAL RELEASE  

* 2023.7.1 v0.6
  - developer preview version 
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
  - Hatch inside of entity is not supported yet (extracted hatch only)
  - Selection entities by rubber banding is not supported yet
  - Editing child of EntityGroup is not supported yet
  - Multi-languages are not supported yet
* General
  - x32 (32bits windows) is not supported officially
  - SIRIUS2 is not compatible with SIRIUS(old version)
  
   
