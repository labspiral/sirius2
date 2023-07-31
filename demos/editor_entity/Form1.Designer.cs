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
            this.siriusEditorUserControl1 = new SpiralLab.Sirius2.Winforms.UI.SiriusEditorUserControl();
            this.SuspendLayout();
            // 
            // siriusEditorUserControl1
            // 
            this.siriusEditorUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusEditorUserControl1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusEditorUserControl1.Laser = null;
            this.siriusEditorUserControl1.Location = new System.Drawing.Point(0, 0);
            this.siriusEditorUserControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.siriusEditorUserControl1.Marker = null;
            this.siriusEditorUserControl1.Name = "siriusEditorUserControl1";
            this.siriusEditorUserControl1.Rtc = null;
            this.siriusEditorUserControl1.Size = new System.Drawing.Size(1264, 861);
            this.siriusEditorUserControl1.TabIndex = 0;
            this.siriusEditorUserControl1.TitleName = "NoName";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1264, 861);
            this.Controls.Add(this.siriusEditorUserControl1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Sirius2 Editor Demo - (c)SpiralLAB";
            this.ResumeLayout(false);

        }

        #endregion

        private SpiralLab.Sirius2.Winforms.UI.SiriusEditorUserControl siriusEditorUserControl1;
    }
}