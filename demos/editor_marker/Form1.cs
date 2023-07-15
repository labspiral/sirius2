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
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
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

            // Event will be fired when select scanner field correction 2d at popup-menu
            SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection2D += Config_OnScannerFieldCorrection2D;
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

            marker.OnStarted -= Marker_OnStarted;
            marker.OnFinished -= Marker_OnFinished;
            marker.OnFailed -= Marker_OnFailed;
            marker.OnBeforeLayer -= Marker_OnBeforeLayer;
            marker.OnAfterLayer -= Marker_OnAfterLayer;

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
            switch (rtcType.Trim().ToLower())
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
            // Create custom laser source
            float maxWatt = 20;
            ILaser laser = new MyLaser(0, "MyLaser", maxWatt);

            // Assign RTC into laser source
            laser.Rtc = rtc;

            // Initialize laser source
            success &= laser.Initialize();

            // Default output 10% power
            if (laser is ILaserPowerControl powerControl)
                success &= powerControl.CtlPower(maxWatt * 0.1);
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
                default:
                    marker = new MyRtcMarker(0, "MyMarker");// Create custom marker for RTC5,6
                    break;
                case "syncaxis":
                    marker = new MySyncAxisMarker(0, "MyMarker");// Create custom marker for syncAXIS
                    break;
            }

            // Assign instances to user control
            siriusEditorUserControl1.Marker = marker;

            // Assign Document, Rtc, Laser at IMarker
            marker.Ready(siriusEditorUserControl1.Document, siriusEditorUserControl1.View, siriusEditorUserControl1.Rtc, siriusEditorUserControl1.Laser);

            marker.OnStarted += Marker_OnStarted;
            marker.OnFinished += Marker_OnFinished;
            marker.OnFailed += Marker_OnFailed;
            marker.OnBeforeLayer += Marker_OnBeforeLayer;
            marker.OnAfterLayer += Marker_OnAfterLayer;
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
        private void Marker_OnFailed(IMarker marker, TimeSpan timeSpan)
        {
            // marker has failed
        }
        private void Marker_OnFinished(IMarker marker, TimeSpan timeSpan)
        {
            // marker has finished
        }
        #endregion

        private RtcCorrection2D Config_OnScannerFieldCorrection2D(IRtc rtc)
        {
            // Measured x,y error data
            int rows = 7;
            int cols = 7;
            float interval = 10.0f;
            var rtcCorrection2D = new RtcCorrection2D(rtc.KFactor, rows, cols, interval, interval, rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName, string.Empty);
            float left = -interval * (float)(int)(cols / 2);
            float top = interval * (float)(int)(rows / 2);
            var rand = new Random();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    rtcCorrection2D.AddRelative(row, col,
                        new System.Numerics.Vector2(left + col * interval, top - row * interval),
                        new System.Numerics.Vector2(
                            rand.Next(20) / 1000.0f - 0.01f,
                            rand.Next(20) / 1000.0f - 0.01f)
                        );
                }
            }
            return rtcCorrection2D;
        }

        #region Control by remotely
        /// <summary>
        /// Ready status
        /// </summary>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
        public bool Stop()
        {
            var marker = siriusEditorUserControl1.Marker;
            return marker.Stop();
        }
        /// <summary>
        /// Reset marker status
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            var marker = siriusEditorUserControl1.Marker;
            return marker.Reset();
        }
        #endregion
    }
}
