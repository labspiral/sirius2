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
 * Description : Customizable SiriusEditorForm winforms
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.IO;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using OpenTK;

namespace Demos
{
    /// <summary>
    /// SiriusEditorForm (for Customization)
    /// </summary>
    public partial class SiriusEditorForm : Form
    {
        /// <summary>
        /// Title name
        /// </summary>
        public string TitleName
        {
            get { return lblName.Text; }
            set { lblName.Text = value; }
        }

        /// <summary>
        /// <c>IDocument</c>
        /// </summary>
        public IDocument Document
        {
            get { return document; }
            protected set 
            {
                if (document != null)
                {
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
                EditorCtrl.Document = document;

                if (document != null)
                {
                    document.OnSaved += Document_OnSaved;
                    document.OnOpened += Document_OnOpened;
                    if (EditorCtrl.View is ViewBase vb)
                    {
                        vb.Renderer.MouseMove += Renderer_MouseMove;
                        vb.Renderer.Paint += Renderer_Paint;
                    }
                }
                PropertyGridCtrl.SelecteObject = null;
            }
        }  
        private IDocument document;

        /// <summary>
        /// IRtc
        /// </summary>
        public IRtc Rtc
        {
            get { return rtc; }
            set
            {
                if (rtc != null)
                {
                    if (rtc is IRtcMoF mof)
                    {
                        mof.OnEncoderChanged -= Mof_OnEncoderChanged;
                    }
                    myDIExt1.Dispose();
                    myDILaserPort.Dispose();
                    myDOExt1.Dispose();
                    myDOExt2.Dispose();
                    myDOLaserPort.Dispose();

                    myDIExt1 = null;
                    myDILaserPort = null;
                    myDOExt1 = null;
                    myDOExt2 = null;
                    myDOLaserPort = null;
                }
                
                rtc = value;
                rtcControl1.Rtc = rtc;
                rtcDIUserControl1.Rtc = rtc;
                rtcDOUserControl1.Rtc = rtc;
                EditorCtrl.Rtc = rtc;
                TreeViewCtrl.Rtc = rtc;

                if (rtc != null)
                {
                    // RTC extension DIO
                    myDIExt1 = IOFactory.CreateInputExtension1(rtc);
                    myDILaserPort = IOFactory.CreateInputLaserPort(rtc);
                    myDOExt1 = IOFactory.CreateOutputExtension1(rtc);
                    myDOExt2 = IOFactory.CreateOutputExtension2(rtc);
                    myDOLaserPort = IOFactory.CreateOutputLaserPort(rtc);

                    myDIExt1.Initialize();
                    myDILaserPort.Initialize();
                    myDOExt1.Initialize();
                    myDOExt2.Initialize();
                    myDOLaserPort.Initialize();

                    rtcDIUserControl1.DIExt1 = myDIExt1;
                    rtcDIUserControl1.DILaserPort = myDILaserPort;
                    rtcDIUserControl1.UpdateExtension1PortNames(Config.DIN_RtcExtension1Port);
                    rtcDIUserControl1.UpdateLaserPortNames(Config.DIN_RtcLaserPort);
                    
                    rtcDOUserControl1.DOExt1 = myDOExt1;
                    rtcDOUserControl1.DOExt2 = myDOExt2;
                    rtcDOUserControl1.DOLaserPort = myDOLaserPort;
                    rtcDOUserControl1.UpdateExtension1PortNames(Config.DOut_RtcExtension1Port);
                    rtcDOUserControl1.UpdateExtension2PortNames(Config.DOut_RtcExtension2Port);
                    rtcDOUserControl1.UpdateLaserPortNames(Config.DOut_RtcLaserPort);

                    EntityPen.PropertyVisibility(rtc);
                    EntityLayer.PropertyVisibility(rtc);
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
        public ILaser Laser
        {
            get { return laser; }
            set
            {
                
                laser = value;
                laserControl1.Laser = laser;
                if (null != laser)
                {
                    EntityPen.PropertyVisibility(laser);
                }
            }
        }
        private ILaser laser;

        /// <summary>
        /// <c>IMarker</c>
        /// </summary>
        public IMarker Marker
        {
            get { return marker; }
            set
            {
                if (marker != null)
                {
                    marker.OnStarted -= Marker_OnStarted;
                    marker.OnFinished -= Marker_OnFinished;
                    marker.OnFailed -= Marker_OnFailed;
                }

                marker = value;
                markerControl1.Marker = marker;
                offsetControl1.Marker = marker;

                if (marker != null)
                {
                    marker.OnStarted += Marker_OnStarted;
                    marker.OnFinished += Marker_OnFinished;
                    marker.OnFailed += Marker_OnFailed;
                }
            }
        }
        private IMarker marker;

        /// <summary>
        /// <c>IView</c>
        /// </summary>
        public IView View
        {
            get { return EditorCtrl.View; }
        }

        IDInput myDIExt1;
        IDInput myDILaserPort;
        IDOutput myDOExt1;
        IDOutput myDOExt2;
        IDOutput myDOLaserPort;

        internal TreeViewUserControl TreeViewCtrl
        { 
            get { return treeViewControl1; } 
        }
        internal PropertyGridUserControl PropertyGridCtrl
        {
            get { return propertyGridControl1; }
        }
        internal EditorUserControl EditorCtrl
        {
            get { return editorControl1; }
        }
        internal PenUserControl PenCtrl
        {
            get { return penControl1; }
        }
        internal LaserUserControl LaserCtrl
        {
            get { return laserControl1; }
        }
        internal RtcUserControl RtcCtrl
        {
            get { return rtcControl1; }
        }
        internal MarkerUserControl MarkerCtrl
        {
            get { return markerControl1; }
        }
        internal OffsetUserControl OffsetCtrl
        {
            get { return offsetControl1; }
        }
        internal RtcDIUserControl RtcDICtrl
        {
            get { return rtcDIUserControl1; }
        }
        internal RtcDOUserControl RtcDOCtrl
        {
            get { return rtcDOUserControl1; }
        }
        internal LogUserControl LogCtrl
        {
            get { return logControl1; }
        }

        System.Windows.Forms.Timer timerProgress = new System.Windows.Forms.Timer();
        Stopwatch timerProgressStopwatch = new Stopwatch();
        System.Windows.Forms.Timer timerStatus  = new System.Windows.Forms.Timer();

        /// <summary>
        /// Constructor
        /// </summary>
        public SiriusEditorForm()
        {
            InitializeComponent();

            // Create document by default
            this.Document = new DocumentBase();

            this.Load += SiriusEditorUserControl_Load;
            this.Disposed += SiriusEditorUserControl_Disposed;

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
            btnSave.Click += BtnSave_Click;
            btnCopy.Click += BtnCopy_Click;
            btnCut.Click += BtnCut_Click;
            btnPaste.Click += BtnPaste_Click;
            btnPasteArray.Click += BtnPasteArray_Click;
            btnZoomOut.Click += BtnZoomOut_Click;
            btnZoomIn.Click += BtnZoomIn_Click;
            btnDelete.Click += BtnDelete_Click;
            btnDivide.Click += BtnDivide_Click;

            btnLine.Click += BtnLine_Click;
            btnPoint.Click += BtnPoint_Click;
            btnPoints.Click += BtnPoints_Click;
            btnRectangle.Click += BtnRectangle_Click;
            btnCircle.Click += BtnCircle_Click;
            btnSpiral.Click += BtnSpiral_Click;
            btnImage.Click += BtnImage_Click;
            btnImageText.Click += BtnImageText_Click;
            btnImportFile.Click += BtnImportFile_Click;
            mnuMeasurementBeginEnd.Click += BtnMeasurementBeginEnd_Click;
            mnuTimer.Click += BtnTimer_Click;
            mnuMofXYBeginEnd.Click += MnuMofXYBeginEnd_Click;
            mnuMofXYWait.Click += MnuMofXYWait_Click;
            mnuMofAngularBeginEnd.Click += MnuMofAngularBeginEnd_Click;
            mnuMofAngularWait.Click += MnuMofAngularWait_Click;
            mnuZDefocus.Click += MnuZDefocus_Click;
            mnuMarginLeft.Click += MnuMarginLeft_Click;
            mnuMarginRight.Click += MnuMarginRight_Click;
            mnuMarginTop.Click += MnuMarginTop_Click;
            mnuMarginBottom.Click += MnuMarginBottom_Click;

            TreeViewCtrl.View = EditorCtrl.View;
            PropertyGridCtrl.View = EditorCtrl.View;
        }

        /// <summary>
        /// Global process short cut keys for F5, CTRL+F5, F6
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F5))
            {
                MarkerCtrl.BtnStart_Click(this, EventArgs.Empty);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.F5))
            {
                MarkerCtrl.BtnStop_Click(this, EventArgs.Empty);
                return true;
            }
            else if (keyData == (Keys.F6))
            {
                MarkerCtrl.BtnReset_Click(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MnuMarginBottom_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignment.Bottom);
            DoRender();
        }

        private void MnuMarginTop_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignment.Top);
            DoRender();
        }

        private void MnuMarginRight_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignment.Right);
            DoRender();
        }

        private void MnuMarginLeft_Click(object sender, EventArgs e)
        {
            document.ActAlign(document.Selected, MarginAlignment.Left);
            DoRender();
        }

        private void MnuZDefocus_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateZDefocus(0);
            document.ActAdd(entity);
        }

        private void LblEncoder_DoubleClick(object sender, EventArgs e)
        {
            if (null == Rtc)
                return;
            var rtcMoF = Rtc as IRtcMoF;
            if (rtcMoF == null)
                return;

            var form = new SpiralLab.Sirius2.Winforms.MessageBox($"Do you really want to reset encoder values ?", "Warning", MessageBoxButtons.YesNo);
            DialogResult dialogResult = form.ShowDialog(this);
            if (dialogResult != DialogResult.Yes)
                return;

            rtcMoF.CtlMofEncoderReset();
        }

        private void MnuMofAngularWait_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateMoFWait(EncoderWaitCondition.Over, 90);
            document.ActAdd(entity);
        }

        private void MnuMofAngularBeginEnd_Click(object sender, EventArgs e)
        {
            {
                var entity = EntityFactory.CreateMoFEnd(Vector2.Zero);
                document.ActAdd(entity);
            }
            {
                var entity = EntityFactory.CreateMoFBegin(MoFEncoderType.Angular);
                document.ActInsert(entity, document.ActiveLayer, 0);
            }
        }

        private void MnuMofXYWait_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateMoFWait(RtcEncoder.EncX, EncoderWaitCondition.Over, 10);
            document.ActAdd(entity);
        }

        private void MnuMofXYBeginEnd_Click(object sender, EventArgs e)
        {
            {
                var entity = EntityFactory.CreateMoFEnd(Vector2.Zero);
                document.ActAdd(entity);
            }
            {
                var entity = EntityFactory.CreateMoFBegin(MoFEncoderType.XY);
                document.ActInsert(entity, document.ActiveLayer, 0);
            }
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

        private void SiriusEditorUserControl_Load(object sender, EventArgs e)
        {

            Document.ActNew();
            timerStatus.Enabled = true;
        }

        private void BtnImportFile_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = Config.FileImportModelFilters;
            dlg.Title = "Import Model File";
            dlg.InitialDirectory = SpiralLab.Sirius2.Winforms.Config.SamplePath;
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            Cursor.Current = Cursors.WaitCursor;
            Document.ActImport(dlg.FileName, out var entity);
            Cursor.Current = Cursors.Default;
        }

        private void BtnRectangle_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateRectangle(Vector2.Zero, 10,10);
            document.ActAdd(entity);
        }

        private void BtnImage_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = Config.FileImportImageFilters;
            dlg.Title = "Import Image File";
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            Cursor.Current = Cursors.WaitCursor;
            var entity = EntityFactory.CreateImage(dlg.FileName, 10);
            Document.ActAdd(entity);
            Cursor.Current = Cursors.Default;
        }

        private void BtnTimer_Click(object sender, EventArgs e)
        {
            var entity = EntityFactory.CreateTimer(1);
            document.ActAdd(entity);
        }

        private void BtnImageText_Click(object sender, EventArgs e)
        {
            var form = new ImageTextForm();
            if (DialogResult.OK != form.ShowDialog())
                return;
            var entity = EntityFactory.CreateImageText(form.FontName, form.ImageText, form.Style, form.IsFill, form.OutlineSize, form.PixelSize, 2);
            Document.ActAdd(entity);
        }

        private void BtnDivide_Click(object sender, EventArgs e)
        {
            if (document.Selected.Length > 0)
                Document.ActDivide(document.Selected);
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

        private void BtnPasteArray_Click(object sender, EventArgs e)
        {
            if (Document.Clipboard == null || Document.Clipboard.Length == 0)
            {
                var form = new SpiralLab.Sirius2.Winforms.MessageBox($"Clipboard are empty. Please copy or cut at first", "Warning", MessageBoxButtons.OK);
                form.ShowDialog(this);
                return;
            }
            {
                var form = new ArrayForm();
                if (DialogResult.OK != form.ShowDialog(this))
                    return;
                foreach (var o in form.Calcuated)
                {
                    IEntity[] pastedEntities = Document.ActPaste(null);
                    foreach (var entity in pastedEntities)
                    {
                        entity.Translate(o.Dx, o.Dy);
                    }
                }
            }
            DoRender();
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
            var form = new AboutForm();
            form.ShowDialog();
        }

        private void SiriusEditorUserControl_Disposed(object sender, EventArgs e)
        {
            timerStatus.Tick -= TimerStatus_Tick;
            timerProgress.Tick -= TimerProgress_Tick;
        }

        private void Document_OnOpened(IDocument document, string fileName)
        {
            lblFileName.Text = fileName;
        }
        private void Document_OnSaved(IDocument document, string fileName)
        {
            lblFileName.Text = fileName;
        }

        private void Renderer_Paint(object sender, PaintEventArgs e)
        {
            DoRender();
        }
        private void Renderer_MouseMove(object sender, MouseEventArgs e)
        {
            var intersect = OpenTKHelper.ScreenToWorldPlaneZIntersect(e.Location, Vector3.Zero, EditorCtrl.View.Camera.ViewMatrix, EditorCtrl.View.Camera.ProjectionMatrix);
            lblPos.Text = $"XY: {intersect.X:F3}, {intersect.Y:F3}, P: {e.Location.X}, {e.Location.Y}";

        }
        private void Mof_OnEncoderChanged(IRtcMoF rtcMoF, int encX, int encY)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                switch (rtcMoF.EncoderType)
                {
                    default:
                    case MoFEncoderType.XY:
                        {
                            rtcMoF.CtlMofGetEncoder(out var x, out var y, out var xMm, out var yMm);
                            lblEncoder.Text = $"ENC XY: {x}, {y} [{xMm:F3}, {yMm:F3}]";
                        }
                        break;
                    case MoFEncoderType.Angular:
                        { 
                            rtcMoF.CtlMofGetAngularEncoder(out var x, out var angle);
                            lblEncoder.Text = $"ENC X,0: {x} [{angle:F3}˚]";
                        }
                        break;
                }
            }));
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            if (document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.MessageBox($"Do you really want to new file without save ?", "Warning", MessageBoxButtons.YesNo);
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
            var dlg = new OpenFileDialog();
            dlg.Filter = Config.FileOpenFilters;
            dlg.Title = "Open File";
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            if (Document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.MessageBox($"Do you really want to open new file without save ?", "Warning", MessageBoxButtons.YesNo);
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

        private void BtnMeasurementBeginEnd_Click(object sender, EventArgs e)
        {
            {
                var entity = EntityFactory.CreateMeasurementEnd();
                document.ActAdd(entity);
            }
            {
                var entity = EntityFactory.CreateMeasurementBegin(5 * 1000);
                document.ActInsert(entity, document.ActiveLayer, 0);
            }
        }
        private void LblHelp_Click(object sender, EventArgs e)
        {
            var form = new SpiralLab.Sirius2.Winforms.MessageBox(Config.KeyboardHelpMessage, "Help - Keyboards", MessageBoxButtons.OK);
            DialogResult dialogResult = form.ShowDialog(this);
        }

        private void EnableDisableControlByMarking(bool enable)
        {
            // block (or disable) controls when marking (busy)
            TreeViewCtrl.Enabled = enable;
            EditorCtrl.Enabled = enable;
            PenCtrl.Enabled = enable;
            LaserCtrl.Enabled = enable;
            RtcCtrl.Enabled = enable;
            OffsetCtrl.Enabled = enable;
            PropertyGridCtrl.Enabled = enable;

            DoRender();
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
                if (0 == timerStatusColorCounts++ % 2)
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
            if (null != EditorCtrl.View)
                lblRenderTime.Text = $"Render: {EditorCtrl.View.RenderTime} ms";
        }

        int timerProgressColorCounts = 0;
        private void TimerProgress_Tick(object sender, EventArgs e)
        {
            if (0 == timerProgressColorCounts++ % 2)
                lblProcessTime.ForeColor = statusStrip1.ForeColor;
            else
                lblProcessTime.ForeColor = Color.Red;

            lblProcessTime.Text = $"Marking: {timerProgressStopwatch.ElapsedMilliseconds / 1000.0:F1} sec";
        }
        private void Marker_OnStarted(IMarker marker)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                EnableDisableControlByMarking(false);
                timerProgressStopwatch.Restart();
                timerProgress.Start();
            }));
        }
        private void Marker_OnFinished(IMarker marker, TimeSpan ts)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                timerProgressStopwatch.Stop();
                timerProgress.Stop();
                lblProcessTime.ForeColor = statusStrip1.ForeColor;
                lblProcessTime.Text = $"Marked: {ts.TotalSeconds:F1} sec";
                EnableDisableControlByMarking(true);
            }));

        }
        private void Marker_OnFailed(IMarker marker, TimeSpan ts)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                timerProgressStopwatch.Stop();
                timerProgress.Stop();
                lblProcessTime.ForeColor = Color.Red;
                lblProcessTime.Text = $"Failed: {ts.TotalSeconds:F1} sec";
                EnableDisableControlByMarking(true);
            }));
        }

        void DoRender()
        {
            EditorCtrl.View.Render();
            lblRenderTime.Text = $"Render: {EditorCtrl.View.RenderTime} ms";
        }
    }
}
