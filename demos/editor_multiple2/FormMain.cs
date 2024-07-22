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
    public partial class FormMain : Form
    {
        Form formCurrent = null;
        FormEditor[] formEditors = null;

        public FormMain()
        {
            InitializeComponent();

            formEditors = new FormEditor[2]
            {
                new FormEditor(0),
                new FormEditor(1),
                //new FormEditor(2),
                //new FormEditor(3),
            };
            formEditors[0].siriusEditorUserControl1.TitleName = "[1] UV";
            formEditors[1].siriusEditorUserControl1.TitleName = "[2] IR";
            //formEditors[2].siriusEditorUserControl1.TitleName = "[3] CO2";
            //formEditors[3].siriusEditorUserControl1.TitleName = "[4] GREEN";

            button1.Click += Button_Click;
            button2.Click += Button_Click;
            //button3.Click += FormMain_Click;
            //button4.Click += FormMain_Click;

            FormClosing += FormMain_FormClosing;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var editor in formEditors)
                editor.Close();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse((string)button.Tag);

            var editorCtrl = formEditors[index].siriusEditorUserControl1;

            // open sirius file at editor and marker to ready
            //string fileName = Path.Combine(SpiralLab.Sirius2.Winforms.Config.RecipePath, "cal_100mm_5x5.sirius2");
            //editorCtrl.Document.ActOpen(fileName);
            //
            //or
            //
            // open another sirius file and marker to ready
            //var doc = DocumentFactory.CreateDefault();
            //if (!doc.ActOpen(fileName))
            //    return;
            //editorCtrl.Marker.Ready(doc);

            SwitchForm(panel1, formEditors[index]);
        }

        public void SwitchForm(Panel destination, Form target)
        {
            if (this.formCurrent == target)
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

            this.formCurrent = target;
        }
    }
}
