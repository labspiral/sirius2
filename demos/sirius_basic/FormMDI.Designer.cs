namespace Demos
{
    partial class FormMDI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMDI));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inspectionResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chbInspectionResultsIntoMarkerOffsets = new System.Windows.Forms.ToolStripMenuItem();
            this.chbInspectionResultsIntoScannerFieldCorrection = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.horizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.editorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.logScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAutoFocus = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.windowsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip1.Size = new System.Drawing.Size(1258, 29);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem.Image")));
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(141, 34);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.inspectionResultsToolStripMenuItem,
            this.mnuAutoFocus});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(92, 29);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // inspectionResultsToolStripMenuItem
            // 
            this.inspectionResultsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chbInspectionResultsIntoMarkerOffsets,
            this.chbInspectionResultsIntoScannerFieldCorrection});
            this.inspectionResultsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("inspectionResultsToolStripMenuItem.Image")));
            this.inspectionResultsToolStripMenuItem.Name = "inspectionResultsToolStripMenuItem";
            this.inspectionResultsToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.inspectionResultsToolStripMenuItem.Text = "Inspection";
            // 
            // chbInspectionResultsIntoMarkerOffsets
            // 
            this.chbInspectionResultsIntoMarkerOffsets.CheckOnClick = true;
            this.chbInspectionResultsIntoMarkerOffsets.Image = ((System.Drawing.Image)(resources.GetObject("chbInspectionResultsIntoMarkerOffsets.Image")));
            this.chbInspectionResultsIntoMarkerOffsets.Name = "chbInspectionResultsIntoMarkerOffsets";
            this.chbInspectionResultsIntoMarkerOffsets.Size = new System.Drawing.Size(328, 34);
            this.chbInspectionResultsIntoMarkerOffsets.Text = "To Marker Offset";
            // 
            // chbInspectionResultsIntoScannerFieldCorrection
            // 
            this.chbInspectionResultsIntoScannerFieldCorrection.CheckOnClick = true;
            this.chbInspectionResultsIntoScannerFieldCorrection.Image = ((System.Drawing.Image)(resources.GetObject("chbInspectionResultsIntoScannerFieldCorrection.Image")));
            this.chbInspectionResultsIntoScannerFieldCorrection.Name = "chbInspectionResultsIntoScannerFieldCorrection";
            this.chbInspectionResultsIntoScannerFieldCorrection.Size = new System.Drawing.Size(328, 34);
            this.chbInspectionResultsIntoScannerFieldCorrection.Text = "To Scanner Field Correction";
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cascadeToolStripMenuItem,
            this.verticalToolStripMenuItem,
            this.horizontalToolStripMenuItem,
            this.toolStripMenuItem1,
            this.editorToolStripMenuItem,
            this.visionToolStripMenuItem,
            this.toolStripMenuItem2,
            this.logScreenToolStripMenuItem});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(102, 29);
            this.windowsToolStripMenuItem.Text = "&Windows";
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cascadeToolStripMenuItem.Image")));
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.cascadeToolStripMenuItem.Text = "Cascade";
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.cascadeToolStripMenuItem_Click);
            // 
            // verticalToolStripMenuItem
            // 
            this.verticalToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("verticalToolStripMenuItem.Image")));
            this.verticalToolStripMenuItem.Name = "verticalToolStripMenuItem";
            this.verticalToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.verticalToolStripMenuItem.Text = "Vertical";
            this.verticalToolStripMenuItem.Click += new System.EventHandler(this.verticalToolStripMenuItem_Click);
            // 
            // horizontalToolStripMenuItem
            // 
            this.horizontalToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("horizontalToolStripMenuItem.Image")));
            this.horizontalToolStripMenuItem.Name = "horizontalToolStripMenuItem";
            this.horizontalToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.horizontalToolStripMenuItem.Text = "Horizontal";
            this.horizontalToolStripMenuItem.Click += new System.EventHandler(this.horizontalToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(277, 6);
            // 
            // editorToolStripMenuItem
            // 
            this.editorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editorToolStripMenuItem.Image")));
            this.editorToolStripMenuItem.Name = "editorToolStripMenuItem";
            this.editorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.editorToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.editorToolStripMenuItem.Text = "Editor Screen";
            this.editorToolStripMenuItem.Click += new System.EventHandler(this.editorToolStripMenuItem_Click);
            // 
            // visionToolStripMenuItem
            // 
            this.visionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("visionToolStripMenuItem.Image")));
            this.visionToolStripMenuItem.Name = "visionToolStripMenuItem";
            this.visionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.visionToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.visionToolStripMenuItem.Text = "Vision Screen";
            this.visionToolStripMenuItem.Click += new System.EventHandler(this.visionToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(277, 6);
            // 
            // logScreenToolStripMenuItem
            // 
            this.logScreenToolStripMenuItem.CheckOnClick = true;
            this.logScreenToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("logScreenToolStripMenuItem.Image")));
            this.logScreenToolStripMenuItem.Name = "logScreenToolStripMenuItem";
            this.logScreenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.logScreenToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.logScreenToolStripMenuItem.Text = "Log Screen";
            this.logScreenToolStripMenuItem.Click += new System.EventHandler(this.logScreenToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // mnuAutoFocus
            // 
            this.mnuAutoFocus.Image = ((System.Drawing.Image)(resources.GetObject("mnuAutoFocus.Image")));
            this.mnuAutoFocus.Name = "mnuAutoFocus";
            this.mnuAutoFocus.Size = new System.Drawing.Size(270, 34);
            this.mnuAutoFocus.Text = "Auto focus";
            this.mnuAutoFocus.Click += new System.EventHandler(this.mnuAutoFocus_Click);
            // 
            // FormMDI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1258, 968);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormMDI";
            this.Text = "Sirius2 Demo - (c)SpiralLAB";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cascadeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem verticalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inspectionResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chbInspectionResultsIntoMarkerOffsets;
        private System.Windows.Forms.ToolStripMenuItem chbInspectionResultsIntoScannerFieldCorrection;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem editorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem horizontalToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem logScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuAutoFocus;
    }
}