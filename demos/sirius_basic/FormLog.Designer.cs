namespace Demos
{
    partial class FormLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLog));
            this.chbTop = new System.Windows.Forms.CheckBox();
            this.logUserControl1 = new SpiralLab.Sirius2.Winforms.UI.LogUserControl();
            this.SuspendLayout();
            // 
            // chbTop
            // 
            this.chbTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chbTop.AutoSize = true;
            this.chbTop.BackColor = System.Drawing.Color.Transparent;
            this.chbTop.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbTop.Location = new System.Drawing.Point(699, 8);
            this.chbTop.Name = "chbTop";
            this.chbTop.Size = new System.Drawing.Size(60, 23);
            this.chbTop.TabIndex = 1;
            this.chbTop.Text = "Top";
            this.chbTop.UseVisualStyleBackColor = false;
            this.chbTop.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // logUserControl1
            // 
            this.logUserControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.logUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logUserControl1.Font = new System.Drawing.Font("Arial", 9F);
            this.logUserControl1.IsDetailLog = false;
            this.logUserControl1.Location = new System.Drawing.Point(0, 0);
            this.logUserControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.logUserControl1.Name = "logUserControl1";
            this.logUserControl1.Size = new System.Drawing.Size(784, 161);
            this.logUserControl1.TabIndex = 0;
            // 
            // FormLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 161);
            this.ControlBox = false;
            this.Controls.Add(this.chbTop);
            this.Controls.Add(this.logUserControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormLog";
            this.Opacity = 0.95D;
            this.Text = "Log - (c)SpiralLAB";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SpiralLab.Sirius2.Winforms.UI.LogUserControl logUserControl1;
        private System.Windows.Forms.CheckBox chbTop;
    }
}