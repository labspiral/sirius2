﻿/*
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
 * Description : Many kinds of laser sources
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
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

            // Create target laser source 
            ILaser laser = null;
            int laserType = 0;
            int laserId = 0;
            float maxWatt = 20;
            Console.Write("select laser (0~17) : ");
            try
            {
                laserType = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception)
            {
                laserType = 0;
            }
            switch (laserType)
            {
                default:
                case 0:
                    // custom
                    laser = LaserFactory.CreateVirtual(laserId, maxWatt);
                    break;
                case 1:
                    // analog (V)
                    laser = LaserFactory.CreateVirtualAnalog(laserId, maxWatt, 1);
                    break;
                case 2:
                    // frequency (Hz)
                    laser = LaserFactory.CreateVirtualFrequency(laserId, maxWatt, 0, 50*1000);
                    break;
                case 3:
                    // duty cycle (0~99%)
                    laser = LaserFactory.CreateVirtualDutyCycle(laserId, maxWatt, 0, 99);
                    break;
                case 4:
                    // 16bits (0~65535)
                    laser = LaserFactory.CreateVirtualDO16Bits(laserId, maxWatt);
                    break;
                case 5:
                    // 8bits (0~255)
                    laser = LaserFactory.CreateVirtualDO8Bits(laserId, maxWatt);
                    break;
           
            }
            Console.WriteLine($"laser source [{laserType}]: {laser.Name} has created");

            // Assign RTC into laser
            laser.Rtc = rtc;
            // Initialize laser
            success &= laser.Initialize();
            // Default output power to 1%
            if (laser is ILaserPowerControl powerControl)
                success &= powerControl.CtlPower(laser.MaxPowerWatt * 0.01);
            Debug.Assert(success);

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for control variouse laser sources");
                Console.WriteLine("'1' : draw circle");
                Console.WriteLine("'2' : draw circles with change output power");
                Console.WriteLine("'P' : change output power");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                switch (key.Key)
                {
                    case ConsoleKey.D1 :
                        DrawCircle(laser, rtc);
                        break;
                    case ConsoleKey.P:
                        Console.Write($"target power (W) (max= {laser.MaxPowerWatt}): ");
                        try
                        {
                            double watt = Convert.ToDouble(Console.ReadLine());
                            if (laser is ILaserPowerControl powerControl2)
                                success &= powerControl2.CtlPower(watt);
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case ConsoleKey.D2:
                        DrawCircle2(laser, rtc);
                        break;

                }
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        private static bool DrawCircle(ILaser laser, IRtc rtc)
        {
            bool success = true;
            // Start list
            success &= rtc.ListBegin();
            success &= laser.ListBegin();
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
                success &= laser.ListEnd();
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }
        private static bool DrawCircle2(ILaser laser, IRtc rtc)
        {
            bool success = true;
            var laserPowerControl = laser as ILaserPowerControl;
            Debug.Assert(laserPowerControl != null);

            // Start list
            success &= rtc.ListBegin();
            success &= laser.ListBegin();
            for (int i = 0; i < 10; i++)
            {
                // Increase laser power  (0, 10, 20, ... 100 %)
                success &= laserPowerControl.ListPower(laser.MaxPowerWatt / 10 * i);

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
                success &= laser.ListEnd();
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }
    }
}
