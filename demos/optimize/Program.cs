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
 * Description : Optimize laser, scanner delays and mark speed and laser power
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
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

            bool success = true;

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

            // Create virtual laser source with max 20W
            var laser = LaserFactory.CreateVirtual(0, 20);
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

                Console.WriteLine("Testcase for optimize laser, scanner delays and mark speed and laser power");
                Console.WriteLine("'1': draw shapes for optimize laser delays");
                Console.WriteLine("'2': draw shapes for optimize scanner delays");
                Console.WriteLine("'3': draw shapes for optimize laser power and scan speed");
                Console.WriteLine("'Q': quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                var sw = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        DrawOptimizeLaserDelays(rtc, laser);
                        Logger.Log(Logger.Types.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                    case ConsoleKey.D2:
                        DrawOptimizeScannerDelays(rtc, laser);
                        Logger.Log(Logger.Types.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                    case ConsoleKey.D3:
                        DrawOptimizeLaserPowerAndSpeed(rtc, laser);
                        Logger.Log(Logger.Types.Debug, $"Processing time: {sw.Elapsed.TotalSeconds:F3} sec");
                        break;
                }
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        private static bool DrawOptimizeLaserDelays(IRtc rtc, ILaser laser, int numberOfGridPositions = 11, float gridFactor = 2.5f)
        {
            const float size = 1;

            double startOnDelay = -10; //us
            double incrementOnDelay = 2; //us
            double startOffDelay = 0; //us
            double incrementOffDelay = 2; //us

            //left bottom
            Vector2 offsetInitial = new Vector2(
                -(numberOfGridPositions - 1) / 2 * gridFactor * size,
                -(numberOfGridPositions - 1) / 2 * gridFactor * size);
            Vector2 offset = offsetInitial;

            bool success = true;
            success &= rtc.ListBegin();
            for (int x = 0; x < numberOfGridPositions; ++x)
            {
                var newLaserOffDelay = startOffDelay + x * incrementOffDelay;
                offset = new Vector2(gridFactor * size * x + offsetInitial.X, offsetInitial.Y);
                for (int y = 0; y < numberOfGridPositions; ++y)
                {
                    var newLaserOnDelay = startOnDelay + y * incrementOnDelay;
                    success &= rtc.ListDelay(newLaserOnDelay, newLaserOffDelay, jumpDelay, markDelay, polygonDelay);
                    /*
                     *  +
                     *  
                     *  L       ---     ---     ---      ---  
                     *  a        |       |       |        |   
                     *  s        |       |       |        |   
                     *  e       ---     ---     ---      ---  
                     *  r       ---     ---     ---      ---  
                     *  O        |       |       |        |   
                     *  n        |       |       |        |   
                     *  D       ---     ---     ---      ---  
                     *  e       ---     ---     ---      ---  
                     *  l        |       |       |        |   
                     *  a        |       |       |        |   
                     *  y       ---     ---     ---      ---  
                     *  T       ---     ---     ---      ---  
                     *  i        |       |       |        |   
                     *  m        |       |       |        |   
                     *  e       ---     ---     ---      ---  
                     *    
                     *  -        Laser Off Delay Time      +
                     *  
                     */
                    offset = new Vector2(offset.X, gridFactor * size * y + offsetInitial.Y);
                    success &= DrawShape(rtc, laser, offset);
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute();
            return success;
        }

        private static bool DrawOptimizeScannerDelays(IRtc rtc, ILaser laser, int numberOfGridPositions = 11, float gridFactor = 2.5f)
        {
            const float size = 1;
            double startJumpDelay = 0; //us
            double incrementJumpDelay = 20; //us
            double startMarkDelay = 0; //us
            double incrementMarkDelay = 20; //us

            //left bottom
            Vector2 offsetInitial = new Vector2(
                -(numberOfGridPositions - 1) / 2 * gridFactor * size,
                -(numberOfGridPositions - 1) / 2 * gridFactor * size);
            Vector2 offset = offsetInitial;

            bool success = true;
            success &= rtc.ListBegin();
            for (int x = 0; x < numberOfGridPositions; ++x)
            {
                var newJumpDelay = startJumpDelay + x * incrementJumpDelay;
                offset = new Vector2(gridFactor * size * x + offsetInitial.X, offsetInitial.Y);
                for (int y = 0; y < numberOfGridPositions; ++y)
                {
                    var newMarkDelay = startMarkDelay + y * incrementMarkDelay;
                    success &= rtc.ListDelay(laserOnDelay, laserOffDelay, newJumpDelay, newMarkDelay, polygonDelay);
                    /*
                     *  +
                     *  
                     *  S       ---     ---     ---      ---  
                     *  c        |       |       |        |   
                     *  a        |       |       |        |   
                     *  n       ---     ---     ---      ---  
                     *  n       ---     ---     ---      ---  
                     *  e        |       |       |        |   
                     *  r        |       |       |        |   
                     *  M       ---     ---     ---      ---  
                     *  a       ---     ---     ---      ---  
                     *  r        |       |       |        |   
                     *  k        |       |       |        |   
                     *  D       ---     ---     ---      ---  
                     *  e       ---     ---     ---      ---  
                     *  l        |       |       |        |   
                     *  a        |       |       |        |   
                     *  y       ---     ---     ---      ---  
                     *    
                     *  -        Scanner Jump Delay Time    +
                     *  
                     */
                    offset = new Vector2(offset.X, gridFactor * size * y + offsetInitial.Y);
                    success &= DrawShape(rtc, laser, offset);
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute();
            return success;
        }

        private static bool DrawOptimizeLaserPowerAndSpeed(IRtc rtc, ILaser laser, int numberOfGridPositions = 11, float gridFactor = 2.5f)
        {
            var laserPowerControl = laser as ILaserPowerControl;
            Debug.Assert(laserPowerControl != null);

            const float size = 1;
            double startPower = laser.MaxPowerWatt / 2; //starting half power (W)
            double incrementPowerWatt = (laser.MaxPowerWatt - startPower) / (numberOfGridPositions - 1); //ending max power(W)
            double startSpeed = markSpeed; //staring speed (mm/s)
            double incrementSpeed = 100; // + 100 mm/s

            //left bottom
            Vector2 offsetInitial = new Vector2(
                -(numberOfGridPositions - 1) / 2 * gridFactor * size,
                -(numberOfGridPositions - 1) / 2 * gridFactor * size);
            Vector2 offset = offsetInitial;

            bool success = true;
            success &= rtc.ListBegin();
            for (int x = 0; x < numberOfGridPositions; ++x)
            {
                var newSpeed = startSpeed + x * incrementSpeed;
                success &= rtc.ListSpeed(newSpeed, newSpeed);
                offset = new Vector2(gridFactor * size * x + offsetInitial.X, offsetInitial.Y);
                for (int y = 0; y < numberOfGridPositions; ++y)
                {
                    var newPower = startPower + y * incrementPowerWatt;
                    success &= laserPowerControl.ListPower(newPower);

                    /*
                     *  +
                     *  
                     *  L       ---     ---     ---      ---  
                     *  a        |       |       |        |   
                     *  s        |       |       |        |   
                     *  e       ---     ---     ---      ---  
                     *  r       ---     ---     ---      ---  
                     *           |       |       |        |   
                     *  P        |       |       |        |   
                     *  o       ---     ---     ---      ---  
                     *  w       ---     ---     ---      ---  
                     *  e        |       |       |        |   
                     *  r        |       |       |        |   
                     *          ---     ---     ---      ---  
                     *  W       ---     ---     ---      ---  
                     *  a        |       |       |        |   
                     *  t        |       |       |        |   
                     *  t       ---     ---     ---      ---  
                     *    
                     *  -        Scanner Mark Speed      +
                     *  
                     */

                    offset = new Vector2(offset.X, gridFactor * size * y + offsetInitial.Y);
                    success &= DrawShape(rtc, laser, offset);
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute();
            return success;
        }

        private static bool DrawShape(IRtc rtc, ILaser laser, Vector2 offset)
        {
            const float size = 1;
            const float gap = 0.1F;

            bool success = true;
            rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(offset.X, offset.Y, 0));
            success &= rtc.ListJumpTo(new Vector2(-size / 2, -size));
            success &= rtc.ListMarkTo(new Vector2(-gap / 2, -size));
            success &= rtc.ListJumpTo(new Vector2(gap / 2, -size));
            success &= rtc.ListMarkTo(new Vector2(size / 2, -size));
            success &= rtc.ListJumpTo(new Vector2(0, -size));
            success &= rtc.ListMarkTo(new Vector2(0, size));
            success &= rtc.ListJumpTo(new Vector2(size / 2, size));
            success &= rtc.ListMarkTo(new Vector2(gap / 2, size));
            success &= rtc.ListJumpTo(new Vector2(-gap / 2, size));
            success &= rtc.ListMarkTo(new Vector2(-size / 2, size));
            success &= rtc.ListJumpTo(new Vector2(-size / 2 - 0.001f, size));
            rtc.MatrixStack.Pop();
            return success;
        }
    }
}
