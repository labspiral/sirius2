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
 * Description : Custom marker for syncAXIS
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 */

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Collections.Concurrent;
using System.Collections;
using SpiralLab.Sirius2.Common;
using SpiralLab.Sirius2.Mathematics;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.OpenGL;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Scanner.Rtc.SyncAxis;

namespace SpiralLab.Sirius2.Winforms.Marker
{
    /// <summary>
    /// Custom marker for syncAXIS
    /// </summary>
    public class MySyncAxisMarker
        : MarkerBase
    {
        /// <summary>
        /// Op status 
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("SyncAXIS")]
        [DisplayName("Status")]
        [Description("Operation Status")]
        public System.Drawing.Color OperationStatusColor { get; protected set; }

        /// <summary>
        /// Is plot simulation output to syncAxis viewer
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("SyncAXIS")]
        [DisplayName("Plot")]
        [Description("Plot Simulated Output by syncAXIS")]
        public virtual bool IsMeasurementPlot
        {
            get { return isMeasurementPlot; }
            set
            {
                if (!File.Exists(SpiralLab.Sirius2.Config.SyncAxisViewerProgramPath))
                {
                    MessageBox.Show($"syncaxis viewer program is not exist at '{SpiralLab.Sirius2.Config.MeasurementGNUPlotProgramPath}'", "Warning", MessageBoxButtons.OK);
                    return;
                }
                isMeasurementPlot = value;
            }
        }
        /// <summary>
        /// Is plot measurement session to graph or not
        /// </summary>
        protected bool isMeasurementPlot;

        /// <summary>
        /// Internal marker thread 
        /// </summary>
        protected Thread thread;
        /// <summary>
        /// list of layers to mark
        /// </summary>
        protected List<EntityLayer> layers;
        System.Windows.Forms.Timer timerStatus = new System.Windows.Forms.Timer();
        private bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public MySyncAxisMarker()
              : base()
        {
            isMeasurementPlot = true;
            IsJumpToOriginAfterFinished = true;
            OperationStatusColor = Color.DarkGray;
            timerStatus.Interval = 100;
            timerStatus.Tick += TimerStatus_Tick;
            timerStatus.Enabled = true;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="name">Name</param>
        public MySyncAxisMarker(int index, string name)
            : this()
        {
            Index = index;
            Name = name;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MySyncAxisMarker()
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
                timerStatus.Enabled = false;
                timerStatus.Tick -= TimerStatus_Tick;
                this.Stop();
            }
            this.disposed = true;
            base.Dispose(disposing);
        }

        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            var rtcSyncAxis = Rtc as IRtcSyncAxis;
            if (rtcSyncAxis == null)
                return;
            switch(rtcSyncAxis.OpStatus)
            {
                case OperationStatus.Unknown:
                    if (OperationStatusColor != Color.DarkGray)
                    {
                        OperationStatusColor = Color.DarkGray;
                        NotifyPropertyChanged("OperationStatusColor");
                    }
                    break;
                case OperationStatus.Red:
                    if (OperationStatusColor != Color.Red)
                    {
                        OperationStatusColor = Color.Red;
                        NotifyPropertyChanged("OperationStatusColor");
                    }
                    break;
                case OperationStatus.Yellow:
                    if (OperationStatusColor != Color.Yellow)
                    {
                        OperationStatusColor = Color.Yellow;
                        NotifyPropertyChanged("OperationStatusColor");
                    }
                    break;
                case OperationStatus.Green:
                    if (OperationStatusColor != Color.Green)
                    {
                        OperationStatusColor = Color.Green;
                        NotifyPropertyChanged("OperationStatusColor");
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override bool Initialize()
        {
            Logger.Log(Logger.Type.Info, $"marker [{Index}]: initialized");
            return true;
        }
        /// <inheritdoc/>
        public override bool Ready(IDocument document, IView view, IRtc rtc, ILaser laser)
        {
            if (this.IsBusy)
            {
                Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to ready. marker status is busy");
                return false;
            }

            this.Document = document;
            this.View = view;
            this.Rtc = rtc;
            this.Laser = laser;

            if (rtc is Rtc5 || rtc is Rtc6)
            {
                this.Rtc = null;
                Logger.Log(Logger.Type.Error, $"marker [{Index}]: assigned invalid RTC instance");
                return false;
            }
            return true;
        }
        /// <inheritdoc/>
        public override bool Start()
        {
            if (Document == null || Rtc == null || Laser == null)
            {
                Logger.Log(Logger.Type.Warn, $"marker [{Index}]: ready at first");
                return false;
            }
            if (this.IsBusy)
            {
                Logger.Log(Logger.Type.Error, $"marker [{Index}]: busy now !");
                return false;
            }
            if (this.IsError)
            {
                Logger.Log(Logger.Type.Error, $"marker [{Index}]: has a error. reset at first");
                return false;
            }
            if (!this.IsReady)
            {
                Logger.Log(Logger.Type.Error, $"marker [{Index}]: is not ready yet");
                return false;
            }

            var rtc = this.Rtc;            
            var laser = this.Laser;
            var doc = this.Document;

            if (rtc.CtlGetStatus(RtcStatus.Busy))
            {
                Logger.Log(Logger.Type.Error, $"marker [{Index}]: busy now !");
                return false;
            }
            if (!rtc.CtlGetStatus(RtcStatus.NoError))
            {
                Logger.Log(Logger.Type.Error, $"marker [{Index}]: rtc has a internal error. reset at first");
                return false;
            }

            if (laser.IsError)
            {
                Logger.Log(Logger.Type.Error, $"marker [{Index}]: laser has a error status. reset at first");
                return false;
            }

            if (null == Offsets || 0 == Offsets.Length)
                this.Offsets = new Offset[1] { Offset.Zero };

            // Reset to start
            this.CurrentPenColor = Color.Transparent;

            Logger.Log(Logger.Type.Warn, $"marker [{Index}]: trying to start mark with {this.Offsets.Length} offsets");
            if (null != thread)
            {
                if (!this.thread.Join(500))
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: previous works has not finished yet");
                    return false;
                }
            }
            // Shallow copy for cross-thread issue
            layers = new List<EntityLayer>(Document.InternalData.Layers);
            AccumulatedMarks++;
            switch (MarkProcedure)
            {
                default:
                    this.thread = new Thread(this.MarkerThreadLayerFirst);
                    break;
                case MarkProcedures.OffsetFirst:
                    this.thread = new Thread(this.MarkerThreadOffsetFirst);
                    break;
            }
            this.thread.Name = $"MyMarker [{Index}]: {this.Name}";
            this.thread.Start();
            return true;
        }
        /// <inheritdoc/>
        public override bool Stop()
        {
            if (null == Rtc || null == Laser)
                return false;
            bool success = true;
            success &= Rtc.CtlAbort();
            success &= Laser.CtlAbort();

            if (null != thread)
            {
                var sw = Stopwatch.StartNew();
                do
                {
                    Application.DoEvents();
                    if (this.thread.Join(0))
                    {
                        thread = null;
                        break;
                    }
                    if (sw.ElapsedMilliseconds > 500)
                    {
                        success = false;
                        Logger.Log(Logger.Type.Error, $"marker [{Index}]: waiting for stop but timed out");
                        break; // Timed out
                    }
                }
                while (true);
            }
            this.isInternalBusy = false;
            return success;
        }
        /// <inheritdoc/>
        public override bool Reset()
        {
            if (null == Rtc || null == Laser)
                return false;
            bool success = true;
            success &= Rtc.CtlReset();
            success &= Laser.CtlReset();

            return success;
        }

        /// <summary>
        /// Marker thread #1
        /// </summary>
        /// <remarks>
        /// <c>MarkProc.LayerFirst</c> <br/>
        /// Move offset1 and Mark layers -> Move offset2 and Mark layers , ... <br/>
        /// </remarks>
        protected virtual void MarkerThreadLayerFirst()
        {
            var rtc = this.Rtc;
            var laser = this.Laser;
            var document = this.Document;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtc != null);
            Debug.Assert(laser != null);
            Debug.Assert(document != null);
            Debug.Assert(null != rtcSyncAxis);

            this.isInternalBusy = true;
            this.NotifyStarted();
            var dtStarted = DateTime.Now;
            bool success = true;
            var oldMatrixStack = (IMatrixStack<System.Numerics.Matrix4x4>)rtc.MatrixStack.Clone();

            for (int i = 0; i < Offsets.Length; i++)
            {
                Logger.Log(Logger.Type.Debug, $"marker [{Index}]: offset index= {i}, xyzt= {Offsets[i].ToString()}");
                rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                foreach (var layer in layers)
                {
                    if (!layer.IsMarkerable)
                        continue;
                    success &= NotifyBeforeLayer(layer);
                    if (!success)
                    {
                        Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to mark layer at before event handler"); ;
                        break;
                    }

                    switch (layer.MotionType)
                    {
                        case MotionType.StageOnly:
                            success &= rtcSyncAxis.CtlMotionType(MotionType.StageOnly);
                            break;
                        case MotionType.ScannerOnly:
                            success &= rtcSyncAxis.CtlMotionType(MotionType.ScannerOnly);
                            break;
                        case MotionType.StageAndScanner:
                            success &= rtcSyncAxis.CtlMotionType(MotionType.StageAndScanner);
                            success &= rtcSyncAxis.CtlBandWidth(layer.BandWidth);
                            break;
                    }
                    success &= laser.ListBegin();
                    success &= rtcSyncAxis.ListBegin(layer.MotionType);
                    success &= LayerWork(i, layer, Offsets[i]);
                    if (!success)
                        break;
                    if (IsJumpToOriginAfterFinished)
                        success &= rtc.ListJumpTo(System.Numerics.Vector2.Zero);
                    success &= laser.ListEnd();
                    success &= rtc.ListEnd();
                    if (success)
                        success &= rtc.ListExecute(true);
                    if (!success)
                        break;
                    success &= NotifyAfterLayer(layer);
                    if (!success)
                    {
                        Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to mark layer at after event handler");
                        break;
                    }
                    if (success)
                    {
                        if (this.IsMeasurementPlot)
                        {
                            if (rtcSyncAxis.IsSimulationMode)
                            {
                                string simulatedFileName = Path.Combine(SpiralLab.Sirius2.Config.SyncAxisSimulateFilePath, rtcSyncAxis.SimulationFileName);
                                SyncAxisViewerHelper.Plot(simulatedFileName);
                            }
                        }
                    }
                }
                rtc.MatrixStack.Pop();
                if (!success)
                    break;
            }

            rtc.MatrixStack = oldMatrixStack;
            this.TimeSpan = DateTime.Now - dtStarted;
            this.isInternalBusy = false;
            this.NotifyEnded(success);
            if (success)
            {
                Logger.Log(Logger.Type.Info, $"marker [{Index}]: mark has finished with {this.TimeSpan.TotalSeconds:F3}s");
            }
            else
            {
                Logger.Log(Logger.Type.Error, $"marker [{Index}]: mark has failed with {this.TimeSpan.TotalSeconds:F3}s");
            }
        }

        /// <summary>
        /// Marker thread #2
        /// </summary>
        /// <remarks>
        /// <c>MarkProc.OffsetFirst</c> <br/>
        /// Mark layer1 with offset1 and offset2, ... -> Mark layer2 with offset1 and offset2, ... <br/>
        /// </remarks>
        protected virtual void MarkerThreadOffsetFirst()
        {
            var rtc = this.Rtc;
            var laser = this.Laser;
            var document = this.Document;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtc != null);
            Debug.Assert(laser != null);
            Debug.Assert(document != null);
            Debug.Assert(null != rtcSyncAxis);

            this.isInternalBusy = true;
            this.NotifyStarted();
            var dtStarted = DateTime.Now;
            bool success = true;
            var oldMatrixStack = (IMatrixStack<System.Numerics.Matrix4x4>)rtc.MatrixStack.Clone();

            foreach (var layer in layers)
            {
                if (!layer.IsMarkerable)
                    continue;
                success &= NotifyBeforeLayer(layer);
                if (!success)
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to mark layer at before event handler"); ;
                    break;
                }

                switch (layer.MotionType)
                {
                    case MotionType.StageOnly:
                        success &= rtcSyncAxis.CtlMotionType(MotionType.StageOnly);
                        break;
                    case MotionType.ScannerOnly:
                        success &= rtcSyncAxis.CtlMotionType(MotionType.ScannerOnly);
                        break;
                    case MotionType.StageAndScanner:
                        success &= rtcSyncAxis.CtlMotionType(MotionType.StageAndScanner);
                        success &= rtcSyncAxis.CtlBandWidth(layer.BandWidth);
                        break;
                }
                success &= laser.ListBegin();
                success &= rtcSyncAxis.ListBegin(layer.MotionType);

                for (int i = 0; i < Offsets.Length; i++)
                {
                    Logger.Log(Logger.Type.Debug, $"marker [{Index}]: offset index= {i}, xyzt= {Offsets[i].ToString()}");
                    rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                    success &= LayerWork(i, layer, Offsets[i]);
                    rtc.MatrixStack.Pop();
                    if (!success)
                        break;
                }
                if (IsJumpToOriginAfterFinished)
                    success &= rtc.ListJumpTo(System.Numerics.Vector2.Zero);
                success &= laser.ListEnd();
                success &= rtc.ListEnd();
                if (success)
                    success &= rtc.ListExecute(true);
                if (!success)
                    break;

                success &= NotifyAfterLayer(layer);
                if (!success)
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to mark layer at after event handler");
                    break;
                }
                if (success)
                {
                    if (this.IsMeasurementPlot)
                    {
                        if (rtcSyncAxis.IsSimulationMode)
                        {
                            string simulatedFileName = Path.Combine(SpiralLab.Sirius2.Config.SyncAxisSimulateFilePath, rtcSyncAxis.SimulationFileName);
                            SyncAxisViewerHelper.Plot(simulatedFileName);
                        }
                    }
                }

                if (!success)
                    break;
            }

            rtc.MatrixStack = oldMatrixStack;
            this.TimeSpan = DateTime.Now - dtStarted;
            this.isInternalBusy = false;
            this.NotifyEnded(success);
            if (success)
            {
                Logger.Log(Logger.Type.Info, $"marker [{Index}]: mark has finished with {this.TimeSpan.TotalSeconds:F3}s");
            }
            else
            {
                Logger.Log(Logger.Type.Error, $"marker [{Index}]: mark has failed with {this.TimeSpan.TotalSeconds:F3}s");
            }
        }
    }
}
