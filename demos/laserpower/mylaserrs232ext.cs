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
 * Description : Laser Source (External RS-232C communication power control)
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.PowerMap;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;

namespace Demos
{
    /// <summary>
    /// External RS232C communication to power control
    /// </summary>
    /// <remarks>Used with external RS232C(or COM) port at PC</remarks>
    public class MyLaserRS232External
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
                    receiver.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        public virtual LaserTypes LaserType { get { return LaserTypes.UserDefined1; } }

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Power Control")]
        [DisplayName("Map")]
        [Description("Assigned PowerMap")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual IPowerMap PowerMap { get; set; }

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
                        Logger.Log(Logger.Types.Info, $"laser [{this.Index}]: error occurs");
                    }
                }
            }
        }
        protected bool isError;

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual IScanner Scanner { get; set; }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual bool IsPowerControl { get; protected set; }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual bool IsGuideControl { get; protected set; }

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Power Control")]
        [DisplayName("Power (max)")]
        [Description("Max Power (W)")]
        public virtual double MaxPowerWatt { get; set; }

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Power Control")]
        [DisplayName("Power (last)")]
        [Description("Last Commanded Power (W)")]
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

        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Power Control")]
        [DisplayName("Method")]
        [Description("Laser Power Control Method")]
        public virtual PowerControlMethods PowerControlMethod { get; set; }

        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Power Control")]
        [DisplayName("Delay")]
        [Description("Power Control Delay Time (msec)")]
        public virtual double PowerControlDelayTime { get; set; }

        /// <summary>
        /// COM port no
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Power Control")]
        [DisplayName("COM")]
        [Description("COM Port No")]
        public virtual int ComPort { get; protected set; }

        /// <summary>
        /// <see cref="SerialPort">SerialPort</see> instance
        /// </summary>
        [Browsable(false)]
        protected System.IO.Ports.SerialPort serialPort;


        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual object Tag { get; set; }

        private bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public MyLaserRS232External()
        {
            this.SyncRoot = new object();
            this.Name = "MyLaser";
            this.IsPowerControl = true;
            // RTC RS232 Port
            this.PowerControlMethod = PowerControlMethods.Custom;
            this.PowerControlDelayTime = 200;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Identifier</param>
        /// <param name="name">Name</param>
        /// <param name="maxPowerWatt">Max power (W)</param>
        /// <param name="comPort">COM port no</param>
        public MyLaserRS232External(int index, string name, double maxPowerWatt, int comPortNo)
            : this()
        {
            this.Index = index;
            this.Name = name;
            this.MaxPowerWatt = maxPowerWatt;
            this.ComPort = comPortNo;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MyLaserRS232External()
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
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.serialPort?.Close();
                    this.serialPort?.Dispose();
                }
                this.disposed = true;
            }
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
            bool success = true;
            try
            {
                this.serialPort?.Close();
                this.serialPort?.Dispose();
                // By your communication spec at here
                this.serialPort = new SerialPort();
                this.serialPort.PortName = $"COM{ComPort}";
                this.serialPort.BaudRate = (int)57600;
                this.serialPort.DataBits = (int)8;
                this.serialPort.Parity = Parity.None;
                this.serialPort.StopBits = StopBits.One;
                this.serialPort.ReadTimeout = (int)200;
                this.serialPort.WriteTimeout = (int)200;
                this.serialPort.Open();
                Logger.Log(Logger.Types.Info, $"laser [{this.Index}]: initialized COM port= {this.ComPort} ");
            }
            catch (Exception ex)
            {
                success = false;
                Logger.Log(Logger.Types.Error, $"laser [{this.Index}]: fail to initialize {ex.Message} at COM port= {this.ComPort}");
                return false;
            }

            LastPowerWatt = 0;
            return success;
        }
        /// <inheritdoc/>  
        public virtual bool CtlAbort()
        {
            return true;
        }
        /// <inheritdoc/>  
        public virtual bool CtlReset()
        {
            lock (SyncRoot)
            {
                IsError = false;
                return true;
            }
        }

        #region ILaserPowerControl impl
        /// <inheritdoc/>  
        public virtual bool CtlPower(double targetWatt, string category = "")
        {
            Debug.Assert(this.MaxPowerWatt > 0);
            var rtc = Scanner as IRtc;
            Debug.Assert(rtc != null);
            bool success = true;
            double compensatedWatt = targetWatt;
            if (null != PowerMap && !string.IsNullOrEmpty(category))
            {
                success &= PowerMap.LookUp(category, targetWatt, out compensatedWatt, out double x1, out double x2);
                if (!success)
                    return false;
            }
            success &= CommandToChangePower(compensatedWatt);
            return success;
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
        public virtual bool ListPower(double targetWatt, string category = "")
        {
            Debug.Assert(this.MaxPowerWatt > 0);
            var rtc = Scanner as IRtc;
            Debug.Assert(rtc != null);
            double compensatedWatt = targetWatt;
            bool success = true;
            if (null != PowerMap && !string.IsNullOrEmpty(category))
            {
                success &= PowerMap.LookUp(category, targetWatt, out compensatedWatt, out double x1, out double x2);
                if (!success)
                    return false;
            }
            // Flush(or wait until finished) list commands at RTC buffer 
            var listType = rtc.ListType;
            success &= rtc.ListEnd();
            success &= rtc.ListExecute(true);
            if (!success)
                return false;
            success &= CommandToChangePower(compensatedWatt);
            // Start list buffer at RTC again
            if (success)
                success &= rtc.ListBegin(listType);
            return success;
        }
        #endregion

        private bool CommandToChangePower(double watt)
        {
            if (null == this.serialPort)
                return false;
            if (!this.serialPort.IsOpen)
                return false;
            bool success = true;
            // By communication protocol by specific laser source vendor
            byte[] cmd = { 0x00, 0x01, 0x02 };
            try
            {
                this.serialPort.Write(cmd, 0, cmd.Length);
                // Waiting for some delay time
                Thread.Sleep((int)this.PowerControlDelayTime);
                // this.serialPort.Read ...to confirm
            }
            catch (System.TimeoutException ex)
            {
                IsError = true;
                Logger.Log(Logger.Types.Error, $"laser [{this.Index}]: fail to receive data. timed out");
                success &= false;
            }
            catch (System.Exception ex)
            {
                Logger.Log(Logger.Types.Error, ex);
                success &= false;
            }
          
            if (success)
            {
                LastPowerWatt = watt;
                Logger.Log(Logger.Types.Warn, $"laser [{this.Index}]: power(W): {watt:F3}");
            }
            else
                Logger.Log(Logger.Types.Error, $"fail to change laser power to {watt:F3}W");
            return success;
        }
    }
}
