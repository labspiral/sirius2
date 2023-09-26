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
 - Support MoF(Marking on the Fly), 2nd head, 3D and Sky writing.
 - Support Ramp(Automatic Laser Control) by position dependent, velocity dependent and defined-vector.
 - Support SCANahead control, SDC(Spot Distance Control) with RTC6.
 - Support measure and profile scanner trajectory with output signals by plotted graph.
 - Support many kinds of laser source control by frequency, duty cycle, analog, digital output signals.
 - Open sourced code with editor, marker, laser and pen control for customization.
 - Support many kinds of executable demo programs.


----


**3. What's major changes in Sirius2**

|                       |                         Sirius2                       |       Sirius(Old)       |
|:---------------------:|:------------------------------------------------------|:------------------------|
| Matrix operation      |4x4 (3D)                                               |3x3 (2D)                 |
| Camera                |Perspective                                            |Orthogonal               |
| Editor                |3D                                                     |2D                       |
| Render engine         |OpenTK (with shaders)                                  |SharpGL                  |
| Render speed          |Faster                                                 |Acceptable               |
| Field correction      |correXionPro and CalibrationTool                       |correXionPro             |
| Font                  |Windows fonts and cxf, lff format                      |ttf, cxf                 | 
| Customization         |Expandable                                             |Acceptable               |


----


**4. Libraries**

 - spirallab.sirius2.dll
    - Target frameworks: .NET standard 2.0, .NET 6.0 or .NET Framework 4.7.2
    - Target platforms: Windows (x64) only
 - spirallab.sirius2.winforms.dll
    - Target frameworks: .NET Framework 4.7.2 only
    - Target platforms: Windows (x64) only
 - Dependencies
    - SCANLAB RTC5 (2022.11.11)
    - SCANLAB RTC6 v.1.15.4 (2023.01.23)
    - SCANLAB syncAXIS v.1.8.2 (2023.3.9)
    - OpenTK v3.3.3 (https://www.nuget.org/packages/OpenTK/3.3.3)
    - OpenTK.GLControl v3.3.3 (https://www.nuget.org/packages/OpenTK.GLControl/)


----


**5. How to use ?**

 - Demo 'init' console project for beginner
 - Demo 'editor_basic' winforms project for beginner
 - Demo 'editor_entity' winforms project for create entities
    - config "config.ini" for RTC5 or RTC6
    - config "config_syncaxis.ini" for XL-SCAN
 - Demo 'editor_marker' winforms project for custom marker
 - Demo 'editor_remote' winforms project for control by remotely 
 - Demo 'editor_barcode' winforms project for mark individual barcode entities
 - Demo 'editor_ui' winforms project for custom ui
 - Demo 'editor_multiple' winforms project for multiple instances


----

  
**6. License**

 * Per instance
   - Option1: MoF (For xy or angular)
   - Option2: 3D (For 3d calibration)
 * Evaluation mode would be activated during 30 mins without license
 
 
----


**7. Copyright**
 
 - 2023 Copyright to (c)SpiralLAB. All rights reserved. 
 - Homepage: http://spirallab.co.kr
 - Email: <a href="mailto:hcchoi@spirallab.co.kr">hcchoi@spirallab.co.kr</a> 
 

----


**8. Version history**

* 2023.9.26 v.1.3.770
  - added) pdf417 barcode entity  
  - added) SCANahead demo project
  - added) optimize demo project
  - added) IRtcRaster at Rtc6SyncAxis
  - fixed) barcode 2d with fixed aspect ratio

* 2023.9.21 v.1.2.760
  - added) raster modes
     - modes: jump and shoot or micro-vector
     - support raster mode at image and barcode entity
  - added) OnMarkPen event 
  - added) repeat list buffer at marker
  - added) ListLaserPower at rtc6
  - added) list pause or restart at RtcExtension
  - fixed) rename automatic laser control to alc

* 2023.9.15 v.1.1.740
  - added) editor_3d demo project 
  - added) trepan entity
  - added) jump to entity
  - added) korean language resources (koKR) 
  - fixed) 3D calibration
     - updated) SCANLAB's calibration library to v1.4.1.1
     - fixed) duplicated vertices bug at pointscloud
     - fixed) fixed invalid directional vector at plane
  - fixed) invalid rubber banding when out of view control  
  - fixed) kz-scale bug at RTC6
  - added) support EntityRampBegin/End at syncAXIS

* 2023.9.6 v.1.0.720 (Release Candidate Version)
  - added) 1D barcode entity
  - added) 3D calibration by plane
  - added) hatch within entity
  - added) multi-languages
  - fixed) renamed EntityBarcodeBase to EntityBarcode2DBase

* 2023.9.1 v.0.9.8
  - added) entity_barcode demo project
     - fixed) OnTextConvert events
  - added) MarkProcedures at IMarker
  - added) ICalibration3D interface
     - fixed) extract pointscloud bugs
  - added) EntityZDelta entity

* 2023.8.25 v.0.9.6
  - added) EntityWriteDataExt16Cond entity
  - added) EntityWaitDataExt16Cond entity
  - added) ManualUserControl winforms
  - fixed) config.ini for support 2nd head

* 2023.8.22 v.0.9.5
  - added) support compressed file format by optional
     - Config.IsCompressedFileFormat
  - fixed) EntityPointsCloud
     - added) event OnPointsCloudCalibrated 
     - added) IArrayOfOffset interface (for multiple 3d models)
     - added) configurable z order when extract vertices
     - added) support load and select 3d correction table by optional
  - fixed) bugs
     - invalid pen color at propertygrid by double-click
     - background color at DIO winforms
     - options at path optimizer winforms
     - wrong z positon with alignment 
     - invalid category names at propertygrid
     - invalid with OnResize 

* 2023.8.16 v.0.9.3
  - added) editor_multiple demo
  - added) zoom to fit (CTRL + 'F')
  - added) editorhelper class for easy to use
  - added) editable real bound box
  - changed) file format with header information
  - added) event OnOpenSirius, OnSaveSirius at editor
  - changed) event OnEnded at marker
  - changed) config.ini to support multiple devices

* 2023.8.10 v0.9.1
  - added) hit-test by rubber banding 
  - fixed) minor bugs
 
* 2023.8.8 v0.9.0
  - added) editor_remote project
  - added) new event handler 'OnScannerFieldCorrection2DApply'
  - added) dashed mark/arc functions at rtc6syncaxis
  - modified) Config.OnTextConvert event
     - text and barcode entities are now support convertible text format
     - also possible to marks at multiple offset positions with individual data
  - added) (experimental) ITextRegisterable interface with characterset text entities 
  - added) min/max digital out value at laservirtual
  
* 2023.8.1 v0.8.5
  - added) circular text entity
  - added) support High DPI

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

* 2023.7.1 v0.6 (Developer Preview Version) 
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

* 2023.4.8 v0.1
   - first commit 
  

----


**9. Known Issues**

* General
  - ActDivide is not supported yet  
  