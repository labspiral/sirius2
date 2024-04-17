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
 * Description : Secondary(2nd) scan-head control
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Common;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Mathematics;
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

            var rtc2ndHead = rtc as IRtc2ndHead;
            Debug.Assert(null != rtc2ndHead);

            // RTC controller must be has 2nd head option
            Debug.Assert(rtc.Is2ndHead);

            var correctionFile2 = Path.Combine(Config.CorrectionPath, "cor_1to1.ct5");
            rtc.CtlLoadCorrectionFile(CorrectionTables.Table2, correctionFile2);
            // primary(1st) scan head : Table1
            // secondary(2nd) scan head : Table2
            rtc.CtlSelectCorrection(CorrectionTables.Table1, CorrectionTables.Table2);

            // Distance bewteen primary and secondary head
            //
            //
            //         Primary Head       Secondary Head
            //              |                   |
            //              |                   |
            //              |                   |
            //    ----------+--------|-|--------+---------
            //    |                  | |                 |
            //    |                  |o|                 |
            //    |                  |v|                 |         
            //    |                  |e|                 |
            //    |         +        |r|        +        |
            //    |                  |l|                 |
            //    |                  |a|                 |
            //    |                  |p|                 |
            //    |                  | |                 |
            //    ----------+--------|-|--------+---------
            //              |                   |
            //              |                   |
            //              |                   |
            //              |----- Distance --->|
            //
            //                  

            // 'DistanceToSecondaryHead' value is not used meaningfully by internally
            // Let you know physical distance between scan-heads
            float overlapped = 10;
            rtc.DistanceToSecondaryHead = new Vector2((float)fov - overlapped, 0);

            // Reset each head's base offset
            rtc.PrimaryHeadBaseOffset = Offset.Zero;
            rtc.SecondaryHeadBaseOffset = Offset.Zero;

            // Reset each head's user offset
            rtc.PrimaryHeadUserOffset = Offset.Zero;
            rtc.SecondaryHeadUserOffset = Offset.Zero;

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
                Console.WriteLine("Testcase for 3d control and convert new correction file");
                Console.WriteLine("'1' : draw circle without offset");
                Console.WriteLine("'2' : draw circle with base offset");
                Console.WriteLine("'3' : draw circle with user offset");
                Console.WriteLine("'4' : draw circle with total offset by list");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                switch (key.Key)
                {
                    case ConsoleKey.D1 :
                        rtc2ndHead.PrimaryHeadBaseOffset = Offset.Zero;
                        rtc2ndHead.SecondaryHeadBaseOffset = Offset.Zero;
                        rtc2ndHead.PrimaryHeadUserOffset = Offset.Zero;
                        rtc2ndHead.SecondaryHeadUserOffset = Offset.Zero;
                        DrawCircle(laser, rtc);
                        break;
                    case ConsoleKey.D2:
                        // Base offset 
                        // Total offset = base offset + user offset
                        rtc2ndHead.PrimaryHeadBaseOffset = new Offset(10, 0, 0, 0.1f);
                        rtc2ndHead.SecondaryHeadBaseOffset = new Offset(-10, 0, 0, 0.1f);
                        DrawCircle(laser, rtc);
                        break;
                    case ConsoleKey.D3:
                        // User offset
                        // Total offset = base offset + user offset
                        rtc2ndHead.PrimaryHeadUserOffset = new Offset(-5, 0, 0, 0);
                        rtc2ndHead.SecondaryHeadUserOffset = new Offset(5, 0, 0, 0);
                        DrawCircle(laser, rtc);
                        break;
                    case ConsoleKey.D4:
                        DrawCircle2(laser, rtc);
                        break;
                }
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        private static bool DrawCircle(ILaser laser, IRtc rtc)
        {
            var rtc2ndHead = rtc as IRtc2ndHead;
            Debug.Assert(null != rtc2ndHead);

            bool success = true;
            // Start list
            success &= rtc.ListBegin();
            for (int i = 0; i < 10; i++)
            {
                // Draw line
                success &= rtc.ListJumpTo(new Vector2(0, 0));
                success &= rtc.ListMarkTo(new Vector2(10, 0));
                // Draw circle
                success &= rtc.ListJumpTo(new Vector2((float)10, 0));
                success &= rtc.ListArcTo(new Vector2(0, 0), 360.0);
                if (!success)
                    break;
            }
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }

        private static bool DrawCircle2(ILaser laser, IRtc rtc)
        {
            var rtc2ndHead = rtc as IRtc2ndHead;
            Debug.Assert(null != rtc2ndHead);

            bool success = true;
            // Start list
            success &= rtc.ListBegin();
            // Rotate and translate each heads
            success &= rtc2ndHead.ListHeadOffset(ScannerHeads.Primary, new Offset(5, 0, 0, 0));
            success &= rtc2ndHead.ListHeadOffset(ScannerHeads.Secondary, new Offset(-5, 0, 0, 0));
            for (int i = 0; i < 10; i++)
            {
                // Draw line
                success &= rtc.ListJumpTo(new Vector2(0, 0));
                success &= rtc.ListMarkTo(new Vector2(10, 0));
                // Draw circle
                success &= rtc.ListJumpTo(new Vector2((float)10, 0));
                success &= rtc.ListArcTo(new Vector2(0, 0), 360.0);
                if (!success)
                    break;
            }
            // Revert
            success &= rtc2ndHead.ListHeadOffset(ScannerHeads.Primary, Offset.Zero);
            success &= rtc2ndHead.ListHeadOffset(ScannerHeads.Secondary, Offset.Zero);
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }
    }
}
