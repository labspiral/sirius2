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
 * Description : Example sirius2 editor for digital input by event handler
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

            // Naming for D.IO at extension 1 port
            SpiralLab.Sirius2.Winforms.Config.DIn_RtcExtension1Port = new string[16]
            {
                "Start",
                "Stop",
                "Reset",
                "D03",
                "D04",
                "D05",
                "D06",
                "D07",
                "D08",
                "D09",
                "D10",
                "D11",
                "D12",
                "D13",
                "D14",
                "StartFieldCorrection",
            };
            SpiralLab.Sirius2.Winforms.Config.DOut_RtcExtension1Port = new string[16]
            {
                "Ready",
                "Busy",
                "Error",
                "Remote",
                "D04",
                "D05",
                "D06",
                "D07",
                "D08",
                "D09",
                "D10",
                "D11",
                "D12",
                "D13",
                "D14",
                "D15",
            };

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

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser, powerMeter, remote);

            // Enable timer for update D.IO status
            timer.Interval = 100; //10hz
            timer.Tick += Timer_Tick;
            timer.Enabled = true;

            siriusEditorUserControl1.DIExt1.OnChanged += DIExt1_OnChanged;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated)
                return;
            UpdateDIOStatus();
        }

        void UpdateDIOStatus()
        {
            if (null != siriusEditorUserControl1.DIExt1)
            {
                //D.In
                siriusEditorUserControl1.DIExt1.Update();
            }
            if (null != siriusEditorUserControl1.DOExt1)
            {
                //D.Out
                var marker = siriusEditorUserControl1.Marker;
                if (null != marker)
                {
                    if (marker.IsReady) 
                        siriusEditorUserControl1.DOExt1.OutOn(0);
                    else 
                        siriusEditorUserControl1.DOExt1.OutOff(0);
                    if (marker.IsBusy)
                        siriusEditorUserControl1.DOExt1.OutOn(1);
                    else
                        siriusEditorUserControl1.DOExt1.OutOff(1);
                    if (marker.IsError)
                        siriusEditorUserControl1.DOExt1.OutOn(2);
                    else
                        siriusEditorUserControl1.DOExt1.OutOff(2);
                    if (null != marker.Remote)
                    {
                        switch (marker.Remote.ControlMode)
                        {
                            case SpiralLab.Sirius2.Winforms.Remote.ControlModes.Local:
                                siriusEditorUserControl1.DOExt1.OutOff(3);
                                break;
                            case SpiralLab.Sirius2.Winforms.Remote.ControlModes.Remote:
                                siriusEditorUserControl1.DOExt1.OutOn(3);
                                break;
                        }
                    }
                    siriusEditorUserControl1.DOExt1.Update();
                }
            }
        }
        private void DIExt1_OnChanged(SpiralLab.Sirius2.IO.IDInput dInput, int bitNo, SignalEdges edge)
        {
            var marker = siriusEditorUserControl1.Marker;
            if (null == marker)
                return;
            if (null != marker.Remote)
            {
                if (marker.Remote.ControlMode == SpiralLab.Sirius2.Winforms.Remote.ControlModes.Local)
                {
                    Logger.Log(Logger.Types.Debug, $"external dio [{bitNo}]:{edge} has changed but control mode is LOCAL");
                    return;
                }
            }
            switch (bitNo)
            {
                case 0: //start
                    if (edge == SignalEdges.High)
                    {
                        Logger.Log(Logger.Types.Info, "trying to start marker by external dio");
                        marker.Ready(siriusEditorUserControl1.Document);
                        marker.Start();
                    }
                    break;
                case 1: //stop
                    if (edge == SignalEdges.High)
                    {
                        Logger.Log(Logger.Types.Info, "trying to stop marker by external dio");
                        marker.Stop();
                    }
                    break;
                case 2: //reset
                    if (edge == SignalEdges.High)
                    {
                        Logger.Log(Logger.Types.Info, "trying to reset marker by external dio");
                        marker.Reset();
                    }
                    break;
                case 15: //mark field correction by pre-cretaed sirius2 file
                    if (edge == SignalEdges.High)
                    {
                        Logger.Log(Logger.Types.Info, "trying to mark field corretion by external dio");
                        var doc = DocumentFactory.CreateDefault();
                        string fileName = Path.Combine(SpiralLab.Sirius2.Winforms.Config.RecipePath, "cal_100mm_5x5.sirius2");
                        if (!doc.ActOpen(fileName))
                            break;
                        marker.Ready(doc);
                        marker.Start();
                        // User MUST revert document as original on after marker has finished !
                        // by marker.Ready(siriusEditorUserControl1.Document, ...);
                    }
                    break;
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
                siriusEditorUserControl1.DIExt1.OnChanged -= DIExt1_OnChanged;
                timer.Enabled = false;
                timer.Dispose();

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