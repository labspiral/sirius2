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
 * Description : User defined(or implemented) laser sources
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

            int laserType = 1;
            Console.Write("Select laser (1: Digital 8bits, 2: Digital 16bits, 3: Analog1, 4: Analog2, 5: Frequency 6: Duty cycle, 7: RS232, 8: Digital 8bits+Guide 9: Custom, 0:External COM port) (Default= 1) : ");
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
                    var myLaserDOut8 = new MyLaserDOut(0, "My DOut 8Bits Laser", maxWatt, 0, 255);
                    myLaserDOut8.PowerControlMethod = PowerControlMethods.DigitalBits;
                    myLaserDOut8.DigitalBitsPortNo = 2; //RTC EXTENSION2 PORT (8bits)
                    laser = myLaserDOut8;
                    break;
                case 2:
                    var myLaserDOut16 = new MyLaserDOut(0, "My DOut 16Bits Laser", maxWatt, 0, 65535);
                    myLaserDOut16.PowerControlMethod = PowerControlMethods.DigitalBits;
                    myLaserDOut16.DigitalBitsPortNo = 1; //RTC EXTENSION1 PORT (16bits)
                    laser = myLaserDOut16;
                    break;
                case 3:
                    var myLaserAnalog1 = new MyLaserAnalog(0, "My Analog1 Laser", maxWatt);
                    myLaserAnalog1.PowerControlMethod = PowerControlMethods.Analog;
                    myLaserAnalog1.AnalogPortNo = 1;
                    laser = myLaserAnalog1;
                    break;
                case 4:
                    var myLaserAnalog2 = new MyLaserAnalog(0, "My Analog2 Laser", maxWatt);
                    myLaserAnalog2.PowerControlMethod = PowerControlMethods.Analog;
                    myLaserAnalog2.AnalogPortNo = 2;
                    laser = myLaserAnalog2;
                    break;
                case 5:
                    var myLaserFrequency = new MyLaserFrequency(0, "My Frequency Laser", maxWatt);
                    myLaserFrequency.PowerControlMethod = PowerControlMethods.Frequency;
                    laser = myLaserFrequency;
                    break;
                case 6:
                    var myLaserPulseWidth = new MyLaserDutyCycle(0, "My DutyCycle Laser", maxWatt);
                    myLaserPulseWidth.PowerControlMethod = PowerControlMethods.DutyCycle;
                    laser = myLaserPulseWidth;
                    break;
                case 7:
                    var myLaserRS232 = new MyLaserRS232(0, "My Internal RS232 Communication Laser", maxWatt);
                    laser = myLaserRS232;
                    break;
                case 8:
                    var myLaserDOut82 = new MyLaserDOut2(0, "My DOut 8Bits Laser", maxWatt, 0, 255);
                    myLaserDOut82.PowerControlMethod = PowerControlMethods.DigitalBits;
                    myLaserDOut82.DigitalBitsPortNo = 2; //RTC EXTENSION2 PORT (8bits)
                    laser = myLaserDOut82;
                    break;
                case 9:
                    var myLaserCustom = new MyLaserCustom(0, "My Custom Communication Laser", maxWatt);
                    laser = myLaserCustom;
                    break;
                case 0:
                    int comPortNo = 1;
                    var myLaserExternalRS232 = new MyLaserRS232External(0, "My External RS232 Laser", maxWatt, comPortNo);
                    laser = myLaserExternalRS232;
                    break;
            }
            // Assign RTC into laser
            laser.Scanner = rtc;
            // Initialize laser
            success &= laser.Initialize();
            // Default power as 2W
            var laserPowerControl = laser as ILaserPowerControl;
            Debug.Assert(null != laserPowerControl);
            success &= laserPowerControl.CtlPower(2);
            var laserGuideControl = laser as ILaserGuideControl;
            Debug.Assert(success);

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for various laser power control methods");
                Console.WriteLine("'F1' : draw circles (by 8bits digital output)");
                Console.WriteLine("'F2' : draw circles (by 16bits digital output)");
                Console.WriteLine("'F3' : draw circles (by analog1 voltage output)");
                Console.WriteLine("'F4' : draw circles (by analog2 voltage output)");
                Console.WriteLine("'F5' : draw circles (by frequency)");
                Console.WriteLine("'F6' : draw circles (by duty cycle output)");
                Console.WriteLine("'F7' : draw circles (by custom)");
                Console.WriteLine("'0' : laser off");
                Console.WriteLine("'1' : laser on (warning !!!)");
                Console.WriteLine("'2' : guide laser on");
                Console.WriteLine("'3' : guide laser off");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                double watt = maxWatt / 10; // default: 10 %
                Console.Write($"Power (W) (Default= {watt}, Max= {laser.MaxPowerWatt}): ");
                try {
                    watt = Convert.ToDouble(Console.ReadLine());
                }
                catch(Exception) {
                }
                var sw = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.F1:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannels.ExtDO8, watt);
                        break;
                    case ConsoleKey.F2:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannels.ExtDO16, watt);
                        break;
                    case ConsoleKey.F3:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannels.ExtAO1, watt);
                        break;
                    case ConsoleKey.F4:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannels.ExtAO2, watt);
                        break;
                    case ConsoleKey.F5:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannels.OutputPeriod, watt);
                        break;
                    case ConsoleKey.F6:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannels.PulseLength, watt);
                        break;
                    case ConsoleKey.F7:
                        DrawCircleWithMeasurement(rtc, laser, MeasurementChannels.None, watt);
                        break;

                    case ConsoleKey.D0:
                        rtc.CtlLaserOff();
                        break;
                    case ConsoleKey.D1:
                        rtc.CtlMoveTo(Vector2.Zero);
                        rtc.CtlLaserOn();
                        break;
                    case ConsoleKey.D2:
                        laserGuideControl?.CtlGuide(true);
                        break;
                    case ConsoleKey.D3:
                        laserGuideControl?.CtlGuide(false);
                        break;
                }
                Logger.Log(Logger.Types.Info, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        /// <summary>
        /// Measuremented range 
        /// <para>
        /// EXTENSION1 PORT (D.Out 16bits) : 0~65535 <br/>
        /// EXTENSION2 PORT (D.Out 8bits) : 0~255 <br/>
        /// Analog 1/2 : 0~10 V <br/>
        /// Pulse width : usec <br/>
        /// RS232 : unsupported <br/>
        /// Custom : unsupported <br/>
        /// </para>
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="channel"></param>
        /// <param name="watt"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private static bool DrawCircleWithMeasurement(IRtc rtc, ILaser laser, MeasurementChannels channel, double watt, double radius = 10)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var laserPowerControl = laser as ILaserPowerControl;
            Debug.Assert(laserPowerControl != null);

            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 channel, 
            };

            bool success = true;
            // List begin with double buffered list
            success &= rtc.ListBegin(ListTypes.Auto);
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
