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
 * Description : MarkerRtcFast 
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
using SpiralLab.Sirius2.PowerMeter;
using SpiralLab.Sirius2.Winforms.Remote;

namespace Demos
{
    /// <summary>
    /// Custom MarkerRtcFast
    /// </summary>
    /// <remarks>
    /// Used with RTC5,6,6e <br/>
    /// Used with <see cref="IRtc.ListBegin">IRtc.ListBegin</see> and <see cref="IRtc.ListEnd">IRtc.ListEnd</see>  to process whole <see cref="EntityLayer">EntityLayer</see>for speed up. <br/>
    /// Disabled <see cref="EntityLayerRtc.IsALC">EntityLayer.IsALC</see> feature. <br/>
    /// Disabled <c>IsExternalStart</c> feature. <br/>
    /// </remarks>
    public class MarkerRtcFast
        : MarkerBase
    {
        /// <summary>
        /// Mark targets
        /// </summary>
        public enum MarkTargets
        {
            /// <summary>
            /// All entities
            /// </summary>
            All = 0,
            /// <summary>
            /// Selected entities
            /// </summary>
            Selected = 1,
        }

        /// <summary>
        /// Target entities to mark
        /// </summary>
        /// <remarks>
        /// Default: <see cref="MarkTargets.All">MarkTargets.All</see> <br/>
        /// </remarks>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Data")]
        [DisplayName("Mark Target")]
        [Description("Mark Target")]
        public virtual MarkTargets MarkTarget
        {
            get { return markTarget; }
            set
            {
                if (this.IsBusy)
                {
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to set mark target during busy");
                    return;
                }
                var oldMarkTarget = markTarget;
                markTarget = value;
                if (markTarget != oldMarkTarget)
                    this.NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Internal <c>MarkTargets</c>
        /// </summary>
        protected MarkTargets markTarget = MarkTargets.All;

        /// <summary>
        /// <c>ListType</c>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Data")]
        [DisplayName("List type")]
        [Description("List type")]
        public virtual ListTypes ListType
        { 
            get { return listType; }
            set {
                if (this.IsBusy)
                {
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to set list type during busy");
                    return;
                }
                listType = value; 
            }
        }
        private ListTypes listType;

        /// <summary>
        /// Array of <see cref="MeasurementSession">MeasurementSession</see> 
        /// </summary>
        /// <remarks>
        /// Session = <see cref="EntityMeasurementBegin">EntityMeasurementBegin</see> + <see cref="EntityMeasurementEnd">EntityMeasurementEnd</see> <br/>
        /// Valid when <see cref="EntityMeasurementBegin">EntityMeasurementBegin</see> has executed. <br/>
        /// </remarks>
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
        /// <summary>
        /// Queue for <c>MeasurementSession</c> 
        /// </summary>
        protected ConcurrentQueue<MeasurementSession> sessionQueue = new ConcurrentQueue<MeasurementSession>();
        /// <summary>
        /// Current (or last measurement session)
        /// </summary>
        /// <remarks>
        /// Valid when <c>EntityMeasurementBegin</c> had executed<br/>
        /// </remarks>
        internal MeasurementSession CurrentSession { get; set; }

        /// <summary>
        /// Is plot measurement session to graph or not
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
        /// <summary>
        /// Is plot measurement session to graph or not
        /// </summary>
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
        public MarkerRtcFast()
            : base()
        {
            listType = ListTypes.Auto;
            isMeasurementPlot = true;
            markTarget = MarkTargets.All;

            IsCheckTempOk = false;
            IsCheckPowerOk = false;
            IsCheckPositionAck = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="name">Name</param>
        public MarkerRtcFast(int index, string name)
            : this()
        {
            Index = index;
            Name = name;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MarkerRtcFast()
        {
            this.Dispose(false);
        }
        /// <inheritdoc/>  
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }
                this.disposed = true;
            }
            base.Dispose(disposing);
        }
        /// <inheritdoc/>
        public override bool Initialize()
        {
            Logger.Log(Logger.Types.Info, $"marker [{Index}]: initialized");
            return true;
        }
        /// <inheritdoc/>
        public override bool Ready(IDocument document, IView view, IRtc rtc, ILaser laser, IPowerMeter powerMeter, IRemote remote)
        {
            if (this.IsBusy)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to ready. marker status is busy");
                return false;
            }

            base.Document = document;
            base.View = view;
            base.Rtc = rtc;
            base.Laser = laser;
            base.PowerMeter = powerMeter;
            base.Remote = remote;

            if (rtc is IRtcSyncAxis rtcSyncAxis)
            {
                this.Rtc = null;
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: assigned invalid RTC instance");
                return false;
            }
            Logger.Log(Logger.Types.Debug, $"marker [{Index}]: ready with doc= {document?.FileName}, view= {view?.Name}, rtc= {rtc?.Name}, laser= {laser?.Name}, pm= {powerMeter?.Name}, remote= {remote?.Name}");
            return true;
        }
        /// <inheritdoc/>
        public override bool Ready(IDocument document)
        {
            if (this.IsBusy)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to ready. marker status is busy");
                return false;
            }

            base.Document = document;
            Logger.Log(Logger.Types.Debug, $"marker [{Index}]: ready with doc= {document?.FileName}");
            return true;
        }
        /// <summary>
        /// Register (or download) character set (font glyph) into RTC controller if <c>ITextRegisterable</c> has exist. <br/>
        /// </summary>
        /// <remarks>
        /// If <c>ITextRegisterable</c> entity has exist and modifed, it should be registered(or downloaded). <br/>
        /// Re-registering(or downloading) takes time. <br/>
        /// </remarks>
        /// <returns>Success or failed</returns>
        public virtual bool RegisterCharacterSet()
        {
            bool success = true;
            if (Document.FindByType(typeof(ITextRegisterable), out IEntity[] entities))
            {
                IsTextRegistering = true;
                switch (this.MarkTarget)
                {
                    case MarkTargets.All:
                        {
                            ITextRegisterable[] textRegisterables = Array.ConvertAll(entities, item => (ITextRegisterable)item);
                            success &= TextRegisterHelper.Register(textRegisterables, this);
                        }
                        break;
                    case MarkTargets.Selected:
                        {
                            List<ITextRegisterable> textRegisterables = new List<ITextRegisterable>(4);
                            foreach (var entity in entities)
                                if (entity.IsSelected)
                                    textRegisterables.Add(entity as ITextRegisterable);
                            success &= TextRegisterHelper.Register(textRegisterables.ToArray(), this);
                        }
                        break;
                }
                IsTextRegistering = false;
            }
            return success;
        }
        /// <inheritdoc/>
        public override bool Start()
        {
            if (Document == null || Rtc == null || Laser == null)
            {
                Logger.Log(Logger.Types.Warn, $"marker [{Index}]: ready at first");
                return false;
            }
            if (this.IsBusy)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: busy now !");
                return false;
            }
            if (this.IsError)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: has a error. reset at first");
                return false;
            }
            if (!this.IsReady)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: is not ready yet");
                return false;
            }

            var rtc = this.Rtc;
            var laser = this.Laser;
            var doc = this.Document;

            if (rtc.CtlGetStatus(RtcStatus.Busy))
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: busy now !");
                return false;
            }
            if (!rtc.CtlGetStatus(RtcStatus.NoError))
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: rtc has a internal error. reset at first");
                return false;
            }
            if (laser.IsError)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: laser has a error status. reset at first");
                return false;
            }

            if (IsCheckTempOk && !rtc.CtlGetStatus(RtcStatus.TempOK))
            {
                Logger.Log(Logger.Types.Error, $"marker: {this.Name} scanner temp is no ok");
                return false;
            }
            if (IsCheckPowerOk && !rtc.CtlGetStatus(RtcStatus.PowerOK))
            {
                Logger.Log(Logger.Types.Error, $"marker: {this.Name} scanner power is not ok !");
                return false;
            }
            if (IsCheckPositionAck && !rtc.CtlGetStatus(RtcStatus.PositionAckOK))
            {
                Logger.Log(Logger.Types.Error, $"marker: {this.Name} scanner position is not acked");
                return false;
            }

            if (null != thread)
            {
                if (!this.thread.Join(500))
                {
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: previous works has not finished yet");
                    return false;
                }
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

            CurrentOffset = Offset.Zero;
            CurrentOffsetIndex = 0;
            CurrentLayer = null;
            CurrentLayerIndex = 0;
            CurrentEntity = null;
            CurrentEntityIndex = 0;
            AccumulatedMarks++;

            Logger.Log(Logger.Types.Warn, $"marker [{Index}]: trying to start mark with target= {MarkTarget}, offset(s)= {this.Offsets.Length}");
            this.thread = new Thread(this.MarkerThreadLayerFirst);                   
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
                        Logger.Log(Logger.Types.Error, $"marker [{Index}]: waiting for stop but timed out");
                        // Timed out
                        break; 
                    }
                }
                while (true);
            }

            var rtc = this.Rtc;
            var rtcExtension = rtc as IRtcExtension;
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
        /// Mark each <c>EntityLayer</c>
        /// </summary>
        /// <remarks>
        /// Helpful current working sets are <see cref="MarkerBase.CurrentOffsetIndex">CurrentOffsetIndex</see>, <see cref="MarkerBase.CurrentOffset">CurrentOffset</see>, <see cref="MarkerBase.CurrentLayerIndex">CurrentLayerIndex</see>, <see cref="MarkerBase.CurrentLayer">CurrentLayer</see>. <br/>
        /// Consider as its working within async threads. <br/>
        /// </remarks> 
        /// <param name="offsetIndex">Current index of offset (0,1,2,...)</param>
        /// <param name="layer">Current <c>EntityLayer</c></param>
        /// <param name="offset">Current <c>Offset</c></param>
        /// <returns>Success or failed</returns>
        protected virtual bool LayerWork(int offsetIndex, EntityLayer layer, Offset offset)
        {
            bool success = true;
            CurrentLayer = layer;
            for (int i = 0; i < layer.Repeats; i++)
            {
                CurrentLayerIndex = i;
                for (int j = 0; j < layer.Children.Count; j++)
                {
                    var entity = layer.Children[j];
                        CurrentEntityIndex = j;
                        CurrentEntity = entity;
                    if (!entity.IsMarkerable)
                        continue;
                    if (entity is IMarkerable markerable)
                    {
                        switch (MarkTarget)
                        {
                            case MarkTargets.All:
                                success &= EntityWork(offsetIndex, layer, j, entity);
                                break;
                            case MarkTargets.Selected:
                                if (entity.IsSelected)
                                    success &= EntityWork(offsetIndex, layer, j, entity);
                                break;
                        }
                    }
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }
            return success;
        }
        /// <summary>
        /// Mark each <c>IEntity</c>
        /// </summary>
        /// <remarks>
        /// Helpful current working sets are <see cref="MarkerBase.CurrentOffsetIndex">CurrentOffsetIndex</see>, <see cref="MarkerBase.CurrentOffset">CurrentOffset</see>, <see cref="MarkerBase.CurrentLayerIndex">CurrentLayerIndex</see>, <see cref="MarkerBase.CurrentLayer">CurrentLayer</see>. <br/>
        /// Consider as its working within async threads. <br/>
        /// </remarks> 
        /// <param name="offsetIndex">Current index of offset (0,1,2,...)</param>
        /// <param name="layer">Current <c>EntityLayer</c></param>
        /// <param name="entityIndex">Current index of entity</param>
        /// <param name="entity">Current <c>IEntity</c></param>
        /// <returns>Success or failed</returns>
        protected virtual bool EntityWork(int offsetIndex, EntityLayer layer, int entityIndex, IEntity entity)
        {
            bool success = true;
            success &= NotifyBeforeEntity(entity);
            if (!success)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to mark entity at before event handler"); ;
                return success;
            }
            if (entity is IMarkerable markerable)
            {
                // During each marks, internal entity data should be synchronized or locked
                lock (entity.SyncRoot)
                    success &= markerable.Mark(this);
            }
            if (!success)
                return success;
            success &= NotifyAfterEntity(entity);
            if (!success)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to mark entity at after event handler"); ;
                return success;
            }
            return success;
        }

        /// <summary>
        /// Marker thread
        /// </summary>
        /// <remarks>        
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

            success &= laser.ListBegin();
            success &= rtc.ListBegin(ListType);

            for (int i = 0; i < Offsets.Length; i++)
            {
                if (!success)
                    break;
                CurrentOffset = Offsets[i];
                CurrentOffsetIndex = i;
                rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                Logger.Log(Logger.Types.Debug, $"marker [{Index}]: offset index= {i}, xyzt= {Offsets[i].ToString()}");
                foreach (var layer in layers)
                {
                    if (!layer.IsMarkerable)
                        continue;
                    success &= NotifyBeforeLayer(layer);
                    if (!success)
                    {
                        Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to mark layer at before event handler"); ;
                        break;
                    }
                    success &= LayerWork(i, layer, Offsets[i]);                   
                    if (!success)
                        break;
                    success &= NotifyAfterLayer(layer);
                    if (!success)
                    {
                        Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to mark layer at after event handler"); ;
                        break;
                    }
                }
                // Pop offset matrix
                rtc.MatrixStack.Pop();
                if (!success)
                    break;
            }

            if (success)
            {
                success &= laser.ListEnd();
                success &= rtc.ListEnd();
                if (success)
                    success &= rtc.ListExecute(true);
                if (success)
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

            if (null != rtcMoF)
            {
                if (rtc.CtlGetStatus(RtcStatus.MofOutOfRange))
                {
                    if (rtc is Rtc5 rtc5)
                    {
                        var info = rtc5.MarkingInfo;
                        Logger.Log(Logger.Types.Warn, $"marker [{Index}]: mof out of range. markinfg info= {info.Value}");
                    }
                    else if (rtc is Rtc6 rtc6)
                    {
                        var info = rtc6.MarkingInfo;
                        Logger.Log(Logger.Types.Warn, $"marker [{Index}]: mof out of range. markinfg info= {info.Value}");
                    }
                }
            }
            if (IsJumpToOriginAfterFinished)
            {
                if (rtc.Is3D)
                {
                    success &= rtc3D.CtlZDefocus(0);
                    success &= rtc3D.CtlMoveTo(System.Numerics.Vector3.Zero, 500);
                }
                else
                {
                    success &= rtc.CtlMoveTo(System.Numerics.Vector2.Zero, 500);
                }
            }
            rtc.MatrixStack = oldMatrixStack;
            this.TimeSpan = DateTime.Now - dtStarted;
            this.isInternalBusy = false;
            this.NotifyEnded(success);
            if (success)
            {
                Logger.Log(Logger.Types.Info, $"marker [{Index}]: mark has finished with {this.TimeSpan.TotalSeconds:F3}s");
                if (this.IsMeasurementPlot)
                    this.NotifyPlot();
            }
            else
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: mark has failed with {this.TimeSpan.TotalSeconds:F3}s");
            }
        }

        /// <summary>
        /// Plot measurement to graph by gnuplot
        /// </summary>
        protected virtual void NotifyPlot()
        {
            // Plot as a graph
            foreach (var session in sessionQueue)
                session.Plot();
        }
    }
}
