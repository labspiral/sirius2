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
 * 2025 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : SiriusVisionControl usercontrol
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Vision.Camera;
using SpiralLab.Sirius2.Vision.Common;
using SpiralLab.Sirius2.Vision.Inspector;
using SpiralLab.Sirius2.Vision.Process;
using SpiralLab.Sirius2.Vision.Script;

namespace Demos
{
    /// <summary>
    /// SiriusVisionControl
    /// </summary>
    /// <remarks>
    /// User can insert(or create) usercontrol at own winforms. <br/>
    /// <img src="images/siriusvisioncontrol.png"/> <br/>
    /// </remarks>
    public partial class SiriusVisionControl
        : UserControl
    {
        /// <summary>
        /// Event for before open sirius file at <see cref="SiriusVisionControl">SiriusVisionControl</see>
        /// </summary>
        /// <remarks>
        /// User can register event handler for customized dialog-box before open vision file. <br/>
        /// Event will be fired when user has click 'Open' menu at <see cref="SiriusVisionControl">SiriusVisionControl</see>. <br/>
        /// If user event handler is not attached, default routine has executed. <br/>
        /// If returned <c>False</c>, default routine would be executed. <br/>
        /// </remarks>
        public event Func<SiriusVisionControl, bool> OnOpenBefore;
        /// <summary>
        /// Notify before open vision file
        /// </summary>
        /// <returns>Success or failed</returns>
        bool NotifyOpenBefore()
        {
            var receivers = OnOpenBefore?.GetInvocationList();
            if (null == receivers)
                return false;
            bool success = true;
            foreach (Func<SiriusVisionControl, bool> receiver in receivers)
                success &= receiver.Invoke(this);
            return success;
        }
        /// <summary>
        /// Event for save sirius file at <see cref="SiriusVisionControl">SiriusVisionControl</see>
        /// </summary>
        /// <remarks>
        /// User can register event handler for customized dialog-box before save vision file. <br/>
        /// Event will be fired when user has click 'Save' menu at <see cref="SiriusVisionControl">SiriusVisionControl</see>. <br/>
        /// If user event handler is not attached, default routine has executed. <br/>
        /// If returned <c>False</c>, default routine would be executed. <br/>
        /// </remarks>
        public event Func<SiriusVisionControl, bool> OnSaveBefore;
        /// <summary>
        /// Notify save vision file
        /// </summary>
        /// <returns>Success or failed</returns>
        bool NotifySaveBefore()
        {
            var receivers = OnSaveBefore?.GetInvocationList();
            if (null == receivers)
                return false;
            bool success = true;
            foreach (Func<SiriusVisionControl, bool> receiver in receivers)
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
        [Description("Title Name of Display")]
        public string TitleName
        {
            get { return lblName.Text; }
            set { lblName.Text = value; }
        }
        /// <summary>
        /// <see cref="SpiralLab.Sirius2.Vision.IDocument">IDocument</see> (aka. Recipe data)
        /// </summary>
        /// <remarks>
        /// Core data like as process, layer are exist within <see cref="IDocument.InternalData">IDocument.InternalData</see>. <br/>
        /// Created <see cref="DocumentFactory.CreateDefault">DocumentFactory.CreateDefault</see> by default. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Document")]
        [Description("Document Instance")]
        public SpiralLab.Sirius2.Vision.IDocument Document
        {
            get { return document; }
            set
            {
                if (document != null)
                {
                    propertyGridUserControl1.SelecteObjects = null;
                    document.OnSaved -= Document_OnSaved;
                    document.OnOpened -= Document_OnOpened;
                }
                document = value;

                TreeViewCtrl.Document = document;
                PropertyGridCtrl.Document = document;
                CameraCtrl.Document = document;
                DisplayCtrl.Document = document;
                //InspectorCtrl.Document = document;
                if (document != null)
                {
                    document.OnSaved += Document_OnSaved;
                    document.OnOpened += Document_OnOpened;
                    PropertyGridCtrl.SelecteObjects = document.Selected;
                }
            }
        }
        SpiralLab.Sirius2.Vision.IDocument document;
        /// <summary>
        /// <see cref="SpiralLab.Sirius2.Vision.Camera.ICamera">ICamera</see>
        /// </summary>
        /// <remarks>
        /// Created by <see cref="CameraFactory">CameraFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Camera")]
        [Description("Camera Instance")]
        public SpiralLab.Sirius2.Vision.Camera.ICamera Camera
        {
            get { return camera; }
            set
            {
                if (camera != null)
                {
                }
                if (null != cameraStitched)
                {
                    cameraStitched.OnStitchedImageGrabStarted -= CameraStitched_OnStitchedImageGrabStarted;
                    cameraStitched.OnStitchedImageGrabEnded -= CameraStitched_OnStitchedImageGrabEnded;
                }

                cameraStitched = null;
                camera = value;

                DisplayCtrl.Camera = camera;
                CameraCtrl.Camera = camera;
                InspectorCtrl.Camera = camera;
                TreeViewCtrl.Camera = camera;
                CameraCtrl.DisplayCtrl = DisplayCtrl;
                TreeViewCtrl.DisplayCtrl = DisplayCtrl;
                if (camera != null)
                {
                    cameraStitched = camera as ICameraStitched;

                }

                if (null != cameraStitched)
                {
                    cameraStitched.OnStitchedImageGrabStarted += CameraStitched_OnStitchedImageGrabStarted;
                    cameraStitched.OnStitchedImageGrabEnded += CameraStitched_OnStitchedImageGrabEnded;
                }
            }
        }
        SpiralLab.Sirius2.Vision.Camera.ICamera camera;
        SpiralLab.Sirius2.Vision.Camera.ICameraStitched cameraStitched = null;
        /// <summary>
        /// <see cref="SpiralLab.Sirius2.Scanner.Rtc.IRtc">IRtc</see>
        /// </summary>
        /// <remarks>
        /// Created by <see cref="SpiralLab.Sirius2.Scanner.ScannerFactory">ScannerFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Rtc")]
        [Description("Rtc Instance")]
        public SpiralLab.Sirius2.Scanner.Rtc.IRtc Rtc
        {
            get { return rtc; }
            set
            {
                if (rtc != null)
                {
                }
                rtc = value;
                if (rtc != null)
                {
                }
            }
        }
        SpiralLab.Sirius2.Scanner.Rtc.IRtc rtc;
        /// <summary>
        /// <see cref="SpiralLab.Sirius2.Vision.Inspector.IInspector">IInspector</see>
        /// </summary>
        /// <remarks>
        /// Created by <see cref="SpiralLab.Sirius2.Vision.Inspector.InspectorFactory">InspectorFactory</see>. <br/>
        /// </remarks>
        [Browsable(false)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Inspector")]
        [Description("Inspector Instance")]
        public SpiralLab.Sirius2.Vision.Inspector.IInspector Inspector
        {
            get { return inspector; }
            set
            {
                if (inspector != null)
                {
                    inspector.OnStarted -= Inspector_OnStarted;
                    inspector.OnEnded -= Inspector_OnEnded;
                }
                inspector = value;
                InspectorCtrl.Inspector = inspector;
                TreeViewCtrl.Inspector = inspector;
                DisplayCtrl.Inspector = inspector;
                ScriptCtrl.Inspector = inspector;
                if (inspector != null)
                {
                    inspector.OnStarted += Inspector_OnStarted;
                    inspector.OnEnded += Inspector_OnEnded;
                }
            }
        }
        private SpiralLab.Sirius2.Vision.Inspector.IInspector inspector;
        /// <summary>
        /// <see cref="SpiralLab.Sirius2.Vision.UI.TreeViewUserControl">TreeViewUserControl</see> user control for <see cref="SpiralLab.Sirius2.Vision.Process.IProcess">IProcess</see> within <see cref="SpiralLab.Sirius2.Vision.Process.ProcessLayer">ProcessLayer</see> nodes
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("TreeViewUserControl")]
        [Description("TreeView UserControl")]
        public SpiralLab.Sirius2.Vision.UI.TreeViewUserControl TreeViewCtrl
        {
            get { return treeviewProcessUserControl; }
        }
        /// <summary>
        /// <see cref="SpiralLab.Sirius2.Vision.UI.PropertyGridUserControl">PropertyGridUserControl</see> user control for properties of <see cref="SpiralLab.Sirius2.Vision.Process.IProcess">IProcess</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("PropertyGridUserControl")]
        [Description("PropertyGrid UserControl")]
        public SpiralLab.Sirius2.Vision.UI.PropertyGridUserControl PropertyGridCtrl
        {
            get { return propertyGridUserControl1; }
        }
        /// <summary>
        /// <see cref="SpiralLab.Sirius2.Vision.UI.DisplayUserControl">DisplayUserControl</see> user control 
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("DisplayControl")]
        [Description("Display UserControl")]
        public SpiralLab.Sirius2.Vision.UI.DisplayUserControl DisplayCtrl
        {
            get { return displayUserControl1; }
        }
        /// <summary>
        /// <see cref="SpiralLab.Sirius2.Vision.UI.CameraUserControl">CameraUserControl</see> user control 
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("CameraUserControl")]
        [Description("Camera UserControl")]
        public SpiralLab.Sirius2.Vision.UI.CameraUserControl CameraCtrl
        {
            get { return cameraUserControl1; }
        }
        /// <summary>
        /// <see cref="SpiralLab.Sirius2.Vision.UI.InspectorUserControl">InspectorUserControl</see> user control 
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("InspectorUserControl")]
        [Description("Inspector UserControl")]
        public SpiralLab.Sirius2.Vision.UI.InspectorUserControl InspectorCtrl
        {
            get { return inspectorUserControl1; }
        }
        /// <summary>
        /// ScriptUserControl user control for <see cref="SpiralLab.Sirius2.Vision.Script.IScript">IScript</see>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("ScriptUserControl")]
        [Description("Script UserControl")]
        public SpiralLab.Sirius2.Vision.UI.ScriptUserControl ScriptCtrl
        {
            get { return scriptUserControl1; }
        }
        /// <summary>
        /// <see cref="SpiralLab.Sirius2.Vision.UI.LogUserControl">LogUserControl</see> user control for logged messages
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("LogUserControl")]
        [Description("Log UserControl")]
        public SpiralLab.Sirius2.Vision.UI.LogUserControl LogCtrl
        {
            get { return logUserControl1; }
        }
        /// <summary>
        /// Show(or Visible) <see cref="SpiralLab.Sirius2.Vision.UI.LogUserControl">LogUserControl</see> usercontrol or not
        /// </summary>
        [ReadOnly(false)]
        [Category("Sirius2")]
        [DisplayName("Show LogUserControl")]
        [Description("Show Log UserControl or Not")]
        public bool IsShowLogCtrl
        {
            get { return !spcMain.Panel2Collapsed; }
            set { spcMain.Panel2Collapsed = !value; }
        }

        System.Windows.Forms.Timer timerProgress = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timerStatus = new System.Windows.Forms.Timer();
        Stopwatch timerProgressStopwatch = new Stopwatch();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// Create devices likes as <c>ICamera</c>, ... and assign. <br/>
        /// Create <c>IInspector</c> and assign. <br/>
        /// <c>IDocument</c> is created by automatically. <br/>
        /// </remarks>
        public SiriusVisionControl()
        {
            InitializeComponent();

            treeviewProcessUserControl.PropertyGridCtrl = this.PropertyGridCtrl;

            VisibleChanged += SiriusVisionControl_VisibleChanged;
            Disposed += SiriusVisionControl_Disposed;

            timerProgress.Interval = 100;
            timerProgress.Tick += TimerProgress_Tick;
            timerStatus.Interval = 100;
            timerStatus.Tick += TimerStatus_Tick;
            lblLog.DoubleClick += LblLog_DoubleClick;
            lblLog.DoubleClickEnabled = true;

            btnNew.Click += btnNew_Click;
            btnOpen.Click += btnOpen_Click;
            btnSave.Click += btnSave_Click;
            btnCopy.Click += BtnCopy_Click;
            btnCut.Click += BtnCut_Click;
            btnPaste.Click += BtnPaste_Click;
            btnDelete.Click += BtnDelete_Click;

            btnLine.Click += BtnLine_Click;
            btnCross.Click += BtnCross_Click;
            btnCircle.Click += BtnCircle_Click;
            btnBlob.Click += BtnBlob_Click;
            btnPattern.Click += BtnPattern_Click;
            btnBarcode.Click += BtnBarcode_Click;


            // Create document by default 
            Document = SpiralLab.Sirius2.Vision.DocumentFactory.CreateDefault();
        }

        /// <inheritdoc/>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //Inspect: F4
            if (keyData == SpiralLab.Sirius2.Vision.Config.KeyboardInspector)
            {
                if (null != Camera && null != Document)
                {
                    bool success = true;
                    success &= Inspector.Ready(this.Document, Camera);
                    var arg = new InspectorArg(DisplayCtrl);
                    success &= Inspector.Start(arg);
                    return true;
                }
            }
            // Start: F5
            else if (keyData == SpiralLab.Sirius2.Vision.Config.KeyboardGrabAndInspector) 
            {
                if (null != Inspector && null != Document)
                {
                    bool success = true;
                    if (Document.Selected.Length > 0)
                    {
                        if (1 != Document.Selected.Length)
                        {
                            var form = new SpiralLab.Sirius2.Vision.UI.MessageBox($"Please select single process to grab and inspect", "Error", MessageBoxButtons.OK);
                            form.ShowDialog();
                            success = false;
                            return true;
                        }
                        success = Camera.CtlGrab(Document.Selected[0]);
                    }
                    else
                        success = Camera.CtlGrab(null);
                    return true;
                }
            }
            // Reset: F8
            else if (keyData == SpiralLab.Sirius2.Vision.Config.KeyboardInspectorReset)
            {
                if (null != Inspector)
                {
                    Inspector.Reset();
                    return true;
                }
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void SiriusVisionControl_Disposed(object sender, EventArgs e)
        {
            timerStatus.Enabled = false;
            timerProgress.Enabled = false;
            timerStatus.Tick -= TimerStatus_Tick;
            timerProgress.Tick -= TimerProgress_Tick;
        }
        private void SiriusVisionControl_VisibleChanged(object sender, EventArgs e)
        {
            timerStatus.Enabled = Visible;

            //if (Visible)
            //{ 
            //    switch (Config.ProcessEngine)
            //    {
            //        case Config.ProcessEngines.VisionPro:
            //            btnPatternMulti.Enabled = true;
            //            break;
            //        case Config.ProcessEngines.OpenCV:
            //            btnPatternMulti.Enabled = false;
            //            break;
            //    }
            //}
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Document.ActNew();

            //CogDispHelper.ClearDisp(this.cogDispOrigin);
            //imageOrigin.Clear();
            //cogDispOrigin.Image = imageOrigin.Image;
            //cogDispOrigin.Fit(true);
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (NotifyOpenBefore())
                return;
            var dlg = new OpenFileDialog();
            dlg.Filter = SpiralLab.Sirius2.Vision.Config.FileOpenFilters;
            dlg.Title = "Open File";
            dlg.InitialDirectory = SpiralLab.Sirius2.Vision.Config.RecipePath;
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;
            if (Document.IsModified)
            {
                //var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox(
                //    Properties.Resources.DocumentOpen,
                //    Properties.Resources.Warning,
                //    MessageBoxButtons.YesNo);
                //DialogResult dialogResult = form.ShowDialog(this);
                //if (dialogResult != DialogResult.Yes)
                //    return;
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
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (NotifySaveBefore())
                return;
            var dlg = new SaveFileDialog();
            dlg.Filter = SpiralLab.Sirius2.Vision.Config.FileSaveFilters;
            dlg.Title = "Save File";
            dlg.InitialDirectory = SpiralLab.Sirius2.Vision.Config.RecipePath;
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
        private void BtnCopy_Click(object sender, EventArgs e)
        {
            Document.ActCopy();
        }
        private void BtnPaste_Click(object sender, EventArgs e)
        {
            Document.ActPaste();
        }
        private void BtnCut_Click(object sender, EventArgs e)
        {
            Document.ActCut();
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            document.ActRemove(document.Selected);
        }

        private void BtnLine_Click(object sender, EventArgs e)
        {
            var process = ProcessFactory.CreateLine();
            Document.ActAdd(process);
        }

        private void BtnCross_Click(object sender, EventArgs e)
        {
            var process = ProcessFactory.CreateCross();
            Document.ActAdd(process);
        }
        private void BtnCircle_Click(object sender, EventArgs e)
        {
            var process = ProcessFactory.CreateCircle();
            Document.ActAdd(process);
        }
        private void BtnBlob_Click(object sender, EventArgs e)
        {
            var process = ProcessFactory.CreateBlob();
            Document.ActAdd(process);
        }
      
        private void BtnPattern_Click(object sender, EventArgs e)
        {
            var process = ProcessFactory.CreatePattern();
            Document.ActAdd(process);
        }
        private void BtnBarcode_Click(object sender, EventArgs e)
        {
            var process = ProcessFactory.CreateBarcode();
            Document.ActAdd(process);
        }

        /// <summary>
        /// Event handler for <c>Document</c> has opened
        /// </summary>
        /// <param name="document"><c>IDocument</c></param>
        /// <param name="fileName">Filename</param>
        private void Document_OnOpened(SpiralLab.Sirius2.Vision.IDocument document, string fileName)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;
            this.Invoke(new MethodInvoker(delegate ()
            {
                lblFileName.Text = fileName;
                PropertyGridCtrl.Refresh();
            }));
        }
        /// <summary>
        /// Event handler for <c>Document</c> has saved
        /// </summary>
        /// <param name="document"><c>IDocument</c></param>
        /// <param name="fileName">Filename</param>
        private void Document_OnSaved(SpiralLab.Sirius2.Vision.IDocument document, string fileName)
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
            IsShowLogCtrl = !IsShowLogCtrl;
        }

        int timerStatusColorCounts = 0;
        /// <summary>
        /// Update status by timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            if (null == this.Inspector)
                return;
            if (this.Inspector.IsReady)
            {
                lblReady.ForeColor = Color.Black;
                lblReady.BackColor = Color.Lime;
            }
            else
            {
                lblReady.ForeColor = Color.White;
                lblReady.BackColor = Color.Green;
            }

            if (this.Inspector.IsBusy)
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

            if (this.Inspector.IsError)
            {
                lblError.ForeColor = Color.White;
                lblError.BackColor = Color.Red;
            }
            else
            {
                lblError.ForeColor = Color.White;
                lblError.BackColor = Color.Maroon;
            }


        }
        private void Inspector_OnStarted(IInspector arg1, SpiralLab.Sirius2.Vision.Inspector.InspectorArg arg)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;
            timerProgressStopwatch.Restart();

            this.Invoke(new MethodInvoker(delegate ()
            {
                lblProcessTime.Text = $"Inspecting ...";
                timerProgress.Enabled = true;
                EditabilityByInspectorBusy(false);
            }));
        }
        int timerProgressColorCounts = 0;
        /// <summary>
        /// Update inspector progressing by timer
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
        private void Inspector_OnEnded(SpiralLab.Sirius2.Vision.Inspector.IInspector inspector, SpiralLab.Sirius2.Vision.Inspector.InspectorArg arg)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;
            timerProgressStopwatch.Stop();
            this.Invoke(new MethodInvoker(delegate ()
            {
                timerProgress.Enabled = false;
                lblProcessTime.Text = $"{arg.ExecuteTime.TotalSeconds:F3} sec";
                if (arg.OverAllResult)
                    lblProcessTime.ForeColor = stsBottom.ForeColor;
                else
                    lblProcessTime.ForeColor = Color.Red;
                EditabilityByInspectorBusy(true);
                DisplayCtrl.Focus();
            }));
        }
        private bool CameraStitched_OnStitchedImageGrabStarted(SpiralLab.Sirius2.Vision.Camera.ICameraStitched camera, SpiralLab.Sirius2.Vision.Process.IProcess process, object state)
        {
            EditabilityByInspectorBusy(false);
            return true;
        }
        private bool CameraStitched_OnStitchedImageGrabEnded(SpiralLab.Sirius2.Vision.Camera.ICameraStitched camera, SpiralLab.Sirius2.Vision.Process.IProcess process, object state)
        {
            EditabilityByInspectorBusy(true);
            return true;
        }

        /// <summary>
        /// Enable(or disable) controls during <see cref="IInspector">IInspector</see> status is busy
        /// </summary>
        /// <param name="isEnable">Enable(or disable) controls</param>
        /// <remarks>
        /// To disable edit operations during inspector is busy status. <br/>
        /// </remarks>
        public virtual void EditabilityByInspectorBusy(bool isEnable)
        {
            if (!this.IsHandleCreated || this.IsDisposed)
                return;
            this.Invoke(new MethodInvoker(delegate ()
            {
                tlsTop1.Enabled = isEnable;
                tlsTop2.Enabled = isEnable;
                tbcLeft.Enabled = isEnable;
                tbcRight.Enabled = isEnable;
#if DEBUG
                // let them enabled for debugging purpose
#else

#endif
            }));
        }
    }
}
