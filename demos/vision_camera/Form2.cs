/*
 * 
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
 * Description : Example custom vision camera
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Vision;
using SpiralLab.Sirius2.Vision.Camera;
using SpiralLab.Sirius2.Vision.Inspector;
using SpiralLab.Sirius2.Vision.Process;

namespace Demos
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            // Set language
            VisionHelper.SetLanguage();

            InitializeComponent();

            this.Shown += Form2_Shown;
            this.FormClosing += Form2_FormClosing;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            var camera = visionEditorDisp1.Camera;
            var inspector = visionEditorDisp1.Inspector;
            visionEditorDisp1.Camera = null;
            visionEditorDisp1.Inspector = null;

            VisionHelper.DestroyDevices(camera, inspector);
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            // Initialize sirius2 library
            VisionHelper.Initialize();

            bool success = true;

            //VisionHelper.CreateDevices(out var camera, out var inspector, out var rtc, out var dInExt1, out var dInLaserPort, out var dOutExt1, out var dOutExt2, out var dOutLaserPort, out var laser);
            //or
            // Create your custom camera devices 
            success &= CreateMyDevices( out var camera, out var inspector);


            visionEditorDisp1.Camera = camera;
            visionEditorDisp1.Inspector = inspector;

            var doc = visionEditorDisp1.Document;
            VisionHelper.CreateTestProcesses(doc);

            inspector.Ready(doc, camera);
        }

        private bool CreateMyDevices(out ICamera camera, out IInspector inspector, IRtc rtc = null, int index = 0)
        {
            camera = null;
            inspector = null;

            // And create my own camera and inspector instance 
            bool success = true;
            string cameraType = NativeMethods.ReadIni(VisionHelper.ConfigFileName, $"CAMERA{index}", "TYPE", "Virtual");
            var camearSerialNo = NativeMethods.ReadIni(VisionHelper.ConfigFileName, $"CAMERA{index}", "SERIAL_NO", string.Empty);
            var camearIPaddress = NativeMethods.ReadIni(VisionHelper.ConfigFileName, $"CAMERA{index}", "IP_ADDRESS", string.Empty);
            var cameraWidth = NativeMethods.ReadIni<int>(VisionHelper.ConfigFileName, $"CAMERA{index}", "WIDTH", 1024);
            var cameraHeight = NativeMethods.ReadIni<int>(VisionHelper.ConfigFileName, $"CAMERA{index}", "HEIGHT", 768);
            var cameraPixelSize = NativeMethods.ReadIni<double>(VisionHelper.ConfigFileName, $"CAMERA{index}", "PIXEL_SIZE", 0.005);
            var lensMag = NativeMethods.ReadIni<double>(VisionHelper.ConfigFileName, $"CAMERA{index}", "LENS_MAGNIFICATION", 1);
            var exposureTime = NativeMethods.ReadIni<double>(VisionHelper.ConfigFileName, $"CAMERA{index}", "EXPOSURE_TIME", 50 * 1000);
            var cameraFps = NativeMethods.ReadIni<int>(VisionHelper.ConfigFileName, $"CAMERA{index}", "FPS", 30);
            var cameraRotateFlipStr = NativeMethods.ReadIni<string>(VisionHelper.ConfigFileName, $"CAMERA{index}", "ROTATE_FLIP", "RotateNoneFlipNone");
            RotateFlipType cameraRotateFlip = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), cameraRotateFlipStr);
            int enableStitch = NativeMethods.ReadIni<int>(VisionHelper.ConfigFileName, $"CAMERA{index}", $"STITCH_ENABLE");
            var stitchRows = NativeMethods.ReadIni<int>(VisionHelper.ConfigFileName, $"CAMERA{index}", "STITCH_ROWS", 5);
            var stitchCols = NativeMethods.ReadIni<int>(VisionHelper.ConfigFileName, $"CAMERA{index}", "STITCH_COLS", 7);
            var stitchMarginWidth = NativeMethods.ReadIni<int>(VisionHelper.ConfigFileName, $"CAMERA{index}", "STITCH_MARGIN_WIDTH", 5);
            var stitchMarginHeight = NativeMethods.ReadIni<int>(VisionHelper.ConfigFileName, $"CAMERA{index}", "STITCH_MARGIN_HEIGHT ", 7);
            if (0 == enableStitch)
            {
                // Your stand alone camera
                camera = new MyCameraStandAlone(index, cameraWidth, cameraHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip);
            }
            else
            {
                // Your custom stitch supported camera
                camera = new MyCamera(index, cameraWidth, cameraHeight, cameraPixelSize, lensMag, cameraFps, cameraRotateFlip, stitchRows, stitchCols, stitchMarginWidth, stitchMarginHeight);
            }

            int enableTransform = NativeMethods.ReadIni<int>(VisionHelper.ConfigFileName, $"CAMERA{index}", $"TRANFORM_ENABLE");
            if (0 != enableTransform)
            {
                camera.UnitTransform.XmmPerPixel = NativeMethods.ReadIni<double>(VisionHelper.ConfigFileName, $"CAMERA{index}", "TRANFORM_X_MM_PER_PIXEL", 0.01); ;
                camera.UnitTransform.YmmPerPixel = NativeMethods.ReadIni<double>(VisionHelper.ConfigFileName, $"CAMERA{index}", "TRANFORM_Y_MM_PER_PIXEL", 0.01); ;
                camera.UnitTransform.Angle = NativeMethods.ReadIni<double>(VisionHelper.ConfigFileName, $"CAMERA{index}", "TRANFORM_ANGLE", 0); ;
            }

            camera.OnCalibratorSaved += VisionHelper.Camera_OnCalibratorSaved;
            // Load calibration file into camera
            string calibrationFile = NativeMethods.ReadIni(VisionHelper.ConfigFileName, $"CAMERA{index}", "CALIBRATION", string.Empty);
            if (!string.IsNullOrEmpty(calibrationFile))
            {
                string calibrationPath = Path.Combine(SpiralLab.Sirius2.Vision.Config.CalibrationPath, calibrationFile);
                if (CenterCalibratorSerializer.Open(calibrationPath, out var calibrator))
                    camera.CenterCalibrator = calibrator;
            }
            // Load calibration file into camera (if stitching supported)
            if (0 != enableStitch)
            {
                if (camera is ICameraStitched cameraStitched)
                {
                    cameraStitched.CreateStitchedCells();
                    //    cameraStitched.OnStitchedCalibratorSaved += CameraStitched_OnStitchedCalibratorSaved;
                    //    string calibrationStitcedFile = NativeMethods.ReadIni(ConfigFileName, $"CAMERA{index}", "CALIBRATION_STITCHED", string.Empty);
                    //    if (!string.IsNullOrEmpty(calibrationStitcedFile))
                    //    {
                    //        string calibrationPath = Path.Combine(SpiralLab.Sirius2.Vision.Config.CalibrationPath, calibrationStitcedFile);
                    //        if (ProcessSerializer.OpenProcess(calibrationPath, out var process))
                    //        {
                    //            cameraStitched.StitchedCalibrator = process as ProcessCalibrator;
                    //            cameraStitched.IsCalibrateStitchEnabled = true;
                    //        }
                    //    }
                    //    // Create center position for stitced images
                    //    CalculateStitchedPoints(cameraStitched, rtc);
                }
            }

            // Assign RTC instance
            // To enable stitich, Rtc instance must be created and assinged at camera
            camera.Rtc = rtc;

            success &= camera.Initialize();
            success &= camera.CtlExposureTime(exposureTime);

            // Create inspector for processing
            switch (cameraType.Trim().ToLower())
            {
                default:
                    inspector = InspectorFactory.CreateDefault(0, "Insp0");
                    break;
            }

            return success;
        }
    }
}
