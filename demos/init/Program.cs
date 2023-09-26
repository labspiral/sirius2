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
 * Description : Initialize Sirius2 library 
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
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
        /// <summary>
        /// Fied of view : 60mm
        /// </summary>
        static double fov = 60.0;
        /// <summary>
        /// Default scanner  jump speed (mm/s)
        /// </summary>
        static double jumpSpeed = 500;
        /// <summary>
        /// Default scanner mark speed (mm/s)
        /// </summary>
        static double markSpeed = 500;
        /// <summary>
        /// Scanner jump delay (usec)
        /// </summary>
        static double jumpDelay = 200;
        /// <summary>
        /// Scanner mark delay (usec)
        /// </summary>
        static double markDelay = 200;
        /// <summary>
        /// Scanner polygon delay (usec)
        /// </summary>
        static double polygonDelay = 0;
        /// <summary>
        /// Laser on delay (usec)
        /// </summary>
        static double laserOnDelay = 10;
        /// <summary>
        /// Laser off delay (usec)
        /// </summary>
        static double laserOffDelay = 10;


        [STAThread]
        static void Main(string[] args)
        {
            // Initialize sirius2 library
            SpiralLab.Sirius2.Core.Initialize();
            // Get license information
            SpiralLab.Sirius2.Core.License(out var license);
            Console.WriteLine($"License: {license.ToString()}");

            bool success = true;
        
            // RTC5,6 using 20bits resolution
            var kfactor = Math.Pow(2, 20) / fov;

            // Default (1:1) correction file
            // Field correction file path: \correction\cor_1to1.ct5
            var correctionFile = Path.Combine(Config.CorrectionPath, "cor_1to1.ct5");

            // Create virtual RTC controller (without valid RTC controller)
            //var rtc = ScannerFactory.CreateVirtual(0, kfactor, correctionFile);
            // Create RTC5 controller
            var rtc = ScannerFactory.CreateRtc5(0, kfactor, LaserMode.Yag5, RtcSignalLevel.ActiveHigh, RtcSignalLevel.ActiveHigh, correctionFile);
            // Create RTC6 controller
            //var rtc = ScannerFactory.CreateRtc6(0, kfactor, LaserMode.Yag5, RtcSignalLevel.ActiveHigh, RtcSignalLevel.ActiveHigh, correctionFile);
            // Create RTC6 Ethernet controller
            //var rtc = ScannerFactory.CreateRtc6Ethernet(0, "192.168.0.100", "255.255.255.0", kfactor, LaserMode.Yag5, RtcSignalLevel.ActiveHigh, RtcSignalLevel.ActiveHigh, correctionFile);

            // Initialize RTC controller
            success &= rtc.Initialize();

            // Create virtual laser source with max 20W
            var laser = LaserFactory.CreateVirtual(0, 20);
            // Assign RTC into laser
            laser.Scanner = rtc;
            // Initialize laser
            success &= laser.Initialize();
            Debug.Assert(success);

            ConsoleKeyInfo key;
            do
            {
                // 50KHz, 2 usec
                success &= rtc.CtlFrequency(50 * 1000, 2);
                // 500 mm/s
                success &= rtc.CtlSpeed(jumpSpeed, markSpeed);
                // Basic delays
                success &= rtc.CtlDelay(laserOnDelay, laserOffDelay, jumpDelay, markDelay, polygonDelay);
                // Default power as 2W
                success &= laser.CtlPower(2);

                Debug.Assert(success);

                Console.WriteLine("Testcase for initialize and mark simple shapes");
                Console.WriteLine("'F1' : draw rectangle");
                Console.WriteLine("'F2' : draw circles");
                Console.WriteLine("'F3' : draw circle + measurement");
                Console.WriteLine("'F4' : draw line + matrix + measurement");
                Console.WriteLine("'F5' : draw circles with vary speed + measurement");
                Console.WriteLine("'Q'  : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                var sw = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.F1:
                        DrawRectangles(rtc, laser);
                        Logger.Log(Logger.Type.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                    case ConsoleKey.F2:                        
                        DrawCircles(rtc, laser);
                        Logger.Log( Logger.Type.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                    case ConsoleKey.F3:
                        DrawCircleWithMeasurement(rtc, laser, 5);
                        Logger.Log(Logger.Type.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                    case ConsoleKey.F4:
                        DrawLineWithMatrixAndMeasurement(rtc, laser);
                        Logger.Log(Logger.Type.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                    case ConsoleKey.F5:
                        DrawCirclesWithVarySpeedAndMeasurement(rtc, laser);
                        Logger.Log(Logger.Type.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                }                
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        private static bool DrawRectangles(IRtc rtc, ILaser laser, float width = 20, float height = 20)
        {
            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListType.Single);
            success &= rtc.ListJumpTo(new Vector2(-width / 2, height / 2));
            success &= rtc.ListMarkTo(new Vector2(width / 2, height / 2));
            success &= rtc.ListMarkTo(new Vector2(width / 2, -height / 2));
            success &= rtc.ListMarkTo(new Vector2(-width / 2, -height / 2));
            success &= rtc.ListMarkTo(new Vector2(-width / 2, height / 2));
            success &= rtc.ListJumpTo(Vector2.Zero);
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }

        private static bool DrawCircles(IRtc rtc, ILaser laser, float radius = 10)
        {
            int circleRepeats = 10;
            bool success = true;
            // List buffer with double buffered 
            success &= rtc.ListBegin(ListType.Auto);
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360 * circleRepeats);
            success &= rtc.ListJumpTo(Vector2.Zero);
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }

        private static bool DrawCircleWithMeasurement(IRtc rtc, ILaser laser, float radius = 10)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000; 
            // Max 4 channels at RTC5
            var channels = new MeasurementChannel[4]
            {
                 MeasurementChannel.SampleX, //X commanded
                 MeasurementChannel.SampleY, //Y commanded
                 MeasurementChannel.LaserOn, //Gate signal 0/1
                 MeasurementChannel.OutputPeriod, //KHz
            };

            bool success = true;
            // List begin with double buffered list
            success &= rtc.ListBegin(ListType.Auto);
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
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_circle.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "CIRCLE");
            }
            return success;
        }

        private static bool DrawLineWithMatrixAndMeasurement(IRtc rtc, ILaser laser, float length = 20)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannel[4]
            {
                 MeasurementChannel.SampleX, //X commanded
                 MeasurementChannel.SampleY, //Y commanded
                 MeasurementChannel.LaserOn, //Gate signal 0/1
                 MeasurementChannel.OutputPeriod, //KHz
            };

            bool success = true;
            // List begin with double buffered list
            success &= rtc.ListBegin(ListType.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            for (double angle = 0; angle < 360; angle += 45)
            {
                var radian = angle * Math.PI / 180.0;
                rtc.MatrixStack.Push(Matrix4x4.CreateRotationZ((float)radian));
                success &= rtc.ListJumpTo(new Vector2(length / 2, 0));
                success &= rtc.ListMarkTo(new Vector2(-length / 2, 0));
                rtc.MatrixStack.Pop();
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
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_line_rotating.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "LINES WITH ROTATING");
            }
            return success;
        }

        private static bool DrawCirclesWithVarySpeedAndMeasurement(IRtc rtc, ILaser laser, float radius = 10)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannel[4]
            {
                 MeasurementChannel.SampleX, //X commanded
                 MeasurementChannel.SampleY, //Y commanded
                 MeasurementChannel.LaserOn, //Gate signal 0/1
                 MeasurementChannel.OutputPeriod, //KHz
            };

            bool success = true;

            // List begin with double buffered list
            success &= rtc.ListBegin(ListType.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            // 500 mm/s
            success &= rtc.ListSpeed(500, 500);
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360);
            // 1000 mm/s
            success &= rtc.ListSpeed(1000, 1000);
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360);
            // 2000 mm/s
            success &= rtc.ListSpeed(2000, 2000);
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
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_circles_with_vary_speed.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "VARY SPEED");
            }
            return success;
        }
    }
}
