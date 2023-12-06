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

            // Create entities for XY MoF test
            var mofBegin = EntityFactory.CreateMoFBegin(RtcEncoderTypes.XY, true);
            document.ActAdd(mofBegin);

            /*      
             *                     |
             *                     |
             *                     |
             *                     |
             *     . .             |
             *      .        |     |
             *       .       | |   |
             *     .         | | | |
             *  ----.--▯--@--|-|-|-+-------------------    => ENC +
             *       .       | | | |                       => MOVING DIRECTION 
             *     .         | |   |
             *      .        |     |
             *        .            |
             *      .              |
             *                     |
             *                     |
             *                     |
             */

            double x1 = -1;
            var mofWait1 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x1);
            document.ActAdd(mofWait1);
            var line1 = EntityFactory.CreateLine(x1, 10, x1, -10);
            document.ActAdd(line1);

            double x2 = -5;
            var mofWait2 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x2);
            document.ActAdd(mofWait2);
            var line2 = EntityFactory.CreateLine(x2, 15, x2, -15);
            document.ActAdd(line2);

            double x3 = -10;
            var mofWait3 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x3);
            document.ActAdd(mofWait3);
            var line3 = EntityFactory.CreateLine(x3, 20, x3, -20);
            document.ActAdd(line3);

            double x4 = -15;
            var mofWait4 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x4);
            document.ActAdd(mofWait4);
            var spiral = EntityFactory.CreateSpiral(x4, 0, 2, 4, 0, 5, true);
            document.ActAdd(spiral);

            double x5 = -20;
            var mofWait5 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x5);
            document.ActAdd(mofWait5);
            var dataMatrix = EntityFactory.CreateDataMatrix("SIRIUS2", Barcode2DCells.Outline, 3, 4, 4);
            dataMatrix.RotateZ(90);
            dataMatrix.Name = "MyBarcode1";
            dataMatrix.Translate(x5, -10);
            document.ActAdd(dataMatrix);

            var text = EntityFactory.CreateText(SpiralLab.Sirius2.Winforms.Config.InstalledFontNames[0], "SIRIUS2", FontStyle.Bold, 4);
            text.RotateZ(90);
            text.Translate(x5, 10);
            document.ActAdd(text);

            double x6 = -40;
            var mofWait6 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x6);
            document.ActAdd(mofWait6);
            var mofAndPointsList = new List<IEntity>(100);
            double xRange = 2;
            double yRange = 30;
            var rnd = new Random();
            for (int i = 0; i < 20; i++)
            {
                double x = x6 + rnd.NextDouble() * (xRange + xRange) - xRange;
                double y = rnd.NextDouble() * (yRange + yRange) - yRange;
                var point = EntityFactory.CreatePoint(new Vector2((float)x, (float)y), 20);
                mofAndPointsList.Add(point);
            }
            var group = EntityFactory.CreateGroup("20 Points", mofAndPointsList.ToArray());
            document.ActAdd(group);            
            
            var mofEnd = EntityFactory.CreateMoFEnd(Vector2.Zero);
            document.ActAdd(mofEnd);

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser, powerMeter);

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);

            // Start simulated encoders as x= 1, y=0 mm/s by rtcMoF.CtlMofEncoderSpeed(1, 0);
            rtcMoF.CtlMofEncoderSpeed(1, 0);
            // or
            // Edit 'Simulated x speed at MoF = 1' at propertygrid of Laser page
            // and
            // Marker.Start
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
