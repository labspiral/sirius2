/*
 *                                                             ,--,      ,--,                              
 *              ,-.----.                                     ,---.'|   ,---.'|                              
 *    .--.--.   \    /  \     ,---,,-.----.      ,---,       |   | :   |   | :      ,---,           ,---,.  
 *   /  /    '. |   :    \ ,`--.' |\    /  \    '  .' \      :   : |   :   : |     '  .' \        ,'  .'  \ 
 *  |  :  /`. / |   |  .\ :|   :  :;   :    \  /  ;    '.    |   ' :   |   ' :    /  ;    '.    ,---.' .' | 
 *  ;  |  |--`  .   :  |: |:   |  '|   | .\ : :  :       \   ;   ; '   ;   ; '   :  :       \   |   |  |: | 
 *  |  :  ;_    |   |   \ :|   :  |.   : |: | :  |   /\   \  '   | |__ '   | |__ :  |   /\   \  :   :  :  / 
 *   \  \    `. |   : .   /'   '  ;|   |  \ : |  :  ' ;.   : |   | :.'||   | :.'||  :  ' ;.   : :   |    ;  
 *    `----.   \;   | |`-' |   |  ||   : .  / |  |  ;/  \   \'   :    ;'   :    ;|  |  ;/  \   \|   :     \ 
 *    __ \  \  ||   | ;    '   :  ;;   | |  \ '  :  | \  \ ,'|   |  ./ |   |  ./ '  :  | \  \ ,'|   |   . | 
 *   /  /`--'  /:   ' |    |   |  '|   | ;\  \|  |  '  '--'  ;   : ;   ;   : ;   |  |  '  '--'  '   :  '; | 
 *  '--'.     / :   : :    '   :  |:   ' | \.'|  :  :        |   ,/    |   ,/    |  :  :        |   |  | ;  
 *    `--'---'  |   | :    ;   |.' :   : :-'  |  | ,'        '---'     '---'     |  | ,'        |   :   /   
 *              `---'.|    '---'   |   |.'    `--''                              `--''          |   | ,'    
 *                `---`            `---'                                                        `----'   
 * 
 * 2023 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Control Output Laser Power 
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;

namespace Demos
{
    internal class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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

            int laserType = 1;
            Console.Write("select laser (1: D.Out8bits, 2: D.Out16bits, 3: Analog1, 4: Analog2, 5: Pulse width, 6: RS232, 7: Custom) (Default= 1) : ");
            try {
                laserType = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception) {
                laserType = 1;
            }
            double maxWatt = 20;
            ILaser laser = null;
            switch (laserType)
            {
                default:
                case 1:
                    var myLaserDOut8 = new MyLaserDOut(0, "My DOut 8Bits Laser", maxWatt);
                    myLaserDOut8.PowerControlMethod = PowerControlMethod.DigitalBits;
                    myLaserDOut8.DigitalBitsPortNo = 2; //RTC EXTENSION PORT 2 (8bits)
                    laser = myLaserDOut8;
                    break;
                case 2:
                    var myLaserDOut16 = new MyLaserDOut(0, "My DOut 16Bits Laser", maxWatt);
                    myLaserDOut16.PowerControlMethod = PowerControlMethod.DigitalBits;
                    myLaserDOut16.DigitalBitsPortNo = 1; //RTC EXTENSION PORT 1 (16bits)
                    laser = myLaserDOut16;
                    break;
                case 3:
                    var myLaserAnalog1 = new MyLaserAnalog(0, "My Analog1 Laser", maxWatt);
                    myLaserAnalog1.PowerControlMethod = PowerControlMethod.Analog;
                    myLaserAnalog1.AnalogPortNo = 1;
                    laser = myLaserAnalog1;
                    break;
                case 4:
                    var myLaserAnalog2 = new MyLaserAnalog(0, "My Analog2 Laser", maxWatt);
                    myLaserAnalog2.PowerControlMethod = PowerControlMethod.Analog;
                    myLaserAnalog2.AnalogPortNo = 2;
                    laser = myLaserAnalog2;
                    break;
                case 5:
                    var myLaserPulseWidth = new MyDutyCycle(0, "My DutyCycle Laser", maxWatt);
                    myLaserPulseWidth.PowerControlMethod = PowerControlMethod.DutyCycle;
                    laser = myLaserPulseWidth;
                    break;
                case 6:
                    var myLaserRS232 = new MyLaserRS232(0, "My RS232 Communication Laser", maxWatt);
                    laser = myLaserRS232;
                    break;
                case 7:
                    var myLaserCustom = new MyLaserCustom(0, "Custom Communication Laser", maxWatt);
                    laser = myLaserCustom;
                    break;
            }
            // Assign RTC into laser
            laser.Scanner = rtc;
            // Initialize laser
            success &= laser.Initialize();
            // Default power as 2W
            var powerControl = laser as ILaserPowerControl;
            Debug.Assert(null != powerControl);
            success &= powerControl.CtlPower(2);
            Debug.Assert(success);

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for various laser power control methods");
                Console.WriteLine("'F1' : draw circles (by 8bits digital output)");
                Console.WriteLine("'F2' : draw circles (by 16bits digital output)");
                Console.WriteLine("'F3' : draw circles (by analog1 voltage output)");
                Console.WriteLine("'F4' : draw circles (by analog2 voltage output)");
                Console.WriteLine("'F5' : draw circles (by duty cycle output)");
                Console.WriteLine("'F6' : draw circles (by rs232 communication)");
                Console.WriteLine("'F7' : draw circles (by custom control)");
                Console.WriteLine("'1' : laser signal on");
                Console.WriteLine("'0' : laser signal off");
                Console.WriteLine("'Q'  : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                double watt = maxWatt / 10; // default: 10 %
                Console.Write($"power (W) (default= {watt}, max= {laser.MaxPowerWatt}): ");
                try {
                    watt = Convert.ToDouble(Console.ReadLine());
                }
                catch(Exception) {
                }
                var sw = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.F1:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannel.ExtDO8, watt);
                        break;
                    case ConsoleKey.F2:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannel.ExtDO16, watt);
                        break;
                    case ConsoleKey.F3:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannel.ExtAO1, watt);
                        break;
                    case ConsoleKey.F4:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannel.ExtAO2, watt);
                        break;
                    case ConsoleKey.F5:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannel.PulseLength, watt);
                        break;
                    case ConsoleKey.F6:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannel.FreeVariable0, watt);
                        break;
                    case ConsoleKey.F7:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannel.FreeVariable1, watt);                        
                        break;
                    case ConsoleKey.D1:
                        rtc.CtlMoveTo(Vector2.Zero);
                        rtc.CtlLaserOn();
                        break;
                    case ConsoleKey.D0:
                        rtc.CtlLaserOff();
                        break;
                }
                Logger.Log(Logger.Type.Info, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        /// <summary>
        /// Measuremented range 
        /// EXTENSION PORT 2 (D.Out 8bits) : 0~255
        /// EXTENSION PORT 1 (D.Out 16bits) : 0~65535
        /// Analog 1/2 : 0~10 V
        /// Pulse width : usec
        /// RS232 : unsupported
        /// Custom : unsupported
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="channel"></param>
        /// <param name="watt"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private static bool DrawCircleWithMeasurement(IRtc rtc, ILaser laser, MeasurementChannel channel, double watt, double radius = 10)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var laserPowerControl = laser as ILaserPowerControl;
            Debug.Assert(laserPowerControl != null);

            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannel[4]
            {
                 MeasurementChannel.SampleX, //X commanded
                 MeasurementChannel.SampleY, //Y commanded
                 MeasurementChannel.LaserOn, //Gate signal 0/1
                 channel, 
            };

            bool success = true;
            // List begin with double buffered list
            success &= rtc.ListBegin(ListType.Auto);
            success &= laser.ListBegin();
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);            
            success &= laserPowerControl.ListPower(watt);

            success &= rtc.ListJumpTo(new Vector2((float)radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360);
            success &= rtc.ListJumpTo(Vector2.Zero);

            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= laser.ListEnd();
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, $"measurement_circle_{channel}_{watt}.txt");
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"LASER POWER BY {channel}");
            }
            return success;
        }
       
    }
}
