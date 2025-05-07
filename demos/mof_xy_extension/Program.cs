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
 * Description : MoF(Marking on the Fly) X/Y by fly extension (above RTC6 only)
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

            // Field of view : 60mm
            var fov = 60.0;
            // RTC5,6 using 20bits resolution
            var kfactor = Math.Pow(2, 20) / fov;

            // Default (1:1) correction file
            // Field correction file path: \correction\cor_1to1.ct5
            var correctionFile = Path.Combine(Config.CorrectionPath, "cor_1to1.ct5");

            // To use IRtcMoFExtension
            // above RTC6 only  !

            // Create RTC6 controller
            var rtc = ScannerFactory.CreateRtc6(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            // Create RTC6 Ethernet controller
            //var rtc = ScannerFactory.CreateRtc6Ethernet(0, "192.168.0.100", "255.255.255.0", kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);

            // Initialize RTC controller
            success &= rtc.Initialize();
            // 50KHz, 2 usec
            success &= rtc.CtlFrequency(50 * 1000, 2);
            // 500 mm/s
            success &= rtc.CtlSpeed(100, 100);
            // Basic delays
            success &= rtc.CtlDelay(10, 100, 200, 200, 0);
            Debug.Assert(success);

            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);

            // Assign encoder scale = encoder counts/mm
            rtcMoF.EncXCountsPerMm = 2000;
            rtcMoF.EncYCountsPerMm = 2000;

            rtcMoF.OnEncoderChanged += RtcMoF_OnEncoderChanged;
            
            var rtcMoFExtension = rtc as IRtcMoFExtension;
            Debug.Assert(rtcMoFExtension != null);

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
                Console.WriteLine("Testcase for fly extension (xy and above RTC6 only)");
                Console.WriteLine("'S' : reset encoder scale");
                Console.WriteLine("'R' : encoder reset");
                Console.WriteLine("'X' : enable simulate encoder x");
                Console.WriteLine("'Y' : enable simulate encoder y");
                Console.WriteLine("'A' : enable simulate encoder x/y");
                Console.WriteLine("'D' : disable simulate encoder x/y");
                Console.WriteLine("'C' : compensate tracking error");
                Console.WriteLine("'F' : following during 10s");
                Console.WriteLine("'W' : draw circle + wait encoder + measurement");
                Console.WriteLine("'Z' : draw zigzag + wait encoder + measurement");
                Console.WriteLine("'3' : draw 3d + wait encoder + measurement");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                var timer = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.S:
                        // encoder scale = encoder counts / mm 
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
                        Console.Write("encoder x speed (mm/s): ");
                        double xspeed = Convert.ToDouble(Console.ReadLine());
                        Console.Write("encoder y speed (mm/s): ");
                        double yspeed = Convert.ToDouble(Console.ReadLine());
                        rtcMoF.CtlMofEncoderSpeed(xspeed, yspeed);
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
                    case ConsoleKey.D3:
                        MofWith3DAndWaitEncoder(laser, rtc, false);
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
            rtcMoF.CtlMofGetEncoder(out var x, out var y, out var mmX, out var mmY);
            Console.Title = $"XY: [{mmX:F3}, {mmY:F3}], ENC: {x}, {y}";
        }

        /// <summary>
        /// Scanner position movement has affected by accumulate encoder values
        /// </summary>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="externalStart">Enable external /START trigger or not</param>
        private static bool MofWithFollowOnly(ILaser laser, IRtc rtc, bool externalStart)
        {
            var rtcMoF = rtc as IRtcMoF;
            var rtcMoFExtension = rtc as IRtcMoFExtension;
            var rtcExtension = rtc as IRtcExtension;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoFExtension != null);
            Debug.Assert(rtcExtension != null);

            bool success = true;
            // Start list buffer
            success &= rtc.ListBegin(ListTypes.Single);
            // MoF begin 
            success &= rtcMoFExtension.ListMofExtBegin(
                RtcEncoderModeExtension.Mode1, 
                RtcEncoderModeExtension.Mode2, 
                0, 0, true);
            // Goes to origin
            success &= rtc.ListJumpTo(Vector2.Zero);
            
            // Laser on during 10s (comment for safety issue)
            //success &= rtc.ListLaserOn(1000 * 10);
            // or Waiting 10 secs
            success &= rtc.ListWait(1000 * 10);

            // MoF end 
            success &= rtcMoFExtension.ListMofExtEnd(Vector2.Zero);
            if (!externalStart)
            {
                // Execute now
                rtcExtension.CtlExternalControl(Rtc6ExternalControlMode.Empty);
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
                var extCtrl = Rtc6ExternalControlMode.Empty;
                extCtrl.Add(Rtc6ExternalControlMode.Bit.ExternalStart);
                extCtrl.Add(Rtc6ExternalControlMode.Bit.ExternalStartAgain);
                rtcExtension.CtlExternalControl(extCtrl);
            }
            return success;
        }
        /// <summary>
        /// Wait encoder position and draw circle
        /// </summary>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="externalStart">Enable external /START trigger or not</param>
        /// <returns></returns>
        private static bool MofWithCircleAndWaitEncoder(ILaser laser, IRtc rtc, bool externalStart)
        {
            Console.WriteLine("WARNING !!! LASER IS BUSY ...");

            var rtcMoF = rtc as IRtcMoF;
            var rtcMoFExtension = rtc as IRtcMoFExtension;
            var rtcExtension = rtc as IRtcExtension;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoFExtension != null);
            Debug.Assert(rtcExtension != null);

            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);

            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 8 channels at RTC6
            var channels = new MeasurementChannels[]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.Enc0Counter,
                 MeasurementChannels.Enc1Counter,
                 //MeasurementChannels.OutputPeriod,
                 //MeasurementChannels.PulseLength,
                 //MeasurementChannels.ExtAO1,
            };

            bool success = true;
            // Start list buffer
            success &= rtc.ListBegin(ListTypes.Single);
            // Measurement begin
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            // Draw line
            success &= rtc.ListJumpTo(new Vector2(0, 0));
            success &= rtc.ListMarkTo(new Vector2(-5, -10));

            // MoF begin
            success &= rtcMoFExtension.ListMofExtBegin(
                RtcEncoderModeExtension.Mode1, 
                RtcEncoderModeExtension.Mode2, 
                0, 0, true);

            // Draw line            
            success &= rtc.ListMarkTo(new Vector2(-10, 10));

            // Wait until encoder x over 10 mm 
            success &= rtcMoFExtension.ListMofExtWait(
                RtcEncoderModeExtension.Mode19, 
                10, 
                RtcEncoderRangeConditionsExtension.OverFlow,
                true, 
                false);

            // Draw circle
            success &= rtc.ListJumpTo(new Vector2((float)0, 0));
            success &= rtc.ListArcTo(new Vector2(5, 0), 360.0f);

            // MoF end
            success &= rtcMoFExtension.ListMofExtEnd(Vector2.Zero);
            // Measurement end
            success &= rtcMeasurement.ListMeasurementEnd();

            if (!externalStart)
            {
                // Execute now
                rtcExtension.CtlExternalControl(Rtc6ExternalControlMode.Empty);
                if (success)
                {
                    success &= rtc.ListEnd();
                    success &= rtc.ListExecute();
                    CheckMofOverflow(rtc);
                }

                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_mofext_xy.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "MoFExt XY");
            }
            else
            {
                success &= rtc.ListEnd();
                // Execute by external /START trigger 
                var extCtrl6 = Rtc6ExternalControlMode.Empty;
                extCtrl6.Add(Rtc6ExternalControlMode.Bit.ExternalStart);
                extCtrl6.Add(Rtc6ExternalControlMode.Bit.ExternalStartAgain);
                rtcExtension.CtlExternalControl(extCtrl6);
            }
            return success;
        }
        /// <summary>
        /// Wait encoder position and draw zigZag
        /// </summary>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="externalStart">Enable external /START trigger or not</param>
        /// <returns></returns>
        private static bool MofWithZigZagAndWaitEncoder(ILaser laser, IRtc rtc, bool externalStart)
        {
            Console.WriteLine("WARNING !!! LASER IS BUSY ...");

            var rtcMoF = rtc as IRtcMoF;
            var rtcMoFExtension = rtc as IRtcMoFExtension;
            var rtcExtension = rtc as IRtcExtension;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoFExtension != null);
            Debug.Assert(rtcExtension != null);

            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);

            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 8 channels at RTC6
            var channels = new MeasurementChannels[]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.Enc0Counter,
                 MeasurementChannels.Enc1Counter,
                 //MeasurementChannels.OutputPeriod,
                 //MeasurementChannels.PulseLength,
                 //MeasurementChannels.ExtAO1,
            };

            bool success = true;
            // Start list buffer
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);

            //                             |                                   
            //                             |                                   
            //                             |                                   
            //                             |            <--- MATERIAL ENC X+
            //                             |                                   
            //                             |                                   
            //                             |                
            //                             . . . . .       
            //                             .       . 
            //                             .       .         Repeat 10 times
            //                             .       .         
            // ----------------------------+-------.-------.--------
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
                success &= rtcMoFExtension.ListMofExtBegin(
                             RtcEncoderModeExtension.Mode1,
                             RtcEncoderModeExtension.Mode2,
                             0, 0, true);
                // Wait until condition has matched
                success &= rtcMoFExtension.ListMofExtWait(
                              RtcEncoderModeExtension.Mode19,
                              0,
                              RtcEncoderRangeConditionsExtension.OverFlow,
                              true,
                              false);
                success &= rtc.ListMarkTo(new Vector2(0, height));
                success &= rtc.ListMarkTo(new Vector2(width, height));
                success &= rtc.ListMarkTo(new Vector2(width, -height));
                success &= rtc.ListMarkTo(new Vector2(width * 2, -height));
                success &= rtc.ListMarkTo(new Vector2(width * 2, 0));
                // MoF end
                success &= rtcMoFExtension.ListMofExtEnd(Vector2.Zero);
            }

            success &= rtcMeasurement.ListMeasurementEnd();

            if (!externalStart)
            {
                // Execute now
                rtcExtension.CtlExternalControl(Rtc6ExternalControlMode.Empty);
                if (success)
                {
                    success &= rtc.ListEnd();
                    success &= rtc.ListExecute();
                    CheckMofOverflow(rtc);
                }

                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_mofext_xy_zigzag.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "MoFExt XY ZigZag");
            }
            else
            {
                success &= rtc.ListEnd();

                // Execute by external /START trigger 
                var extCtrl6 = Rtc6ExternalControlMode.Empty;
                extCtrl6.Add(Rtc6ExternalControlMode.Bit.ExternalStart);
                extCtrl6.Add(Rtc6ExternalControlMode.Bit.ExternalStartAgain);
                rtcExtension.CtlExternalControl(extCtrl6);
            }
            return success;
        }
        /// <summary>
        /// Wait encoder position and draw 3d hellix shape (xy and z)
        /// </summary>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="externalStart">Enable external /START trigger or not</param>
        /// <returns></returns>
        private static bool MofWith3DAndWaitEncoder(ILaser laser, IRtc rtc, bool externalStart)
        {
            Console.WriteLine("WARNING !!! LASER IS BUSY ...");

            var rtcMoF = rtc as IRtcMoF;
            var rtcMoFExtension = rtc as IRtcMoFExtension;
            var rtcExtension = rtc as IRtcExtension;
            var rtc3D = rtc as IRtc3D;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoFExtension != null);
            Debug.Assert(rtcExtension != null);
            Debug.Assert(rtc3D != null);

            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);

            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 8 channels at RTC6
            var channels = new MeasurementChannels[]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.SampleZ, //Z commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.Enc0Counter,
                 MeasurementChannels.Enc1Counter,
                 //MeasurementChannels.OutputPeriod,
                 //MeasurementChannels.PulseLength,
                 //MeasurementChannels.ExtAO1,
            };

            bool success = true;
            // Start list buffer
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtc.ListJumpTo(Vector2.Zero);

            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);

            // MoF begin
            success &= rtcMoFExtension.ListMofExtBegin(
                         RtcEncoderModeExtension.Mode1,
                         RtcEncoderModeExtension.Mode2,
                         RtcEncoderModeExtension.Mode3);
            // Wait until x > 5 mm condition has matched
            success &= rtcMoFExtension.ListMofExtWait(
                          RtcEncoderModeExtension.Mode19,
                          5,
                          RtcEncoderRangeConditionsExtension.OverFlow,
                          true,
                          false);

            // draw helix shape
            // helix height per revolution (mm) 
            float helixHeightPitchPerRev = 0.5f;
            // helix revolutions
            int heliRevolutions = 5;
            // helix radius (mm)
            float helixRadius = 5;

            for (int i = 0; i < heliRevolutions; i++)
            {
                float z = i * helixHeightPitchPerRev;
                success &= rtc3D.ListJumpTo(new Vector3(helixRadius, 0, z));
                for (float angle = 10; angle <= 360; angle += 10)
                {
                    double x = helixRadius * Math.Cos(angle * Math.PI / 180.0);
                    double y = helixRadius * Math.Sin(angle * Math.PI / 180.0);
                    success &= rtc3D.ListMarkTo(new Vector3((float)x, (float)y, z + helixHeightPitchPerRev * (angle / 360.0f)));
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }
            // MoF end
            success &= rtcMoFExtension.ListMofExtEnd(Vector3.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(false);
            }

            if (!externalStart)
            {
                // Execute now
                rtcExtension.CtlExternalControl(Rtc6ExternalControlMode.Empty);
                if (success)
                {
                    success &= rtc.ListEnd();
                    success &= rtc.ListExecute();
                    CheckMofOverflow(rtc);
                }

                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_mofext_xyz.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "MoFExt XYZ 3D");
            }
            else
            {
                success &= rtc.ListEnd();

                // Execute by external /START trigger 
                var extCtrl6 = Rtc6ExternalControlMode.Empty;
                extCtrl6.Add(Rtc6ExternalControlMode.Bit.ExternalStart);
                extCtrl6.Add(Rtc6ExternalControlMode.Bit.ExternalStartAgain);
                rtcExtension.CtlExternalControl(extCtrl6);
            }
            return success;
        }
        private static void CheckMofOverflow(IRtc rtc)
        {
            if (rtc.CtlGetStatus(RtcStatus.MofOutOfRange))
            {
                var rtc6 = rtc as Rtc6;
                var info6 = rtc6.MarkingInfo;
                Console.WriteLine($"MoF out of range: marking info= {info6.Value}");
            }
        }
    }
}
