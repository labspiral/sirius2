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
 * Description : Digital I/O manipulation
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.IO;
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
            var rtc = ScannerFactory.CreateRtc5(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            // Create RTC6 controller
            //var rtc = ScannerFactory.CreateRtc6(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
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
                Console.WriteLine("Testcase for digital io (EXTENSION 1 PORT)");
                Console.WriteLine("'1' : read digital 16bits input");
                Console.WriteLine("'2' : write digital 16bits output");
                Console.WriteLine("'3' : write digital 16bits output with counter (list)");
                Console.WriteLine("'4' : wait io input and draw circle");
                Console.WriteLine("'5' : easy to use dio by IDInput, IDOutput");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                switch (key.Key)
                {
                    case ConsoleKey.D1 :
                        ReadDIO(rtc, laser);
                        break;
                    case ConsoleKey.D2:
                        WriteDIO(rtc, laser);
                        break;
                    case ConsoleKey.D3:
                        WriteDIOCounter(rtc, laser);
                        break;
                    case ConsoleKey.D4:
                        DrawCircleByTrigger(rtc, laser, 10, 1);
                        break;
                    case ConsoleKey.D5:
                        EasyToUseDIO(rtc, laser);
                        break;
                }
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        private static bool ReadDIO(IRtc rtc, ILaser laser)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            bool success = true;
            success &= rtc.CtlReadData<uint>(ExtensionChannels.ExtDI16, out uint bits16);
            Debug.WriteLine($"EXTENTION1 PORT DIN= {bits16:X}");
            return true;
        }
        private static bool WriteDIO(IRtc rtc, ILaser laser, uint bits16 = 0xFFFF)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            bool success = true;
            success &= rtc.CtlWriteData<uint>(ExtensionChannels.ExtDO16, bits16);
            Debug.WriteLine($"EXTENTION1 PORT DOUT= {bits16:X}");
            return true;
        }
        private static bool WriteDIOCounter(IRtc rtc, ILaser laser)
        {
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[4]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.ExtDO16, //0~65535
            };

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListType.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            // 16 bits incremental counter
            for (uint i=0; i<65536; i++)
            {
                success &= rtc.ListWriteData<uint>(ExtensionChannels.ExtDO16, i);
                success &= rtc.ListWait(0.1);
            }
            success &= rtc.ListWriteData<uint>(ExtensionChannels.ExtDO16, 0x0000);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_io_16bits_counter.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"DIO 16BITS COUNTER");
            }
            return success;
        }
        private static bool DrawCircleByTrigger(IRtc rtc, ILaser laser, float radius = 10, int bitPos = 0)
        {
            var rtcExtension = rtc as IRtcExtension;
            Debug.Assert(rtcExtension != null);

            int circleRepeats = 10;
            bool success = true;
            // List buffer with double buffered 
            success &= rtc.ListBegin(ListType.Single);

            // Wait until EXTENSTION1 PORT DIN0 goes to HIGH 
            success &= rtcExtension.ListReadExtDI16WaitUntil((uint)(0x01 << bitPos), 0x00);
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360 * circleRepeats);
            success &= rtc.ListJumpTo(Vector2.Zero);
            if (success)
            {
                success &= rtc.ListEnd();
                //To repeats
                //success &= rtcExtension.ListMoFCall(1);
                success &= rtc.ListExecute(true);
            }
            return success;
        }

        #region EXTENSION 1 PORT IO DEFINITION  (1 channel = 16 bits)
        enum DIN
        {
            D0 = 0,
            D1 = 1,
            D2 = 2,
            D3 = 3,
            D4 = 4,
            D5 = 5,
            D6 = 6,
            D7 = 7,
            D8 = 8,
            D9 = 9,
            D10 = 10,
            D11 = 11,
            D12 = 12,
            D13 = 13,
            D14 = 14,
            D15 = 15,
        }
        enum DOUT
        {
            D0 = 0,
            D1 = 1,
            D2 = 2,
            D3 = 3,
            D4 = 4,
            D5 = 5,
            D6 = 6,
            D7 = 7,
            D8 = 8,
            D9 = 9,
            D10 = 10,
            D11 = 11,
            D12 = 12,
            D13 = 13,
            D14 = 14,
            D15 = 15,
        } 
        #endregion

        private static bool EasyToUseDIO(IRtc rtc, ILaser laser)
        {
            var dIn = IOFactory.CreateInputExtension1(rtc);
            //var dIn = IOFactory.CreateInputLaserPort(rtc);
            //var dIn = IOFactory.CreateVirtualInput(0);

            var dOut = IOFactory.CreateOutputExtension1(rtc);
            //var dOut = IOFactory.CreateOutputExtension1(rtc);
            //var dOut = IOFactory.CreateOutputLaserPort(rtc);
            //var dOut = IOFactory.CreateVirtualOutput(0);

            do
            {
                dIn.Update();

                // for example,
                // if IN.D0 is HIGH, OUT.D0 set to HIGH

                // by Enum
                if (dIn.IsAOn(DIN.D0))
                {
                    Console.WriteLine("DIN.D0 is on");
                    dOut.OutOn(DOUT.D0);
                }
                else
                    dOut.OutOff(DOUT.D0);

                // by bit position
                if (dIn.IsAOn(0))
                {
                    Console.WriteLine("DIN.D0 is on");
                    dOut.OutOn(0);
                }
                else
                    dOut.OutOff(0);

                dOut.Update();

            } while (true);

            dIn.Dispose();
            dOut.Dispose();
        }
    }
}
