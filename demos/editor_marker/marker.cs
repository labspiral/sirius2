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
 * Description : Custom marker
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
#if NETFRAMEWORK
using OpenTK;
#elif NET
using OpenTK.Mathematics;
#endif

namespace Demos
{
    /// <summary>
    /// Custom marker
    /// </summary>
    public class MyMarker
        : MarkerBase
    {
        /// <summary>
        /// Enable/Disable External /START trigger
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Data")]
        [DisplayName("External /START")]
        [Description("External /START")]
        public virtual bool IsExternalStart { get; set; }

        /// <summary>
        /// <c>ListType</c>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Data")]
        [DisplayName("List type")]
        [Description("List type")]
        public virtual ListType ListType { get; set; }

        /// <summary>
        /// EntityMeasurementBegin (if executed)
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Measurement")]
        [DisplayName("Session")]
        [Description("Session")]
        public virtual MeasurementSession[] Session
        {
            get { return sessionQueue.ToArray(); }            
        }
        protected ConcurrentQueue<MeasurementSession> sessionQueue = new ConcurrentQueue<MeasurementSession>();

        /// <summary>
        /// Current (or last measurement session)
        /// Assigned by <c>EntityMeasurementBegin</c>
        /// </summary>
        internal MeasurementSession CurrentSession { get; set; }

        /// <summary>
        /// Plot Measurement session to graph
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Measurement")]
        [DisplayName("Plot")]
        [Description("Plot")]
        public virtual bool IsMeasurementPlot 
        { 
            get { return isMeasurementPlot; } 
            set { 
                if (!File.Exists(SpiralLab.Sirius2.Config.MeasurementGNUPlotProgramPath))
                {
                    //gnuplot program is not exit !
                    return;
                }
                isMeasurementPlot = value;
            } 
        }
        protected bool isMeasurementPlot;


        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Check")]
        [DisplayName("Temp")]
        [Description("Temperature Check")]
        public virtual bool IsCheckTempOk { get; set; }

        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Check")]
        [DisplayName("Power")]
        [Description("Power Supply Check")]
        public virtual bool IsCheckPowerOk { get; set; }

        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Check")]
        [DisplayName("Position")]
        [Description("Position ACK Check")]
        public virtual bool IsCheckPositionAck { get; set; }

        /// <summary>
        /// Internal marker thread 
        /// </summary>
        protected Thread thread;
        /// <summary>
        /// List of layers to mark
        /// </summary>
        protected List<EntityLayer> layers;
        private bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public MyMarker()
            : base()
        {
            IsExternalStart = false;
            ListType = ListType.Auto;
            isMeasurementPlot = false;

            IsCheckTempOk = true;
            IsCheckPowerOk = true;
            IsCheckPositionAck = true;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="name">Name</param>
        public MyMarker(int index, string name)
            : this()
        {
            Index = index;
            Name = name;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MyMarker()
        {
            this.Dispose(false);
        }
        /// <summary>
        /// Dispose internal resources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing)
            {
                this.Stop();               
            }
            this.disposed = true;
            base.Dispose(disposing);
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

            if (rtc is IRtcSyncAxis rtcSyncAxis)
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

            if (IsCheckTempOk && !rtc.CtlGetStatus(RtcStatus.TempOK))
            {
                Logger.Log(Logger.Type.Error, $"marker: {this.Name} scanner temp is no ok");
                return false;
            }
            if (IsCheckPowerOk && !rtc.CtlGetStatus(RtcStatus.PowerOK))
            {
                Logger.Log(Logger.Type.Error, $"marker: {this.Name} scanner power is not ok !");
                return false;
            }
            if (IsCheckPositionAck && !rtc.CtlGetStatus(RtcStatus.PositionAckOK))
            {
                Logger.Log(Logger.Type.Error, $"marker: {this.Name} scanner position is not acked");
                return false;
            }

            if (null == Offsets || 0 == Offsets.Length)
                this.Offsets = new Offset[1] { Offset.Zero };

            // Reset to start
            this.CurrentPenColor = Color.Transparent;

            // Reset measurement session
            this.CurrentSession = null;

            if (IsExternalStart)
                Logger.Log(Logger.Type.Warn, $"marker [{Index}]: trying to start mark by external trigger with {this.Offsets.Length} offsets");
            else
                Logger.Log(Logger.Type.Warn, $"marker [{Index}]: trying to start mark with {this.Offsets.Length} offsets");

            if (null != thread)
            {
                if (!this.thread.Join(500))
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: previous works has not finished yet");
                    return false;
                }
            }
            // Clear queue
            while (sessionQueue.Count > 0) 
                sessionQueue.TryDequeue(out var dummy);

            // Shallow copy for cross-thread issue
            layers = new List<EntityLayer>(Document.InternalData.Layers);

            CurrentOffsetIndex = 0;
            CurrentLayerIndex = 0;
            CurrentLayer = null;
            CurrentEntityIndex = 0;
            CurrentEntity = null;
            AccumulatedMarks++;

            this.thread = new Thread(this.MarkerThread);
            this.thread.Name = $"MyMarker: {this.Name}";
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
                        break; //Timed out
                    }
                }
                while (true);
            }

            var rtc = this.Rtc;
            var rtcExtension = rtc as IRtcExtension;

            if (this.IsExternalStart)
            {
                if (rtc is Rtc5)
                {
                    var extMode = Rtc5ExternalControlMode.Empty;
                    success = rtcExtension.CtlExternalControl(extMode);
                }
                else if (rtc is Rtc6 || rtc is Rtc6Ethernet)
                {
                    var extMode = Rtc6ExternalControlMode.Empty;
                    success = rtcExtension.CtlExternalControl(extMode);
                }
            }
            this.isThreadBusy = false;
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
        /// Marker thread for marking 
        /// </summary>
        protected virtual void MarkerThread()
        {
            var rtc = this.Rtc;
            var laser = this.Laser;
            var document = this.Document;
            var rtc3D = rtc as IRtc3D;
            var rtc2ndHead = rtc as IRtc2ndHead;
            var rtcExtension = rtc as IRtcExtension;
            var rtcAlc = rtc as IRtcAutoLaserControl;
            var rtcMoF = rtc as IRtcMoF;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtc != null);
            Debug.Assert(laser != null);
            Debug.Assert(document != null);
            Debug.Assert(null == rtcSyncAxis);

            this.isThreadBusy = true;
            this.NotifyStarted();
            var dtStarted = DateTime.Now;            
            bool success = true;

            if (IsExternalStart)
            {
                // Set to list type to single by forcily if external /START used 
                ListType = ListType.Single; 
                Debug.Assert(document.InternalData.Layers.Count == 1); 
            }

            var oldMatrixStack = (IMatrixStack<System.Numerics.Matrix4x4>)rtc.MatrixStack.Clone();
            if (null != rtcMoF)
            {
                if (rtc.IsMoF)
                    rtcMoF.CtlMofOverflowClear();
            }
            if (null != rtc3D)
            {
                if (rtc.Is3D)
                    rtc3D.ZDefocus = 0;
            }

            for (int i = 0; i < Offsets.Length; i++)
            {
                Logger.Log(Logger.Type.Debug, $"marker [{Index}]: offset index= {i}, xyzt= {Offsets[i]}");
                rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                CurrentOffsetIndex = i;
                foreach (var layer in layers)
                {
                    if (!layer.IsMarkerable)
                        continue;
                    success &= NotifyBeforeLayer(layer);
                    if (!success)
                    {
                        Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to mark layer by before event handler"); ;
                        break;
                    }
                    if (null != rtcAlc && layer.IsALC)
                    {
                        success &= rtcAlc.CtlAutoLaserControlByPositionTable(layer.AlcByPositionTable);
                        switch (layer.AlcSignal)
                        {
                            case AutoLaserControlSignal.ExtDO16:
                            case AutoLaserControlSignal.ExtDO8:
                                success &= rtcAlc.CtlAutoLaserControl<uint>(layer.AlcSignal, layer.AlcMode, (uint)layer.AlcPercentage100, (uint)layer.AlcMinValue, (uint)layer.AlcMaxValue);
                                break;
                            default:
                                success &= rtcAlc.CtlAutoLaserControl<double>(layer.AlcSignal, layer.AlcMode, layer.AlcPercentage100, layer.AlcMinValue, layer.AlcMaxValue);
                                break;
                        }
                    }
                    success &= laser.ListBegin();
                    success &= rtc.ListBegin(ListType);

                    success &= LayerWork(i, layer, Offsets[i]);
                    if (!success)
                        break;
                    if (success)
                    {
                        if (IsJumpToOriginAfterFinished)
                            success &= rtc3D.ListJumpTo(System.Numerics.Vector3.Zero);
                        success &= laser.ListEnd();
                        success &= rtc.ListEnd();
                        if (success && !IsExternalStart)
                            success &= rtc.ListExecute(true);
                        if (success && !IsExternalStart)
                        {
                            if (null != CurrentSession && !CurrentSession.IsEmpty)
                            {
                                if (CurrentSession.Save(this.Rtc as IRtcMeasurement))
                                {
                                    sessionQueue.Enqueue(CurrentSession);
                                }
                            }
                        }
                    }

                    if (null != rtcAlc && layer.IsALC && !IsExternalStart)
                    {
                        success &= rtcAlc.CtlAutoLaserControlByPositionTable(null);
                        success &= rtcAlc.CtlAutoLaserControl<uint>(AutoLaserControlSignal.Disabled, AutoLaserControlMode.Disabled, 0, 0, 0);
                    }
                    if (!success)
                        break;
                    success &= NotifyAfterLayer(layer);
                    if (!success)
                    {
                        Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to mark layer by after event handler"); ;
                        break;
                    }
                }
                rtc.MatrixStack.Pop();
                if (!success)
                    break;
            }

            if (null != rtcMoF)
            {
                if (rtc.CtlGetStatus(RtcStatus.MofOutOfRange))
                {
                    if (rtc is Rtc5 rtc5)
                    {
                        var info = rtc5.MarkingInfo;
                        Logger.Log(Logger.Type.Warn, $"marker [{Index}]: mof out of range. markinfg info= {info.Value}");
                    }
                    else if (rtc is Rtc6 rtc6)
                    {
                        var info = rtc6.MarkingInfo;
                        Logger.Log(Logger.Type.Warn, $"marker [{Index}]: mof out of range. markinfg info= {info.Value}");
                    }
                }
            }

            rtc.MatrixStack = oldMatrixStack;
            this.TimeSpan = DateTime.Now - dtStarted;
            this.isThreadBusy = false;
            if (!IsExternalStart)
            {
                if (success)
                {
                    Logger.Log(Logger.Type.Debug, $"marker [{Index}]: mark has finished");
                    this.NotifyFinished();
                    if (this.IsMeasurementPlot)
                        this.NotifyPlot();
                }
                else
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: mark has failed");
                    this.NotifyFailed();
                }
            }
            else
            {
                if (rtc is Rtc5)
                {
                    var extMode = Rtc5ExternalControlMode.Empty;
                    extMode.Add(Rtc5ExternalControlMode.Bit.ExternalStart);
                    extMode.Add(Rtc5ExternalControlMode.Bit.ExternalStartAgain);
                    extMode.Add(Rtc5ExternalControlMode.Bit.ExternalStop);
                    //extMode.Add(Rtc5ExternalControlMode.Bit.EncoderReset);
                    extMode.Add(Rtc5ExternalControlMode.Bit.TrackDelay);
                    success &= rtcExtension.CtlExternalControl(extMode);
                }
                else if (rtc is Rtc6 || rtc is Rtc6Ethernet)
                {
                    var extMode = Rtc6ExternalControlMode.Empty;
                    extMode.Add(Rtc6ExternalControlMode.Bit.ExternalStart);
                    extMode.Add(Rtc6ExternalControlMode.Bit.ExternalStartAgain);
                    extMode.Add(Rtc6ExternalControlMode.Bit.ExternalStop);
                    //extMode.Add(Rtc6ExternalControlMode.Bit.EncoderReset);
                    extMode.Add(Rtc6ExternalControlMode.Bit.TrackDelay);
                    success &= rtcExtension.CtlExternalControl(extMode);
                }
                Logger.Log(Logger.Type.Warn, $"marker [{Index}]: waiting for /START trigger");
            }
        }

        /// <summary>
        /// Plot measurement to graph by gnuplot
        /// </summary>
        protected void NotifyPlot()
        {
            foreach (var session in sessionQueue)
            {
                session.Plot();
            }            
        }
    }
}
