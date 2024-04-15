namespace Demos
{
    partial class MyLaserCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyLaserCtrl));
            this.chbGuide = new System.Windows.Forms.CheckBox();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.ppgLaser = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // chbGuide
            // 
            this.chbGuide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chbGuide.Appearance = System.Windows.Forms.Appearance.Button;
            this.chbGuide.Image = ((System.Drawing.Image)(resources.GetObject("chbGuide.Image")));
            this.chbGuide.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chbGuide.Location = new System.Drawing.Point(13, 569);
            this.chbGuide.Name = "chbGuide";
            this.chbGuide.Size = new System.Drawing.Size(120, 42);
            this.chbGuide.TabIndex = 78;
            this.chbGuide.Text = "Guide";
            this.chbGuide.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chbGuide.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.chbGuide.UseVisualStyleBackColor = true;
            // 
            // btnAbort
            // 
            this.btnAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbort.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnAbort.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbort.Image = ((System.Drawing.Image)(resources.GetObject("btnAbort.Image")));
            this.btnAbort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAbort.Location = new System.Drawing.Point(209, 569);
            this.btnAbort.Margin = new System.Windows.Forms.Padding(4);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(120, 42);
            this.btnAbort.TabIndex = 77;
            this.btnAbort.Text = "&Abort";
            this.btnAbort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAbort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAbort.UseVisualStyleBackColor = false;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.Image = ((System.Drawing.Image)(resources.GetObject("btnReset.Image")));
            this.btnReset.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReset.Location = new System.Drawing.Point(338, 569);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 42);
            this.btnReset.TabIndex = 76;
            this.btnReset.Text = "&Reset";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReset.UseVisualStyleBackColor = false;
            // 
            // ppgLaser
            // 
            this.ppgLaser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ppgLaser.Location = new System.Drawing.Point(8, 8);
            this.ppgLaser.Margin = new System.Windows.Forms.Padding(0);
            this.ppgLaser.Name = "ppgLaser";
            this.ppgLaser.Size = new System.Drawing.Size(450, 542);
            this.ppgLaser.TabIndex = 79;
            // 
            // MyLaserCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ppgLaser);
            this.Controls.Add(this.chbGuide);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnReset);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MyLaserCtrl";
            this.Size = new System.Drawing.Size(466, 621);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chbGuide;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.PropertyGrid ppgLaser;
    }
}
