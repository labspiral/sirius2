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
 * Description : Custom marker for RTC5,6
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
using SpiralLab.Sirius2.Winforms.Common;

namespace Demos
{
    /// <summary>
    /// Custom marker for RTC5,6
    /// </summary>
    public class MyRtcMarker
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
        public virtual bool IsExternalStart
        {
            get { return isExternalStart; }
            set {
                if (this.IsBusy)
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to set external start during busy");
                    return;
                }
                isExternalStart = value;
                if (isExternalStart)
                {
                    listType = ListType.Single;
                    if (1 != this.Document.InternalData.Layers.Count)
                        MessageBox.Show($"Should be single layer only to use external /START", "Warning", MessageBoxButtons.OK);
                }
                else
                    listType = ListType.Auto;
            }
        }
        private bool isExternalStart = false;

        /// <summary>
        /// <c>ListType</c>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Data")]
        [DisplayName("List type")]
        [Description("List type")]
        public virtual ListType ListType
        {
            get { return listType; }
            set {
                if (this.IsBusy)
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to set list type during busy");
                    return;
                }
                listType = value;
            }
        }
        private ListType listType;

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
                    if (DialogResult.Yes == MessageBox.Show($"gnuplot program is not exist at '{SpiralLab.Sirius2.Config.MeasurementGNUPlotProgramPath}'.{Environment.NewLine}Press 'Yes' to open downloadable webpage", "Warning", MessageBoxButtons.YesNo))
                        System.Diagnostics.Process.Start("http://tmacchant33.starfree.jp/gnuplot_bin.html");
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
        public MyRtcMarker()
            : base()
        {
            IsExternalStart = false;
            ListType = ListType.Auto;
            isMeasurementPlot = false;

            // set 'True' if check scanner status
            IsCheckTempOk = false;
            IsCheckPowerOk = false;
            IsCheckPositionAck = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="name">Name</param>
        public MyRtcMarker(int index, string name)
            : this()
        {
            Index = index;
            Name = name;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MyRtcMarker()
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
            // Clear registered characterset when ready
            TextRegisterHelper.Unregister(this);
            return true;
        }
        /// <summary>
        /// Register (or download) character set (font glyph) into RTC controller if <c>ITextRegisterable</c> has modified. <br/>
        /// </summary>
        /// <remarks>
        /// If <c>ITextRegisterable</c> entity has modified, it should be re-registered. <br/>
        /// </remarks>
        /// <returns>Success or failed</returns>
        public virtual bool RegisterCharacterSet()
        {
            bool success = true;
            if (Document.FindByType(typeof(ITextRegisterable), out IEntity[] entities))
            {
                foreach (var entity in entities)
                {
                    if (entity is ITextRegisterable textRegisterable)
                    {
                        switch (this.MarkTarget)
                        {
                            case MarkTargets.All:
                                success &= TextRegisterHelper.Register(textRegisterable, this, out var dummy1);
                                break;
                            case MarkTargets.Selected:
                                if (entity.IsSelected)
                                    success &= TextRegisterHelper.Register(textRegisterable, this, out var dummy2);
                                break;
                        }
                    }
                }
            }
            return success;
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

            if (null != thread)
            {
                if (!this.thread.Join(500))
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: previous works has not finished yet");
                    return false;
                }
            }

            if (IsExternalStart)
            {
                // Should be exist only single layer for External /START 
                if (Document.InternalData.Layers.Count != 1)
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: should be single layer only for external /START");
                    return false;
                }
                // list type to single for external /START by forcily 
                listType = ListType.Single;
            }

            if (null == Offsets || 0 == Offsets.Length)
                this.Offsets = new Offset[1] { Offset.Zero };

            // Reset current(or last) pen color
            this.CurrentPenColor = Color.Transparent;

            // Reset measurement session
            this.CurrentSession = null;

            // Clear measurement session queue
            while (sessionQueue.Count > 0)
                sessionQueue.TryDequeue(out var dummy);

            // Register text if regened(or modified)
            RegisterCharacterSet();

            // Shallow copy for cross-thread issue
            layers = new List<EntityLayer>(Document.InternalData.Layers);

            CurrentOffsetIndex = 0;
            CurrentLayerIndex = 0;
            CurrentLayer = null;
            CurrentEntityIndex = 0;
            CurrentEntity = null;
            AccumulatedMarks++;

            if (IsExternalStart)
                Logger.Log(Logger.Type.Warn, $"marker [{Index}]: trying to start mark by external trigger with {this.Offsets.Length} offsets");
            else
                Logger.Log(Logger.Type.Warn, $"marker [{Index}]: trying to start mark with {this.Offsets.Length} offsets");

            switch (MarkProcedure)
            {
                default:
                    this.thread = new Thread(this.MarkerThreadLayerFirst);
                    break;
                case MarkProcedures.OffsetFirst:
                    this.thread = new Thread(this.MarkerThreadOffsetFirst);
                    break;
            }
            this.thread.Name = $"Marker: {this.Name}";
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
                        // Timed out
                        break;
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

            this.NotifyStarted();
            var dtStarted = DateTime.Now;
            bool success = true;

            this.isInternalBusy = true;
            var oldMatrixStack = (IMatrixStack<System.Numerics.Matrix4x4>)rtc.MatrixStack.Clone();
            if (null != rtcMoF && rtc.IsMoF)
            {
                rtcMoF.CtlMofOverflowClear();
                //rtcMoF.MofAngularCenter = System.Numerics.Vector2.Zero;
            }

            for (int i = 0; i < Offsets.Length; i++)
            {
                Logger.Log(Logger.Type.Debug, $"marker [{Index}]: offset index= {i}, xyzt= {Offsets[i].ToString()}");
                rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                CurrentOffsetIndex = i;
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
                    if (null != rtcAlc && layer.IsALC)
                    {
                        success &= rtcAlc.CtlAlcByPositionTable(layer.AlcByPositionTable);
                        switch (layer.AlcSignal)
                        {
                            case AutoLaserControlSignal.ExtDO16:
                            case AutoLaserControlSignal.ExtDO8:
                                success &= rtcAlc.CtlAlc<uint>(layer.AlcSignal, layer.AlcMode, (uint)layer.AlcPercentage100, (uint)layer.AlcMinValue, (uint)layer.AlcMaxValue);
                                break;
                            default:
                                success &= rtcAlc.CtlAlc<double>(layer.AlcSignal, layer.AlcMode, layer.AlcPercentage100, layer.AlcMinValue, layer.AlcMaxValue);
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
                        success &= rtcAlc.CtlAlcByPositionTable(null);
                        success &= rtcAlc.CtlAlc<uint>(AutoLaserControlSignal.Disabled, AutoLaserControlMode.Disabled, 0, 0, 0);
                    }
                    if (!success)
                        break;
                    success &= NotifyAfterLayer(layer);
                    if (!success)
                    {
                        Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to mark layer at after event handler"); ;
                        break;
                    }
                }
                // Pop offset matrix
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
            if (IsJumpToOriginAfterFinished)
            {
                if (rtc.Is3D)
                {
                    success &= rtc3D.CtlZDefocus(0);
                    success &= rtc3D.CtlMoveTo(System.Numerics.Vector3.Zero);
                }
                else
                {
                    success &= rtc.CtlMoveTo(System.Numerics.Vector2.Zero);
                }
            }
            rtc.MatrixStack = oldMatrixStack;
            this.TimeSpan = DateTime.Now - dtStarted;
            this.isInternalBusy = false;
            if (!IsExternalStart)
            {
                this.NotifyEnded(success);
                if (success)
                {
                    Logger.Log(Logger.Type.Info, $"marker [{Index}]: mark has finished with {this.TimeSpan.TotalSeconds:F3}s");
                    if (this.IsMeasurementPlot)
                        this.NotifyPlot();
                }
                else
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: mark has failed with {this.TimeSpan.TotalSeconds:F3}s");
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
                else if (rtc is Rtc6)
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

            this.NotifyStarted();
            var dtStarted = DateTime.Now;
            bool success = true;

            this.isInternalBusy = true;
            var oldMatrixStack = (IMatrixStack<System.Numerics.Matrix4x4>)rtc.MatrixStack.Clone();
            if (null != rtcMoF && rtc.IsMoF)
            {
                rtcMoF.CtlMofOverflowClear();
                //rtcMoF.MofAngularCenter = System.Numerics.Vector2.Zero;
            }

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
                if (null != rtcAlc && layer.IsALC)
                {
                    success &= rtcAlc.CtlAlcByPositionTable(layer.AlcByPositionTable);
                    switch (layer.AlcSignal)
                    {
                        case AutoLaserControlSignal.ExtDO16:
                        case AutoLaserControlSignal.ExtDO8:
                            success &= rtcAlc.CtlAlc<uint>(layer.AlcSignal, layer.AlcMode, (uint)layer.AlcPercentage100, (uint)layer.AlcMinValue, (uint)layer.AlcMaxValue);
                            break;
                        default:
                            success &= rtcAlc.CtlAlc<double>(layer.AlcSignal, layer.AlcMode, layer.AlcPercentage100, layer.AlcMinValue, layer.AlcMaxValue);
                            break;
                    }
                }
                success &= laser.ListBegin();
                success &= rtc.ListBegin(ListType);

                for (int i = 0; i < Offsets.Length; i++)
                {
                    try
                    {
                        rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                        CurrentOffsetIndex = i;
                        Logger.Log(Logger.Type.Debug, $"marker [{Index}]: offset index= {i}, xyzt= {Offsets[i].ToString()}");
                        success &= LayerWork(i, layer, Offsets[i]);
                        if (!success)
                            break;
                    }
                    finally
                    {
                        // Pop offset matrix
                        rtc.MatrixStack.Pop();
                    }
                }

                if (success)
                {
                    if (IsJumpToOriginAfterFinished)
                    {
                        if (rtc.Is3D)
                        {
                            success &= rtc3D.ListZDefocus(0);
                            success &= rtc3D.ListJumpTo(System.Numerics.Vector3.Zero);
                        }
                        else
                        {
                            success &= rtc.ListJumpTo(System.Numerics.Vector2.Zero);
                        }
                    }
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
                    success &= rtcAlc.CtlAlcByPositionTable(null);
                    success &= rtcAlc.CtlAlc<uint>(AutoLaserControlSignal.Disabled, AutoLaserControlMode.Disabled, 0, 0, 0);
                }
                if (!success)
                    break;
                success &= NotifyAfterLayer(layer);
                if (!success)
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: fail to mark layer at after event handler");
                    break;
                }
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
            this.isInternalBusy = false;
            if (!IsExternalStart)
            {
                this.NotifyEnded(success);
                if (success)
                {
                    Logger.Log(Logger.Type.Info, $"marker [{Index}]: mark has finished with {this.TimeSpan.TotalSeconds:F3}s");
                    if (this.IsMeasurementPlot)
                        this.NotifyPlot();
                }
                else
                {
                    Logger.Log(Logger.Type.Error, $"marker [{Index}]: mark has failed with {this.TimeSpan.TotalSeconds:F3}s");
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
            // Plot as a graph
            foreach (var session in sessionQueue)
                session.Plot();
        }
    }
}
