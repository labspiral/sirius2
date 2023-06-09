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
 * Description : Custom laser
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2;

namespace Demos
{
    /// <summary>
    /// MyLaser 
    /// </summary>
    /// <remarks>
    /// For example, controlled laser power by analog voltage output 
    /// </remarks>
    public class MyLaser
        : LaserBase
        , ILaserPowerControl
    {
        /// <inheritdoc/>  
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Basic")]
        [DisplayName("Type")]
        [Description("Type")]
        public override LaserType LaserType { get { return LaserType.UserDefined1; } }

        #region Control by analog
        /// <summary>
        /// Min Analog Voltage (V)
        /// <para>Default: 0</para>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Control (Analog)")]
        [DisplayName("Min Voltage")]
        [Description("Min Voltage (V)")]
        public virtual double MinVoltage
        {
            get { return minVoltage; }
            set
            {
                if (value < 0)
                    return;
                minVoltage = value;
                if (minVoltage > 10)
                    minVoltage = 10;
                if (minVoltage < 10)
                    minVoltage = 0;
            }
        }
        private double minVoltage = 0;
        /// <summary>
        /// Max Analog Voltage (V)
        /// <para>Default: 10</para>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Control (Analog)")]
        [DisplayName("Max Voltage")]
        [Description("Max Voltage (V)")]
        public virtual double MaxVoltage 
        { 
            get { return maxVoltage; }
            set
            {
                if (value < 0)
                    return;
                maxVoltage = value;
                if (maxVoltage > 10)
                    maxVoltage = 10;
            }
        }
        private double maxVoltage = 10;
        /// <summary>
        /// Port for Analog 
        /// <para>Default: 1</para>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Control (Analog)")]
        [DisplayName("Anglog Port")]
        [Description("RTC Analog Port (1,2)")]
        public virtual int AnalogPortNo
        {
            get { return analogPortNo; }
            set
            {
                if (value < 1 || value > 2)
                    return;
                analogPortNo = value;
            }
        }
        private int analogPortNo = 1;
        #endregion

        private bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public MyLaser()
            : base()
        {
            this.Name = "MyLaser";
            this.IsPowerControl = true;
            this.PowerControlMethod = PowerControlMethod.Custom;
            this.PowerControlDelayTime = 0;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Identifier</param>
        /// <param name="name">Name</param>
        /// <param name="maxPowerWatt">Max power (W)</param>
        /// <param name="powerControlMethod"><c>PowerControlMethod</c></param>
        public MyLaser(int index, string name, double maxPowerWatt)
            : this()
        {
            this.Index = index;
            this.Name = name;
            this.MaxPowerWatt = maxPowerWatt;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MyLaser()
        {
            this.Dispose(false);
        }
        /// <inheritdoc/>  
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing)
            {
                IsReady = false;
            }
            this.disposed = true;
            base.Dispose(disposing);
        }

        /// <inheritdoc/>  
        public override bool Initialize()
        {
            lock (SyncRoot)
            {
                IsReady = true;
                IsBusy = false;
                IsError = false;
                LastPowerWatt = 0; 
                return true;
            }
        }
        /// <inheritdoc/>  
        public override bool CtlAbort()
        {
            lock (SyncRoot)
            {
                IsReady = false;
                IsBusy = false;
                IsError = true;
            }
            return true;
        }
        /// <inheritdoc/>  
        public override bool CtlReset()
        {
            lock (SyncRoot)
            {
                IsError = false;
                IsReady = true;
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

                double dataVoltage = percentage / 100.0 * (this.MaxVoltage - this.MinVoltage) + this.MinVoltage;
                if (1 == this.AnalogPortNo)
                    success &= this.Rtc.CtlWriteData<double>(ExtensionChannel.ExtAO1, dataVoltage);
                else if (2 == this.AnalogPortNo)
                    success &= this.Rtc.CtlWriteData<double>(ExtensionChannel.ExtAO2, dataVoltage);

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
        public override bool ListBegin()
        {
            return true;
        }
        /// <inheritdoc/>  
        public override bool ListEnd()
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

                double dataVoltage = percentage / 100.0 * (this.MaxVoltage - this.MinVoltage) + this.MinVoltage;
                if (1 == this.AnalogPortNo)
                    success &= this.Rtc.ListWriteData<double>(ExtensionChannel.ExtAO1, dataVoltage);
                else
                    success &= this.Rtc.ListWriteData<double>(ExtensionChannel.ExtAO2, dataVoltage);
                success &= this.Rtc.ListWait(this.PowerControlDelayTime);
                       
                if (success)
                    LastPowerWatt = watt;
                return success;
            }
        }
        #endregion
    }
}
