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

