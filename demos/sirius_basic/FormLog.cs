using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demos
{
    public partial class FormLog : Form
    {
        public FormLog()
        {
            InitializeComponent();

            this.chbTop.Checked = true;
            this.logUserControl1.IsDetailLog = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = chbTop.Checked;
        }
    }
}
