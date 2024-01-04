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
 * Description : StreamParser
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
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
        /// <summary>
        /// Default scanner  jump speed (mm/s)
        /// </summary>
        static double jumpSpeed = 500;
        /// <summary>
        /// Default scanner mark speed (mm/s)
        /// </summary>
        static double markSpeed = 500;
        /// <summary>
        /// Scanner jump delay (usec)
        /// </summary>
        static double jumpDelay = 200;
        /// <summary>
        /// Scanner mark delay (usec)
        /// </summary>
        static double markDelay = 200;
        /// <summary>
        /// Scanner polygon delay (usec)
        /// </summary>
        static double polygonDelay = 0;
        /// <summary>
        /// Laser on delay (usec)
        /// </summary>
        static double laserOnDelay = 10;
        /// <summary>
        /// Laser off delay (usec)
        /// </summary>
        static double laserOffDelay = 10;


        [STAThread]
        static void Main(string[] args)
        {
            // Initialize sirius2 library
            SpiralLab.Sirius2.Core.Initialize();
            // Get license information
            SpiralLab.Sirius2.Core.License(out var license);
            Console.WriteLine($"License: {license.ToString()}");

            bool success = true;
        
            // RTC5,6 using 20bits resolution
            var kfactor = Math.Pow(2, 20) / fov;

            // Default (1:1) correction file
            // Field correction file path: \correction\cor_1to1.ct5
            var correctionFile = Path.Combine(Config.CorrectionPath, "cor_1to1.ct5");
            
            // RTC6 ethernet only
            var rtc = ScannerFactory.CreateRtc6Ethernet(0, "192.168.0.100", "255.255.255.0", kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            // iDRIVE scanner only

            // Initialize RTC controller
            success &= rtc.Initialize();

            // Create stream parser and connect to server
            RtcStreamParserHelper streamParserHelper = new RtcStreamParserHelper(rtc.Index, $"MyStreamParser", rtc);
            streamParserHelper.OnConnected += StreamParserHelper_OnConnected;
            streamParserHelper.OnReceived += StreamParserHelper_OnReceived;
            streamParserHelper.OnSaveChannels += StreamParserHelper_OnSaveChannels;
            streamParserHelper.OnDisconnected += StreamParserHelper_OnDisconnected;

            // Create virtual laser source with max 20W
            var laser = LaserFactory.CreateVirtual(0, 20);
            //var laser = LaserFactory.CreateVirtualAnalog(0, 20, 1);
            //var laser = LaserFactory.CreateVirtualDO16Bits(0, 20);
            //var laser = LaserFactory.CreateVirtualDO8Bits(0, 20);
            //var laser = LaserFactory.CreateVirtualDutyCycle(0, 20);

            // Assign RTC into laser
            laser.Scanner = rtc;
            // Initialize laser
            success &= laser.Initialize();
            Debug.Assert(success);

            ConsoleKeyInfo key;
            do
            {
                // 50KHz, 2 usec
                success &= rtc.CtlFrequency(50 * 1000, 2);
                // 500 mm/s
                success &= rtc.CtlSpeed(jumpSpeed, markSpeed);
                // Basic delays
                success &= rtc.CtlDelay(laserOnDelay, laserOffDelay, jumpDelay, markDelay, polygonDelay);
                // Default power as 2W
                success &= laser.CtlPower(2);

                Debug.Assert(success);

                Console.WriteLine("Testcase for stream parser");
                Console.WriteLine("'F1' : connect to stream parser");
                Console.WriteLine("'F2' : switch list dependent <-> independent");
                Console.WriteLine("'F3' : save stream parser");
                Console.WriteLine("'F4' : disconnect to stream parser");
                Console.WriteLine("'F5' : draw rectangle");
                Console.WriteLine("'F6' : draw circles");
                Console.WriteLine("'F7' : draw circle + measurement");
                Console.WriteLine("'F8' : draw line + matrix + measurement");
                Console.WriteLine("'F9' : draw circles with vary speed + measurement");
                Console.WriteLine("'Q'  : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                var sw = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.F1:
                        streamParserHelper.Connect();
                        break;
                    case ConsoleKey.F2:
                        // list dependent or list independent
                        rtc.IsStreamParserByListDependent = !rtc.IsStreamParserByListDependent;
                        break;
                    case ConsoleKey.F3:
                        var dlg = new SaveFileDialog();
                        dlg.Filter = "stream parser file (txt)|*.txt;|All files|*.*";
                        dlg.InitialDirectory = Config.MeasurementPath;
                        dlg.Title = "Save Stream Parser File";
                        dlg.FileName = "streamparser.txt";
                        if (dlg.ShowDialog() != DialogResult.OK)
                            return;
                        // Event only
                        //streamParserHelper.Save();
                        // Or Save into file and firing events
                        streamParserHelper.Save(dlg.FileName);
                        break;
                    case ConsoleKey.F4:
                        streamParserHelper.Disconnect();
                        break;
                    case ConsoleKey.F5:
                        DrawRectangles(rtc, laser);
                        Logger.Log(Logger.Types.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                    case ConsoleKey.F6:                        
                        DrawCircles(rtc, laser);
                        Logger.Log( Logger.Types.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                    case ConsoleKey.F7:
                        DrawCircleWithMeasurement(rtc, laser, 5);
                        Logger.Log(Logger.Types.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                    case ConsoleKey.F8:
                        DrawLineWithMatrixAndMeasurement(rtc, laser);
                        Logger.Log(Logger.Types.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                    case ConsoleKey.F9:
                        DrawCirclesWithVarySpeedAndMeasurement(rtc, laser);
                        Logger.Log(Logger.Types.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                }                
            } while (true);

            streamParserHelper.Disconnect();
            streamParserHelper.Dispose();
            rtc.Dispose();
            laser.Dispose();
        }

        private static bool DrawRectangles(IRtc rtc, ILaser laser, float width = 20, float height = 20)
        {
            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Single);
            success &= rtc.ListJumpTo(new Vector2(-width / 2, height / 2));
            success &= rtc.ListMarkTo(new Vector2(width / 2, height / 2));
            success &= rtc.ListMarkTo(new Vector2(width / 2, -height / 2));
            success &= rtc.ListMarkTo(new Vector2(-width / 2, -height / 2));
            success &= rtc.ListMarkTo(new Vector2(-width / 2, height / 2));
            success &= rtc.ListJumpTo(Vector2.Zero);
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }

        private static bool DrawCircles(IRtc rtc, ILaser laser, float radius = 10)
        {
            int circleRepeats = 10;
            bool success = true;
            // List buffer with double buffered 
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360 * circleRepeats);
            success &= rtc.ListJumpTo(Vector2.Zero);
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }

        private static bool DrawCircleWithMeasurement(IRtc rtc, ILaser laser, float radius = 10)
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
                 MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            bool success = true;
            // List begin with double buffered list
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels); 
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360);
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }           
            return success;
        }

        private static bool DrawLineWithMatrixAndMeasurement(IRtc rtc, ILaser laser, float length = 20)
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
                 MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
            };

            bool success = true;
            // List begin with double buffered list
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            for (double angle = 0; angle < 360; angle += 45)
            {
                var radian = angle * Math.PI / 180.0;
                rtc.MatrixStack.Push(Matrix4x4.CreateRotationZ((float)radian));
                success &= rtc.ListJumpTo(new Vector2(length / 2, 0));
                success &= rtc.ListMarkTo(new Vector2(-length / 2, 0));
                rtc.MatrixStack.Pop();
                if (!success)
                    break;
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            return success;
        }

        private static bool DrawCirclesWithVarySpeedAndMeasurement(IRtc rtc, ILaser laser, float radius = 10)
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
                 MeasurementChannels.OutputPeriod, //KHz
            };

            bool success = true;

            // List begin with double buffered list
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            // 100 mm/s
            success &= rtc.ListSpeed(100, 100);
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360);
            // 600 mm/s
            success &= rtc.ListSpeed(600, 600);
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360);
            // 2000 mm/s
            success &= rtc.ListSpeed(2000, 2000);
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360);
            success &= rtc.ListJumpTo(Vector2.Zero);

            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }            
            return success;
        }

        private static void StreamParserHelper_OnConnected(RtcStreamParserHelper rtcStreamParser)
        {
            Console.Title = "CONNECTED";
        }
        private static void StreamParserHelper_OnReceived(RtcStreamParserHelper rtcStreamParser, System.Collections.Generic.List<IntPtr> streamListBuffer)
        {
            Console.WriteLine($"stream parser data has received");
        }
        private static void StreamParserHelper_OnSaveChannels(RtcStreamParserHelper rtcStreamParser, int[] channelData)
        {
            foreach(var data in channelData)
                Console.Write($"{data} ;");
            Console.Write(Environment.NewLine);
        }
        private static void StreamParserHelper_OnDisconnected(RtcStreamParserHelper rtcStreamParser)
        {
            Console.Title = "DISCONNECTED";
        }
    }
}
