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
 * Description : SiriusEditor usercontrol
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Runtime.InteropServices;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.UI;
using SpiralLab.Sirius2.IO;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.PowerMeter;
using SpiralLab.Sirius2.PowerMap;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Scanner.Rtc.SyncAxis;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.Common;
using SpiralLab.Sirius2.Winforms.Remote;
using OpenTK;

namespace Demos
{
    /// <summary>
    /// SiriusEditorUserControl
    /// </summary>
    /// <remarks>
    /// User can insert(or create) usercontrol at own winforms. <br/>
    /// <img src="images/siriuseditorcontrol.png"/> <br/>
    /// 1. <see cref="TreeViewUserControl">TreeViewUserControl</see> <br/>
    /// 2. <see cref="TreeViewBlockUserControl">TreeViewBlockUserControl</see> <br/>
    /// 3. <see cref="PenUserControl">PenUserControl</see> <br/>
    /// 4. <see cref="EditorUserControl">EditorUserControl</see> <br/>
    /// 5. <see cref="RtcUserControl">RtcUserControl</see> <br/>
    /// 6. <see cref="LaserUserControl">LaserUserControl</see> <br/>
    /// 7. <see cref="MarkerUserControl">MarkerUserControl</see> <br/>
    /// 8. <see cref="ManualUserControl">ManualUserControl</see> <br/>
    /// 9. <see cref="RtcDIUserControl">RtcDIUserControl</see> <br/>
    /// 10. <see cref="RtcDOUserControl">RtcDOUserControl</see> <br/>
    /// 11. <see cref="RtcDOUserControl">RtcDOUserControl</see> <br/>
    /// 12. <see cref="PowerMeterUserControl">PowerMeterUserControl</see> <br/>
    /// 13. <see cref="PowerMapUserControl">PowerMapUserControl</see> <br/>
    /// 14. <see cref="ScriptUserControl">ScriptUserControl</see> <br/>
    /// 15. <see cref="RemoteUserControl">RemoteUserControl</see> <br/>
    /// 16. <see cref="PropertyGridUserControl">PropertyGridUserControl</see> <br/>
    /// 17. <see cref="LogUserControl">LogUserControl</see> <br/>
    /// </remarks>
    public partial class SiriusEditorUserControl : Form
    {
        /// <summary>
        /// Event for before open sirius file at <c>SiriusEditorUserControl</c>
        /// </summary>
        /// <remarks>
        /// User can register event handler for customized dialog-box before open sirius file. <br/>
        /// Event will be fired when user has click 'Open' menu at <c>SiriusEditorUserControl</c>. <br/>
        /// If user event handler is not attached, default routine has executed. <br/>
        /// If returned 'False', default routine would be executed. <br/>
        /// </remarks>
        public event Func<SiriusEditorUserControl, bool> OnOpenBefore;
        /// <summary>
        /// Notify before open sirius file
        /// </summary>
        /// <returns>Success or failed</returns>
        bool NotifyOpenBefore()
        {
            var receivers = OnOpenBefore?.GetInvocationList();
            if (null == receivers)
                return false;
            bool success = true;
            foreach (Func<SiriusEditorUserControl, bool> receiver in receivers)
                success &= receiver.Invoke(this);
            return success;
        }
        /// <summary>
        /// Event for save sirius file at <c>SiriusEditorUserControl</c>
        /// </summary>
        /// <remarks>
        /// User can register event handler for customized dialog-box before save sirius file. <br/>
        /// Event will be fired when user has click 'Save' menu at <c>SiriusEditorUserControl</c>. <br/>
        /// If user event handler is not attached, default routine has executed. <br/>
        /// If returned 'False', default routine would be executed. <br/>
        /// </remarks>
        public event Func<SiriusEditorUserControl, bool> OnSaveBefore;
        /// <summary>
        /// Notify save sirius file
        /// </summary>
        /// <returns>Success or failed</returns>
        bool NotifySaveBefore()
        {
            var receivers = OnSaveBefore?.GetInvocationList();
            if (null == receivers)
                return false;
            bool success = true;
            foreach (Func<SiriusEditorUserControl, bool> receiver in receivers)
                success &= receiver.Invoke(this);
            return success;
        }

        /// <summary>
        /// Title name
        /// </summary>
        public string TitleName
        {
            get { return lblName.Text; }
            set { lblName.Text = value; }
        }

        /// <summary>
        /// Disable UI controls or not
        /// </summary>
        /// <remarks>
        /// To do not allow user operations during marker is working. <br/>
        /// </remarks>
        public bool IsDisableControl
        {
            get { return isDisableControl; }
            set
            {
                isDisableControl = value;
                if (isDisableControl)
                {
                    TreeViewCtrl.Enabled = false;
                    TreeViewBlockCtrl.Enabled = false;
                    PropertyGridCtrl.Enabled = false;
                    EditorCtrl.Enabled = false;
                    //PenCtrl.Enabled = false;
                    LaserCtrl.Enabled = false;
                    //MarkerCtrl.Enabled = false;
                    OffsetCtrl.Enabled = false;
                    RtcDICtrl.Enabled = false;
                    RtcDOCtrl.Enabled = false;
                    ManualCtrl.Enabled = false;
                }
                else
                {
                    TreeViewCtrl.Enabled = true;
                    TreeViewBlockCtrl.Enabled = true;
                    PropertyGridCtrl.Enabled = true;
                    EditorCtrl.Enabled = true;
                    //PenCtrl.Enabled = false;
                    LaserCtrl.Enabled = true;
                    //MarkerCtrl.Enabled = true;
                    OffsetCtrl.Enabled = true;
                    RtcDICtrl.Enabled = true;
                    RtcDOCtrl.Enabled = true;
                    ManualCtrl.Enabled = true;
                }
            }
        }
        private bool isDisableControl;

        /// <summary>
        /// <c>IDocument</c> (aka. Recipe data)
        /// </summary>
        /// <remarks>
        /// <c>Document</c> would be created by <c>OnLoad</c> event handler.<br/>
        /// Do action by <c>IDocument.Act...</c> functions. <br/>
        /// </remarks>
        public IDocument Document
        {
            get { return document; }
            protected set 
            {
                if (document == value)
                    return;
                if (document != null)
                {
                    PropertyGridCtrl.SelecteObject = null;
                    document.OnSelected -= Document_OnSelected;
                    document.OnSaved -= Document_OnSaved;
                    document.OnOpened -= Document_OnOpened;
                    if (EditorCtrl.View is ViewBase vb)
                    {
                        vb.Renderer.MouseMove -= Renderer_MouseMove;
                        vb.Renderer.Paint -= Renderer_Paint;
                    }
                }
                document = value;
                PropertyGridCtrl.Document = document;
                PenCtrl.Document = document;
                TreeViewCtrl.Document = document;
                TreeViewBlockCtrl.Document = document;
                EditorCtrl.Document = document;
                PowerMapCtrl.Document = document;
                //RtcControl
                //LaserCOntrol
                if (document != null)
                {
                    document.OnSelected += Document_OnSelected;
                    document.OnSaved += Document_OnSaved;
                    document.OnOpened += Document_OnOpened;
                    if (EditorCtrl.View is ViewBase vb)
                    {
                        vb.Renderer.MouseMove += Renderer_MouseMove;
                        vb.Renderer.Paint += Renderer_Paint;
                    }
                    PropertyGridCtrl.SelecteObject = document.Selected;
                }
            }
        }  
        private IDocument document;

        /// <summary>
        /// <c>IRtc</c>
        /// </summary>
        /// <remarks>
        /// Created by <c>ScannerFactory</c>. <br/>
        /// </remarks>
        public IRtc Rtc
        {
            get { return rtc; }
            set
            {
                if (rtc == value)
                    return;                
                if (rtc != null)
                {
                    if (rtc is IRtcMoF mof)
                    {
                        mof.OnEncoderChanged -= Mof_OnEncoderChanged;
                    }
                }
                
                rtc = value;
                RtcCtrl.Rtc = rtc;
                RtcDICtrl.Rtc = rtc;
                RtcDOCtrl.Rtc = rtc;
                ManualCtrl.Rtc = rtc;
                EditorCtrl.Rtc = rtc;
                //TreeViewCtrl.Rtc = rtc;
                TreeViewBlockCtrl.Rtc = rtc;
                PowerMapCtrl.Rtc = rtc;

                if (rtc != null)
                {
                    PropertyVisibility();
                    MenuVisibility();
                    if (rtc is IRtcMoF mof)
                    {
                        mof.OnEncoderChanged += Mof_OnEncoderChanged;
                    }
                }
            }
        }
        private IRtc rtc;

        /// <summary>
        /// <c>ILaser</c>
        /// </summary>
        /// <remarks>
        /// Created by <c>LaserFactory</c>. <br/>
        /// </remarks>
        public ILaser Laser
        {
            get { return laser; }
            set
            {
                if (laser == value)
                    return;
                laser = value;
                LaserCtrl.Laser = laser;
                ManualCtrl.Laser = laser;
                PowerMeterCtrl.Laser = laser;
                PowerMapCtrl.Laser = laser;
                if (null != laser)
                {
                    EntityPen.PropertyVisibility(laser);
                    var powerControl = laser as ILaserPowerControl;
                    foreach (var pen in document.InternalData.Pens)
                    {
                        pen.PowerMax = laser.MaxPowerWatt;
                        if (null != powerControl)
                            pen.PowerMap = powerControl.PowerMap;
                        else
                            pen.PowerMap = null;
                    }
                }
            }
        }
        private ILaser laser;

        /// <summary>
        /// <c>IMarker</c>
        /// </summary>
        /// <remarks>
        /// Created by <c>MarkerFactory</c>. <br/>
        /// </remarks>
        public IMarker Marker
        {
            get { return marker; }
            set
            {
                if (marker == value)
                    return;
                if (marker != null)
                {
                    marker.OnStarted -= Marker_OnStarted;
                    marker.OnEnded -= Marker_OnEnded;
                }
                marker = value;
                MarkerCtrl.Marker = marker;
                OffsetCtrl.Marker = marker;
                ScriptCtrl.Marker = marker;
                TreeViewCtrl.Marker = marker;
                EditorCtrl.View.Marker = marker;
                //marker browsable
                if (marker != null)
                {
                    marker.OnStarted += Marker_OnStarted;
                    marker.OnEnded += Marker_OnEnded;
                }
            }
        }
        private IMarker marker;

        /// <summary>
        /// <c>IView</c>
        /// </summary>
        /// <remarks>
        /// Created by internally. <br/>
        /// </remarks>
        public IView View
        {
            get { return EditorCtrl.View; }
        }

        /// <summary>
        /// <c>IRemote</c>
        /// </summary>
        /// <remarks>
        /// Created by <c>RemoteFactory</c>. <br/>
        /// To do control by remotely. <br/>
        /// </remarks>
        public IRemote Remote 
        {
            get { return remote; }
            set
            {
                if (remote == value)
                    return;
                if (remote != null)
                {
                }

                remote = value;
                remoteUserControl1.Remote = remote;
                if (marker != null)
                {
                }
            }
        }
        private IRemote remote;

        /// <summary>
        /// <c>IPowerMeter</c>
        /// </summary>
        /// <remarks>
        /// Created by <c>PowerMeterFactory</c>. <br/>
        /// To do control <c>IPowerMeter</c>. <br/>
        /// </remarks>
        public IPowerMeter PowerMeter
        {
            get { return powerMeter; }
            set
            {
                if (powerMeter == value)
                    return;
                if (powerMeter != null)
                {
                    powerMeter.OnStarted -= PowerMeter_OnStarted;
                    powerMeter.OnStopped -= PowerMeter_OnStopped;
                    powerMeter.OnMeasured -= PowerMeter_OnMeasured;
                    powerMeter.OnCleared -= PowerMeter_OnCleared;
                }
                powerMeter = value;
                PowerMeterCtrl.PowerMeter = powerMeter;
                PowerMapCtrl.PowerMeter = powerMeter;
                if (powerMeter != null)
                {
                    lblPowerWatt.Text = "0.0 W";
                    powerMeter.OnStarted += PowerMeter_OnStarted;
                    powerMeter.OnStopped += PowerMeter_OnStopped;
                    powerMeter.OnMeasured += PowerMeter_OnMeasured;
                    powerMeter.OnCleared += PowerMeter_OnCleared;
                }     
            }
        }
        private IPowerMeter powerMeter;
            
        /// <summary>
        /// RTC DI extension1 port (16 bits)
        /// </summary>
        public IDInput DIExt1 
        { 
            get { return dIExt1; }
            set {
                if (dIExt1 == value)
                    return;
                if (dIExt1 != null)
                {
                    dIExt1?.Dispose();
                    dIExt1 = null;
                }
                dIExt1 = value; 
                rtcDIUserControl1.DIExt1 = dIExt1;
            } 
        }
        private IDInput dIExt1;
        /// <summary>
        /// RTC DI laser port (2 bits)
        /// </summary>
        public IDInput DILaserPort
        {
            get { return dILaserPort; }
            set
            {
                if (dILaserPort == value)
                    return;
                if (dILaserPort != null)
                {
                    dILaserPort?.Dispose();
                    dILaserPort = null;
                }
                dILaserPort = value;
                rtcDIUserControl1.DILaserPort = dILaserPort;
            }
        }
        private IDInput dILaserPort;
        /// <summary>
        /// RTC DO extension1 port (16 bits)
        /// </summary>
        public IDOutput DOExt1
        {
            get { return dOExt1; }
            set
            {
                if (dOExt1 == value)
                    return;
                if (dOExt1 != null)
                {
                    dOExt1?.Dispose();
                    dOExt1 = null;
                }
                dOExt1 = value;
                rtcDOUserControl1.DOExt1 = dOExt1;
            }
        }
        private IDOutput dOExt1;
        /// <summary>
        /// RTC DO extension2 port (8 bits)
        /// </summary>
        public IDOutput DOExt2
        {
            get { return dOExt2; }
            set
            {
                if (dOExt2 == value)
                    return;
                if (dOExt2 != null)
                {
                    dOExt2?.Dispose();
                    dOExt2 = null;
                }
                dOExt2 = value;
                rtcDOUserControl1.DOExt2 = dOExt2;
            }
        }
        private IDOutput dOExt2;
        /// <summary>
        /// RTC DO laser port (2 bits)
        /// </summary>
        public IDOutput DOLaserPort
        {
            get { return dOLaserPort; }
            set
            {
                if (dOLaserPort == value)
                    return;
                if (dOLaserPort != null)
                {
                    dOLaserPort?.Dispose();
                    dOLaserPort = null;
                }
                dOLaserPort = value;
                rtcDOUserControl1.DOLaserPort = dOLaserPort;
            }
        }
        private IDOutput dOLaserPort;

        /// <summary>
        /// Treeview user control for <c>IEntity</c> within <c>EntityLayer</c> nodes
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.TreeViewUserControl TreeViewCtrl
        { 
            get { return treeViewControl1; } 
        }
        /// <summary>
        /// Treeview user control for <c>EntityBlock</c> nodes
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.TreeViewBlockUserControl TreeViewBlockCtrl
        {
            get { return treeViewBlockControl1; }
        }
        /// <summary>
        /// PropertyGrid user control for properties of <c>IEntity</c>
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.PropertyGridUserControl PropertyGridCtrl
        {
            get { return propertyGridControl1; }
        }
        /// <summary>
        /// Editor(by OpenTK) user control for rendering view 
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.EditorUserControl EditorCtrl
        {
            get { return editorControl1; }
        }
        /// <summary>
        /// User control for list of <c>EntityPen</c>
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.PenUserControl PenCtrl
        {
            get { return penControl1; }
        }
        /// <summary>
        /// PropertyGrid user control for <c>ILaser</c>
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.LaserUserControl LaserCtrl
        {
            get { return laserControl1; }
        }
        /// <summary>
        /// PropertyGrid user control for <c>IRtc</c>
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.RtcUserControl RtcCtrl
        {
            get { return rtcControl1; }
        }
        /// <summary>
        /// PropertyGrid user control for <c>IMarer</c>
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.MarkerUserControl MarkerCtrl
        {
            get { return markerControl1; }
        }
        /// <summary>
        /// User control for list of <c>IMarker.Offsets</c>
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.OffsetUserControl OffsetCtrl
        {
            get { return offsetControl1; }
        }
        /// <summary>
        /// User control for RTC DI (extension 1 and laser port)
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.RtcDIUserControl RtcDICtrl
        {
            get { return rtcDIUserControl1; }
        }
        /// <summary>
        /// User control for RTC DO (extension 1,2 and laser port)
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.RtcDOUserControl RtcDOCtrl
        {
            get { return rtcDOUserControl1; }
        }
        /// <summary>
        /// User control for manual control
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.ManualUserControl ManualCtrl
        {
            get { return manualUserControl1; }
        }
        /// <summary>
        /// User control for <c>IPowerMeter</c>
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.PowerMeterUserControl PowerMeterCtrl
        {
            get { return powerMeterControl1; }
        }
        /// <summary>
        ///  User control for <c>IPowerMap</c>
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.PowerMapUserControl PowerMapCtrl
        {
            get { return powerMapControl1; }
        }
        /// <summary>
        /// User control for <c>IScript</c>
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.ScriptUserControl ScriptCtrl
        {
            get { return scriptControlControl1; }
        }
        /// <summary>
        /// PropertyGrid user control for <c>IRemote</c>
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.RemoteUserControl RemoteCtrl
        {
            get { return remoteUserControl1; }
        }
        /// <summary>
        /// User control for logged messages
        /// </summary>
        public SpiralLab.Sirius2.Winforms.UI.LogUserControl LogCtrl
        {
            get { return logControl1; }
        }

        System.Windows.Forms.Timer timerProgress = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timerStatus  = new System.Windows.Forms.Timer();
        Stopwatch timerProgressStopwatch = new Stopwatch();


        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Create devices likes as <c>IRtc</c>, <c>ILaser</c> and <c>IMarker</c> and assign. <br/>
        /// Digital I/O devices likes as <c>DInput</c>s, <c>DInput</c>s are created when assign <c>IRtc</c> by automatically. <br/>
        /// Create <c>IMarker</c> and assign. <br/>
        /// <c>IDocument</c> is created by automatically. <br/>
        /// </remarks>
        public SiriusEditorUserControl()
        {
            InitializeComponent();
        
            VisibleChanged += SiriusEditorUserControl_VisibleChanged;
            Disposed += SiriusEditorUserControl_Disposed;

            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            timerProgress.Interval = 100;
            timerProgress.Tick += TimerProgress_Tick;
            timerStatus.Interval = 100;
            timerStatus.Tick += TimerStatus_Tick;
            lblEncoder.DoubleClick += LblEncoder_DoubleClick;
            lblEncoder.DoubleClickEnabled = true;

            lblHelp.Click += LblHelp_Click;
            btnAbout.Click += BtnAbout_Click;
            btnNew.Click += BtnNew_Click;
            btnOpen.Click += BtnOpen_Click;
            btnImportFile.Click += BtnImportFile_Click;
            btnSave.Click += BtnSave_Click;
            btnCopy.Click += BtnCopy_Click;
            btnCut.Click += BtnCut_Click;
            btnPaste.Click += BtnPaste_Click;
            btnPasteArray.Click += BtnPasteArray_Click;
            btnZoomIn.Click += BtnZoomIn_Click;
            btnZoomOut.Click += BtnZoomOut_Click;
            btnZoomFit.Click += BtnZoomFit_Click;
            btnDelete.Click += BtnDelete_Click;

            btnLine.Click += BtnLine_Click;
            btnPoint.Click += BtnPoint_Click;
            btnPoints.Click += BtnPoints_Click;
            btnRectangle.Click += BtnRectangle_Click;
            btnArc.Click += BtnArc_Click;
            btnCircle.Click += BtnCircle_Click;
            btnTrepan.Click += BtnTrepan_Click;
            btnSpiral.Click += BtnSpiral_Click;
            btnText.Click += BtnText_Click;
            btnImageText.Click += BtnImageText_Click;
            btnRaster.Click += BtnRaster_Click;
            btnCircularText.Click += BtnCircularText_Click;
            btnCharacterSetText.Click += BtnCharacterSetText_Click;
            btnSiriusText.Click += BtnSiriusText_Click;
            btnSiriusCharacterSetText.Click += BtnSiriusCharacterSetText_Click;

            mnuDataMatrix.Click += MnuDataMatrix_Click;
            mnuQRCode.Click += MnuQRCode_Click;
            mnuPDF417.Click += MnuPDF417_Click;
            mnuBarcode1D.Click += MnuBarcode1D_Click;
            mnuMeasurementBeginEnd.Click += MnuMeasurementBeginEnd_Click;
            mnuTimer.Click += MnuTimer_Click;
            mnuJumpTo.Click += MnuJumpTo_Click;

            mnuMoFXYBeginEnd.Click += MnuMofXYBeginEnd_Click;
            mnuMoFXYWait.Click += MnuMofXYWait_Click;
            mnuMoFAngularBeginEnd.Click += MnuMofAngularBeginEnd_Click;
            mnuMoFAngularWait.Click += MnuMofAngularWait_Click;
            mnuMoFExternalStartDelay.Click += MnuMoFExternalStartDelay_Click;

            mnuZDelta.Click += MnuZDelta_Click;
            mnuZDefocus.Click += MnuZDefocus_Click;

            mnuMarginLeft.Click += MnuMarginLeft_Click;
            mnuMarginRight.Click += MnuMarginRight_Click;
            mnuMarginTop.Click += MnuMarginTop_Click;
            mnuMarginBottom.Click += MnuMarginBottom_Click;
            mnuAlcDefinedVector.Click += MnuAlcDefinedVector_Click;

            mnuWriteData.Click += MnuWriteData_Click;
            mnuWriteDataExt16.Click += MnuWriteDataExt16_Click;
            mnuWriteDataExt16Cond.Click += MnuWriteDataExt16Cond_Click;
            mnuWaitDataExt16Cond.Click += MnuWaitDataExt16Cond_Click;
            mnuWaitDataExt16EdgeCond.Click += MnuWaitDataExt16EdgeCond_Click;
            mnuScriptEvent.Click += MnuScriptEvent_Click;

            lblRemote.DoubleClick += LblRemote_DoubleClick;
            lblRemote.DoubleClickEnabled = true;
        }

        /// <inheritdoc/>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InternalOnLoad(e);
        }
        private void InternalOnLoad(EventArgs e)
        {
            //TreeViewCtrl.View = EditorCtrl.View;
            TreeViewBlockCtrl.View = EditorCtrl.View;
            PropertyGridCtrl.View = EditorCtrl.View;

            // Create document by default 
            this.Document = DocumentFactory.CreateDefault();
        }
        /// <inheritdoc/>
        public override void Refresh()
        {
            base.Refresh();
            PropertyGridCtrl.Refresh();
            OffsetCtrl.Refresh();
            EditorCtrl.View.Render();
        }

        /// <inheritdoc/>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Stop: F6
            if (keyData == Config.KeyboardMarkerStop)
            {
                if (null != Marker)
                {
                    Marker.Stop();
                    return true;
                }
            }
            // Start: F5
            else if (keyData == Config.KeyboardMarkerStart)
            {
                if (null != Marker)
                {
                    if (!Marker.IsBusy)
                    {
                        var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to start mark ?", "Warning", MessageBoxButtons.YesNo);
                        DialogResult dialogResult = form.ShowDialog(this);
                        if (dialogResult == DialogResult.Yes)
                        {
                            Marker.Start();
                            return true;
                        }
                    }
                }
            }
            // Reset: F8
            else if (keyData == Config.KeyboardMarkerReset)
            {
                if (null != Marker)
                {
                    Marker.Reset();
                    return true;
                }
            }
            // Cursor: F9
            else if (keyData == Config.KeyboardMoveToCursor)
            {
                if (this.Focused)
                    return Config.NotifyMoveToCursor(Document, this.lastCurrentPos);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void SiriusEditorUserControl_Disposed(object sender, EventArgs e)
        {
            timerStatus.Enabled = false;
            timerProgress.Enabled = false;
            timerStatus.Tick -= TimerStatus_Tick;
            timerProgress.Tick -= TimerProgress_Tick;
        }
        private void SiriusEditorUserControl_VisibleChanged(object sender, EventArgs e)
        {
            timerStatus.Enabled = Visible;
        }
        /// <summary>
        /// Menu visibility
        /// </summary>
        private void MenuVisibility()
        {
            Debug.Assert(rtc != null);
            if (null == rtc)
                return;
            switch (rtc.RtcType)
            {
                case RtcTypes.RtcVirtual:
                    break;
                case RtcTypes.Rtc4:
                case RtcTypes.Rtc5:
                case RtcTypes.Rtc6:
                case RtcTypes.Rtc6e:
                    break;
                case RtcTypes.Rtc6SyncAxis:
                    btnImageText.Enabled = false;
                    mnuMeasurementBeginEnd.Enabled = false;
                    mnuMoF.Enabled = false;
                    mnuZDelta.Enabled = false;
                    mnuZDefocus.Enabled = false;
                    mnuWriteDataExt16Cond.Enabled = false;
                    mnuWaitDataExt16Cond.Enabled = false;

                    lblEncoder.Visible = false;
                    btnCharacterSetText.Enabled = false;
                    btnSiriusCharacterSetText.Enabled = false;
                    break;
            }
        }
        /// <summary>
        /// Specific property visibility
        /// </summary>
        private void PropertyVisibility()
        {
            EntityPen.PropertyVisibility(rtc);
            EntityPen.PropertyVisibility(laser);
            EntityLayer.PropertyVisibility(rtc);
            EntityPoints.PropertyVisibility(rtc);
            EntityRampBegin.PropertyVisibility(rtc);
        }
        private void VisibilityByMarking(bool enable)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;
            if (!IsDisableControl)
            {
                tlsTop.Enabled = enable;
                tlsTop2.Enabled = enable;
                TreeViewCtrl.Enabled = enable;
                TreeViewBlockCtrl.Enabled = enable;
                EditorCtrl.Enabled = enable;
                PenCtrl.Enabled = enable;
                LaserCtrl.Enabled = enable;
                RtcCtrl.Enabled = enable;
                OffsetCtrl.Enabled = enable;
                PropertyGridCtrl.Enabled = enable;
            }
            DoRender();
        }
        /// <summary>
        /// Switch view mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null == Document)
                return;
            Document.ActSelectClear();
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    EditorCtrl.View.ViewMode = ViewModes.Entity;
                    // Regen whole document data if modified 
                    EditorCtrl.Document.ActRegen();
                    break;
                case 1:
                    EditorCtrl.View.ViewMode = ViewModes.Block;
                    break;
            }
            EditorCtrl.View.Render();
        }

        private void MnuQRCode_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateQRCode("SIRIUS2", Barcode2DCells.Dots, 3, 2, 2);
            document.ActAdd(entity);
        }
        private void MnuDataMatrix_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateDataMatrix("SIRIUS2", Barcode2DCells.Dots, 3, 2, 2);
            document.ActAdd(entity);
        }
        private void MnuPDF417_Click(object sender, EventArgs e)
        {
            double height = 2;
            var entity = EntityFactory.CreatePDF417("SIRIUS2", Barcode2DCells.Dots, 3, height * 3.75, height);
            document.ActAdd(entity);
        }
        private void MnuBarcode1D_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateBarcode("1234567890", Barcode1DFormats.Code128, 3, 5, 2);
            document.ActAdd(entity);
        }
        private void MnuWriteDataExt16_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateWriteDataExt16(0, false);
            document.ActAdd(entity);
        }
        private void MnuWriteDataExt16Cond_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateWriteDataExt16Cond("0000 0000 0000 0000", "0000 0000 0000 0000", "0000 0000 0000 0000");
            document.ActAdd(entity);
        }
        private void MnuWaitDataExt16Cond_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateWaitDataExt16Cond("0000 0000 0000 0000", "0000 0000 0000 0000");
            document.ActAdd(entity);
        }
        private void MnuWaitDataExt16EdgeCond_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateWaitDataExt16EdgeCond(0);
            document.ActAdd(entity);
        }
        private void MnuScriptEvent_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateScriptEvent();
            document.ActAdd(entity);
        }
        private void MnuWriteData_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateWriteData(ExtensionChannels.ExtAO2, 0);
            document.ActAdd(entity);
        }
        private void MnuAlcDefinedVector_Click(object sender, EventArgs e)
        {
            {
                var entity = EntityFactory.CreateRampEnd();
                document.ActAdd(entity);
            }
            {
                var entity = EntityFactory.CreateRampBegin(AutoLaserControlSignals.Frequency, 50 * 1000);
                document.ActInsert(entity, document.ActiveLayer, 0);
            }
        }
        private void MnuMarginBottom_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignments.Bottom);
            DoRender();
        }
        private void MnuMarginTop_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignments.Top);
            DoRender();
        }
        private void MnuMarginRight_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignments.Right);
            DoRender();
        }
        private void MnuMarginLeft_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignments.Left);
            DoRender();
        }
        private void MnuZDelta_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateZDelta(0);
            document.ActAdd(entity);
        }
        private void MnuZDefocus_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateZDefocus(0);
            document.ActAdd(entity);
        }
        private void MnuMofAngularWait_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateMoFWait(RtcEncoderWaitConditions.Over, 90);
            document.ActAdd(entity);
        }
        private void MnuMofAngularBeginEnd_Click(object sender, EventArgs e)
        {
            {
                var entity = EntityFactory.CreateMoFEnd(Vector2.Zero);
                document.ActAdd(entity);
                
            }
            {
                var entity = EntityFactory.CreateMoFBegin(RtcMoFTypes.Angular);
                document.ActInsert(entity, document.ActiveLayer, 0);
            }
        }
        private void MnuMofXYWait_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, 10);
            document.ActAdd(entity);
        }
        private void MnuMofXYBeginEnd_Click(object sender, EventArgs e)
        {
            {
                var entity = EntityFactory.CreateMoFEnd(Vector2.Zero);
                document.ActAdd(entity);
            }
            {
                var entity = EntityFactory.CreateMoFBegin(RtcMoFTypes.XY);
                document.ActInsert(entity, document.ActiveLayer, 0);
            }
        }
        private void MnuMoFExternalStartDelay_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateMoFExternalStartDelay( RtcEncoders.EncX, 0);
            document.ActAdd(entity);
        }
        private void MnuTimer_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateTimer(1);
            document.ActAdd(entity);
        }
        private void MnuJumpTo_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateJumpTo(Vector3.Zero);
            document.ActAdd(entity);
        }
        private void MnuMeasurementBeginEnd_Click(object sender, EventArgs e)
        {
            if (rtc is Rtc5)
            {
                var entity1 = EntityFactory.CreateMeasurementEnd();
                document.ActAdd(entity1);
                var channels = new MeasurementChannels[]
                {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.SampleZ,
                    MeasurementChannels.LaserOn,
                };
                var entity2 = EntityFactory.CreateMeasurementBegin(5 * 1000, channels);
                document.ActInsert(entity2, document.ActiveLayer, 0);
            }
            else if (rtc is Rtc6)
            {
                var entity1 = EntityFactory.CreateMeasurementEnd();
                document.ActAdd(entity1);
                var channels = new MeasurementChannels[]
                {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.SampleZ,
                    MeasurementChannels.LaserOn,
                    MeasurementChannels.OutputPeriod,
                    MeasurementChannels.PulseLength,
                    MeasurementChannels.Enc0Counter,
                    MeasurementChannels.Enc1Counter,
                };
                var entity2 = EntityFactory.CreateMeasurementBegin(5 * 1000, channels);
                document.ActInsert(entity2, document.ActiveLayer, 0);
            }
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            if (document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to new document without save ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult != DialogResult.Yes)
                    return;
            }
            document.ActNew();
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            document.ActRemove(document.Selected);
        }
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (NotifyOpenBefore())
                return;
            var dlg = new OpenFileDialog();
            dlg.Filter = Config.FileOpenFilters;
            dlg.Title = "Open File";
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            if (Document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to open without save ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult != DialogResult.Yes)
                    return;
            }
            Cursor.Current = Cursors.WaitCursor;
            document.ActOpen(dlg.FileName);
            Cursor.Current = Cursors.Default;
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (NotifySaveBefore())
                return;
            var dlg = new SaveFileDialog();
            dlg.Filter = Config.FileSaveFilters;
            dlg.Title = "Save File";
            dlg.OverwritePrompt = true;
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            Cursor.Current = Cursors.WaitCursor;
            document.ActSave(dlg.FileName);
            Cursor.Current = Cursors.Default;
        }
        private void BtnZoomIn_Click(object sender, EventArgs e)
        {

            EditorCtrl.View.Camera.ZoomIn(Point.Empty);
            DoRender();
        }
        private void BtnZoomOut_Click(object sender, EventArgs e)
        {
            EditorCtrl.View.Camera.ZoomOut(Point.Empty);
            DoRender();
        }
        private void BtnZoomFit_Click(object sender, EventArgs e)
        {
            if (0 == Document.Selected.Length)
            {
                var bbox = BoundingBox.RealBoundingBox(Document.InternalData.Layers.ToArray());
                EditorCtrl.View.Camera.ZoomFit(bbox);
            }
            else
            {
                var bbox = BoundingBox.RealBoundingBox(Document.Selected);
                EditorCtrl.View.Camera.ZoomFit(bbox);
            }
            DoRender();
        }
        private void BtnPasteArray_Click(object sender, EventArgs e)
        {
            if (Document.Clipboard == null || Document.Clipboard.Length == 0)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Clipboard are empty. Please copy or cut at first", "Warning", MessageBoxButtons.OK);
                form.ShowDialog(this);
                return;
            }
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.ArrayForm();
                if (DialogResult.OK != form.ShowDialog(this))
                    return;
                for (int i = 0; i < form.Calcuated.Length; i++)
                {
                    IEntity[] pastedEntities = Document.ActPaste(null);
                    var offset = form.Calcuated[i];
                    foreach (var entity in pastedEntities)
                    {
                        entity.Translate(offset.Dx, offset.Dy);
                    }
                }
            }
            DoRender();
        }

        private void BtnPoints_Click(object sender, EventArgs e)
        {
            Vector2[] locations = new Vector2[]
            {
                new Vector2(-1,1),
                new Vector2(-1.1f,1.2f),
                new Vector2(-1.2f,1.0f),
                new Vector2(-0.9f,-0.8f),
                new Vector2(-1,1.1f),
                new Vector2(-1.2f,0.7f),
                new Vector2(-1.4f,1.1f),
                new Vector2(-1.2f,-0.95f),
            };
            var entity = EntityFactory.CreatePoints(locations, 10);
            document.ActAdd(entity);
        }
        private void BtnPoint_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreatePoint(0, 0, 10);
            document.ActAdd(entity);
        }
        private void BtnLine_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateLine(-10, 0, 10, 0);
            document.ActAdd(entity);
        }
        private void BtnArc_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateArc(Vector2.Zero, 10, 0, 180);
            document.ActAdd(entity);
        }
        private void BtnCircle_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateArc(Vector2.Zero, 10, 0, 360);
            document.ActAdd(entity);
        }
        private void BtnSpiral_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateSpiral(Vector2.Zero, 1, 5, 0, 10, true);
            document.ActAdd(entity);
        }
        private void BtnTrepan_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateTrepan(Vector2.Zero, 1, 5, 10);
            document.ActAdd(entity);
        }
        private void BtnImportFile_Click(object sender, EventArgs e)
        {
            //var dlg = new OpenFileDialog();
            //dlg.Filter = Config.FileImportModelFilters;
            //dlg.Title = "Import Model File";
            //dlg.InitialDirectory = SpiralLab.Sirius2.Winforms.Config.SamplePath;
            //DialogResult result = dlg.ShowDialog();
            //if (result != DialogResult.OK)
            //    return;
            //Cursor.Current = Cursors.WaitCursor;
            //Document.ActImport(dlg.FileName, out var entity);
            //Cursor.Current = Cursors.Default;
            
            // or preview import winform
            var form = new SpiralLab.Sirius2.Winforms.UI.ImportForm();
            DialogResult dialogResult = form.ShowDialog(this);
            if (dialogResult != DialogResult.OK)
                return;
            if (null == form.Entity)
                return;

            // to make gl render as current
            //if (View is ViewBase vb)
            //    vb.Renderer.MakeCurrent();
            //or
            DoRender();

            Cursor.Current = Cursors.WaitCursor;
            var cloned = (IEntity)form.Entity.Clone();
            cloned.IsNeedToRegen = true;
            cloned.Parent = null;
            cloned.IsSelected = false;
            Document.ActAdd(cloned);
            Cursor.Current = Cursors.Default;
        }
        private void BtnRectangle_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateRectangle(Vector2.Zero, 10,10);
            document.ActAdd(entity);
        }
        private void BtnText_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateText(Config.FontDefault, $"Hello{Environment.NewLine}SIRIUS2", FontStyle.Regular, 2);
            Document.ActAdd(entity);
        }
        private void BtnImageText_Click(object sender, EventArgs e)
        {
            var form = new SpiralLab.Sirius2.Winforms.UI.ImageTextForm();
            if (DialogResult.OK != form.ShowDialog())
                return;
            var entity = EntityFactory.CreateImageText(form.FontName, form.ImageText, form.Style, form.IsFill, form.OutlinePixel, form.HeightPixel, 5, 10);
            Document.ActAdd(entity);
        }
        private void BtnRaster_Click(object sender, EventArgs e)
        {
            //var entity = new EntityRaster(2, 2, 100, 100);
            //Document.ActAdd(entity);
            var dlg = new OpenFileDialog();
            dlg.Filter = Config.FileImportImageFilters;
            dlg.InitialDirectory = Config.SamplePath;
            dlg.Title = "Open Image File";

            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
          
            var entity = new EntityRaster(2, dlg.FileName);
            Document.ActAdd(entity);
        }
        private void BtnDivide_Click(object sender, EventArgs e)
        {
            //if (document.Selected.Length > 0)
            //    Document.ActDivide(document.Selected, null);
        }
        private void BtnSiriusCharacterSetText_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateSiriusCharacterSetText(Config.FontDefaultSirius, CharacterSetFormats.Date, 5);
            document.ActAdd(entity);
        }
        private void BtnCharacterSetText_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateCharacterSetText(Config.FontDefault, CharacterSetFormats.Date, 5);
            document.ActAdd(entity);
        }
        private void BtnCircularText_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateCircularText(Config.FontDefault, "POWERED BY SIRIUS2 0123456789", FontStyle.Regular, 2, TextCircularDirections.ClockWise, 5, 180);
            document.ActAdd(entity);
        }
        private void BtnSiriusText_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateSiriusText(Config.FontDefaultSirius, "SIRIUS2", 2.5);
            document.ActAdd(entity);
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            Document.ActCopy();
        }
        private void BtnPaste_Click(object sender, EventArgs e)
        {
            Document.ActPaste(null);
        }
        private void BtnCut_Click(object sender, EventArgs e)
        {
            Document.ActCut();
        }
        private void BtnAbout_Click(object sender, EventArgs e)
        {
            var form = new SpiralLab.Sirius2.Winforms.UI.AboutForm();
            form.ShowDialog();
        }

        /// <summary>
        /// Event handler for <c>Document</c> has opened
        /// </summary>
        /// <param name="document"><c>IDocument</c></param>
        /// <param name="fileName">Filename</param>
        private void Document_OnOpened(IDocument document, string fileName)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;
            this.Invoke(new MethodInvoker(delegate ()
            {
                lblFileName.Text = fileName;
                if (null != Laser)
                    foreach (var pen in document.InternalData.Pens)
                        pen.PowerMax = Laser.MaxPowerWatt;
                PropertyGridCtrl.Refresh();
            }));
        }
        /// <summary>
        /// Event handler for <c>Document</c> has saved
        /// </summary>
        /// <param name="document"><c>IDocument</c></param>
        /// <param name="fileName">Filename</param>
        private void Document_OnSaved(IDocument document, string fileName)
        {
            if (!statusStrip1.IsHandleCreated || this.IsDisposed)
                return;
            statusStrip1.Invoke(new MethodInvoker(delegate ()
            {
                lblFileName.Text = fileName;
            }));
        }
        /// <summary>
        /// Event handler for <c>IEntity</c> has selected
        /// </summary>
        /// <param name="document"><c>IDocument</c></param>
        /// <param name="entities">Selected array of <c>IEntity</c></param>
        private void Document_OnSelected(IDocument document, IEntity[] entities)
        {
            if (!statusStrip1.IsHandleCreated || statusStrip1.IsDisposed)
                return;
            statusStrip1.Invoke(new MethodInvoker(delegate ()
            {
                lblSelected.Text = $"Selected: {entities.Length}";
            }));
        }

        private void Renderer_Paint(object sender, PaintEventArgs e)
        {
            DoRender();
        }

        /// <summary>
        /// Last mouse cursor location
        /// </summary>
        OpenTK.Vector3 lastCurrentPos = OpenTK.Vector3.Zero;
        private void Renderer_MouseMove(object sender, MouseEventArgs e)
        {
            var intersect = OpenTKHelper.ScreenToWorldPlaneZIntersect(e.Location, Vector3.Zero, EditorCtrl.View.Camera.ViewMatrix, EditorCtrl.View.Camera.ProjectionMatrix);
            lastCurrentPos = intersect;
            lblPos.Text = $"XY: {intersect.X:F3}, {intersect.Y:F3}mm [{e.Location.X}, {e.Location.Y}]";
        }

        private void LblHelp_Click(object sender, EventArgs e)
        {
            var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox(Config.KeyboardHelpMessage, "Help - Keyboards", MessageBoxButtons.OK);
            form.ShowDialog(this);
        }
        private void LblEncoder_DoubleClick(object sender, EventArgs e)
        {
            if (null == Rtc)
                return;
            var rtcMoF = Rtc as IRtcMoF;
            if (rtcMoF == null)
                return;
            var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to reset encoder values ?", "Warning", MessageBoxButtons.YesNo);
            DialogResult dialogResult = form.ShowDialog(this);
            if (dialogResult == DialogResult.Yes)
                rtcMoF.CtlMofEncoderReset();
        }

        int timerStatusColorCounts = 0;
        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            if (null == this.Marker)
                return;
            if (this.Marker.IsReady)
            {
                lblReady.ForeColor = Color.Black;
                lblReady.BackColor = Color.Lime;
            }
            else
            {
                lblReady.ForeColor = Color.White;
                lblReady.BackColor = Color.Green;
            }

            if (this.Marker.IsBusy)
            {
                timerStatusColorCounts = unchecked(timerStatusColorCounts + 1);
                if (0 == timerStatusColorCounts % 2)
                {
                    lblBusy.BackColor = Color.Red;
                    lblBusy.ForeColor = Color.White;
                }
                else
                {
                    lblBusy.BackColor = Color.Maroon;
                    lblBusy.ForeColor = Color.White;
                }
            }
            else
            {
                lblBusy.ForeColor = Color.White;
                lblBusy.BackColor = Color.Maroon;
                timerStatusColorCounts = 0;
            }

            if (this.Marker.IsError)
            {
                lblError.ForeColor = Color.White;
                lblError.BackColor = Color.Red;
            }
            else
            {
                lblError.ForeColor = Color.White;
                lblError.BackColor = Color.Maroon;
            }
            if (null == this.Remote || !Remote.IsConnected)
            {
                lblRemote.Text = " DISCONNECTED ";
                lblRemote.ForeColor = Color.White;
                lblRemote.BackColor = Color.Maroon;
            }
            else
            {
                lblRemote.ForeColor = Color.Black;
                switch(Remote.ControlMode)
                {
                    case ControlModes.Local:
                        lblRemote.Text = " CONNECTED /LOCAL ";
                        lblRemote.BackColor = Color.Yellow;
                        break;
                    case ControlModes.Remote:
                        lblRemote.Text = " CONNECTED /REMOTE ";
                        lblRemote.BackColor = Color.Lime;
                        break;
                }
            }

            if (null != EditorCtrl.View)
                lblRenderTime.Text = $"Render: {EditorCtrl.View.RenderTime} ms";
        }

        /// <summary>
        /// Event handler for <c>IMarker</c> has started
        /// </summary>
        /// <param name="marker"><c>IMarker</c></param>
        private void Marker_OnStarted(IMarker marker)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;
            timerProgressStopwatch.Restart();

            this.Invoke(new MethodInvoker(delegate ()
            {
                timerProgress.Enabled = true;
                VisibilityByMarking(false);
            }));
        }
        int timerProgressColorCounts = 0;
        private void TimerProgress_Tick(object sender, EventArgs e)
        {
            if (!statusStrip1.IsHandleCreated || this.IsDisposed)
                return;
            if (0 == timerProgressColorCounts++ % 2)
                lblProcessTime.ForeColor = statusStrip1.ForeColor;
            else
                lblProcessTime.ForeColor = Color.Red;

            lblProcessTime.Text = $"{timerProgressStopwatch.ElapsedMilliseconds / 1000.0:F3} sec";
        }
        /// <summary>
        /// Event handler for <c>IMarker</c> has ended
        /// </summary>
        /// <param name="marker"><c>IMarker</c></param>
        /// <param name="success">Sucess(or failed) to mark</param>
        /// <param name="ts"><c>TimeSpan</c></param>
        private void Marker_OnEnded(IMarker marker, bool success, TimeSpan ts)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;
            timerProgressStopwatch.Stop();
            this.Invoke(new MethodInvoker(delegate ()
            {
                timerProgress.Enabled = false;
                lblProcessTime.Text = $"{ts.TotalSeconds:F3} sec";
                if (success)
                    lblProcessTime.ForeColor = statusStrip1.ForeColor;
                else
                    lblProcessTime.ForeColor = Color.Red;
                VisibilityByMarking(true);
                EditorCtrl.Focus();
            }));
        }
        /// <summary>
        /// Event handler for external ENCODER values has changed
        /// </summary>
        /// <param name="rtcMoF"><c>IRtcMoF</c></param>
        /// <param name="encX">ENC X(0)</param>
        /// <param name="encY">ENC Y(1)</param>
        private void Mof_OnEncoderChanged(IRtcMoF rtcMoF, int encX, int encY)
        {
            if (!statusStrip1.IsHandleCreated || this.IsDisposed)
                return;
            switch (rtcMoF.MoFType)
            {
                default:
                case RtcMoFTypes.XY:
                    {
                        rtcMoF.CtlMofGetEncoder(out var x, out var y, out var xMm, out var yMm);
                        statusStrip1.Invoke(new MethodInvoker(delegate ()
                        {
                            lblEncoder.Text = $"ENC XY: {xMm:F3}, {yMm:F3}mm [{x}, {y}]";
                        }));
                    }
                    break;
                case RtcMoFTypes.Angular:
                    {
                        rtcMoF.CtlMofGetAngularEncoder(out var x, out var angle);
                        statusStrip1.Invoke(new MethodInvoker(delegate ()
                        {
                            lblEncoder.Text = $"ENC X,0: {angle:F3}˚ [{x}]";
                        }));
                    }
                    break;
            }
        }

        /// <summary>
        /// Event handler for powermeter data has cleared
        /// </summary>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        private void PowerMeter_OnCleared(IPowerMeter powerMeter)
        {
            if (!statusStrip1.IsHandleCreated || this.IsDisposed)
                return;
            statusStrip1.Invoke(new MethodInvoker(delegate ()
            {
                lblPowerWatt.Text = $"(Empty)";
            }));
        }
        /// <summary>
        /// Event handler for powermeter has started
        /// </summary>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        private void PowerMeter_OnStarted(IPowerMeter powerMeter)
        {
            if (!statusStrip1.IsHandleCreated || this.IsDisposed)
                return;
            statusStrip1.Invoke(new MethodInvoker(delegate ()
            {
                lblPowerWatt.Text = $"Started...";
            }));
        }
        /// <summary>
        /// Event handler for powermeter has stopped
        /// </summary>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        private void PowerMeter_OnStopped(IPowerMeter powerMeter)
        {
            if (!statusStrip1.IsHandleCreated || this.IsDisposed)
                return;
            statusStrip1.Invoke(new MethodInvoker(delegate ()
            {
                //lblPowerWatt.Text = $"0.0 W";
            }));
        }
        /// <summary>
        /// Event handler for powermeter has mesaured data
        /// </summary>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        /// <param name="dt"><c>DateTime</c></param>
        /// <param name="watt">Measured data(W)</param>
        private void PowerMeter_OnMeasured(IPowerMeter powerMeter, DateTime dt, double watt)
        {
            if (!statusStrip1.IsHandleCreated || this.IsDisposed)
                return;
            statusStrip1.Invoke(new MethodInvoker(delegate ()
            {
                lblPowerWatt.Text = $"{watt:F3} W";
            }));
        }

        /// <summary>
        /// Do <c>IView</c> render by forcily
        /// </summary>
        public void DoRender()
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                EditorCtrl.View.Render();
                lblRenderTime.Text = $"Render: {EditorCtrl.View.RenderTime} ms";
            }));
        }

        private void LblRemote_DoubleClick(object sender, EventArgs e)
        {
            if (null == this.Remote || !this.Remote.IsConnected)
                return;
            
            switch(this.Remote.ControlMode)
            {
                case ControlModes.Remote:
                    {
                        var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to switch 'LOCAL' mode ?", "Remote Control", MessageBoxButtons.YesNo);
                        DialogResult dialogResult = form.ShowDialog(this);
                        if (dialogResult == DialogResult.Yes)
                            this.Remote.ControlMode = ControlModes.Local;
                    }
                    break;
                case ControlModes.Local:
                    {
                        var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to switch 'REMOTE' mode  ?", "Remote Control", MessageBoxButtons.YesNo);
                        DialogResult dialogResult = form.ShowDialog(this);
                        if (dialogResult == DialogResult.Yes)
                            this.Remote.ControlMode = ControlModes.Remote;
                    }
                    break;
            }
        }
    }
}
