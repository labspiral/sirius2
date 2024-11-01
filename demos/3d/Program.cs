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
 * Description : Control x,y and z position. Also convert scanner field correction by pointscloud at 3D surface.
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
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
            var correctionFile = Path.Combine(Config.CorrectionPath, "D3_2982.ct5");
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

            var rtc3D = rtc as IRtc3D;
            Debug.Assert(null != rtc3D);

            // RTC controller must be has 3D option 
            Debug.Assert(rtc.Is3D);

            // For control Z offset
            var rtc2ndHead = rtc as IRtc2ndHead;
            Debug.Assert(null != rtc2ndHead);

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
                Console.WriteLine("Testcase for 3d (with convert new 3d correction file)");
                Console.WriteLine("'S' : get status");
                Console.WriteLine("'R' : reset z-offset, z-defocus and status");
                Console.WriteLine("'A' : abort");
                Console.WriteLine("'O' : z-offset");
                Console.WriteLine("'D' : z-defocus");
                Console.WriteLine("'F1' : laser on during 5 secs");
                Console.WriteLine("'F2' : mark square at specific z height");
                Console.WriteLine("'F3' : mark squares with translate z height");
                Console.WriteLine("'F4' : mark helix shape");
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("'C' : reset/revert correction file");
                Console.WriteLine("'F5' : convert correction file by cylinder");
                Console.WriteLine("'F6' : convert correction file by cone");
                Console.WriteLine("'F7' : convert correction file by plane");
                Console.WriteLine("'F8' : convert correction file by points cloud");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                
                switch (key.Key)
                {
                    case ConsoleKey.S:  
                        // RTC internal status 
                        if (rtc.CtlGetStatus(RtcStatus.Busy))
                            Console.WriteLine($"Rtc is busy!");
                        if (!rtc.CtlGetStatus(RtcStatus.PowerOK))
                            Console.WriteLine($"Scanner power is not ok");
                        if (!rtc.CtlGetStatus(RtcStatus.PositionAckOK))
                            Console.WriteLine($"Scanner position is not acked");
                        if (!rtc.CtlGetStatus(RtcStatus.NoError))
                            Console.WriteLine($"Rtc status has an error");
                        break;
                    case ConsoleKey.R:
                        {
                          
                            // Reset z offset
                            var oldOffset = rtc2ndHead.PrimaryHeadUserOffset;
                            var newOffset = new SpiralLab.Sirius2.Mathematics.Offset(oldOffset.Dz, oldOffset.Dy, 0);
                            rtc2ndHead.PrimaryHeadUserOffset = newOffset;
                            // z defocus to 0
                            rtc3D.ZDefocus = 0;
                            rtc.CtlReset();
                        }
                        break;
                    case ConsoleKey.A:
                        rtc.CtlAbort();
                        break;
                    case ConsoleKey.O:
                        {
                            // Set z offset
                            Console.Write("Z offset (mm) = ");
                            float zOffset = float.Parse(Console.ReadLine());
                            var oldOffset = rtc2ndHead.PrimaryHeadUserOffset;
                            var newOffset = new SpiralLab.Sirius2.Mathematics.Offset(oldOffset.Dz, oldOffset.Dy, zOffset);
                            rtc2ndHead.PrimaryHeadUserOffset = newOffset;
                        }
                        break;
                    case ConsoleKey.D:
                        {
                            // Set z defocus
                            Console.Write("Z defocus (mm) = ");
                            rtc3D.ZDefocus = float.Parse(Console.ReadLine());
                        }
                        break;
                    case ConsoleKey.F1:
                        success &= rtc.ListBegin();
                        // Laser ON during 5 secs
                        success &= rtc.ListLaserOn(5 * 1000);
                        if (success)
                        {
                            success &= rtc.ListEnd();
                            success &= rtc.ListExecute(false);
                        }
                        break;
                    case ConsoleKey.F2:
                        {
                            Console.Write("Z height (mm) = ");
                            float zHeight = float.Parse(Console.ReadLine());
                            float halfSquareSize = 10;
                            success &= rtc.ListBegin();
                            success &= rtc3D.ListJumpTo(new Vector3(-halfSquareSize, halfSquareSize, zHeight));
                            success &= rtc3D.ListMarkTo(new Vector3(halfSquareSize, halfSquareSize, zHeight));
                            success &= rtc3D.ListJumpTo(new Vector3(halfSquareSize, -halfSquareSize, zHeight));
                            success &= rtc3D.ListJumpTo(new Vector3(-halfSquareSize, -halfSquareSize, zHeight));
                            success &= rtc3D.ListJumpTo(new Vector3(-halfSquareSize, halfSquareSize, zHeight));
                            success &= rtc3D.ListJumpTo(Vector3.Zero);
                            if (success)
                            {
                                success &= rtc.ListEnd();
                                success &= rtc.ListExecute(false);
                            }
                        }
                        break;
                    case ConsoleKey.F3:
                        {
                            Console.Write("Z height starting from (mm) = ");
                            float zHeightStart = float.Parse(Console.ReadLine());
                            Console.Write("Z height end (mm) (increase step: 1mm) = ");
                            float zHeightEnd = float.Parse(Console.ReadLine());
                            Debug.Assert(zHeightStart <= zHeightEnd);
                            float halfSquareSize = 10;
                            success &= rtc.ListBegin();
                            for (float dz = zHeightStart; dz <= zHeightEnd; dz += 1)
                            {
                                rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(0, 0, dz));
                                success &= rtc.ListJumpTo(new Vector2(-halfSquareSize, halfSquareSize));
                                success &= rtc.ListMarkTo(new Vector2(halfSquareSize, halfSquareSize));
                                success &= rtc.ListJumpTo(new Vector2(halfSquareSize, -halfSquareSize));
                                success &= rtc.ListJumpTo(new Vector2(-halfSquareSize, -halfSquareSize));
                                success &= rtc.ListJumpTo(new Vector2(-halfSquareSize, halfSquareSize));
                                rtc.MatrixStack.Pop();
                            }
                            success &= rtc.ListJumpTo(Vector2.Zero);
                            if (success)
                            {
                                success &= rtc.ListEnd();
                                success &= rtc.ListExecute(false);
                            }
                        }
                        break;
                    case ConsoleKey.F4:
                        // helix height per revolution (mm) 
                        float helixHeightPitchPerRev = 0.5f;
                        // helix revolutions
                        int heliRevolutions = 5;
                        // helix radius (mm)
                        float helixRadius = 10;
                        success &= rtc.ListBegin();
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
                                    if (!success)
                                    break;
                            }
                            if (!success)
                                break;
                        }
                        success &= rtc3D.ListJumpTo(Vector3.Zero);
                        if (success)
                        {
                            success &= rtc.ListEnd();
                            success &= rtc.ListExecute(false);
                        }
                        break;
                    case ConsoleKey.C:
                        //revert(or reset) correction table by default
                        switch (rtc.RtcType)
                        {
                            case RtcTypes.Rtc5:
                                success &= rtc.CtlSelectCorrection(CorrectionTables.Table1);
                                break;
                            case RtcTypes.Rtc6:
                                success &= rtc.CtlSelectCorrection(CorrectionTables.Table1);
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                        Logger.Log(Logger.Types.Info, $"3D calibration has reset to original(or default) correction table");
                        break;
                    case ConsoleKey.F5:
                        {
                            // Cylinder
                            var inputCtFileName = rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName;
                            var newCtFileName = string.Empty;
                            string ext = Path.GetExtension(inputCtFileName);
                            switch (ext.ToLower())
                            {
                                case ".ct5":
                                    newCtFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "correction",
                                        Path.GetFileNameWithoutExtension(inputCtFileName)) + "_cylinder.ct5";
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                            if (File.Exists(newCtFileName))
                                File.Delete(newCtFileName);
                            if (RtcCalibrationLibrary.CylinderCalibration(Vector3.Zero, Vector3.UnitX, 20, inputCtFileName, null, newCtFileName, out var returnCode))
                            {
                                LoadAndSelectCorrectionFile(rtc, newCtFileName);
                            }
                        }
                        break;
                    case ConsoleKey.F6:
                        {
                            // Cone
                            var inputCtFileName = rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName;
                            var newCtFileName = string.Empty;
                            string ext = Path.GetExtension(inputCtFileName);
                            switch (ext.ToLower())
                            {
                                case ".ct5":
                                    newCtFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "correction",
                                        Path.GetFileNameWithoutExtension(inputCtFileName)) + "_cone.ct5";
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                            if (File.Exists(newCtFileName))
                                File.Delete(newCtFileName);
                            // Positive(+) if the cone widens for larger values of x
                            // Negative(-) if it becomes more narrow
                            float radian1 = (float)(-10.0 * (Math.PI / 180.0));
                            if (RtcCalibrationLibrary.ConeCalibration(Vector3.Zero, Vector3.UnitX, 20, radian1, inputCtFileName, null, newCtFileName, out var returnCode))
                            {
                                LoadAndSelectCorrectionFile(rtc, newCtFileName);
                            }
                        }
                        break;
                    case ConsoleKey.F7:
                        {
                            // Plane
                            var inputCtFileName = rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName;
                            var newCtFileName = string.Empty;
                            string ext = Path.GetExtension(inputCtFileName);
                            switch (ext.ToLower())
                            {
                                case ".ct5":
                                    newCtFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "correction",
                                        Path.GetFileNameWithoutExtension(inputCtFileName)) + "_plane.ct5";
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                            if (File.Exists(newCtFileName))
                                File.Delete(newCtFileName);
                            // Rotated 10 degree at x,y axis
                            float radian2 = (float)(10.0 * (Math.PI / 180.0));
                            // Rotate x,y normal vector
                            var dirX = Vector3.Transform(Vector3.UnitX, Matrix4x4.CreateRotationX(radian2));
                            var dirY = Vector3.Transform(Vector3.UnitY, Matrix4x4.CreateRotationX(radian2));
                            if (RtcCalibrationLibrary.PlaneCalibration(Vector3.Zero, Vector3.Normalize(dirX), Vector3.Normalize(dirY), inputCtFileName, null, newCtFileName, out var returnCode))
                            {
                                LoadAndSelectCorrectionFile(rtc, newCtFileName);
                            }
                        }
                        break;
                    case ConsoleKey.F8:
                        {
                            // Extract points cloud (X,Y,Z surface data) data from 3D model
                            List<Vector3> pointsCloud = new List<Vector3>();
                            // You must add more points cloud by pointsClouds.Add( ... );
                            // For example
                            pointsCloud.Add(new Vector3(-20, -20, -5));
                            pointsCloud.Add(new Vector3(-20, 20, -1));
                            pointsCloud.Add(new Vector3(0, 0, 0));
                            pointsCloud.Add(new Vector3(20, 20, 1));
                            pointsCloud.Add(new Vector3(20, -20, 5));

                            var inputCtFileName = rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName;
                            string dirName = Path.GetDirectoryName(inputCtFileName);
                            string fileName = Path.GetFileNameWithoutExtension(inputCtFileName);
                            var newCtFileName = Path.Combine(dirName, $"{fileName}_PointsCloud.ct5");
                            if (RtcCalibrationLibrary.PointsCloudCalibration(pointsCloud.ToArray(), inputCtFileName, null, newCtFileName, out var returnCode))
                            {
                                LoadAndSelectCorrectionFile(rtc, newCtFileName);
                            }
                        }
                        break;
                }
            } while (true);
            rtc.Dispose();
            laser.Dispose();
        }

        private static bool LoadAndSelectCorrectionFile(IRtc rtc, string newCtFileName)
        {
            bool success = true;
            CorrectionTables targetTable = CorrectionTables.None;
            switch (rtc.RtcType)
            {
                case RtcTypes.Rtc5:
                    targetTable = CorrectionTables.Table4;
                    success &= rtc.CtlLoadCorrectionFile(targetTable, newCtFileName);
                    // select new correction table at primary/secondary head
                    success &= rtc.CtlSelectCorrection(targetTable, targetTable);
                    break;
                case RtcTypes.Rtc6:
                    targetTable = CorrectionTables.Table8;
                    success &= rtc.CtlLoadCorrectionFile(targetTable, newCtFileName);
                    // select new correction table at primary/secondary head
                    success &= rtc.CtlSelectCorrection(targetTable, targetTable);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (success)
                Logger.Log(Logger.Types.Info, $"new 3D calibration has applied: {newCtFileName} at {targetTable}");
            else
                Logger.Log(Logger.Types.Error, $"fail to load and select 3D calibration: {newCtFileName} at {targetTable}");
            return success;
        }
    }
}
