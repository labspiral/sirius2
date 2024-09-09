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
 * Description : Create powermap, verify and compensate by user-defined routines
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.PowerMap;
using SpiralLab.Sirius2.PowerMeter;
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

        static double maxPowerWatt = 20;

        /// <summary>
        /// Powermap file path
        /// </summary>
        static string mapFile = Path.Combine(Config.PowerMapPath, "default.map");

        [STAThread]
        static void Main(string[] args)
        {
            // Initialize sirius2 library
            SpiralLab.Sirius2.Core.Initialize();

            bool success = true;

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
            var laser = LaserFactory.CreateVirtual(0, maxPowerWatt);
            var powerControl = laser as ILaserPowerControl;

            // Create powermeter
            var powerMeter = PowerMeterFactory.CreateVirtual(0, maxPowerWatt);
            //var powerMeter = PowerMeterFactory.CreateCoherentPowerMax(0, 1);
            //var powerMeter = PowerMeterFactory.CreateOphirPhotonics(0, "SERIALNO");
            //var powerMeter = PowerMeterFactory.CreateThorlabs(0, "SERIALNO");
            success &= powerMeter.Initialize();
            powerMeter.OnStarted += PowerMeter_OnStarted;
            powerMeter.OnMeasured += PowerMeter_OnMeasured;
            powerMeter.OnStopped += PowerMeter_OnStopped;

            // Create powermap and assign powermap into laser
            var powerMap = PowerMapFactory.CreateDefault(0, $"MAP");
            //or Create user-customized powermap 
            //var powerMap = new MyPowerMap(0, $"MAP");

            if (null != powerControl)
                powerControl.PowerMap = powerMap;
            // Open powermap file if exist already
            if (File.Exists(mapFile))
                success &= PowerMapSerializer.Open(mapFile, powerMap);

            // Assign RTC into laser
            laser.Scanner = rtc;
            // Initialize laser
            success &= laser.Initialize();
            // Default power as 2W
            success &= laser.CtlPower(2);
            //success &= laser.CtlPower(2, "YOUR TARGET CATEGORY");

            Debug.Assert(success);

            powerMap.Laser = laser;
            powerMap.Rtc = rtc;
            powerMap.PowerMeter = powerMeter;

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine($"{Environment.NewLine}");
                Console.WriteLine("Testcase for powermap");
                Console.WriteLine($"-------------------------------------");
                Console.WriteLine("'1' : laser on (warning !!!)");
                Console.WriteLine("'2' : laser off");
                Console.WriteLine("'F1' : start power mapping");
                Console.WriteLine("'F2' : start power verifying");
                Console.WriteLine("'F3' : start power compensating");
                Console.WriteLine("'ESC' : stop powermap");
                Console.WriteLine("'O' : open power map");
                Console.WriteLine("'S' : save power map");
                Console.WriteLine("'A' : start power meter");
                Console.WriteLine("'B' : stop power meter");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                Console.WriteLine($"{Environment.NewLine}");
                if (key.Key == ConsoleKey.Q)
                    break;
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine($"{Environment.NewLine}");
                        double watt = maxPowerWatt / 10; // default: 10 %
                        Console.Write($"Power (W) (Max= {laser.MaxPowerWatt}): ");
                        try
                        {
                            watt = Convert.ToDouble(Console.ReadLine());
                            Console.Write($"Power (Category): ");
                            string category = Console.ReadLine();
                            success &= laser.CtlPower(2, category);
                            success &= rtc.CtlLaserOn();
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case ConsoleKey.D2:
                        rtc.CtlLaserOff();
                        break;
                    case ConsoleKey.F1:
                        powerMap.CtlReset();
                        StartPowerMap(powerMap);
                        break;
                    case ConsoleKey.F2:
                        powerMap.CtlReset();
                        StartPowerVerify(powerMap);
                        break;
                    case ConsoleKey.F3:
                        powerMap.CtlReset();
                        StartPowerCompensate(powerMap);
                        break;
                    case ConsoleKey.Escape:
                        powerMap.CtlStop();                        
                        break;
                    case ConsoleKey.A:
                        powerMeter.CtlReset();
                        powerMeter.CtlStart();
                        break;
                    case ConsoleKey.B:
                        powerMeter.CtlStop();
                        break;
                    case ConsoleKey.O:
                        // Re-open powermap from file
                        success &= PowerMapSerializer.Open(mapFile, powerMap);
                        break;                 
                    case ConsoleKey.S:
                        // Save powermap into file
                        success &= PowerMapSerializer.Save(mapFile, powerMap);
                        break;
                }
            } while (true);

            powerMeter.OnStarted -= PowerMeter_OnStarted;
            powerMeter.OnMeasured -= PowerMeter_OnMeasured;
            powerMeter.OnStopped -= PowerMeter_OnStopped;

            powerMeter.Dispose();
            rtc.Dispose();
            laser.Dispose();
        }
        private static bool StartPowerMap(IPowerMap powerMap)
        {
            // Override config values
            Config.PowerMapHoldTimeMs = 5000;
            Config.PowerMapInRangeThreshold = 5;
            // Category: 100 khz
            const string category = "100000";
            // Power range: 10 steps (0~20 W)
            return powerMap.CtlMapping(
                new string[] { category }, //100 KHz
                new double[] { 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 } //target x watts
                );
        }
        private static bool StartPowerVerify(IPowerMap powerMap)
        {
            // Override config values
            Config.PowerMapHoldTimeMs = 5000;
            Config.PowerMapInRangeThreshold = 5;
            // Category: 100 khz
            const string category = "100000";
            // Target power: 3, 5, 8 W
            return powerMap.CtlVerify(new KeyValuePair<string, double>[]
                {
                    new KeyValuePair<string, double>(category, 3 ),
                    new KeyValuePair<string, double>(category, 5 ),
                    new KeyValuePair<string, double>(category, 8 ),
                });
        }
        private static bool StartPowerCompensate(IPowerMap powerMap)
        {
            // Override config values
            Config.PowerMapHoldTimeMs = 5000;
            Config.PowerMapInRangeThreshold = 5;
            Config.PowerMapOutOfRangeThreshold = 20;
            // Category: 100 khz
            const string category = "100000";
            // Target power: 2.5, 5.5, 7.5 W
            return powerMap.CtlCompensate(new KeyValuePair<string, double>[]
                {
                    new KeyValuePair<string, double>(category, 2.5 ),
                    new KeyValuePair<string, double>(category, 5.5 ),
                    new KeyValuePair<string, double>(category, 7.5 ),
                });
        }


        private static void PowerMeter_OnStopped(IPowerMeter obj)
        {
            Console.Title = $"Stopped PowerMeter";
        }

        private static void PowerMeter_OnMeasured(IPowerMeter powerMeter, DateTime dt, double watt)
        {
            Console.Title = $"Power: {watt:F3} W";
        }

        private static void PowerMeter_OnStarted(IPowerMeter obj)
        {
            Console.Title = $"Started PowerMeter";
        }

    }
}
