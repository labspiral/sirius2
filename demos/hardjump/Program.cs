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
 * Description : Hard jump tuning mode
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
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
            var rtcJumpMode = rtc as IRtcJumpMode;
            Debug.Assert(rtcJumpMode != null);

            // Enable output synchronization if needed
            // Connect: LASER SOURCE SYNC_OUT signal -> RTC DIN1 at LASER PORT
            /*
            var lcs = rtc.LaserControlSignal;
            lcs.Add(Rtc5LaserControlSignal.Bit.OutputSynchronization);
            lcs.Add(Rtc5LaserControlSignal.Bit.ExtPulseSignalRisingEdge);
            
            //if RTC6
            //lcs.Add(Rtc6LaserControlSignal.Bit.OutputSynchronization);
            //lcs.Add(Rtc6LaserControlSignal.Bit.ExtPulseSignalRisingEdge);
            rtc.LaserControlSignal = lcs;
            // and 
            //rtc.CtlPulseSynchronization(true, 0);
            */

            // Create virtual laser source with max 20W
            var laser = LaserFactory.CreateVirtual(0, 20);

            // Assign RTC into laser
            laser.Rtc = rtc;
            // Initialize laser
            success &= laser.Initialize();
            // Default power as 2W
            success &= laser.CtlPower(2);
            Debug.Assert(success);

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for hard jump tuning mode");
                Console.WriteLine("'V' : disable jump mode (vector mode)");
                Console.WriteLine("'J' : enable jump mode (jump mode)");
                Console.WriteLine("'1' : jump and on");
                Console.WriteLine("'2' : jump(hard/microvector) and on");
                Console.WriteLine("'3' : jump and drills (iDRIVE: intelliSCAN, intellicube, intelliWELD or intelliDRILL scan system only)");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                Console.WriteLine("WARNING !!! LASER IS BUSY ...");
                switch (key.Key)
                {
                    case ConsoleKey.V:
                        {
                            // Configure vector tuning mode at primary scan head
                            var jumpMode = RtcJumpMode.Empty;
                            jumpMode.Flag = JumpModeFlag.Disable;
                            success &= rtcJumpMode.CtlJumpMode(jumpMode);
                        }
                        break;
                    case ConsoleKey.J:
                        {
                            // Configure jump tuning mode at primary scan head
                            var jumpMode = RtcJumpMode.Empty;
                            jumpMode.Flag = JumpModeFlag.EnabledButDeactivated;
                            jumpMode.JumpTuningPrimaryX = 1;
                            jumpMode.JumpTuningPrimaryY = 1;
                            jumpMode.LimitLength = 0;
                            success &= rtcJumpMode.CtlJumpMode(jumpMode);
                        }
                        break;
                    case ConsoleKey.D1:
                        DrawJumpAndOn(laser, rtc, 0);
                        break;
                    case ConsoleKey.D2:
                        DrawJumpAndOn(laser, rtc, 1);
                        break;
                    case ConsoleKey.D3:
                        DrawJumpAndDrill(laser, rtc);
                        break;
                }
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }
       
        private static bool DrawJumpAndOn(ILaser laser, IRtc rtc, int hardJumpIfOne)
        {
            var rtcJumpMode = rtc as IRtcJumpMode;
            Debug.Assert(rtcJumpMode != null);

            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 20KHz Sample rate (max 100KHz)
            double sampleRateHz = 20 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannel[4]
            {
                 MeasurementChannel.SampleX, //X commanded
                 MeasurementChannel.SampleY, //Y commanded
                 MeasurementChannel.LaserOn, //Gate signal 0/1
                 MeasurementChannel.OutputPeriod, //Converted to mm
            };

            bool success = true;
            // Start list buffer
            success &= rtc.ListBegin( ListType.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);

            // Each dx = 1 mm
            for (float x = -10; x <= 10; x += 1)
            {
                if (1 != hardJumpIfOne)
                {
                    success &= rtcJumpMode.ListJumpMode(JumpModeFlag.EnabledAndActivated);
                    success &= rtc.ListJumpTo(new Vector2(x, 0));
                }
                else
                    success &= rtcJumpMode.ListJumpHard(new Vector2(x, 0), 0, 0);
                // 1ms
                success &= rtc.ListLaserOn(1);
                if (!success)
                    break;
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute();
            }

            // Temporary measurement file
            var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_jumpmode{hardJumpIfOne}.txt");
            // Save measurement result to file
            success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
            // Plot as a graph
            RtcMeasurementHelper.Plot(measurementFile, $"Jump and Shoots {hardJumpIfOne}");
          
            return success;
        }
        private static bool DrawJumpAndDrill(ILaser laser, IRtc rtc)
        {
            var rtcJumpMode = rtc as IRtcJumpMode;
            Debug.Assert(rtcJumpMode != null);

            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 20KHz Sample rate (max 100KHz)
            double sampleRateHz = 20 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannel[4]
            {
                 MeasurementChannel.SampleX, //X commanded
                 MeasurementChannel.SampleY, //Y commanded
                 MeasurementChannel.LaserOn, //Gate signal 0/1
                 MeasurementChannel.OutputPeriod, //Converted to mm
            };

            bool success = true;
            // Start list buffer
            success &= rtc.ListBegin(ListType.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            success &= rtcJumpMode.ListJumpMode(JumpModeFlag.EnabledAndActivated);

            // Each dx = 1 mm
            for (float x = -10; x <= 10; x += 1)
            {
                // 1ms
                success &= rtcJumpMode.ListJumpAndDrill(new Vector2(x, 0), 1000);
                if (!success)
                    break;
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute();
            }

            // Temporary measurement file
            var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_jumpmode3.txt");
            // Save measurement result to file
            success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
            // Plot as a graph
            RtcMeasurementHelper.Plot(measurementFile, "Jump and Drills");

            return success;
        }
    }
}
