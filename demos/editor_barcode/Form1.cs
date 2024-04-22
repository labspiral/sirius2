﻿/*
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
using SpiralLab.Sirius2.Mathematics;
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
            EditorHelper.CreateDevices(out var rtc, out var dInExt1, out var dInLaserPort, out var dOutExt1, out var dOutExt2, out var dOutLaserPort, out var laser, out var powerMeter, out var marker, out var remote, this.siriusEditorUserControl1);

            // Assign devices into usercontrol
            siriusEditorUserControl1.Rtc = rtc;
            siriusEditorUserControl1.Laser = laser;
            siriusEditorUserControl1.Marker = marker;
            siriusEditorUserControl1.PowerMeter = powerMeter;
            siriusEditorUserControl1.Remote = remote;
            siriusEditorUserControl1.DIExt1 = dInExt1;
            siriusEditorUserControl1.DILaserPort = dInLaserPort;
            siriusEditorUserControl1.DOExt1 = dOutExt1;
            siriusEditorUserControl1.DOExt2 = dOutExt2;
            siriusEditorUserControl1.DOLaserPort = dOutLaserPort;

            var document = siriusEditorUserControl1.Document;
            var view = siriusEditorUserControl1.View;

            // Color of index '0' is White
            Color penColor = SpiralLab.Sirius2.Winforms.Config.PenColors[0];

            // Create entities for test. for example: Datamatrix barcode entity
            var dataMatrix = EntityFactory.CreateDataMatrix("SIRIUS2", Barcode2DCells.Circles, 3, 4, 4);
            dataMatrix.Name = "MyBarcode1";
            dataMatrix.Color = penColor;
            dataMatrix.IsConvertedText = true;
            document.ActAdd(dataMatrix);

            // Create entities for test. for example: text entity
            var text = EntityFactory.CreateText("Arial", $"SIRIUS2", FontStyle.Bold, 1);
            text.Name = "MyText1";
            text.Color = penColor;
            text.IsConvertedText = true;
            text.Translate(0, -1);
            document.ActAdd(text);

            // Create entities for test. for example: 1D barcode entity
            var bcd1 = EntityFactory.CreateBarcode("1234567890", Barcode1DFormats.Code128, 3, 6, 2);
            bcd1.Name = "MyBarcode1";
            bcd1.Color = penColor;
            bcd1.IsConvertedText = true;
            bcd1.Translate(0, -3.2);
            document.ActAdd(bcd1);

            // Zoom to fit by manually
            var bbox = BoundingBox.RealBoundingBox(document.ActiveLayer);
            siriusEditorUserControl1.View.Camera.ZoomFit(bbox);

            // Set pen parameters by manually
            bool founded = document.FindByPenColor(penColor, out var pen);
            Debug.Assert(founded);
            pen.JumpSpeed = 1000;
            pen.MarkSpeed = 1000;
            pen.Power = pen.PowerMax * 0.5; // 50% power

            // Attach event handler for convert barcode and text data (event will be fired every do mark)
            //marker.OnTextConvert += Text_OnTextConvert;
            //OR external script file
            marker.ScriptFile = Path.Combine(SpiralLab.Sirius2.Winforms.Config.ScriptPath, "demo1.cs");
            //marker.sScriptFile = Path.Combine(SpiralLab.Sirius2.Winforms.Config.ScriptPath, "demo2.cs");

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser, powerMeter, remote);

            // Assign 6 offset array positions
            var offsets = new List<Offset>()
            {
                new Offset(-20, 10, 0),
                new Offset(0, 10, 0),
                new Offset(20, 10, 0),
                new Offset(-20, -10, 0),
                new Offset(0, -10, 0),
                new Offset(20, -10, 0),
            };
            marker.Offsets = offsets.ToArray();

            // Do mark procedure as offset first
            if (marker is MarkerRtc markerRtc)
                markerRtc.MarkProcedure = MarkerRtc.MarkProcedures.OffsetFirst;
            else if (marker is MarkerSyncAxis markerRtcSyncAxis)
                markerRtcSyncAxis.MarkProcedure = MarkerSyncAxis.MarkProcedures.OffsetFirst;
        }

        private string Text_OnTextConvert(IMarker marker, ITextConvertible textConvertible)
        {
            var currentLayer = marker.CurrentLayer;
            var currentLayerIndex = marker.CurrentLayerIndex;
            //var currentEntity = textConvertible as IEntity;
            var currentEntity = marker.CurrentEntity;
            var currentEntityIndex = marker.CurrentEntityIndex;
            var currentOffset = marker.CurrentOffset;
            var currentOffsetIndex = marker.CurrentOffsetIndex;

            switch (currentEntity.Name)
            {
                case "MyBarcode1":
                    return $"SIRIUS2 {currentOffsetIndex}";
                case "MyText1":
                    return $"SIRIUS2 {DateTime.Now.ToString("HH:mm:ss")} {currentOffsetIndex}";
                default:
                    // Not modified
                    return textConvertible.SourceText;
            }
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
                {
                    e.Cancel = true;
                    return;
                }
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