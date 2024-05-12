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
 * 2023 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : How to use multiple RTC instances
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection.Emit;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;

namespace Demos
{
    internal class Program
    {

        static readonly int InstanceCounts = 4;
        static readonly IRtc[] RtcArray = new IRtc[InstanceCounts];
        static readonly ILaser[] LaserArray = new ILaser[InstanceCounts];

        [STAThread]
        static void Main(string[] args)
        {
            // Initialize sirius2 library
            SpiralLab.Sirius2.Core.Initialize();

            // Init each instance
            for (int i = 0; i < InstanceCounts; i++)
            {
                bool success = InitInstance(i, out var rtc, out var laser);
                if (success)
                {
                    RtcArray[i] = rtc;
                    LaserArray[i] = laser;
                }
            }

            ConsoleKeyInfo key;
            //0~3
            int targetInstance = 0;
            do
            {
                Console.Title = $"Target Instance: {targetInstance}";
                Console.WriteLine("Testcase for raster operation");
                for(int i= 0; i < InstanceCounts; i++)
                    Console.WriteLine($"'{i}' : change target instance");
                Console.WriteLine("'A' : draw raster line");
                Console.WriteLine("'B' : darw circle + measurement");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                switch (key.Key)
                {
                    case ConsoleKey.D0:
                        targetInstance = 0;
                        break;
                    case ConsoleKey.D1:
                        targetInstance = 1;
                        break;
                    case ConsoleKey.D2:
                        targetInstance = 2;
                        break;
                    case ConsoleKey.D3:
                        targetInstance = 3;
                        break;
                    case ConsoleKey.A:
                        DrawRasterLine(targetInstance);
                        break;
                    case ConsoleKey.B:
                        DrawCircleWithMeasurement(targetInstance);
                        break;
                }
            } while (true);

            for (int i = 0; i < InstanceCounts; i++)
            {
                RtcArray[i]?.Dispose();
                LaserArray[i]?.Dispose();
            }
        }

        static bool InitInstance(int index, out IRtc rtc, out ILaser laser)
        {
            bool success = true;

            // Fied of view : 60mm
            var fov = 60.0;
            // RTC5,6 using 20bits resolution
            var kfactor = Math.Pow(2, 20) / fov;

            // Default (1:1) correction file
            // Field correction file path: \correction\cor_1to1.ct5
            var correctionFile = Path.Combine(Config.CorrectionPath, "cor_1to1.ct5");
            // If RTC4
            //var correctionFile = Path.Combine(Config.CorrectionPath, "cor_1to1.ctb");

            // Create RTC controller 
            //var rtc = ScannerFactory.CreateVirtual(0, kfactor, correctionFile);
            //var rtc = ScannerFactory.CreateRtc4(0, kfactor, LaserModes.Yag1, correctionFile);
            //var rtc = ScannerFactory.CreateRtc5(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            rtc = ScannerFactory.CreateRtc6(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
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

            var rtcRaster = rtc as IRtcRaster;
            Debug.Assert(rtcRaster != null);

            // Create virtual laser source with max 20W
            laser = LaserFactory.CreateVirtual(0, 20);

            var laserPowerControl = laser as ILaserPowerControl;
            Debug.Assert(laserPowerControl != null);
            // Assign RTC into laser
            laser.Scanner = rtc;
            // Initialize laser
            success &= laser.Initialize();
            // Default power as 2W
            success &= laserPowerControl.CtlPower(2);
            Debug.Assert(success);

            return success;
        }

        private static bool DrawRasterLine(int index)
        {
            var rtc = RtcArray[index];
            var laser = LaserArray[index];

            bool success = true;
            var rtcRaster = rtc as IRtcRaster;
            // Start list
            success &= rtc.ListBegin();
            // Jump to start
            success &= rtc.ListJumpTo(new Vector2(-10, 0));
            
            // Pixel period: 100 usec (0.0001 s)
            double period = 100;
            // Pixel duration: 10 usec
            double duration = 10;
            // Distance = 0.1 mm
            float dx = 0.1f;
            // Calculated speed (mm/s) = 1000 mm/s (= 0.1mm / 0.0001s)
            uint counts = 1000;
            // Prepare raster horizontal line
            success &= rtcRaster.ListRasterLine(RasterModes.JumpAndShoot, period, new Vector2(dx, 0), counts);
            for (int i = 0; i < counts; i++)
            {
                // laser on during 10 usec
                success &= rtcRaster.ListRasterPixel(duration);
                if (!success)
                    break;
            }

            if (success)
            {
                // End of list
                success &= rtc.ListEnd();
                // Execute list
                success &= rtc.ListExecute(true);
            }
            return success;
        }

        private static bool DrawCircleWithMeasurement(int index, float radius = 10)
        {
            var rtc = RtcArray[index];
            var laser = LaserArray[index];

            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };
            // Max 8 channels at RTC6
            //var channels = new MeasurementChannels[]
            //{
            //     MeasurementChannels.SampleX, //X commanded
            //     MeasurementChannels.SampleY, //Y commanded
            //     MeasurementChannels.LaserOn, //Gate signal 0/1
            //     MeasurementChannels.Enc0Counter, 
            //     MeasurementChannels.Enc1Counter,
            //     MeasurementChannels.OutputPeriod,
            //     MeasurementChannels.PulseLength,
            //     MeasurementChannels.ExtAO1,
            //};

            bool success = true;
            // List begin with double buffered list
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360);
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, $"{index}measurement_circle.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"Instance [{index}] CIRCLE");
            }
            return success;
        }
    }
}
