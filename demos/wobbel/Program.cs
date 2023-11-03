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
 * Description : Wobbel shapes
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;


namespace Demos
{
    internal class Program
    {
        /// <summary>
        /// When defining the wobbel shape and its frequency take the dynamics of the scan head and laser into account.
        /// Otherwise, an overheating and even a permanent DAMAGE !!! of the system may occur 
        /// </summary>
        /// <remarks>
        /// Max. Wobbel frequency (Hz): Must be lower than 1/(Tracking error * 10).
        /// For example:
        /// BasicCube 10: Tracking Error 0.14ms -> Hz= 1/0.0014s = 714 Hz (Max)
        /// IntelliSCAN III 10: Tracking Error 0.11ms -> Hz= 1/0.0011s = 909 Hz (Max)
        /// </remarks>
        static double WobbelFrequency = 200;

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
                Console.WriteLine("Testcase for wobbel shapes");
                Console.WriteLine("'C' : draw circle");
                Console.WriteLine("'1' : draw circle with wobbel 0.5, 0.5, ellipse");
                Console.WriteLine("'2' : draw circle with wobbel 1, 5, ellipse");
                Console.WriteLine("'3' : draw circle with wobbel 0.5, 0.5, figure8");
                Console.WriteLine("'R' : draw rectangle");
                Console.WriteLine("'4' : draw rectangle with wobbel 0.5, 0.5, ellipse");
                Console.WriteLine("'5' : draw rectangle with wobbel 0.5, 1.0, ellipse");
                Console.WriteLine("'6' : draw rectangle with wobbel 0.5, 0.5, figure8");
                Console.WriteLine("'7' : draw rectangle with freely defined wobbel (zigzag)");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                var sw = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.C:
                        DrawCircle(laser, rtc);
                        break;
                    case ConsoleKey.D1:
                        DrawCircleWithWobbel1(laser, rtc);
                        break;
                    case ConsoleKey.D2:
                        DrawCircleWithWobbel2(laser, rtc);
                        break;
                    case ConsoleKey.D3:
                        DrawCircleWithWobbel3(laser, rtc);
                        break;
                    case ConsoleKey.R:
                        DrawRectangle(laser, rtc);
                        break;
                    case ConsoleKey.D4:
                        DrawRectangleWithWobbel1(laser, rtc);
                        break;
                    case ConsoleKey.D5:
                        DrawRectangleWithWobbel2(laser, rtc);
                        break;
                    case ConsoleKey.D6:
                        DrawRectangleWithWobbel3(laser, rtc);
                        break;
                    case ConsoleKey.D7:
                        DrawRectangleWithDefinedWobbel(laser, rtc);
                        break;
                }                
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        private static bool DrawCircle(ILaser laser, IRtc rtc, int repeats = 1, float radius = 10)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);

            // 50KHz Sample rate (max 100KHz)
            double sampleRateHz = 50 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };
            // Max 8 channels at RTC6
            //var channels = new MeasurementChannel[8]
            //{
            //     MeasurementChannels.SampleX, //X commanded
            //     MeasurementChannels.SampleY, //Y commanded
            //     MeasurementChannels.LaserOn, //Gate signal 0/1
            //     MeasurementChannels.Enc0Counter, 
            //     MeasurementChannels.Enc1Counter,
            //     MeasurementChannels.OutputPeriod,
            //     MeasurementChannels.PulseLength,
            //     MeasurementChannels.WobbelAmplitude,
            //};

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            for (int i = 0; i < repeats; i++)
            {
                success &= rtc.ListJumpTo(new Vector2(radius, 0));
                success &= rtc.ListArcTo(Vector2.Zero, 360);
                if (!success)
                    break;
            }
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
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_circle.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "CIRCLE");
            }
            return success;
        }
        private static bool DrawCircleWithWobbel1(ILaser laser, IRtc rtc, int repeats = 1, float radius = 10)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcWobbel = rtc as IRtcWobbel;
            Debug.Assert(rtcWobbel != null);

            // 50KHz Sample rate (max 100KHz)
            double sampleRateHz = 50 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };
            // Max 8 channels at RTC6
            //var channels = new MeasurementChannel[8]
            //{
            //     MeasurementChannels.SampleX, //X commanded
            //     MeasurementChannels.SampleY, //Y commanded
            //     MeasurementChannels.LaserOn, //Gate signal 0/1
            //     MeasurementChannels.Enc0Counter, 
            //     MeasurementChannels.Enc1Counter,
            //     MeasurementChannels.OutputPeriod,
            //     MeasurementChannels.PulseLength,
            //     MeasurementChannels.WobbelAmplitude,
            //};

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtcWobbel.ListWobbelBegin(0.5, 0.5, WobbelFrequency, WobbelShapes.Ellipse);
            for (int i = 0; i < repeats; i++)
            {
                success &= rtc.ListJumpTo(new Vector2(radius, 0));
                success &= rtc.ListArcTo(Vector2.Zero, 360);
                if (!success)
                    break;
            }
            success &= rtcWobbel.ListWobbelEnd();
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
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_circle_wobbel1.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"Wobbel (0.5, 0.5, {WobbelFrequency}Hz, Ellipse)");
            }
            return success;
        }
        private static bool DrawCircleWithWobbel2(ILaser laser, IRtc rtc, int repeats = 1, float radius = 10)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcWobbel = rtc as IRtcWobbel;
            Debug.Assert(rtcWobbel != null);

            // 50KHz Sample rate (max 100KHz)
            double sampleRateHz = 50 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                MeasurementChannels.SampleX, //X commanded
                MeasurementChannels.SampleY, //Y commanded
                MeasurementChannels.LaserOn, //Gate signal 0/1
                MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtcWobbel.ListWobbelBegin(1, 5, WobbelFrequency, WobbelShapes.Ellipse);
            for (int i = 0; i < repeats; i++)
            {
                success &= rtc.ListJumpTo(new Vector2(radius, 0));
                success &= rtc.ListArcTo(Vector2.Zero, 360);
                if (!success)
                    break;
            }
            success &= rtcWobbel.ListWobbelEnd();
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
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_circle_wobbel2.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"Wobbel (1, 5, {WobbelFrequency}Hz, Ellipse)");
            }
            return success;
        }
        private static bool DrawCircleWithWobbel3(ILaser laser, IRtc rtc, int repeats = 1, float radius = 10)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcWobbel = rtc as IRtcWobbel;
            Debug.Assert(rtcWobbel != null);

            // 50KHz Sample rate (max 100KHz)
            double sampleRateHz = 50 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                MeasurementChannels.SampleX, //X commanded
                MeasurementChannels.SampleY, //Y commanded
                MeasurementChannels.LaserOn, //Gate signal 0/1
                MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtcWobbel.ListWobbelBegin(0.5, 0.5, WobbelFrequency, WobbelShapes.Perpendicular8); // WobbelShapes.Parallel8
            for (int i = 0; i < repeats; i++)
            {
                success &= rtc.ListJumpTo(new Vector2(radius, 0));
                success &= rtc.ListArcTo(Vector2.Zero, 360);
                if (!success)
                    break;
            }
            success &= rtcWobbel.ListWobbelEnd();
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
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_circle_wobbel3.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"Wobbel (0.5, 0.5, {WobbelFrequency}Hz, Figure of 8)");
            }
            return success;
        }

        private static bool DrawRectangle(ILaser laser, IRtc rtc, int repeats = 1, float width = 20, float height = 20)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);

            // 50KHz Sample rate (max 100KHz)
            double sampleRateHz = 50 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
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
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_rectangle.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "RECTANGLE");
            }
            return success;
        }
        private static bool DrawRectangleWithWobbel1(ILaser laser, IRtc rtc, int repeats = 1, float width = 20, float height = 20)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcWobbel = rtc as IRtcWobbel;
            Debug.Assert(rtcWobbel != null);

            // 50KHz Sample rate (max 100KHz)
            double sampleRateHz = 50 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtcWobbel.ListWobbelBegin(0.5, 0.5, WobbelFrequency, WobbelShapes.Ellipse);
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
            success &= rtcWobbel.ListWobbelEnd();
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
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_rectangle_wobbel1.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"Wobbel (0.5, 0.5, {WobbelFrequency}Hz, Ellipse)");
            }
            return success;
        }
        private static bool DrawRectangleWithWobbel2(ILaser laser, IRtc rtc, int repeats = 1, float width = 20, float height = 20)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcWobbel = rtc as IRtcWobbel;
            Debug.Assert(rtcWobbel != null);

            // 50KHz Sample rate (max 100KHz)
            double sampleRateHz = 50 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                MeasurementChannels.SampleX, //X commanded
                MeasurementChannels.SampleY, //Y commanded
                MeasurementChannels.LaserOn, //Gate signal 0/1
                MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtcWobbel.ListWobbelBegin(0.5, 1.0, WobbelFrequency, WobbelShapes.Ellipse);
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
            success &= rtcWobbel.ListWobbelEnd();
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
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_rectangle_wobbel2.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"Wobbel (0.5, 1.0, {WobbelFrequency}Hz, Ellipse)");
            }
            return success;
        }
        private static bool DrawRectangleWithWobbel3(ILaser laser, IRtc rtc, int repeats = 1, float width = 20, float height = 20)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcWobbel = rtc as IRtcWobbel;
            Debug.Assert(rtcWobbel != null);

            // 50KHz Sample rate (max 100KHz)
            double sampleRateHz = 50 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                MeasurementChannels.SampleX, //X commanded
                MeasurementChannels.SampleY, //Y commanded
                MeasurementChannels.LaserOn, //Gate signal 0/1
                MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtcWobbel.ListWobbelBegin(0.5, 0.5, WobbelFrequency, WobbelShapes.Perpendicular8); // WobbelShapes.Parallel8
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
            success &= rtcWobbel.ListWobbelEnd();
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
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_rectangle_wobbel3.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"Wobbel (0.5, 0.5, {WobbelFrequency}Hz, Figure of 8)");
            }
            return success;
        }
        private static bool DrawRectangleWithDefinedWobbel(ILaser laser, IRtc rtc, int repeats = 1, float width = 20, float height = 20)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcWobbel = rtc as IRtcWobbel;
            Debug.Assert(rtcWobbel != null);

            // 50KHz Sample rate (max 100KHz)
            double sampleRateHz = 50 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                MeasurementChannels.SampleX, //X commanded
                MeasurementChannels.SampleY, //Y commanded
                MeasurementChannels.LaserOn, //Gate signal 0/1
                MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);

            var list = new List<KeyValuePair<Vector2, uint>>();
            list.Add(new KeyValuePair<Vector2, uint>(new Vector2(0.1f, 0), 5));
            list.Add(new KeyValuePair<Vector2, uint>(new Vector2(-0.1f, 0), 5*2));
            list.Add(new KeyValuePair<Vector2, uint>(new Vector2(0.1f, 0), 5));
            // ZigZag with 100um 
            // |                    
            // |                             .
            // |                           .   .
            // |                         .       .   
            // |                       .           .
            // |                     .               . 
            // .-------------------.-------------------.------->
            // | .               .
            // |   .           .
            // |     .       .
            // |       .   .
            // |         .
            // |
            success &= rtcWobbel.ListWobbelDefine(list.ToArray());
            success &= rtcWobbel.ListWobbelBegin(0.5, 0.5, WobbelFrequency, WobbelShapes.Defined);
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
            success &= rtcWobbel.ListWobbelEnd();
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
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_rectangle_wobbel4.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"Wobbel (0.5, 0.5, {WobbelFrequency}Hz, Free Defined: ZigZag)");
            }
            return success;
        }
    }
}
