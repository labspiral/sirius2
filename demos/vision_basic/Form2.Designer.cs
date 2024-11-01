namespace Demos
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.visionEditorDisp1 = new SpiralLab.Sirius2.Vision.UI.SiriusVisionControl();
            this.SuspendLayout();
            // 
            // visionEditorDisp1
            // 
            this.visionEditorDisp1.Camera = null;
            this.visionEditorDisp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visionEditorDisp1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.visionEditorDisp1.Inspector = null;
            this.visionEditorDisp1.Location = new System.Drawing.Point(0, 0);
            this.visionEditorDisp1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.visionEditorDisp1.Name = "visionEditorDisp1";
            this.visionEditorDisp1.Rtc = null;
            this.visionEditorDisp1.Size = new System.Drawing.Size(1264, 985);
            this.visionEditorDisp1.TabIndex = 1;
            this.visionEditorDisp1.TitleName = "NoName";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 985);
            this.Controls.Add(this.visionEditorDisp1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form2";
            this.Text = "Sirius2 Vision Demo - (c)SpiralLAB";
            this.ResumeLayout(false);

        }

        #endregion

        private SpiralLab.Sirius2.Vision.UI.SiriusVisionControl visionEditorDisp1;
    }
}