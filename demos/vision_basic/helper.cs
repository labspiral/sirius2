/*
 * 
 *                                                            ,--,      ,--,                              
 *             ,-.----.                                     ,---.'|   ,---.'|                              
 *   .--.--.   \    /  \     ,---,,-.----.      ,---,       |   | :   |   | :      ,---,           ,---,.  
 *  /  /    '. |   :    \ ,`--.' |\    /  \    '  .' \      :   : |   :   : |     '  .' \        ,'  .'  \ 
 * |  :  /`. / |   |  .\ :|   :  :;   :    \  /  ;    '.    |   ' :   |   ' :    /  ;    '.    ,---.' .' | 
 * ;  |  |--`  .   :  |: |:   |  '|   | .\ : :  :       \   ;   ; '   ;   ; '   :  :       \   |   |  |: | 
 * |  :  ;_    |   |   \ :|   :  |.   : |: | :  |   /\   \  '   | |__ '   | |__ :  |   /\   \  :   :  :  / 
 *  \  \    `. |   : .   /'   '  ;|   |  \ : |  :  ' ;.   : |   | :.'||   | :.'||  :  ' ;.   : :   |    ;  
 *   `----.   \;   | |`-' |   |  ||   : .  / |  |  ;/  \   \'   :    ;'   :    ;|  |  ;/  \   \|   :     \ 
 *   __ \  \  ||   | ;    '   :  ;;   | |  \ '  :  | \  \ ,'|   |  ./ |   |  ./ '  :  | \  \ ,'|   |   . | 
 *  /  /`--'  /:   ' |    |   |  '|   | ;\  \|  |  '  '--'  ;   : ;   ;   : ;   |  |  '  '--'  '   :  '; | 
 * '--'.     / :   : :    '   :  |:   ' | \.'|  :  :        |   ,/    |   ,/    |  :  :        |   |  | ;  
 *   `--'---'  |   | :    ;   |.' :   : :-'  |  | ,'        '---'     '---'     |  | ,'        |   :   /   
 *             `---'.|    '---'   |   |.'    `--''                              `--''          |   | ,'    
 *               `---`            `---'                                                        `----'   
 * 
 * 2023 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : vision helper
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.IO;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.PowerMap;
using SpiralLab.Sirius2.PowerMeter;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Scanner.Rtc.SyncAxis;
using SpiralLab.Sirius2.Vision;
using SpiralLab.Sirius2.Vision.Camera;
using SpiralLab.Sirius2.Vision.Inspector;
using SpiralLab.Sirius2.Vision.Process;

namespace Demos
{
    /// <summary>
    /// Demo vision helper
    /// </summary>
    public static class VisionHelper
    {
        /// <summary>
        /// Your config ini file
        /// </summary>
        public static string ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "config.ini");
        // Config file for XL-SCAN (syncAXIS)
        //public static string ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "config_syncaxis.ini");

        /// <summary>
        /// Initialize sirius2 library
        /// </summary>
        /// <remarks>
        /// Edit <c>ConfigFileName</c> as "config.ini" or "config_syncaxis.ini" before execute
        /// </remarks>
        /// <returns>Success or failed</returns>
        public static bool Initialize()
        {
            // Enable Cognex VisionPro
            SpiralLab.Sirius2.Vision.Config.ProcessEngine = SpiralLab.Sirius2.Vision.Config.ProcessEngines.VisionPro;
            //SpiralLab.Sirius2.Vision.Config.ProcessEngine = SpiralLab.Sirius2.Vision.Config.ProcessEngines.OpenCV;

            return SpiralLab.Sirius2.Core.Initialize();
        }

        /// <summary>
        /// Support multiple languages
        /// </summary>
        public static void SetLanguage()
        {
            string cultureInfo = NativeMethods.ReadIni<string>(ConfigFileName, $"GLOBAL", "LANGUAGE", "en-US");            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureInfo);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cultureInfo);
        }

        /// <summary>
        /// Create devices (like as  <c>ICamera</c>, <c>IInspector</c> )
        /// </summary>
        /// <param name="camera"><c>ICamera</c></param>
        /// <param name="inspector"><c>IInspector</c></param>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="index">Index (assign value if using multiple devices) (0,1,2,...)</param>
        /// <returns>Success or failed</returns>
        public static bool CreateDevices(out ICamera camera, out IInspector inspector, IRtc rtc = null, int index = 0)
        {
            camera = null;
            inspector = null;
            bool success = true;

            #region Initialize Camera
            string cameraType = NativeMethods.ReadIni(ConfigFileName, $"CAMERA{index}", "TYPE", "Virtual");
            var camearSerialNo = NativeMethods.ReadIni(ConfigFileName, $"CAMERA{index}", "SERIAL_NO", string.Empty);
            var camearIPaddress = NativeMethods.ReadIni(ConfigFileName, $"CAMERA{index}", "IP_ADDRESS", string.Empty);
            var cameraRawWidth = NativeMethods.ReadIni<int>(ConfigFileName, $"CAMERA{index}", "WIDTH", 1024);
            var cameraRawHeight = NativeMethods.ReadIni<int>(ConfigFileName, $"CAMERA{index}", "HEIGHT", 768);
            var cameraPixelSize = NativeMethods.ReadIni<double>(ConfigFileName, $"CAMERA{index}", "PIXEL_SIZE", 0.005);
            var lensMag = NativeMethods.ReadIni<double>(ConfigFileName, $"CAMERA{index}", "LENS_MAGNIFICATION", 1);
            var exposureTime = NativeMethods.ReadIni<double>(ConfigFileName, $"CAMERA{index}", "EXPOSURE_TIME", 50 * 1000);
            var cameraFps = NativeMethods.ReadIni<int>(ConfigFileName, $"CAMERA{index}", "FPS", 30);
            var cameraRotateFlipStr = NativeMethods.ReadIni<string>(ConfigFileName, $"CAMERA{index}", "ROTATE_FLIP", "RotateNoneFlipNone");
            RotateFlipType cameraRotateFlip = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), cameraRotateFlipStr);
            int enableStitch = NativeMethods.ReadIni<int>(ConfigFileName, $"CAMERA{index}", $"STITCH_ENABLE");
            var stitchRows = NativeMethods.ReadIni<int>(ConfigFileName, $"CAMERA{index}", "STITCH_ROWS", 5);
            var stitchCols = NativeMethods.ReadIni<int>(ConfigFileName, $"CAMERA{index}", "STITCH_COLS", 7);
            var stitchMarginWidth = NativeMethods.ReadIni<int>(ConfigFileName, $"CAMERA{index}", "STITCH_MARGIN_WIDTH", 5);
            var stitchMarginHeight = NativeMethods.ReadIni<int>(ConfigFileName, $"CAMERA{index}", "STITCH_MARGIN_HEIGHT ", 7);
            switch (cameraType.Trim().ToLower())
            {
                default:
                case "virtual":
                    if (0 == enableStitch)
                    {
                        var virtualCam = CameraFactory.CreateVirtual(index, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip);
                        camera = virtualCam;
                    }
                    else
                    {
                        var virtualCam = CameraFactory.CreateVirtual(index, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip, stitchRows, stitchCols, stitchMarginWidth, stitchMarginHeight);
                        camera = virtualCam;
                    }
                    break;
                case "pylon":
                    if (0 == enableStitch)
                    {
                        var pylon = CameraFactory.CreatePylon(index, camearSerialNo, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraRotateFlip);
                        camera = pylon;
                    }
                    else
                    {
                        var pylon = CameraFactory.CreatePylon(index, camearSerialNo, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraRotateFlip, stitchRows, stitchCols, stitchMarginWidth, stitchMarginHeight);
                        camera = pylon;
                    }
                    break;
                case "sentech":
                    if (0 == enableStitch)
                    {
                        var sentech = CameraFactory.CreateSentech(index, camearIPaddress, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip);
                        camera = sentech;
                    }
                    else
                    {
                        var sentech = CameraFactory.CreateSentech(index, camearIPaddress, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip, stitchRows, stitchCols, stitchMarginWidth, stitchMarginHeight);
                        camera = sentech;
                    }
                    break;
                case "crevis":
                    if (0 == enableStitch)
                    {
                        var pylon = CameraFactory.CreateCrevis(index, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip);
                        camera = pylon;
                    }
                    else
                    {
                        var pylon = CameraFactory.CreateCrevis(index, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip, stitchRows, stitchCols, stitchMarginWidth, stitchMarginHeight);
                        camera = pylon;
                    }
                    break;
                case "euresys":
                    var frameGraberIndex = NativeMethods.ReadIni<uint>(ConfigFileName, $"CAMERA{index}", "FG_INDEX", 0);
                    var camFile = NativeMethods.ReadIni<string>(ConfigFileName, $"CAMERA{index}", "CAM_FILE", string.Empty);
                    var cameraFilePath = Path.Combine(SpiralLab.Sirius2.Vision.Config.CameraPath, camFile);
                    var connectorStr = NativeMethods.ReadIni<string>(ConfigFileName, $"CAMERA{index}", "FG_CONNECTOR", "A");
                    CameraEuresys.Connectors connector = (CameraEuresys.Connectors)Enum.Parse(typeof(CameraEuresys.Connectors), connectorStr);
                    if (0 == enableStitch)
                    {
                        var pylon = CameraFactory.CreateEuresys(index, frameGraberIndex, cameraFilePath, connector, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip);
                        camera = pylon;
                    }
                    else
                    {
                        var pylon = CameraFactory.CreateEuresys(index, frameGraberIndex, cameraFilePath, connector, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip, stitchRows, stitchCols, stitchMarginWidth, stitchMarginHeight);
                        camera = pylon;
                    }
                    break;
                case "webcam":
                    if (0 == enableStitch)
                    {
                        var web = CameraFactory.CreateWebCam(index, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip);
                        camera = web;
                    }
                    else
                    {
                        var cv = CameraFactory.CreateWebCam(index, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip, stitchRows, stitchCols, stitchMarginWidth, stitchMarginHeight);
                        camera = cv;
                    }
                    break;
                case "rtspcam":
                    var rtspAddress = NativeMethods.ReadIni(ConfigFileName, $"CAMERA{index}", "RTSP_ADDRESS", string.Empty);
                    if (0 == enableStitch)
                    {
                        var rtsp = CameraFactory.CreateRTSP(index, rtspAddress, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip);
                        camera = rtsp;
                    }
                    else
                    {
                        var rtsp = CameraFactory.CreateRTSP(index, rtspAddress, cameraRawWidth, cameraRawHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip, stitchRows, stitchCols, stitchMarginWidth, stitchMarginHeight);
                        camera = rtsp;
                    }
                    break;
            }

            // Set default unit conversion
            double xMmPerPixel = camera.PixelSize / camera.LensMagnification;
            double yMmPerPixel = camera.PixelSize / camera.LensMagnification;
            camera.UnitTransform = new UnitTransformBase(xMmPerPixel, yMmPerPixel);
            // Override unit conversion
            int enableTransform = NativeMethods.ReadIni<int>(ConfigFileName, $"CAMERA{index}", $"TRANFORM_ENABLE");
            if (0 != enableTransform)
            {
                camera.UnitTransform.XmmPerPixel = NativeMethods.ReadIni<double>(ConfigFileName, $"CAMERA{index}", "TRANFORM_X_MM_PER_PIXEL", xMmPerPixel);
                camera.UnitTransform.YmmPerPixel = NativeMethods.ReadIni<double>(ConfigFileName, $"CAMERA{index}", "TRANFORM_Y_MM_PER_PIXEL", xMmPerPixel);
                camera.UnitTransform.Angle = NativeMethods.ReadIni<double>(ConfigFileName, $"CAMERA{index}", "TRANFORM_ANGLE", 0);
            }
            camera.OnCalibratorSaved += Camera_OnCalibratorSaved;

            // Load calibration file into camera
            string calibrationFile = NativeMethods.ReadIni(ConfigFileName, $"CAMERA{index}", "CALIBRATION", string.Empty);
            if (!string.IsNullOrEmpty(calibrationFile))
            {
                string calibrationPath = Path.Combine(SpiralLab.Sirius2.Vision.Config.CalibrationPath, calibrationFile);
                if (CenterCalibratorSerializer.Open(calibrationPath, out var calibrator))
                    camera.CenterCalibrator = calibrator;
            }

            if (0 != enableStitch)
            {
                // Load calibration file into camera (if stitching supported)
                if (camera is ICameraStitched cameraStitched)
                {
                    // Create center position for stitced images
                    cameraStitched.CreateStitchedCells();
                    string calibrationStitcedFile = NativeMethods.ReadIni(ConfigFileName, $"CAMERA{index}", "CALIBRATION_STITCHED", string.Empty);
                    if (!string.IsNullOrEmpty(calibrationStitcedFile))
                    {
                        string calibrationPath = Path.Combine(SpiralLab.Sirius2.Vision.Config.CalibrationPath, calibrationStitcedFile);
                        if (StitchCalibratorSerializer.Open(calibrationPath, out var calibrator))
                            cameraStitched.StitchCalibrator = calibrator;
                    }
                    cameraStitched.OnStitchedCalibratorSaved += CameraStitched_OnStitchedCalibratorSaved;
                }
            }
            // Assign RTC instance
            // To enable stitich, Rtc instance must be created and assinged at camera
            camera.Rtc = rtc;

            // Event for to control light intensity 
            camera.OnProcessLightIntensity += Camera_OnProcessLightIntensity;
            // Initialize camera
            success &= camera.Initialize();
            success &= camera.CtlExposureTime(exposureTime);

            // Create inspector for processing
            switch (cameraType.Trim().ToLower())
            {
                default:
                    inspector = InspectorFactory.CreateDefault(0, "Insp0");
                    break;
            }

            // external script file
            var visionScriptFileName = NativeMethods.ReadIni(ConfigFileName, $"INSPECTOR{index}", "SCRIPT_FILE", string.Empty);
            if (!string.IsNullOrEmpty(visionScriptFileName))
                inspector.ScriptFile = Path.Combine(SpiralLab.Sirius2.Vision.Config.ScriptPath, visionScriptFileName);
            #endregion

            return success;
        }
        /// <summary>
        /// Create devices (like as <c>IRtc</c>, <c>IDInput</c>, <c>IDOutput</c>, <c>ILaser</c>, ...)
        /// </summary>
        /// <param name="camera"><c>ICamera</c></param>
        /// <param name="inspector"><c>IInspector</c></param>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="dInExt1">RTC D.Input EXTENSION1 port</param>
        /// <param name="dInLaserPort">RTC D.Input LASER port</param>
        /// <param name="dOutExt1">RTC D.Output EXTENSION1 port</param>
        /// <param name="dOutExt2">RTC D.Output EXTENSION2 port</param>
        /// <param name="dOutLaserPort">RTC D.Output LASER port</param>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="index">Index (assign value if using multiple devices) (0,1,2,...)</param>
        /// <returns>Success or failed</returns>
        public static bool CreateDevices(out ICamera camera, out IInspector inspector, out IRtc rtc, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out ILaser laser, int index = 0)
        {
            camera = null;
            inspector = null;
            rtc = null;
            dInExt1 = null;
            dInLaserPort = null;
            dOutExt1 = null;
            dOutExt2 = null;
            dOutLaserPort = null;
            laser = null;

            bool success = true;

            #region Initialize RTC controller
            // FOV size would be used for calcualte k-factor
            var fov = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "FOV", 100.0f);
            // K-Factor = bits/mm
            // RTC5,6 using 20 bits resolution
            var kfactor = Math.Pow(2, 20) / fov;
            // Field correction file path: \correction\cor_1to1.ct5
            // Default (1:1) correction file
            //var correctionFile = Path.Combine(Config.CorrectionPath, "cor_1to1.ct5");
            string correctionFile = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CORRECTION", "cor_1to1.ct5");
            string correctionPath = Path.Combine(SpiralLab.Sirius2.Config.CorrectionPath, correctionFile);
            RtcSignalLevels signalLevelLaser12 = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SIGNALLEVEL_LASER12", "High") == "High" ? RtcSignalLevels.ActiveHigh : RtcSignalLevels.ActiveLow;
            RtcSignalLevels signalLevelLaserOn = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SIGNALLEVEL_LASERON", "High") == "High" ? RtcSignalLevels.ActiveHigh : RtcSignalLevels.ActiveLow;
            string rtcType = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "TYPE", "Rtc5");
            string sLaserMode = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "LASERMODE", "Yag1");
            LaserModes laserMode = (LaserModes)Enum.Parse(typeof(LaserModes), sLaserMode);
            string ipAddress, subnetMask;
            switch (rtcType.Trim().ToLower())
            {
                default:
                case "virtual":
                    rtc = ScannerFactory.CreateVirtual(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc4":
                    kfactor = Math.Pow(2, 16) / fov;
                    correctionFile = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CORRECTION", "cor_1to1.ctb");
                    rtc = ScannerFactory.CreateRtc4(index, kfactor, laserMode, correctionPath);
                    break;
                case "rtc4e":
                    ipAddress = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "IP_ADDRESS", "192.168.0.100");
                    subnetMask = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SUBNET_MASK", "255.255.255.0");
                    rtc = ScannerFactory.CreateRtc4Ethernet(index, ipAddress, subnetMask, kfactor, laserMode, correctionPath);
                    break;
                case "rtc5":
                    rtc = ScannerFactory.CreateRtc5(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc6":
                    rtc = ScannerFactory.CreateRtc6(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc6e":
                    ipAddress = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "IP_ADDRESS", "192.168.0.100");
                    subnetMask = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SUBNET_MASK", "255.255.255.0");
                    rtc = ScannerFactory.CreateRtc6Ethernet(index, ipAddress, subnetMask, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "syncaxis":
                    string configXmlFileName = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CONFIG_XML", string.Empty);
                    string configXmlFilePath = Path.Combine(SpiralLab.Sirius2.Config.SyncAxisPath, configXmlFileName);
                    rtc = ScannerFactory.CreateRtc6SyncAxis(index, configXmlFilePath);
                    break;
            }

            // Initialize RTC controller
            success &= rtc.Initialize();
            Debug.Assert(success);

            // Load 2nd correction file into TABLE2 if assigned
            string correction2File = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CORRECTION2", string.Empty);
            string correction2Path = Path.Combine(SpiralLab.Sirius2.Config.CorrectionPath, correction2File);
            if (File.Exists(correction2Path))
            {
                success &= rtc.CtlLoadCorrectionFile(CorrectionTables.Table2, correction2Path);
                success &= rtc.CtlSelectCorrection(CorrectionTables.Table1, CorrectionTables.Table2);
            }

            // RTC DIO
            dInExt1 = IOFactory.CreateInputExtension1(rtc);
            dOutExt1 = IOFactory.CreateOutputExtension1(rtc);
            success &= dInExt1.Initialize();
            success &= dOutExt1.Initialize();
            if (rtc is Rtc6SyncAxis)
            {
            }
            else
            {
                dInLaserPort = IOFactory.CreateInputLaserPort(rtc);
                dOutExt2 = IOFactory.CreateOutputExtension2(rtc);
                dOutLaserPort = IOFactory.CreateOutputLaserPort(rtc);
                success &= dInLaserPort.Initialize();
                success &= dOutExt2.Initialize();
                success &= dOutLaserPort.Initialize();
            }

            // 2nd Head
            var rtc2ndHead = rtc as IRtc2ndHead;
            if (null != rtc2ndHead)
            {
                // Primary head base offset
                float dx1 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "PRIMARY_BASE_OFFSET_X");
                float dy1 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "PRIMARY_BASE_OFFSET_Y");
                float angle1 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "PRIMARY_BASE_OFFSET_ANGLE");
                rtc2ndHead.PrimaryHeadBaseOffset = new SpiralLab.Sirius2.Mathematics.Offset(dx1, dy1, 0, angle1);

                int enable2ndHead = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", $"SECONDARY_HEAD_ENABLE");
                if (0 != enable2ndHead)
                {
                    var secondaryCorrectionFileName = NativeMethods.ReadIni<string>(ConfigFileName, $"RTC{index}", "SECONDARY_CORRECTION");
                    var secondaryCorrectionFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "correction", secondaryCorrectionFileName);
                    success &= rtc.CtlLoadCorrectionFile(CorrectionTables.Table2, secondaryCorrectionFullPath);
                    success &= rtc.CtlSelectCorrection(rtc.PrimaryHeadTable, CorrectionTables.Table2);
                    
                    float distX = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "PRIMARY_TO_SECONDARY_DISTANCE_X");
                    float distY = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "PRIMARY_TO_SECONDARY_DISTANCE_Y");
                    
                    // Distance from primary to secondary head
                    rtc2ndHead.DistanceToSecondaryHead = new System.Numerics.Vector2(distX, distY);

                    // Secondary head base offset
                    float dx2 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "SECONDARY_BASE_OFFSET_X");
                    float dy2 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "SECONDARY_BASE_OFFSET_Y");
                    float angle2 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "SECONDARY_BASE_OFFSET_ANGLE");
                    rtc2ndHead.SecondaryHeadBaseOffset = new SpiralLab.Sirius2.Mathematics.Offset(dx2, dy2, 0, angle2);
                }
            }

            // 3D
            var rtc3D = rtc as IRtc3D;
            if (null != rtc3D)
            {
                var kzScaleStr = NativeMethods.ReadIni<string>(ConfigFileName, $"RTC{index}", "KZ_SCALE");
                var scaleTokens = kzScaleStr.Split(',');
                Debug.Assert(2 == scaleTokens.Length);
                rtc3D.KZScale = new System.Numerics.Vector2(float.Parse(scaleTokens[0]), float.Parse(scaleTokens[1]));
            }

            // MoF 
            var rtcMoF = rtc as IRtcMoF;
            if (null != rtcMoF)
            {
                rtcMoF.EncXCountsPerMm = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_X_ENC_COUNTS_PER_MM", 0);
                rtcMoF.EncYCountsPerMm = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_X_ENC_COUNTS_PER_MM", 0);
                rtcMoF.EncCountsPerRevolution = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_ANGULAR_ENC_COUNTS_PER_REVOLUTION", 0);
                var trackingError = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_TRACKING_ERROR", 0);
                rtcMoF.CtlMofTrackingError(trackingError, trackingError);
            }

            // Default frequency and pulse width: 50KHz, 2 usec 
            success &= rtc.CtlFrequency(50 * 1000, 2);
            // Default jump and mark speed: 500 mm/s
            success &= rtc.CtlSpeed(500, 500);
            #endregion

            #region Initialize Laser source
            var laserType = NativeMethods.ReadIni(ConfigFileName, $"LASER{index}", "TYPE", "Virtual");
            var laserMaxPower = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "MAXPOWER", 10);
            var laserCOMPort = NativeMethods.ReadIni<int>(ConfigFileName, $"LASER{index}", "COM_PORT", 1);
            var laserIPaddress = NativeMethods.ReadIni<string>(ConfigFileName, $"LASER{index}", "IP_ADDRESS", string.Empty);
            var rtcAnalogPort = NativeMethods.ReadIni<int>(ConfigFileName, $"LASER{index}", "ANALOG_PORT", 1);
            var virtuaLaserPowerControl = NativeMethods.ReadIni(ConfigFileName, $"LASER{index}", "POWERCONTROL", "Unknown");
            switch (laserType.Trim().ToLower())
            {
                default:
                case "virtual":
                    switch (virtuaLaserPowerControl.Trim().ToLower())
                    {
                        default:
                        case "unknown":
                            laser = LaserFactory.CreateVirtual(index, laserMaxPower);
                            break;
                        case "analog1":
                            {
                                var voltageMin = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MIN", 0);
                                var voltageMax = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MAX", 10);
                                laser = LaserFactory.CreateVirtualAnalog(index, laserMaxPower, 1, voltageMin, voltageMax);
                            }
                            break;
                        case "analog2":
                            {
                                var voltageMin = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MIN", 0);
                                var voltageMax = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MAX", 10);
                                laser = LaserFactory.CreateVirtualAnalog(index, laserMaxPower, 2, voltageMin, voltageMax);
                            }
                            break;
                        case "frequency":
                            var freqMin = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_FREQUENCY_MIN", 0);
                            var freqMax = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_FREQUENCY_MIN", 50000);
                            laser = LaserFactory.CreateVirtualFrequency(index, laserMaxPower, freqMin, freqMax);
                            break;
                        case "dutycycle":
                            var dutyCycleMin = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DUTYCYCLE_MIN", 0);
                            var dutyCycleMax = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DUTYCYCLE_MAX", 99);
                            laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, dutyCycleMin, dutyCycleMax);
                            break;
                        case "digitalbits16":
                            var dOut16Min = NativeMethods.ReadIni<ushort>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DO16_MIN", 0);
                            var dOut16Max = NativeMethods.ReadIni<ushort>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DO16_MAX", 65535);
                            laser = LaserFactory.CreateVirtualDO16Bits(index, laserMaxPower, dOut16Min, dOut16Max);
                            break;
                        case "digitalbits8":
                            var dOut8Min = NativeMethods.ReadIni<ushort>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DO8_MIN", 0);
                            var dOut8Max = NativeMethods.ReadIni<ushort>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DO8_MAX", 255);
                            laser = LaserFactory.CreateVirtualDO8Bits(index, laserMaxPower, dOut8Min, dOut8Max);
                            break;
                    }
                    break;
                case "advancedoptowaveaopico":
                    laser = LaserFactory.CreateAdvancedOptoWaveAOPico(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "advancedoptowaveaopicoprecision":
                    laser = LaserFactory.CreateAdvancedOptoWaveAOPicoPrecision(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "advancedoptowavefotia":
                    laser = LaserFactory.CreateAdvancedOptoWaveFotia(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "coherentavialx":
                    laser = LaserFactory.CreateCoherentAviaLX(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "coherentdiamondcseries":
                    laser = LaserFactory.CreateCoherentDiamondCSeries(index, $"LASER{index}", laserMaxPower);
                    break;
                case "ipgylptyped":
                    laser = LaserFactory.CreateIPGYLPTypeD(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "ipgylptypee":
                    laser = LaserFactory.CreateIPGYLPTypeE(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "ipgylpulpn":
                    laser = LaserFactory.CreateIPGYLPULPN(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "ipgylpn":
                    laser = LaserFactory.CreateIPGYLPN(index, $"LASER{index}", laserCOMPort, laserMaxPower, rtcAnalogPort);
                    break;
                case "jpttypee":
                    laser = LaserFactory.CreateJPTTypeE(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "photonicsindustrydx":
                    laser = LaserFactory.CreatePhotonicsIndustryDX(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "photonicsindustryrghaio":
                    laser = LaserFactory.CreatePhotonicsIndustryRGHAIO(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "spectraphysicshippo":
                    laser = LaserFactory.CreateSpectraPhysicsHippo(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "spectraphysicstalon":
                    laser = LaserFactory.CreateSpectraPhysicsTalon(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "spig4":
                    laser = LaserFactory.CreateSPIG4(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
            }
           
            var laserPowerControlDelay = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DELAY", 0);
            laser.PowerControlDelayTime = laserPowerControlDelay;
            
            // Initialize PowerMap 
            var powerControl = laser as ILaserPowerControl;
            var enablePowerMap = NativeMethods.ReadIni<int>(ConfigFileName, $"LASER{index}", "POWERMAP_ENABLE", 0);
            if (0 != enablePowerMap)
            {
                var powerMap = PowerMapFactory.CreateDefault(index, $"MAP{index}");
                //powerMap.OnOpened += PowerMap_OnMappingOpened;
                //powerMap.OnSaved += PowerMap_OnMappingSaved;
                var powerMapFile = NativeMethods.ReadIni<string>(ConfigFileName, $"LASER{index}", "POWERMAP_FILE", string.Empty);
                var powerMapFullPath = Path.Combine(SpiralLab.Sirius2.Config.PowerMapPath, powerMapFile);
                if (File.Exists(powerMapFullPath))
                    success &= PowerMapSerializer.Open(powerMapFullPath, powerMap);
                if (null != powerControl)
                {
                    // Enable lookup powermap table 
                    powerMap.IsEnableLookUp = true;
                    powerControl.PowerMap = powerMap;
                }
            }
            // Assign RTC into laser source
            laser.Scanner = rtc;
            // Initialize laser source
            success &= laser.Initialize();

            // Set Default Power
            var laserDefaultPower = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "DEFAULT_POWER", 1);
            if (null != powerControl)
                success &= powerControl.CtlPower(laserDefaultPower);
            #endregion

            #region Initialize camera and inspector
            success &= CreateDevices(out camera, out inspector, rtc);
            #endregion

            return success;
        }
        /// <summary>
        ///  Dispose resources (like as <c>ICamera</c>, <c>IInspector</c>)
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="inspector"></param>
        public static void DestroyDevices(ICamera camera, IInspector inspector)
        {
            inspector?.Dispose();
            camera?.Dispose();
        }
        /// <summary>
        /// Dispose resources (like as <c>ICamera</c>, <c>IInspector</c>, <c>IRtc</c>, <c>ILaser</c>, )
        /// </summary>
        /// <param name="camera"><c>ICamera</c></param>
        /// <param name="inspector"><c>IInspector</c></param>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="laser"><c>ILaser</c></param>
        public static void DestroyDevices(ICamera camera, IInspector inspector, IRtc rtc, ILaser laser)
        {
            DestroyDevices(camera, inspector);
            laser?.Dispose();
            rtc?.Dispose();
        }

        public static void Camera_OnCalibratorSaved(ICamera camera, CenterCalibrator calibrator, string fileName)
        {
            int index = camera.Index;
            // File path should be in "bin\correction\vision"
            var fileNameOnly = Path.GetFileName(fileName);
            NativeMethods.WriteIni<string>(ConfigFileName, $"CAMERA{index}", "CALIBRATION", fileNameOnly);
        }
        public static void CameraStitched_OnStitchedCalibratorSaved(ICameraStitched cameraStitched, StitchCalibrator calibrator, string fileName)
        {
            var camera = cameraStitched as ICamera;
            int index = camera.Index;
            // File path should be in "bin\correction"
            var fileNameOnly = Path.GetFileName(fileName);
            NativeMethods.WriteIni<string>(ConfigFileName, $"CAMERA{index}", "CALIBRATION_STITCHED", fileNameOnly);
        }
        public static void CreateTestProcesses(SpiralLab.Sirius2.Vision.IDocument doc)
        {
            doc.ActNew();

            var proc1 = ProcessFactory.CreateCircle();
            doc.ActAdd(proc1);
            var proc2 = ProcessFactory.CreateLine();
            doc.ActAdd(proc2);
            var proc3 = ProcessFactory.CreatePattern();
            doc.ActAdd(proc3);
            var proc4 = ProcessFactory.CreateBlob();
            doc.ActAdd(proc4);
            var proc5 = ProcessFactory.CreatePattern();
            proc5.Children.Add(ProcessFactory.CreateCircle());
            doc.ActAdd(proc5);
           
            var layer = ProcessFactory.CreateLayer("Stitched Image");
            layer.Children.Add(ProcessFactory.CreateCircle());
            layer.Children.Add(ProcessFactory.CreateLine());
            layer.Children.Add(ProcessFactory.CreatePattern());
            layer.Children.Add(ProcessFactory.CreateBlob());
            var procPattern = ProcessFactory.CreatePattern();
            procPattern.Children.Add(ProcessFactory.CreateCircle());
            layer.Children.Add(procPattern);
            doc.ActAdd(layer);
        }
     
        private static bool Camera_OnProcessLightIntensity(ICamera camera, IProcess process, object userState)
        {
            // Target process is not selected.
            // No need to control intensity .
            if (null == process)
                return true; ;

            //set intensity value into your light controller
            var intensity = process.LightIntensity;
            // ...

            return true;
        }
    }
}
