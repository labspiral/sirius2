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
        public Form1()
        {
            // Set language
            EditorHelper.SetLanguage();

            InitializeComponent();

            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize sirius2 library
            EditorHelper.Initialize();

            // Important !!!
            // To do fire OnLoad events at usercontrols (EditorUserControl)
            for (int i = 0; i < tabControl1.TabCount; i++)
                tabControl1.SelectedIndex = i;
            tabControl1.SelectedIndex = 0;

            // index 0 
            int index = 0;

            // Create index 0 devices 
            EditorHelper.CreateDevices(out var rtc0, out var laser0, index);
            // Assign index 0 devices into usercontrol
            siriusEditorUserControl1.Rtc = rtc0;
            siriusEditorUserControl1.Laser = laser0;
            // Create index 0 marker
            EditorHelper.CreateMarker(out var marker0, index);
            // Assign index 0 marker to user control
            siriusEditorUserControl1.Marker = marker0;
            siriusEditorUserControl1.TitleName = $"Laser1";
            var document0 = siriusEditorUserControl1.Document;
            var view0 = siriusEditorUserControl1.View;
            // Create index 0 entities for test
            EditorHelper.CreateTestEntities(rtc0, view0, document0);
            // Assign Document, View, Rtc, Laser into marker at index 0
            marker0.Ready(document0, view0, rtc0, laser0);

            // index 1
            index = 1;
            // Create index 1 devices
            EditorHelper.CreateDevices(out var rtc1, out var laser1, index);
            // Assign index 1 devices into usercontrol
            siriusEditorUserControl2.Rtc = rtc1;
            siriusEditorUserControl2.Laser = laser1;
            // Create index 1 marker
            EditorHelper.CreateMarker(out var marker1, index);
            // Assign index 1 marker to user control
            siriusEditorUserControl2.Marker = marker1;
            siriusEditorUserControl2.TitleName = $"Laser2";
            var document1 = siriusEditorUserControl2.Document;
            var view1 = siriusEditorUserControl2.View;
            // Create index 1 entities for test
            EditorHelper.CreateTestEntities(rtc1, view1, document1);
            // Assign Document, View, Rtc, Laser into marker at index 1
            marker1.Ready(document1, view1, rtc1, laser1);

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // destory index 0 devices
            var document0 = siriusEditorUserControl1.Document;
            var marker0 = siriusEditorUserControl1.Marker;
            var laser0 = siriusEditorUserControl1.Laser;
            var rtc0 = siriusEditorUserControl1.Rtc;
            EditorHelper.DestroyDevices(rtc0, laser0, marker0);
            siriusEditorUserControl1.Rtc = null;
            siriusEditorUserControl1.Laser = null;
            siriusEditorUserControl1.Marker = null;

            // destory index 1 devices
            var document1 = siriusEditorUserControl2.Document;
            var marker1 = siriusEditorUserControl2.Marker;
            var laser1 = siriusEditorUserControl2.Laser;
            var rtc1 = siriusEditorUserControl2.Rtc;
            EditorHelper.DestroyDevices(rtc1, laser1, marker1);
            siriusEditorUserControl2.Rtc = null;
            siriusEditorUserControl2.Laser = null;
            siriusEditorUserControl2.Marker = null;
        }
    }
}
