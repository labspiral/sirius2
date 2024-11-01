using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Vision;
using SpiralLab.Sirius2.Vision.Process;

namespace Demos
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            // Set language
            VisionHelper.SetLanguage();

            InitializeComponent();

            this.Shown += Form2_Shown;
            this.FormClosing += Form2_FormClosing;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            visionEditorDisp1.Inspector?.Dispose();
            visionEditorDisp1.Camera?.Dispose();
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            // Initialize sirius2 library
            VisionHelper.Initialize();

            // Create devices 
            VisionHelper.CreateDevices(out var camera, out var inspector, out var rtc, out var dInExt1, out var dInLaserPort, out var dOutExt1, out var dOutExt2, out var dOutLaserPort, out var laser);

            visionEditorDisp1.Camera = camera;
            visionEditorDisp1.Inspector = inspector;
            visionEditorDisp1.Rtc = rtc;

            var doc = visionEditorDisp1.Document;
            VisionHelper.CreateTestProcesses(doc);

            inspector.Ready(doc, camera);
        }
    }
}
