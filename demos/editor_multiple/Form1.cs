/*
 * 
 *                                                            ,--,      ,--,                              
 *             ,-.----.                                     ,---.'|   ,---.'|                              
 *   .--.--.   \    /  \     ,---,,-.----.      ,---,       |   | :   |   | :      ,---,           ,---,.  
 *  /  /    '. |   :    \ ,`--.' |\    /  \    '  .' \      :   : |   :   : |     '  .' \        ,'  .'  \ 
 * |  :  /`. / |   |  .\ :|   :  :;   :    \  /  ;    '.    |   ' :   |   ' :    /  ;    '.    ,---.' .' | 
 * ;  |  |--`  .   :  |: |:   |  '|   | .\ : :  :       \   ;   ; '   ;   ; '   :  :       \   |   |  |: | 
 * |  :  ;_    |   |   \ :|   :  |.   : |: | :  |   /\   \  '   | |__ '   | |__ :  |   /\   \  :   :  :  / 
 *  \  \    `. |   : .   /'   '  ;|   |  \ : |  :  ' ;.   : |   | :.'||   | :.'||  :  ' ;.   : :   |    ;  
 *   `----.   \;   | |`-' |   |  ||   : .  / |  |  ;/  \   \'   :    ;'   :    ;|  |  ;/  \   \|   :     \ 
 *   __ \  \  ||   | ;    '   :  ;;   | |  \ '  :  | \  \ ,'|   |  ./ |   |  ./ '  :  | \  \ ,'|   |   . | 
 *  /  /`--'  /:   ' |    |   |  '|   | ;\  \|  |  '  '--'  ;   : ;   ;   : ;   |  |  '  '--'  '   :  '; | 
 * '--'.     / :   : :    '   :  |:   ' | \.'|  :  :        |   ,/    |   ,/    |  :  :        |   |  | ;  
 *   `--'---'  |   | :    ;   |.' :   : :-'  |  | ,'        '---'     '---'     |  | ,'        |   :   /   
 *             `---'.|    '---'   |   |.'    `--''                              `--''          |   | ,'    
 *               `---`            `---'                                                        `----'   
 * 
 * 2023 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Example sirius2 editor for multiple RTC instances
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

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
            EditorHelper.CreateDevices(out var rtc0, out var dInExt10, out var dInLaserPort0, out var dOutExt10, out var dOutExt20, out var dOutLaserPort0, out var laser0, out var powerMeter0, out var marker0, out var remote0, index);

            // Assign index 0 devices into usercontrol
            siriusEditorUserControl1.Rtc = rtc0;
            siriusEditorUserControl1.Laser = laser0;
            siriusEditorUserControl1.Marker = marker0;
            siriusEditorUserControl1.PowerMeter = powerMeter0;
            siriusEditorUserControl1.Remote = remote0;
            siriusEditorUserControl1.DIExt1 = dInExt10;
            siriusEditorUserControl1.DILaserPort = dInLaserPort0;
            siriusEditorUserControl1.DOExt1 = dOutExt10;
            siriusEditorUserControl1.DOExt2 = dOutExt20;
            siriusEditorUserControl1.DOLaserPort = dOutLaserPort0;
            siriusEditorUserControl1.TitleName = $"Laser1";
        
            var document0 = siriusEditorUserControl1.Document;
            var view0 = siriusEditorUserControl1.View;
            EditorHelper.CreateTestEntities(rtc0, document0);
            // Assign Document, View, Rtc, Laser into marker at index 0
            marker0.Ready(document0, view0, rtc0, laser0, powerMeter0, remote0);

            // index 1
            index = 1;
            // Create index 1 devices
            EditorHelper.CreateDevices(out var rtc1, out var dInExt11, out var dInLaserPort1, out var dOutExt11, out var dOutExt21, out var dOutLaserPort1, out var laser1, out var powerMeter1, out var marker1, out var remote1, index);

            // Assign index 1 devices into usercontrol
            siriusEditorUserControl2.Rtc = rtc1;
            siriusEditorUserControl2.Laser = laser1;
            siriusEditorUserControl2.Marker = marker1;
            siriusEditorUserControl2.PowerMeter = powerMeter1;
            siriusEditorUserControl2.Remote = remote1;
            siriusEditorUserControl2.DIExt1 = dInExt11;
            siriusEditorUserControl2.DILaserPort = dInLaserPort1;
            siriusEditorUserControl2.DOExt1 = dOutExt11;
            siriusEditorUserControl2.DOExt2 = dOutExt21;
            siriusEditorUserControl2.DOLaserPort = dOutLaserPort1;
            siriusEditorUserControl2.TitleName = $"Laser2";

            var document1 = siriusEditorUserControl2.Document;
            var view1 = siriusEditorUserControl2.View;
            // Create index 1 entities for test
            EditorHelper.CreateTestEntities(rtc1, document1);

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Assign Document, View, Rtc, Laser into marker at index 1
            marker1.Ready(document1, view1, rtc1, laser1, powerMeter1, remote1);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var remote0 = siriusEditorUserControl1.Remote;
            var document0 = siriusEditorUserControl1.Document;
            var marker0 = siriusEditorUserControl1.Marker;
            var laser0 = siriusEditorUserControl1.Laser;
            var rtc0 = siriusEditorUserControl1.Rtc;
            var powerMeter0 = siriusEditorUserControl1.PowerMeter;
          
            var remote1 = siriusEditorUserControl2.Remote;
            var document1 = siriusEditorUserControl2.Document;
            var marker1 = siriusEditorUserControl2.Marker;
            var laser1 = siriusEditorUserControl2.Laser;
            var rtc1 = siriusEditorUserControl2.Rtc;
            var powerMeter1 = siriusEditorUserControl2.PowerMeter;

            if (document0.IsModified || document1.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit without save ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (marker0.IsBusy || marker1.IsBusy)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit during working on progressing... ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            // destory index 0 devices
            siriusEditorUserControl1.Remote = null;
            siriusEditorUserControl1.PowerMeter = null;
            siriusEditorUserControl1.Marker = null;
            siriusEditorUserControl1.Rtc = null;
            siriusEditorUserControl1.Laser = null;
            EditorHelper.DestroyDevices(rtc0, laser0, powerMeter0, marker0, remote0);

            // destory index 1 devices
            siriusEditorUserControl2.Remote = null;
            siriusEditorUserControl2.PowerMeter = null;
            siriusEditorUserControl2.Rtc = null;
            siriusEditorUserControl2.Laser = null;
            siriusEditorUserControl2.Marker = null;
            EditorHelper.DestroyDevices(rtc1, laser1, powerMeter1, marker1, remote1);
        }
    }
}
