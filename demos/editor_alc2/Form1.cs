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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
            // Set language
            EditorHelper.SetLanguage();

            InitializeComponent();

            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize sirius2 library
            EditorHelper.Initialize();

            // Create devices 
            EditorHelper.CreateDevices(out var rtc, out var laser, out var powerMeter, out var marker, out var remote, this.siriusEditorUserControl1);

            // Assign devices into usercontrol
            siriusEditorUserControl1.Rtc = rtc;
            siriusEditorUserControl1.Laser = laser;
            siriusEditorUserControl1.Marker = marker;
            siriusEditorUserControl1.PowerMeter = powerMeter;
            siriusEditorUserControl1.Remote = remote;

            var document = siriusEditorUserControl1.Document;
            var view = siriusEditorUserControl1.View;

            var layer = document.ActiveLayer;
            Debug.Assert(layer != null);

            layer.IsALC = true;
            layer.AlcSignal = AutoLaserControlSignals.Frequency;
            // Only for iDRIVE scanner products !
            layer.AlcMode = AutoLaserControlModes.ActualVelocity;            
            layer.AlcMinValue = 40 * 1000;
            layer.AlcPercentage100 = 50 * 1000;
            layer.AlcMaxValue = 60 * 1000;

            // Set high mark speed for test
            document.FindByPenColor(Color.White, out var pen);
            pen.MarkSpeed = 5 * 1000;
            pen.JumpSpeed= 5 * 1000;

            double samplingHz = 50 * 1000; //50KHz
            var ch = new MeasurementChannels[]
            {
                 MeasurementChannels.SampleX,
                 MeasurementChannels.SampleY,
                 MeasurementChannels.OutputPeriod, //Converted Raw Data to Frequency(KHz)
                 MeasurementChannels.LaserOn,
            };
            var measurementBegin = EntityFactory.CreateMeasurementBegin(samplingHz, ch);
            measurementBegin.Title = "Demo for Editor ALC2";
            document.ActAdd(measurementBegin);
          
            double x1 = -3;
            var line1 = EntityFactory.CreateLine(x1, 10, x1, -10);
            document.ActAdd(line1);

            double x2 = -2;
            var line2 = EntityFactory.CreateLine(x2, 15, x2, -15);
            document.ActAdd(line2);

            double x3 = -1;
            var line3 = EntityFactory.CreateLine(x3, 20, x3, -20);
            document.ActAdd(line3);

            double x4 = 1;
            var line4 = EntityFactory.CreateLine(x4, 10, x4, -10);
            document.ActAdd(line4);

            double x5 = 2;
            var line5 = EntityFactory.CreateLine(x5, 15, x5, -15);
            document.ActAdd(line5);

            double x6 = 3;
            var line6 = EntityFactory.CreateLine(x6, 20, x6, -20);
            document.ActAdd(line6);

            var measurementEnd = EntityFactory.CreateMeasurementEnd();
            document.ActAdd(measurementEnd);

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser, powerMeter);

            var rtcAlc = rtc as IRtcAutoLaserControl;
            Debug.Assert(rtcAlc != null);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var document = siriusEditorUserControl1.Document;
            var marker = siriusEditorUserControl1.Marker;
            var laser = siriusEditorUserControl1.Laser;
            var rtc = siriusEditorUserControl1.Rtc;
            var powerMeter = siriusEditorUserControl1.PowerMeter;
            var remote = siriusEditorUserControl1.Remote;

            if (document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit without save ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            if (marker.IsBusy)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit during working on progressing... ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            if (e.Cancel == false)
            {
                siriusEditorUserControl1.Remote = null;
                siriusEditorUserControl1.PowerMeter = null;
                siriusEditorUserControl1.Marker = null;
                siriusEditorUserControl1.Rtc = null;
                siriusEditorUserControl1.Laser = null;
                EditorHelper.DestroyDevices(rtc, laser, powerMeter, marker, remote);
            }
        }
    }
}
