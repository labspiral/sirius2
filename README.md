# SIRIUS2 

**1. Descriptions**

 SuperEasy Library for Control Scanner and Laser


![sirius2_ex1](https://user-images.githubusercontent.com/58460570/271742677-780f1905-9248-4873-b457-685cb2b45292.png)

![sirius2_ex2](https://user-images.githubusercontent.com/58460570/271741908-eb3df067-329c-483f-a983-073b6e32e95c.png)

![sirius2_ex3](https://user-images.githubusercontent.com/58460570/271741915-1b836b5d-f386-47f1-aca5-f0c24b1536ee.png)

![sirius2_ex4](https://user-images.githubusercontent.com/58460570/271742410-2527b985-e64b-4146-97cb-273522a01b99.png)

![sirius2_ex5](https://user-images.githubusercontent.com/58460570/273743004-802904d1-4142-4eda-9282-f810a3b5bf11.png)

![sirius2_ex6](https://user-images.githubusercontent.com/58460570/279851007-10e24e50-c205-4c68-a62f-2410af495d2d.png)


----


**2. Features**
 
 - Support SCANLAB's RTC controllers.
    - RTC5
    - RTC6 
    - RTC6e 
    - XL-SCAN (aka. syncAxis) by RTC6 + ACS 
 - Support 2D, 3D scanner field correction.
 - Support calibration tool for 3D surfaces.
    - Plane 
    - Cone 
    - Cylinder 
    - Points cloud  
 - Support powerful options.
    - MoF(Marking on the Fly)
    - 2nd head
    - 3D 
    - Sky writing
 - Support ramp(Automatic Laser Control) controls.
    - Position dependent
    - Velocity dependent 
    - Defined-vector
 - Support SCANahead control, SDC(Spot Distance Control) with RTC6.
 - Support measure and profile scanner trajectory with output signals by plotted graph.
 - Support many kinds of laser source controls.
    - Frequency
    - Duty cycle
    - Analog output
    - Digital output 
 - Support specific laser source vendors.
    - AdvancedOptoWave 
       - AOPico
       - AOPico Precision,
       - Fotia
    - Coherent
       - Avia LX
       - Diamond C-Series
    - IPG 
       - YLP N
       - YLP Type D
       - YLP Type E
       - YLP ULP N
    - JPT Type E
    - Photonics Industry
       - DX
       - RGH AIO
    - Spectra Physics
       - Hippo
       - Talon
 - Support many kinds of powermeters.
    - Coherent PowerMax
    - Thorlabs (by OPM)
    - Ophir (by StarLab)
 - Support powermap table for compensate output laser power.
 - Support remote controls.
    - TCP/IP communication
    - Serial(RS-232) communication
 - Support powerful external script by C# language.
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
| Font                  |cxf, lff format and Windows fonts                      |ttf, cxf format          |
| Remote control        |Supported (TCP/IP, Serial)                             |None                     |
| Multi-language        |Supported                                              |None                     |
| Customization         |Expandable                                             |Acceptable               |


----


**4. Libraries**

 - Spirallab.sirius2.dll
 - Spirallab.sirius2.winforms.dll
    - Target frameworks: .NET Framework 4.7.2
    - Target platforms: Windows 
 - Dependencies
    - SCANLAB RTC5: (2022.11.11)
    - SCANLAB RTC6: v.1.16.3 (2023.12.19)
    - SCANLAB syncAXIS: v.1.8.2 (2023.3.9)
    - OpenTK: v3.3.3 (https://www.nuget.org/packages/OpenTK/3.3.3)
    - OpenTK.GLControl: v3.3.3 (https://www.nuget.org/packages/OpenTK.GLControl/)


----


**5. How to use ?**

 - Demo 'init' console project for beginner
 - Demo 'editor_basic' winforms project for beginner
    - config "config.ini" for RTC5 or RTC6
    - config "config_syncaxis.ini" for XL-SCAN
 - Demo 'editor_entity' winforms project for create entities
 - Demo 'editor_barcode' winforms project for mark individual barcode entities
 - Demo 'editor_mof' winforms project for encoder based MoF
 - Demo 'editor_multiple' winforms project for multiple instances
 - Demo 'editor_marker' winforms project for custom marker
 - Demo 'editor_ui' winforms project for custom ui


----

  
**6. Copyrights**
 
 - 2023 Copyright to (c)SpiralLAB. All rights reserved. 
 - Homepage: http://spirallab.co.kr
 - Email: <a href="mailto:hcchoi@spirallab.co.kr">hcchoi@spirallab.co.kr</a> 
 - Evaluation copy mode would be activated during 30 mins without license
 

----


**7. Version history**

* 2023.12.2 v.1.15.1000
 - updated) RTC6 v1.16.3 (2023-12-19)
 - fixed) powermap 

* 2023.12.19 v.1.14.990
  - fixed) memory leak by text entity
  - fixed) stability issues
  
* 2023.12.11 v.1.13.965
  - added) script engine for convert text data
  - fixed) OnTextConvert event at IMarker
  
* 2023.12.6 v.1.12.945
  - added) power map with mapping, verify, compensate
  - added) power map demo project
  - fixed) ILaserPowerControl interface for powermap

* 2023.11.23 v.1.11.920
  - added) support various powermeters
    - Coherent PowerMax
    - Thorlabs (by OPM)
    - Ophir (by StarLab)
  - added) powermeter demo project
  - fixed) remote commands

* 2023.11.20 v.1.10.910
  - added) support various laser sources
     - Advanced OptoWave
     - Coherent
     - IPG
     - JPT
     - Photonics Industry
     - Spectra Physics
  - added) support load correction file at RTC propertygrid 
  - added) laser power slider at manual tab
  - fixed) 3D and MoF options are available with evaluation copy mode

* 2023.11.9 v.1.9.890
  - added) RTC functions
     - IRtcStartStopInfo 
     - CtlSimulatedExternalStart 
     - SerialMaxNo at IRtcCharacterSet
  - updated) RTC6_Software_Package_Rev.1.15.5
  - fixed) ImageText render bug 
  - fixed) ITextRegisterable
     - download character set if modified
     - works with matrix (rotate z and scale)
     - support letter space 
     - apply pen parameters bug 
     - mark repeat counts bug

* 2023.11.3 v.1.8.865
  - added) grid checker (extract positions of pattern from image file)
  - added) editor_alc demo
  - added) editor_mof demo
  - added) serial no reset entity
  - fixed) many kinds of bugs at runtime 

* 2023.10.27 v.1.7.850
  - added) raster entity
  - added) ILaserGuideControl interface at ILaser
  - added) shortcuts
     - F5: start marker
     - CTRL + F5: stop marker
     - F6: reset marker

* 2023.10.19 v.1.6.840
  - hot fixed) laser power value has not applied 
  - added) external start delay entity
  - added) more remote commands
  
* 2023.10.18 v.1.5.830
  - added) remote communication 
  - removed) useless remote demo project

* 2023.10.10 v.1.4.780
  - added) 3d calibration by cone(or cylinder) 

* 2023.9.27 v.1.3.770
  - added) pdf417 barcode entity  
  - added) SCANahead demo project
  - added) optimize demo project
  - added) IRtcRaster at Rtc6SyncAxis
  - fixed) barcode 2d with fixed aspect ratio
  - fixed) renamed enum types

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
  