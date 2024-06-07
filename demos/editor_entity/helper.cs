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
 * Description : editor helper
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
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.IO;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.PowerMap;
using SpiralLab.Sirius2.PowerMeter;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Scanner.Rtc.SyncAxis;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.Remote;
using SpiralLab.Sirius2.Winforms.UI;

namespace Demos
{
    /// <summary>
    /// Demo editor helper
    /// </summary>
    public static class EditorHelper
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
        /// Create devices (like as <c>IRtc</c>, <c>ILaser</c>, ...)
        /// </summary>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="dInExt1">RTC D.Input EXTENSION1 port</param>
        /// <param name="dInLaserPort">RTC D.Input LASER port</param>
        /// <param name="dOutExt1">RTC D.Output EXTENSION1 port</param>
        /// <param name="dOutExt2">RTC D.Output EXTENSION2 port</param>
        /// <param name="dOutLaserPort">RTC D.Output LASER port</param>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        /// <param name="marker"><c>IMarker</c></param>
        /// <param name="remote"><c>IRemote</c></param>
        /// <param name="editorUserControl">Target <c>SiriusEditorUserControl</c></param>
        /// <param name="index">Index (assign value if using multiple devices) (0,1,2,...)</param>
        /// <returns>Success or failed</returns>
        public static bool CreateDevices(out IRtc rtc, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out ILaser laser, out IPowerMeter powerMeter, out IMarker marker, out IRemote remote, SiriusEditorUserControl editorUserControl, int index = 0)
        {
            rtc = null;
            dInExt1 = null;
            dInLaserPort = null;
            dOutExt1 = null;
            dOutExt2 = null;
            dOutLaserPort = null;
            laser = null;
            powerMeter = null;
            marker = null;
            remote = null;

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
            var correctionFile = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CORRECTION", "cor_1to1.ct5");
            var correctionPath = Path.Combine(SpiralLab.Sirius2.Config.CorrectionPath, correctionFile);
            var signalLevelLaser12 = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SIGNALLEVEL_LASER12", "High") == "High" ? RtcSignalLevels.ActiveHigh : RtcSignalLevels.ActiveLow;
            var signalLevelLaserOn = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SIGNALLEVEL_LASERON", "High") == "High" ? RtcSignalLevels.ActiveHigh : RtcSignalLevels.ActiveLow;
            var rtcType = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "TYPE", "Rtc5");
            var sLaserMode = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "LASERMODE", "Yag1");
            var laserMode = (LaserModes)Enum.Parse(typeof(LaserModes), sLaserMode);
            switch (rtcType.Trim().ToLower())
            {
                default:
                case "virtual":
                    rtc = ScannerFactory.CreateVirtual(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc4":
                    rtc = ScannerFactory.CreateRtc4(index, kfactor, laserMode, correctionPath);
                    break;
                case "rtc5":
                    rtc = ScannerFactory.CreateRtc5(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc6":
                    rtc = ScannerFactory.CreateRtc6(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc6e":
                    var ipAddress = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "IP_ADDRESS", "192.168.0.100");
                    var subnetMask = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SUBNET_MASK", "255.255.255.0");
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

            // Set FOV area: WxH, it will be drawn as red square
            SpiralLab.Sirius2.Winforms.Config.ViewFovSize = new SizeF(fov, fov);

            // Set virtual image field area
            if (rtc.IsMoF)
            {
                if (rtc is Rtc4 rtc4)
                {
                    //no virtual image field 
                    SpiralLab.Sirius2.Winforms.Config.ViewVirtualImageSize = SizeF.Empty;
                }
                else if (rtc is Rtc5 rtc5)
                {
                    //2^24 bits 
                    SpiralLab.Sirius2.Winforms.Config.ViewVirtualImageSize = new SizeF(fov * (float)Math.Pow(2, 4), fov * (float)Math.Pow(2, 4));
                }
                else if (rtc is Rtc6 rtc6)
                {
                    //2^29 bits 
                    SpiralLab.Sirius2.Winforms.Config.ViewVirtualImageSize = new SizeF(fov * (float)Math.Pow(2, 9), fov * (float)Math.Pow(2, 9));
                }
            }

            // To check out of range for jump and mark x,y locations
            //rtc.FieldSizeLimit = new SizeF(fov, fov);

            // 2nd Head
            var rtc2ndHead = rtc as IRtc2ndHead;
            int enable2ndHead = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", $"SECONDARY_HEAD_ENABLE");
            if (0 != enable2ndHead && null != rtc2ndHead)
            { 
                var secondaryCorrectionFileName = NativeMethods.ReadIni<string>(ConfigFileName, $"RTC{index}", "SECONDARY_CORRECTION");
                var secondaryCorrectionFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "correction", secondaryCorrectionFileName);
                success &= rtc.CtlLoadCorrectionFile(CorrectionTables.Table2, secondaryCorrectionFullPath);
                success &= rtc.CtlSelectCorrection(rtc.PrimaryHeadTable, CorrectionTables.Table2);

                float distX = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "PRIMARY_TO_SECONDARY_DISTANCE_X");
                float distY = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "PRIMARY_TO_SECONDARY_DISTANCE_Y");
                // Distance from primary to secondary head
                rtc2ndHead.DistanceToSecondaryHead = new System.Numerics.Vector2(distX, distY);

                // Primary head base offset
                float dx1 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "PRIMARY_BASE_OFFSET_X");
                float dy1 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "PRIMARY_BASE_OFFSET_Y");
                float angle1 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "PRIMARY_BASE_OFFSET_ANGLE");
                rtc2ndHead.PrimaryHeadBaseOffset = new SpiralLab.Sirius2.Mathematics.Offset(dx1, dy1, 0, angle1);

                // Secondary head base offset
                float dx2 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "SECONDARY_BASE_OFFSET_X");
                float dy2 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "SECONDARY_BASE_OFFSET_Y");
                float angle2 = NativeMethods.ReadIni<float>(ConfigFileName, $"RTC{index}", "SECONDARY_BASE_OFFSET_ANGLE");
                rtc2ndHead.SecondaryHeadBaseOffset = new SpiralLab.Sirius2.Mathematics.Offset(dx2, dy2, 0, angle2);
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

            #region Initialize Powermeter
            var enablePowerMeter = NativeMethods.ReadIni<int>(ConfigFileName, $"POWERMETER{index}", "ENABLE", 0);
            if (0 != enablePowerMeter)
            {
                var powerMeterType = NativeMethods.ReadIni(ConfigFileName, $"POWERMETER{index}", "TYPE", "Virtual");
                var powerMeterSerialNo = NativeMethods.ReadIni(ConfigFileName, $"POWERMETER{index}", "SERIAL_NO", string.Empty);
                var powerMeterSerialPort = NativeMethods.ReadIni<int>(ConfigFileName, $"POWERMETER{index}", "SERIAL_PORT", 0);
                switch (powerMeterType.Trim().ToLower())
                {
                    default:
                    case "virtual":
                        var laserVirtualMaxPower = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "MAXPOWER", 10);
                        powerMeter = PowerMeterFactory.CreateVirtual(index, laserVirtualMaxPower);
                        break;
                    case "ophirphotonics":
                        powerMeter = PowerMeterFactory.CreateOphirPhotonics(index, powerMeterSerialNo);
                        break;
                    case "coherentpowermax":
                        powerMeter = PowerMeterFactory.CreateCoherentPowerMax(index, powerMeterSerialPort);
                        break;
                    case "thorlabs":
                        powerMeter = PowerMeterFactory.CreateThorlabs(index, powerMeterSerialNo);
                        break;
                }
                success &= powerMeter.Initialize();
                // uncomment to auto start 
                //success &= powerMeter.CtlStart();
            }
            #endregion

            #region Initialize Laser source
            var laserType = NativeMethods.ReadIni(ConfigFileName, $"LASER{index}", "TYPE", "Virtual");
            var laserMaxPower = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "MAXPOWER", 10);
            var laserComPort = NativeMethods.ReadIni<int>(ConfigFileName, $"LASER{index}", "COM_PORT", 1);
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
                    laser = LaserFactory.CreateAdvancedOptoWaveAOPico(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "advancedoptowaveaopicoprecision":
                    laser = LaserFactory.CreateAdvancedOptoWaveAOPicoPrecision(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "coherentavialx":
                    laser = LaserFactory.CreateCoherentAviaLX(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "coherentdiamondcseries":
                    laser = LaserFactory.CreateCoherentDiamondCSeries(index, $"LASER{index}", laserMaxPower);
                    break;
                case "ipgylptyped":
                    laser = LaserFactory.CreateIPGYLPTypeD(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "ipgylptypee":
                    laser = LaserFactory.CreateIPGYLPTypeE(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "ipgylpulpn":
                    laser = LaserFactory.CreateIPGYLPULPN(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "ipgylpn":
                    laser = LaserFactory.CreateIPGYLPN(index, $"LASER{index}", laserComPort, laserMaxPower, rtcAnalogPort);
                    break;
                case "jpttypee":
                    laser = LaserFactory.CreateJPTTypeE(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "photonicsindustrydx":
                    laser = LaserFactory.CreatePhotonicsIndustryDX(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "photonicsindustryrghaio":
                    laser = LaserFactory.CreatePhotonicsIndustryRGHAIO(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "spectraphysicshippo":
                    laser = LaserFactory.CreateSpectraPhysicsHippo(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "spectraphysicstalon":
                    laser = LaserFactory.CreateSpectraPhysicsTalon(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
                case "spig4":
                    laser = LaserFactory.CreateSPIG4(index, $"LASER{index}", laserComPort, laserMaxPower);
                    break;
            }
            if (powerMeter != null)
            {
                if (powerMeter is PowerMeterVirtual powerMeterVirtual)
                {
                    powerMeterVirtual.Laser = laser;
                }
            }
            var laserPowerControlDelay = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DELAY", 0);
            laser.PowerControlDelayTime = laserPowerControlDelay;
            
            // Initialize PowerMap 
            var powerControl = laser as ILaserPowerControl;
            var enablePowerMap = NativeMethods.ReadIni<int>(ConfigFileName, $"LASER{index}", "POWERMAP_ENABLE", 0);
            if (0 != enablePowerMap)
            {
                var powerMap = PowerMapFactory.CreateDefault(index, $"MAP{index}");
                powerMap.OnOpened += PowerMap_OnMappingOpened;
                powerMap.OnSaved += PowerMap_OnMappingSaved;
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

            #region Marker
            switch (rtcType.Trim().ToLower())
            {
                default:
                case "virtual":
                    marker = MarkerFactory.CreateVirtual(index);
                    break;
                case "rtc4":
                case "rtc5":
                case "rtc6":
                case "rtc6e":
                    marker = MarkerFactory.CreateRtc(index);
                    //marker = MarkerFactory.CreateRtcFast(index);
                    break;
                case "syncaxis":
                    marker = MarkerFactory.CreateSyncAxis(index);
                    break;
            }
            var scriptFileName = NativeMethods.ReadIni(ConfigFileName, $"MARKER{index}", "SCRIPT_FILENAME", string.Empty);
            if (!string.IsNullOrEmpty(scriptFileName))
                marker.ScriptFile = Path.Combine(SpiralLab.Sirius2.Winforms.Config.ScriptPath, scriptFileName);
            #endregion

            #region Remote
            var enableRemote = NativeMethods.ReadIni<int>(ConfigFileName, $"REMOTE{index}", "ENABLE", 0);
            if (0 != enableRemote)
            {
                string protocol = NativeMethods.ReadIni<string>(ConfigFileName, $"REMOTE{index}", $"PROTOCOL", "tcpip");
                switch (protocol.ToLower().Trim())
                {
                    default:
                    case "virtual":
                        remote = RemoteFactory.CreateVirtual(index, "Virtual", marker);
                        break;
                    case "tcp":
                    case "tcpip":
                        int tcpPort = NativeMethods.ReadIni<int>(ConfigFileName, $"REMOTE{index}", $"TCP_PORT", 5001);
                        remote = RemoteFactory.CreateTcpServer(index, "TCP/IP", marker, tcpPort);
                        break;
                    case "rs232":
                    case "rs232c":
                    case "serial":
                        int serialPort = NativeMethods.ReadIni<int>(ConfigFileName, $"REMOTE{index}", $"SERIAL_PORT", 1);
                        int serialBaudRate = NativeMethods.ReadIni<int>(ConfigFileName, $"REMOTE{index}", $"SERIAL_BAUDRATE=", 57600);
                        remote = RemoteFactory.CreateSerial(index, "RS232C", marker, serialPort, serialBaudRate);
                        break;
                }
                remote.EditorControl = editorUserControl;
                success &= remote.Start();
            }
            #endregion
            
            return success;
        }
        
        private static void PowerMap_OnMappingOpened(IPowerMap powerMap, string fileName)
        {
            //var index = powerMap.Index;
            //var name = Path.GetFileName(fileName);
            //NativeMethods.WriteIni<string>(ConfigFileName, $"LASER{index}", "POWERMAP_FILE", name);

        }
        private static void PowerMap_OnMappingSaved(IPowerMap powerMap, string fileName)
        {
            var index = powerMap.Index;
            // File path should be in "bin\powermap"
            var fileNameOnly = Path.GetFileName(fileName);
            NativeMethods.WriteIni<string>(ConfigFileName, $"LASER{index}", "POWERMAP_FILE", fileNameOnly);
        }

        /// <summary>
        /// Create test entities at <c>IDocument</c>
        /// </summary>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="view"><c>IView</c></param>
        /// <param name="document"><c>IDocument</c></param>
        /// <returns>Success or failed</returns>
        public static bool CreateTestEntities(IRtc rtc, IView view, IDocument document)
        {
            Debug.Assert(null != document.ActiveLayer);
            bool success = true;

            // Line entity
            var line1 = EntityFactory.CreateLine(new Vector2(-40, 40), new Vector2(40, 40));
            success &= document.ActAdd(line1);
            var line2 = EntityFactory.CreateLine(new Vector2(40, 40), new Vector2(40, -40));
            success &= document.ActAdd(line2);

            // Point1 entity
            var p1 = EntityFactory.CreatePoint(new Vector2(-3, 1), 1);
            success &= document.ActAdd(p1);

            // Points2 entity
            var locations1 = new List<Vector2>();
            locations1.Add(new Vector2(-2, 2));
            locations1.Add(new Vector2(-3, 2));
            locations1.Add(new Vector2(-4, 2));
            locations1.Add(new Vector2(-2, 3));
            locations1.Add(new Vector2(-3, 3));
            locations1.Add(new Vector2(-4, 3));
            locations1.Add(new Vector2(-2, 4));
            locations1.Add(new Vector2(-3, 4));
            locations1.Add(new Vector2(-4, 4));
            var p2 = EntityFactory.CreatePoints(locations1.ToArray(), 1);
            success &= document.ActAdd(p2);

            // Points3 entity
            var locations2 = new List<Vector2>();
            var rnd = new Random();
            for (int i = 0; i < 1000; i++)
                locations2.Add(new Vector2((float)(rnd.NextDouble() * -5.0 - 1), (float)(rnd.NextDouble() * -5.0 - 1)));
            var p3 = EntityFactory.CreatePoints(locations2.ToArray(), 1);
            success &= document.ActAdd(p3);

            // Arc entity
            var arc1 = EntityFactory.CreateArc(Vector2.Zero, 10, 0, 360);
            arc1.Translate(20, 20);
            success &= document.ActAdd(arc1);
            var arc2 = EntityFactory.CreateArc(Vector2.Zero, 8, 90, -300);
            arc2.Translate(20, 20);
            success &= document.ActAdd(arc2);
            var arc3 = EntityFactory.CreateArc(Vector2.Zero, 6, 45, -180);
            arc3.Translate(20, 20);
            success &= document.ActAdd(arc3);

            // Rectangle entity
            var rectangle1 = EntityFactory.CreateRectangle(new Vector2(10, -10), 10, 5);
            success &= document.ActAdd(rectangle1);
            // Hatch with polygon within rectangle
            var rectanglehatch = rectangle1.Hatch(HatchModes.Polygon, HatchJoints.Miter, false, 0, 0, 0.2f, 0, 0);
            foreach (var hatch in rectanglehatch.Children)
                hatch.Color = SpiralLab.Sirius2.Winforms.Config.PenColors[1];
            success &= document.ActAdd(rectanglehatch);

            // Spiral entity
            var spiral1 = EntityFactory.CreateSpiral(Vector2.Zero, 8, 12, 5, 20, false);
            spiral1.Scale(0.9);
            spiral1.Rotate(-50, 0, 0);
            spiral1.Translate(-35, 0);
            success &= document.ActAdd(spiral1);

            // Image entity
            var filename1 = Path.Combine("sample", "lena.bmp");
            var image1 = EntityFactory.CreateImage(filename1, 10, 10);
            image1.RasterMode = RasterModes.JumpAndShoot;
            image1.Translate(-30, -15);
            success &= document.ActAdd(image1);

            // Image entity 
            var filename2 = Path.Combine("sample", "checkerboard.bmp");
            var image2 = EntityFactory.CreateImage(filename2, 10);
            image2.RasterMode = RasterModes.JumpAndShoot;
            image2.Translate(-30, 25);
            success &= document.ActAdd(image2);

            // ImageText entity
            var imagetext1 = EntityFactory.CreateImageText("Arial", $"12345 67890{Environment.NewLine}ABCDEFGHIJKLMNOPQRSTUVWXYZ{Environment.NewLine}`~!@#$%^&*()-_=+[{{]|}}\\|;:'\",<.>/?{Environment.NewLine}abcdefghijklmnopqrstuvwxyz", FontStyle.Regular, false, 3, 64, 10);
            imagetext1.Name = "MyText1";
            imagetext1.RasterMode = RasterModes.JumpAndShoot;
            imagetext1.Translate(-30, -30);
            success &= document.ActAdd(imagetext1);

            // Raster entity 
            var raster1 = EntityFactory.CreateRaster(filename1, 5);
            raster1.Name = "Raster1";
            raster1.RasterMode = RasterModes.MicroVector;
            raster1.PixelPeriod = 100;
            raster1.PixelTime = 100;
            raster1.Direction = RasterDirections.LeftToRight;
            raster1.Translate(-36, -10);
            success &= document.ActAdd(raster1);

            // Text entity
            var text1 = EntityFactory.CreateText("Arial", $"12345 67890{Environment.NewLine}ABCDEFGHIJKLMNOPQRSTUVWXYZ{Environment.NewLine}`~!@#$%^&*()-_=+[{{]|}}\\|;:'\",<.>/?{Environment.NewLine}abcdefghijklmnopqrstuvwxyz", FontStyle.Bold, 2.5);
            text1.Name = "MyText1";
            success &= document.ActAdd(text1);

            // Text circular entity
            var textCircular1 = EntityFactory.CreateCircularText("Times New Roman", $"ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789", FontStyle.Regular, 1.5, TextCircularDirections.ClockWise, 6, 90);
            textCircular1.Name = "MyTextCircular1";
            textCircular1.Translate(25, -23);
            success &= document.ActAdd(textCircular1);

            // Text circular entity
            var textCircular2 = EntityFactory.CreateCircularText("Century Gothic", $"ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789", FontStyle.Regular, 2, TextCircularDirections.CounterClockWise, 8, -180);
            textCircular2.Name = "MyTextCircular2";
            textCircular2.Translate(25, -23);
            success &= document.ActAdd(textCircular2);

            // Sirius text entity
            var text2 = EntityFactory.CreateSiriusText("romans2.cxf", $"12345 67890{Environment.NewLine}ABCDEFGHIJKLMNOPQRSTUVWXYZ{Environment.NewLine}`~!@#$%^&*()-_=+[{{]|}}\\|;:'\",<.>/?{Environment.NewLine}abcdefghijklmnopqrstuvwxyz", 2);
            text2.Name = "MyText3";
            text2.Translate(18, -12);
            success &= document.ActAdd(text2);

            // Registerable text characterset
            switch (rtc.RtcType)
            {
                case RtcTypes.Rtc6SyncAxis:
                    //syncaxis is not support IRtcCharacterSet
                    break;
                default:
                    // Text
                    var textCharacterSet1 = EntityFactory.CreateCharacterSetText("Arial", CharacterSetFormats.Time, 2);
                    textCharacterSet1.Name = "MyTextCharacterset";
                    textCharacterSet1.CharacterSetTime.TimeFormat = TimeFormats.Hours24;
                    textCharacterSet1.CharacterSetTime.IsLeadingWithZero = true;
                    textCharacterSet1.Translate(24, -22);
                    success &= document.ActAdd(textCharacterSet1);
                    // Sirius text
                    var textSiriusCharacterSet1 = EntityFactory.CreateSiriusCharacterSetText("romans2.cxf", CharacterSetFormats.SerialNo, 2);
                    textSiriusCharacterSet1.Name = "MySiriusTextCharacterset";
                    textSiriusCharacterSet1.CharacterSetSerialNo.NumOfDigits = 4;
                    textSiriusCharacterSet1.CharacterSetSerialNo.SerialFormat = SerialNoFormats.LeadingWithZero;
                    textSiriusCharacterSet1.Translate(24, -25);
                    success &= document.ActAdd(textSiriusCharacterSet1);
                    break;
            }

            // Polyline2D entity
            var vertices1 = new List<EntityVertex2D>();
            vertices1.Add(new EntityVertex2D(0, 0));
            vertices1.Add(new EntityVertex2D(-10, -10));
            vertices1.Add(new EntityVertex2D(-20, -25));
            vertices1.Add(new EntityVertex2D(-2, -30, 0.8));
            vertices1.Add(new EntityVertex2D(1, 1));
            var polyline1 = EntityFactory.CreatePolyline2D(vertices1.ToArray(), true);
            polyline1.Translate(-10, -10, 0);
            success &= document.ActAdd(polyline1);

            // Expanded Polyline2D 
            var expandedPolylines = polyline1.Expand(HatchJoints.Round, 0.5);
            foreach (var expandedPolyline in expandedPolylines)
            {
                expandedPolyline.Color = SpiralLab.Sirius2.Winforms.Config.PenColors[2];
                success &= document.ActAdd(expandedPolyline);
            }

            // Shrink Polyline2D
            var shrinkedPolylines = polyline1.Expand(HatchJoints.Miter, -2);
            foreach (var shrinkedPolyline in shrinkedPolylines)
            {
                shrinkedPolyline.Color = SpiralLab.Sirius2.Winforms.Config.PenColors[3];
                success &= document.ActAdd(shrinkedPolyline);
            }

            // Polyline2D entity
            var vertices2 = new List<EntityVertex2D>();
            vertices2.Add(new EntityVertex2D(-5, -5));
            vertices2.Add(new EntityVertex2D(-5, 5, -1));
            vertices2.Add(new EntityVertex2D(5, 5));
            vertices2.Add(new EntityVertex2D(5, -5, -1));
            var polyline2 = EntityFactory.CreatePolyline2D(vertices2.ToArray(), true);
            polyline2.Translate(-10, 10, 0);
            success &= document.ActAdd(polyline2);

            // Group entity has include 2 arcs and rectangle entities
            var arc_g1 = EntityFactory.CreateArc(Vector2.Zero, 1, 0, 360);
            arc_g1.Color = SpiralLab.Sirius2.Winforms.Config.PenColors[1];
            var arc_g2 = EntityFactory.CreateArc(Vector2.Zero, 2, 0, 360);
            arc_g2.Color = SpiralLab.Sirius2.Winforms.Config.PenColors[3];
            arc_g2.Translate(3, 2, 0);
            var rec_g3 = EntityFactory.CreateRectangle(Vector2.Zero, 5, 4);
            var group1 = EntityFactory.CreateGroup("Arcs", new IEntity[] { arc_g1, arc_g2, rec_g3 });
            group1.Translate(10, -20);
            success &= document.ActAdd(group1);

            // Block item
            var vertices3 = new List<EntityVertex2D>();
            vertices3.Add(new EntityVertex2D(10, 10));
            vertices3.Add(new EntityVertex2D(10, 15));
            vertices3.Add(new EntityVertex2D(5, 20));
            vertices3.Add(new EntityVertex2D(4, 8));
            var polyline3 = EntityFactory.CreatePolyline2D(vertices3.ToArray());
            // Block entity
            var block = EntityFactory.CreateBlock("MyBlock", new IEntity[] { polyline3 });
            // Block entity has include polyline2D entity 
            success &= document.ActAdd(block);

            // BlockInsert entity
            var insert1 = EntityFactory.CreateBlockInsert(block.Name, Vector3.Zero);
            success &= document.ActAdd(insert1);

            // BlockInsert entity
            var insert2 = EntityFactory.CreateBlockInsert(block.Name, Vector3.Zero);
            insert2.Scale(1.5);
            insert2.RotateZ(30);
            insert2.Translate(-1, 2, 0);
            success &= document.ActAdd(insert2);

            // Curve(conic spline) entity
            var vertices4 = new List<Vector2>();
            vertices4.Add(new Vector2(10, -3));
            vertices4.Add(new Vector2(11, -4));
            vertices4.Add(new Vector2(14, -1));
            var conicSpline = EntityFactory.CreateCurve(vertices4.ToArray());
            success &= document.ActAdd(conicSpline);

            // Curve(cubic spline) entity
            var vertices5 = new List<Vector2>();
            vertices5.Add(new Vector2(10, -3));
            vertices5.Add(new Vector2(11, -4));
            vertices5.Add(new Vector2(14, -1));
            vertices5.Add(new Vector2(16, -2));
            var cubicSpline = EntityFactory.CreateCurve(vertices5.ToArray());
            cubicSpline.Translate(0, -2);
            success &= document.ActAdd(cubicSpline);

            // Create new layer
            var layer1 = EntityFactory.CreateLayer("1");
            success &= document.ActAdd(layer1);
            Debug.Assert(document.ActiveLayer == layer1);

            // STL entity
            if (EntityFactory.CreateStereoLithography(Path.Combine("sample", "Nefertiti_face.stl"), out var stl))
            {
                stl.Alignment = Alignments.MiddleCenter;
                stl.Scale(0.2);
                stl.Translate(30, 10);
                stl.RotateZ(-90);
                success &= document.ActAdd(stl);
            }

            // Dxf entity
            if (EntityFactory.CreateDxf(Path.Combine("sample", "BIKE.dxf"), out var dxf))
            {
                dxf.Alignment = Alignments.MiddleCenter;
                dxf.Scale(0.01);
                dxf.Translate(25, -40);
                success &= document.ActAdd(dxf);
            }

            // HPGL entity
            if (EntityFactory.CreateHpgl(Path.Combine("sample", "SimplexOpt.plt"), out var hpgl))
            {
                hpgl.Alignment = Alignments.MiddleCenter;
                hpgl.Scale(0.02);
                hpgl.Translate(-2, 37);
                success &= document.ActAdd(hpgl);
            }

            // HPGL entity
            if (EntityFactory.CreateHpgl(Path.Combine("sample", "columbiao.plt"), out var hpgl2))
            {
                hpgl2.Alignment = Alignments.MiddleCenter;
                hpgl2.Scale(0.01);
                hpgl2.Translate(-35, -20);
                success &= document.ActAdd(hpgl2);
            }

            // Datamatrix barcode cell by dots
            var dataMatrix1 = EntityFactory.CreateDataMatrix("0123456789", Barcode2DCells.Dots, 3, 4, 4);
            dataMatrix1.CellDot.RasterMode = RasterModes.JumpAndShoot;
            dataMatrix1.CellDot.PixelChannel = ExtensionChannels.ExtAO2;
            dataMatrix1.CellDot.PixelPeriod = 1000;
            dataMatrix1.CellDot.PixelPeriod = 100;
            dataMatrix1.CellDot.IsZigZag = false;
            dataMatrix1.Translate(-23, 2);
            success &= document.ActAdd(dataMatrix1);

            // Datamatrix barcode cell by lines
            var dataMatrix2 = EntityFactory.CreateDataMatrix("SIRIUS2", Barcode2DCells.Lines, 4, 4, 4);
            dataMatrix2.CellLine.IsZigZag = false;
            dataMatrix2.CellLine.Direction = LineDirections.Horizontal;
            dataMatrix2.Translate(-23, 7);
            success &= document.ActAdd(dataMatrix2);

            // Datamatrix barcode cell by circles
            var dataMatrix3 = EntityFactory.CreateDataMatrix("ABCDEFGHIJKLMNOPQRSTUVWXYZ", Barcode2DCells.Circles, 3, 4, 4);
            dataMatrix3.CellCircle.RadiusFactor = 0.9;
            dataMatrix3.CellCircle.IsZigZag = false;
            dataMatrix3.Translate(-28, 2);
            success &= document.ActAdd(dataMatrix3);

            // Datamatrix barcode cell by outline
            var dataMatrix4 = EntityFactory.CreateDataMatrix("abcdefghijklmnopqrstuvwxyz", Barcode2DCells.Outline, 2, 4, 4);
            dataMatrix4.Translate(-28, 7);
            success &= document.ActAdd(dataMatrix4);

            // QRcode barcode cell by hatch
            var qr1 = EntityFactory.CreateQRCode("0123456789", Barcode2DCells.Hatch, 3, 4, 4);
            qr1.CellHatch.HatchMode = HatchModes.CrossLine;
            qr1.CellHatch.HatchInterval = 0.05f;
            qr1.CellHatch.HatchAngle = 100;
            qr1.CellHatch.HatchAngle2 = 10;
            qr1.Translate(-23, 12);
            success &= document.ActAdd(qr1);

            // QRcode barcode cell by outline
            var qr2 = EntityFactory.CreateQRCode("abcdefghijklmnopqrstuvwxyz", Barcode2DCells.Outline, 3, 4, 4);
            qr2.Translate(-28, 12);
            success &= document.ActAdd(qr2);

            // PDF417 barcode cell by lines
            var pdf417 = EntityFactory.CreatePDF417("abcdefghijklmnopqrstuvwxyz", Barcode2DCells.Lines, 3, 4 * 3.75, 4);
            pdf417.CellLine.IsZigZag = false;
            pdf417.CellLine.Direction = LineDirections.Vertical;
            pdf417.Translate(-45, 12);
            success &= document.ActAdd(pdf417);

            // 1D barcode by Code128
            var bcd1 = EntityFactory.CreateBarcode("1234567890", Barcode1DFormats.Code128, 5, 5, 2);
            bcd1.Translate(-28, 19);
            success &= document.ActAdd(bcd1);

            // 1D barcode by CODABAR
            var bcd2 = EntityFactory.CreateBarcode("1234567890", Barcode1DFormats.CODABAR, 3, 5, 2);
            bcd2.Translate(-22, 19);
            success &= document.ActAdd(bcd2);

            return success;
        }
        /// <summary>
        /// Attach user event handlers
        /// </summary>
        public static void AttachEventHandlers()
        {
            // Event will be fired when select scanner field correction 2d at popup-menu
            SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection2DShow += Config_OnScannerFieldCorrection2DShow;

            // Event will be fired when apply scanner field correction
            SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection2DApply += Config_OnScannerFieldCorrection2DApply;

        }
        /// <summary>
        /// Dispose resources (like as <c>IRtc</c>, <c>ILaser</c>, <c>IMarker</c>,<c>IPowerMeter</c>, <c>IRemote</c> ...)
        /// </summary>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        /// <param name="powerMap"><c>IPowerMap</c></param>
        /// <param name="marker"><c>IMarker</c></param>
        /// <param name="remote"><c>IRemote</c></param>
        public static void DestroyDevices(IRtc rtc, ILaser laser, IPowerMeter powerMeter, IMarker marker, IRemote remote)
        {
            remote?.Dispose();
            marker?.Dispose();
            //powerMap?.Dispose();
            powerMeter?.Dispose();
            laser?.Dispose();
            rtc?.Dispose();
        }

        /// <summary>
        /// Popup scanner field correction 2d winforms
        /// </summary>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <returns><c>RtcCorrection2D</c></returns>
        private static RtcCorrection2D Config_OnScannerFieldCorrection2DShow(IRtc rtc)
        {
            // For example, 7x7 grids 
            int rows = 7;
            int cols = 7;
            float interval = 10.0f;
            var rtcCorrection2D = new RtcCorrection2D(rtc.KFactor, rows, cols, interval, interval, rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName, string.Empty);
            float left = -interval * (float)(int)(cols / 2);
            float top = interval * (float)(int)(rows / 2);
            var rand = new Random();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // x,y deviation by image inspection
                    var errorXy = new System.Numerics.Vector2(
                           rand.Next(20) / 1000.0f - 0.01f,
                           rand.Next(20) / 1000.0f - 0.01f);
                    rtcCorrection2D.AddRelative(row, col,
                        new System.Numerics.Vector2(left + col * interval, top - row * interval), errorXy);
                }
            }
            return rtcCorrection2D;
        }
        /// <summary>
        /// When press 'apply' button at scanner field correction 2d winforms
        /// </summary>
        /// <param name="form"><c>RtcCorrection2DForm</c></param>
        /// <returns>Success or failed</returns>
        private static bool Config_OnScannerFieldCorrection2DApply(RtcCorrection2DForm form)
        {
            var ctFullFileName = form.RtcCorrection.TargetCorrectionFile;
            Debug.Assert(File.Exists(ctFullFileName));
            bool success = true;
            var index = form.Rtc.Index;
            // Load and select new scanner field correction file
            var currentTable = form.Rtc.PrimaryHeadTable;
            success &= form.Rtc.CtlLoadCorrectionFile(currentTable, ctFullFileName);
            success &= form.Rtc.CtlSelectCorrection(currentTable);
            Debug.Assert(success);
            System.Windows.Forms.MessageBox.Show($"New correction file: {ctFullFileName} has applied", "Scanner Field Correction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            // Save changed .ct5 into config ini file
            var ctFileName = ctFullFileName.Replace(SpiralLab.Sirius2.Config.CorrectionPath + "\\", "");
            NativeMethods.WriteIni<string>(ConfigFileName, $"RTC{index}", "CORRECTION", ctFileName);
            return true;
        }

    }
}
