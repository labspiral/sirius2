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
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Winforms.Script;
using OpenTK;

namespace Demos
{
    /// <summary>
    /// SiriusEditorUserControl v2 (2nd edition)
    /// </summary>
    /// <remarks>
    /// User can insert(or create) usercontrol at own winforms. <br/>
    /// <img src="images/siriuseditorcontrolv2.png"/> <br/>
    /// 1. <see cref="TreeViewUserControl">TreeViewUserControl</see> <br/>
    /// 2. <see cref="TreeViewBlockUserControl">TreeViewBlockUserControl</see> <br/>
    /// 3. <see cref="PenUserControl">PenUserControl</see> <br/>
    /// 4. <see cref="EditorUserControl">EditorUserControl</see> <br/>
    /// 5. <see cref="ScannerUserControl">RtcUserControl</see> <br/>
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
    public partial class SiriusEditorUserControlV2
        //: Form
        : UserControl
    {
        /// <summary>
        /// Event for before open sirius file at <see cref="SiriusEditorUserControlV2">SiriusEditorUserControlV2</see>
        /// </summary>
        /// <remarks>
        /// User can register event handler for customized dialog-box before open sirius file. <br/>
        /// Event will be fired when user has click 'Open' menu at <see cref="SiriusEditorUserControlV2">SiriusEditorUserControlV2</see>. <br/>
        /// If user event handler is not attached, default routine has executed. <br/>
        /// If returned <c>False</c>, default routine would be executed. <br/>
        /// </remarks>
        public event Func<SiriusEditorUserControlV2, bool> OnOpenBefore;
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
            foreach (Func<SiriusEditorUserControlV2, bool> receiver in receivers)
                success &= receiver.Invoke(this);
            return success;
        }
        /// <summary>
        /// Event for save sirius file at <see cref="SiriusEditorUserControlV2">SiriusEditorUserControlV2</see>
        /// </summary>
        /// <remarks>
        /// User can register event handler for customized dialog-box before save sirius file. <br/>
        /// Event will be fired when user has click 'Save' menu at <see cref="SiriusEditorUserControlV2">SiriusEditorUserControlV2</see>. <br/>
        /// If user event handler is not attached, default routine has executed. <br/>
        /// If returned <c>False</c>, default routine would be executed. <br/>
        /// </remarks>
        public event Func<SiriusEditorUserControlV2, bool> OnSaveBefore;
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
            foreach (Func<SiriusEditorUserControlV2, bool> receiver in receivers)
                success &= receiver.Invoke(this);
            return success;
        }

        /// <summary>
        /// Title name
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Title Name")]
        [Description("Title Name of Editor")]
        public string TitleName
        {
            get { return lblName.Text; }
            set { lblName.Text = value; }
        }

        /// <summary>
        /// <see cref="IDocument">IDocument</see> (aka. Recipe data)
        /// </summary>
        /// <remarks>
        /// Core data like as entity, layer, block, pen are exist within <see cref="IDocument.InternalData">IDocument.InternalData</see>. <br/>
        /// Created <see cref="DocumentFactory.CreateDefault">DocumentFactory.CreateDefault</see> by default. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Document")]
        [Description("Document Instance")]
        public IDocument Document
        {
            get { return document; }
            set 
            {
                if (document == value)
                    return;
                if (document != null)
                {
                    PropertyGridCtrl.SelecteObject = null;
                    document.OnSaved -= Document_OnSaved;
                    document.OnOpened -= Document_OnOpened;
                }
                document = value;
                MarkerCtrl.Document = document;
                PropertyGridCtrl.Document = document;
                PenCtrl.Document = document;
                TreeViewCtrl.Document = document;
                TreeViewBlockCtrl.Document = document;
                EditorCtrl.Document = document;
                PowerMapCtrl.Document = document;
                //RtcControl
                //LaserControl
                MarkerCtrl.View = EditorCtrl.View;
                TreeViewBlockCtrl.View = EditorCtrl.View;
                PropertyGridCtrl.View = EditorCtrl.View;
                if (document != null)
                {
                    document.OnSaved += Document_OnSaved;
                    document.OnOpened += Document_OnOpened;
                    PropertyGridCtrl.SelecteObject = document.Selected;
                }
            }
        }  
        private IDocument document;

        /// <summary>
        /// <see cref="IRtc">IRtc</see>
        /// </summary>
        /// <remarks>
        /// Created by <see cref="ScannerFactory">ScannerFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Rtc")]
        [Description("RTC Instance")]
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
                RtcCtrl.Scanner = rtc;
                MarkerCtrl.Rtc = rtc;
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
        /// <see cref="ILaser">ILaser</see>
        /// </summary>
        /// <remarks>
        /// Created by <see cref="LaserFactory">LaserFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Laser")]
        [Description("Laser Instance")]
        public ILaser Laser
        {
            get { return laser; }
            set
            {
                if (laser == value)
                    return;
                laser = value;
                LaserCtrl.Laser = laser;
                MarkerCtrl.Laser = laser;
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
        /// <see cref="IMarker">IMarker</see>
        /// </summary>
        /// <remarks>
        /// Created by <see cref="MarkerFactory">MarkerFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Marker")]
        [Description("Marker Instance")]
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
                ManualCtrl.Marker = marker;
                RtcDOCtrl.Marker = marker;
                OffsetCtrl.Marker = marker;
                ScriptCtrl.Marker = marker;
                RemoteCtrl.Marker = marker;
                TreeViewCtrl.Marker = marker;
                TreeViewBlockCtrl.Marker = marker;
                EditorCtrl.Marker = marker;
                PropertyGridCtrl.Marker = marker;
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
        /// <see cref="IView">IView</see>
        /// </summary>
        /// <remarks>
        /// Created by internally. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("View")]
        [Description("View Instance")]
        public IView View
        {
            get {                 
                return EditorCtrl.View; }
        }

        /// <summary>
        /// <see cref="IRemote">IRemote</see>
        /// </summary>
        /// <remarks>
        /// Created by <see cref="RemoteFactory">RemoteFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Remote")]
        [Description("Remote Instance")]
        public IRemote Remote 
        {
            get { return remote; }
            set
            {
                if (remote == value)
                    return;
                if (remote != null)
                {
                    remote.OnModeChanged -= Remote_OnModeChanged;
                }

                remote = value;
                remoteUserControl1.Remote = remote;
                MarkerCtrl.Remote = remote;
                if (remote != null)
                {
                    remote.OnModeChanged += Remote_OnModeChanged;
                }
            }
        }
        private IRemote remote;

        /// <summary>
        /// <see cref="IPowerMeter">IPowerMeter</see>
        /// </summary>
        /// <remarks>
        /// Created by <see cref="PowerMeterFactory">PowerMeterFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("PowerMeter")]
        [Description("PowerMeter Instance")]
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
                MarkerCtrl.PowerMeter = powerMeter;
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
        /// <remarks>
        /// Created by <see cref="IOFactory">IOFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("DInput")]
        [Description("IDInput Instance for RTC Extension1 Port")]
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
        /// RTC DI LASER port (2 bits)
        /// </summary>
        /// <remarks>
        /// Created by <see cref="IOFactory">IOFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("DInput")]
        [Description("IDInput Instance for RTC LASER Port")]
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
        /// RTC DO EXTENSION1 port (16 bits)
        /// </summary>
        /// <remarks>
        /// Created by <see cref="IOFactory">IOFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("DOutput")]
        [Description("IDOutput Instance for RTC EXTENSION1 Port")]
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
        /// RTC DO EXTENSION2 port (8 bits)
        /// </summary>
        /// <remarks>
        /// Created by <see cref="IOFactory">IOFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("DOutput")]
        [Description("IDOutput Instance for RTC EXTENSION2 Port")]
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
        /// RTC DO LASER port (2 bits)
        /// </summary>
        /// <remarks>
        /// Created by <see cref="IOFactory">IOFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("DOutput")]
        [Description("IDOutput Instance for RTC LASER Port")]
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
        /// Treeview user control for <see cref="IEntity">IEntity</see> within <see cref="EntityLayer">EntityLayer</see> nodes
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("TreeViewUserControl")]
        [Description("TreeView UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.TreeViewUserControl TreeViewCtrl
        { 
            get { return trvEntity; } 
        }
        /// <summary>
        /// Treeview user control for <see cref="EntityBlock">EntityBlock</see> nodes
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("TreeViewBlockUserControl")]
        [Description("TreeViewBlock UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.TreeViewBlockUserControl TreeViewBlockCtrl
        {
            get { return trvBlock; }
        }
        /// <summary>
        /// PropertyGrid user control for properties of <see cref="IEntity">IEntity</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("PropertyGridUserControl")]
        [Description("PropertyGrid UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.PropertyGridUserControl PropertyGridCtrl
        {
            get { return propertyGridControl1; }
        }
        /// <summary>
        /// Editor(by OpenTK) user control for rendering view 
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("EditorUserControl")]
        [Description("Editor UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.EditorUserControl EditorCtrl
        {
            get { return editorControl1; }
        }
        /// <summary>
        /// User control for list of <see cref="EntityPen">EntityPen</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("PenUserControl")]
        [Description("Pen UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.PenUserControl PenCtrl
        {
            get { return penControl1; }
        }
        /// <summary>
        /// PropertyGrid user control for <see cref="ILaser">ILaser</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("LaserUserControl")]
        [Description("Laser UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.LaserUserControl LaserCtrl
        {
            get { return laserControl1; }
        }

        /// <summary>
        /// PropertyGrid user control for <see cref="IRtc">IRtc</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("RtcUserControl")]
        [Description("Rtc UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.ScannerUserControl RtcCtrl
        {
            get { return rtcControl1; }
        }
        /// <summary>
        /// PropertyGrid user control for <see cref="IMarker">IMarker</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("MarkerUserControl")]
        [Description("Marker UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.MarkerUserControl MarkerCtrl
        {
            get { return markerControl1; }
        }
        /// <summary>
        /// User control for list of <see cref="IMarker.Offsets">IMarker.Offsets</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("OffsetUserControl")]
        [Description("Offset UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.OffsetUserControl OffsetCtrl
        {
            get { return offsetControl1; }
        }
        /// <summary>
        /// User control for RTC DI (EXTENSION1 and LASER port) for <see cref="IDInput">IDInput</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("RtcDIUserControl")]
        [Description("RtcDI UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.RtcDIUserControl RtcDICtrl
        {
            get { return rtcDIUserControl1; }
        }
        /// <summary>
        /// User control for RTC DO (EXTENSION1,2 and LASER port) for <see cref="IDOutput">IDOutput</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("RtcDOUserControl")]
        [Description("RtcDO UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.RtcDOUserControl RtcDOCtrl
        {
            get { return rtcDOUserControl1; }
        }
        /// <summary>
        /// User control for manual control
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("ManualUserControl (Customized)")]
        [Description("Manual UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.ManualUserControl ManualCtrl
        {
            get { return manualUserControl1; }
        }
        /// <summary>
        /// UserControl for customized manual control
        /// </summary>
        /// <remarks>
        /// Replaced internal <c>ManualCtrl</c> as external control <br/>
        /// </remarks>
        public UserControl ManualUserCtrl
        {
            get { return manualUserCtrl; }
            set
            {
                if (manualUserCtrl == value)
                    return;
                try
                {
                    this.tabManual.SuspendLayout();
                    if (null != manualUserCtrl)
                    {
                        this.tabManual.Controls.Remove(manualUserCtrl);
                    }
                    manualUserCtrl = value;
                    if (null != manualUserCtrl)
                    {
                        this.tabManual.Controls.Clear();
                        this.tabManual.Controls.Add(manualUserCtrl);
                        manualUserCtrl.Dock = DockStyle.Fill;
                    }
                }
                finally
                {
                    this.tabManual.ResumeLayout();
                }
            }
        }
        private UserControl manualUserCtrl = null;
        /// <summary>
        /// User control for for <see cref="IPowerMeter">IPowerMeter</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("PowerMeterUserControl")]
        [Description("PowerMeter UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.PowerMeterUserControl PowerMeterCtrl
        {
            get { return powerMeterControl1; }
        }
        /// <summary>
        ///  User control for <see cref="IPowerMap">IPowerMap</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("PowerMapUserControl")]
        [Description("PowerMap UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.PowerMapUserControl PowerMapCtrl
        {
            get { return powerMapControl1; }
        }
        /// <summary>
        /// User control for <see cref="IScript">IScript</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("ScriptUserControl")]
        [Description("Script UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.ScriptUserControl ScriptCtrl
        {
            get { return scriptControlControl1; }
        }
        /// <summary>
        /// PropertyGrid user control for <see cref="IRemote">IRemote</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("RemoteUserControl (Customized)")]
        [Description("Remote UserControl")]
        public SpiralLab.Sirius2.Winforms.UI.RemoteUserControl RemoteCtrl
        {
            get { return remoteUserControl1; }
        }

        /// <summary>
        /// User control for logged messages
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("LogUserControl")]
        [Description("Log UserControl")]
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
        /// Create devices likes as <c>IRtc</c>, <c>ILaser</c>, <c>IPowerMeter</c>, ... and assign. <br/>
        /// Digital I/O devices likes as <c>DInput</c>s, <c>DInput</c>s are created when assign <c>IRtc</c> by automatically. <br/>
        /// Create <c>IMarker</c> and assign. <br/>
        /// <c>IDocument</c> is created by automatically. <br/>
        /// <c>IView</c> is created by automatically. <br/>
        /// </remarks>
        public SiriusEditorUserControlV2()
        {
            InitializeComponent();

            VisibleChanged += SiriusEditorUserControl_VisibleChanged;
            Disposed += SiriusEditorUserControl_Disposed;

            tbcLeft.SelectedIndexChanged += tbcLeft_SelectedIndexChanged;
            timerProgress.Interval = 100;
            timerProgress.Tick += TimerProgress_Tick;
            timerStatus.Interval = 100;
            timerStatus.Tick += TimerStatus_Tick;
            lblEncoder.DoubleClick += LblEncoder_DoubleClick;
            lblEncoder.DoubleClickEnabled = true;
            lblLog.DoubleClick += LblLog_DoubleClick;
            lblLog.DoubleClickEnabled = true;
    
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
            btnTriangle.Click += BtnTriangle_Click;
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
            btnZPL.Click += BtnZPL_Click;

            mnuDataMatrix.Click += MnuDataMatrix_Click;
            mnuQRCode.Click += MnuQRCode_Click;
            mnuPDF417.Click += MnuPDF417_Click;
            mnuBarcode1D.Click += MnuBarcode1D_Click;
            mnuMeasurementBeginEnd.Click += MnuMeasurementBeginEnd_Click;
            mnuTimer.Click += MnuTimer_Click;
            mnuJumpTo.Click += MnuJumpTo_Click;

            mnuMoFXYBeginEnd.Click += MnuMofXYBeginEnd_Click;
            mnuMoFXYWait.Click += MnuMofXYWait_Click;
            mnuMoFXYWaitRange.Click += MnuMoFXYWaitRange_Click;
            mnuMoFAngularBeginEnd.Click += MnuMofAngularBeginEnd_Click;
            mnuMoFAngularWait.Click += MnuMofAngularWait_Click;
            mnuMoFExternalStartDelay.Click += MnuMoFExternalStartDelay_Click;

            mnuSelectCorrectionTable.Click += MnuSelectCorrectionTable_Click;

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

            lblRemote.DoubleClickEnabled = true;
            lblRemote.DoubleClick += LblRemote_DoubleClick;

            // Create document by default 
            this.Document = DocumentFactory.CreateDefault();
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
                        var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox(
                            Properties.Resources.MarkerTryingToStart,
                            Properties.Resources.Warning,
                            MessageBoxButtons.YesNo);
                        DialogResult dialogResult = form.ShowDialog(this);
                        if (dialogResult == DialogResult.Yes)
                        {
                            Marker.Ready(Document);
                            Marker.Start();
                            return true;
                        }
                    }
                }
            }
            // Preview: F4
            else if (keyData == Config.KeyboardMarkerPreview)
            {
                if (null != Marker)
                {
                    if (!Marker.IsBusy)
                    {
                        Marker.Ready(Document);
                        Marker.Preview();
                        return true;
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
            // Simulation: F1
            else if (keyData == Config.KeyboardSimulationStart)
            {
                Document.ActSimulateStart(Document.Selected, Marker, SimulationSpeeds.Fast);
                return true;

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
        /// Switch view mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbcLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null == Document)
                return;
            Document.ActSelectClear();
            switch (tbcLeft.SelectedIndex)
            {
                case 0:
                    EditorCtrl.View.ViewMode = ViewModes.Entity;
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        // Regen whole document data by forcily (possible to be modified)
                        EditorCtrl.Document.ActRegen();
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
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
        private void BtnZPL_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("^XA");
            sb.Append("");
            sb.Append("^FX Top section with logo, name and address.");
            sb.Append("^CF0,60");
            sb.Append("^FO50,50^GB100,100,100^FS");
            sb.Append("^FO75,75^FR^GB100,100,100^FS");
            sb.Append("^FO93,93^GB40,40,40^FS");
            sb.Append("^FO220,50^FDIntershipping, Inc.^FS");
            sb.Append("^CF0,30");
            sb.Append("^FO220,115^FD1000 Shipping Lane^FS");
            sb.Append("^FO220,155^FDShelbyville TN 38102^FS");
            sb.Append("^FO220,195^FDUnited States (USA)^FS");
            sb.Append("^FO50,250^GB700,3,3^FS");
            sb.Append("");
            sb.Append("^FX Second section with recipient address and permit information.");
            sb.Append("^CFA,30");
            sb.Append("^FO50,300^FDJohn Doe^FS");
            sb.Append("^FO50,340^FD100 Main Street^FS");
            sb.Append("^FO50,380^FDSpringfield TN 39021^FS");
            sb.Append("^FO50,420^FDUnited States (USA)^FS");
            sb.Append("^CFA,15");
            sb.Append("^FO600,300^GB150,150,3^FS");
            sb.Append("^FO638,340^FDPermit^FS");
            sb.Append("^FO638,390^FD123456^FS");
            sb.Append("^FO50,500^GB700,3,3^FS");
            sb.Append("");
            sb.Append("^FX Third section with bar code.");
            sb.Append("^BY5,2,270");
            sb.Append("^FO100,550^BC^FD12345678^FS");
            sb.Append("");
            sb.Append("^FX Fourth section (the two boxes on the bottom).");
            sb.Append("^FO50,900^GB700,250,3^FS");
            sb.Append("^FO400,900^GB3,250,3^FS");
            sb.Append("^CF0,40");
            sb.Append("^FO100,960^FDCtr. X34B-1^FS");
            sb.Append("^FO100,1010^FDREF1 F00B47^FS");
            sb.Append("^FO100,1060^FDREF2 BL4H8^FS");
            sb.Append("^CF0,190");
            sb.Append("^FO470,955^FDCA^FS");
            sb.Append("");
            sb.Append("^XZ");
            var entity = EntityFactory.CreateImageZPL(sb.ToString(), 4 * 25.4, 6 * 25.4);
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
            EditorCtrl.DoRender();
        }
        private void MnuMarginTop_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignments.Top);
            EditorCtrl.DoRender();
        }
        private void MnuMarginRight_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignments.Right);
            EditorCtrl.DoRender();
        }
        private void MnuMarginLeft_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignments.Left);
            EditorCtrl.DoRender();
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
        private void MnuMoFXYWaitRange_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateMoFWaitRange(new Vector2(5, 5), new Vector2(6, 6));
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
            if (rtc is Rtc4)
            {
                var entity1 = EntityFactory.CreateMeasurementEnd();
                document.ActAdd(entity1);
                var channels = new MeasurementChannels[]
                {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                };
                var entity2 = EntityFactory.CreateMeasurementBegin(5 * 1000, channels);
                document.ActInsert(entity2, document.ActiveLayer, 0);
            }
            else
            {
                var entity1 = EntityFactory.CreateMeasurementEnd();
                document.ActAdd(entity1);
                var channels = new MeasurementChannels[]
                {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.SampleZ,
                    MeasurementChannels.LaserOn,
                    //MeasurementChannels.OutputPeriod,
                    //MeasurementChannels.PulseLength,
                    //MeasurementChannels.Enc0Counter,
                    //MeasurementChannels.Enc1Counter,
                };
                var entity2 = EntityFactory.CreateMeasurementBegin(5 * 1000, channels);
                document.ActInsert(entity2, document.ActiveLayer, 0);
            }
        }
        private void MnuSelectCorrectionTable_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateSelectCorrectionTable();
            document.ActAdd(entity);
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            if (document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox(
                    Properties.Resources.DocumentNew,
                    Properties.Resources.Warning,
                    MessageBoxButtons.YesNo);
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
            dlg.InitialDirectory = Config.RecipePath;
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            if (Document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox(
                    Properties.Resources.DocumentOpen,
                    Properties.Resources.Warning,
                    MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult != DialogResult.Yes)
                    return;
            }
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                document.ActOpen(dlg.FileName);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (NotifySaveBefore())
                return;
            var dlg = new SaveFileDialog();
            dlg.Filter = Config.FileSaveFilters;
            dlg.Title = "Save File";
            dlg.InitialDirectory = Config.RecipePath;
            dlg.OverwritePrompt = true;
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                document.ActSave(dlg.FileName);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void BtnZoomIn_Click(object sender, EventArgs e)
        {

            EditorCtrl.View.Camera.ZoomIn(Point.Empty);
            EditorCtrl.DoRender();
        }
        private void BtnZoomOut_Click(object sender, EventArgs e)
        {
            EditorCtrl.View.Camera.ZoomOut(Point.Empty);
            EditorCtrl.DoRender(); 
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
            EditorCtrl.DoRender();
        }
        private void BtnPasteArray_Click(object sender, EventArgs e)
        {
            if (Document.Clipboard == null || Document.Clipboard.Length == 0)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox(
                    Properties.Resources.DocumentClipboardEmpty,
                    Properties.Resources.Warning,
                    MessageBoxButtons.OK);
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
            EditorCtrl.DoRender(); 
        }

        private void BtnPoints_Click(object sender, EventArgs e)
        {
			// create test 500 points
			int counts = 500;
			var rand = new Random();
			var locations = new List<Vector2>(counts);
			for (int i = 0; i < counts; i++)
			{
				locations.Add(new Vector2(
						(float)(rand.NextDouble() * 10.0),
						(float)(rand.NextDouble() * 10.0)
						));
			}
			var entity = EntityFactory.CreatePoints(locations.ToArray(), 50);
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

            // or using import(or preview) winform
            var form = new SpiralLab.Sirius2.Winforms.UI.ImportForm();
            DialogResult dialogResult = form.ShowDialog(this);
            if (dialogResult != DialogResult.OK)
                return;
            if (null == form.Entity)
                return;

            EditorCtrl.DoRender();

            // clone entity at import(or preview) winform
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                var cloned = (IEntity)form.Entity.Clone();
                cloned.IsNeedToRegen = true;
                cloned.Parent = null;
                cloned.IsSelected = false;
                Document.ActAdd(cloned);
            }
            finally
            { 
                Cursor.Current = Cursors.Default;
            }
        }
        private void BtnRectangle_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateRectangle(Vector2.Zero, 10,10);
            document.ActAdd(entity);
        }
        private void BtnTriangle_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateTriangle(Vector2.Zero, 5, 7);
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
            // you can create by manually
            //var entity = new EntityRaster(2, 2, 100, 100);
            //Document.ActAdd(entity);

            // or using image file
            var dlg = new OpenFileDialog();
            dlg.Filter = Config.FileImportImageFilters;
            dlg.InitialDirectory = Config.SamplePath;
            dlg.Title = "Open Image File";

            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            var entity = new EntityRaster(dlg.FileName, 2);
            Document.ActAdd(entity);
        }
        private void BtnSiriusCharacterSetText_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateSiriusCharacterSetText(Config.SiriusFontDefault, CharacterSetFormats.Date, 5);
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
            var entity = EntityFactory.CreateSiriusText(Config.SiriusFontDefault, "SIRIUS2", 2.5);
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
            if (!stsBottom.IsHandleCreated || this.IsDisposed)
                return;
            stsBottom.Invoke(new MethodInvoker(delegate ()
            {
                lblFileName.Text = fileName;
            }));
        }

        /// <summary>
        /// Show(or hide) log screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblLog_DoubleClick(object sender, EventArgs e)
        {
            spcMain.Panel2Collapsed = !spcMain.Panel2Collapsed;
        }
        /// <summary>
        /// Encoder reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblEncoder_DoubleClick(object sender, EventArgs e)
        {
            if (null == Rtc)
                return;
            var rtcMoF = Rtc as IRtcMoF;
            if (rtcMoF == null)
                return;
            var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox(
                Properties.Resources.DocumentEncoderReset,
                Properties.Resources.Warning,
                MessageBoxButtons.YesNo);
            DialogResult dialogResult = form.ShowDialog(this);
            if (dialogResult == DialogResult.Yes)
                rtcMoF.CtlMofEncoderReset();
        }
        int timerStatusColorCounts = 0;
        /// <summary>
        /// Update status by timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    lblBusy.BackColor = Color.Orange;
                    lblBusy.ForeColor = Color.Black;
                }
                else
                {
                    lblBusy.BackColor = Color.Olive;
                    lblBusy.ForeColor = Color.White;
                }
            }
            else
            {
                lblBusy.BackColor = Color.Olive;
                lblBusy.ForeColor = Color.White;
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

            if (null != this.Remote)
            {
                if (this.Remote.IsConnected)
                {
                    lblRemote.BackColor = Color.Lime;
                    lblRemote.ForeColor = Color.Black;
                }
                else
                {
                    lblRemote.BackColor = Color.Green;
                    lblRemote.ForeColor = Color.White;
                }
                switch (Remote.ControlMode)
                {
                    case ControlModes.Local:
                        lblRemote.Text = $"{Properties.Resources.Local}";
                        break;
                    case ControlModes.Remote:
                        lblRemote.Text = $"{Properties.Resources.Remote}";
                        break;
                }
            }  

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
                EditabilityByMarkerBusy(false);
            }));
        }
        int timerProgressColorCounts = 0;
        /// <summary>
        /// Update marker progressing by timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerProgress_Tick(object sender, EventArgs e)
        {
            if (!stsBottom.IsHandleCreated || this.IsDisposed)
                return;
            if (0 == timerProgressColorCounts++ % 2)
                lblProcessTime.ForeColor = stsBottom.ForeColor;
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
                    lblProcessTime.ForeColor = stsBottom.ForeColor;
                else
                    lblProcessTime.ForeColor = Color.Red;
                EditabilityByMarkerBusy(true);
                EditorCtrl.Focus();
            }));
        }
        /// <summary>
        /// Event handler for <c>IRemote</c> has ended
        /// </summary>
        /// <param name="remote"><c>IRemote</c></param>
        /// <param name="mode"><c>ControlModes</c></param>
        private void Remote_OnModeChanged(IRemote remote, ControlModes mode)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                switch (mode)
                {
                    case ControlModes.Local:
                        VisibilityByRemoteControlMode(true);
                        break;
                    case ControlModes.Remote:
                        VisibilityByRemoteControlMode(false);
                        break;
                }
            }));
        }
        /// <summary>
        /// Switch control mode (local and remote)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblRemote_DoubleClick(object sender, EventArgs e)
        {
            if (null == this.Remote)
                return;
            switch (this.Remote.ControlMode)
            {
                case ControlModes.Remote:
                    {
                        var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox(
                            Properties.Resources.RemoteToLocal,
                            Properties.Resources.RemoteControl,
                            MessageBoxButtons.YesNo);
                        DialogResult dialogResult = form.ShowDialog(this);
                        if (dialogResult == DialogResult.Yes)
                            this.Remote.ControlMode = ControlModes.Local;
                    }
                    break;
                case ControlModes.Local:
                    {
                        var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox(
                            Properties.Resources.RemoteToRemote,
                            Properties.Resources.RemoteControl,
                            MessageBoxButtons.YesNo);
                        DialogResult dialogResult = form.ShowDialog(this);
                        if (dialogResult == DialogResult.Yes)
                            this.Remote.ControlMode = ControlModes.Remote;
                    }
                    break;
            }
        }
        /// <summary>
        /// Event handler for external ENCODER values has changed
        /// </summary>
        /// <param name="rtcMoF"><c>IRtcMoF</c></param>
        /// <param name="encX">ENC X(0)</param>
        /// <param name="encY">ENC Y(1)</param>
        private void Mof_OnEncoderChanged(IRtcMoF rtcMoF, int encX, int encY)
        {
            if (!stsBottom.IsHandleCreated || this.IsDisposed)
                return;
            try
            {
                switch (rtcMoF.MoFType)
                {
                    default:
                    case RtcMoFTypes.XY:
                        {
                            rtcMoF.CtlMofGetEncoder(out var x, out var y, out var xMm, out var yMm);
                            stsBottom.BeginInvoke(new MethodInvoker(delegate ()
                            {
                                lblEncoder.Text = string.Format(Properties.Resources.EncoderXY, xMm, yMm, x, y);
                            }));
                        }
                        break;
                    case RtcMoFTypes.Angular:
                        {
                            rtcMoF.CtlMofGetAngularEncoder(out var x, out var angle);
                            stsBottom.BeginInvoke(new MethodInvoker(delegate ()
                            {
                                lblEncoder.Text = string.Format(Properties.Resources.EncoderAngular, angle, x);
                            }));
                        }
                        break;
                }
            }
            catch (Exception)
            { }
        }
        /// <summary>
        /// Event handler for powermeter data has cleared
        /// </summary>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        private void PowerMeter_OnCleared(IPowerMeter powerMeter)
        {
            if (!stsBottom.IsHandleCreated || this.IsDisposed)
                return;
            stsBottom.Invoke(new MethodInvoker(delegate ()
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
            if (!stsBottom.IsHandleCreated || this.IsDisposed)
                return;
            stsBottom.Invoke(new MethodInvoker(delegate ()
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
        }
        /// <summary>
        /// Event handler for powermeter has mesaured data
        /// </summary>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        /// <param name="dt"><c>DateTime</c></param>
        /// <param name="watt">Measured data(W)</param>
        private void PowerMeter_OnMeasured(IPowerMeter powerMeter, DateTime dt, double watt)
        {
            if (!stsBottom.IsHandleCreated || this.IsDisposed)
                return;
            try
            {
                stsBottom.BeginInvoke(new MethodInvoker(delegate ()
                {
                    lblPowerWatt.Text = $"{watt:F3} W";
                }));
            }
            catch (Exception)
            { }
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
                case RtcTypes.Rtc4e:
                    btnCharacterSetText.Enabled = false;
                    btnSiriusCharacterSetText.Enabled = false;
                    mnuAlcDefinedVector.Enabled = false;

                    mnuWriteDataExt16Cond.Enabled = false;
                    mnuWaitDataExt16Cond.Enabled = false;
                    mnuWaitDataExt16EdgeCond.Enabled = false;

                    mnuMoFXYWait.Enabled = false;
                    mnuMoFXYWaitRange.Enabled = false;
                    mnuMoFAngularWait.Enabled = false;
                    break;
                case RtcTypes.Rtc5:
                case RtcTypes.Rtc6:
                case RtcTypes.Rtc6e:
                    break;
                case RtcTypes.Rtc6SyncAxis:
                    mnuMeasurementBeginEnd.Enabled = false;
                    mnuMoF.Enabled = false;
                    mnuZDelta.Enabled = false;
                    mnuZDefocus.Enabled = false;
                    mnuWriteDataExt16Cond.Enabled = false;
                    mnuWaitDataExt16Cond.Enabled = false;
                    mnuWaitDataExt16EdgeCond.Enabled = false;
                    mnuSelectCorrectionTable.Enabled = false;

                    lblEncoder.Visible = false;
                    btnCharacterSetText.Enabled = false;
                    btnSiriusCharacterSetText.Enabled = false;
                    break;
            }
        }
        /// <summary>
        /// Entity Property visibility
        /// </summary>
        private void PropertyVisibility()
        {
            EntityPen.PropertyVisibility(rtc);
            EntityPen.PropertyVisibility(laser);
            EntityLayer.PropertyVisibility(rtc);
            EntityPoints.PropertyVisibility(rtc);
            EntityPoint.PropertyVisibility(rtc);
            EntityLine.PropertyVisibility(rtc);
            EntityArc.PropertyVisibility(rtc);
            EntityRampBegin.PropertyVisibility(rtc);
            EntityMoFBegin.PropertyVisibility(rtc);
        }
        /// <summary>
        /// Enable(or disable) controls during <see cref="IMarker">IMarker</see> status is busy
        /// </summary>
        /// <param name="isEnable">Enable(or disable) controls</param>
        /// <remarks>
        /// To disable edit operations during marker is busy status. <br/>
        /// </remarks>
        public virtual void EditabilityByMarkerBusy(bool isEnable)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;
            this.Invoke(new MethodInvoker(delegate ()
            {
                if (null != Remote)
                {
                    switch (Remote.ControlMode)
                    {
                        case ControlModes.Local:
                            View.IsEditMode = isEnable;
                            break;
                        case ControlModes.Remote:
                            break;
                    }
                }
                else
                    View.IsEditMode = isEnable;

                tlsTop1.Enabled = isEnable;
                tlsTop2.Enabled = isEnable;
                tbcLeft.SelectedIndex = 0;
                tbcLeft.Enabled = isEnable;
                //EditorCtrl.Enabled = isEnable;
#if DEBUG
                // let them enabled for debugging purpose
#else
                OffsetCtrl.Enabled = isEnable;
                ManualCtrl.Enabled = isEnable;
                RtcCtrl.Enabled = isEnable;
                LaserCtrl.Enabled = isEnable;
#endif
            }));
        }
        /// <summary>
        /// Hide UI controls if remote control mode
        /// </summary>
        /// <remarks>
        /// To do visible(or invisible) controls (like as treeview, propertygrid,...) to prevent edit operations at locally. <br/>
        /// Applied whenever <see cref="IRemote.ControlMode">IRemote.ControlModes</see> has changed. <br/>
        /// </remarks>
        /// <param name="isLocalMode">Local(visible) or Remote(invisible)</param>
        public virtual void VisibilityByRemoteControlMode(bool isLocalMode = true)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;

            this.SuspendLayout();
            if (isLocalMode)
            {
                if (null != Marker && null != View)
                    if (!Marker.IsBusy)
                        View.IsEditMode = true;
                spcRight.Visible = true;
                tlsTop2.Enabled = true;
                btnNew.Enabled = true;
                btnSave.Enabled = true;
                btnCopy.Enabled = true;
                btnCut.Enabled = true;
                btnPaste.Enabled = true;
                btnPasteArray.Enabled = true;
                btnDelete.Enabled = true;
                ddbAlign.Enabled = true;
            }
            else
            {
                if (null != View)
                    View.IsEditMode = false;
                spcRight.Visible = false;
                tlsTop2.Enabled = false;
                btnNew.Enabled = false;
                btnSave.Enabled = false;
                btnCopy.Enabled = false;
                btnCut.Enabled = false;
                btnPaste.Enabled = false;
                btnPasteArray.Enabled = false;
                btnDelete.Enabled = false;
                ddbAlign.Enabled = false;
            }
            this.ResumeLayout();
        }
    }
}
