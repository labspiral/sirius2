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
 * Description : 4x4 Matrix Operations
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner.Rtc;
using System.Windows.Forms;
using SpiralLab.Sirius2.Scanner;

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
            //var rtc = ScannerFactory.CreateVirtual(0, kfactor, correctionFile);
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
            Debug.Assert(success);

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for matrix operations");
                Console.WriteLine("'1' : draw rectangle with rotate -> translate");
                Console.WriteLine("'2' : draw rectangle with translate -> rotate");
                Console.WriteLine("'3' : draw rectangle with scale -> rotate -> translate");
                Console.WriteLine("'4' : draw rectangle with scale -> rotate -> translate (with pre-calculated matrix) + rotated scanner");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                switch (key.Key)
                {
                    case ConsoleKey.D1 :
                        DrawRectangle1(rtc, laser);
                        break;
                    case ConsoleKey.D2:
                        DrawRectangle2(rtc, laser);
                        break;
                    case ConsoleKey.D3:
                        DrawRectangle3(rtc, laser);
                        break;
                    case ConsoleKey.D4:
                        var dXyz = new System.Numerics.Vector3(10, 0, 0);
                        float angleZ = 30;
                        float scale = 1.2f;
                        DrawRectangle4(rtc, laser, dXyz, angleZ, scale); 
                        break;
                }
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }
       
        /// <summary>
        /// Rotate -> translate
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="radius"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static bool DrawRectangle1(IRtc rtc, ILaser laser, float radius = 10, float width = 5, float height = 5)
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
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            for (float angle = 0; angle < 360; angle += 10)
            {
                // 2. translate
                rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(radius, 0, 0));
                var radian = angle * Math.PI / 180.0;
                // 1. rotate
                rtc.MatrixStack.Push(Matrix4x4.CreateRotationZ((float)radian));
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |                 Rotating (CCW)
                //                        |               /
                //                        |            -^- 
                // -----------------------+---------- < + > ---------
                //                        |            -v- 
                //                        |                 
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                //                        |
                success &= rtc.ListJumpTo(new Vector2(-width / 2, height / 2));
                success &= rtc.ListMarkTo(new Vector2(width / 2, height / 2));
                success &= rtc.ListMarkTo(new Vector2(width / 2, -height / 2));
                success &= rtc.ListMarkTo(new Vector2(-width / 2, -height / 2));
                success &= rtc.ListMarkTo(new Vector2(-width / 2, height / 2));
                rtc.MatrixStack.Pop();
                rtc.MatrixStack.Pop();
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_matrix1.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"rotate z -> translate");
            }
            return success;
        }
        /// <summary>
        /// Translate -> rotate
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="radius"></param>
        /// <param name="width"></param>
        /// <param name="heigth"></param>
        /// <returns></returns>
        private static bool DrawRectangle2(IRtc rtc, ILaser laser, float radius = 10, float width = 5, float heigth = 5)
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
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            for (float angle = 0; angle < 360; angle +=10)
            {
                var radian = angle * Math.PI / 180.0;
                // 2. rotate
                rtc.MatrixStack.Push(Matrix4x4.CreateRotationZ((float)radian));
                // 1. translate
                rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(radius, 0, 0));
                //                        |  Rotating (CCW) 
                //                        |/
                //                  .     |     . 
                //               .        |        .         
                //             .          |           .
                //           .            |            . 
                //          .             |             .   
                //          .             |            --- 
                // -----------------------+---------- | + |---------
                //          .             |            --- 
                //           .            |            .       
                //            .           |           .
                //              .         |         .
                //                 .      |      .
                //                     .  |  .
                //                        |
                //                        |
                success &= rtc.ListJumpTo(new Vector2(-width / 2, heigth / 2));
                success &= rtc.ListMarkTo(new Vector2(width / 2, heigth / 2));
                success &= rtc.ListMarkTo(new Vector2(width / 2, -heigth / 2));
                success &= rtc.ListMarkTo(new Vector2(-width / 2, -heigth / 2));
                success &= rtc.ListMarkTo(new Vector2(-width / 2, heigth / 2));
                rtc.MatrixStack.Pop();
                rtc.MatrixStack.Pop();
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_matrix2.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"translate -> rotate z");
            }
            return success;
        }
        /// <summary>
        /// Scale -> rotate -> translate
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="radius"></param>
        /// <param name="width"></param>
        /// <param name="heigth"></param>
        /// <returns></returns>
        private static bool DrawRectangle3(IRtc rtc, ILaser laser, float radius = 10, float width = 5, float heigth = 5)
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
            Random random = new Random();

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            for (float angle = 0; angle < 360; angle += 10)
            {
                var radian = angle * Math.PI / 180.0;
                // 3. rotate
                rtc.MatrixStack.Push(Matrix4x4.CreateRotationZ((float)radian));
                // 2. translate
                rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(radius, 0, 0));
                // 1. scale
                var scale = random.NextDouble() * (2 - 0.5) + 0.5;
                rtc.MatrixStack.Push(Matrix4x4.CreateScale((float)scale));
                //                        |  
                //                        |  
                //                  .     |     . 
                //               .        |        .         
                //             .          |           .
                //           .            |            . 
                //          .             |             .   
                //          .             |            -^- 
                // -----------------------+---------- <   >---------
                //          .             |            -v- 
                //           .            |            .       
                //            .           |           .
                //              .         |         .
                //                 .      |      .
                //                     .  |  .
                //                        |
                success &= rtc.ListJumpTo(new Vector2(-width / 2, heigth / 2));
                success &= rtc.ListMarkTo(new Vector2(width / 2, heigth / 2));
                success &= rtc.ListMarkTo(new Vector2(width / 2, -heigth / 2));
                success &= rtc.ListMarkTo(new Vector2(-width / 2, -heigth / 2));
                success &= rtc.ListMarkTo(new Vector2(-width / 2, heigth / 2));
                rtc.MatrixStack.Pop();
                rtc.MatrixStack.Pop();
                rtc.MatrixStack.Pop();
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_matrix3.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"scale -> rotate z -> translate");
            }
            return success;
        }
        /// <summary>
        /// Scale -> rotate -> translate (with pre-calculated matrix)
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="dXyz"></param>
        /// <param name="angleZ"></param>
        /// <param name="scale"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static bool DrawRectangle4(IRtc rtc, ILaser laser, Vector3 dXyz, float angleZ, float scale, float width = 5, float height = 5)
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

            var m1 = Matrix4x4.CreateScale(scale);
            var radian = angleZ * Math.PI / 180.0;
            var m2 = Matrix4x4.CreateRotationZ((float)radian);
            var m3 = Matrix4x4.CreateTranslation(dXyz);

            // Numerics.Matrix4x4 is column major order
            // So, transform applied left to right order
            // Scale -> rotate z -> translate
            var matrix = m1 * m2 * m3;

            // If scanner rotate at 90 deg
            rtc.MatrixStack.BaseMatrix = Matrix4x4.CreateRotationZ((float)(90 * Math.PI / 180.0));
            //or 
            //var m4 = Matrix3x2.CreateRotation((float)(90 * Math.PI / 180.0));
            //rtc.CtlMatrix(ScannerHeads.Primary, m4);
            //or 
            //var head2nd = rtc as IRtc2ndHead;
            //if (head2nd != null)
            //{
            //    var offset = new SpiralLab.Sirius2.Mathematics.Offset(0, 0, 90);
            //    head2nd.PrimaryHeadBaseOffset = offset;
            //}

            bool success = true;
            // List buffer with single buffered
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
           
            // With tranform by matrix
            rtc.MatrixStack.Push(matrix);
            success &= rtc.ListJumpTo(new Vector2(-width / 2, height / 2));
            success &= rtc.ListMarkTo(new Vector2(width / 2, height / 2));
            success &= rtc.ListMarkTo(new Vector2(width / 2, -height / 2));
            success &= rtc.ListMarkTo(new Vector2(-width / 2, -height / 2));
            success &= rtc.ListMarkTo(new Vector2(-width / 2, height / 2));
            success &= rtc.ListJumpTo(Vector2.Zero);
            rtc.MatrixStack.Pop();

            // Without transform by matrix
            success &= rtc.ListJumpTo(new Vector2(-width / 2, height / 2));
            success &= rtc.ListMarkTo(new Vector2(width / 2, height / 2));
            success &= rtc.ListMarkTo(new Vector2(width / 2, -height / 2));
            success &= rtc.ListMarkTo(new Vector2(-width / 2, -height / 2));
            success &= rtc.ListMarkTo(new Vector2(-width / 2, height / 2));

            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtcMeasurement.ListMeasurementEnd();
            if (success)
            {
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_matrix4.txt");
                // Adjust laser on scale factor if need to zoom
                Config.MeasurementLaserOnFactor = 2;
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, $"scale -> rotate z -> translate (with pre-calculated matrix)");
            }

            // Revert base matrix
            rtc.MatrixStack.BaseMatrix = Matrix4x4.Identity;
            return success;
        }
    }
}
