﻿using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Marker;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Demos
{
    internal static class Program
    {        
        public static SiriusEditorUserControl EditorForm { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 1)
                EditorHelper.ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", args[0]);

            // Set language
            EditorHelper.SetLanguage();

            // Create main form
            EditorForm = new SiriusEditorUserControl();

            // Initialize sirius2 library
            EditorHelper.Initialize();

            // To do something after form has shown
            EditorForm.Shown += EditorForm_Shown;

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Main form
            Application.Run(EditorForm);

            // Dispose 
            var rtc = EditorForm.Rtc;
            var laser = EditorForm.Laser;
            var marker = EditorForm.Marker;
            var powerMeter = EditorForm.PowerMeter;
            var remote = EditorForm.Remote;

            EditorHelper.DestroyDevices(rtc, laser, powerMeter, marker, remote);
        }

        private static void EditorForm_Shown(object sender, EventArgs e)
        {
            // Create devices 
            EditorHelper.CreateDevices(out var rtc, out var dInExt1, out var dInLaserPort, out var dOutExt1, out var dOutExt2, out var dOutLaserPort, out var laser, out var powerMeter, out var marker, out var remote, null);

            // Assign devices into usercontrol
            EditorForm.Rtc = rtc;
            EditorForm.Laser = laser;
            EditorForm.PowerMeter = powerMeter;
            EditorForm.Marker = marker;
            EditorForm.Remote = remote;
            EditorForm.DIExt1 = dInExt1;
            EditorForm.DILaserPort = dInLaserPort;
            EditorForm.DOExt1 = dOutExt1;
            EditorForm.DOExt2 = dOutExt2;
            EditorForm.DOLaserPort = dOutLaserPort;

            var document = EditorForm.Document;
            var view = EditorForm.View;
            // Create entities for test
            EditorHelper.CreateTestEntities(rtc, view, document);

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser, powerMeter, remote);
        }
    }
}
