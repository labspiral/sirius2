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
 * Description : MoF(Marking on the Fly) with X,Y coordinates
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

            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            // Assign encoder scale = encoder counts/mm
            rtcMoF.EncXCountsPerMm = 2000;
            rtcMoF.EncYCountsPerMm = 2000;

            rtcMoF.OnEncoderChanged += RtcMoF_OnEncoderChanged;

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
                Console.WriteLine("Testcase for processing on the fly (x,y)");
                Console.WriteLine("'S' : reset encoder scale");
                Console.WriteLine("'R' : encoder reset");
                Console.WriteLine("'X' : enable simulate encoder x");
                Console.WriteLine("'Y' : enable simulate encoder y");
                Console.WriteLine("'A' : enable simulate encoder x/y");
                Console.WriteLine("'D' : disable simulate encoder x/y");
                Console.WriteLine("'C' : compensate tracking error");
                Console.WriteLine("'F' : following only");
                Console.WriteLine("'W' : draw circle + wait encoder + measurement");
                Console.WriteLine("'Z' : draw zigzag + wait encoder + measurement");
                Console.WriteLine("'T' : draw circle + encoder compensate table");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                Console.WriteLine("WARNING !!! LASER IS BUSY ...");
                var timer = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.S:
                        // encoder scale = accumulated encoder counts / mm 
                        //rtcMoF.EncXCountsPerMm = 1000;
                        //rtcMoF.EncYCountsPerMm = 1000;
                        break;
                    case ConsoleKey.R:
                        rtcMoF.CtlMofEncoderReset();
                        break;
                    case ConsoleKey.X:
                        Console.Write("encoder x speed (mm/s): ");
                        double speedX = Convert.ToDouble(Console.ReadLine());
                        rtcMoF.CtlMofEncoderSpeed(speedX, 0);
                        break;
                    case ConsoleKey.Y:
                        Console.Write("encoder y speed (mm/s): ");
                        double speedY = Convert.ToDouble(Console.ReadLine());
                        rtcMoF.CtlMofEncoderSpeed(0, speedY);
                        break;
                    case ConsoleKey.A:
                        Console.Write("encoder xy speed (mm/s): ");
                        double speed = Convert.ToDouble(Console.ReadLine());
                        rtcMoF.CtlMofEncoderSpeed(speed, speed);
                        break;
                    case ConsoleKey.D:
                        rtcMoF.CtlMofEncoderSpeed(0, 0);
                        break;
                    case ConsoleKey.C:
                        rtcMoF.CtlMofTrackingError(150, 150);
                        break;
                    case ConsoleKey.F:
                        MofWithFollowOnly(laser, rtc, false);
                        break;
                    case ConsoleKey.W:
                        MofWithCircleAndWaitEncoder(laser, rtc, false);
                        break;
                    case ConsoleKey.Z:
                        MofWithZigZagAndWaitEncoder(laser, rtc, false);
                        break;
                    case ConsoleKey.T:
                        MofWithCompensateTable(laser, rtc, false);
                        break;
                }
                Console.WriteLine($"Processing time= {timer.ElapsedMilliseconds / 1000.0:F3}s");
            } while (true);

            rtcMoF.OnEncoderChanged -= RtcMoF_OnEncoderChanged;
            rtc.Dispose();
            laser.Dispose();
        }

        private static void RtcMoF_OnEncoderChanged(IRtcMoF rtcMoF, int encX, int encY)
        {
            //Console.Title = $"ENC0,1= {encX}, {encY}";
            rtcMoF.CtlMofGetEncoder(out var x, out var y, out var mmX, out var mmY);
            Console.Title = $"ENC X,Y= {x}, {y}, Distance X,Y= [{mmX:F3}, {mmY:F3}]";
        }

        /// <summary>
        /// Scanner position movement has affected by accumulate encoder values
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        /// <param name="externalStart"></param>
        private static bool MofWithFollowOnly(ILaser laser, IRtc rtc, bool externalStart)
        {
            var rtcMof = rtc as IRtcMoF;
            var rtcExtension = rtc as IRtcExtension;
            Debug.Assert(rtcMof != null);
            Debug.Assert(rtcExtension != null);

            bool success = true;
            // Start list buffer
            success &= rtc.ListBegin(ListType.Single);
            // MoF begin 
            success &= rtcMof.ListMofBegin(true);
            // Goes to origin
            success &= rtc.ListJumpTo(Vector2.Zero);
            
            // Laser on during 60 s (comment for safety issue)
            //success &= rtc.ListLaserOn(1000 * 60);
            // or Waiting 60 secs
            success &= rtc.ListWait(1000 * 60);

            // MoF end 
            success &= rtcMof.ListMofEnd(Vector2.Zero);
            if (!externalStart)
            {
                // Execute now
                rtcExtension.CtlExternalControl(Rtc5ExternalControlMode.Empty);
                if (success)
                {
                    success &= rtc.ListEnd();
                    success &= rtc.ListExecute();
                    CheckMofOverflow(rtc);
                }
            }
            else
            {
                success &= rtc.ListEnd();
                // Execute by external /START trigger 
                var extCtrl = Rtc5ExternalControlMode.Empty;
                extCtrl.Add(Rtc5ExternalControlMode.Bit.ExternalStart);
                extCtrl.Add(Rtc5ExternalControlMode.Bit.ExternalStartAgain);
                rtcExtension.CtlExternalControl(extCtrl);
            }
            return success;
        }
        /// <summary>
        /// Wait encoder position and draw circle
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        /// <param name="externalStart"></param>
        /// <returns></returns>
        private static bool MofWithCircleAndWaitEncoder(ILaser laser, IRtc rtc, bool externalStart)
        {
            var rtcMof = rtc as IRtcMoF;
            var rtcExtension = rtc as IRtcExtension;
            Debug.Assert(rtcMof != null);
            Debug.Assert(rtcExtension != null);

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
                 MeasurementChannels.Enc0Counter, //Converted to mm
            };
            // Max 8 channels at RTC6
            //var channels = new MeasurementChannel[8]
            //{
            //     MeasurementChannels.SampleX, //X commanded
            //     MeasurementChannels.SampleY, //Y commanded
            //     MeasurementChannels.LaserOn, //Gate signal 0/1
            //     MeasurementChannels.Enc0Counter, 
            //     MeasurementChannels.Enc1Counter,
            //     MeasurementChannels.OutputPeriod,
            //     MeasurementChannels.PulseLength,
            //     MeasurementChannels.ExtAO1,
            //};

            bool success = true;
            // Start list buffer
            success &= rtc.ListBegin( ListType.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            // Draw line
            success &= rtc.ListJumpTo(new Vector2(0, 0));
            success &= rtc.ListMarkTo(new Vector2(0, 10));

            // MoF begin
            success &= rtcMof.ListMofBegin();
            // Wait until condition has matched
            success &= rtcMof.ListMofWait(RtcEncoders.EncX, 10, RtcEncoderWaitConditions.Over);

            // Draw circle
            success &= rtc.ListJumpTo(new Vector2((float)10, 0));
            success &= rtc.ListArcTo(new Vector2(0, 0), 360.0f);

            // MoF end
            success &= rtcMof.ListMofEnd(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();

            if (!externalStart)
            {
                // Execute now
                rtcExtension.CtlExternalControl(Rtc5ExternalControlMode.Empty);
                if (success)
                {
                    success &= rtc.ListEnd();
                    success &= rtc.ListExecute();
                    CheckMofOverflow(rtc);
                }

                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_mof_xy.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "MoF XY");
            }
            else
            {
                success &= rtc.ListEnd();
                // Execute by external /START trigger 
                var extCtrl = Rtc5ExternalControlMode.Empty;
                extCtrl.Add(Rtc5ExternalControlMode.Bit.ExternalStart);
                extCtrl.Add(Rtc5ExternalControlMode.Bit.ExternalStartAgain);
                rtcExtension.CtlExternalControl(extCtrl);
            }
            return success;
        }
        /// <summary>
        /// Wait encoder position and draw zigZag
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        /// <param name="externalStart"></param>
        /// <returns></returns>
        private static bool MofWithZigZagAndWaitEncoder(ILaser laser, IRtc rtc, bool externalStart)
        {
            var rtcMof = rtc as IRtcMoF;
            var rtcExtension = rtc as IRtcExtension;
            Debug.Assert(rtcMof != null);
            Debug.Assert(rtcExtension != null);

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
                 MeasurementChannels.Enc0Counter, //Converted to mm
            };
            // Max 8 channels at RTC6
            //var channels = new MeasurementChannel[8]
            //{
            //     MeasurementChannels.SampleX, //X commanded
            //     MeasurementChannels.SampleY, //Y commanded
            //     MeasurementChannels.LaserOn, //Gate signal 0/1
            //     MeasurementChannels.Enc0Counter, 
            //     MeasurementChannels.Enc1Counter,
            //     MeasurementChannels.OutputPeriod,
            //     MeasurementChannels.PulseLength,
            //     MeasurementChannels.ExtAO1,
            //};

            bool success = true;
            // Start list buffer
            success &= rtc.ListBegin(ListType.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);

            //                             |                                   
            //                             |                                   
            //                             |                                   
            //                             |            <--- ENC X-
            //                             |                                   
            //                             |                                   
            //                             |                
            //                             . . . . .       
            //                             .       . 
            //                             .       .         Repeat 10times
            //                             .       .         
            // ----------------------------.-------.-------.--------
            //                             |       .       .
            //                             |       .       .
            //                             |       .       .
            //                             |       . . . . .
            //                             |                 
            //                             |                
            //                             |
            //                             |            ---> SCANNER X+ 
            //                             |   
            //                             |                
            //                             | 

            float width = 5;
            float height = 5;
            int repeats = 10;
            success &= rtc.ListJumpTo(Vector2.Zero);
            for (int i = 0; i < repeats; i++)
            {
                // MoF begin
                success &= rtcMof.ListMofBegin();
                // Wait until condition has matched
                success &= rtcMof.ListMofWait(RtcEncoders.EncX, 0, RtcEncoderWaitConditions.Over);
                success &= rtc.ListMarkTo(new Vector2(0, height));
                success &= rtc.ListMarkTo(new Vector2(width, height));
                success &= rtc.ListMarkTo(new Vector2(width, -height));
                success &= rtc.ListMarkTo(new Vector2(width * 2, -height));
                success &= rtc.ListMarkTo(new Vector2(width * 2, 0));
                // MoF end
                success &= rtcMof.ListMofEnd(Vector2.Zero);
            }

            success &= rtcMeasurement.ListMeasurementEnd();

            if (!externalStart)
            {
                // Execute now
                rtcExtension.CtlExternalControl(Rtc5ExternalControlMode.Empty);
                if (success)
                {
                    success &= rtc.ListEnd();
                    success &= rtc.ListExecute();
                    CheckMofOverflow(rtc);
                }

                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_mof_xy_zigzag.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "MoF XY ZigZag");
            }
            else
            {
                success &= rtc.ListEnd();
                // Execute by external /START trigger 
                var extCtrl = Rtc5ExternalControlMode.Empty;
                extCtrl.Add(Rtc5ExternalControlMode.Bit.ExternalStart);
                extCtrl.Add(Rtc5ExternalControlMode.Bit.ExternalStartAgain);
                rtcExtension.CtlExternalControl(extCtrl);
            }
            return success;
        }

        /// <summary>
        /// Draw circle + 2d encoder compensate table
        /// </summary>
        /// <param name="laser"></param>
        /// <param name="rtc"></param>
        /// <param name="externalStart"></param>
        private static bool MofWithCompensateTable(ILaser laser, IRtc rtc, bool externalStart)
        {
            var rtcMof = rtc as IRtcMoF;
            var rtcExtension = rtc as IRtcExtension;
            Debug.Assert(rtcMof != null);
            Debug.Assert(rtcExtension != null);

            bool success = true;
            // X, Y and Error dX, dY
            var encCompensate2DTable = new List<KeyValuePair<Vector2, Vector2>>
            {
                new KeyValuePair<Vector2, Vector2>(new Vector2(-50, 50), new Vector2(-0.007f, 0.009f)),
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 50), new Vector2(0.003f, 0.005f)),
                new KeyValuePair<Vector2, Vector2>(new Vector2(50, 50), new Vector2(0.004f, 0.002f)),
                new KeyValuePair<Vector2, Vector2>(new Vector2(-50, 0), new Vector2(-0.001f, 0.002f)),
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 0), new Vector2(0, 0)),
                new KeyValuePair<Vector2, Vector2>(new Vector2(50, 0), new Vector2(0.005f, 0.003f)),
                new KeyValuePair<Vector2, Vector2>(new Vector2(-50, -50), new Vector2(-0.001f, 0.002f)),
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, 50), new Vector2(0.005f, 0.008f)),
                new KeyValuePair<Vector2, Vector2>(new Vector2(50, -50), new Vector2(-0.009f, -0.008f)),
            };
            // Encoder compensate table
            success &= rtcMof.CtlMofCompensateTable(encCompensate2DTable.ToArray());
            // Clear 2d compensate table
            //success &= rtcMof.CtlMofCompensateTable(null); 

            // Start list buffer
            success &= rtc.ListBegin(ListType.Single);
            // MoF begin 
            success &= rtcMof.ListMofBegin(true);
            // Goes to 20,0
            success &= rtc.ListJumpTo(new Vector2(20, 0));

            // Draw circle * 10 times
            success &= rtc.ListArcTo(new Vector2(0, 0), 360 * 10);

            // MoFf end
            success &= rtcMof.ListMofEnd(Vector2.Zero);

            if (!externalStart)
            {
                // Execute now
                rtcExtension.CtlExternalControl(Rtc5ExternalControlMode.Empty);
                //rtcExtension.CtlExternalControl(Rtc6ExternalControlMode.Empty);
                if (success)
                {
                    success &= rtc.ListEnd();
                    success &= rtc.ListExecute();

                    CheckMofOverflow(rtc);
                }
            }
            else
            {
                success &= rtc.ListEnd();

                // Execute by external /START trigger 
                var extCtrl = Rtc5ExternalControlMode.Empty;
                extCtrl.Add(Rtc5ExternalControlMode.Bit.ExternalStart);
                extCtrl.Add(Rtc5ExternalControlMode.Bit.ExternalStartAgain);
                rtcExtension.CtlExternalControl(extCtrl);

                //var extCtrl = Rtc6ExternalControlMode.Empty;
                //extCtrl.Add(Rtc6ExternalControlMode.Bit.ExternalStart);
                //extCtrl.Add(Rtc6ExternalControlMode.Bit.ExternalStartAgain);
                //rtcExtension.CtlExternalControl(extCtrl);
            }
            return success;
        }
        

        private static void CheckMofOverflow(IRtc rtc)
        {
            if (rtc.CtlGetStatus(RtcStatus.MofOutOfRange))
            {
                if (rtc is Rtc5 rtc5)
                {
                    var info = rtc5.MarkingInfo;
                    Console.WriteLine($"MoF out of range: marking info= {info.Value}");
                }
                else if (rtc is Rtc6 rtc6)
                {
                    var info = rtc6.MarkingInfo;
                    Console.WriteLine($"MoF out of range: marking info= {info.Value}");
                }
            }
        }
    }
}
