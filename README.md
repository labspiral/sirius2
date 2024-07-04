# SIRIUS2 

**1. Descriptions**

 SuperEasy Library for Control Scanner and Laser


![3dengine](https://user-images.githubusercontent.com/58460570/271742677-780f1905-9248-4873-b457-685cb2b45292.png)

![freecamera](https://user-images.githubusercontent.com/58460570/271741908-eb3df067-329c-483f-a983-073b6e32e95c.png)

![dxf](https://user-images.githubusercontent.com/58460570/271741915-1b836b5d-f386-47f1-aca5-f0c24b1536ee.png)

![simulation](https://user-images.githubusercontent.com/58460570/271742410-2527b985-e64b-4146-97cb-273522a01b99.png)

![3dcalibration](https://user-images.githubusercontent.com/58460570/273743004-802904d1-4142-4eda-9282-f810a3b5bf11.png)

![powermapping](https://github.com/labspiral/sirius2/assets/58460570/f7d4bc39-6ef3-4292-bc2d-3e8c9379fb6b)

![script](https://github.com/labspiral/sirius2/assets/58460570/6fab7058-a88b-443d-a7f0-a8c0a914c01a)

![gridchecker](https://user-images.githubusercontent.com/58460570/279851007-10e24e50-c205-4c68-a62f-2410af495d2d.png)



----


**2. Features**
 
 - Support SCANLAB's RTC controllers.
    - RTC4
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
 - Support stream parser software.
 - Support many kinds of laser source controls.
    - Frequency
    - Duty cycle
    - Analog output
    - Digital output 
 - Support specific laser source vendors.
    - AdvancedOptoWave (AOPico, AOPico Precision, Fotia)
    - Coherent (Avia LX, Diamond C-Series)
    - IPG (YLP N, Type D, Type E, ULP N)
    - JPT (Type E)
    - Photonics Industry (DX, RGH AIO)
    - Spectra Physics (Hippo, Talon)
 - Support many kinds of powermeters.
    - Coherent (PowerMax)
    - Thorlabs (by OPM)
    - Ophir (by StarLab)
 - Support powermap table for compensate output laser power.
 - Support remote controls.
    - TCP/IP communication
    - Serial(RS-232) communication
 - Support powerful external script by C# language.
 - Open sourced codes with editor, marker, laser source control for customization.
 - Support many kinds of executable demo programs.


----


**3. What's major changes in Sirius2**

|                       |                         Sirius2                       |    Sirius (Deprecated)   |
|:---------------------:|:------------------------------------------------------|:-------------------------|
| Matrix operation      |4x4 (3D)                                               |3x3 (2D)                  |
| Camera                |Perspective                                            |Orthogonal                |
| Editor                |3D                                                     |2D                        |
| Render engine         |OpenTK                                                 |SharpGL                   |
| Render speed          |Faster                                                 |Acceptable                |
| Field correction      |correXionPro and CalibrationTool                       |correXionPro              |
| Font                  |cxf, lff format and Windows fonts (semi ocr also)      |external ttf, cxf format  |
| Remote control        |Supported (TCP/IP, Serial)                             |x                         |
| Script                |C# script                                              |x                         |
| Processing on the fly |Classic and Fly extension                              |Classic                   |
| Stream parser         |Supported                                              |x                         |
| Multi-language        |Supported                                              |x                         |
| Customization         |Expandable                                             |Acceptable                |


----


**4. Libraries**

 - spirallab.sirius2.dll
 - spirallab.sirius2.winforms.dll
    - Target frameworks: .NET Framework 4.7.2
    - Target platforms: Windows 
 - Dependencies
    - SCANLAB RTC4: (2023.11.02)
    - SCANLAB RTC5: (2022.11.11)
    - SCANLAB RTC6: v.1.18.0 (2024.6.17)
    - SCANLAB syncAXIS: v.1.8.2 (2023.3.9)
    - OpenTK: v3.3.3 (https://www.nuget.org/packages/OpenTK/3.3.3)
    - OpenTK.GLControl: v3.3.3 (https://www.nuget.org/packages/OpenTK.GLControl/)
  - Dependencies (optional)
    - OphirPhotonics StarLab v3.9 (https://www.ophiropt.com/en/g/starlab-for-usb)
    - Thorlabs OPM v5.0 (https://www.thorlabs.com/software_pages/ViewSoftwarePage.cfm?Code=OPM)


----


**5. How to use ?**

 - Copy all files and subdirectories at 'bin' to your working(or output) directory
 - Add reference 'spirallab.sirius2.dll', 'spirallab.sirius2.winforms.dll', 'OpenTK.dll'(optional) into your project
  
  
 ----


**6. Examples**

 - Demo 'init' console project for beginner
 - Demo 'laserpower' console project for customized laser source (open sourced)
 - Demo 'powermeter' console project for customized powermeter (open sourced)
 - Demo 'editor_basic', 'editor_basic_v2'  winforms project for beginner
    - config 'config.ini' for RTC4, RTC5 or RTC6
    - config 'config_syncaxis.ini' for XL-SCAN
 - Demo 'editor_entity', 'editor_entity_v2' winforms project for create entities
 - Demo 'editor_barcode' winforms project for mark individual barcode entities
 - Demo 'editor_mof' winforms project for encoder based MoF
 - Demo 'editor_mof_barcode' winforms project for mark text, barcode by script with MoF
 - Demo 'editor_mof_text' winforms project for mark out of ranged texts by script with MoF
 - Demo 'editor_dio' winforms project for control digital input/output
 - Demo 'editor_multiple' winforms project for multiple RTC instances
 - Demo 'editor_marker' winforms project for customized marker (open sourced)
 - Demo 'editor_ui' winforms project for customized ui (open sourced)


----

  
**7. Copyrights**
 
 - Evaluation copy mode would be activated during 30 mins without license.
 - All rights reserved. 2021-2024 Copyright to (c)SpiralLAB. 
 - RTC and syncAXIS are trademarks of (c)SCANLAB GmbH.
 - Homepage: http://spirallab.co.kr
 - Email: <a href="mailto:hcchoi@spirallab.co.kr">hcchoi@spirallab.co.kr</a> 
 

----


**8. Version history**

* 2024.7.5 v.1.39.1600
  - added) new SiriusEditorControlV2 control 
     - editor_basic_v2 demo project
     - editor_entity_v2 demo project
  - added) ActBlock at document  
  - fixed) invalid powermap events
  - fixed) updated marker status with IPowerMap.IsError status
  - fixed) exception when convert to block entity

* 2024.7.1 v.1.38.1580
  - fixed) thorlabs powermeter 
     - added) serial communication
  - fixed) can be edit length of line

* 2024.6.24 v.1.37.1575
  - fixed) ListRasterLine signature
     - jump to start location by automatically
     - skip(or jump to next pixel) if pixel duration time is below 10 usec at JumpAndShoot mode for speed up
   
* 2024.6.17 v.1.36.1571
  - updated) RTC6 v.1.18.0 (2024.6.17)
  - fixed) reverse mark at EntityBarcode1D
  - fixed) reverse mark CellCircle at EntityBarcode2D

* 2024.6.11 v.1.35.1560
  - added) events for wait MoF position by automatically
     - Config.OnMarkSiriusTextEachGlyph for sirius text entity 
     - Config.OnMarkTextEachGlyph for text entity
     - Config.OnMarkImageTextEachGlyph for image text entity 
  - fixed) image text 
     - invalid size when do clone
 
* 2024.6.7 v.1.34.1540
  - added) PDF entity 
     - can be import PDF file by image
     - do marks like as rasterized image
  - added) (experimental) Config.IsWaitEncoderXForEachGlyph 
     - wait each font glyphs by automatically
     - works with siriustext, text entity
  - fixed) exception when start thorlabs powermeter
  - removed) useless dll files

* 2024.6.3 v.1.33.1520
  - fixed) thorlabs powermeter
     - updated) dlls files for OPM v5.0 
     - fixed) seperated instance for specific(x32 or x64) runtime 
  - fixed) ophir powermeter
     - updated) starlab v3.9
  - fixed) image, imagetext entity
     - added) zigzag mark order
  - fixed) text entity
     - removed) duplicated vertices 

* 2024.5.27 v.1.32.1500
  - added) editor_wpf demo 

* 2024.5.21 v.1.31.1490
  - added) editor_mof_text demo 
  - fixed) range can be checked by IRtc.FieldSizeLimit 
  - fixed) fail to initialize RTC4 
  
* 2024.5.14 v.1.30.1460
  - RTC6e 
     - added) high performance mode 
     - added) connection loss behavior
  - added) (experimental) global hard jump mode
     - can be set by Config.IsConvertJumpToHardJump
  - added) (experimental) interactive camera
  - refactor) IRtcMoF signatures to support matrix stack
  - fixed) invisible ray when do simulation 
  - fixed) invalid Bit flags at RTC

* 2024.5.2 v.1.29.1420
  - added) EntityStitchedImage entity 
  - added) (experimental version) RTC4 controller
  - added) internal measurement plot form
  - added) output pulse syncronization mode at RTC6
  - refactor) IRtc interfaces
     - added) IRtcVariableDelay interface
     - added) IRtcConditionalIO interface
     - added) IRtcInformation interface 
     - added) IRtcSignalLevel interface 
     - added) IRtcFreeVariable interface 

* 2024.4.25 v.1.28.1360
  - fixed) reversed start/end when mark at line entity
  - refactor) MoveToCursor
     - added) CursorPositionList at EditorUserControl
  - added) show(or hide) log window 
  - added) zoom to fit with margin scale
  - fixed) IDocument can be replaceable now
  - fixed) convert measurement z data format correctly (like as SampleAZ_Coor,...) 
  - fixed) editor_ui demo project by usercontrol
  - fixed) editor_marker demo project 
  - removed) editor_laser demo project

* 2024.4.22 v.1.27.1330
  - added) model sx, sy, sz scale by individually
  - added) support expand/shrink at arc, rectangle, triangle entity
  - added) support clipping at point entity
  - added) display flag values with checked listbox control
  - added) support custom alignment 
  - fixed) redesign EditorCtrl UI
  - fixed) invalid CtlLaserControlSignal at rtc
  
* 2024.4.15 v.1.26.1270
  - added) triangle entity 
  - added) editor_laser project
  - added) CtlMatrix, ListMatrix functions at RTC interface
  - added) remote virtual
     - remote control mode switched to not editable view
  - fixed) MoF
     - supported negative(-) encoder scale 
     - supported negative(-) simulation speed
  - fixed) reset offset array when document has opened
  - fixed) marker function signature
  - fixed) bugs
     - deadlock(or race) condition
     - reduce too many event for propertychanged

* 2024.4.8 v.1.25.1220
  - added) select correction table entity
  - added) calculate approx. mof velocity
  - added) render stipple lines if markerable is false
  - added) ready target document at marker
  - fixed) hit test bug at rectangle, group and stl entity 
  - fixed) create grids with invalid interval   
  
* 2024.4.2 v.1.24.1190
  - added) editor_dio demo project 
  - added) ScriptHelper 
     - user can read(or write) script property values
     - renamed) script file and instance at marker 
  - refactor) IScript interface
  - fixed) save measurement result by raw data format
  - fixed) ko-KR language resources for IRtc, ILaser
  - fixed) exception when initialize syncaxis instance

* 2024.3.27 v.1.23.1170
  - fixed) marks bug at IRtc.ListArc  
  - fixed) display invalid category name at propertygrid

* 2024.3.23 v.1.22.1140
  - added) semi orc font files
     - semi_ocr_single.cxf
     - semi_ocr_double.cxf
  - added) mark sirius text with reverse order
  - added) read/write script values by remote communication
  - added) editor_tsv demo project
  - fixed) scaling bug at siriustext entity
  - fixed) auto scale mode to 'font' at forms
  - fixed) refactor DIO forms
   
* 2024.3.18 v.1.21.1120
  - added) entity_mof_barcode demo project 
  - added) mark barcodes with reverse order
  - added) wait extension1 io input condition 
  - added) read/write free variable
  - added) script event and navigate line of script code if failed
  - added) OnChanged event at IDInput
  - fixed) crash bug when select multi-language
 
* 2024.3.12 v.1.20.1110
  - added) laser on/off shift with SCANahead at RTC6 
  - fixed) create RTC DIOs by manually
  - fixed) config RTC laser1/2 signal levels
  - fixed) OnMoveToCursor event handler

* 2024.3.6 v.1.19.1100
  - fixed) arithmetic exception when initializing RTC card
  - added) (experimental) clipping(or divide) entities
  - fixed) PoD(pulse on demand)
     - spot distance control for SCANahead
  - fixed) enabled mouse hot tracking by default
  - fixed) IDOutput.OutOff arithmetic exception bug 

* 2024.2.7 v.1.18.1080
  - added) IRtcMoFExtension interface for RTC6
     - aka. Fly extension
  - added) mof_xy_extension demo project

* 2024.1.17 v.1.17.1050
  - added) event for Config.OnMoveToCursor (shortcut: F9)
  - changed) marker keyboard (shortcut: F5, F6 and F8)
  - fixed) render issue at blockinsert entity
  - fixed) stream parser will be saved with more detail information
  
* 2024.1.3 v.1.16.1020
  - added) MarkerFast for fast processing
  - fixed) IMarker for more customizable
  - added) (experimental) RtcStreamParserHelper
     - by SCANLAB StreamParser v1.1 
  - added) stream parser demo project
  - fixed) config values has renamed

* 2023.12.22 v.1.15.1000
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
  