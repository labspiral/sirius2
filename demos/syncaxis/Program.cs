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
 * Description : XL-SCAN (by syncAXIS) (RTC6 + SCANahead + ACS motion)
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Numerics;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Scanner.Rtc.SyncAxis;
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

            // XML configuration file
            string configXmlFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "syncaxis", "syncAXISConfig.xml");

            // Rtc6SyncAxis support x64 runtime only !
            Debug.Assert(SpiralLab.Sirius2.Core.IsRunningPlatform64);
            // Create syncAXIS(XL-SCAN) controller
            var rtc = ScannerFactory.CreateRtc6SyncAxis(0, configXmlFileName);

            // Initialize XL-SCAN controller
            success &= rtc.Initialize();
            Debug.Assert(success);

            // 50KHz, 2 usec
            rtc.CtlFrequency(50 * 1000, 2);
            // 500 mm/s
            rtc.CtlSpeed(10, 10);

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
                Console.WriteLine("'ESC' : abort (stop)");
                Console.WriteLine("'S' : get status");
                Console.WriteLine("'R' : reset");
                Console.WriteLine("'J' : simulation mode");
                Console.WriteLine("'H' : hardware mode");
                Console.WriteLine("'F' : follow mode");
                Console.WriteLine("'U' : unfollow mode");
                Console.WriteLine("'V' : syncaxis viewer with simulation result");
                Console.WriteLine("'C' : job characteristic");
                Console.WriteLine("'O' : move to origin position (scanner and stage)");
                Console.WriteLine("'F1' : draw square 2D with scanner only");
                Console.WriteLine("'F2' : draw square 2D with stage only");
                Console.WriteLine("'F3' : draw square 2D with scanner and stage");
                Console.WriteLine("'F4' : draw circle 2D with scanner only");
                Console.WriteLine("'F5' : draw circle 2D with stage only");
                Console.WriteLine("'F6' : draw circle 2D with scanner and stage");
                Console.WriteLine("'F7' : draw for scanner calibration");
                Console.WriteLine("'F8' : draw for optimize laser delays");
                Console.WriteLine("'F9' : draw for check calibration");
                Console.WriteLine("'F10' : draw for check system delays");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        Console.WriteLine("Aborting ... ");
                        // Emergency stop
                        rtc.CtlAbort();
                        // Soft stop 
                        //rtc.CtlStop();
                        break;
                    case ConsoleKey.S:
                        if (rtc.CtlGetStatus(RtcStatus.Busy))
                            Console.WriteLine("rtc is busy now ...");
                        else
                            Console.WriteLine("rtc is not busy ...");
                        if (!rtc.CtlGetStatus(RtcStatus.NoError))
                            Console.WriteLine("rtc has error(s)");
                        if (rtc.CtlGetInternalErrMsg(out var kvErrors))
                            foreach (var kv in kvErrors)
                                Console.WriteLine($"syncaxis error: [{kv.Key}]= {kv.Value}");
                        break;
                    case ConsoleKey.R:
                        rtc.CtlReset();
                        break;
                    case ConsoleKey.O:
                        rtc.CtlMoveScannerPosition(Vector2.Zero);
                        // If multiple stages 
                        //rtc.CtlSelectStage( Stage.Stage1, CorrectionTableIndex.Table1);
                        rtc.StageMoveSpeed = 10;
                        rtc.StageMoveTimeOut = 5;
                        rtc.CtlMoveStagePosition(Vector2.Zero);
                        break;
                    case ConsoleKey.C:
                        PrintJobCharacteristic(rtc);
                        break;
                    case ConsoleKey.J:
                        rtc.CtlSimulationMode(true);
                        break;
                    case ConsoleKey.H:
                        rtc.CtlSimulationMode(false);
                        break;
                    case ConsoleKey.F:
                        rtc.CtlMotionMode(MotionModes.Follow);
                        break;
                    case ConsoleKey.U:
                        rtc.CtlMotionMode(MotionModes.Unfollow);
                        break;
                    case ConsoleKey.V:
                        SyncAxisViewer(rtc);
                        break;
                    case ConsoleKey.F1:
                        DrawSquare(rtc, laser, MotionTypes.ScannerOnly);
                        break;
                    case ConsoleKey.F2:
                        DrawSquare(rtc, laser, MotionTypes.StageOnly);
                        break;
                    case ConsoleKey.F3:
                        //rtc.BandWidth = 2.0f;
                        //rtc.Head1Offset = new Offset(5, 0, 0, 0);
                        //rtc.Head2Offset = 
                        //rtc.Head3Offset = 
                        //rtc.Head4Offset = 
                        DrawSquare(rtc, laser, MotionTypes.StageAndScanner);
                        break;
                    case ConsoleKey.F4:
                        DrawCircle(rtc, laser, MotionTypes.ScannerOnly);
                        break;
                    case ConsoleKey.F5:
                        DrawCircle(rtc, laser, MotionTypes.StageOnly);
                        break;
                    case ConsoleKey.F6:
                        //rtc.BandWidth = 2.0f;
                        //rtc.Head1Offset = new Offset(5, 0, 0, 0);
                        //rtc.Head2Offset = 
                        //rtc.Head3Offset = 
                        //rtc.Head4Offset = 
                        DrawCircle(rtc, laser, MotionTypes.StageAndScanner);
                        break;
                    case ConsoleKey.F7:
                        DrawScannerCalibration(rtc, laser);
                        break;
                    case ConsoleKey.F8:
                        DrawOptimizeLaserDelay(rtc, laser);
                        break;
                    case ConsoleKey.F9:
                        DrawCheckCalibration(rtc, laser);
                        break;
                    case ConsoleKey.F10:
                        DrawCheckSystemDelay(rtc, laser);
                        break;
                }
                Console.WriteLine(Environment.NewLine);
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        /// <summary>
        /// Draw square
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="motionType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        static bool DrawSquare(IRtc rtc, ILaser laser, MotionTypes motionType, float size = 40)
        {
            bool success = true;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtcSyncAxis != null);

            success &= rtcSyncAxis.ListBegin(motionType);
            //success &= rtc.ListFrequency(50 * 1000, 2);
            //success &= rtc.ListSpeed(100, 100);
            success &= rtc.ListJumpTo(new Vector2(-size / 2.0f, size / 2.0f));
            success &= rtc.ListMarkTo(new Vector2(size / 2.0f, size / 2.0f));
            success &= rtc.ListMarkTo(new Vector2(size / 2.0f, -size / 2.0f));
            success &= rtc.ListMarkTo(new Vector2(-size / 2.0f, -size / 2.0f));
            success &= rtc.ListMarkTo(new Vector2(-size / 2.0f, size / 2.0f));
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(false);
            return success;
        }
        /// <summary>
        /// Draw circle
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="motionType"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        static bool DrawCircle(IRtc rtc, ILaser laser, MotionTypes motionType, float radius = 20)
        {
            bool success = true;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtcSyncAxis != null);

            success &= rtcSyncAxis.ListBegin(motionType);
            //success &= rtc.ListFrequency(50 * 1000, 2);
            //success &= rtc.ListSpeed(100, 100);
            success &= rtc.ListJumpTo(new Vector2(radius, 0));
            success &= rtc.ListArcTo(Vector2.Zero, 360.0);
            success &= rtc.ListJumpTo(Vector2.Zero);
            if (success)
                success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(false);
            return success;
        }
        /// <summary>
        /// Draw circle grids for scanner field correction</para>
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="grids"></param>
        /// <param name="fieldSize"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        static bool DrawScannerCalibration(IRtc rtc, ILaser laser, int grids = 5, float fieldSize = 50, float velocity = 200)
        {
            bool success = true;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtcSyncAxis != null);

            Vector2 start = new Vector2(-fieldSize / 2, -fieldSize / 2);
            float interval = fieldSize / (grids - 1);
            float dotSize = 0.3f; //diameter = 0.3mm 

            var oldTrajectory = rtcSyncAxis.Trajectory;
            var oldMode = rtcSyncAxis.MotionMode;

            // draw 'o' by scanner
            success &= rtcSyncAxis.ListBegin(MotionTypes.ScannerOnly);
            success &= rtc.ListSpeed(velocity, velocity);
            for (int row = 0; row < grids; row++)
            {
                for (int col = 0; col < grids; col++)
                {
                    Vector2 center = start + new Vector2(interval * col, interval * row);
                    rtc.MatrixStack.Push( Matrix4x4.CreateTranslation(center.X, center.Y, 0));
                    success &= rtc.ListJumpTo(new Vector2(dotSize / 2.0f, 0));
                    success &= rtc.ListArcTo(new Vector2(0, 0), 360);
                    rtc.MatrixStack.Pop();
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(false);
            return success;
        }
        /// <summary>
        /// Optimize for Laser Delay CHECK_LASERDELAYS
        /// <code>
        /// +
        /// 
        /// L       ---     ---     ---      ---  
        /// a        |       |       |        |   
        /// s        |       |       |        |   
        /// e       ---     ---     ---      ---  
        /// r       ---     ---     ---      ---  
        /// P        |       |       |        |   
        /// r        |       |       |        |   
        /// e       ---     ---     ---      ---  
        /// T       ---     ---     ---      ---  
        /// r        |       |       |        |   
        /// i        |       |       |        |   
        /// g       ---     ---     ---      ---  
        /// g       ---     ---     ---      ---  
        /// e        |       |       |        |   
        /// r        |       |       |        |   
        /// T       ---     ---     ---      ---  
        /// i                              
        /// m      
        /// e      -  Laser Switch Offset Time  +
        /// 
        /// -
        /// </code>
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="vScanner">scanner velocity (mm/s)</param>
        /// <param name="numberOfGridPositions">11x11</param>
        /// <param name="gridFactor">grid interval (mm)</param>
        /// <returns></returns>
        static bool DrawOptimizeLaserDelay(IRtc rtc, ILaser laser, float vScanner = 500, int numberOfGridPositions = 11, float gridFactor = 2.5f)
        {
            bool success = true;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtcSyncAxis != null);

            const float size = 1;
            const float gap = 0.1F;
            double startSwitchOffset = -40; //us
            double incrementSwitchOffset = 5; //us
            double startPreTrigger = -10; //us
            double incrementPreTrigger = 2; //us

            var oldMode = rtcSyncAxis.MotionMode;
            var oldTrajectory = rtcSyncAxis.Trajectory;
            var newTrajectory = rtcSyncAxis.Trajectory;
            newTrajectory.Mark.JumpSpeed = vScanner;
            newTrajectory.Mark.MarkSpeed = vScanner;

            //left bottom
            Vector2 offsetInitial = new Vector2(
                -(numberOfGridPositions - 1) / 2 * gridFactor * size,
                -(numberOfGridPositions - 1) / 2 * gridFactor * size);
            Vector2 offset = offsetInitial;
            int gridCounter = 0;

            for (int x = 0; x < numberOfGridPositions; ++x)
            {
                newTrajectory.Mark.LaserSwitchOffsetTime = (x * incrementSwitchOffset + startSwitchOffset);
                offset = new Vector2(gridFactor * size * x + offsetInitial.X, offsetInitial.Y);

                for (int y = 0; y < numberOfGridPositions; ++y)
                {
                    newTrajectory.Mark.LaserPreTriggerTime = (y * incrementPreTrigger + startPreTrigger);
                    success &= rtcSyncAxis.CtlSetTrajectory(newTrajectory);

                    success &= rtcSyncAxis.ListBegin(MotionTypes.ScannerOnly);
                    offset = new Vector2(offset.X, gridFactor * size * y + offsetInitial.Y);
                    /*
                     *  +
                     *  
                     *  L       ---     ---     ---      ---  
                     *  a        |       |       |        |   
                     *  s        |       |       |        |   
                     *  e       ---     ---     ---      ---  
                     *  r       ---     ---     ---      ---  
                     *  P        |       |       |        |   
                     *  r        |       |       |        |   
                     *  e       ---     ---     ---      ---  
                     *  T       ---     ---     ---      ---  
                     *  r        |       |       |        |   
                     *  i        |       |       |        |   
                     *  g       ---     ---     ---      ---  
                     *  g       ---     ---     ---      ---  
                     *  e        |       |       |        |   
                     *  r        |       |       |        |   
                     *  T       ---     ---     ---      ---  
                     *  i                              
                     *  m      
                     *  e      -  Laser Switch Offset Time  +
                     *  
                     *  -
                     */
                    rtc.MatrixStack.Push( Matrix4x4.CreateTranslation(offset.X, offset.Y, 0));
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
                    success &= rtc.ListJumpTo(Vector2.Zero);
                    success &= rtc.ListEnd();
                    if (success)
                        success &= rtc.ListExecute(false);
                    gridCounter++;
                }
                if (!success)
                    break;
            }
            return success;
        }
        /// <summary>
        /// Check for Stage 2D Compensate + Scanner Field Correction
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="grids">grid counts of X,Y </param>
        /// <param name="fieldSize">field Size (mm)</param>
        /// <param name="vStage">stage velocity (mm/s)</param>
        /// <returns></returns>
        static bool DrawCheckCalibration(IRtc rtc, ILaser laser, int grids = 5, float fieldSize = 60, float vStage = 20)
        {
            bool success = true;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtcSyncAxis != null);

            Vector2 start = new Vector2(-fieldSize / 2, -fieldSize / 2);
            float interval = fieldSize / (grids - 1);
            float dotSize = 0.5f; //diameter = 0.5mm 

            var oldTrajectory = rtcSyncAxis.Trajectory;
            var oldMode = rtcSyncAxis.MotionMode;

            // draw '+' by stage
            success &= rtcSyncAxis.ListBegin(MotionTypes.StageOnly);
            success &= rtc.ListSpeed(vStage, vStage);
            // -  -  -  -  -
            // -  -  -  -  -
            // -  -  -  -  -
            // -  -  -  -  -
            // -  -  -  -  -
            for (int row = 0; row < grids; row++)
            {
                Vector2 center = start + new Vector2(0, interval * row);
                for (int col = 0; col < grids; col++)
                {
                    center = center + new Vector2(interval * col, 0);
                    rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(center.X, center.Y, 0));
                    success &= rtc.ListJumpTo(new Vector2(-dotSize / 2.0f, 0));
                    success &= rtc.ListMarkTo(new Vector2(dotSize / 2.0f, 0));
                    rtc.MatrixStack.Pop();
                }
            }
            // +  +  +  +  +
            // +  +  +  +  +
            // +  +  +  +  +
            // +  +  +  +  +
            // +  +  +  +  +
            for (int col = 0; col < grids; col++)
            {
                Vector2 center = start + new Vector2(interval * col, 0);
                for (int row = 0; row < grids; row++)
                {
                    center = center + new Vector2(0, interval * row);
                    rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(center.X, center.Y, 0));
                    success &= rtc.ListJumpTo(new Vector2(0, -dotSize / 2.0f));
                    success &= rtc.ListMarkTo(new Vector2(0, dotSize / 2.0f));
                    rtc.MatrixStack.Pop();
                }
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(true);
            if (!success)
                return false;

            // draw 'o' by scanner
            success &= rtcSyncAxis.ListBegin(MotionTypes.ScannerOnly);
            //success &= rtc.ListSpeed(velocity*5, velocity*5);
            for (int row = 0; row < grids; row++)
            {
                for (int col = 0; col < grids; col++)
                {
                    Vector2 center = start + new Vector2(interval + col, interval * row);
                    rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(center.X, center.Y, 0));
                    success &= rtc.ListJumpTo(new Vector2(dotSize / 2.0f, 0));
                    success &= rtc.ListArcTo(new Vector2(0, 0), 360);
                    rtc.MatrixStack.Pop();
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(false);
            return success;
        }
        /// <summary>
        /// Check whether Scanner and Stage are synchronous (CHECK_SYSTEMDELAYS)
        /// <code>
        ///                       |   
        ///                       |   
        ///                   /   |   
        ///                   ----------      
        ///                   \   |   
        ///          |            |          /|\
        ///          |            |           | 
        ///          |            |           | 
        /// ---------|------------|-----------|------------
        ///          |            |           |
        ///         \|/           |           |
        ///                       |
        ///                       |   \
        ///                   ----------    
        ///                       |   /
        ///                       |   
        ///                       |    
        ///                       |     
        ///                       
        /// 
        ///                       +
        /// 
        /// 
        ///                       |   
        ///                       |   
        ///                 ||||||||||||||
        ///                 ||||||||||||||   
        ///         ====          |          ====  
        ///         ====          |          ====
        ///         ====          |          ====
        ///         ====          |          ====
        /// ----------------------|-----------------------
        ///         ====          |          ====
        ///         ====          |          ====
        ///         ====          |          ====
        ///         ====          |          ====
        ///                 ||||||||||||||
        ///                 ||||||||||||||
        ///                       |   
        ///                       |    
        ///                       |    
        /// 
        /// </code>
        /// <para>
        /// Two sets of lines will be marked with high stage velocity. 
        /// The lines are marked perpendicular to the stage's motion direction.
        /// If the positions of the lines do not match in stage motion direction, please contact SCANLAB. 
        /// The test is repeated in 4 directions.
        /// </para>
        /// <para>
        /// To mark rows of lines orthogonal to mechanical motion.The lines are executed in positive and negative directions, and then repeated for all 4 spatial directions.
        /// The objective is to check whether the lines of both mechanical motion directions are collinear or whether an offset(in the direction of the mechanical motion) can be seen.
        /// In case the lines are not collinear(offset in the direction of the mechanical motion), the positioning stage motion is not perfectly synchronized with the scan device motion. 
        /// If this is the case, contact SCANLAB.An arrow indicates the mechanical direction of motion.
        /// </para>
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="laser"></param>
        /// <param name="vStage">stage velocity (mm/s)</param>
        /// <param name="rStage">stage range (mm)</param>
        /// <returns></returns>
        static bool DrawCheckSystemDelay(IRtc rtc, ILaser laser, float vStage = 100, float rStage = 60)
        {
            bool success = true;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtcSyncAxis != null);

            float v_aLimit = (float)Math.Sqrt(rStage / 2.0 * 0.42 * vStage * 10);
            vStage = vStage < v_aLimit ? vStage : v_aLimit;
            float jumpSpeed = 4 * vStage;
            float markSpeed = jumpSpeed;
            float lineLength = 3;

            var oldMode = rtcSyncAxis.MotionMode;
            var oldTrajectory = rtcSyncAxis.Trajectory;
            var newTrajectory = rtcSyncAxis.Trajectory;
            newTrajectory.Mark.JumpSpeed = jumpSpeed;
            newTrajectory.Mark.MarkSpeed = markSpeed;

            success &= rtcSyncAxis.CtlSetTrajectory(newTrajectory);
            float offsetY = 0;
            offsetY = 5 * lineLength - 2 * lineLength;
            success &= rtcSyncAxis.ListBegin(MotionTypes.StageAndScanner);
            for (int i = 0; i < 4; ++i)
            {
                //arrow
                //                        . 
                //                        .
                //                        .
                //                        .
                //                        .                    \
                //                        .                     \
                //                        .                      \
                //                        .                       \
                //  ------------------------------------------------
                //  -5                    0                       /  5
                //                        .                      /
                //                        .                     /
                //                        .                    /
                //                        .
                //                        .
                //                        .
                //                        .
                //

                float angle = i * 90;
                rtc.MatrixStack.Push(Matrix4x4.CreateRotationZ((float)(angle * Math.PI / 180.0)));
                rtc.MatrixStack.Push(Matrix4x4.CreateTranslation( 0, -offsetY, 0));
                success &= rtc.ListJumpTo(new Vector2(-5, 0));
                success &= rtc.ListMarkTo(new Vector2(5, 0));
                success &= rtc.ListJumpTo(new Vector2(3, 2));
                success &= rtc.ListMarkTo(new Vector2(5, 0));
                success &= rtc.ListMarkTo(new Vector2(3, -2));
                success &= rtc.ListJumpTo(new Vector2(0, 0));
                rtc.MatrixStack.Pop();
                rtc.MatrixStack.Pop();
                if (!success)
                    break;
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            if (success)
                success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(true);
            if (!success)
                return false;

            offsetY = 5 * lineLength;
            const int totalNumberOfLines = 20;
            const float increment = 1;
            success &= rtcSyncAxis.ListBegin( MotionTypes.StageAndScanner);
            for (int i = 0; i < 4; ++i)
            {
                //line block
                //
                //
                //              |   |    |   |    |   |    |   |    |   |
                //              |   |    |   |    |   |    |   |    |   |   
                //   ...        |   |    |   |    |   |    |   |    |   |   ...
                //              |   |    |   |    |   |    |   |    |   |
                // 
                //
                float angle = i * 90;
                rtc.MatrixStack.Push(Matrix4x4.CreateRotationZ((float)(angle * Math.PI / 180.0)));
                rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(0, -offsetY, 0));
                success &= rtc.ListSpeed(vStage, markSpeed);
                success &= rtc.ListJumpTo(new Vector2(-rStage, 0));
                double usec = newTrajectory.Mark.LaserMinOffTime + (newTrajectory.Mark.LaserPreTriggerTime > 0 ? newTrajectory.Mark.LaserPreTriggerTime : 0);
                success &= rtc.ListWait((float)usec * 1000.0f);
                success &= rtc.ListJumpTo(new Vector2(-rStage, lineLength));
                success &= rtc.ListWait((float)usec * 1000.0f);
                success &= rtc.ListJumpTo(new Vector2(- (totalNumberOfLines / 2.0f) * increment, lineLength));
                success &= rtc.ListSpeed(jumpSpeed, markSpeed);
                for (int lineNumber = 0; lineNumber <= totalNumberOfLines; lineNumber += 2)
                {
                    success &= rtc.ListJumpTo(new Vector2((lineNumber - totalNumberOfLines / 2.0f) * increment, lineLength));
                    success &= rtc.ListMarkTo(new Vector2((lineNumber - totalNumberOfLines / 2.0f) * increment, 0.1f));
                    if (lineNumber + 1 <= totalNumberOfLines)
                    {
                        success &= rtc.ListJumpTo(new Vector2(lineNumber + 1 - totalNumberOfLines / 2.0f) * increment, 0.1f);
                        success &= rtc.ListMarkTo(new Vector2(lineNumber + 1 - totalNumberOfLines / 2.0f) * increment, lineLength);
                    }
                }
                success &= rtc.ListSpeed(vStage, markSpeed);
                success &= rtc.ListJumpTo(new Vector2(rStage, lineLength));
                success &= rtc.ListWait((float)usec * 1000.0f);
                success &= rtc.ListJumpTo(new Vector2(rStage, -lineLength));
                success &= rtc.ListWait((float)usec * 1000.0f);
                success &= rtc.ListJumpTo(new Vector2((totalNumberOfLines / 2.0f) * increment, -lineLength));
                success &= rtc.ListSpeed(jumpSpeed, markSpeed);
                for (int lineNumber = totalNumberOfLines; lineNumber >= 0; lineNumber -= 2)
                {
                    success &= rtc.ListJumpTo(new Vector2((lineNumber - totalNumberOfLines / 2.0f) * increment, -lineLength));
                    success &= rtc.ListMarkTo(new Vector2((lineNumber - totalNumberOfLines / 2.0f) * increment, -0.1f));
                    if (lineNumber - 1 >= 0)
                    {
                        success &= rtc.ListJumpTo(new Vector2((lineNumber - 1 - totalNumberOfLines / 2.0f) * increment, -0.1f));
                        success &= rtc.ListMarkTo(new Vector2((lineNumber - 1 - totalNumberOfLines / 2.0f) * increment, -lineLength));
                    }
                }
                success &= rtc.ListSpeed(vStage, markSpeed);
                success &= rtc.ListJumpTo(new Vector2(-rStage, -lineLength));
                success &= rtc.ListWait((float)usec * 1000.0f);
                rtc.MatrixStack.Pop();
                rtc.MatrixStack.Pop();
                if (!success)
                    break;
            }
            success &= rtc.ListJumpTo(Vector2.Zero);
            if (success)
                success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(false);
            //rtcSyncAxis.CtlSetTrajectory(oldTrajectory);
            //rtcSyncAxis.CtlMotionMode(oldMode);
            return success;
        }

        /// <summary>
        /// Execute syncAXIS Viewer for viewing simulated output log file
        /// </summary>
        /// <param name="rtcSyncAxis"></param>
        static void SyncAxisViewer(IRtcSyncAxis rtcSyncAxis)
        {
            var exeFileName = Config.SyncAxisViewerProgramPath;
            string simulatedFileName = Path.Combine(Config.SyncAxisSimulateFilePath, rtcSyncAxis.SimulationFileName);
            if (File.Exists(simulatedFileName))
            {
                Console.WriteLine($"Trying to open syncAXIS Viewer: {simulatedFileName}");
                Task.Run(() =>
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "syncaxis", "Tools", "syncAXIS_Viewer");
                    startInfo.CreateNoWindow = false;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = Config.SyncAxisViewerProgramPath;
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;
                    startInfo.Arguments = "-a";//string.Empty;
                    if (!string.IsNullOrEmpty(simulatedFileName))
                        startInfo.Arguments = Path.Combine(Config.SyncAxisSimulateFilePath, simulatedFileName);
                    try
                    {
                        using (var proc = Process.Start(startInfo))
                        {
                            proc.WaitForExit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(Logger.Types.Error, ex.Message);
                    }
                });
            }
        }
        /// <summary>
        /// Print last executed Job's Characteristic
        /// </summary>
        /// <param name="rtc"></param>
        static void PrintJobCharacteristic(Rtc6SyncAxis rtc)
        {
            int counts = rtc.JobHistory.Length;
            if (counts > 0)
            {
                var lastJob = rtc.JobHistory[counts - 1] as JobSyncAxis;
                Console.WriteLine($"Job ID: [{lastJob.ID}]. Name= {lastJob.Name}. Result= {lastJob.Result}. Exec Time= {lastJob.ExecutionTime}s. Started= {lastJob.StartTime}. Ended= {lastJob.EndTime}");
                Console.WriteLine($"Scanner Utilization: {lastJob.UtilizedScanner}");
                Console.WriteLine($"Scanner Position Max: {lastJob.Characteristic.Scanner.ScanPosMax} mm");
                Console.WriteLine($"Scanner Velocity Max: {lastJob.Characteristic.Scanner.ScanVelMax} mm/s");
                Console.WriteLine($"Scanner Accelation Max: {lastJob.Characteristic.Scanner.ScanAccMax} (mm/s²)");
                Console.WriteLine($"Stage Position Max: {lastJob.Characteristic.Stage.StagePosMax} mm");
                Console.WriteLine($"Stage Velocity Max: {lastJob.Characteristic.Stage.StageVelMax} mm/s");
                Console.WriteLine($"Stage Accelation Max: {lastJob.Characteristic.Stage.StageAccMax} (mm/s²)");
                Console.WriteLine($"Stage Jerk Max: {lastJob.Characteristic.Stage.StageJerkMax} (mm/s³)");
                for (int i = 0; i < rtc.StageCounts; i++)
                    Console.WriteLine($"Stage{i + 1} Utilization: {lastJob.UtilizedStages[i]}");
            }
        }
    }
}
