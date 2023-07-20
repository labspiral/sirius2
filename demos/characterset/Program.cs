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
 * Description : Download Characterset and Mark Text, Date, Time and SerialNo 
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
            var rtc = ScannerFactory.CreateRtc5(0, kfactor, LaserMode.Yag5, RtcSignalLevel.ActiveHigh, RtcSignalLevel.ActiveHigh, correctionFile);
            // Create RTC6 controller
            //var rtc = ScannerFactory.CreateRtc6(0, kfactor, LaserMode.Yag5, RtcSignalLevel.ActiveHigh, RtcSignalLevel.ActiveHigh, correctionFile);
            // Create RTC6 Ethernet controller
            //var rtc = ScannerFactory.CreateRtc6Ethernet(0, "192.168.0.100", "255.255.255.0", kfactor, LaserMode.Yag5, RtcSignalLevel.ActiveHigh, RtcSignalLevel.ActiveHigh, correctionFile);

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

            var rtcCharacterSet = rtc as IRtcCharacterSet;
            Debug.Assert(rtcCharacterSet != null);

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for character set");
                Console.WriteLine("'C' : create character set");
                Console.WriteLine("'B' : delete character set");
                Console.WriteLine("'T' : mark to text");
                Console.WriteLine("'D' : mark to date");
                Console.WriteLine("'F1' : datetime offset");
                Console.WriteLine("'F2' : datetime reset");
                Console.WriteLine("'S' : mark to serial number");
                Console.WriteLine("'R' : reset serial number");
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
                        CreateCharacterSet(laser, rtc);
                        break;
                    case ConsoleKey.B:
                        DeleteCharacterSet(rtc);
                        break;
                    case ConsoleKey.T:
                        MarkToText(laser, rtc);
                        break;
                    case ConsoleKey.D:
                        MarkToDate(laser, rtc);
                        break;
                    case ConsoleKey.I:
                        MarkToTime(laser, rtc);
                        break;
                    case ConsoleKey.F1:
                        // Datetime offset
                        rtcCharacterSet.DateTimeOffsetDays = 0;
                        rtcCharacterSet.DateTimeOffsetHours = 1;
                        rtcCharacterSet.DateTimeOffsetMinutes = -30;
                        rtcCharacterSet.IsDateTimeOffset = true;
                        break;
                    case ConsoleKey.F2:
                        rtcCharacterSet.IsDateTimeOffset = false;
                        break;
                    case ConsoleKey.S:
                        MarkToSerial(laser, rtc);
                        break;
                    case ConsoleKey.R:
                        // Reset serial no
                        rtcCharacterSet.CtlSerialNoReset(100, 5);
                        break;
                }                
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        /// <summary>
        /// Create character set and download into RTC
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        private static void CreateCharacterSet(ILaser laser, IRtc rtc)
        {
            var rtcCharacterSet = rtc as IRtcCharacterSet;
            Debug.Assert(rtcCharacterSet != null);

            // Begin to download character 9 (downloaded into list buffer 3)
            rtcCharacterSet.CtlCharacterSetBegin(CharacterSet._0);

            // Sample characterset
            // 0~9 (Digits)
            // ; (Colon)
            // - (Dash)
            //   (Space)
            rtcCharacterSet.CtlCharacterBegin('0');
            rtc.ListJumpTo(new Vector2(5, 10));
            rtc.ListMarkTo(new Vector2(5 + 0, 10 - 5));
            rtc.ListMarkTo(new Vector2(5 + 0 + 0, 10 - 5 - 5));
            rtc.ListMarkTo(new Vector2(5 + 0 + 0 - 5, 10 - 5 - 5));
            rtc.ListMarkTo(new Vector2(5 + 0 + 0 - 5, 10 - 5 - 5 + 5));
            rtc.ListMarkTo(new Vector2(5 + 0 + 0 - 5 + 0, 10 - 5 - 5 + 5 + 5));
            rtc.ListMarkTo(new Vector2(5 + 0 + 0 - 5 + 0 + 5, 10 - 5 - 5 + 5 + 5 + 0));
            rtc.ListJumpTo(new Vector2(5 + 0 + 0 - 5 + 0 + 5 + 5, 10 - 5 - 5 + 5 + 5 + 0 - 10));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin('1');
            rtc.ListJumpTo(new Vector2(5, 0));
            rtc.ListMarkTo(new Vector2(5 + 0, 0 + 10));
            rtc.ListJumpTo(new Vector2(5 + 0 + 5, 0 + 10 - 10));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin('2');
            rtc.ListJumpTo(new Vector2(0, 10));
            rtc.ListMarkTo(new Vector2(0 + 5, 10 + 0));
            rtc.ListMarkTo(new Vector2(0 + 5 + 0, 10 + 0 - 5));
            rtc.ListMarkTo(new Vector2(0 + 5 + 0 - 5, 10 + 0 - 5 + 0));
            rtc.ListMarkTo(new Vector2(0 + 5 + 0 - 5 + 0, 10 + 0 - 5 + 0 - 5));
            rtc.ListMarkTo(new Vector2(0 + 5 + 0 - 5 + 0 + 5, 10 + 0 - 5 + 0 - 5 + 0));
            rtc.ListJumpTo(new Vector2(0 + 5 + 0 - 5 + 0 + 5 + 5, 10 + 0 - 5 + 0 - 5 + 0 + 0));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin('3');
            rtc.ListJumpTo(new Vector2(0, 10));
            rtc.ListMarkTo(new Vector2(0 + 5, 10 + 0));
            rtc.ListMarkTo(new Vector2(0 + 5 + 0, 10 + 0 - 5));
            rtc.ListMarkTo(new Vector2(0 + 5 + 0 - 5, 10 + 0 - 5 + 0));
            rtc.ListJumpTo(new Vector2(0 + 5 + 0 - 5 + 5, 10 + 0 - 5 + 0 + 0));
            rtc.ListMarkTo(new Vector2(0 + 5 + 0 - 5 + 5 + 0, 10 + 0 - 5 + 0 + 0 - 5));
            rtc.ListMarkTo(new Vector2(0 + 5 + 0 - 5 + 5 + 0 - 5, 10 + 0 - 5 + 0 + 0 - 5 + 0));
            rtc.ListJumpTo(new Vector2(0 + 5 + 0 - 5 + 5 + 0 - 5 + 10, 10 + 0 - 5 + 0 + 0 - 5 + 0 + 0));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin('4');
            rtc.ListJumpTo(new Vector2(0, 10));
            rtc.ListMarkTo(new Vector2(0 + 0, 10 - 5));
            rtc.ListMarkTo(new Vector2(0 + 0 + 5, 10 - 5 + 0));
            rtc.ListJumpTo(new Vector2(0 + 0 + 5 + 0, 10 - 5 + 0 + 5));
            rtc.ListMarkTo(new Vector2(0 + 0 + 5 + 0 + 0, 10 - 5 + 0 + 5 - 10));
            rtc.ListJumpTo(new Vector2(0 + 0 + 5 + 0 + 0 + 5, 10 - 5 + 0 + 5 - 10 + 0));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin('5');
            rtc.ListJumpTo(new Vector2(5, 10));
            rtc.ListMarkTo(new Vector2(5 - 5, 10 + 0));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0, 10 + 0 - 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5, 10 + 0 - 5 + 0));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5 + 0, 10 + 0 - 5 + 0 - 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5 + 0 - 5, 10 + 0 - 5 + 0 - 5 + 0));
            rtc.ListJumpTo(new Vector2(5 - 5 + 0 + 5 + 0 - 5 + 10, 10 + 0 - 5 + 0 - 5 + 0 + 0));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin('6');
            rtc.ListJumpTo(new Vector2(5, 10));
            rtc.ListMarkTo(new Vector2(5 - 5, 10 + 0));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0, 10 + 0 - 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 0, 10 + 0 - 5 - 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 0 + 5, 10 + 0 - 5 - 5 + 0));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 0 + 5 + 0, 10 + 0 - 5 - 5 + 0 + 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 0 + 5 + 0 - 5, 10 + 0 - 5 - 5 + 0 + 5 + 0));
            rtc.ListJumpTo(new Vector2(5 - 5 + 0 + 0 + 5 + 0 - 5 + 10, 10 + 0 - 5 - 5 + 0 + 5 + 0 - 5));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin('7');
            rtc.ListJumpTo(new Vector2(0, 10));
            rtc.ListMarkTo(new Vector2(0 + 5, 10 + 0));
            rtc.ListMarkTo(new Vector2(0 + 5 + 0, 10 + 0 - 5));
            rtc.ListMarkTo(new Vector2(0 + 5 + 0 + 0, 10 + 0 - 5 - 5));
            rtc.ListJumpTo(new Vector2(0 + 5 + 0 + 0 + 5, 10 + 0 - 5 - 5 + 0));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin('8');
            rtc.ListJumpTo(new Vector2(5, 10));
            rtc.ListMarkTo(new Vector2(5 - 5, 10 + 0));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0, 10 + 0 - 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5, 10 + 0 - 5 + 0));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5 + 0, 10 + 0 - 5 + 0 + 5));
            rtc.ListJumpTo(new Vector2(5 - 5 + 0 + 5 + 0 + 0, 10 + 0 - 5 + 0 + 5 - 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5 + 0 + 0 + 0, 10 + 0 - 5 + 0 + 5 - 5 - 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5 + 0 + 0 + 0 - 5, 10 + 0 - 5 + 0 + 5 - 5 - 5 + 0));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5 + 0 + 0 + 0 - 5 + 0, 10 + 0 - 5 + 0 + 5 - 5 - 5 + 0 + 5));
            rtc.ListJumpTo(new Vector2(5 - 5 + 0 + 5 + 0 + 0 + 0 - 5 + 0 + 10, 10 + 0 - 5 + 0 + 5 - 5 - 5 + 0 + 5 - 5));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin('9');
            rtc.ListJumpTo(new Vector2(5, 10));
            rtc.ListMarkTo(new Vector2(5 - 5, 10 + 0));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0, 10 + 0 - 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5, 10 + 0 - 5 + 0));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5 + 0, 10 + 0 - 5 + 0 + 5));
            rtc.ListJumpTo(new Vector2(5 - 5 + 0 + 5 + 0 + 0, 10 + 0 - 5 + 0 + 5 - 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5 + 0 + 0 + 0, 10 + 0 - 5 + 0 + 5 - 5 - 5));
            rtc.ListMarkTo(new Vector2(5 - 5 + 0 + 5 + 0 + 0 + 0 - 5, 10 + 0 - 5 + 0 + 5 - 5 - 5 + 0));
            rtc.ListJumpTo(new Vector2(5 - 5 + 0 + 5 + 0 + 0 + 0 - 5 + 10, 10 + 0 - 5 + 0 + 5 - 5 - 5 + 0 + 0));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin(';');
            rtc.ListJumpTo(new Vector2(0, 2));
            rtc.ListMarkTo(new Vector2(0 + 2, 2 + 0));
            rtc.ListMarkTo(new Vector2(0 + 2 + 0, 2 + 0 + 2));
            rtc.ListMarkTo(new Vector2(0 + 2 + 0 - 2, 2 + 0 + 2 + 0));
            rtc.ListMarkTo(new Vector2(0 + 2 + 0 - 2 + 0, 2 + 0 + 2 + 0 - 2));
            rtc.ListJumpTo(new Vector2(0 + 2 + 0 - 2 + 0 + 0, 2 + 0 + 2 + 0 - 2 + 4));
            rtc.ListMarkTo(new Vector2(0 + 2 + 0 - 2 + 0 + 0 + 2, 2 + 0 + 2 + 0 - 2 + 4 + 0));
            rtc.ListMarkTo(new Vector2(0 + 2 + 0 - 2 + 0 + 0 + 2 + 0, 2 + 0 + 2 + 0 - 2 + 4 + 0 + 2));
            rtc.ListMarkTo(new Vector2(0 + 2 + 0 - 2 + 0 + 0 + 2 + 0 - 2, 2 + 0 + 2 + 0 - 2 + 4 + 0 + 2 + 0));
            rtc.ListMarkTo(new Vector2(0 + 2 + 0 - 2 + 0 + 0 + 2 + 0 - 2 + 0, 2 + 0 + 2 + 0 - 2 + 4 + 0 + 2 + 0 - 2));
            rtc.ListJumpTo(new Vector2(0 + 2 + 0 - 2 + 0 + 0 + 2 + 0 - 2 + 0 + 5, 2 + 0 + 2 + 0 - 2 + 4 + 0 + 2 + 0 - 2 - 6));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin('-');
            rtc.ListJumpTo(new Vector2(0, 5));
            rtc.ListMarkTo(new Vector2(0 + 5, 5 + 0));
            rtc.ListJumpTo(new Vector2(0 + 5 + 5, 5 + 0 - 5));
            rtcCharacterSet.CtlCharacterEnd();

            rtcCharacterSet.CtlCharacterBegin(' ');
            rtc.ListJumpTo(new Vector2(5, 0));
            rtcCharacterSet.CtlCharacterEnd();

            // End to download character
            rtcCharacterSet.CtlCharacterSetEnd();
        }

        /// <summary>
        /// Delete character set
        /// </summary>
        /// <param name="rtc"></param>
        private static void DeleteCharacterSet(IRtc rtc)
        {
            var rtcCharacterSet = rtc as IRtcCharacterSet;
            Debug.Assert(rtcCharacterSet != null);

            rtcCharacterSet.CtlCharacterSetClear();
            Debug.Assert(false == rtcCharacterSet.CtlCharacterSetIsExist('0'));
        }

        /// <summary>
        /// Mark text
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        private static bool MarkToText(ILaser laser, IRtc rtc)
        {
            if (rtc.CtlGetStatus(RtcStatus.Busy))
                return false;
            bool success = true;
            var rtcCharacterSet = rtc as IRtcCharacterSet;
            Debug.Assert(rtcCharacterSet != null);

            success &= rtc.ListBegin(ListType.Single);
            success &= rtc.ListJumpTo(new Vector2(-10, 0));
            success &= rtcCharacterSet.ListText("123 456");
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute();
            }
            return success;
        }

        /// <summary>
        /// Mark date
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        private static bool MarkToDate(ILaser laser, IRtc rtc)
        {
            if (rtc.CtlGetStatus(RtcStatus.Busy))
                return false;
            bool success = true;
            var rtcCharacterSet = rtc as IRtcCharacterSet;
            Debug.Assert(rtcCharacterSet != null);

            success &= rtc.ListBegin(ListType.Single);
            success &= rtc.ListJumpTo(new Vector2(-10, 0));
            success &= rtcCharacterSet.ListDate(DateFormat.MonthDigits, true);
            success &= rtc.ListJumpTo(new Vector2(10, 0));
            success &= rtcCharacterSet.ListDate(DateFormat.Day, true);
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute();
            }
            return success;
        }
        /// <summary>
        /// Mark time
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        private static bool MarkToTime(ILaser laser, IRtc rtc)
        {
            if (rtc.CtlGetStatus(RtcStatus.Busy))
                return false;
            bool success = true;
            var rtcCharacterSet = rtc as IRtcCharacterSet;
            Debug.Assert(rtcCharacterSet != null);

            success &= rtc.ListBegin( ListType.Single);
            success &= rtc.ListJumpTo(new Vector2(-10, 0));
            success &= rtcCharacterSet.ListTime(TimeFormat.Hours24, true);
            success &= rtc.ListJumpTo(new Vector2(10, 0));
            success &= rtcCharacterSet.ListTime(TimeFormat.Minutes, true);
            success &= rtc.ListJumpTo(new Vector2(30, 0));
            success &= rtcCharacterSet.ListTime(TimeFormat.Seconds, true);
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute();
            }
            return success;
        }
        /// <summary>
        /// Mark serial no
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        private static bool MarkToSerial(ILaser laser, IRtc rtc)
        {
            if (rtc.CtlGetStatus(RtcStatus.Busy))
                return false;
            bool success = true;
            var rtcCharacterSet = rtc as IRtcCharacterSet;
            Debug.Assert(rtcCharacterSet != null);

            //Reset serial no as 1000 with + 1
            rtcCharacterSet.CtlSerialNoReset(1000, 1);

            success &= rtc.ListBegin(ListType.Single);
            success &= rtc.ListJumpTo(new Vector2(-10, -20));
            success &= rtcCharacterSet.ListSerialNo(4, SerialNoFormat.LeadingWithZero);
            success &= rtc.ListJumpTo(new Vector2(-10, 0));
            success &= rtcCharacterSet.ListSerialNo(4, SerialNoFormat.LeadingWithZero);
            success &= rtc.ListJumpTo(new Vector2(-10, 20));
            success &= rtcCharacterSet.ListSerialNo(4, SerialNoFormat.LeadingWithZero);
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute();
            }
            return success;
        }
    }
}
