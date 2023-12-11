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
            EditorHelper.CreateDevices(out var rtc0, out var laser0, out var powerMeter0, out var marker0, out var remote0, this.siriusEditorUserControl1, index);
            // Assign index 0 devices into usercontrol
            siriusEditorUserControl1.Rtc = rtc0;
            siriusEditorUserControl1.Laser = laser0;
            siriusEditorUserControl1.Marker = marker0;
            siriusEditorUserControl1.PowerMeter = powerMeter0;
            siriusEditorUserControl1.Remote = remote0;
            siriusEditorUserControl1.TitleName = $"Laser1";
        
            var document0 = siriusEditorUserControl1.Document;
            var view0 = siriusEditorUserControl1.View;
            EditorHelper.CreateTestEntities(rtc0, view0, document0);
            // Assign Document, View, Rtc, Laser into marker at index 0
            marker0.Ready(document0, view0, rtc0, laser0, powerMeter0, remote0);

            // index 1
            index = 1;
            // Create index 1 devices
            EditorHelper.CreateDevices(out var rtc1, out var laser1, out var powerMeter1, out var marker1, out var remote1, this.siriusEditorUserControl2, index);
            // Assign index 1 devices into usercontrol
            siriusEditorUserControl2.Rtc = rtc1;
            siriusEditorUserControl2.Laser = laser1;
            siriusEditorUserControl2.Marker = marker1;
            siriusEditorUserControl2.PowerMeter = powerMeter1;
            siriusEditorUserControl2.Remote = remote1;
            siriusEditorUserControl2.TitleName = $"Laser2";

            var document1 = siriusEditorUserControl2.Document;
            var view1 = siriusEditorUserControl2.View;
            // Create index 1 entities for test
            EditorHelper.CreateTestEntities(rtc1, view1, document1);

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Assign Document, View, Rtc, Laser into marker at index 1
            marker1.Ready(document1, view1, rtc1, laser1, powerMeter1, remote1);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // destory index 0 devices
            var remote0 = siriusEditorUserControl1.Remote;
            var document0 = siriusEditorUserControl1.Document;
            var marker0 = siriusEditorUserControl1.Marker;
            var laser0 = siriusEditorUserControl1.Laser;
            var rtc0 = siriusEditorUserControl1.Rtc;
            var powerMeter0 = siriusEditorUserControl1.PowerMeter;

            siriusEditorUserControl1.Remote = null;
            siriusEditorUserControl1.PowerMeter = null;
            siriusEditorUserControl1.Marker = null;
            siriusEditorUserControl1.Rtc = null;
            siriusEditorUserControl1.Laser = null;
            EditorHelper.DestroyDevices(rtc0, laser0, powerMeter0, marker0, remote0);

            // destory index 1 devices
            var remote1 = siriusEditorUserControl2.Remote;
            var document1 = siriusEditorUserControl2.Document;
            var marker1 = siriusEditorUserControl2.Marker;
            var laser1 = siriusEditorUserControl2.Laser;
            var rtc1 = siriusEditorUserControl2.Rtc;
            var powerMeter1 = siriusEditorUserControl2.PowerMeter;

            siriusEditorUserControl2.Remote = null;
            siriusEditorUserControl2.PowerMeter = null;
            siriusEditorUserControl2.Rtc = null;
            siriusEditorUserControl2.Laser = null;
            siriusEditorUserControl2.Marker = null;
            EditorHelper.DestroyDevices(rtc1, laser1, powerMeter1, marker1, remote1);
        }
    }
}
