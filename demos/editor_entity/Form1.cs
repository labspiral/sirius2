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
 * Description : Example sirius2 editor
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
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;

namespace Demos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize sirius2 library
            SpiralLab.Sirius2.Core.Initialize();

            CreateDevices();
            CreateMarker();
            CreateTestEntities();
            CustomConverter();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var document = siriusEditorUserControl1.Document;
            var marker = siriusEditorUserControl1.Marker;
            var laser = siriusEditorUserControl1.Laser;
            var rtc = siriusEditorUserControl1.Rtc;

            if (document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit without save ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            if (rtc.CtlGetStatus(RtcStatus.Busy) ||
                laser.IsBusy ||
                marker.IsBusy)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit during working on progressing... ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            if (e.Cancel == false)
                this.DestroyDevices();
        }
        private void DestroyDevices()
        {
            var marker = siriusEditorUserControl1.Marker;
            var laser = siriusEditorUserControl1.Laser;
            var rtc = siriusEditorUserControl1.Rtc;
            marker.Stop();
            marker.Dispose();
            laser.Dispose();
            rtc.Dispose();
            siriusEditorUserControl1.Rtc = null;
            siriusEditorUserControl1.Laser = null;
            siriusEditorUserControl1.Marker = null;
        }
        private void CreateDevices()
        {
            bool success = true;

            #region Initialize RTC controller
            // FOV size would be used for calcualte k-factor
            var fov = NativeMethods.ReadIni<float>(Program.ConfigFileName, "RTC", "FOV", 100.0f);
            // K-Factor = bits/mm
            // RTC5,6 using 20 bits resolution
            var kfactor = Math.Pow(2, 20) / fov;
            // Field correction file path: \correction\cor_1to1.ct5
            // Default (1:1) correction file
            //var correctionFile = Path.Combine(Config.CorrectionPath, "cor_1to1.ct5");
            var correctionFile = NativeMethods.ReadIni(Program.ConfigFileName, "RTC", "CORRECTION", "cor_1to1.ct5");
            var correctionPath = Path.Combine(SpiralLab.Sirius2.Config.CorrectionPath, correctionFile);
            var signalLevel = NativeMethods.ReadIni(Program.ConfigFileName, "RTC", "SIGNALLEVEL", "High") == "High" ? RtcSignalLevel.ActiveHigh : RtcSignalLevel.ActiveLow;
            var rtcId = NativeMethods.ReadIni<int>(Program.ConfigFileName, "RTC", "ID", 0);
            var rtcType = NativeMethods.ReadIni(Program.ConfigFileName, "RTC", "TYPE", "Rtc5");
            var sLaserMode = NativeMethods.ReadIni(Program.ConfigFileName, "RTC", "LASERMODE", "Yag5");
            var laserMode = (LaserMode)Enum.Parse(typeof(LaserMode), sLaserMode);

            IRtc rtc = null;
            switch(rtcType.Trim().ToLower())
            {
                case "virtual":
                    rtc = ScannerFactory.CreateVirtual(rtcId, kfactor, correctionFile);
                    break;
                case "rtc5":
                    rtc = ScannerFactory.CreateRtc5(rtcId, kfactor, laserMode, signalLevel, signalLevel, correctionPath);
                    break;
                case "rtc6":
                    rtc = ScannerFactory.CreateRtc6(rtcId, kfactor, laserMode, signalLevel, signalLevel, correctionPath);
                    break;
                case "rtc6e":
                    rtc = ScannerFactory.CreateRtc6Ethernet(rtcId, "192.168.0.100", "255.255.255.0", kfactor, laserMode, signalLevel, signalLevel, correctionPath);
                    break;
                case "syncaxis":
                    string configXmlFileName = NativeMethods.ReadIni(Program.ConfigFileName, "RTC", "CONFIG_XML", string.Empty);
                    rtc = ScannerFactory.CreateRtc6SyncAxis(rtcId, configXmlFileName);
                    break;
            }

            // Initialize RTC controller
            success &= rtc.Initialize();
            Debug.Assert(success);

            // Set FOV area: WxH, it will be drawn as red rectangle
            SpiralLab.Sirius2.Winforms.Config.DocumentDefaultViewFovSize = new SizeF(fov, fov);
            // Set Virtual image field area
            if (rtc.IsMoF)
            {
                if (rtc is Rtc5 rtc5)
                {
                    //2^24
                    SpiralLab.Sirius2.Winforms.Config.DocumentDefaultViewVirtualImageSize = new SizeF(fov * (float)Math.Pow(2, 4), fov * (float)Math.Pow(2, 4));
                }
                else if (rtc is Rtc6 rtc6)
                {
                    //2^29
                    SpiralLab.Sirius2.Winforms.Config.DocumentDefaultViewVirtualImageSize = new SizeF(fov * (float)Math.Pow(2, 9), fov * (float)Math.Pow(2, 9));
                }
            }

            // MoF 
            var rtcMoF = rtc as IRtcMoF;
            if (null != rtcMoF)
            {
                rtcMoF.EncXCountsPerMm = NativeMethods.ReadIni<int>(Program.ConfigFileName, "MOF", "MOF_X_ENC_COUNTS_PER_MM", 0);
                rtcMoF.EncYCountsPerMm = NativeMethods.ReadIni<int>(Program.ConfigFileName, "MOF", "MOF_X_ENC_COUNTS_PER_MM", 0);
                rtcMoF.EncCountsPerRevolution = NativeMethods.ReadIni<int>(Program.ConfigFileName, "MOF", "MOF_ANGULAR_ENC_COUNTS_PER_REVOLUTION", 0);
            }

            // Default frequency and pulse width: 50KHz, 2 usec 
            success &= rtc.CtlFrequency(50 * 1000, 2);
            // Default jump and mark speed: 50 mm/s
            success &= rtc.CtlSpeed(50, 50);
            #endregion

            #region Initialize Laser source
            var laserId = NativeMethods.ReadIni<int>(Program.ConfigFileName, "RTC", "ID", 0);
            var laserType = NativeMethods.ReadIni(Program.ConfigFileName, "LASER", "TYPE", "Virtual");
            var laserPowerControl = NativeMethods.ReadIni(Program.ConfigFileName, "LASER", "POWERCONTROL", "Unknown");
            var laserMaxPower = NativeMethods.ReadIni<float>(Program.ConfigFileName, "LASER", "MAXPOWER", 10);
            var laserDefaultPower = NativeMethods.ReadIni<float>(Program.ConfigFileName, "LASER", "DEFAULTPOWER", 1);
            ILaser laser = null;
            switch (laserType.Trim().ToLower())
            {
                case "virtual":
                    switch (laserPowerControl.Trim().ToLower())
                    {
                        default:
                        case "unknown":
                            laser = LaserFactory.CreateVirtual(laserId, laserMaxPower);
                            break;
                        case "analog1":
                            laser = LaserFactory.CreateVirtualAnalog(laserId, laserMaxPower, 1);
                            break;
                        case "analog2":
                            laser = LaserFactory.CreateVirtualAnalog(laserId, laserMaxPower, 2);
                            break;
                        case "frequency":
                            var freqMin = NativeMethods.ReadIni<float>(Program.ConfigFileName, "LASER", "POWERCONTROL_FREQUENCY_MIN", 0);
                            var freqMax = NativeMethods.ReadIni<float>(Program.ConfigFileName, "LASER", "POWERCONTROL_FREQUENCY_MIN", 50000);
                            laser = LaserFactory.CreateVirtualFrequency(laserId, laserMaxPower, freqMin, freqMax);
                            break;
                        case "dutycycle":
                            var dutyCycleMin = NativeMethods.ReadIni<float>(Program.ConfigFileName, "LASER", "POWERCONTROL_DUTYCYCLE_MIN", 0);
                            var dutyCycleMax = NativeMethods.ReadIni<float>(Program.ConfigFileName, "LASER", "POWERCONTROL_DUTYCYCLE_MAX", 99);
                            laser = LaserFactory.CreateVirtualDutyCycle(laserId, laserMaxPower, dutyCycleMin, dutyCycleMax);
                            break;
                        case "digitalbits16":
                            laser = LaserFactory.CreateVirtualDO16Bits(laserId, laserMaxPower);
                            break;
                        case "digitalbits8":
                            laser = LaserFactory.CreateVirtualDO8Bits(laserId, laserMaxPower);
                            break;
                    }
                    break;
            }

            // Assign RTC into laser source
            laser.Scanner = rtc;

            // Initialize laser source
            success &= laser.Initialize();

            // Default power 
            if (laser is ILaserPowerControl powerControl)
            {
                var laserPowerControlDelay = NativeMethods.ReadIni<float>(Program.ConfigFileName, "LASER", "POWERCONTROLDELAY", 0);
                powerControl.PowerControlDelayTime = laserPowerControlDelay;
                success &= powerControl.CtlPower(laserDefaultPower);
            }
            Debug.Assert(success); 
            #endregion

            // Assign instances to user control
            siriusEditorUserControl1.Rtc = rtc;
            siriusEditorUserControl1.Laser = laser;
        }
        private void CreateMarker()
        {
            var rtcType = NativeMethods.ReadIni(Program.ConfigFileName, "RTC", "TYPE", "Rtc5");
            IMarker marker = null;
            switch (rtcType.Trim().ToLower())
            {
                case "virtual":
                    marker = MarkerFactory.CreateVirtual(0);
                    break;
                case "rtc5":
                    marker = MarkerFactory.CreateRtc5(0);
                    break;
                case "rtc6":
                case "rtc6e":
                    marker = MarkerFactory.CreateRtc6(0);
                    break;
                case "syncaxis":
                    marker = MarkerFactory.CreateSyncAxis(0);
                    break;
            }
            // Assign instances to user control
            siriusEditorUserControl1.Marker = marker;

            // Assign Document, Rtc, Laser at IMarker
            marker.Ready(siriusEditorUserControl1.Document, siriusEditorUserControl1.View, siriusEditorUserControl1.Rtc, siriusEditorUserControl1.Laser);
        }
        private void CreateTestEntities()
        {
            var document = siriusEditorUserControl1.Document;
            Debug.Assert(null != document.ActiveLayer);

            // Line entity
            var line1 = EntityFactory.CreateLine(new Vector2(-40, 40), new Vector2(40, 40));
            document.ActAdd(line1);
            var line2 = EntityFactory.CreateLine(new Vector2(40, 40), new Vector2(40, -40));
            document.ActAdd(line2);

            // Point1 entity
            var p1 = EntityFactory.CreatePoint(new Vector2(-3, 1), 1);
            document.ActAdd(p1);

            // Points2 entity
            var locations1 = new List<Vector2>();
            locations1.Add(new Vector2(-2, 2));
            locations1.Add(new Vector2(-3, 2));
            locations1.Add(new Vector2(-4, 2));
            locations1.Add(new Vector2(-2, 3));
            locations1.Add(new Vector2(-3, 3));
            locations1.Add(new Vector2(-4, 3));
            locations1.Add(new Vector2(-2, 4));
            locations1.Add(new Vector2(-3, 4));
            locations1.Add(new Vector2(-4, 4));
            var p2 = EntityFactory.CreatePoints(locations1.ToArray(), 1);
            document.ActAdd(p2);

            // Points3 entity
            var locations2 = new List<Vector2>();
            var rnd = new Random();
            for (int i = 0; i < 1000; i++)
                locations2.Add(new Vector2((float)(rnd.NextDouble() * -5.0 - 1), (float)(rnd.NextDouble() * -5.0 - 1)));
            var p3 = EntityFactory.CreatePoints(locations2.ToArray(), 1);
            document.ActAdd(p3);

            // Arc entity
            var arc1 = EntityFactory.CreateArc(Vector2.Zero, 10, 0, 360);
            arc1.Translate(20, 20);
            document.ActAdd(arc1);
            var arc2 = EntityFactory.CreateArc(Vector2.Zero, 8, 90, -300);
            arc2.Translate(20, 20);
            document.ActAdd(arc2);
            var arc3 = EntityFactory.CreateArc(Vector2.Zero, 6, 45, -180);
            arc3.Translate(20, 20);
            document.ActAdd(arc3);

            // Rectangle entity
            var rectangle1 = EntityFactory.CreateRectangle(new Vector2(10, -10), 10, 5);
            document.ActAdd(rectangle1);
            // Hatch with polygon within rectangle
            var rectanglehatch = rectangle1.Hatch(HatchModes.Polygon, HatchJoints.Miter, false, 0, 0, 0.2f, 0, 0);
            foreach(var hatch in rectanglehatch.Children)
                hatch.Color = SpiralLab.Sirius2.Winforms.Config.PensColor[1];
            document.ActAdd(rectanglehatch);

            // Spiral entity
            var spiral1 = EntityFactory.CreateSpiral(Vector2.Zero, 8, 12, 5, 20, false);
            spiral1.Scale(0.9);
            spiral1.Rotate(-50, 0, 0);
            spiral1.Translate(-35, 0);
            document.ActAdd(spiral1);

            // Image entity
            var filename1 = Path.Combine("sample", "lena.bmp");
            var image1 = EntityFactory.CreateImage(filename1, 10, 10);
            image1.Translate(-30, -15);
            document.ActAdd(image1);

            // Image entity 
            var filename2 = Path.Combine("sample", "checkerboard.bmp");
            var image2 = EntityFactory.CreateImage(filename2, 10);
            image2.Translate(-30, 25);
            document.ActAdd(image2);

            // ImageText entity
            var imagetext1 = EntityFactory.CreateImageText("Arial", $"12345 67890{Environment.NewLine}ABCDEFGHIJKLMNOPQRSTUVWXYZ{Environment.NewLine}`~!@#$%^&*()-_=+[{{]|}}\\|;:'\",<.>/?{Environment.NewLine}abcdefghijklmnopqrstuvwxyz", FontStyle.Regular, false, 3, 64, 10);
            imagetext1.Name = "MyText1";
            imagetext1.Translate(-30, -30);
            document.ActAdd(imagetext1);

            // Text entity
            var text1 = EntityFactory.CreateText("Arial", $"12345 67890{Environment.NewLine}ABCDEFGHIJKLMNOPQRSTUVWXYZ{Environment.NewLine}`~!@#$%^&*()-_=+[{{]|}}\\|;:'\",<.>/?{Environment.NewLine}abcdefghijklmnopqrstuvwxyz", FontStyle.Bold, 2.5);
            text1.Name = "MyText2";
            document.ActAdd(text1);

            // Sirius text entity
            var text2 = EntityFactory.CreateSiriusText("romans2.cxf", $"12345 67890{Environment.NewLine}ABCDEFGHIJKLMNOPQRSTUVWXYZ{Environment.NewLine}`~!@#$%^&*()-_=+[{{]|}}\\|;:'\",<.>/?{Environment.NewLine}abcdefghijklmnopqrstuvwxyz", 2.5); 
            text2.Name = "MyText3";
            text2.Translate(18, -12);
            document.ActAdd(text2);

            // Polyline2D entity
            var vertices1 = new List<EntityVertex2D>();
            vertices1.Add(new EntityVertex2D(0, 0));
            vertices1.Add(new EntityVertex2D(-10, -10));
            vertices1.Add(new EntityVertex2D(-20, -25));
            vertices1.Add(new EntityVertex2D(-2, -30, 0.8));
            vertices1.Add(new EntityVertex2D(1, 1));
            var polyline1 = EntityFactory.CreatePolyline2D(vertices1.ToArray(), true);
            polyline1.Translate(-10, -10, 0);
            document.ActAdd(polyline1);

            // Expanded Polyline2D 
            var expandedPolylines = polyline1.Expand(HatchJoints.Round, 0.5);
            foreach (var expandedPolyline in expandedPolylines)
            {
                expandedPolyline.Color = SpiralLab.Sirius2.Winforms.Config.PensColor[2];
                document.ActAdd(expandedPolyline);
            }

            // Shrink Polyline2D
            var shrinkedPolylines = polyline1.Expand(HatchJoints.Miter, -2);
            foreach (var shrinkedPolyline in shrinkedPolylines)
            {
                shrinkedPolyline.Color = SpiralLab.Sirius2.Winforms.Config.PensColor[3];
                document.ActAdd(shrinkedPolyline);
            }

            // Polyline2D entity
            var vertices2 = new List<EntityVertex2D>();
            vertices2.Add(new EntityVertex2D(-5, -5));
            vertices2.Add(new EntityVertex2D(-5, 5, -1));
            vertices2.Add(new EntityVertex2D(5, 5));
            vertices2.Add(new EntityVertex2D(5, -5, -1));
            var polyline2 = EntityFactory.CreatePolyline2D(vertices2.ToArray(), true);
            polyline2.Translate(-10, 10, 0);
            document.ActAdd(polyline2);

            // Group entity has include 2 arcs and rectangle entities
            var arc_g1 = EntityFactory.CreateArc(Vector2.Zero, 1, 0, 360);
            arc_g1.Color = SpiralLab.Sirius2.Winforms.Config.PensColor[1];
            var arc_g2 = EntityFactory.CreateArc(Vector2.Zero, 2, 0, 360);
            arc_g2.Color = SpiralLab.Sirius2.Winforms.Config.PensColor[3];
            arc_g2.Translate(3, 2, 0);
            var rec_g3 = EntityFactory.CreateRectangle(Vector2.Zero, 5, 4);
            var group1 = EntityFactory.CreateGroup("Arcs", new IEntity[] { arc_g1, arc_g2, rec_g3 });
            group1.Translate(10, -20);
            document.ActAdd(group1);

            // Block item
            var vertices3 = new List<EntityVertex2D>();
            vertices3.Add(new EntityVertex2D(10, 10));
            vertices3.Add(new EntityVertex2D(10, 15));
            vertices3.Add(new EntityVertex2D(5, 20));
            vertices3.Add(new EntityVertex2D(4, 8));
            var polyline3 = EntityFactory.CreatePolyline2D(vertices3.ToArray());
            // Block entity
            var block = EntityFactory.CreateBlock("MyBlock", new IEntity[] { polyline3 });
            // Block entity has include polyline2D entity 
            document.ActAdd(block);

            // BlockInsert entity
            var insert1 = EntityFactory.CreateBlockInsert(block.Name);
            document.ActAdd(insert1);

            // BlockInsert entity
            var insert2 = EntityFactory.CreateBlockInsert(block.Name);
            insert2.Scale(1.5);
            insert2.RotateZ(30);
            insert2.Translate(-1, 2, 0);
            document.ActAdd(insert2);

            // Curve(conic spline) entity
            var vertices4 = new List<Vector2>();
            vertices4.Add(new Vector2(10, -3));
            vertices4.Add(new Vector2(11, -4));
            vertices4.Add(new Vector2(14, -1));
            var conicSpline = EntityFactory.CreateCurve(vertices4.ToArray());
            document.ActAdd(conicSpline);

            // Curve(cubic spline) entity
            var vertices5 = new List<Vector2>();
            vertices5.Add(new Vector2(10, -3));
            vertices5.Add(new Vector2(11, -4));
            vertices5.Add(new Vector2(14, -1));
            vertices5.Add(new Vector2(16, -2));
            var cubicSpline = EntityFactory.CreateCurve(vertices5.ToArray());
            cubicSpline.Translate(0, -2);
            document.ActAdd(cubicSpline);

            // Create new layer
            var layer1 = EntityFactory.CreateLayer("1");
            document.ActAdd(layer1);
            Debug.Assert(document.ActiveLayer == layer1);

            // STL entity
            if (EntityFactory.CreateStereoLithography(Path.Combine("sample", "Nefertiti_face.stl"), out var stl))
            {
                stl.Alignment = Alignments.MiddleCenter;
                stl.Scale(0.2);
                stl.Translate(30, 10);
                stl.RotateZ(-90);
                document.ActAdd(stl);
            }

            // Dxf entity
            if (EntityFactory.CreateDxf(Path.Combine("sample", "BIKE.dxf"), out var dxf))
            {
                dxf.Alignment = Alignments.MiddleCenter;
                dxf.Scale(0.02);
                dxf.Translate(25, -35);
                document.ActAdd(dxf);
            }

            // HPGL entity
            if (EntityFactory.CreateHpgl(Path.Combine("sample", "SimplexOpt.plt"), out var hpgl))
            {
                hpgl.Alignment = Alignments.MiddleCenter;
                hpgl.Scale(0.02);
                hpgl.Translate(-2, 37);
                document.ActAdd(hpgl);
            }

            // HPGL entity
            if (EntityFactory.CreateHpgl(Path.Combine("sample", "columbiao.plt"), out var hpgl2))
            {
                hpgl2.Alignment = Alignments.MiddleCenter;
                hpgl2.Scale(0.01);
                hpgl2.Translate(-35, -20);
                document.ActAdd(hpgl2);
            }
            var dataMatrix1 = EntityFactory.CreateDataMatrix("0123456789", BarcodeCells.Dots, 3, 4, 4);
            dataMatrix1.Translate(-23, 2);
            document.ActAdd(dataMatrix1);
            var dataMatrix2 = EntityFactory.CreateDataMatrix("SIRIUS2", BarcodeCells.Lines, 4, 4, 4);
            dataMatrix2.Translate(-23, 7);
            document.ActAdd(dataMatrix2);
            var dataMatrix3 = EntityFactory.CreateDataMatrix("ABCDEFGHIJKLMNOPQRSTUVWXYZ", BarcodeCells.Circles, 3, 4, 4);
            dataMatrix3.Translate(-28, 2);
            document.ActAdd(dataMatrix3);
            var dataMatrix4 = EntityFactory.CreateDataMatrix("abcdefghijklmnopqrstuvwxyz", BarcodeCells.Outline, 2, 4, 4);
            dataMatrix4.Translate(-28, 7);
            document.ActAdd(dataMatrix4);

            var qr1 = EntityFactory.CreateQRCode("0123456789", BarcodeCells.Hatch, 3, 4, 4);
            qr1.CellHatch.HatchMode = HatchModes.CrossLine;
            qr1.CellHatch.HatchInterval = 0.05f;
            qr1.CellHatch.HatchAngle = 100;
            qr1.CellHatch.HatchAngle2 = 10;
            qr1.Translate(-23, 12);
            document.ActAdd(qr1);

            var qr2 = EntityFactory.CreateQRCode("abcdefghijklmnopqrstuvwxyz", BarcodeCells.Outline, 3, 4, 4);
            qr2.Translate(-28, 12);
            document.ActAdd(qr2);
        }
        private void CustomConverter()
        {
            var document = siriusEditorUserControl1.Document;
            SpiralLab.Sirius2.Winforms.Config.OnTextConvert += Text_OnTextConvert;
        }
        private bool Text_OnTextConvert(IMarker marker, ITextConvertible textConvertible)
        {
            var entity = textConvertible as IEntity;
            if (entity.Name == "MyText1")
            {
                // For example, convert string to DateTime format
                // link: https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
                 string dateTimeStr = textConvertible.SourceText; //like as "yyyyMMdd HH:mm:ss"
                textConvertible.ConvertedText = DateTime.Now.ToString(dateTimeStr);

                // For eaxmple, convert hex string to hex binary
                //string hexStr = textConvertible.SourceText; //like as "fe3009333137303130323031f9200134fe300120fc2006";
                //var byteArray = Enumerable.Range(0, hexStr.Length / 2).Select(x => Convert.ToByte(hexStr.Substring(x * 2, 2), 16)).ToArray();
                //var byteArrayAsString = new String(byteArray.Select(b => (char)b).ToArray());
                Logger.Log(Logger.Type.Debug, $"entity: {entity.Name} [{entity.Id}] text {textConvertible.SourceText} -> {textConvertible.ConvertedText}");
            }
            return true;
        }

        #region Control by remotely
        /// <summary>
        /// Ready status
        /// </summary>
        /// <returns>Status</returns>
        public bool IsReady
        {
            get
            {
                var marker = siriusEditorUserControl1.Marker;
                return marker.IsReady;
            }
        }
        /// <summary>
        /// Busy status
        /// </summary>
        /// <returns>Status</returns>
        public bool IsBusy
        {
            get
            {
                var marker = siriusEditorUserControl1.Marker;
                return marker.IsBusy;
            }
        }
        /// <summary>
        /// Error status
        /// </summary>
        /// <returns>Status</returns>
        public bool IsError
        {
            get
            {
                var marker = siriusEditorUserControl1.Marker;
                return marker.IsError;
            }
        }

        /// <summary>
        /// Open recipe (.sirius2 file)
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <returns>Success or failed</returns>
        public bool Open(string fileName)
        {
            if (this.IsBusy)
                return false;
            var doc = siriusEditorUserControl1.Document;
            return doc.ActOpen(fileName);
        }
        /// <summary>
        /// Start marker
        /// </summary>
        /// <param name="offets">Array of offset</param>
        /// <returns>Success or failed</returns>
        public bool Start(SpiralLab.Sirius2.Mathematics.Offset[] offets = null)
        {
            if (!this.IsReady)
                return false;
            if (this.IsBusy)
                return false;
            if (this.IsError)
                return false;
            var marker = siriusEditorUserControl1.Marker;
            marker.Offsets = offets;
            return marker.Start();
        }
        /// <summary>
        /// Stop marker
        /// </summary>
        ///<returns>Success or failed</returns>
        public bool Stop()
        {
            var marker = siriusEditorUserControl1.Marker;
            return marker.Stop();
        }
        /// <summary>
        /// Reset marker status
        /// </summary>
        /// <returns>Success or failed</returns>
        public bool Reset()
        {
            var marker = siriusEditorUserControl1.Marker;
            return marker.Reset();
        }
        #endregion
    }
}