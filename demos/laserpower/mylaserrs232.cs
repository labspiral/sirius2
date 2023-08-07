/*
 *                                                             ,--,      ,--,                              
 *              ,-.----.                                     ,---.'|   ,---.'|                              
 *    .--.--.   \    /  \     ,---,,-.----.      ,---,       |   | :   |   | :      ,---,           ,---,.  
 *   /  /    '. |   :    \ ,`--.' |\    /  \    '  .' \      :   : |   :   : |     '  .' \        ,'  .'  \ 
 *  |  :  /`. / |   |  .\ :|   :  :;   :    \  /  ;    '.    |   ' :   |   ' :    /  ;    '.    ,---.' .' | 
 *  ;  |  |--`  .   :  |: |:   |  '|   | .\ : :  :       \   ;   ; '   ;   ; '   :  :       \   |   |  |: | 
 *  |  :  ;_    |   |   \ :|   :  |.   : |: | :  |   /\   \  '   | |__ '   | |__ :  |   /\   \  :   :  :  / 
 *   \  \    `. |   : .   /'   '  ;|   |  \ : |  :  ' ;.   : |   | :.'||   | :.'||  :  ' ;.   : :   |    ;  
 *    `----.   \;   | |`-' |   |  ||   : .  / |  |  ;/  \   \'   :    ;'   :    ;|  |  ;/  \   \|   :     \ 
 *    __ \  \  ||   | ;    '   :  ;;   | |  \ '  :  | \  \ ,'|   |  ./ |   |  ./ '  :  | \  \ ,'|   |   . | 
 *   /  /`--'  /:   ' |    |   |  '|   | ;\  \|  |  '  '--'  ;   : ;   ;   : ;   |  |  '  '--'  '   :  '; | 
 *  '--'.     / :   : :    '   :  |:   ' | \.'|  :  :        |   ,/    |   ,/    |  :  :        |   |  | ;  
 *    `--'---'  |   | :    ;   |.' :   : :-'  |  | ,'        '---'     '---'     |  | ,'        |   :   /   
 *              `---'.|    '---'   |   |.'    `--''                              `--''          |   | ,'    
 *                `---`            `---'                                                        `----'   
 * 
 * 2023 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : MyLaser4 Source (8bit digital output power control)
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;

namespace Demos
{
    /// <summary>
    /// RS232 communication to power control
    /// </summary>
    /// <remarks>Used with RS232 port at RTC card</remarks>
    public class MyLaserRS232
        : ILaser
        , ILaserPowerControl
    {
        /// <inheritdoc/>
        public virtual event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Nofity property value has changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var receivers = this.PropertyChanged?.GetInvocationList();
            if (null != receivers)
                foreach (PropertyChangedEventHandler receiver in receivers)
                    receiver.BeginInvoke(this, new PropertyChangedEventArgs(propertyName), null, null);
        }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual object SyncRoot { get; protected set; }

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Basic")]
        [DisplayName("Index")]
        [Description("Identifier")]
        public virtual int Index { get; set; }

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Basic")]
        [DisplayName("Name")]
        [Description("Name")]
        public virtual string Name { get; set; }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual LaserType LaserType { get { return LaserType.UserDefined1; } }

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Basic")]
        [DisplayName("Max Power")]
        [Description("Max Power (W)")]
        public virtual double MaxPowerWatt { get; set; }

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Status")]
        [DisplayName("Ready")]
        [Description("Ready Status")]
        public virtual bool IsReady
        {
            get { return isReady; }
            protected set
            {
                if (isReady != value)
                {
                    isReady = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        protected bool isReady;

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Status")]
        [DisplayName("Busy")]
        [Description("Busy Status")]
        public virtual bool IsBusy
        {
            get { return isBusy; }
            protected set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        protected bool isBusy;

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Status")]
        [DisplayName("Error")]
        [Description("Error Status")]
        public virtual bool IsError
        {
            get { return isError; }
            protected set
            {
                if (isError != value)
                {
                    isError = value;
                    this.NotifyPropertyChanged();
                    if (isError)
                    {
                        Logger.Log(Logger.Type.Info, $"laser [{this.Index}]: error occurs");
                    }
                }
            }
        }
        protected bool isError;

        /// <inheritdoc/>
        [Browsable(false)]
        public virtual bool IsTimedOut { get; protected set; }

        /// <inheritdoc/>
        [Browsable(false)]
        public virtual bool IsProtocolError { get; protected set; }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual IScanner Scanner { get; set; }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual bool IsPowerControl { get; set; }

        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Control")]
        [DisplayName("Power Control Method")]
        [Description("Power Control Method")]
        public virtual PowerControlMethod PowerControlMethod { get; set; }

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Control")]
        [DisplayName("Power Control Delay")]
        [Description("Power Control Delay Time (msec)")]
        public virtual double PowerControlDelayTime { get; set; }

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Control")]
        [DisplayName("Last Power (W)")]
        [Description("Commanded Last Output Power (W)")]
        public virtual double LastPowerWatt
        {
            get { return laserPowerWatt; }
            protected set
            {
                if (laserPowerWatt != value)
                {
                    laserPowerWatt = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        protected double laserPowerWatt;

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual bool IsCommControl { get; protected set; }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual bool IsShutterControl { get; protected set; }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual bool IsGuideControl { get; protected set; }


        /// <summary>
        /// Data format for RS-232 communication
        /// <para>string.Format</para>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Control (RS-232)")]
        [DisplayName("Format")]
        [Description("Data Format")]
        public virtual string Rs232StringFormat
        {
            get { return rs232StringFormat; }
            set
            {
                rs232StringFormat = value;
            }
        }
        protected string rs232StringFormat = "Current={0:F3}";

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual object Tag { get; set; }

        private bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public MyLaserRS232()
        {
            this.SyncRoot = new object();
            this.Name = "MyLaser";
            this.IsPowerControl = true;
            // RTC RS232 Port
            this.PowerControlMethod = PowerControlMethod.Rs232;
            this.PowerControlDelayTime = 200;
            this.IsCommControl = true;
            this.IsShutterControl = false;
            this.IsGuideControl = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Identifier</param>
        /// <param name="name">Name</param>
        /// <param name="maxPowerWatt">Max power (W)</param>
        public MyLaserRS232(int index, string name, double maxPowerWatt)
            : this()
        {
            this.Index = index;
            this.Name = name;
            this.MaxPowerWatt = maxPowerWatt;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MyLaserRS232()
        {
            this.Dispose(false);
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Dispose internal resources
        /// </summary>
        /// <param name="disposing">Explicit dispose or not</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing)
            {
            }
            this.disposed = true;
        }
        /// <inheritdoc/> 
        public virtual bool CheckErrors()
        {
            //IsError = 
            return true;
        }
        /// <inheritdoc/> 
        public virtual bool CheckReady()
        {
            //IsReady = 
            return true;
        }
        /// <inheritdoc/> 
        public virtual bool CheckBusy()
        {
            //IsBusy = 
            return true;
        }

        /// <inheritdoc/>  
        public bool Initialize()
        {
            var rtc = Scanner as IRtc;
            Debug.Assert(rtc != null);
            var rtcSerialComm = rtc as IRtcSerialComm;
            Debug.Assert(null != rtcSerialComm);
            //config baudrate
            bool success = rtcSerialComm.CtlSerialConfig(9600);

            Debug.Assert(success);

            LastPowerWatt = 0;
            return success;
        }
        /// <inheritdoc/>  
        public virtual bool CtlAbort()
        {
            lock (SyncRoot)
            {
                //prevPowerWatt = 0;
            }
            return true;
        }
        /// <inheritdoc/>  
        public virtual bool CtlReset()
        {
            lock (SyncRoot)
            {
                //IsTimedOut = false;
                IsError = false;
                return true;
            }
        }

        #region ILaserPowerControl impl
        /// <inheritdoc/>  
        public virtual bool CtlPower(double watt)
        {
            if (!this.IsPowerControl)
                return true;
            Debug.Assert(this.MaxPowerWatt > 0);
            var rtc = Scanner as IRtc;
            Debug.Assert(rtc != null);
            bool success = true;
            if (watt > this.MaxPowerWatt)
                watt = this.MaxPowerWatt;
            lock (SyncRoot)
            {
                double percentage = watt / this.MaxPowerWatt * 100.0;
                if (percentage > 100)
                    percentage = 100;

                switch (this.PowerControlMethod)
                {
                    default:
                        Logger.Log(Logger.Type.Error, $"laser [{this.Index}]: unsupported !");
                        return false;
                    case PowerControlMethod.Rs232:
                        if (rtc is IRtcSerialComm rtcSerialComm)
                        {
                            //to cleanup recv buffer
                            rtcSerialComm.CtlSerialRead(out byte[] dummy);
                            string text = string.Format(rs232StringFormat, percentage);
                            success &= rtcSerialComm.CtlSerialWrite(text);
                            //byte[] bytes = Encoding.UTF8.GetBytes(text);
                            //success &= rtcSerialComm.CtlSerialWrite(bytes);
                        }
                        break;
                }
                Thread.Sleep((int)this.PowerControlDelayTime);
                if (success)
                {
                    LastPowerWatt = watt;
                    Logger.Log(Logger.Type.Warn, $"laser [{this.Index}]: power: {watt:F3} / {MaxPowerWatt:F3}W");
                }
                return success;
            }
        }

        /// <inheritdoc/>  
        public virtual bool ListBegin()
        {
            return true;
        }
        /// <inheritdoc/>  
        public virtual bool ListEnd()
        {
            return true;
        }
        /// <inheritdoc/>  
        public virtual bool ListPower(double watt)
        {
            if (!this.IsPowerControl)
                return true;
            Debug.Assert(this.MaxPowerWatt > 0);
            var rtc = Scanner as IRtc;
            Debug.Assert(rtc != null);
            if (watt > this.MaxPowerWatt)
                watt = this.MaxPowerWatt;
            lock (SyncRoot)
            {
                bool success = true;
                double percentage = watt / this.MaxPowerWatt * 100.0;
                if (percentage > 100)
                    percentage = 100;

                switch (this.PowerControlMethod)
                {
                    default:
                        Logger.Log(Logger.Type.Error, $"laser [{this.Index}]: unsupported !");
                        return false;
                case PowerControlMethod.Rs232:
                    if (rtc is IRtcSerialComm rtcSerialComm)
                    {
                        string text = string.Format(rs232StringFormat, percentage);
                        success &= rtcSerialComm.ListSerialWrite(text);
                        //byte[] bytes = Encoding.UTF8.GetBytes(text);
                        //success &= rtcSerialComm.ListSerialWrite(bytes);
                        success &= rtc.ListWait(this.PowerControlDelayTime);
                    }
                    break;
                }
                if (success)
                    LastPowerWatt = watt;
                return success;
            }
        }
        #endregion
    }
}
