namespace Demos
{
    partial class Form1
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

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.visionEditorDisp1 = new SpiralLab.Sirius2.Vision.SiriusVisionEditorForm();
            this.SuspendLayout();
            // 
            // visionEditorDisp1
            // 
            this.visionEditorDisp1.Camera = null;
            this.visionEditorDisp1.AliasName = "NoName";
            this.visionEditorDisp1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visionEditorDisp1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.visionEditorDisp1.Inspector = null;
            this.visionEditorDisp1.Location = new System.Drawing.Point(0, 0);
            this.visionEditorDisp1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.visionEditorDisp1.Name = "visionEditorDisp1";
            this.visionEditorDisp1.SelectedProcess = null;
            this.visionEditorDisp1.Size = new System.Drawing.Size(941, 709);
            this.visionEditorDisp1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 709);
            this.Controls.Add(this.visionEditorDisp1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SpiralLab.Sirius2.Vision.SiriusVisionEditorForm visionEditorDisp1;
    }
}

