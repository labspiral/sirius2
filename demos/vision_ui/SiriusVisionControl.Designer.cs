namespace Demos
{
    partial class SiriusVisionControl
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SiriusVisionControl));
            this.tlsTop1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAbout = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCopy = new System.Windows.Forms.ToolStripButton();
            this.btnCut = new System.Windows.Forms.ToolStripButton();
            this.btnPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.stsBottom = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel14 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblProcessTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblFileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel6 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel10 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblLog = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblReady = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel8 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBusy = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel9 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblError = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tlsTop2 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLine = new System.Windows.Forms.ToolStripButton();
            this.btnCross = new System.Windows.Forms.ToolStripButton();
            this.btnCircle = new System.Windows.Forms.ToolStripButton();
            this.btnBlob = new System.Windows.Forms.ToolStripButton();
            this.btnPattern = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.btnBarcode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ddbControl = new System.Windows.Forms.ToolStripDropDownButton();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbcLeft = new System.Windows.Forms.TabControl();
            this.tabLayer = new System.Windows.Forms.TabPage();
            this.treeviewProcessUserControl = new SpiralLab.Sirius2.Vision.UI.TreeViewUserControl();
            this.tabCamera = new System.Windows.Forms.TabPage();
            this.cameraUserControl1 = new SpiralLab.Sirius2.Vision.UI.CameraUserControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbcRight = new System.Windows.Forms.TabControl();
            this.tabProperty = new System.Windows.Forms.TabPage();
            this.propertyGridUserControl1 = new SpiralLab.Sirius2.Vision.UI.PropertyGridUserControl();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.spcMain = new System.Windows.Forms.SplitContainer();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabDisplay = new System.Windows.Forms.TabPage();
            this.displayUserControl1 = new SpiralLab.Sirius2.Vision.UI.DisplayUserControl();
            this.tabInspector = new System.Windows.Forms.TabPage();
            this.inspectorUserControl1 = new SpiralLab.Sirius2.Vision.UI.InspectorUserControl();
            this.tabScript = new System.Windows.Forms.TabPage();
            this.scriptUserControl1 = new SpiralLab.Sirius2.Vision.UI.ScriptUserControl();
            this.logUserControl1 = new SpiralLab.Sirius2.Vision.UI.LogUserControl();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tlsTop1.SuspendLayout();
            this.stsBottom.SuspendLayout();
            this.tlsTop2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tbcLeft.SuspendLayout();
            this.tabLayer.SuspendLayout();
            this.tabCamera.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tbcRight.SuspendLayout();
            this.tabProperty.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).BeginInit();
            this.spcMain.Panel1.SuspendLayout();
            this.spcMain.Panel2.SuspendLayout();
            this.spcMain.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabDisplay.SuspendLayout();
            this.tabInspector.SuspendLayout();
            this.tabScript.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlsTop1
            // 
            resources.ApplyResources(this.tlsTop1, "tlsTop1");
            this.tlsTop1.GripMargin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.tlsTop1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tlsTop1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tlsTop1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator5,
            this.btnAbout,
            this.toolStripSeparator6,
            this.btnNew,
            this.btnOpen,
            this.btnSave,
            this.toolStripSeparator4,
            this.toolStripSeparator2,
            this.btnCopy,
            this.btnCut,
            this.btnPaste,
            this.toolStripSeparator7,
            this.btnDelete});
            this.tlsTop1.Name = "tlsTop1";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // btnAbout
            // 
            this.btnAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnAbout, "btnAbout");
            this.btnAbout.Name = "btnAbout";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnNew, "btnNew");
            this.btnNew.Name = "btnNew";
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnOpen, "btnOpen");
            this.btnOpen.Name = "btnOpen";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // btnCopy
            // 
            this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnCopy, "btnCopy");
            this.btnCopy.Name = "btnCopy";
            // 
            // btnCut
            // 
            this.btnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnCut, "btnCut");
            this.btnCut.Name = "btnCut";
            // 
            // btnPaste
            // 
            this.btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnPaste, "btnPaste");
            this.btnPaste.Name = "btnPaste";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            // 
            // stsBottom
            // 
            resources.ApplyResources(this.stsBottom, "stsBottom");
            this.stsBottom.GripMargin = new System.Windows.Forms.Padding(0);
            this.stsBottom.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.stsBottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel14,
            this.lblName,
            this.toolStripStatusLabel2,
            this.lblProcessTime,
            this.toolStripStatusLabel5,
            this.lblFileName,
            this.toolStripStatusLabel6,
            this.toolStripStatusLabel10,
            this.lblLog,
            this.toolStripStatusLabel3,
            this.lblReady,
            this.toolStripStatusLabel8,
            this.lblBusy,
            this.toolStripStatusLabel9,
            this.lblError});
            this.stsBottom.Name = "stsBottom";
            this.stsBottom.ShowItemToolTips = true;
            this.stsBottom.SizingGrip = false;
            // 
            // toolStripStatusLabel14
            // 
            resources.ApplyResources(this.toolStripStatusLabel14, "toolStripStatusLabel14");
            this.toolStripStatusLabel14.Name = "toolStripStatusLabel14";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // toolStripStatusLabel2
            // 
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            // 
            // lblProcessTime
            // 
            resources.ApplyResources(this.lblProcessTime, "lblProcessTime");
            this.lblProcessTime.Name = "lblProcessTime";
            // 
            // toolStripStatusLabel5
            // 
            resources.ApplyResources(this.toolStripStatusLabel5, "toolStripStatusLabel5");
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            // 
            // lblFileName
            // 
            resources.ApplyResources(this.lblFileName, "lblFileName");
            this.lblFileName.Name = "lblFileName";
            // 
            // toolStripStatusLabel6
            // 
            resources.ApplyResources(this.toolStripStatusLabel6, "toolStripStatusLabel6");
            this.toolStripStatusLabel6.Name = "toolStripStatusLabel6";
            // 
            // toolStripStatusLabel10
            // 
            this.toolStripStatusLabel10.Name = "toolStripStatusLabel10";
            resources.ApplyResources(this.toolStripStatusLabel10, "toolStripStatusLabel10");
            this.toolStripStatusLabel10.Spring = true;
            // 
            // lblLog
            // 
            this.lblLog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.lblLog, "lblLog");
            this.lblLog.Name = "lblLog";
            // 
            // toolStripStatusLabel3
            // 
            resources.ApplyResources(this.toolStripStatusLabel3, "toolStripStatusLabel3");
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            // 
            // lblReady
            // 
            this.lblReady.ActiveLinkColor = System.Drawing.Color.Red;
            this.lblReady.BackColor = System.Drawing.Color.Green;
            resources.ApplyResources(this.lblReady, "lblReady");
            this.lblReady.ForeColor = System.Drawing.Color.White;
            this.lblReady.Name = "lblReady";
            // 
            // toolStripStatusLabel8
            // 
            resources.ApplyResources(this.toolStripStatusLabel8, "toolStripStatusLabel8");
            this.toolStripStatusLabel8.Name = "toolStripStatusLabel8";
            // 
            // lblBusy
            // 
            this.lblBusy.BackColor = System.Drawing.Color.Olive;
            resources.ApplyResources(this.lblBusy, "lblBusy");
            this.lblBusy.ForeColor = System.Drawing.Color.White;
            this.lblBusy.Name = "lblBusy";
            // 
            // toolStripStatusLabel9
            // 
            resources.ApplyResources(this.toolStripStatusLabel9, "toolStripStatusLabel9");
            this.toolStripStatusLabel9.Name = "toolStripStatusLabel9";
            // 
            // lblError
            // 
            this.lblError.BackColor = System.Drawing.Color.Maroon;
            resources.ApplyResources(this.lblError, "lblError");
            this.lblError.ForeColor = System.Drawing.Color.White;
            this.lblError.Name = "lblError";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "cube_24px.png");
            this.imageList1.Images.SetKeyName(1, "binary_code2_24px.png");
            this.imageList1.Images.SetKeyName(2, "welder_24px.png");
            this.imageList1.Images.SetKeyName(3, "sheets_24px.png");
            this.imageList1.Images.SetKeyName(4, "candy_cane_pattern_24px.png");
            this.imageList1.Images.SetKeyName(5, "spreadsheet_file_24px.png");
            this.imageList1.Images.SetKeyName(6, "Electricity Hazard_24px.png");
            this.imageList1.Images.SetKeyName(7, "paint_palette_24px.png");
            this.imageList1.Images.SetKeyName(8, "sheets2_24px.png");
            this.imageList1.Images.SetKeyName(9, "chain_intermediate_24px.png");
            this.imageList1.Images.SetKeyName(10, "Stacked Organizational Chart.png");
            this.imageList1.Images.SetKeyName(11, "Login.png");
            this.imageList1.Images.SetKeyName(12, "Binary Code.png");
            this.imageList1.Images.SetKeyName(13, "Ctrl.png");
            this.imageList1.Images.SetKeyName(14, "Video Card.png");
            this.imageList1.Images.SetKeyName(15, "Picture.png");
            this.imageList1.Images.SetKeyName(16, "Property.png");
            this.imageList1.Images.SetKeyName(17, "3D Object.png");
            this.imageList1.Images.SetKeyName(18, "Circled Play.png");
            this.imageList1.Images.SetKeyName(19, "Paint Palette.png");
            this.imageList1.Images.SetKeyName(20, "Vending Machine.png");
            this.imageList1.Images.SetKeyName(21, "Processor.png");
            this.imageList1.Images.SetKeyName(22, "Processor2.png");
            this.imageList1.Images.SetKeyName(23, "Pencil.png");
            this.imageList1.Images.SetKeyName(24, "Design.png");
            this.imageList1.Images.SetKeyName(25, "Video Card.png");
            this.imageList1.Images.SetKeyName(26, "light_on_24px.png");
            this.imageList1.Images.SetKeyName(27, "Broadcasting.png");
            this.imageList1.Images.SetKeyName(28, "RS-232 Male.png");
            this.imageList1.Images.SetKeyName(29, "bar_chart_30px.png");
            this.imageList1.Images.SetKeyName(30, "Graph2.png");
            this.imageList1.Images.SetKeyName(31, "Graph.png");
            this.imageList1.Images.SetKeyName(32, "line_chart_24px.png");
            this.imageList1.Images.SetKeyName(33, "line_chart_26px.png");
            this.imageList1.Images.SetKeyName(34, "stocks_24px.png");
            this.imageList1.Images.SetKeyName(35, "stocks_32px.png");
            this.imageList1.Images.SetKeyName(36, "C Sharp Logo.png");
            this.imageList1.Images.SetKeyName(37, "7077517_csharp_file_icon.png");
            this.imageList1.Images.SetKeyName(38, "free-icon-file-and-folder-2807467.png");
            this.imageList1.Images.SetKeyName(39, "csharp.ico");
            this.imageList1.Images.SetKeyName(40, "Voltage.png");
            this.imageList1.Images.SetKeyName(41, "light_on2_24px.png");
            this.imageList1.Images.SetKeyName(42, "Camera.png");
            this.imageList1.Images.SetKeyName(43, "aspect_ratio_24px.png");
            this.imageList1.Images.SetKeyName(44, "Searchlight.png");
            this.imageList1.Images.SetKeyName(45, "creategrid.png");
            this.imageList1.Images.SetKeyName(46, "Camera2.png");
            this.imageList1.Images.SetKeyName(47, "Web Camera.png");
            this.imageList1.Images.SetKeyName(48, "Vintage Camera.png");
            this.imageList1.Images.SetKeyName(49, "Camera3.png");
            this.imageList1.Images.SetKeyName(50, "Hide Grid.png");
            this.imageList1.Images.SetKeyName(51, "Grid3.png");
            this.imageList1.Images.SetKeyName(52, "Grid2.png");
            this.imageList1.Images.SetKeyName(53, "Chess Board.png");
            this.imageList1.Images.SetKeyName(54, "Idea.png");
            this.imageList1.Images.SetKeyName(55, "Headlight2.png");
            this.imageList1.Images.SetKeyName(56, "Headlight.png");
            this.imageList1.Images.SetKeyName(57, "Searchlight2.png");
            this.imageList1.Images.SetKeyName(58, "Search Bar.png");
            this.imageList1.Images.SetKeyName(59, "Search More2.png");
            this.imageList1.Images.SetKeyName(60, "Search More.png");
            // 
            // tlsTop2
            // 
            resources.ApplyResources(this.tlsTop2, "tlsTop2");
            this.tlsTop2.GripMargin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.tlsTop2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tlsTop2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tlsTop2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator12,
            this.btnLine,
            this.btnCross,
            this.btnCircle,
            this.btnBlob,
            this.btnPattern,
            this.toolStripSeparator3,
            this.toolStripSeparator13,
            this.btnBarcode,
            this.toolStripSeparator1,
            this.ddbControl});
            this.tlsTop2.Name = "tlsTop2";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // btnLine
            // 
            this.btnLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnLine, "btnLine");
            this.btnLine.Name = "btnLine";
            // 
            // btnCross
            // 
            this.btnCross.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnCross, "btnCross");
            this.btnCross.Name = "btnCross";
            // 
            // btnCircle
            // 
            this.btnCircle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnCircle, "btnCircle");
            this.btnCircle.Name = "btnCircle";
            // 
            // btnBlob
            // 
            this.btnBlob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnBlob, "btnBlob");
            this.btnBlob.Name = "btnBlob";
            // 
            // btnPattern
            // 
            this.btnPattern.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnPattern, "btnPattern");
            this.btnPattern.Name = "btnPattern";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            // 
            // btnBarcode
            // 
            this.btnBarcode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnBarcode, "btnBarcode");
            this.btnBarcode.Name = "btnBarcode";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // ddbControl
            // 
            this.ddbControl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.ddbControl, "ddbControl");
            this.ddbControl.Name = "ddbControl";
            // 
            // splitter3
            // 
            resources.ApplyResources(this.splitter3, "splitter3");
            this.splitter3.Name = "splitter3";
            this.splitter3.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tbcLeft);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tbcLeft
            // 
            this.tbcLeft.Controls.Add(this.tabLayer);
            this.tbcLeft.Controls.Add(this.tabCamera);
            resources.ApplyResources(this.tbcLeft, "tbcLeft");
            this.tbcLeft.HotTrack = true;
            this.tbcLeft.ImageList = this.imageList1;
            this.tbcLeft.Name = "tbcLeft";
            this.tbcLeft.SelectedIndex = 0;
            // 
            // tabLayer
            // 
            this.tabLayer.Controls.Add(this.treeviewProcessUserControl);
            resources.ApplyResources(this.tabLayer, "tabLayer");
            this.tabLayer.Name = "tabLayer";
            this.tabLayer.UseVisualStyleBackColor = true;
            // 
            // treeviewProcessUserControl
            // 
            this.treeviewProcessUserControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.treeviewProcessUserControl.Camera = null;
            this.treeviewProcessUserControl.DisplayCtrl = null;
            resources.ApplyResources(this.treeviewProcessUserControl, "treeviewProcessUserControl");
            this.treeviewProcessUserControl.Document = null;
            this.treeviewProcessUserControl.Inspector = null;
            this.treeviewProcessUserControl.Name = "treeviewProcessUserControl";
            this.treeviewProcessUserControl.PropertyGridCtrl = null;
            // 
            // tabCamera
            // 
            this.tabCamera.Controls.Add(this.cameraUserControl1);
            resources.ApplyResources(this.tabCamera, "tabCamera");
            this.tabCamera.Name = "tabCamera";
            this.tabCamera.UseVisualStyleBackColor = true;
            // 
            // cameraUserControl1
            // 
            this.cameraUserControl1.Camera = null;
            this.cameraUserControl1.DisplayCtrl = null;
            resources.ApplyResources(this.cameraUserControl1, "cameraUserControl1");
            this.cameraUserControl1.Document = null;
            this.cameraUserControl1.Name = "cameraUserControl1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tbcRight);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // tbcRight
            // 
            this.tbcRight.Controls.Add(this.tabProperty);
            resources.ApplyResources(this.tbcRight, "tbcRight");
            this.tbcRight.HotTrack = true;
            this.tbcRight.ImageList = this.imageList1;
            this.tbcRight.Multiline = true;
            this.tbcRight.Name = "tbcRight";
            this.tbcRight.SelectedIndex = 0;
            // 
            // tabProperty
            // 
            this.tabProperty.Controls.Add(this.propertyGridUserControl1);
            resources.ApplyResources(this.tabProperty, "tabProperty");
            this.tabProperty.Name = "tabProperty";
            this.tabProperty.UseVisualStyleBackColor = true;
            // 
            // propertyGridUserControl1
            // 
            resources.ApplyResources(this.propertyGridUserControl1, "propertyGridUserControl1");
            this.propertyGridUserControl1.Document = null;
            this.propertyGridUserControl1.Inspector = null;
            this.propertyGridUserControl1.Name = "propertyGridUserControl1";
            this.propertyGridUserControl1.SelecteObject = null;
            this.propertyGridUserControl1.SelecteObjects = new object[0];
            // 
            // splitter2
            // 
            resources.ApplyResources(this.splitter2, "splitter2");
            this.splitter2.Name = "splitter2";
            this.splitter2.TabStop = false;
            // 
            // spcMain
            // 
            resources.ApplyResources(this.spcMain, "spcMain");
            this.spcMain.Name = "spcMain";
            // 
            // spcMain.Panel1
            // 
            this.spcMain.Panel1.Controls.Add(this.tabControl3);
            // 
            // spcMain.Panel2
            // 
            this.spcMain.Panel2.Controls.Add(this.logUserControl1);
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabDisplay);
            this.tabControl3.Controls.Add(this.tabInspector);
            this.tabControl3.Controls.Add(this.tabScript);
            resources.ApplyResources(this.tabControl3, "tabControl3");
            this.tabControl3.HotTrack = true;
            this.tabControl3.ImageList = this.imageList1;
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            // 
            // tabDisplay
            // 
            this.tabDisplay.Controls.Add(this.displayUserControl1);
            resources.ApplyResources(this.tabDisplay, "tabDisplay");
            this.tabDisplay.Name = "tabDisplay";
            this.tabDisplay.UseVisualStyleBackColor = true;
            // 
            // displayUserControl1
            // 
            this.displayUserControl1.AllowDrop = true;
            this.displayUserControl1.Camera = null;
            resources.ApplyResources(this.displayUserControl1, "displayUserControl1");
            this.displayUserControl1.Document = null;
            this.displayUserControl1.Inspector = null;
            this.displayUserControl1.IsEditMode = true;
            this.displayUserControl1.Name = "displayUserControl1";
            // 
            // tabInspector
            // 
            this.tabInspector.Controls.Add(this.inspectorUserControl1);
            resources.ApplyResources(this.tabInspector, "tabInspector");
            this.tabInspector.Name = "tabInspector";
            this.tabInspector.UseVisualStyleBackColor = true;
            // 
            // inspectorUserControl1
            // 
            this.inspectorUserControl1.Camera = null;
            resources.ApplyResources(this.inspectorUserControl1, "inspectorUserControl1");
            this.inspectorUserControl1.Document = null;
            this.inspectorUserControl1.Inspector = null;
            this.inspectorUserControl1.Name = "inspectorUserControl1";
            // 
            // tabScript
            // 
            this.tabScript.Controls.Add(this.scriptUserControl1);
            resources.ApplyResources(this.tabScript, "tabScript");
            this.tabScript.Name = "tabScript";
            this.tabScript.UseVisualStyleBackColor = true;
            // 
            // scriptUserControl1
            // 
            resources.ApplyResources(this.scriptUserControl1, "scriptUserControl1");
            this.scriptUserControl1.Inspector = null;
            this.scriptUserControl1.Name = "scriptUserControl1";
            // 
            // logUserControl1
            // 
            this.logUserControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.logUserControl1, "logUserControl1");
            this.logUserControl1.IsDetailLog = false;
            this.logUserControl1.Name = "logUserControl1";
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // SiriusVisionControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.spcMain);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter3);
            this.Controls.Add(this.tlsTop2);
            this.Controls.Add(this.stsBottom);
            this.Controls.Add(this.tlsTop1);
            this.Name = "SiriusVisionControl";
            this.tlsTop1.ResumeLayout(false);
            this.tlsTop1.PerformLayout();
            this.stsBottom.ResumeLayout(false);
            this.stsBottom.PerformLayout();
            this.tlsTop2.ResumeLayout(false);
            this.tlsTop2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tbcLeft.ResumeLayout(false);
            this.tabLayer.ResumeLayout(false);
            this.tabCamera.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tbcRight.ResumeLayout(false);
            this.tabProperty.ResumeLayout(false);
            this.spcMain.Panel1.ResumeLayout(false);
            this.spcMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).EndInit();
            this.spcMain.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabDisplay.ResumeLayout(false);
            this.tabInspector.ResumeLayout(false);
            this.tabScript.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip tlsTop1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton btnAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.StatusStrip stsBottom;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel14;
        private System.Windows.Forms.ToolStripStatusLabel lblName;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel lblProcessTime;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel lblFileName;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel10;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel lblReady;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel8;
        private System.Windows.Forms.ToolStripStatusLabel lblBusy;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel9;
        private System.Windows.Forms.ToolStripStatusLabel lblError;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel6;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStrip tlsTop2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton btnLine;
        private System.Windows.Forms.ToolStripButton btnCircle;
        private System.Windows.Forms.ToolStripButton btnCross;
        private System.Windows.Forms.ToolStripButton btnBlob;
        private System.Windows.Forms.ToolStripButton btnPattern;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tbcLeft;
        private System.Windows.Forms.TabPage tabLayer;
        private SpiralLab.Sirius2.Vision.UI.TreeViewUserControl treeviewProcessUserControl;
        private System.Windows.Forms.TabPage tabCamera;
        private SpiralLab.Sirius2.Vision.UI.CameraUserControl cameraUserControl1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tbcRight;
        private System.Windows.Forms.TabPage tabProperty;
        private SpiralLab.Sirius2.Vision.UI.PropertyGridUserControl propertyGridUserControl1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.SplitContainer spcMain;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabDisplay;
        private SpiralLab.Sirius2.Vision.UI.DisplayUserControl displayUserControl1;
        private SpiralLab.Sirius2.Vision.UI.LogUserControl logUserControl1;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripButton btnCut;
        private System.Windows.Forms.ToolStripButton btnPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton ddbControl;
        private System.Windows.Forms.TabPage tabInspector;
        private SpiralLab.Sirius2.Vision.UI.InspectorUserControl inspectorUserControl1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.TabPage tabScript;
        private SpiralLab.Sirius2.Vision.UI.ScriptUserControl scriptUserControl1;
        private System.Windows.Forms.ToolStripStatusLabel lblLog;
        private System.Windows.Forms.ToolStripMenuItem btnBarcode;
    }
}
