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
            this.visionEditorDisp1 = new Demos.SiriusVisionControl();
            this.SuspendLayout();
            // 
            // visionEditorDisp1
            // 
            this.visionEditorDisp1.Camera = null;
            resources.ApplyResources(this.visionEditorDisp1, "visionEditorDisp1");
            this.visionEditorDisp1.Inspector = null;
            this.visionEditorDisp1.Name = "visionEditorDisp1";
            this.visionEditorDisp1.Rtc = null;
            this.visionEditorDisp1.TitleName = "NoName";
            // 
            // Form2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.visionEditorDisp1);
            this.Name = "Form2";
            this.ResumeLayout(false);

        }

        #endregion

        private Demos.SiriusVisionControl visionEditorDisp1;
        //private Demos.SiriusVisionControlV2 visionEditorDisp1;
    }
}