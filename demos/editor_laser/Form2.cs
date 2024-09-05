using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SpiralLab.Sirius2.Winforms;

namespace Demos
{
    public partial class Form2 : Form
    {
        Form FormCurrent = null;

        Form1 form2 = new Form1();

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string fileName = Path.Combine(SpiralLab.Sirius2.Winforms.Config.RecipePath, "cal_100mm_5x5.sirius2");
            //var doc = DocumentFactory.CreateDefault();
            //if (!doc.ActOpen(fileName))
            //    return;
            form2.siriusEditorUserControl1.Document.ActOpen(fileName);

            //marker.Ready(doc);
            //marker.Start();
            // User MUST revert document as original on after marker has finished !
            // by marker.Ready(siriusEditorUserControl1.Document, ...);

            SwitchForm(panel1, form2);

        }

        public void SwitchForm(Panel destination, Form target)
        {
            if (this.FormCurrent == target)
                return;

            destination.SuspendLayout();

            foreach (Form f in destination.Controls)
            {
                f.Visible = false;
                f.Hide();
            }
            destination.Controls.Clear();
            target.SuspendLayout();
            target.TopLevel = false;
            target.FormBorderStyle = FormBorderStyle.None;
            target.Dock = DockStyle.Fill;
            target.Visible = true;
            destination.Controls.Add(target);
            target.ResumeLayout();
            destination.ResumeLayout();

            this.FormCurrent = target;
        }
    }
}
