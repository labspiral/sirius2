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
 * Description : MarkerStageXY 
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
    /// Custom Marker with X,Y Stage <br/>
    /// Move stage x,y at each entity center position and marks repeatly. <br/>
    /// </summary>
    /// <remarks>
    /// Used with RTC5,6,6e <br/>
    /// Used with <see cref="IRtc.ListBegin">IRtc.ListBegin</see> and <see cref="IRtc.ListEnd">IRtc.ListEnd</see>  to process whole <see cref="EntityLayer">EntityLayer</see>for speed up. <br/>
    /// Disabled <see cref="EntityLayerRtc.IsALC">EntityLayer.IsALC</see> feature. <br/>
    /// Disabled <c>IsExternalStart</c> feature. <br/>
    /// Disabled <c>IRtcMoF</c> feature. <br/>
    /// Disabled <c>IRtcMeasurement</c> feature. <br/>
    /// Disabled <c>IRtcCharacterSet</c> feature. <br/>
    /// </remarks>
    public class MarkerStageXY
        : MarkerBase
    {
        /// <summary>
        /// <c>IStageXy</c>
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Data")]
        [DisplayName("Stage")]
        [Description("Stage XY")]
        public virtual IStageXy StageXy { get; set; }

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
        public MarkerStageXY()
            : base()
        {
            listType = ListTypes.Auto;

            IsCheckTempOk = false;
            IsCheckPowerOk = false;
            IsCheckPositionAck = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="name">Name</param>
        public MarkerStageXY(int index, string name)
            : this()
        {
            Index = index;
            Name = name;
        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~MarkerStageXY()
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

            if (null == StageXy)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: assigned invalid StageXy instance");
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
            // -------------------------------------------------------------
            // Check X,Y Stage status
            if (StageXy.Status != StageStatus.Ready)
            {
                Logger.Log(Logger.Types.Error, $"marker: {this.Name} stage is not ready status");
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

            // Shallow copy for cross-thread issue
            layers = new List<EntityLayer>(Document.InternalData.Layers);

            CurrentOffset = Offset.Zero;
            CurrentOffsetIndex = 0;
            CurrentLayer = null;
            CurrentLayerIndex = 0;
            CurrentEntity = null;
            CurrentEntityIndex = 0;
            AccumulatedMarks++;

            Logger.Log(Logger.Types.Warn, $"marker [{Index}]: trying to start mark with offset(s)= {this.Offsets.Length}");
            this.thread = new Thread(this.MarkerThreadWithStageXy);                   
            this.thread.Name = $"MyMarker: {this.Name}";
            this.thread.Start();
            return true;
        }
        /// <inheritdoc/>
        public override bool Preview()
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

            if (Rtc.CtlGetStatus(RtcStatus.Busy))
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: busy now !");
                return false;
            }
            if (!Rtc.CtlGetStatus(RtcStatus.NoError))
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: rtc has a internal error. reset at first");
                return false;
            }
            if (Laser.IsError)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: laser has a error status. reset at first");
                return false;
            }

            if (IsCheckTempOk && !Rtc.CtlGetStatus(RtcStatus.TempOK))
            {
                Logger.Log(Logger.Types.Error, $"marker: {this.Name} scanner temp is no ok");
                return false;
            }
            if (IsCheckPowerOk && !Rtc.CtlGetStatus(RtcStatus.PowerOK))
            {
                Logger.Log(Logger.Types.Error, $"marker: {this.Name} scanner power is not ok !");
                return false;
            }
            if (IsCheckPositionAck && !Rtc.CtlGetStatus(RtcStatus.PositionAckOK))
            {
                Logger.Log(Logger.Types.Error, $"marker: {this.Name} scanner position is not acked");
                return false;
            }
            if (null == Document.Selected || 0 == Document.Selected.Length)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: select target entity to preview at first");
                return false;
            }
            var laserGuideControl = Laser as ILaserGuideControl;
            if (null == laserGuideControl)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: laser is not supported guide control");
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

            // Shallow copy for cross-thread issue
            layers = new List<EntityLayer>(Document.InternalData.Layers);

            Logger.Log(Logger.Types.Warn, $"marker [{Index}]: trying to start preview mark");
            this.thread = new Thread(this.MarkerThreadPreview);
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
            // Stop X,Y Stage
            StageXy.Stop();


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

            // -------------------------------------------------------------
            // Reset X,Y Stage
            StageXy.Reset();

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
        /// <param name="offset">Current <see cref="MarkerBase.CurrentOffset">CurrentOffset</see></param>
        /// <param name="layerIndex">Current layer of offset (0,1,2,...)</param>
        /// <param name="layer">Current <see cref="MarkerBase.CurrentLayer">CurrentLayer</see></param>
        /// <returns>Success or failed</returns>
        protected virtual bool LayerWork(int offsetIndex, Offset offset, int layerIndex, EntityLayer layer)
        {
            bool success = true;
            CurrentLayerIndex = layerIndex;
            CurrentLayer = layer;
            for (int i = 0; i < layer.Repeats; i++)
            {
                for (int j = 0; j < layer.Children.Count; j++)
                {
                    var entity = layer.Children[j];
                    CurrentEntityIndex = j;
                    CurrentEntity = entity;
                    if (!entity.IsMarkerable)
                        continue;
                    if (entity is IMarkerable markerable)
                    {
                        success &= EntityWork(offsetIndex, offset, layerIndex, layer, j, entity);
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
        /// <param name="offset">Current <see cref="MarkerBase.CurrentOffset">CurrentOffset</see></param>
        /// <param name="layerIndex">Current index of layer (0,1,2,...)</param>
        /// <param name="layer">Current <see cref="MarkerBase.CurrentLayer">CurrentLayer</see></param>
        /// <param name="entityIndex">Current index of entity (0,1,2,...)</param>
        /// <param name="entity">Current <see cref="MarkerBase.CurrentEntity">CurrentEntity</see></param>
        /// <returns>Success or failed</returns>
        protected virtual bool EntityWork(int offsetIndex, Offset offset, int layerIndex, EntityLayer layer, int entityIndex, IEntity entity)
        {
            bool success = true;
            success &= NotifyBeforeEntity(entity);
            if (!success)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to mark entity at before event handler"); 
                return success;
            }
            var rtc = this.Rtc;
            var laser = this.Laser;

            if (entity is IMarkerable markerable)
            {
                // -------------------------------------------------------------
                var realCenter = entity.BBox.RealCenter;
                // Move X,Y Stage at entity center position
                success &= StageXy.MoveAbs(realCenter.Xy.ToNumeric());
                // Wait until moving has done (or in-position)
                if (!success)
                {
                    Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to move stage");
                    return success;
                }

                // Begin Rtc list buffer
                success &= laser.ListBegin();
                success &= rtc.ListBegin(ListType);

                // Translate current entity to scanner center position
                // Push translate matrix into Rtc 
                rtc.MatrixStack.Push( System.Numerics.Matrix4x4.CreateTranslation(-realCenter.ToNumeric()));

                // Mark entity
                success &= markerable.Mark(this);
                
                // Pop translate matrix from Rtc 
                rtc.MatrixStack.Pop();

                if (success)
                {
                    success &= laser.ListEnd();
                    success &= rtc.ListEnd();
                    if (success)
                        success &= rtc.ListExecute(true);
                }
            }
            if (!success)
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to mark entity at after event handler"); 
                return success;
            }
            success &= NotifyAfterEntity(entity);
            return success;
        }

        /// <summary>
        /// Marker thread
        /// </summary>
        /// <remarks>        
        /// Move offset1 and Mark layers -> Move offset2 and Mark layers , ... <br/>
        /// </remarks>
        protected virtual void MarkerThreadWithStageXy()
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
            this.isInternalBusy = true;
            this.NotifyStarted();
            base.StartTime = base.EndTime = DateTime.Now;
            bool success = true;
            var oldMatrixStack = (IMatrixStack<System.Numerics.Matrix4x4>)rtc.MatrixStack.Clone();
          
            for (int i = 0; i < Offsets.Length; i++)
            {
                CurrentOffset = Offsets[i];
                CurrentOffsetIndex = i;
                rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                Logger.Log(Logger.Types.Debug, $"marker [{Index}]: offset index= {i}, xyzt= {Offsets[i].ToString()}");
                for (int j = 0; j < layers.Count; j++)
                {
                    var layer = layers[j];
                    if (!layer.IsMarkerable)
                        continue;
                    success &= NotifyBeforeLayer(layer);
                    if (!success)
                    {
                        Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to mark layer at before event handler"); 
                        break;
                    }
                    success &= LayerWork(i, Offsets[i], j, layer);
                    if (!success)
                        break;
                    success &= NotifyAfterLayer(layer);
                    if (!success)
                    {
                        Logger.Log(Logger.Types.Error, $"marker [{Index}]: fail to mark layer at after event handler");
                        break;
                    }
                }
                // Pop offset matrix
                rtc.MatrixStack.Pop();
                if (!success)
                    break;
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
            base.EndTime = DateTime.Now;
            this.isInternalBusy = false;
            this.NotifyEnded(success);
            if (success)
            {
                Logger.Log(Logger.Types.Info, $"marker [{Index}]: mark has finished with {base.ExecuteTime.TotalSeconds:F3}s");
            }
            else
            {
                Logger.Log(Logger.Types.Error, $"marker [{Index}]: mark has failed with {base.ExecuteTime.TotalSeconds:F3}s");
            }
        }
        /// <summary>
        /// Marker preview
        /// </summary>
        /// <remarks>
        /// Mark bounding box with <see cref="ILaserGuideControl">ILaserGuideControl</see>
        /// </remarks>
        protected virtual void MarkerThreadPreview()
        {
            var rtc = this.Rtc;
            var laser = this.Laser;
            var laserGuideControl = Laser as ILaserGuideControl;
            var document = this.Document;
            var rtc3D = rtc as IRtc3D;
            var rtc2ndHead = rtc as IRtc2ndHead;
            var rtcExtension = rtc as IRtcExtension;
            var rtcAlc = rtc as IRtcAutoLaserControl;
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtc != null);
            Debug.Assert(laser != null);
            Debug.Assert(document != null);
            var bbox = BoundingBox.RealBoundingBox(document.Selected); 
            Debug.Assert(!bbox.IsEmpty);

            this.isInternalBusy = true;
            bool success = true;
            success &= laserGuideControl.CtlGuide(true);
            var oldMatrixStack = (IMatrixStack<System.Numerics.Matrix4x4>)rtc.MatrixStack.Clone();

            var oldSpeedJump = rtc.SpeedJump;
            var oldSpeedMark = rtc.SpeedMark;
            success &= rtc.ListBegin(ListTypes.Auto);
            success &= laser.ListBegin();
            success &= rtc.ListSpeed(SpiralLab.Sirius2.Winforms.Config.MarkPreviewSpeed, SpiralLab.Sirius2.Winforms.Config.MarkPreviewSpeed);
            for (int j = 0; j < SpiralLab.Sirius2.Winforms.Config.MarkPreviewRepeats; j++)
            {
                for (int i = 0; i < Offsets.Length; i++)
                {
                    try
                    {
                        // Push offset matrix
                        rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                        // Rectangle by bouding box 
                        // 2 1
                        // 3 4
                        success &= rtc.ListJumpTo(new System.Numerics.Vector2(bbox.RealMax.X, bbox.RealMax.Y));
                        success &= rtc.ListMarkTo(new System.Numerics.Vector2(bbox.RealMin.X, bbox.RealMax.Y));
                        success &= rtc.ListMarkTo(new System.Numerics.Vector2(bbox.RealMin.X, bbox.RealMin.Y));
                        success &= rtc.ListMarkTo(new System.Numerics.Vector2(bbox.RealMax.X, bbox.RealMin.Y));
                        success &= rtc.ListMarkTo(new System.Numerics.Vector2(bbox.RealMax.X, bbox.RealMax.Y));
                    }
                    finally
                    {
                        // Pop offset matrix
                        rtc.MatrixStack.Pop();
                    }
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }

            if (success)
            {
                success &= rtc.ListJumpTo(System.Numerics.Vector2.Zero);
                success &= laser.ListEnd();
                success &= rtc.ListEnd();
                success &= rtc.ListExecute(true);
            }
            success &= rtc.CtlSpeed(oldSpeedJump, oldSpeedMark);
            success &= laserGuideControl.CtlGuide(false);
            rtc.MatrixStack = oldMatrixStack;
            this.isInternalBusy = false;
        }
       
    }
}
