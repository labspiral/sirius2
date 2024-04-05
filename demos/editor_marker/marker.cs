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
using SpiralLab.Sirius2.PowerMeter;
using SpiralLab.Sirius2.Winforms.Remote;

namespace Demos
{
    /// <summary>
    /// Custom MarkerRtc
    /// </summary>
    /// <remarks>
    /// Used with RTC5,6,6e <br/>
    /// Used with <see cref="IRtc.ListBegin">IRtc.ListBegin</see> and <see cref="IRtc.ListEnd">IRtc.ListEnd</see> at each <see cref="EntityLayer">EntityLayer</see>. <br/>
    /// Supported <see cref="EntityLayerRtc.IsALC">EntityLayer.IsALC</see> feature. <br/>
    /// Supported <see cref="MarkerRtc.IsExternalStart">IsExternalStart</see> feature. <br/>
    /// Supported useful features like as <see cref="MarkerRtc.MarkProcedures">MarkProcedures</see> and <see cref="MarkerRtc.MarkTargets">MarkTargets</see>. <br/>
    /// </remarks>
    public class MyRtcMarker
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
        /// Mark procedures
        /// </summary>
        public enum MarkProcedures
        {
            /// <summary>
            /// Order of marks: Mark Layer(s) at Offset1 -> Mark Layer(s) at Offset2, ...
            /// <code>
            /// //Pseudo codes
            /// for (int i = 0; i &lt; Offsets.Length; i++)
            /// {
            ///     foreach (var layer in Layers)
            ///     {
            ///         Laser.ListBegin();
            ///         Rtc.ListBegin();
            ///         ...
            ///         LayerWork(i, layer, Offsets[i]);
            ///         ...
            ///         Laser.ListEnd();
            ///         Rtc.ListEnd();
            ///         Rtc.ListExecute(true);
            ///         ...
            ///     }
            /// }
            /// </code>
            /// <remarks>
            /// Default: <see cref="MarkProcedures.LayerFirst">MarkProcedures.LayerFirst</see> 
            /// </remarks>
            /// </summary>
            LayerFirst = 0,
            /// <summary>
            /// Order of marks: Mark Layer1 at Offset(s) -> Mark Layer2 at Offset(s), ... 
            /// <code>
            /// //Pseudo codes
            /// foreach (var layer in Layers)
            /// {
            ///     Laser.ListBegin();
            ///     Rtc.ListBegin();        
            ///     for (int i = 0; i &lt; Offsets.Length; i++)
            ///     {
            ///         ...
            ///         LayerWork(i, layer, Offsets[i]);
            ///         ...
            ///     }
            ///     Laser.ListEnd();
            ///     Rtc.ListEnd();
            ///     Rtc.ListExecute(true);
            /// }
            /// </code>
            /// </summary>
            OffsetFirst = 1,
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
        /// Mark procedure
        /// </summary>
        /// <remarks>
        /// Default: <see cref="MarkProcedures.LayerFirst">MarkProcedures.LayerFirst</see> <br/>
        /// </remarks>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Data")]
        [DisplayName("Mark Proc")]
        [Description("Mark Procedure")]
        public virtual MarkProcedures MarkProcedure
        {
            get { return markProcedure; }
            set
            {
                if (this.IsBusy)
                {
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to set mark procedure during busy");
                    return;
                }
                var oldMarkProcedure = markProcedure;
                markProcedure = value;
                if (markProcedure != oldMarkProcedure)
                    this.NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Internal <c>MarkProcedures</c>
        /// </summary>
        protected MarkProcedures markProcedure = MarkProcedures.LayerFirst;

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
            set
            {
                if (this.IsBusy)
                {
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to set external start during busy");
                    return;
                }
                isExternalStart = value;
                if (isExternalStart)
                {
                    listType = ListTypes.Single;
                    if (1 != this.Document.InternalData.Layers.Count)
                        MessageBox.Show($"Should be single layer only to use external /START", "Warning", MessageBoxButtons.OK);
                }
            }
        }
        private bool isExternalStart = false;

        /// <summary>
        /// <c>ListTypes</c>
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
            ListType = ListTypes.Auto;
            isMeasurementPlot = false;
            markTarget = MarkTargets.All;
            markProcedure = MarkProcedures.LayerFirst;

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
            if (!this.disposed)
            {
                if (disposing)
                {
                }
                this.Stop();
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
            // Clear registered characterset when ready
            TextRegisterHelper.Unregister(this);
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

            if (IsExternalStart)
            {
                // Should be exist only single layer for External /START 
                if (Document.InternalData.Layers.Count != 1)
                {
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: should be single layer only for external /START");
                    return false;
                }
                // list type to single for external /START by forcily 
                listType = ListTypes.Single;
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

            if (IsExternalStart)
                Logger.Log(Logger.Types.Warn, $"marker [{Index}]: trying to start mark by external trigger with {this.Offsets.Length} offsets");
            else
                Logger.Log(Logger.Types.Warn, $"marker [{Index}]: trying to start mark with {this.Offsets.Length} offsets");

            Logger.Log(Logger.Types.Warn, $"marker [{Index}]: trying to start mark with target= {MarkTarget}, proc= {MarkProcedure}, offset(s)= {this.Offsets.Length}");
            switch (MarkProcedure)
            {
                default:
                    this.thread = new Thread(this.MarkerThreadLayerFirst);
                    break;
                case MarkProcedures.OffsetFirst:
                    this.thread = new Thread(this.MarkerThreadOffsetFirst);
                    break;
            }
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
                        Logger.Log(Logger.Types.Error, $"marker [{Index}]: waiting for stop but timed out");
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
        /// Mark each <see cref="EntityLayer">EntityLayer</see> 
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
                    if (!entity.IsMarkerable)
                        continue;
                    entity.Parent = layer;
                    if (entity is IMarkerable markerable)
                    {
                        CurrentEntityIndex = j;
                        CurrentEntity = entity;
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
        /// Mark each <see cref="IEntity">IEntity</see> 
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
        /// Marker thread #1
        /// </summary>
        /// <remarks>
        /// <see cref="MarkProcedures.LayerFirst">LayerFirst</see> <br/>
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
                    if (null != rtcAlc && layer.IsALC)
                    {
                        success &= rtcAlc.CtlAlcByPositionTable(layer.AlcByPositionTable);
                        switch (layer.AlcSignal)
                        {
                            case AutoLaserControlSignals.ExtDO16:
                            case AutoLaserControlSignals.ExtDO8:
                                success &= rtcAlc.CtlAlc<uint>(layer.AlcSignal, layer.AlcMode, (uint)layer.AlcPercentage100, (uint)layer.AlcMinValue, (uint)layer.AlcMaxValue);
                                break;
                            default:
                                success &= rtcAlc.CtlAlc<double>(layer.AlcSignal, layer.AlcMode, layer.AlcPercentage100, layer.AlcMinValue, layer.AlcMaxValue);
                                break;
                        }
                    }
                    if (!success)
                        break;
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
                        success &= rtcAlc.CtlAlc<uint>(AutoLaserControlSignals.Disabled, AutoLaserControlModes.Disabled, 0, 0, 0);
                    }
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
                    Logger.Log(Logger.Types.Info, $"marker [{Index}]: mark has finished with {this.TimeSpan.TotalSeconds:F3}s");
                    if (this.IsMeasurementPlot)
                        this.NotifyPlot();
                }
                else
                {
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: mark has failed with {this.TimeSpan.TotalSeconds:F3}s");
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
                Logger.Log(Logger.Types.Warn, $"marker [{Index}]: waiting for /START trigger");
            }
        }
        /// <summary>
        /// Marker thread #2
        /// </summary>
        /// <remarks>
        /// <see cref="MarkProcedures.OffsetFirst">OffsetFirst</see> <br/>
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
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to mark layer at before event handler"); ;
                    break;
                }
                if (null != rtcAlc && layer.IsALC)
                {
                    success &= rtcAlc.CtlAlcByPositionTable(layer.AlcByPositionTable);
                    switch (layer.AlcSignal)
                    {
                        case AutoLaserControlSignals.ExtDO16:
                        case AutoLaserControlSignals.ExtDO8:
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
                        CurrentOffset = Offsets[i];
                        CurrentOffsetIndex = i;
                        rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                        Logger.Log(Logger.Types.Debug, $"marker [{Index}]: offset index= {i}, xyzt= {Offsets[i].ToString()}");
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
                    success &= rtcAlc.CtlAlc<uint>(AutoLaserControlSignals.Disabled, AutoLaserControlModes.Disabled, 0, 0, 0);
                }
                if (!success)
                    break;
                success &= NotifyAfterLayer(layer);
                if (!success)
                {
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to mark layer at after event handler");
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
                        Logger.Log(Logger.Types.Warn, $"marker [{Index}]: mof out of range. markinfg info= {info.Value}");
                    }
                    else if (rtc is Rtc6 rtc6)
                    {
                        var info = rtc6.MarkingInfo;
                        Logger.Log(Logger.Types.Warn, $"marker [{Index}]: mof out of range. markinfg info= {info.Value}");
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
                    Logger.Log(Logger.Types.Info, $"marker [{Index}]: mark has finished with {this.TimeSpan.TotalSeconds:F3}s");
                    if (this.IsMeasurementPlot)
                        this.NotifyPlot();
                }
                else
                {
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: mark has failed with {this.TimeSpan.TotalSeconds:F3}s");
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
                Logger.Log(Logger.Types.Warn, $"marker [{Index}]: waiting for /START trigger");
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
