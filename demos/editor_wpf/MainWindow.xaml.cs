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
 * Description : Example sirius2 editor at WPF
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.IO;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Common;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.PowerMeter;
using SpiralLab.Sirius2.Winforms.UI;
using System.Windows.Forms;
using Demos;

namespace editor_wpf
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Set language
            EditorHelper.SetLanguage();

            // Comment out the following line to disable visual
            // styles for the hosted Windows Forms control.
            System.Windows.Forms.Application.EnableVisualStyles();

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize sirius2 library
            EditorHelper.Initialize();

            // Set FOV area: WxH, it will be drawn as red rectangle
            SpiralLab.Sirius2.Winforms.Config.ViewFovSize = new System.Drawing.SizeF(100, 100);

            // Create devices 
            EditorHelper.CreateDevices(out var rtc, out var dInExt1, out var dInLaserPort, out var dOutExt1, out var dOutExt2, out var dOutLaserPort, out var laser, out var powerMeter, out var marker, out var remote);

            // Assign devices into usercontrol
            siriusEditor.Laser = laser;
            siriusEditor.Rtc = rtc;
            siriusEditor.Marker = marker;
            siriusEditor.PowerMeter = powerMeter;
            siriusEditor.Remote = remote;
            siriusEditor.DIExt1 = dInExt1;
            siriusEditor.DILaserPort = dInLaserPort;
            siriusEditor.DOExt1 = dOutExt1;
            siriusEditor.DOExt2 = dOutExt2;
            siriusEditor.DOLaserPort = dOutLaserPort;

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Assign Document, View, Rtc, Laser into marker
            var document = siriusEditor.Document;
            var view = siriusEditor.View;

            // Create entities for test
            //EditorHelper.CreateTestEntities(rtc, view, document);

            marker.Ready(document, view, rtc, laser, powerMeter, remote);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var document = siriusEditor.Document;
            var marker = siriusEditor.Marker;
            var laser = siriusEditor.Laser;
            var rtc = siriusEditor.Rtc;
            var powerMeter = siriusEditor.PowerMeter;
            var remote = siriusEditor.Remote;

            if (document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit without save ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    e.Cancel = false;
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (marker.IsBusy)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit during working on progressing... ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            if (e.Cancel == false)
            {
                siriusEditor.Remote = null;
                siriusEditor.PowerMeter = null;
                siriusEditor.Marker = null;
                siriusEditor.Rtc = null;
                siriusEditor.Laser = null;
                EditorHelper.DestroyDevices(rtc, laser, powerMeter, marker, remote);
            }
        }
    }
}
