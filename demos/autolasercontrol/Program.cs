/*
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
 *
 * 2023 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Automatic laser control (ALC) 
 *   1. Position Dependent
 *   2. Velocity Dependent 
 *   3. Defined Vector
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;

namespace Demos
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Initialize sirius2 library
            SpiralLab.Sirius2.Core.Initialize();

            bool success = true;

            // Field of view : 60mm
            var fov = 60.0;
            // RTC5,6 using 20bits resolution
            var kfactor = Math.Pow(2, 20) / fov;

            // Default (1:1) correction file
            // Field correction file path: \correction\cor_1to1.ct5
            var correctionFile = Path.Combine(Config.CorrectionPath, "cor_1to1.ct5");
            // If RTC4
            //var correctionFile = Path.Combine(Config.CorrectionPath, "cor_1to1.ctb");

            // Create RTC controller 
            //var rtc = ScannerFactory.CreateVirtual(0, kfactor, LaserModes.Yag1, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            //var rtc = ScannerFactory.CreateRtc4(0, kfactor, LaserModes.Yag1, correctionFile);
            //var rtc = ScannerFactory.CreateRtc5(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            var rtc = ScannerFactory.CreateRtc6(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            //var rtc = ScannerFactory.CreateRtc6Ethernet(0, "192.168.0.100", "255.255.255.0", kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            //var rtc = ScannerFactory.CreateRtc6SyncAxis(0, "your config xml file");

            // Initialize RTC controller
            success &= rtc.Initialize();
            // 50KHz, 2 usec
            success &= rtc.CtlFrequency(50 * 1000, 2);
            // 500 mm/s
            success &= rtc.CtlSpeed(500, 500);
            // Basic delays
            success &= rtc.CtlDelay(10, 100, 200, 200, 0);
            Debug.Assert(success);

            // Create virtual laser source with max 20W
            var laser = LaserFactory.CreateVirtual(0, 20);

            // Assign RTC into laser
            laser.Scanner = rtc;
            // Initialize laser
            success &= laser.Initialize();
            // Default power as 2W
            success &= laser.CtlPower(2);
            Debug.Assert(success);

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for automatic laser control");
                Console.WriteLine("'S' : get status");
                Console.WriteLine("'A' : set velocity + analog output");
                Console.WriteLine("'B' : set velocity + analog output + position dependent");
                Console.WriteLine("'C' : actual velocity + frequency");
                Console.WriteLine("'D' : defined vector + analog output");
                Console.WriteLine("'E' : spot distance control");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                var sw = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.S:  
                        // RTC internal status
                        if (rtc.CtlGetStatus(RtcStatus.Busy))
                            Console.WriteLine($"\r\nRtc is busy!");
                        if (!rtc.CtlGetStatus(RtcStatus.PowerOK))
                            Console.WriteLine($"\r\nScanner power is not ok");
                        if (!rtc.CtlGetStatus(RtcStatus.PositionAckOK))
                            Console.WriteLine($"\r\nScanner position is not acked");
                        if (!rtc.CtlGetStatus(RtcStatus.NoError))
                            Console.WriteLine($"\r\nRtc status has an error");                        
                        break;
                    case ConsoleKey.A:
                        DrawLine1(laser, rtc, -10, -10, 10, 10);
                        break;
                    case ConsoleKey.B:
                        DrawLine2(laser, rtc, -10, -10, 10, 10);
                        break;
                    case ConsoleKey.C:
                        DrawLine3(laser, rtc, -10, -10, 10, 10);
                        break;
                    case ConsoleKey.D:
                        DrawLine4(laser, rtc, -10, -10, 10, 10);
                        break;
                    case ConsoleKey.E:
                        DrawLine5(laser, rtc, -10, -10, 10, 10);
                        break;
                }                
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        /// <summary>
        /// Line (Set velocity + Analog voltage)
        /// </summary>
        private static bool DrawLine1(ILaser laser, IRtc rtc, float x1, float y1, float x2, float y2)
        {
            var rtcAlc = rtc as IRtcAutoLaserControl;
            if (null == rtcAlc)
                return false;
            bool success = true;
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.ExtAO1, 
            };

            // Position dependent ALC off
            rtcAlc.CtlAlcByPositionTable(null);
            // Analog1: 5V (Min: 4V, Max: 6V)
            success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.Analog1, AutoLaserControlModes.SetVelocity, 5, 4, 6);
            success &= rtc.ListBegin();
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtc.ListJumpTo(new Vector2(x1, y1));
            success &= rtc.ListMarkTo(new Vector2(x2, y2));
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.Disabled, AutoLaserControlModes.Disabled);
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_alc_1.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "ALC = ANALOG1 + SetVelocity");
            }
            return success;
        }

        /// <summary>
        /// Line (Set velocity + Analog voltage + Position dependent)
        /// </summary>
        private static bool DrawLine2(ILaser laser, IRtc rtc, float x1, float y1, float x2, float y2)
        {
            var rtcAlc = rtc as IRtcAutoLaserControl;
            if (null == rtcAlc)
                return false;
            bool success = true;
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.ExtAO1,
            };

            // Distance(or radius) (mm), scale (0~4)
            var kvList = new List<KeyValuePair<double, double>>();
            kvList.Add(new KeyValuePair<double, double>(5, 0.9));
            kvList.Add(new KeyValuePair<double, double>(10, 1));
            kvList.Add(new KeyValuePair<double, double>(15, 1.1));
            // Position dependent ALC on
            success &= rtcAlc.CtlAlcByPositionTable(kvList.ToArray());
            // Analog1: 5V (Min: 4V, Max: 6V)
            success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.Analog1, AutoLaserControlModes.SetVelocity, 5, 0, 10);
            success &= rtc.ListBegin();
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtc.ListJumpTo(new Vector2(x1, y1));
            success &= rtc.ListMarkTo(new Vector2(x2, y2));
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.Disabled, AutoLaserControlModes.Disabled);
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_alc_2.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "ALC = ANALOG1 + SetVelocity + Position dependent");
            }
            return success;
        }

        /// <summary>
        /// Line (Actual velocity + vary Frequency)
        /// To use actual velocity, scanner with iDRIVE~ feature has required
        /// </summary>
        private static bool DrawLine3(ILaser laser, IRtc rtc, float x1, float y1, float x2, float y2)
        {
            var rtcAlc = rtc as IRtcAutoLaserControl;
            if (null == rtcAlc)
                return false;
            bool success = true;
            // Position dependent ALC off
            success &= rtcAlc.CtlAlcByPositionTable(null);
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            // Target frequency : 50KHz
            // Lower cut off frequency : 40KHz
            // Upper cut off frequency : 60KHz
            success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.Frequency, AutoLaserControlModes.ActualVelocity, 50 * 1000, 40 * 1000, 60 * 1000);
            success &= rtc.ListBegin();
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtc.ListJumpTo(new Vector2(x1, y1));
            success &= rtc.ListMarkTo(new Vector2(x2, y2));
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.Disabled, AutoLaserControlModes.Disabled);
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_alc_3.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "ALC = FREQUENCY + ActualVelocity");
            }
            return success;
        }

        /// <summary>
        /// Line (Defined vector + Analog voltage)
        /// </summary>
        private static bool DrawLine4(ILaser laser, IRtc rtc, float x1, float y1, float x2, float y2)
        {
            var rtcAlc = rtc as IRtcAutoLaserControl;
            if (null == rtcAlc)
                return false;
            bool success = true;

            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.ExtAO1,
            };

            // Position dependent ALC off
            success &= rtcAlc.CtlAlcByPositionTable(null);
            success &= rtc.ListBegin();
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            // Analog1 voltage : 5V
            // Jump ramp with 0.5 : 5 * 0.5 = 2.5V
            // Mark ramp with 1.5 : 5 * 1.5 = 7.5V
            success &= rtcAlc.ListAlcByVectorBegin<double>(AutoLaserControlSignals.Analog1, 5); 
            success &= rtc.ListJumpTo(new Vector2(x1, y1), 0.5F); 
            success &= rtc.ListMarkTo(new Vector2(x2, y2), 1.5F); 
            success &= rtcAlc.ListAlcByVectorEnd();
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.Disabled, AutoLaserControlModes.Disabled);
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_alc_4.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "ALC = ANALOG1 + Defined Vector");
            }
            return success;
        }

        /// <summary>
        /// Line (Actual velocity + Spot distance control)
        /// </summary>
        private static bool DrawLine5(ILaser laser, IRtc rtc, float x1, float y1, float x2, float y2)
        {
            var rtcAlc = rtc as IRtcAutoLaserControl;
            if (null == rtcAlc)
                return false;

            // support rtc6 + SCANahead only
            Debug.Assert(rtc is Rtc6);
            Debug.Assert(rtc.IsScanAhead);

            bool success = true;
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            // Position dependent ALC off
            success &= rtcAlc.CtlAlcByPositionTable(null);
            // SDC with distance : 0.1mm
            double distance = 0.1;
            success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.SpotDistance, AutoLaserControlModes.ActualVelocityWithSCANAhead, distance);
            success &= rtc.ListBegin();
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtc.ListJumpTo(new Vector2(x1, y1));
            success &= rtc.ListMarkTo(new Vector2(x2, y2));
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.Disabled, AutoLaserControlModes.Disabled);
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_alc_5.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "ALC = SDC");
            }
            return success;
        }
    }
}
