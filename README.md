# SIRIUS2 

**1. Descriptions**

 SuperEasy Library for Control Scanner, Laser and Vision


![3dengine](https://user-images.githubusercontent.com/58460570/271742677-780f1905-9248-4873-b457-685cb2b45292.png)

![freecamera](https://user-images.githubusercontent.com/58460570/271741908-eb3df067-329c-483f-a983-073b6e32e95c.png)

![dxf](https://user-images.githubusercontent.com/58460570/271741915-1b836b5d-f386-47f1-aca5-f0c24b1536ee.png)

![simulation](https://user-images.githubusercontent.com/58460570/271742410-2527b985-e64b-4146-97cb-273522a01b99.png)

![3dcalibration](https://user-images.githubusercontent.com/58460570/273743004-802904d1-4142-4eda-9282-f810a3b5bf11.png)

![powermapping](https://github.com/labspiral/sirius2/assets/58460570/f7d4bc39-6ef3-4292-bc2d-3e8c9379fb6b)

![script](https://github.com/labspiral/sirius2/assets/58460570/6fab7058-a88b-443d-a7f0-a8c0a914c01a)

![gridchecker](https://user-images.githubusercontent.com/58460570/279851007-10e24e50-c205-4c68-a62f-2410af495d2d.png)

![calibration_source](https://github.com/user-attachments/assets/bb83f626-7c8a-4ba8-a83e-1a2782ab4c7c)

![calibration_result](https://github.com/user-attachments/assets/3adb67af-59e9-43ae-a2bb-1b89419aae55)

![stitch_image](https://github.com/user-attachments/assets/4319bd3a-99e7-4851-bc99-2c40a3cf96ce)

![stitch_calibration](https://github.com/user-attachments/assets/0c1c3ba1-f6ca-4c21-a5ef-8535e16f850f)

![barcode_finder](https://github.com/user-attachments/assets/c9800429-748d-4826-bebc-b797d97cee3f)

![pattern_finder](https://github.com/user-attachments/assets/f9715acc-aefc-47d9-9494-1676ab39c080)

![sirius_basic](https://github.com/user-attachments/assets/0eb745a5-bda7-4714-8790-15c130507503)


----


**2. Features**
 
 - Support SCANLAB's RTC controllers.
    - RTC4
    - RTC4e
    - RTC5
    - RTC6 
    - RTC6e 
    - XL-SCAN(RTC6 + ACS) by syncAXIS
 - Support measure and profile scanner trajectory with output signals by plotted graph.
 - Support powerful options.
    - MoF(Marking on the Fly)
    - 2nd head
    - 3D 
    - Sky writing
 - Support ramp(Automatic Laser Control) controls.
    - Position dependent
    - Velocity dependent 
    - Defined-vector
 - Support SCANahead and SDC(Spot Distance Control) control with RTC6.
 - Support 2D, 3D scanner field correction.
 - Support calibration tool for 3D surfaces.
    - Plane 
    - Cone 
    - Cylinder 
    - Points cloud  
 - Support many kinds of laser controls.
    - Frequency
    - Duty cycle
    - Analog output
    - Digital output 
 - Support specific laser source vendors to control and communication.
    - AdvancedOptoWave (AOPico, AOPico Precision, Fotia)
    - Coherent (Avia LX, Diamond C-Series)
    - IPG (YLP N, Type D, Type E, ULP N)
    - JPT (Type E)
    - Photonics Industry (DX, RGH AIO)
    - Spectra Physics (Hippo, Talon)
 - Support many kinds of powermeters with powermap table for compensate output laser power.
    - Coherent (PowerMax)
    - Thorlabs (by OPM)
    - Ophir (by StarLab)
 - Support remote controls.
    - TCP/IP communication
    - Serial(RS-232) communication
 - Various pre-built entities.
    - Point(s), Line, Arc, Polyline, Triangle, Rectangle, Spiral, Trepan, Curve, Raster
    - Layer, Group, Block and Block insert
    - Text, SiriusText, ImageText, Circular text
    - Image, Stitched image, DXF, PDF, ZPL(zebra programming language)
    - QR code, DataMatrix, PDF417 and Barcodes
    - Plane, Cone(or cylinder), STL(Stereo lithography), Point cloud
    - and more control entities
 - Support powerful external script by C# language.
 - Open source codes with editor, marker, remote and laser source control for customization.
 

----


**3.(optional) Vision**
     
 - Release status: developer preview version
 - Support many kinds of cameras.
    - Basler Pylon camera
    - Sentech camera
    - Crevis camera
    - Euresys grabber
    - WebCam (installed on Windows)
    - RTSP (realtime streaming protocol) camera 
 - Supported co-axial camera behind of scanner for merge stitched images.
    - merge stitched images into a large one by automatically
    - integrated RTC control to acquire stitched images
 - Supported image processing for line, cross, circle, blob, pattern finders.
 - Supported 1D and 2D barcode decoders.
    - DataMatrix, QRCode, PDF417, Code128, Code 39, Code93, Codabar, UPCEAN, I2Of5, PharmaCode, Databasr, EANUCC, Postnet, Planet, FourState and so on.
    - Can be queried metrics information for AIM-DPM, ISO-15415, SEMI T10
 - Calibration for resolve distortion by checkerboard and grid dots are supported.
 - User can select image processing engine to OpenCV or VisionPro.
 - Support powerful external script by C# language.
 - Open source codes with ui, camera control for customization.

  
----


**4. What's major changes in Sirius2**

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
| Option                |spirallab.sirius2.vision.dll                           |x                         |


----


**5. Libraries**

 - spirallab.sirius2.dll
 - spirallab.sirius2.winforms.dll
    - Target frameworks: .NET Framework 4.7.2
    - Target platforms: Windows x64 
    - Dependencies
       - SCANLAB RTC4 2023.11.02 (https://www.scanlab.de/en/products/software/rtc-software/download)
       - SCANLAB RTC5 2024.09.27 (https://www.scanlab.de/en/products/software/rtc-software/download)
       - SCANLAB RTC6 v.1.19.1 (2024.11.4) (https://www.scanlab.de/en/products/software/rtc-software/download)
       - SCANLAB syncAXIS: v.1.8.2 (2023.3.9) (https://www.scanlab.de/en/products/software-calibration/syncaxis/download)
       - OpenTK: v3.3.3 (https://www.nuget.org/packages/OpenTK/3.3.3)
       - OpenTK.GLControl: v3.3.3 (https://www.nuget.org/packages/OpenTK.GLControl/)
       - OphirPhotonics StarLab v3.9 (https://www.ophiropt.com/en/g/starlab-for-usb)
       - Thorlabs OPM v5.0 (https://www.thorlabs.com/software_pages/ViewSoftwarePage.cfm?Code=OPM)
       - Zxing v0.16.9.0 (https://github.com/zxing/zxing/releases)
       - Google OrTools 9.6.2534.0 (https://github.com/google/or-tools/releases)
 - (optional) spirallab.sirius2.vision.dll 
    - Target frameworks: .NET Framework 4.7.2
    - Target platforms: Windows x64
    - Dependencies
       - Cognex VisionPro v9.8 (https://support.cognex.com/ko-kr/downloads/detail/visionpro/4464/1033)
       - Basler Pylon runtime v8.0.0 (https://www2.baslerweb.com/en/downloads/software-downloads/software-pylon-8-0-0-windows/)
       - Sentech v1.2.2 (https://sentech.co.jp/en/information/c/update)
       - Euresys Multicam v6.19.0.5375 (https://www.euresys.com/en/download-area/)
       - Crevis MCam40 SDK v4.8.0.8354 (https://www.crevis.co.kr/Customer/download)
       - OpenCVSharp v4.8.0.20230708 (https://www.nuget.org/packages/OpenCvSharp4/4.8.0.20230708)


----


**6. How to use ?**

 - (Basic) Add reference files into your project
    - spirallab.sirius2.dll
    - spirallab.sirius2.winforms.dll
    - OpenTK.dll 
 - (optional) Add reference files into your project 
    - spirallab.sirius2.vision.dll
 - Copy all files and sub-directories at 'bin' to your working(or output) directory
  
  
 ----


**7. Examples**

 - Demo 'init' console project for RTC beginner 
 - Demo 'io' console project for manipulate DIO at RTC
 - Demo 'laser' console project for various laser source
 - Demo 'laserpower' console project for customized laser source 
    - Open sourced laser output power control
 - Demo 'matrix' console project for push/pop matrix at stack 
 - Demo 'fieldcorrection' console project for do scanner field correction
 - Demo 'wobbel' console project for mark wobbel shapes
 - Demo '3d' console project for control 3d space by x,y and z position
 - Demo 'raster' console project for bitmap(or rasterized) operation
 - Demo 'skywriting' console project for sky-writing operation (RTC5,6)
 - Demo 'alc' console project for automatic laser control (aka. Ramp)
 - Demo 'characterset' console project for download(or register) font family
 - Demo 'hardjump' console project for hard jump and select specific tuning mode
 - Demo 'mof_xy' console project for marking on the fly with x,y encoders
 - Demo 'mof_angular' console project for marking on the fly with rotate encoder
 - Demo 'optimize' console project for how to optimize laser and scanner delays
 - Demo 'timed' console project for timed jump and mark control
 - Demo 'powermeter' console project for customized powermeter 
    - Open sourced PowerMeter control
 - Demo 'powermap' console project for customized power mapping
    - Open sourced power mapping, verify and compensate
 - Demo 'multiple' console project for control multiple RTC cards
 - Demo 'scanahead' console project for specific SCANahead control (RTC6)
 - Demo 'syncaxis' console project for specific XL-SCAN control (RTC6 + ACS motion)
  
 - Demo 'editor_basic', 'editor_basic_v2'  winforms project for beginner
    - config [RTC0], [LASER0] at 'config.ini' for RTC4,5,6
    - config [RTC0], [LASER0] at 'config_syncaxis.ini' for XL-SCAN
 - Demo 'editor_entity', 'editor_entity_v2' winforms project for create entities
 - Demo 'editor_barcode' winforms project for mark individual barcode entities
 - Demo 'editor_mof' winforms project for encoder based MoF
 - Demo 'editor_mof_barcode' winforms project for mark text, barcode by script with MoF
 - Demo 'editor_mof_text' winforms project for mark out of ranged texts by script with MoF
 - Demo 'editor_dio' winforms project for control digital input/output
 - Demo 'editor_remote' winforms project for customized tcp/ip server
   - Open sourced Remote control
 - Demo 'editor_multiple' winforms project for multiple RTC instances
    - config [RTC0], [LASER0] and [RTC1], [LASER1] at 'config.ini' 
 - Demo 'editor_marker' winforms project for customized marker 
    - Open sourced Marker control
 - Demo 'editor_laser' winforms project for customized laser UI 
    - Open sourced Laser control
 - Demo 'editor_ui' winforms project for customized ui 
    - Open sourced SiriuseEitorControl, SiriuseEitorControlv2 UI
    
 - Demo 'vision_basic', 'vision_basic_v2' winforms project for beginner
    - config [CAMERA0] at 'config.ini' 
 - Demo 'vision_stitch' winforms project for integrated with scanner control
 - Demo 'vision_camera' winforms project for customized camera
    - Open sourced Camera control
 - Demo 'vision_ui' winforms project for customized ui 
    - Open sourced SiriuseVisionControl, SiriuseVisionControlv2 UI
    
 - Demo 'sirius_basic' winforms project for integrated editor and vision
    - config [RTC0], [LASER0] and [CAMERA0]
    - auto focus
    - extract scanner field correction data from inspection result
    - extract marker offset position from inspection result


----

  
**8. Copyrights**
 
 - Evaluation copy mode would be activated during 30 mins without license.
 - Homepage: http://spirallab.co.kr
 - Email: <a href="mailto:hcchoi@spirallab.co.kr">hcchoi@spirallab.co.kr</a> 
 - RTC and syncAXIS are trademarks of (c)SCANLAB GmbH.
 - All rights reserved. 2018-2024 Copyright to (c)SpiralLAB. 
 

----


**9. Version history**

* 2024.12.23 v.1.48.2111
  - fixed) accurate executed job time
    - added) IJob.ExecutionTimeRtcOnly
    - added) job history at marker

* 2024.11.19 v.1.47.2110
  - fixed) syncaxis
     - exception after marker has finished

* 2024.11.8 v.1.46.2105
  - updated) RTC6 v1.19.1 
  - added) support reverse order for triangle, rectangle, cross entity
  - added) ProcessFocus process for auto focus
  - fixed) fail to save for StitchCalibrator
  - fixed) show invalid license information 

* 2024.11.1 v.1.45.2100
  - added) spirallab.sirius2.vision.dll 
     - added) demo projects for machine vision
        - 'vision_basic', 'vision_basic_v2' for beginner
        - 'vision_stitch' for stitch images with control RTC
        - 'vision_camera' : customized camera 
        - 'vision_ui' : customized ui (open sourced) 
  - added) 'sirius_basic' demo project 
     - integrated editor and vision   
  - updated) SCANLAB RTC5 dll v2024.09.27
  - added) scanner jog control window at manual screen
     - Jog with arrow keys and CTRL, ALT, SHIFT combination
  - fixed) matrix
     - support scale and inversion at entity
     - support scale and inversion at 4x4 basematrix with matrixstack
     - editable primary/secondary internal 3x3 matrix
     - IExtractPolyline.ToPolylines is not remove internal matrix data

* 2024.9.30 v.1.44.1780
  - added) cross entity 
  - added) Config.OnCreateGrids event
     - fixed) can be created grids with each rows and cols
  - fixed) laser path simulation with more dynamics
  - fixed) 3d calibration at cone entity
  - fixed) stitched image
     - renamed) EntityImageStitched
     - modified) image index order 

* 2024.9.10 v.1.43.1770
  - added) preview at marker 
    - by IMarker.Preview()
    - Shortcut: F4
  - fixed) bugs when create usercontrol at design time
  
* 24.9.5 v.1.42.1760
  - added) customizable laser UI 
     - added) editor_laser demo
  - added) timed demo
     - added) IRtcTimed interface
     - fixed) support timed operation for point, line and arc entities
  - added) ZPL(zebra programming language) entity
  - added) RTC4e controller
  - added) (experimental) read raw .ctb file format and plot to graph
  - added) 1D barcode formats (ITF, MSI)
  - added) shortcut for start simulation by 'F1'
  - fixed) exception when import PDF file
  - fixed) IClipRegion.Clip bug at arc entity
  - fixed) removed hard jumps at points entity
  
* 2024.7.23 v.1.41.1630
  - added) skywriting mode4
  - added) editor_multiple2 demo project
  - fixed) exception at editorcontrol if not shown 

* 2024.7.15 v.1.40.1620
  - added) editor_remote demo project
  - added) Config.DecimalPrecision

* 2024.7.5 v.1.39.1600
  - added) new SiriusEditorControlV2 control 
     - editor_basic_v2 demo project
     - editor_entity_v2 demo project
  - added) ActBlock at document  
  - fixed) invalid powermap events
  - fixed) updated marker status with IPowerMap.IsError status
  - fixed) exception when convert to block entity
  - fixed) log messages are not show up at logcontrol

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
  - added) semi ocr font files
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
  