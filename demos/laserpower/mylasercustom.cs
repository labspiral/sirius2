﻿/*
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
 * Description : MyLaser5 Source (custom)
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

namespace Demos
{
    /// <summary>
    /// custom output power control
    /// </summary>
    public class MyLaserCustom
        : ILaser
        //, ILaserShutterControl
        //, ILaserGuideControl
        , ILaserPowerControl
    {
        /// <inheritdoc/>
        public virtual event PropertyChangedEventHandler PropertyChanged;
        /// <inheritdoc/>
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
        public virtual bool IsCommControl { get; protected set; }

        /// <inheritdoc/>
        [Browsable(false)]
        public virtual bool IsTimedOut { get; protected set; }

        /// <inheritdoc/>
        [Browsable(false)]
        public virtual bool IsProtocolError { get; protected set; }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual IRtc Rtc { get; set; }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual bool IsPowerControl { get; protected set; }

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
        public virtual bool IsShutterControl { get; protected set; }

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual bool IsGuideControl { get; protected set; }
        

        /// <inheritdoc/>  
        [Browsable(false)]
        public virtual object Tag { get; set; }

        private bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public MyLaserCustom()
        {
            this.SyncRoot = new object();
            this.Name = "MyLaser";
            this.IsPowerControl = true;
            this.PowerControlMethod = PowerControlMethod.Custom;
            this.PowerControlDelayTime = 1000;
            this.IsCommControl = false;
            this.IsShutterControl = false;
            this.IsGuideControl = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Identifier</param>
        /// <param name="name">Name</param>
        /// <param name="maxPowerWatt">Max power (W)</param>
        public MyLaserCustom(int index, string name, double maxPowerWatt)
            : this()
        {
            this.Index = index;
            this.Name = name;
            this.MaxPowerWatt = maxPowerWatt;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MyLaserCustom()
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
        /// Dispose internal resource
        /// </summary>
        /// <param name="disposing"></param>
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
        public virtual bool CtlPower(double watt)
        {
            Debug.Assert(this.MaxPowerWatt > 0);
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
                    case PowerControlMethod.Custom:
                        // do something you want to vary output laser power
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
            Debug.Assert(this.MaxPowerWatt > 0);
            if (null == Rtc)
                return true;
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
                    case PowerControlMethod.Custom:
                        // do something you want to vary output laser power
                        // for example
                        success &= this.Rtc.ListEnd();
                        success &= this.Rtc.ListExecute(true);
                        // vary laser output power 
                        // it takes some time
                        // ...
                        Thread.Sleep((int)this.PowerControlDelayTime);
                        //
                        // and then restart list buffer
                        success &= this.Rtc.ListBegin(this.Rtc.ListType);
                        break;
                }
                if (success)
                {
                    LastPowerWatt = watt;
                }
                return success;
            }
        }
        #endregion
    }
}
