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
 * Description : SCANahead + SDC (Spot Distance Control) at RTC6
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading;

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

            // Create RTC6 controller
            var rtc = ScannerFactory.CreateRtc6(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            // Create RTC6 Ethernet controller
            //var rtc = ScannerFactory.CreateRtc6Ethernet(0, "192.168.0.100", "255.255.255.0", kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);

            // Initialize RTC controller
            success &= rtc.Initialize();
            // 50KHz, 2 usec
            success &= rtc.CtlFrequency(50 * 1000, 2);
            // 2000 mm/s
            success &= rtc.CtlSpeed(2*1000, 2*100);

            Debug.Assert(success);

            // Must be activated SCANahead option at RTC card
            Debug.Assert(rtc.IsScanAhead);

            // Activate auto delays
            success &= rtc.CtlDelayAutoByScanAhead(true);

            // Set laser delays
            success &= rtc.CtlDelayScanAhead(10, 100);

            // excelliSCAN scan head must be connected and load pre-stored parameter from excelliSCAN head.
            success &= rtc.CtlAutoDelayParams(AutoDelayParamModes.Load, ScannerHeads.Primary, CorrectionTables.Table1);

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

            var rtcAlc = rtc as IRtcAutoLaserControl;

            ConsoleKeyInfo key;
            do
            {
                Debug.Assert(success);

                Console.WriteLine("Testcase for SCANahead + SDC");
                Console.WriteLine($"----------------------------------------");
                Console.WriteLine("'S' : get status");
                Console.WriteLine("'1' : enable spot distance control");
                Console.WriteLine("'2' : disable spot distance control");
                Console.WriteLine("'3' : line quality scale factor");
                Console.WriteLine("'L' : draw line");
                Console.WriteLine("'C' : draw circle");
                Console.WriteLine("'R' : draw rectangle");
                Console.WriteLine("'D' : draw circle with dots");
                Console.WriteLine("'P' : draw square area with pixel operation");
                Console.WriteLine("'H' : draw heavy and slow job with worker thread");
                Console.WriteLine("'A' : abort to mark and finish the worker thread");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                Console.WriteLine($"{Environment.NewLine}");
                if (key.Key == ConsoleKey.Q)
                    break;
                switch (key.Key)
                {
                    case ConsoleKey.S:  
                        // RTC's status
                        if (rtc.CtlGetStatus(RtcStatus.Busy))
                            Console.WriteLine($"Rtc is busy!");
                        if (!rtc.CtlGetStatus(RtcStatus.PowerOK))
                            Console.WriteLine($"Scanner power is not ok");
                        if (!rtc.CtlGetStatus(RtcStatus.PositionAckOK))
                            Console.WriteLine($"Scanner position is not acked");
                        if (!rtc.CtlGetStatus(RtcStatus.NoError))
                            Console.WriteLine($"Rtc status has an error");
                        break;
                    case ConsoleKey.D1:
                        // SDC on
                        double sdcDistance = 0.1;
                        success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.SpotDistance, AutoLaserControlModes.ActualVelocityWithSCANAhead, sdcDistance);
                        break;
                    case ConsoleKey.D2:
                        // SDC off
                        success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.Disabled, AutoLaserControlModes.Disabled);
                        break;
                    case ConsoleKey.D3:
                        // Line quality scale factor
                        rtc.ScanAheadLineParamsCornerScale = 100;
                        rtc.ScanAheadLineParamsEndScale = 100;
                        rtc.ScanAheadLineParamsAccScale = 0;
                        break;
                    case ConsoleKey.L:
                        // Draw line
                        DrawLine(laser, rtc, -10, -10, 10, 10);
                        break;
                    case ConsoleKey.C:
                        // Draw circle
                        DrawCircle(laser, rtc, 10);
                        break;
                    case ConsoleKey.R:
                        // Draw rectangle 
                        DrawRectangle(laser, rtc, 10, 10);
                        break;
                    case ConsoleKey.D:
                        // Draw dotted circle 
                        DrawCircleWithDots(laser, rtc, 10, 1.0f);
                        break;
                    case ConsoleKey.P:
                        // Draw filled rectangle with raster
                        DrawSquareAreaWithPixels(laser, rtc, 10, 0.2f);
                        break;
                    case ConsoleKey.H:
                        // Draw with heavy works
                        DrawTooHeavyAndSlowJob(laser, rtc);
                        break;
                    case ConsoleKey.A:
                        // Abort operation
                        StopMarkAndReset(laser, rtc);
                        break;
                }
            } while (true);

            StopMarkAndReset(laser, rtc);

            rtc.Dispose();
            laser.Dispose();
        }

        /// <summary>
        /// Draw circle
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        /// <param name="radius"></param>
        private static bool DrawCircle(ILaser laser, IRtc rtc, float radius)
        {
            if (rtc.CtlGetStatus(RtcStatus.Busy))
                return false;
            Console.WriteLine("WARNING !!! LASER IS BUSY ... Draw Circle");
            bool success = true;
            success &= rtc.ListBegin();
            success &= rtc.ListJumpTo(new System.Numerics.Vector2(radius, 0));
            success &= rtc.ListArcTo(new System.Numerics.Vector2(0, 0), 360.0f);
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(true);
            return success;
        }
        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        private static bool DrawLine(ILaser laser, IRtc rtc, float x1, float y1, float x2, float y2)
        {
            if (rtc.CtlGetStatus(RtcStatus.Busy))
                return false;
            Console.WriteLine("WARNING !!! LASER IS BUSY ... DrawLine");
            bool success = true;
            success &= rtc.ListBegin();
            success &= rtc.ListJumpTo(new System.Numerics.Vector2(x1, y1));
            success &= rtc.ListMarkTo(new System.Numerics.Vector2(x2, y2));
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(true);
            return success;
        }
        /// <summary>
        /// Draw rectangle
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static bool DrawRectangle(ILaser laser, IRtc rtc, float width, float height)
        {
            if (rtc.CtlGetStatus(RtcStatus.Busy))
                return false;
            Console.WriteLine("WARNING !!! LASER IS BUSY ... DrawRectangle");
            bool success = true;
            success &= rtc.ListBegin();
            success &= rtc.ListJumpTo(new System.Numerics.Vector2(-width / 2, height / 2));
            success &= rtc.ListMarkTo(new System.Numerics.Vector2(width / 2, height / 2));
            success &= rtc.ListMarkTo(new System.Numerics.Vector2(width / 2, -height / 2));
            success &= rtc.ListMarkTo(new System.Numerics.Vector2(-width / 2, -height / 2));
            success &= rtc.ListMarkTo(new System.Numerics.Vector2(-width / 2, height / 2));
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(true);
            return success;
        }
        /// <summary>
        /// Draw circle with dots
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        /// <param name="radius"></param>
        /// <param name="durationMsec"></param>
        private static bool DrawCircleWithDots(ILaser laser, IRtc rtc, float radius, float durationMsec)
        {
            if (rtc.CtlGetStatus(RtcStatus.Busy))
                return false;
            Console.WriteLine("WARNING !!! LASER IS BUSY ... DrawCircleWithDots");
            bool success = true;
            success &= rtc.ListBegin();
            for (float angle = 0; angle < 360; angle += 1)
            {
                double x = radius * Math.Sin(angle * Math.PI / 180.0);
                double y = radius * Math.Cos(angle * Math.PI / 180.0);
                success &= rtc.ListJumpTo(new System.Numerics.Vector2((float)x, (float)y));
                //laser signal on during specific time
                success &= rtc.ListLaserOn(durationMsec);
                if (!success)
                    break;
            }
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(true);
            return success;
        }
        /// <summary>
        /// Draw square area with pixels
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        /// <param name="length"></param>
        /// <param name="gap"></param>
        private static bool DrawSquareAreaWithPixels(ILaser laser, IRtc rtc, float length, float gap)
        {
            if (rtc.CtlGetStatus(RtcStatus.Busy))
                return false;
            Console.WriteLine("WARNING !!! LASER IS BUSY ... DrawSquareAreaWithPixels");
            var rtcRaster = rtc as IRtcRaster;
            if (null == rtcRaster)
                return false;
            int counts = (int)(length / gap);
            // Every 200 usec
            float period = 200;
            // Gap or interval: distance from pixel to pixel
            var delta = new System.Numerics.Vector2(gap, 0);

            bool success = true;
            success &= rtc.ListBegin();
            for (int i = 0; i < counts; i++)
            {
                // Jump to start position 
                success &= rtc.ListJumpTo(new System.Numerics.Vector2(0, i * gap));
                // Mark raster(or dots) at line
                success &= rtcRaster.ListRasterLine( RasterModes.JumpAndShoot, period, delta, (uint)counts, ExtensionChannels.ExtAO2);
                for (int j = 0; j < counts; j++)
                    success &= rtcRaster.ListRasterPixel(50, 0.5f); // each pixel with 50usec, 5V
                if (!success)
                    break;
            }
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(true);
            return success;
        }

        static Thread thread;
        static IRtc rtc;
        private static void DrawTooHeavyAndSlowJob(ILaser laser, IRtc rtc)
        {
            if (rtc.CtlGetStatus(RtcStatus.Busy))
            {
                Console.WriteLine("Processing are working already !");
                return;
            }

            Program.rtc = rtc;
            Program.thread = new Thread(WorkerThread);
            Program.thread.Start();
        }
        private static void WorkerThread()
        {
            bool success = true;
            float width = 1;
            float height = 1;

            Console.WriteLine("WARNING !!! LASER IS BUSY... DoHeavyWork thread");
            // Auto list buffer (doubled) 
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtc.ListJumpTo(new System.Numerics.Vector2(-width / 2, height / 2));
            for (int i = 0; i < 1000 * 1000 * 10; i++)
            {
                // List commands = 4 * 1000*1000*10 = 40M counts 
                success &= rtc.ListMarkTo(new System.Numerics.Vector2(width / 2, height / 2));
                success &= rtc.ListMarkTo(new System.Numerics.Vector2(width / 2, -height / 2));
                success &= rtc.ListMarkTo(new System.Numerics.Vector2(-width / 2, -height / 2));
                success &= rtc.ListMarkTo(new System.Numerics.Vector2(-width / 2, height / 2));
                if (!success)
                    break;
            }
            if (success)
            {
                success &= rtc.ListEnd();
                // Wait until list commands has finished
                success &= rtc.ListExecute(true);
            }
            if (success)
                Console.WriteLine("Success to mark by worker thread");
            else
                Console.WriteLine("Fail to mark by worker thread !");
        }
        private static void StopMarkAndReset(ILaser laser, IRtc rtc)
        {
            Console.WriteLine("Trying to abort ...");

            // Abort to mark
            rtc.CtlAbort();

            // Wait for rtc busy off (wait for thread has finished)
            rtc.CtlBusyWait();

            Program.thread?.Join();
            Program.thread = null;

            // Reset rtc's status
            rtc.CtlReset();
        }
    }
}
