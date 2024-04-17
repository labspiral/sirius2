using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiralLab.Sirius2.Laser;
using System.Windows.Forms;

namespace Demos
{
    /// <summary>
    /// You can make customized UI for specific laser source
    /// </summary>
    public partial class MyLaserCtrl : UserControl
    {
        /// <summary>
        /// <see cref="ILaser">ILaser</see>
        /// </summary>
        public ILaser Laser
        {
            get { return laser; }
            set
            {
                chbGuide.Enabled = false;
                if (laser != null)
                {
                    ppgLaser.SelectedObject = null;
                    laser.PropertyChanged -= Laser_PropertyChanged;
                }
                laser = value;

                if (laser != null)
                {
                    ppgLaser.SelectedObject = laser;
                    laser.PropertyChanged += Laser_PropertyChanged;
                    if (laser is ILaserGuideControl laserGuideControl)
                    {
                        if (laserGuideControl.IsGuideOn)
                            chbGuide.Checked = true;
                        else
                            chbGuide.Checked = false;
                        chbGuide.Enabled = true;
                    }
                }
            }
        }
        private ILaser laser;

        public MyLaserCtrl(ILaser laser)
        {
            InitializeComponent();

            chbGuide.Click += ChbGuide_Click;
            btnAbort.Click += BtnAbort_Click;
            btnReset.Click += BtnReset_Click;
            Laser = laser;
        }

        private void ChbGuide_Click(object sender, EventArgs e)
        {
            if (null == Laser)
                return;
            if (Laser is ILaserGuideControl laserGuideControl)
            {
                laserGuideControl.CtlGuide(chbGuide.Checked);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            Laser?.CtlReset();
        }

        private void BtnAbort_Click(object sender, EventArgs e)
        {
            Laser?.CtlAbort();
        }

        private void Laser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!this.IsHandleCreated || ppgLaser.IsDisposed || !ppgLaser.IsHandleCreated)
                return;
            ppgLaser.BeginInvoke(new MethodInvoker(delegate ()
            {
                ppgLaser.Refresh();
            }));
        }
    }
}
