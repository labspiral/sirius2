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
 * Description : Skywriting 
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
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

            // Fied of view : 60mm
            var fov = 60.0;
            // RTC5,6 using 20bits resolution
            var kfactor = Math.Pow(2, 20) / fov;

            // Default (1:1) correction file
            // Field correction file path: \correction\cor_1to1.ct5
            var correctionFile = Path.Combine(Config.CorrectionPath, "cor_1to1.ct5");

            // Create virtual RTC controller (without valid RTC controller)
            //var rtc = ScannerFactory.CreateVirtual(0, kfactor, correctionFile);
            // Create RTC5 controller
            //var rtc = ScannerFactory.CreateRtc5(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            // Create RTC6 controller
            var rtc = ScannerFactory.CreateRtc6(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            // Create RTC6 Ethernet controller
            //var rtc = ScannerFactory.CreateRtc6Ethernet(0, "192.168.0.100", "255.255.255.0", kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);

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
                Console.WriteLine("Testcase for skywriting");
                Console.WriteLine("'R' : draw rectangles");
                Console.WriteLine("'1' : draw rectangles with skywriting mode1");
                Console.WriteLine("'2' : draw rectangles with skywriting mode2");
                Console.WriteLine("'3' : draw rectangles with skywriting mode3 (limit: 90)");
                Console.WriteLine("'4' : draw rectangles with skywriting mode3 (limit: 89)");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                var sw = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.R:
                        DrawRectangles(laser, rtc, SkyWritingMode.Deactivate);
                        break;
                    case ConsoleKey.D1:
                        DrawRectangles(laser, rtc, SkyWritingMode.Mode1);
                        break;
                    case ConsoleKey.D2:
                        DrawRectangles(laser, rtc, SkyWritingMode.Mode2);
                        break;
                    case ConsoleKey.D3:
                        CosineLimit = Math.Cos(Math.PI / 2);
                        DrawRectangles(laser, rtc, SkyWritingMode.Mode3);
                        break;
                    case ConsoleKey.D4:
                        CosineLimit = Math.Cos(89 * Math.PI / 180.0);
                        DrawRectangles(laser, rtc, SkyWritingMode.Mode3);
                        break;
                }                
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        static double CosineLimit = 0;

        private static bool DrawRectangles(ILaser laser, IRtc rtc, SkyWritingMode mode, int repeats = 5, float width = 20, float height = 20)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcSkywriting = rtc as IRtcSkyWriting;
            Debug.Assert(rtcSkywriting != null);

            // 50KHz Sample rate (max 100KHz)
            double sampleRateHz = 50 * 1000;
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
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);

            success &= rtcSkywriting.ListSkyWritingBegin(mode, 500, 200, 200 * 0.15, 200 * 0.1, CosineLimit);
            for (int i = 0; i < repeats; i++)
            {
                success &= rtc.ListJumpTo(new Vector2(-width / 2, height / 2));
                success &= rtc.ListMarkTo(new Vector2(width / 2, height / 2));
                success &= rtc.ListMarkTo(new Vector2(width / 2, -height / 2));
                success &= rtc.ListMarkTo(new Vector2(-width / 2, -height / 2));
                success &= rtc.ListMarkTo(new Vector2(-width / 2, height / 2));
                if (!success)
                    break;
            }
            success &= rtcSkywriting.ListSkyWritingEnd();
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
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_skywriting_{mode}_{CosineLimit}.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"SKYWRITING {mode}");
            }
            return success;
        }
    }
}
