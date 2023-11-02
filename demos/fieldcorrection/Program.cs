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
 * Description : Scanner 2D and 3D Field Correction
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.UI;

namespace Demos
{
    class Program1
    {
        static float fov = 60.0f;

        /// <summary>
        /// RTC5,6
        /// </summary>
        static double kfactor20bits = Math.Pow(2, 20) / fov;
        /// <summary>
        /// RTC4
        /// </summary>
        static double kfactor16bits = Math.Pow(2, 16) / fov;
        static int rows = 3;
        static int cols = 3;
        static float interval = 20;
        static string sourceFile = Path.Combine(SpiralLab.Sirius2.Config.CorrectionPath, "D3_2982.ct5");
        static string targetFile = Path.Combine(SpiralLab.Sirius2.Config.CorrectionPath, "D3_2982_new.ct5");

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize sirius2 library
            SpiralLab.Sirius2.Core.Initialize();

            bool success = true;

            // Fied of view : 60mm
            var fov = 60.0;
            // RTC5,6 using 20bits resolution
            var kfactor = Math.Pow(2, 20) / fov;

            var correctionFile = sourceFile;

            // Create virtual RTC controller (without valid RTC controller)
            //var rtc = ScannerFactory.CreateVirtual(0, kfactor, correctionFile);
            // Create RTC5 controller
            //var rtc = ScannerFactory.CreateRtc5(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
            // Create RTC6 controller
            var rtc = ScannerFactory.CreateRtc6(0, kfactor, LaserModes.Yag5, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionFile);
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
                Console.WriteLine("");
                Console.WriteLine("Testcase for scanner field correction");
                Console.WriteLine("'G' : draw grids");
                Console.WriteLine("'2' : convert field correction file for 2D");
                Console.WriteLine("'3' : convert field correction file for 3D");
                Console.WriteLine("'X' : convert field correction file for 2D (by calibratitonlib)");
                Console.WriteLine("'F2' : convert field correction file for 2D by winforms");
                Console.WriteLine("'F3' : convert field correction file for 3D by winforms");
                Console.WriteLine("'Home' : select old(or original) correction (Table1)");
                Console.WriteLine("'End' : select new(or converted) correction (Table2)");
                Console.WriteLine("'Q' : quit");
                Console.Write("select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                switch (key.Key)
                {
                    case ConsoleKey.G:
                        Console.Write("Z offset (mm) (Default= 0) : ");
                        double zOffset = 0;
                        try {
                            zOffset = Convert.ToDouble(Console.ReadLine());
                        }
                        catch(Exception ){
                            zOffset = 0;
                        }
                        DrawGrids(rtc, laser, zOffset);
                        break;
                    case ConsoleKey.D2:
                        ConvertFieldCorrection2DAndSelect(rtc);
                        break;
                    case ConsoleKey.D3:
                        ConvertFieldCorrection3DAndSelect(rtc);
                        break;
                    case ConsoleKey.X:
                        ConvertFieldCorrection2DByCalLibAndSelect(rtc);
                        break;
                    case ConsoleKey.F1:
                        ConvertFieldCorrection2DByWinforms(rtc);
                        break;
                    case ConsoleKey.F2:
                        ConvertFieldCorrection3DByWinforms(rtc);
                        break;
                    case ConsoleKey.Home:
                        rtc.CtlSelectCorrection(CorrectionTables.Table1);
                        break;
                    case ConsoleKey.End:
                        rtc.CtlSelectCorrection(CorrectionTables.Table2);
                        break;
                }
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        private static bool DrawGrids(IRtc rtc, ILaser laser, double z = 0)
        {
            Debug.Assert(rtc.Is3D);
            Console.Write("WARNING !!! Press any key to mark grids ... ");
            Console.ReadKey();

            bool success = true;
            var rtc2ndHead = rtc as IRtc2ndHead;
            var offset = rtc2ndHead.PrimaryHeadBaseOffset;
            offset.Dz = z;
            rtc2ndHead.PrimaryHeadBaseOffset = offset;            

            // List buffer with single buffered
            success &= rtc.ListBegin();

            float bottom = -interval * (int)(rows / 2);
            float top = -bottom;
            float left = -interval * (int)(cols / 2);
            float right = -left;

            // Draw horizontal lines
            //___________
            //___________
            //___________   
            //___________
            for (int row = 0; row < rows; row++)
            {
                rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(0, bottom + row * interval, 0));
                success &= rtc.ListJumpTo(new Vector2(left, 0));
                success &= rtc.ListMarkTo(new Vector2(right, 0));
                rtc.MatrixStack.Pop();
            }
            // Draw vertical lines
            // |  |  |  |
            // |  |  |  |
            // |  |  |  |
            // |  |  |  |
            for (int col = 0; col < cols; col++)
            {
                rtc.MatrixStack.Push(Matrix4x4.CreateTranslation(left + col * interval, 0, 0));
                success &= rtc.ListJumpTo(new Vector2(0, bottom));
                success &= rtc.ListMarkTo(new Vector2(0, top));
                rtc.MatrixStack.Pop();
            }

            success &= rtc.ListJumpTo(Vector2.Zero);
            success &= rtc.ListEnd();
            if (success)
                success &= rtc.ListExecute(true);

            offset.Dz = 0;
            rtc2ndHead.PrimaryHeadBaseOffset = offset;
            return success;
        }

        private static bool ConvertFieldCorrection2DAndSelect(IRtc rtc)
        {
            // 9 points: 3x3
            // interval: 20 mm
            var correction = new RtcCorrection2D(kfactor20bits, rows, cols, interval, interval, sourceFile, targetFile);

            // Data for 3x3 with 20mm interval
            // z plane = 0
            correction.AddRelative(0, 0, new Vector2(-20, 20), new Vector2(0.01f, 0.02f));
            correction.AddRelative(0, 1, new Vector2(0, 20), new Vector2(-0.02f, 0.01f));
            correction.AddRelative(0, 2, new Vector2(20, 20), new Vector2(0.01f, 0.01f));
            correction.AddRelative(1, 0, new Vector2(-20, 0), new Vector2(-0.01f, 0.01f));
            correction.AddRelative(1, 1, new Vector2(0, 0), new Vector2(0.0f, 0.0f));
            correction.AddRelative(1, 2, new Vector2(20, 0), new Vector2(0.01f, 0.01f));
            correction.AddRelative(2, 0, new Vector2(-20, -20), new Vector2(0.01f, 0.01f));
            correction.AddRelative(2, 1, new Vector2(0, -20), new Vector2(0.01f, -0.01f));
            correction.AddRelative(2, 2, new Vector2(20, -20), new Vector2(0.03f, 0.02f));

            bool success = correction.Convert();
            if (success)
            {
                Console.WriteLine($"Success to convert {targetFile} 2d file. so load by Table2");
                success &= rtc.CtlLoadCorrectionFile(CorrectionTables.Table2, targetFile);
                success &= rtc.CtlSelectCorrection(CorrectionTables.Table2);
            }
            return success;
        }
        private static bool ConvertFieldCorrection3DAndSelect(IRtc rtc)
        {
            // 9 points: 3x3
            // Interval: 20 mm
            var correction = new RtcCorrection3D(kfactor20bits, rows, cols, interval, 5, 0, sourceFile, targetFile);

            // Data for 3x3 with 20mm interval
            // Z= 5mm (upper)
            correction.AddRelative(0, 0, new Vector3(-20, 20, 5), new Vector3(0.02f, -0.03f, 0));
            correction.AddRelative(0, 1, new Vector3(0, 20, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(0, 2, new Vector3(20, 20, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(1, 0, new Vector3(-20, 0, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(1, 1, new Vector3(0, 0, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(1, 2, new Vector3(20, 0, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(2, 0, new Vector3(-20, -20, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(2, 1, new Vector3(0, -20, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(2, 2, new Vector3(20, -20, 5), new Vector3(0.02f, 0.025f, 0));

            // Z= 0mm (lower)
            correction.AddRelative(0, 0, new Vector3(-20, 20, 0), new Vector3(0.01f, -0.02f, 0));
            correction.AddRelative(0, 1, new Vector3(0, 20, 0), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(0, 2, new Vector3(20, 20, 05), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(1, 0, new Vector3(-20, 0, 0), new Vector3(-0.01f, 0.01f, 0));
            correction.AddRelative(1, 1, new Vector3(0, 0, 0), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(1, 2, new Vector3(20, 0, 0), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(2, 0, new Vector3(-20, -20, 0), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(2, 1, new Vector3(0, -20, 0), new Vector3(0.01f, -0.01f, 0));
            correction.AddRelative(2, 2, new Vector3(20, -20, 0), new Vector3(0.01f, 0.01f, 0));

            bool success = correction.Convert();
            if (success)
            {
                var targetTable = CorrectionTables.Table2;
                Console.WriteLine($"Success to convert {targetFile} 3d file and loaded at {targetTable}");
                success &= rtc.CtlLoadCorrectionFile(targetTable, targetFile);
                success &= rtc.CtlSelectCorrection(targetTable, targetTable);
            }
            return success;
        }

        private static bool ConvertFieldCorrection2DByCalLibAndSelect(IRtc rtc)
        {
            // 9 points: 3x3
            // Interval: 20 mm
            var correction = new RtcCalibrationLibrary(kfactor20bits, rows, cols, interval, interval, sourceFile, targetFile);

            // Data for 3x3 with 20mm interval
            // Z plane = 0
            correction.AddRelative(0, 0, new Vector2(-20, 20), new Vector2(0.01f, 0.02f));
            correction.AddRelative(0, 1, new Vector2(0, 20), new Vector2(-0.02f, 0.01f));
            correction.AddRelative(0, 2, new Vector2(20, 20), new Vector2(0.01f, 0.01f));
            correction.AddRelative(1, 0, new Vector2(-20, 0), new Vector2(-0.01f, 0.01f));
            correction.AddRelative(1, 1, new Vector2(0, 0), new Vector2(0.0f, 0.0f));
            correction.AddRelative(1, 2, new Vector2(20, 0), new Vector2(0.01f, 0.01f));
            correction.AddRelative(2, 0, new Vector2(-20, -20), new Vector2(0.01f, 0.01f));
            correction.AddRelative(2, 1, new Vector2(0, -20), new Vector2(0.01f, -0.01f));
            correction.AddRelative(2, 2, new Vector2(20, -20), new Vector2(0.03f, 0.02f));

            bool success = correction.Convert();
            if (success)
            {
                var targetTable = CorrectionTables.Table2;
                Console.WriteLine($"Success to convert {targetFile} 2d file and loaded at {targetTable}");
                success &= rtc.CtlLoadCorrectionFile(targetTable, targetFile);
                success &= rtc.CtlSelectCorrection(targetTable);
            }
            return success;
        }
        private static bool ConvertFieldCorrection2DByWinforms(IRtc rtc)
        {
            // 9 points: 3x3
            // Interval: 20 mm
            var correction = new RtcCorrection2D(kfactor20bits, rows, cols, interval, interval, sourceFile, targetFile);

            // Data for 3x3 with 20mm interval
            correction.AddRelative(0, 0, new Vector2(-20, 20), new Vector2(0.01f, 0.02f));
            correction.AddRelative(0, 1, new Vector2(0, 20), new Vector2(-0.02f, 0.01f));
            correction.AddRelative(0, 2, new Vector2(20, 20), new Vector2(0.01f, 0.01f));
            correction.AddRelative(1, 0, new Vector2(-20, 0), new Vector2(-0.01f, 0.01f));
            correction.AddRelative(1, 1, new Vector2(0, 0), new Vector2(0.0f, 0.0f));
            correction.AddRelative(1, 2, new Vector2(20, 0), new Vector2(0.01f, 0.01f));
            correction.AddRelative(2, 0, new Vector2(-20, -20), new Vector2(0.01f, 0.01f));
            correction.AddRelative(2, 1, new Vector2(0, -20), new Vector2(0.01f, -0.01f));
            correction.AddRelative(2, 2, new Vector2(20, -20), new Vector2(0.025f, 0.02f));

            var form = new RtcCorrection2DForm(rtc, correction);
            SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection2DApply += Config_OnScannerFieldCorrection2DApply;
            form.ShowDialog();
            SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection2DApply -= Config_OnScannerFieldCorrection2DApply;
            return true;
        }

        private static bool Config_OnScannerFieldCorrection2DApply(SpiralLab.Sirius2.Winforms.UI.RtcCorrection2DForm form)
        {
            var ctFullFileName = form.RtcCorrection.TargetCorrectionFile;
            Debug.Assert(File.Exists(ctFullFileName));
            bool success = true;
            var targetTable = CorrectionTables.Table2;
            success &= form.Rtc.CtlLoadCorrectionFile(targetTable, ctFullFileName);
            success &= form.Rtc.CtlSelectCorrection(targetTable);
            Debug.Assert(success);
            return true;
        }
        private static bool ConvertFieldCorrection3DByWinforms(IRtc rtc)
        {
            // 9 points: 3x3
            // Interval: 20 mm
            var correction = new RtcCorrection3D(kfactor20bits, rows, cols, interval, 5, 0, sourceFile, targetFile);

            // Data for 3x3 with 20mm interval
            // Z= 5mm (upper)
            correction.AddRelative(0, 0, new Vector3(-20, 20, 5), new Vector3(0.02f, -0.03f, 0));
            correction.AddRelative(0, 1, new Vector3(0, 20, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(0, 2, new Vector3(20, 20, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(1, 0, new Vector3(-20, 0, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(1, 1, new Vector3(0, 0, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(1, 2, new Vector3(20, 0, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(2, 0, new Vector3(-20, -20, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(2, 1, new Vector3(0, -20, 5), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(2, 2, new Vector3(20, -20, 5), new Vector3(0.02f, 0.025f, 0));

            // Z= 0mm (lower)
            correction.AddRelative(0, 0, new Vector3(-20, 20, 0), new Vector3(0.01f, -0.02f, 0));
            correction.AddRelative(0, 1, new Vector3(0, 20, 0), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(0, 2, new Vector3(20, 20, 05), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(1, 0, new Vector3(-20, 0, 0), new Vector3(-0.01f, 0.01f, 0));
            correction.AddRelative(1, 1, new Vector3(0, 0, 0), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(1, 2, new Vector3(20, 0, 0), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(2, 0, new Vector3(-20, -20, 0), new Vector3(0.01f, 0.01f, 0));
            correction.AddRelative(2, 1, new Vector3(0, -20, 0), new Vector3(0.01f, -0.01f, 0));
            correction.AddRelative(2, 2, new Vector3(20, -20, 0), new Vector3(0.01f, 0.01f, 0));

            var form = new RtcCorrection3DForm(rtc, correction);
            SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection3DApply += Config_OnScannerFieldCorrection3DApply;
            form.ShowDialog();
            SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection3DApply -= Config_OnScannerFieldCorrection3DApply;
            return true;
        }
        private static bool Config_OnScannerFieldCorrection3DApply(SpiralLab.Sirius2.Winforms.UI.RtcCorrection3DForm form)
        {
            var ctFullFileName = form.RtcCorrection.TargetCorrectionFile;
            Debug.Assert(File.Exists(ctFullFileName));
            bool success = true;
            var targetTable = CorrectionTables.Table2;
            success &= form.Rtc.CtlLoadCorrectionFile(targetTable, ctFullFileName);
            success &= form.Rtc.CtlSelectCorrection(targetTable, targetTable);
            Debug.Assert(success);
            return true;
        }
    }
}
