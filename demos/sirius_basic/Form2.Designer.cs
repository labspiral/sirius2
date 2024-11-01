namespace Demos
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.siriusVisionControl1 = new SpiralLab.Sirius2.Vision.UI.SiriusVisionControl();
            this.SuspendLayout();
            // 
            // siriusVisionControl1
            // 
            this.siriusVisionControl1.Camera = null;
            this.siriusVisionControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siriusVisionControl1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siriusVisionControl1.Inspector = null;
            this.siriusVisionControl1.IsShowLogCtrl = true;
            this.siriusVisionControl1.Location = new System.Drawing.Point(0, 0);
            this.siriusVisionControl1.Margin = new System.Windows.Forms.Padding(0);
            this.siriusVisionControl1.Name = "siriusVisionControl1";
            this.siriusVisionControl1.Rtc = null;
            this.siriusVisionControl1.Size = new System.Drawing.Size(1264, 985);
            this.siriusVisionControl1.TabIndex = 0;
            this.siriusVisionControl1.TitleName = "NoName";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 985);
            this.Controls.Add(this.siriusVisionControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "Form2";
            this.ShowInTaskbar = false;
            this.Text = "Vision";
            this.ResumeLayout(false);

        }

        #endregion

        private SpiralLab.Sirius2.Vision.UI.SiriusVisionControl siriusVisionControl1;
    }
}

