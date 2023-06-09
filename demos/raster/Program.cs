﻿/*
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
 * Description : Raster(Bitmap, Pixel) Operation
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
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

            // Create virtual RTC controller (without valid RTC controller)
            //var rtc = ScannerFactory.CreateVirtual(0, kfactor, correctionFile);
            // Create RTC5 controller
            var rtc = ScannerFactory.CreateRtc5(0, kfactor, LaserMode.Yag5, RtcSignalLevel.ActiveHigh, RtcSignalLevel.ActiveHigh, correctionFile);
            // Create RTC6 controller
            //var rtc = ScannerFactory.CreateRtc6(0, kfactor, LaserMode.Yag5, RtcSignalLevel.ActiveHigh, RtcSignalLevel.ActiveHigh, correctionFile);
            // Create RTC6 Ethernet controller
            //var rtc = ScannerFactory.CreateRtc6Ethernet(0, "192.168.0.100", "255.255.255.0", kfactor, LaserMode.Yag5, RtcSignalLevel.ActiveHigh, RtcSignalLevel.ActiveHigh, correctionFile);

            // Initialize RTC controller
            success &= rtc.Initialize();
            // 50KHz, 2 usec
            success &= rtc.CtlFrequency(50 * 1000, 2);
            // 500 mm/s
            success &= rtc.CtlSpeed(500, 500);
            // Basic delays
            success &= rtc.CtlDelay(10, 100, 200, 200, 0);
            Debug.Assert(success);

            var rtcRaster = rtc as IRtcRaster;
            Debug.Assert(rtcRaster != null);

            // Create virtual laser source with max 20W
            var laser = LaserFactory.CreateVirtual(0, 20);

            // Assign RTC into laser
            laser.Rtc = rtc;
            // Initialize laser
            success &= laser.Initialize();
            // Default power as 2W
            success &= laser.CtlPower(2);
            Debug.Assert(success);

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for raster operation");
                Console.WriteLine("'1' : draw raster line");
                Console.WriteLine("'2' : darw raster accurate line");
                Console.WriteLine("'3' : darw raster rectangle");
                Console.WriteLine("'4' : draw raster bitmap");
                Console.WriteLine("'Q' : quit");
                Console.Write("Select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                Console.WriteLine($"{Environment.NewLine}");
                var timer = Stopwatch.StartNew();
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        DrawRasterLine(laser, rtc);
                        break;
                    case ConsoleKey.D2:
                        DrawRasterMoreAccuratePitch(laser, rtc);
                        break;
                    case ConsoleKey.D3:
                        DrawRasterRectangle(laser, rtc);
                        break;
                    case ConsoleKey.D4:
                        DrawRasterBitmap(laser, rtc);
                        break;
                }
            } while (true);

            rtc.Dispose();
            laser.Dispose();
        }

        private static bool DrawRasterLine(ILaser laser, IRtc rtc)
        {
            bool success = true;
            var rtcRaster = rtc as IRtcRaster;
            // Start list
            success &= rtc.ListBegin();
            // Jump to start
            success &= rtc.ListJumpTo(new Vector2(-10, 0));
            
            // Pixel period: 100 usec (0.0001 s)
            double period = 100;
            // Pixel duration: 10 usec
            double duration = 10;
            // Distance = 0.1 mm
            float dx = 0.1f;
            // Calculated speed (mm/s) = 1000 mm/s (= 0.1mm / 0.0001s)
            uint counts = 1000;
            // Prepare raster horizontal line
            success &= rtcRaster.ListRasterLine(period, new Vector2(dx, 0), counts);
            for (int i = 0; i < counts; i++)
            {
                // laser on during 10 usec
                success &= rtcRaster.ListRasterPixel(duration);
                if (!success)
                    break;
            }

            if (success)
            {
                // End of list
                success &= rtc.ListEnd();
                // Execute list
                success &= rtc.ListExecute(true);
            }
            return success;
        }

        private static bool DrawRasterMoreAccuratePitch(ILaser laser, IRtc rtc)
        {
            bool success = true;
            var rtcRaster = rtc as IRtcRaster;

            double oldKFactor = rtc.KFactor;
            // Pixel pitch distance are outputted(or calculated) to bits by pitch * kfactor
            // If KFactor = 17476.266666666666666666666666667 bits/mm and and pitch 0.1 mm, then
            // Pitch = 17476.266666666666666666666666667 bits/mm * 0.1 = 1747.6266666666666666666666666667 bits
            // So floating point value would be cut-off
            // Approx error = 0.62 bits
            // If pixel counts are 1000, then accumulated error bit = 0.62 * 1000
            // Totally error is approx. 620 bits (0.62bit * 1000)

            // So reset kfactor as integer value
            rtc.KFactor = 17000;
            // And Do scanner field correction by this kfactor 
            // ...

            // Start list
            success &= rtc.ListBegin();
            // Jump to start
            success &= rtc.ListJumpTo(new Vector2(-10, 0));

            // Pixel period: 100 usec (0.0001 s)
            double period = 100;
            // Pixel duration: 10 usec
            double duration = 10;
            // Distance = 0.1 mm
            float dx = 0.1f;
            // Calculated speed (mm/s) = 1000 mm/s (= 0.1mm / 0.0001s)
            uint counts = 1000;
            // Prepare raster horizontal line
            success &= rtcRaster.ListRasterLine(period, new Vector2(dx, 0), counts);
            for (int i = 0; i < counts; i++)
            {
                // laser on during 10 usec
                success &= rtcRaster.ListRasterPixel(duration);
                if (!success)
                    break;
            }

            if (success)
            {
                // End of list
                success &= rtc.ListEnd();
                // Execute list
                success &= rtc.ListExecute(true);
            }

            rtc.KFactor = oldKFactor;
            return success;
        }
        private static bool DrawRasterRectangle(ILaser laser, IRtc rtc)
        {
            bool success = true;
            var rtcRaster = rtc as IRtcRaster;
            // Start list
            success &= rtc.ListBegin();

            int rows = 10;
            // X pitch = 0.1 mm
            float dx = 0.1f;
            // Y pitch = 0.2 mm
            float dy = 0.2f;
            // Pixel period: 100 usec (0.0001 s)
            double period = 100;
            // Pixel duration: 10 usec
            double duration = 10;

            for (int row = 0; row < rows; row++)
            {
                // Jump to start
                success &= rtc.ListJumpTo(new Vector2(-10, dy + row));
                // Calculated speed (mm/s) = 1000 mm/s (= 0.1mm / 0.0001s)
                uint counts = 1000;
                // Prepare raster horizontal line
                success &= rtcRaster.ListRasterLine(period, new Vector2(dx, 0), counts);
                for (int i = 0; i < counts; i++)
                {
                    // Laser on during 10 usec
                    success &= rtcRaster.ListRasterPixel(duration);
                    if (!success)
                        break;
                }
            }
            if (success)
            {
                // End of list
                success &= rtc.ListEnd();
                // Execute list
                success &= rtc.ListExecute(true);
            }
            return success;
        }

        private static bool DrawRasterBitmap(ILaser laser, IRtc rtc)
        {
            bool success = true;
            var rtcMeasurement = rtc as IRtcMeasurement;
            Debug.Assert(rtcMeasurement != null);
            var rtcRaster = rtc as IRtcRaster;
            var dlg = new OpenFileDialog();

            dlg.Filter = "Image files|*.bmp;*.jpeg;*.jpg;*.png;*.gif";
            dlg.DefaultExt = "bmp";
            if (dlg.ShowDialog() != DialogResult.OK)
                return false;

            System.Drawing.Image image = null;
            try
            {
                image = System.Drawing.Bitmap.FromFile(dlg.FileName);
            }
            catch(Exception ex) 
            {
                Logger.Log(Logger.Type.Error, ex);
                return false;
            }

            var bitmap = image as Bitmap;
            Debug.Assert(bitmap != null);
            int px = bitmap.Width;
            int py = bitmap.Height;
            // pixel pitch = 10um
            float pitch = 0.01f;
            //float dy = 0.01f;

            // Pixel period: 50 usec (0.00005 s)
            double pixelPeriod = 50;
            // Max. pixel time: 20 usec (0.00002 s)
            double pixelTime = 20;
            // Invert black and white color
            bool inverted = false;

            float max = 30; // 30mm
            if (px * pitch > max || py * pitch > max)
            {
                Console.WriteLine($"Too large size: {px}x{py} (FOV limit: {max}mm)");
                return false;
            }

            Console.Write($"Press any key to start with {dlg.FileName} ({px}x{py} pixel) : ({px * pitch}x{py * pitch} mm) ");
            Console.ReadLine();
            Console.WriteLine($"Warning ! It takes for awhile ...");

            // Max 4 channels at RTC5
            var channels = new MeasurementChannel[4]
            {
                 MeasurementChannel.SampleX, //X commanded
                 MeasurementChannel.SampleY, //Y commanded
                 MeasurementChannel.PulseLength, //usec
                 MeasurementChannel.LaserOn, //Gate signal 0/1
            };
            //Must be higher than pixel period (50usec = 20KHz) time
            double sampleRateHz = 100 * 1000; //100KHz
            Stopwatch sw = Stopwatch.StartNew();

            var xPitch = new Vector2(pitch, 0);

            // Start list
            success &= rtc.ListBegin();
            
            success &= rtcMeasurement.ListMeasurementBegin(sampleRateHz, channels);
            for (int y = 0; y < py; y++)
            {
                // Starting from 0,0
                success &= rtc.ListJumpTo(new Vector2(0, py * pitch));
                // Prepare raster horizontal line
                success &= rtcRaster.ListRasterLine(pixelPeriod, xPitch, (uint)px);
                for (int x = 0; x < px; x++)
                {
                    var color = bitmap.GetPixel(x, py - y - 1);
                    // RGB to gray scale: https://stackoverflow.com/questions/2265910/convert-an-image-to-grayscale
                    double grayRatio = 1.0 - (0.3 * color.R + 0.59 * color.G + 0.11 * color.B) / 255.0;
                    if (inverted)
                        grayRatio = 1.0f - grayRatio;
                    // Mark each raster pixels
                    // Activated Laser ON signal during rastering, but vary pulse width by each pixel time
                    success &= rtcRaster.ListRasterPixel(pixelTime * grayRatio);
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }
            if (success)
            {
                success &= rtcMeasurement.ListMeasurementEnd();
                // End of list
                success &= rtc.ListEnd();
                // Execute list
                success &= rtc.ListExecute(true);
            }
            Console.WriteLine($"Processing time: {sw.Elapsed.TotalSeconds} s");
            if (success)
            {
                // Temporary measurement file
                var measurementFile = Path.Combine(Config.MeasurementPath, "measurement_raster.txt");                
                // Save measurement result to file
                success &= RtcMeasurementHelper.Save(measurementFile, rtcMeasurement);
                // Plot as a graph
                RtcMeasurementHelper.Plot(measurementFile, "BITMAP WITH RASTERING");
            }
            image.Dispose();
            return success;
        }
    }
}
