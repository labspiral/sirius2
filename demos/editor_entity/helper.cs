﻿/*
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.UI;

namespace Demos
{
    public static class EditorHelper
    {
        /// <summary>
        /// Your config ini file
        /// </summary>
        public static string ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
        // Or
        // Used this config file if using XL-SCAN (syncAXIS)
        //public static string ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config_syncaxis.ini");

        /// <summary>
        /// Initialize sirius2 library
        /// </summary>
        /// <returns></returns>
        public static bool Initialize()
        {
            // To turn off output console logs
            //SpiralLab.Sirius2.Config.IsLogToConsole = false;

            // Initialize sirius2 library
            return SpiralLab.Sirius2.Core.Initialize();
        }
        /// <summary>
        /// Create devices (like as <c>IRtc</c>, <c>ILaser</c>, ...)
        /// </summary>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="index">Index (assign value if using multiple devices)</param>
        /// <returns>Success or failed</returns>
        public static bool CreateDevices(out IRtc rtc, out ILaser laser, int index = 0)
        {
            rtc = null;
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
            var correctionFile = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CORRECTION", "cor_1to1.ct5");
            var correctionPath = Path.Combine(SpiralLab.Sirius2.Config.CorrectionPath, correctionFile);
            var signalLevel = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SIGNALLEVEL", "High") == "High" ? RtcSignalLevel.ActiveHigh : RtcSignalLevel.ActiveLow;
            var rtcType = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "TYPE", "Rtc5");
            var sLaserMode = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "LASERMODE", "Yag5");
            var laserMode = (LaserMode)Enum.Parse(typeof(LaserMode), sLaserMode);
            switch (rtcType.Trim().ToLower())
            {
                case "virtual":
                    rtc = ScannerFactory.CreateVirtual(index, kfactor, correctionFile);
                    break;
                case "rtc5":
                    rtc = ScannerFactory.CreateRtc5(index, kfactor, laserMode, signalLevel, signalLevel, correctionPath);
                    break;
                case "rtc6":
                    rtc = ScannerFactory.CreateRtc6(index, kfactor, laserMode, signalLevel, signalLevel, correctionPath);
                    break;
                case "rtc6e":
                    rtc = ScannerFactory.CreateRtc6Ethernet(index, "192.168.0.100", "255.255.255.0", kfactor, laserMode, signalLevel, signalLevel, correctionPath);
                    break;
                case "syncaxis":
                    string configXmlFileName = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CONFIG_XML", string.Empty);
                    rtc = ScannerFactory.CreateRtc6SyncAxis(index, configXmlFileName);
                    break;
            }

            // Initialize RTC controller
            success &= rtc.Initialize();
            Debug.Assert(success);

            // Set FOV area: WxH, it will be drawn as red rectangle
            SpiralLab.Sirius2.Winforms.Config.DocumentDefaultViewFovSize = new SizeF(fov, fov);
            // Set Virtual image field area
            if (rtc.IsMoF)
            {
                if (rtc is Rtc5 rtc5)
                {
                    //2^24
                    SpiralLab.Sirius2.Winforms.Config.DocumentDefaultViewVirtualImageSize = new SizeF(fov * (float)Math.Pow(2, 4), fov * (float)Math.Pow(2, 4));
                }
                else if (rtc is Rtc6 rtc6)
                {
                    //2^29
                    SpiralLab.Sirius2.Winforms.Config.DocumentDefaultViewVirtualImageSize = new SizeF(fov * (float)Math.Pow(2, 9), fov * (float)Math.Pow(2, 9));
                }
            }

            // MoF 
            var rtcMoF = rtc as IRtcMoF;
            if (null != rtcMoF)
            {
                rtcMoF.EncXCountsPerMm = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_X_ENC_COUNTS_PER_MM", 0);
                rtcMoF.EncYCountsPerMm = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_X_ENC_COUNTS_PER_MM", 0);
                rtcMoF.EncCountsPerRevolution = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_ANGULAR_ENC_COUNTS_PER_REVOLUTION", 0);
            }

            // Default frequency and pulse width: 50KHz, 2 usec 
            success &= rtc.CtlFrequency(50 * 1000, 2);
            // Default jump and mark speed: 50 mm/s
            success &= rtc.CtlSpeed(50, 50);
            #endregion

            #region Initialize Laser source
            var laserType = NativeMethods.ReadIni(ConfigFileName, $"LASER{index}", "TYPE", "Virtual");
            var laserPowerControl = NativeMethods.ReadIni(ConfigFileName, $"LASER{index}", "POWERCONTROL", "Unknown");
            var laserMaxPower = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "MAXPOWER", 10);
            var laserDefaultPower = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "DEFAULTPOWER", 1);
            switch (laserType.Trim().ToLower())
            {
                case "virtual":
                    switch (laserPowerControl.Trim().ToLower())
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
            }

            // Assign RTC into laser source
            laser.Scanner = rtc;

            // Initialize laser source
            success &= laser.Initialize();

            // Default power 
            if (laser is ILaserPowerControl powerControl)
            {
                var laserPowerControlDelay = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROLDELAY", 0);
                powerControl.PowerControlDelayTime = laserPowerControlDelay;
                success &= powerControl.CtlPower(laserDefaultPower);
            }
            return success;
            #endregion
        }
        /// <summary>
        /// Create marker
        /// </summary>
        /// <param name="marker"><c>IMarker</c></param>
        /// <param name="index">Index (assign value if using multiple devices)</param>
        /// <returns>Success or failed</returns>
        public static bool CreateMarker(out IMarker marker, int index = 0)
        {
            marker = null;
            bool success = true;
            var rtcType = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "TYPE", "Rtc5");
            switch (rtcType.Trim().ToLower())
            {
                case "virtual":
                    marker = MarkerFactory.CreateVirtual(index);
                    break;
                case "rtc5":
                    marker = MarkerFactory.CreateRtc5(index);
                    break;
                case "rtc6":
                case "rtc6e":
                    marker = MarkerFactory.CreateRtc6(index);
                    break;
                case "syncaxis":
                    marker = MarkerFactory.CreateSyncAxis(index);
                    break;
                default:
                    success &= false;
                    break;
            }
            return success;
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
                hatch.Color = SpiralLab.Sirius2.Winforms.Config.PensColor[1];
            success &= document.ActAdd(rectanglehatch);

            // Spiral entity
            var spiral1 = EntityFactory.CreateSpiral(Vector2.Zero, 8, 12, 5, 20, false);
            spiral1.Scale(0.9);
            spiral1.Rotate(-50, 0, 0);
            spiral1.Translate(-35, 0);
            success &= document.ActAdd(spiral1);

            switch (rtc.RtcType)
            {
                case RtcType.Rtc6SyncAxis:
                    break;
                default:
                    // Image entity
                    var filename1 = Path.Combine("sample", "lena.bmp");
                    var image1 = EntityFactory.CreateImage(filename1, 10, 10);
                    image1.Translate(-30, -15);
                    success &= document.ActAdd(image1);

                    // Image entity 
                    var filename2 = Path.Combine("sample", "checkerboard.bmp");
                    var image2 = EntityFactory.CreateImage(filename2, 10);
                    image2.Translate(-30, 25);
                    success &= document.ActAdd(image2);
                    break;
            }

            // ImageText entity
            var imagetext1 = EntityFactory.CreateImageText("Arial", $"12345 67890{Environment.NewLine}ABCDEFGHIJKLMNOPQRSTUVWXYZ{Environment.NewLine}`~!@#$%^&*()-_=+[{{]|}}\\|;:'\",<.>/?{Environment.NewLine}abcdefghijklmnopqrstuvwxyz", FontStyle.Regular, false, 3, 64, 10);
            imagetext1.Name = "MyText1";
            imagetext1.Translate(-30, -30);
            success &= document.ActAdd(imagetext1);

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
            switch (rtc.RtcType)
            {
                case RtcType.Rtc6SyncAxis:
                    break;
                default:
                    var text2 = EntityFactory.CreateSiriusText("romans2.cxf", $"12345 67890{Environment.NewLine}ABCDEFGHIJKLMNOPQRSTUVWXYZ{Environment.NewLine}`~!@#$%^&*()-_=+[{{]|}}\\|;:'\",<.>/?{Environment.NewLine}abcdefghijklmnopqrstuvwxyz", 2);
                    text2.Name = "MyText3";
                    text2.Translate(18, -12);
                    success &= document.ActAdd(text2);
                    break;
            }

            // Registerable text characterset
            switch (rtc.RtcType)
            {
                case RtcType.Rtc6SyncAxis:
                    break;
                default:
                    // Text
                    var textCharacterSet1 = EntityFactory.CreateCharacterSetText("Arial", CharacterSetFormats.Time, 2);
                    textCharacterSet1.Name = "MyTextCharacterset";
                    textCharacterSet1.CharacterSetTime.TimeFormat = TimeFormat.Hours24;
                    textCharacterSet1.CharacterSetTime.IsLeadingWithZero = true;
                    textCharacterSet1.Translate(24, -22);
                    success &= document.ActAdd(textCharacterSet1);
                    // Sirius text
                    var textSiriusCharacterSet1 = EntityFactory.CreateSiriusCharacterSetText("romans2.cxf", CharacterSetFormats.SerialNo, 2);
                    textSiriusCharacterSet1.Name = "MySiriusTextCharacterset";
                    textSiriusCharacterSet1.CharacterSetSerialNo.NumOfDigits = 4;
                    textSiriusCharacterSet1.CharacterSetSerialNo.SerialFormat = SerialNoFormat.LeadingWithZero;
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
                expandedPolyline.Color = SpiralLab.Sirius2.Winforms.Config.PensColor[2];
                success &= document.ActAdd(expandedPolyline);
            }

            // Shrink Polyline2D
            var shrinkedPolylines = polyline1.Expand(HatchJoints.Miter, -2);
            foreach (var shrinkedPolyline in shrinkedPolylines)
            {
                shrinkedPolyline.Color = SpiralLab.Sirius2.Winforms.Config.PensColor[3];
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
            arc_g1.Color = SpiralLab.Sirius2.Winforms.Config.PensColor[1];
            var arc_g2 = EntityFactory.CreateArc(Vector2.Zero, 2, 0, 360);
            arc_g2.Color = SpiralLab.Sirius2.Winforms.Config.PensColor[3];
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
            var insert1 = EntityFactory.CreateBlockInsert(block.Name);
            success &= document.ActAdd(insert1);

            // BlockInsert entity
            var insert2 = EntityFactory.CreateBlockInsert(block.Name);
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
            switch (rtc.RtcType)
            {
                case RtcType.Rtc6SyncAxis:
                    break;
                default:
                    var dataMatrix1 = EntityFactory.CreateDataMatrix("0123456789", BarcodeCells.Dots, 3, 4, 4);
                    dataMatrix1.Translate(-23, 2);
                    success &= document.ActAdd(dataMatrix1);
                    break;
            }

            // Datamatrix barcode cell by lines
            var dataMatrix2 = EntityFactory.CreateDataMatrix("SIRIUS2", BarcodeCells.Lines, 4, 4, 4);
            dataMatrix2.Translate(-23, 7);
            success &= document.ActAdd(dataMatrix2);

            // Datamatrix barcode cell by circles
            var dataMatrix3 = EntityFactory.CreateDataMatrix("ABCDEFGHIJKLMNOPQRSTUVWXYZ", BarcodeCells.Circles, 3, 4, 4);
            dataMatrix3.Translate(-28, 2);
            success &= document.ActAdd(dataMatrix3);

            // Datamatrix barcode cell by outline
            var dataMatrix4 = EntityFactory.CreateDataMatrix("abcdefghijklmnopqrstuvwxyz", BarcodeCells.Outline, 2, 4, 4);
            dataMatrix4.Translate(-28, 7);
            success &= document.ActAdd(dataMatrix4);

            // QRcode barcode cell by hatch
            var qr1 = EntityFactory.CreateQRCode("0123456789", BarcodeCells.Hatch, 3, 4, 4);
            qr1.CellHatch.HatchMode = HatchModes.CrossLine;
            qr1.CellHatch.HatchInterval = 0.05f;
            qr1.CellHatch.HatchAngle = 100;
            qr1.CellHatch.HatchAngle2 = 10;
            qr1.Translate(-23, 12);
            success &= document.ActAdd(qr1);

            // QRcode barcode cell by outline
            var qr2 = EntityFactory.CreateQRCode("abcdefghijklmnopqrstuvwxyz", BarcodeCells.Outline, 3, 4, 4);
            qr2.Translate(-28, 12);
            success &= document.ActAdd(qr2);

            return success;
        }
        /// <summary>
        /// Attach user event handlers
        /// </summary>
        public static void AttachEventHandlers()
        {
            // Event will be fired when trying to convert with ITextConvertible entity
            SpiralLab.Sirius2.Winforms.Config.OnTextConvert += Text_OnTextConvert;

            // Event will be fired when select scanner field correction 2d at popup-menu
            SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection2DShow += Config_OnScannerFieldCorrection2DShow;

            // Event will be fired when apply scanner field correction
            SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection2DApply += Config_OnScannerFieldCorrection2DApply;
        }
        /// <summary>
        /// Dispose resources (like as <c>IRtc</c>, <c>ILaser</c>, <c>IMarker</c>, ...)
        /// </summary>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="marker"><c>IMarker</c></param>
        public static void DestroyDevices(IRtc rtc, ILaser laser, IMarker marker)
        {
            marker.Stop();
            marker.Dispose();
            laser.Dispose();
            rtc.Dispose();
        }

        private static string Text_OnTextConvert(IMarker marker, ITextConvertible textConvertible)
        {
            var entity = textConvertible as IEntity;
            switch (entity.Name)
            {
                case "MyText1":
                    // For example, convert to DateTime format. link: https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
                    // Like as "yyyyMMdd HH:mm:ss"
                    return DateTime.Now.ToString(textConvertible.SourceText);
            }
            return textConvertible.SourceText;
        }
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
                    // x,y deviation by machine vision inspection
                    var errorXy = new System.Numerics.Vector2(
                           rand.Next(20) / 1000.0f - 0.01f,
                           rand.Next(20) / 1000.0f - 0.01f);
                    rtcCorrection2D.AddRelative(row, col,
                        new System.Numerics.Vector2(left + col * interval, top - row * interval), errorXy);
                }
            }
            return rtcCorrection2D;
        }
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