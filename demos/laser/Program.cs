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
 * Description : How to create and control many kinds of laser sources 
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

            // Field of view : 60mm
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

            // Create target laser source 
            ILaser laser = null;
            int laserType = 0;
            int laserId = 0;
            // Max laser output: 20W
            float maxWatt = 20;
            Console.WriteLine("'1' : control virtual(dummy) laser");
            Console.WriteLine("'2' : control analog(V) laser");
            Console.WriteLine("'3' : control frequency(Hz) laser");
            Console.WriteLine("'4' : control duty cycle(%) laser");
            Console.WriteLine("'5' : control digital 16bits output (0~65535) laser");
            Console.WriteLine("'6' : control digital 8bits output (0~255) laser");
            Console.Write("select laser (1~6) : ");
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
                case 1:
                    // custom
                    laser = LaserFactory.CreateVirtual(laserId, maxWatt);
                    break;
                case 2:
                    // analog (V)
                    // analog ch 1 or 2
                    laser = LaserFactory.CreateVirtualAnalog(laserId, maxWatt, 1, 0, 10);
                    break;
                case 3:
                    // frequency (Hz)
                    // output frequency = 40 ~ 50 KHz 
                    laser = LaserFactory.CreateVirtualFrequency(laserId, maxWatt, 40*1000, 50*1000);
                    break;
                case 4:
                    // duty cycle (0~99%)
                    // output pulse width = (1 / frequency ) * duty cycle / 100
                    laser = LaserFactory.CreateVirtualDutyCycle(laserId, maxWatt, 0, 99);
                    break;
                case 5:
                    // 16bits (0~65535)
                    // output D.OUT 16bits at EXTENSION1 port
                    laser = LaserFactory.CreateVirtualDO16Bits(laserId, maxWatt, 0, 65535);
                    break;
                case 6:
                    // 8bits (0~255)
                    // output D.OUT 8bits at EXTENSION2 port
                    laser = LaserFactory.CreateVirtualDO8Bits(laserId, maxWatt, 0, 255);
                    break;
            }
            // or
            // You can create specific vendor's laser source instance 
            //laser = LaserFactory.CreateAdvancedOptoWaveFotia()
            //laser = LaserFactory.CreateCoherentAviaLX()
            //laser = LaserFactory.CreateIPGYLPN();
            //laser = LaserFactory.CreatePhotonicsIndustryDX();
            //laser = LaserFactory.CreateSpectraPhysicsTalon();
            //laser = LaserFactory.CreateSPIG4();
            // and more at LaserFactory ...

            Console.WriteLine($"laser source [{laserType}]: {laser.Name} has created");

            // Assign RTC into laser
            laser.Scanner = rtc;
            // Initialize laser
            success &= laser.Initialize();
            // Default output power to 1%
            ILaserPowerControl laserPowerControl = laser as ILaserPowerControl;
            Debug.Assert(laserPowerControl != null);
            success &= laserPowerControl.CtlPower(laser.MaxPowerWatt * 0.01);
            Debug.Assert(success);

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for control various laser source");
                Console.WriteLine("'1' : draw circle");
                Console.WriteLine("'2' : draw circles with increasing output power");
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
                    case ConsoleKey.D2:
                        DrawCircle2(laser, rtc);
                        break;
                    case ConsoleKey.P:
                        Console.Write($"Target power(W) (Max.{laser.MaxPowerWatt}W): ");
                        try
                        {
                            double watt = Convert.ToDouble(Console.ReadLine());
                            success &= laserPowerControl.CtlPower(watt);
                        }
                        catch (Exception)
                        {
                        }
                        break;
                }
                Debug.Assert(success);
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        private static bool DrawCircle(ILaser laser, IRtc rtc)
        {
            bool success = true;
            // Begin list
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
                // End and execute list 
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

            // Begin list
            success &= rtc.ListBegin();
            success &= laser.ListBegin();
            const int steps = 10;
            for (int i = 1; i <= steps; i++)
            {
                // Increase laser power  (10, 20, ... 100 %)
                success &= laserPowerControl.ListPower(laser.MaxPowerWatt / steps * i);

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
                // End and execute list
                success &= laser.ListEnd();
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }
    }
}
