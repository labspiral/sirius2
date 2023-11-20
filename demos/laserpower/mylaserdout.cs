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
 * Description : Laser Source (8bit digital output power control)
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
    /// 8bit digital output power control 
    /// </summary>
    public class MyLaserDOut
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
        public virtual LaserTypes LaserType { get { return LaserTypes.UserDefined1; } }

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


        #region Control by DigitalBits
        /// <summary>
        /// EXTENSION Port for DigitalBits
        /// <para>Default: 2(8bits) (1: 16bits)</para>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Control (digitalbits)")]
        [DisplayName("Extension port")]
        [Description("RTC Extension Port (1,2)")]
        public virtual int DigitalBitsPortNo
        {
            get { return digitalBitsPortNo; }
            set
            {
                if (value <= 0)
                    return;
                if (value > 2)
                    return;
                digitalBitsPortNo = value;
            }
        }
        protected int digitalBitsPortNo = 1;
        /// <summary>
        /// Min bit value for DigitalBits
        /// <para>Default: 0</para>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Control (digitalbits)")]
        [DisplayName("Min bit")]
        [Description("Min Bit Value")]
        public virtual ushort DigitalBitMinValue { get; set; } = 0;
        /// <summary>
        /// Max bit value for DigitalBits
        /// <para>Default: 255(or 65535)</para>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Control (DigitalBits)")]
        [DisplayName("Max Bit")]
        [Description("Max Bit Value")]
        public virtual ushort DigitalBitMaxValue { get; set; } = 65535;
        #endregion

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual object Tag { get; set; }

        private bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public MyLaserDOut()
        {
            this.SyncRoot = new object();
            this.Name = "MyLaser";
            this.IsPowerControl = true;
            this.PowerControlMethod = PowerControlMethods.DigitalBits;
            this.PowerControlDelayTime = 1;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Identifier</param>
        /// <param name="name">Name</param>
        /// <param name="maxPowerWatt">Max power (W)</param>
        /// <param name="minBitValue">Min bit value (For example: 0)</param>
        /// <param name="maxBitValue">Max bit value (For example: 255 or 65535)</param>
        public MyLaserDOut(int index, string name, double maxPowerWatt, ushort minBitValue, ushort maxBitValue)
            : this()
        {
            this.Index = index;
            this.Name = name;
            this.MaxPowerWatt = maxPowerWatt;
            this.DigitalBitMinValue = minBitValue;
            this.DigitalBitMaxValue = maxBitValue;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MyLaserDOut()
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
            LastPowerWatt = 0;
            return true;
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
        public virtual bool CtlPower(double targetWatt)
        {
            if (!this.IsPowerControl)
                return true;
            Debug.Assert(this.MaxPowerWatt > 0);
            var rtc = Scanner as IRtc;
            Debug.Assert(rtc != null);
            bool success = true;
            if (targetWatt > this.MaxPowerWatt)
                targetWatt = this.MaxPowerWatt;
            lock (SyncRoot)
            {
                double percentage = targetWatt / this.MaxPowerWatt * 100.0;
                if (percentage > 100)
                    percentage = 100;

                switch (this.PowerControlMethod)
                {
                    default:
                        Logger.Log(Logger.Types.Error, $"laser [{this.Index}]: unsupported !");
                        return false;
                    case PowerControlMethods.DigitalBits:
                        double dataBits = this.DigitalBitMinValue + (this.DigitalBitMaxValue - this.DigitalBitMinValue) * percentage / 100.0;
                        if (1 == this.DigitalBitsPortNo)
                        {
                            success &= rtc.CtlWriteData<uint>(ExtensionChannels.ExtDO16, (uint)dataBits);
                        }
                        else if (2 == this.DigitalBitsPortNo)
                        {
                            success &= rtc.CtlWriteData<uint>(ExtensionChannels.ExtDO8, (uint)dataBits);
                        }
                        break;
                }
            }
            Thread.Sleep((int)this.PowerControlDelayTime);
            if (success)
            {
                LastPowerWatt = targetWatt;
                Logger.Log(Logger.Types.Warn, $"laser [{this.Index}]: power: {targetWatt:F3} / {MaxPowerWatt:F3}W");
            }
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
        public virtual bool ListPower(double targetWatt)
        {
            if (!this.IsPowerControl)
                return true;
            Debug.Assert(this.MaxPowerWatt > 0);
            var rtc = Scanner as IRtc;
            Debug.Assert(rtc != null);
            if (targetWatt > this.MaxPowerWatt)
                targetWatt = this.MaxPowerWatt;
            bool success = true;
            lock (SyncRoot)
            {
                double percentage = targetWatt / this.MaxPowerWatt * 100.0;
                if (percentage > 100)
                    percentage = 100;

                switch (this.PowerControlMethod)
                {
                    default:
                        Logger.Log(Logger.Types.Error, $"laser [{this.Index}]: unsupported !");
                        return false;
                    case PowerControlMethods.DigitalBits:
                        double dataBits = this.DigitalBitMinValue + (this.DigitalBitMaxValue - this.DigitalBitMinValue) * percentage / 100.0;
                        if (1 == this.DigitalBitsPortNo)
                        {
                            success &= rtc.ListWriteData<uint>(ExtensionChannels.ExtDO16, (uint)dataBits);
                        }
                        else if (2 == this.DigitalBitsPortNo)
                        {
                            success &= rtc.ListWriteData<uint>(ExtensionChannels.ExtDO8, (uint)dataBits);
                        }
                        success &= rtc.ListWait(this.PowerControlDelayTime);
                        break;
                }
            }
            if (success)
                LastPowerWatt = targetWatt;
            return success;
        }
        #endregion
    }
}
