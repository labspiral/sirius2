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
 * Description : Example sirius2 editor for customized marker
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using OpenTK;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Scanner.Rtc.SyncAxis;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.UI;

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

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Assign marker event handler
            marker.OnStarted += Marker_OnStarted;
            marker.OnEnded += Marker_OnEnded;
            marker.OnBeforeLayer += Marker_OnBeforeLayer;
            marker.OnAfterLayer += Marker_OnAfterLayer;

            var document = siriusEditorUserControl1.Document;
            var view = siriusEditorUserControl1.View;
            // Create entities for test
            //EditorHelper.CreateTestEntities(rtc, document);

            // Override mark routine for each pen color
            //marker.OnMarkPen += Marker_OnMarkPen;

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser, powerMeter, remote);
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
        private void CreateCustomMarker(out IMarker marker)
        {
            marker = null;
            var rtcType = NativeMethods.ReadIni(EditorHelper.ConfigFileName, "RTC", "TYPE", "Rtc5");
            switch (rtcType.Trim().ToLower())
            {
                default:
                    marker = new MyRtcMarker(0, "MyMarker");// Create custom marker for RTC5,6
                    break;
                case "syncaxis":
                    marker = new MySyncAxisMarker(0, "MyMarker");// Create custom marker for syncAXIS
                    break;
            }
        }

        #region Marker events
        private void Marker_OnStarted(IMarker marker)
        {
            // marker has started
        }
        private bool Marker_OnBeforeLayer(IMarker marker, EntityLayer layer)
        {
            // do somethings before mark layer
            return true;
        }
        private bool Marker_OnAfterLayer(IMarker marker, EntityLayer layer)
        {
            // do somethings after mark layer
            return true;
        }
        private void Marker_OnEnded(IMarker marker, bool success, TimeSpan timeSpan)
        {
            // marker has ended
        }

        /// <summary>
        /// Override mark routine for each <c>EntityPen</c>
        /// </summary>
        /// <param name="marker"><c>IMarker</c></param>
        /// <param name="pen"><c>EntityPen</c></param>
        /// <returns></returns>
        private bool Marker_OnMarkPen(IMarker marker, EntityPen pen)
        {
            bool success = true;
            // simplified routine 
            //var rtc = marker.Rtc;
            //var laser = marker.Laser;
            //var rtcExtension = rtc as IRtcExtension;
            //var rtcSkywriting = rtc as IRtcSkyWriting;
            //var rtcWobbel = rtc as IRtcWobbel;
            // if (laser is ILaserPowerControl laserPowerControl)
            //     success &= laserPowerControl.ListPower(pen.Power, pen.PowerMapCategory);
            //success &= rtc.ListDelay(pen.LaserOnDelay, pen.LaserOffDelay, pen.ScannerJumpDelay, pen.ScannerMarkDelay, pen.ScannerPolygonDelay);
            //success &= rtc.ListSpeed(pen.JumpSpeed, pen.MarkSpeed);
            //success &= rtc.ListFirstPulseKiller(pen.LaserFpk);
            //if (null != rtcExtension)
            //    success &= rtcExtension.ListQSwitchDelay(pen.LaserQSwitchDelay);
            //if (null != rtcSkywriting)
            //{
            //    double cosineLimit = Math.Cos(Math.PI / 180.0 * pen.AngularLimit);
            //    if (pen.IsSkyWritingEnabled)
            //        success &= rtcSkywriting.ListSkyWritingBegin(pen.SkyWritingMode, pen.LaserOnShift, pen.TimeLag, pen.Prev, pen.Post, cosineLimit);
            //    else
            //        success &= rtcSkywriting.ListSkyWritingEnd();
            //}
            //if (null != rtcWobbel)
            //{
            //    if (pen.IsWobbelEnabled)
            //        success &= rtcWobbel.ListWobbelBegin(pen.WobbelParallel, pen.WobbelPerpendicular, pen.WobbelFrequency, pen.WobbelShape);
            //    else
            //        success &= rtcWobbel.ListWobbelEnd();
            //}
            // ...
            return success;
        }
        #endregion
    }
}
