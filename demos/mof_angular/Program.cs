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
 * Description : MoF(Marking on the Fly) Angular
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
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

            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            // Assign encoder scale = encoder counts/revolution
            rtcMoF.EncCountsPerRevolution = 3600;

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
                Console.WriteLine("Testcase for processing on the fly (rotate)");
                Console.WriteLine("'A' : reset encoder scale");
                Console.WriteLine("'R' : encoder reset");
                Console.WriteLine("'S' : enable simulate encoder");
                Console.WriteLine("'D' : disable simulate encoder");
                Console.WriteLine("'F' : following only");
                Console.WriteLine("'W' : draw circle + wait encoder + measurement");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");

                var timer = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.A:
                        // encoder scale = encoder counts / rev 
                        //rtcMoF.EncCountsPerRevolution = 3600;
                        break;
                    case ConsoleKey.R:
                        rtcMoF.CtlMofEncoderReset();
                        break;
                    case ConsoleKey.S:
                        Console.Write("encoder angular speed (°/s): ");
                        double speedAngular = Convert.ToDouble(Console.ReadLine());
                        rtcMoF.CtlMofEncoderAngularSpeed(speedAngular);
                        break;
                    case ConsoleKey.D:
                        rtcMoF.CtlMofEncoderAngularSpeed(0);
                        break;
                    case ConsoleKey.F:
                        MofWithFollowOnly(laser, rtc, false);
                        break;
                    case ConsoleKey.W:
                        MofWithCircleAndWaitEncoder(laser, rtc, false);
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
            rtcMoF.CtlMofGetAngularEncoder(out var x, out var angle);
            Console.Title = $"Angle: [{angle:F3}˚], ENC: {x}";
        }

        /// <summary>
        /// Scanner movements affected by accumulate encoder values
        /// </summary>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="externalStart">Enable external /START trigger or not</param>
        private static bool MofWithFollowOnly(ILaser laser, IRtc rtc, bool externalStart)
        {
            var rtcMoF = rtc as IRtcMoF;
            var rtcExtension = rtc as IRtcExtension;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcExtension != null);

            bool success = true;
            // Start list buffer
            success &= rtc.ListBegin(ListTypes.Single);
            var rotateCenter = new Vector2(-50, 0);

            /* global coordinate system
             * 
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |                 |--------|--------|                    
             *                                 |                 |        |        |
             *                                 |                 |        |        |       
             *                                 |                 |      (Fixed)    |    
             *  ------------------------ Rotate Center --------------- Scanner ----|
             *                                 |                 |      0 , 0      |    
             *                                 |                 |        |        |      
             *                                 |                 |        |        |
             *                                 |                 |--------|--------|                    
             *                                 |                          .          
             *                                 |                         .           
             *                                 |                        .            
             *                                 |                      .             
             *                                 |                    .                
             *                                 |                 .                   
             *                                 |             . 
             *                                 |     <- Clock Wise = Angle + = Enc + 
             *                          
             */

            success &= rtcMoF.ListMofAngularBegin(rotateCenter);

            /* new coordinate system
             * 
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |     
             *                                 |                 |--------|--------|                    
             *                                 |                 |        |        |
             *                                 |                 |        |        |       
             *                                 |                 |     (Fixed)     |    
             *  ---------------------------- 0 , 0 --------------|---- Scanner ----|
             *                                 |                 |  RotateCenter   |    
             *                                 |                 |        |        |      
             *                                 |                 |        |        |
             *                                 |                 |--------|--------|                    
             *                                 |                          .          
             *                                 |                         .           
             *                                 |                        .            
             *                                 |                      .             
             *                                 |                    .                
             *                                 |                 .                   
             *                                 |             . 
             *                                 |     <- Clock Wise = Angle + = Enc + 
             *                          
             */

            success &= rtc.ListJumpTo(new Vector2(0, 0));
            // Laser on during 60 s (comment for safety issue)
            //success &= rtc.ListLaserOn(1000 * 60);
            // or Waiting 60 secs
            success &= rtc.ListWait(1000 * 60);

            // MoF end 
            success &= rtcMoF.ListMofEnd(Vector2.Zero);

            if (!externalStart)
            {
                // Execute now
                switch (rtc.RtcType)
                {
                    case RtcTypes.Rtc5:
                        rtcExtension.CtlExternalControl(Rtc5ExternalControlMode.Empty);
                        break;
                    case RtcTypes.Rtc6:
                        rtcExtension.CtlExternalControl(Rtc6ExternalControlMode.Empty);
                        break;
                }
                if (success)
                {
                    success &= rtc.ListEnd();
                    success &= rtc.ListExecute();
                }
            }
            else
            {
                success &= rtc.ListEnd();
                // Execute by external /START trigger 
                switch (rtc.RtcType)
                {
                    case RtcTypes.Rtc5:
                        var extCtrl5 = Rtc5ExternalControlMode.Empty;
                        extCtrl5.Add(Rtc5ExternalControlMode.Bit.ExternalStart);
                        extCtrl5.Add(Rtc5ExternalControlMode.Bit.ExternalStartAgain);
                        rtcExtension.CtlExternalControl(extCtrl5);
                        break;
                    case RtcTypes.Rtc6:
                        var extCtrl6 = Rtc6ExternalControlMode.Empty;
                        extCtrl6.Add(Rtc6ExternalControlMode.Bit.ExternalStart);
                        extCtrl6.Add(Rtc6ExternalControlMode.Bit.ExternalStartAgain);
                        rtcExtension.CtlExternalControl(extCtrl6);
                        break;
                }
            }
            return success;
        }
        /// <summary>
        /// Wait encoder position and draw circle (stationary movement)
        /// </summary>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="externalStart">Enable external /START trigger or not</param>
        /// <returns></returns>
        private static bool MofWithCircleAndWaitEncoder(ILaser laser, IRtc rtc, bool externalStart)
        {
            Console.WriteLine("WARNING !!! LASER IS BUSY ...");

            var rtcMoF = rtc as IRtcMoF;
            var rtcExtension = rtc as IRtcExtension;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcExtension != null);

            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            // 10KHz Sample rate (max 100KHz)
            double sampleRateHz = 10 * 1000;
            // Max 4 channels at RTC5
            var channels = new MeasurementChannels[]
            {
                 MeasurementChannels.SampleX, //X commanded
                 MeasurementChannels.SampleY, //Y commanded
                 MeasurementChannels.LaserOn, //Gate signal 0/1
                 MeasurementChannels.Enc0Counter, //Converted to deg
            };
            // Max 8 channels at RTC6
            //var channels = new MeasurementChannels[]
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
            success &= rtc.ListBegin( ListTypes.Single);
            // Start measurement 
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);

            // Draw line
            success &= rtc.ListJumpTo(new Vector2(0, 0));
            success &= rtc.ListMarkTo(new Vector2(0, 10));

            var rotateCenter = new Vector2(-50, 0);

            /* global coordinate system
             * 
             *                                 |     <- Counter Clock Wise = Angle - = Enc -
             *                                 |              .                      
             *                                 |                 .                   
             *                                 |                    .                
             *                                 |                      .              
             *                                 |                        .             
             *                                 |                         .          
             *                                 |                          .          
             *                                 |                 |--------|--------|                    
             *                                 |                 |        |        |
             *                                 |                 |        |        |       
             *                                 |                 |        |        |    
             *  ------------------------ Rotate Center --------------- Scanner ----|
             *                                 |                 |      0 , 0      |    
             *                                 |                 |        |        |      
             *                                 |                 |        |        |
             *                                 |                 |--------|--------|                    
             *                                 |                          .          
             *                                 |                         .           
             *                                 |                        .            
             *                                 |                      .             
             *                                 |                    .                
             *                                 |                 .                   
             *                                 |             . 
             *                                 |     <- Clock Wise = Angle + = Enc + 
             *                          
             */

            success &= rtcMoF.ListMofAngularBegin(rotateCenter);

            /* new coordinate system
             * 
             *                                 |     <- Counter Clock Wise = Angle - = Enc -
             *                                 |              .                      
             *                                 |                  .                   
             *                                 |                    .                
             *                                 |                      .              
             *                                 |                        .             
             *                                 |                         .          
             *                                 |                          .          
             *                                 |                 |--------|--------|                    
             *                                 |                 |        |        |
             *                                 |                 |        |        |       
             *                                 |                 |        |        |    
             *  ---------------------------- 0 , 0 --------------|---- Scanner ----|
             *                                 |                 |  RotateCenter   |    
             *                                 |                 |        |        |      
             *                                 |                 |        |        |
             *                                 |                 |--------|--------|                    
             *                                 |                          .          
             *                                 |                         .           
             *                                 |                        .            
             *                                 |                      .             
             *                                 |                    .                
             *                                 |                 .                   
             *                                 |             . 
             *                                 |     <- Clock Wise = Angle + = Enc + 
             *                          
             */

            // Wait until condition has matched
            success &= rtcMoF.ListMofAngularWait(0, RtcEncoderWaitConditions.Over);

            // Draw circle
            success &= rtc.ListJumpTo(-rotateCenter + new Vector2(10, 0));
            success &= rtc.ListArcTo(-rotateCenter, 360.0f);

            // Wait until condition has matched
            success &= rtcMoF.ListMofAngularWait(180, RtcEncoderWaitConditions.Over);

            // Draw circle
            success &= rtc.ListJumpTo(rotateCenter + new Vector2(10, 0));
            success &= rtc.ListArcTo(rotateCenter, 360.0);

            // MoF end
            success &= rtcMoF.ListMofEnd(Vector2.Zero);
            // Measurement end
            success &= rtcMeasurement.ListMeasurementEnd();

            if (!externalStart)
            {
                // Execute now
                switch (rtc.RtcType)
                {
                    case RtcTypes.Rtc5:
                        rtcExtension.CtlExternalControl(Rtc5ExternalControlMode.Empty);
                        break;
                    case RtcTypes.Rtc6:
                        rtcExtension.CtlExternalControl(Rtc6ExternalControlMode.Empty);
                        break;
                }
                if (success)
                {
                    success &= rtc.ListEnd();
                    success &= rtc.ListExecute();
                }

                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_mof_angular.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "Angular MoF");
            }
            else
            {
                success &= rtc.ListEnd();

                switch(rtc.RtcType)
                {
                    case RtcTypes.Rtc5:
                        // Execute by external /START trigger 
                        var extCtrl5 = Rtc5ExternalControlMode.Empty;
                        extCtrl5.Add(Rtc5ExternalControlMode.Bit.ExternalStart);
                        extCtrl5.Add(Rtc5ExternalControlMode.Bit.ExternalStartAgain);
                        rtcExtension.CtlExternalControl(extCtrl5);
                        break;
                    case RtcTypes.Rtc6:
                        var extCtrl6 = Rtc6ExternalControlMode.Empty;
                        extCtrl6.Add(Rtc6ExternalControlMode.Bit.ExternalStart);
                        extCtrl6.Add(Rtc6ExternalControlMode.Bit.ExternalStartAgain);
                        rtcExtension.CtlExternalControl(extCtrl6);
                        break;
                }
            }
            return success;
        }        
    }
}
