using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Marker;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demos
{
    internal static class Program
    {
        /// <summary>
        /// Your config ini file
        /// </summary>
        public static string ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
        // Used this config file if using XL-SCAN (syncAXIS)
        // public static string IniFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config_syncaxis.ini");

        public static SiriusEditorUserControl EditorForm { get; set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize sirius2 library
            SpiralLab.Sirius2.Core.Initialize();
        
            // Create main form
            EditorForm = new SiriusEditorUserControl();

            // Create devices
            CreateDevices();

            // Create marker
            CreateMarker();
            
            // Main form
            Application.Run(EditorForm);

            // Dispose 
            DestroyDevices();
        }

        private static void CreateDevices()
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
            EditorForm.Rtc = rtc;
            EditorForm.Laser = laser;
        }
        private static void CreateMarker()
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
            EditorForm.Marker = marker;
            // Assign Document, Rtc, Laser at IMarker
            marker.Ready(EditorForm.Document, EditorForm.View, EditorForm.Rtc, EditorForm.Laser);
        }

        private static void DestroyDevices()
        {
            var marker = EditorForm.Marker;
            var laser = EditorForm.Laser;
            var rtc = EditorForm.Rtc;
            marker.Stop();
            marker.Dispose();
            laser.Dispose();
            rtc.Dispose();
        }
    }
}
