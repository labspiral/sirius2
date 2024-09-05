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
 * Description : How to use timed jump, mark and arc
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner.Rtc;
using System.Windows.Forms;
using SpiralLab.Sirius2.Scanner;

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
                Console.WriteLine("Testcase for timed operations");
                Console.WriteLine("'1' : draw rectangles with rotate");
                Console.WriteLine("'2' : draw arcs with rotate");
                Console.WriteLine("'3' : wait for trigger and draw rectangles");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                switch (key.Key)
                {
                    case ConsoleKey.D1 :
                        DrawRectangles(rtc, laser);
                        break;
                    case ConsoleKey.D2:
                        DrawArcs(rtc, laser);
                        break;
                    case ConsoleKey.D3:
                        DrawRectanglesWaitTrigger(rtc, laser);
                        break;
                }
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }
       
        /// <summary>
        /// Timed rectangles with rotate
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static bool DrawRectangles(IRtc rtc, ILaser laser, float width = 10, float height = 5)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcTimed = rtc as IRtcTimed;
            Debug.Assert(rtcTimed != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.SampleZ, //Z commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
            };

            bool success = true;
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            double msec = 10.0;
            double increase_msec = 20;
            for (float angle = 0; angle < 360; angle += 90)
            {
                var radian = angle * Math.PI / 180.0;
                rtc.MatrixStack.Push(Matrix4x4.CreateRotationZ((float)radian));
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |    . Rotating (CCW)
                //                        |   .            
                //                       -^- .
                // -------------------- < + > ---------------------
                //                       -v- 
                //                        |                 
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                success &= rtcTimed.ListTimedJumpTo(new Vector2(-width / 2, height / 2), msec);
                success &= rtcTimed.ListTimedMarkTo(new Vector2(width / 2, height / 2), msec);
                success &= rtcTimed.ListTimedMarkTo(new Vector2(width / 2, -height / 2), msec);
                success &= rtcTimed.ListTimedMarkTo(new Vector2(-width / 2, -height / 2), msec);
                success &= rtcTimed.ListTimedMarkTo(new Vector2(-width / 2, height / 2), msec);
                rtc.MatrixStack.Pop();
                msec += increase_msec;
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
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_timedrectangles.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"rectangles. start: {msec}. increase: {increase_msec} ms");
            }
            return success;
        }

        /// <summary>
        /// Timed arcs with rotate
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private static bool DrawArcs(IRtc rtc, ILaser laser, float radius = 10)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcTimed = rtc as IRtcTimed;
            Debug.Assert(rtcTimed != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.SampleZ, //Z commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
            };

            bool success = true;
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            double msec = 200.0;
            double increase_msec = 200;
            for (float angle = 0; angle < 360; angle += 90)
            {
                var radian = angle * Math.PI / 180.0;
                rtc.MatrixStack.Push(Matrix4x4.CreateRotationZ((float)radian));
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |    . Rotating (CCW)
                //                        |   .            
                //                       -^- .
                // -------------------- < + > ---------------------
                //                       -v- 
                //                        |                 
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                success &= rtcTimed.ListTimedJumpTo(new Vector2(radius, 0), msec);
                success &= rtcTimed.ListTimedArcTo(Vector2.Zero, 360, msec);
                rtc.MatrixStack.Pop();
                msec += increase_msec;
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
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_timedarcs.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"arcs. start: {msec}. increase: {increase_msec} ms");
            }
            return success;
        }


        /// <summary>
        /// Wait trigger and timed rectangles with rotate
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static bool DrawRectanglesWaitTrigger(IRtc rtc, ILaser laser, float width = 10, float height = 5)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcTimed = rtc as IRtcTimed;
            Debug.Assert(rtcTimed != null);
            var rtcConditionalIO = rtc as IRtcConditionalIO;
            Debug.Assert(rtcConditionalIO != null);

            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.ExtDI16, //EXTENSION1 D.IN PORT 
                 MeasurementChannels.LaserOn, //Gate signal 0/1
            };
            bool success = true;
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            // Waiting D.IN0 pin goes to HIGH at EXTENSION1 PORT 
            uint bitPosition = 0;
            Logger.Log(Logger.Types.Warn, "Waiting D.IN0 trigger at EXTENSION1 PORT ...");
            success &= rtcConditionalIO.ListReadExtDI16WaitEdgeUntil(bitPosition, true);
            double msec = 10.0;
            double increase_msec = 20;
            for (float angle = 0; angle < 360; angle += 90)
            {
                var radian = angle * Math.PI / 180.0;
                rtc.MatrixStack.Push(Matrix4x4.CreateRotationZ((float)radian));
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |    . Rotating (CCW)
                //                        |   .            
                //                       -^- .
                // -------------------- < + > ---------------------
                //                       -v- 
                //                        |                 
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                success &= rtcTimed.ListTimedJumpTo(new Vector2(-width / 2, height / 2), msec);
                success &= rtcTimed.ListTimedMarkTo(new Vector2(width / 2, height / 2), msec);
                success &= rtcTimed.ListTimedMarkTo(new Vector2(width / 2, -height / 2), msec);
                success &= rtcTimed.ListTimedMarkTo(new Vector2(-width / 2, -height / 2), msec);
                success &= rtcTimed.ListTimedMarkTo(new Vector2(-width / 2, height / 2), msec);
                rtc.MatrixStack.Pop();
                msec += increase_msec;
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
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_timedrectangles_wait_trigger.txt");
                // Scale up to x2 for LASERON signal 
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"rectangles(trigger). start: {msec}. increase: {increase_msec} ms");
            }
            return success;
        }
    }
}
