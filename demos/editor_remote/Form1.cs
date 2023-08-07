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
using System.Reflection;
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
                    // user event for apply scanner field correction
                    SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection2DApply += Config_OnScannerFieldCorrection2DApply;
                    break;
                case "rtc6":
                    rtc = ScannerFactory.CreateRtc6(rtcId, kfactor, laserMode, signalLevel, signalLevel, correctionPath);
                    // user event for apply scanner field correction
                    SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection2DApply += Config_OnScannerFieldCorrection2DApply;
                    break;
                case "rtc6e":
                    rtc = ScannerFactory.CreateRtc6Ethernet(rtcId, "192.168.0.100", "255.255.255.0", kfactor, laserMode, signalLevel, signalLevel, correctionPath);
                    // user event for apply scanner field correction
                    SpiralLab.Sirius2.Winforms.Config.OnScannerFieldCorrection2DApply += Config_OnScannerFieldCorrection2DApply;
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
        private void CustomConverter()
        {
            var document = siriusEditorUserControl1.Document;
            SpiralLab.Sirius2.Winforms.Config.OnTextConvert += Text_OnTextConvert;
        }
        private string Text_OnTextConvert(IMarker marker, ITextConvertible textConvertible)
        {
            var entity = textConvertible as IEntity;
            switch (entity.Name)
            {
                case "MyText1":
                    // For example, convert to DateTime format. link: https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
                    // Like as "yyyyMMdd HH:mm:ss"
                    return DateTime.Now.ToString(textConvertible.SourceText);                    
            }
            return textConvertible.SourceText;
        }
        private bool Config_OnScannerFieldCorrection2DApply(SpiralLab.Sirius2.Winforms.UI.RtcCorrection2DForm form)
        {
            var ctFullFileName = form.RtcCorrection.TargetCorrectionFile;
            Debug.Assert(File.Exists(ctFullFileName));
            bool success = true;
            var currentTable = form.Rtc.PrimaryHeadTable;
            success &= form.Rtc.CtlLoadCorrectionFile(currentTable, ctFullFileName);
            success &= form.Rtc.CtlSelectCorrection(currentTable);
            Debug.Assert(success);
            MessageBox.Show($"New correction file: {ctFullFileName} has applied", "Scanner Field Correction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            var ctFileName = ctFullFileName.Replace(SpiralLab.Sirius2.Config.CorrectionPath + "\\", "");
            NativeMethods.WriteIni<string>(Program.ConfigFileName, "RTC", "CORRECTION", ctFileName);
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
            var marker = siriusEditorUserControl1.Marker;
            bool success = true;
            this.Invoke(new MethodInvoker(delegate ()
            {
                success &= doc.ActOpen(fileName);
                success &= marker.Ready(doc, siriusEditorUserControl1.View, siriusEditorUserControl1.Rtc, siriusEditorUserControl1.Laser);
            }));
            return success;
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
            bool success = true;
            this.Invoke(new MethodInvoker(delegate ()
            {
                var marker = siriusEditorUserControl1.Marker;
                marker.Offsets = offets;
                success &= marker.Start();
            }));
            return success;
        }
        /// <summary>
        /// Stop marker
        /// </summary>
        ///<returns>Success or failed</returns>
        public bool Stop()
        {
            var marker = siriusEditorUserControl1.Marker;
            bool success = true;
            this.Invoke(new MethodInvoker(delegate ()
            {
                success &= marker.Stop();
            }));
            return success;
        }
        /// <summary>
        /// Reset marker status
        /// </summary>
        /// <returns>Success or failed</returns>
        public bool Reset()
        {
            var marker = siriusEditorUserControl1.Marker;
            bool success = true;
            this.Invoke(new MethodInvoker(delegate ()
            {
                success &= marker.Reset();
            }));
            return success;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Find <c>IEntity</c> by name
        /// </summary>
        /// <param name="entityName">Entity name</param>
        /// <param name="entity">Founded <c>IEntity</c></param>
        /// <returns>Success or failed</returns>
        public bool EntityFind(string entityName, out IEntity entity)
        {
            var marker = siriusEditorUserControl1.Marker;
            var doc = marker.Document;
            Debug.Assert(doc != null);
            return doc.FindByName(entityName, out entity);
        }
        /// <summary>
        /// Find <c>EntityPen</c> by color
        /// </summary>
        /// <param name="color"><c>System.Drawing.Color</c> value at <c>Config.PensColor</c></param>
        /// <param name="entity">Founded <c>EntityPen</c></param>
        /// <returns>Success or failed</returns>
        public bool EntityFind(System.Drawing.Color color, out EntityPen entity)
        {
            var marker = siriusEditorUserControl1.Marker;
            var doc = marker.Document;
            Debug.Assert(doc != null);
            return doc.FindByPenColor(color, out entity);
        }
        /// <summary>
        /// Translate <c>IEntity</c> 
        /// </summary>
        /// <param name="entity">Target <c>IEntity</c></param>
        /// <param name="deltaXyz">Dx, Dy, Dz (mm)</param>
        /// <returns>Success or failed</returns>
        public bool EntityTranslate(IEntity entity, OpenTK.Vector3 deltaXyz)
        {
            var marker = siriusEditorUserControl1.Marker;
            var doc = marker.Document;
            Debug.Assert(doc != null);
            Debug.Assert(entity != null);
            bool success = true;
            this.Invoke(new MethodInvoker(delegate ()
            {
                success &= doc.ActTransit(new IEntity[] { entity }, deltaXyz);
            }));
            return success;
        }

        /// <summary>
        /// Query property list from <c>IEntity</c> 
        /// </summary>
        /// <remarks>
        /// Target properties are Browasable attribute is <c>True</c> only <br/>
        /// </remarks>
        /// <param name="entity">Target <c>IEntity</c> </param>
        /// <returns><c>Dictionary<string, object></c></returns>
        public Dictionary<string, object> EntityProperties(IEntity entity)
        {
            return PropertyList(entity);
            Dictionary<string, object> PropertyList(object objectType)
            {
                if (objectType == null) return new Dictionary<string, object>();
                Type t = objectType.GetType();
                PropertyInfo[] props = t.GetProperties();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (PropertyInfo prp in props)
                {
                    //Attribute [Browsable] is True only
                    if (prp.GetCustomAttributes<BrowsableAttribute>().Contains(BrowsableAttribute.Yes))
                    {
                        object value = prp.GetValue(objectType, new object[] { });
                        dic.Add(prp.Name, value);
                    }
                }
                return dic;
            }
        }
        /// <summary>
        /// Read property value at <c>IEntity</c> 
        /// </summary>
        /// <param name="entity">Target <c>IEntity</c></param>
        /// <param name="propName">Property name</param>
        /// <param name="propValue">Property value</param>
        /// <returns>Success or failed</returns>
        public bool EntityReadPropertyValue(IEntity entity, string propName, out object propValue)
        {
            propValue = null;
            Debug.Assert(entity != null);
            Type type = entity.GetType();
            var propInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            if (null == propInfo || !propInfo.CanRead)
                return false;
            propValue = propInfo.GetValue(entity);
            return true;
        }
        /// <summary>
        /// Write property value at <c>IEntity</c> 
        /// </summary>
        /// <param name="entity">Target <c>IEntity</c></param>
        /// <param name="propName">Property name</param>
        /// <param name="propValue">Property value</param>
        /// <returns>Success or failed</returns>
        public bool EntityWritePropertyValue(IEntity entity, string propName, object propValue)
        {
            Debug.Assert(entity != null);
            Type type = entity.GetType();
            var propInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            if (null == propInfo || !propInfo.CanWrite)
                return false;
            var convertedValue = Convert.ChangeType(propValue, propInfo.PropertyType);
            this.Invoke(new MethodInvoker(delegate ()
            {
                propInfo.SetValue(entity, convertedValue, null);
                // Regen data by forcily
                entity.Regen();
            }));
            return true;
        }
        #endregion
    }
}