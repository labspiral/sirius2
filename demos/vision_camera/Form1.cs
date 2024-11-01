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
    public partial class Form1 : Form
    {

        SpiralLab.Sirius2.Vision.ICamera camera;
        SpiralLab.Sirius2.Vision.IInspector inspector;


        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ////camera = new SpiralLab.Sirius2.Vision.CameraSentecGigE(0, "Sentec", "192.168.0.11", 21, true, 0, 0.1f, 0.1f, 0);
            //camera = new SpiralLab.Sirius2.Vision.CameraCL(0,0,"None", "camfile", SpiralLab.Sirius2.Vision.CameraCL.Connector.A, 30, false, 0, 10, 10, 1000);
            //camera.Initialize();
            //visionEditorDisp1.Camera = camera;

            //inspector = new SpiralLab.Sirius2.Vision.InspectorDefault(0, "Inspector");
            //visionEditorDisp1.Inspector = inspector;

            //var doc = new SpiralLab.Sirius2.Vision.DocumentDefault();
            //var proc1 = new SpiralLab.Sirius2.Vision.ProcessCircle("Circle");
            //doc.Add(proc1);

            //var proc2 = new SpiralLab.Sirius2.Vision.ProcessPattern("Pattern");
            //doc.Add(proc2);

            //visionEditorDisp1.InitDocument(doc);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //inspector?.Dispose();
            //camera?.Dispose();
        }
    }
}
