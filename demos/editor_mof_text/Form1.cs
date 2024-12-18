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
 * Description : Example sirius2 editor for MoF(marking on the fly) with converted text by script file
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
using SpiralLab.Sirius2.IO;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Mathematics;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.Script;

namespace Demos
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

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
            EditorHelper.CreateDevices(out var rtc, out var dInExt1, out var dInLaserPort, out var dOutExt1, out var dOutExt2, out var dOutLaserPort, out var laser, out var powerMeter, out var marker, out var remote);

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

            // For example
            // 1. wait until DIN0 goes to HIGH at EXTENSION1 Port
            // 2. mof begin with reset encoder
            // 3. mark barcode and text 
            //   - text has changed by script
            //   - increase serial no after marks 
            // 4. mof end
            // and repeat 100 times by watiting DIN0 trigger 

            // Set D.IN0 name
            SpiralLab.Sirius2.Winforms.Config.DIn_RtcExtension1Port[0] = "TRIGGER";

            // Create waiting DIN0 condition entity with rising edge
            var waitExt16Cond = EntityFactory.CreateWaitDataExt16EdgeCond(
                0, //D.IN0
                SignalEdges.High); 
            document.ActAdd(waitExt16Cond);

            // Create mof begin entity
            var mofBegin = EntityFactory.CreateMoFBegin(RtcMoFTypes.XY, true);
            document.ActAdd(mofBegin);
         
            // Create text entity
            var text1 = EntityFactory.CreateText("Arial", "ABCDEFGHIJKLMNOPQRSTUVWXYZ {0}", FontStyle.Bold, 5);
            text1.Name = "MyText1";
            text1.IsConvertedText = true;
            text1.Color = penColor;
            text1.Translate(-45, 0);
            document.ActAdd(text1);

            // Create sirius text entity
            var text2 = EntityFactory.CreateSiriusText("romans2.cxf", "0123456789 {0}", 5);
            text2.Name = "MyText2";
            text2.IsConvertedText = true;
            text2.Color = penColor;
            text2.Translate(60, 0);
            document.ActAdd(text2);

            // Create image text entity
            var text3 = EntityFactory.CreateImageText("Arial", "POWERED BY SIRIUS2 SPIRALLAB {0}", FontStyle.Regular, true, 0, 32, 6);
            text3.Name = "MyText3";
            text3.IsConvertedText = true;
            text3.Color = penColor;
            text3.Translate(105, -1);
            document.ActAdd(text3);

            // Create mof end
            var mofEnd = EntityFactory.CreateMoFEnd(Vector2.Zero);
            document.ActAdd(mofEnd);

            // Create script event
            // 'IScript.ListEvent' script function would be called whenever marker has started
            // By external script file (marker.ScriptFile)
            var scriptEvent = EntityFactory.CreateScriptEvent();
            scriptEvent.Description = "Event for something after each marks";
            document.ActAdd(scriptEvent);

            // Repeats 100 times
            document.ActiveLayer.Repeats = 100;
            // or infinitely
            //document.ActiveLayer.Repeats = int.MaxValue;

            // Zoom to fit by manually
            var bbox = BoundingBox.RealBoundingBox(document.ActiveLayer);
            siriusEditorUserControl1.View.Camera.ZoomFit(bbox);

            // Set pen parameters by manually
            bool founded = document.FindByPenColor(penColor, out var pen);
            Debug.Assert(founded);
            pen.JumpSpeed = 1000; // 1m/s
            pen.MarkSpeed = 1000; // 1m/s
            pen.Power = pen.PowerMax * 0.5; // 50% power

            // Text convert by external script file
            // Target entities should be set as IsConvertedText = true
            marker.ScriptFile = Path.Combine(SpiralLab.Sirius2.Winforms.Config.ScriptPath, "mof_text.cs");
            Debug.Assert(null != marker.ScriptInstance);

            // User can call CtlEvent function at script 
            // For example, reset current serial no as starting no 
            //marker.ScriptInstance.CtlEvent();

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser, powerMeter, remote);

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);

            // Activated simulated encoders for test purpose (x= -10, y=0 mm/s)
            // DO NOT set simulated encoder speed if ENC 0,1 has connected
            rtcMoF.CtlMofEncoderSpeed(-10, 0);

            // To waiting MoF x positions by automatically, set 'IsWaitEncoderXForEachGlyph' to 'True'
            // 1. Remove 'OnBeforeEntity' function at script and 
            // 2. Remove MoF waiting conditions at entity treeview
            // 3. SpiralLab.Sirius2.Winforms.Config.IsWaitEncoderXForEachGlyph = true;
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