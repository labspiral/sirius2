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
 * Description : How to use various powermeters
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
using SpiralLab.Sirius2.PowerMeter;
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

            // Create powermeter
            //var powerMeter = PowerMeterFactory.CreateVirtual(0, laser.MaxPowerWatt);
            //var powerMeter = PowerMeterFactory.CreateCoherentPowerMax(0, 1);
            //var powerMeter = PowerMeterFactory.CreateOphirPhotonics(0, "SERIALNO");
            //var powerMeter = PowerMeterFactory.CreateThorlabs(0, "M00544131"); // for USB communication
            //var powerMeter = PowerMeterFactory.CreateThorlabs(0, 3); // for COM port communication
            var powerMeter = new MyPowerMeter(0, "NAME");
            success &= powerMeter.Initialize();
            Debug.Assert(success);

            powerMeter.OnStarted += PowerMeter_OnStarted;
            powerMeter.OnMeasured += PowerMeter_OnMeasured;
            powerMeter.OnStopped += PowerMeter_OnStopped;

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for powermeter");
                Console.WriteLine("'1' : start powermeter");
                Console.WriteLine("'2' : stop powermeter");
                Console.WriteLine("'3' : vary laser output power");
                Console.WriteLine("'5' : laser on (warning !!!)");
                Console.WriteLine("'6' : laser off");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        powerMeter.CtlReset();
                        powerMeter.CtlClear();
                        success &= powerMeter.CtlStart();
                        break;
                    case ConsoleKey.D2:
                        success &= powerMeter.CtlStop();
                        break;
                    case ConsoleKey.D3:
                        double watt = laser.LastPowerWatt;
                        Console.Write($"Output Power (W) (Max= {laser.MaxPowerWatt}): ");
                        try {
                            watt = Convert.ToDouble(Console.ReadLine());
                            success &= laser.CtlPower(watt);
                        }
                        catch (Exception)
                        { }
                        break;
                    case ConsoleKey.D5:
                        success &= rtc.CtlLaserOn();
                        break;
                    case ConsoleKey.D6:
                        success &= rtc.CtlLaserOff();
                        break;
                }
            } while (true);

            powerMeter.OnStarted -= PowerMeter_OnStarted;
            powerMeter.OnMeasured -= PowerMeter_OnMeasured;
            powerMeter.OnStopped -= PowerMeter_OnStopped;

            rtc.CtlLaserOff();
            powerMeter.Dispose();
            rtc.Dispose();
            laser.Dispose();
        }

        private static void PowerMeter_OnStopped(IPowerMeter powerMeter)
        {
            Console.Title = $"Stopped !";
        }

        private static void PowerMeter_OnMeasured(IPowerMeter powerMeter, DateTime dt, double watt)
        {
            Console.Title = $"Power: {watt:F3} W";
        }

        private static void PowerMeter_OnStarted(IPowerMeter powerMeter)
        {
            Console.Title = $"Started ...";
        }
    }
}
