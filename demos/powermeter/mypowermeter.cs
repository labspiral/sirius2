/*
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
 * Description : User implemented powermeter
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.PowerMeter;

namespace Demos
{
    /// <summary>
    /// MyPowerMeter
    /// </summary>
    public class MyPowerMeter
        : PowerMeterBase
    {
        private System.Threading.Timer timer;
        private bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public MyPowerMeter()
            : base()
        {
            this.Name = "MyPowerMeter";
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Identifier</param>
        /// <param name="name">Name</param>
        public MyPowerMeter(int index, string name)
            : this()
        {
            this.Index = index;
            this.Name = name;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MyPowerMeter()
        {
            this.Dispose(false);
        }
     
        /// <inheritdoc/>  
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.CtlStop();
                if (disposing)
                {
                    this.timer?.Dispose();
                }
                this.disposed = true;
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc/>  
        public override bool Initialize()
        {            
            // Trying to initialize powermeter device ...
            //
            //

            this.IsError = false;
            this.IsReady = true;
            this.CtlClear();
            Logger.Log(Logger.Types.Info, $"powermeter[{this.Index}]: initialized");
            return true;
        }
        /// <inheritdoc/>  
        public override bool CtlStart(string category = "")
        {
            Debug.Assert(this.SamplingRateHz > 0);
            if (!string.IsNullOrEmpty(category))
                this.Category = category;
            if (this.IsBusy)
            {
                Logger.Log(Logger.Types.Warn, $"powermeter[{this.Index}]: trying to start but busy now");
                return true;
            }
            double secs = 1.0 / SamplingRateHz;
            this.timer?.Dispose();
            this.MeasuredPower = 0;

            this.timer = new System.Threading.Timer(OnTimer, null, 0, (int)(secs * 1000.0));
            this.IsBusy = true;
            this.IsReady = false;
            NotifyPropertyChanged("IsBusy");
            NotifyPropertyChanged("IsReady");
            NotifyStarted();
            Logger.Log(Logger.Types.Info, $"powermeter[{this.Index}]: started");
            return true;
        }
        /// <inheritdoc/>  
        public override bool CtlStop()
        {
            this.timer?.Dispose();
            this.IsBusy = false;
            this.IsReady = true;
            NotifyPropertyChanged("IsBusy");
            NotifyPropertyChanged("IsReady");
            NotifyStopped();
            Logger.Log(Logger.Types.Info, $"powermeter[{this.Index}]: stopped");
            return true;
        }
        /// <inheritdoc/>  
        public override void CtlClear()
        {
            lock (this.SyncRoot)
            {
                while (this.Data.TryDequeue(out var dummy))
                    ;
            }
            this.NotifyCleared();
        }
        /// <inheritdoc/>  
        public override void CtlReset()
        {
            lock (SyncRoot)
            {
                // Reset error 
                this.IsError = false;
                this.IsReady = true;
                NotifyPropertyChanged("IsError");
                NotifyPropertyChanged("IsReady");
            }
        }
        private void OnTimer(object state)
        {
            var dt = DateTime.Now;
            double watt = 0;

            // Get output laser power value by watt
            var rand = new Random();
            watt = Math.Round(10.0 * rand.NextDouble(), 3);

            lock (SyncRoot)
            {
                if (this.Data.Count > MaxQueueSize)
                    this.Data.TryDequeue(out var dummy);
                this.Data.Enqueue(watt);
                this.MeasuredPower = watt;
            }
            this.NotifyMeasure(dt, watt); 
        }
    }
}
