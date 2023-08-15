namespace Demos
{
    partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.siriusEditorUserControl1 = new SpiralLab.Sirius2.Winforms.UI.SiriusEditorUserControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.siriusEditorUserControl2 = new SpiralLab.Sirius2.Winforms.UI.SiriusEditorUserControl();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.HotTrack = true;
            this.tabControl1.ItemSize = new System.Drawing.Size(94, 38);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1264, 861);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.siriusEditorUserControl1);
            this.tabPage1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 42);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1256, 815);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "SYSTEM1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // siriusEditorUserControl1
            // 
            this.siriusEditorUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusEditorUserControl1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorUserControl1.Laser = null;
            this.siriusEditorUserControl1.Location = new System.Drawing.Point(3, 3);
            this.siriusEditorUserControl1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.siriusEditorUserControl1.Marker = null;
            this.siriusEditorUserControl1.Name = "siriusEditorUserControl1";
            this.siriusEditorUserControl1.Rtc = null;
            this.siriusEditorUserControl1.Size = new System.Drawing.Size(1250, 809);
            this.siriusEditorUserControl1.TabIndex = 0;
            this.siriusEditorUserControl1.TitleName = "NoName";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.siriusEditorUserControl2);
            this.tabPage2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage2.Location = new System.Drawing.Point(4, 42);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(2138, 1318);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SYSTEM2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // siriusEditorUserControl2
            // 
            this.siriusEditorUserControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusEditorUserControl2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorUserControl2.Laser = null;
            this.siriusEditorUserControl2.Location = new System.Drawing.Point(3, 3);
            this.siriusEditorUserControl2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.siriusEditorUserControl2.Marker = null;
            this.siriusEditorUserControl2.Name = "siriusEditorUserControl2";
            this.siriusEditorUserControl2.Rtc = null;
            this.siriusEditorUserControl2.Size = new System.Drawing.Size(2132, 1312);
            this.siriusEditorUserControl2.TabIndex = 0;
            this.siriusEditorUserControl2.TitleName = "NoName";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1264, 861);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Sirius2 Editor Demo - (c)SpiralLAB";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private SpiralLab.Sirius2.Winforms.UI.SiriusEditorUserControl siriusEditorUserControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private SpiralLab.Sirius2.Winforms.UI.SiriusEditorUserControl siriusEditorUserControl2;
    }
}

