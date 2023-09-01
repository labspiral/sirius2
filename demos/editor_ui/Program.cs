using SpiralLab.Sirius2.Laser;
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
        /// <summary>
        /// Your config ini file
        /// </summary>
        public static string ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
        // Used this config file if using XL-SCAN (syncAXIS)
        // public static string IniFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config_syncaxis.ini");

        public static SiriusEditorUserControl EditorForm { get; set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set language
            EditorHelper.SetLanguage();

            // Create main form
            EditorForm = new SiriusEditorUserControl();

            // Initialize sirius2 library
            EditorHelper.Initialize();

            // To do something after form has shown
            EditorForm.Shown += EditorForm_Shown;

            // Main form
            Application.Run(EditorForm);

            // Dispose 
            var rtc = EditorForm.Rtc;
            var laser = EditorForm.Laser;
            var marker = EditorForm.Marker;
            EditorHelper.DestroyDevices(rtc, laser, marker);
        }

        private static void EditorForm_Shown(object sender, EventArgs e)
        {
            // Create devices 
            EditorHelper.CreateDevices(out var rtc, out var laser);

            // Assign devices into usercontrol
            EditorForm.Rtc = rtc;
            EditorForm.Laser = laser;

            // Create marker
            EditorHelper.CreateMarker(out var marker);

            // Assign marker to user control
            EditorForm.Marker = marker;

            var document = EditorForm.Document;
            var view = EditorForm.View;
            // Create entities for test
            EditorHelper.CreateTestEntities(rtc, view, document);

            // Assign marker to user control
            EditorForm.Marker = marker;

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser);
        }

    }
}
