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
 * Description : Example sirius2 editor
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Mathematics;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;

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

            // Create devices 
            EditorHelper.CreateDevices(out var rtc, out var laser);

            // Assign devices into usercontrol
            siriusEditorUserControl1.Rtc = rtc;
            siriusEditorUserControl1.Laser = laser;

            // Create marker
            EditorHelper.CreateMarker(out var marker);

            // Assign marker to user control
            siriusEditorUserControl1.Marker = marker;

            var document = siriusEditorUserControl1.Document;
            var view = siriusEditorUserControl1.View;

            // Create entities for test. for example: Datamatrix barcode entity
            var dataMatrix = EntityFactory.CreateDataMatrix("SIRIUS2", BarcodeCells.Circles, 3, 4, 4);
            dataMatrix.Name = "MyBarcode1";
            dataMatrix.IsConvertedText = true;
            document.ActAdd(dataMatrix);

            // Create entities for test. for example: text entity
            var text = EntityFactory.CreateText("Arial", $"SIRIUS2", FontStyle.Bold, 1);
            text.Name = "MyText1";
            text.IsConvertedText = true;
            text.Translate(0, -1);
            document.ActAdd(text);

            // Attach event handler for convert barcode text data 
            // Event will be fired when trying to convert with ITextConvertible entity
            SpiralLab.Sirius2.Winforms.Config.OnTextConvert += Text_OnTextConvert;

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser);

            // Assign 6 offset array positions
            var offsets = new List<Offset>()
            {
                new Offset(-20, 10, 0),
                new Offset(0, 10, 0),
                new Offset(20, 10, 0),
                new Offset(-20, -10, 0),
                new Offset(0, -10, 0),
                new Offset(20, -10, 0),
            };
            marker.Offsets = offsets.ToArray();

            // Do mark with offsets at first
            marker.MarkProcedure = MarkProcedures.OffsetFirst;
        }

        private string Text_OnTextConvert(IMarker marker, ITextConvertible textConvertible)
        {
            var entity = textConvertible as IEntity;            
            switch (entity.Name)
            {
                case "MyBarcode1":
                    return $"SIRIUS2 {marker.CurrentOffsetIndex}";
                case "MyText1":
                    return $"SIRIUS2 {DateTime.Now.ToString("HH:mm:ss")} {marker.CurrentOffsetIndex}";
                default:
                    return textConvertible.SourceText;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var document = siriusEditorUserControl1.Document;
            var marker = siriusEditorUserControl1.Marker;
            var laser = siriusEditorUserControl1.Laser;
            var rtc = siriusEditorUserControl1.Rtc;

            if (document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit without save ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            if (rtc.CtlGetStatus(RtcStatus.Busy) ||
                laser.IsBusy ||
                marker.IsBusy)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit during working on progressing... ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            if (e.Cancel == false)
            {
                //SpiralLab.Sirius2.Winforms.Config.OnTextConvert -= Text_OnTextConvert;
                EditorHelper.DestroyDevices(rtc, laser, marker);
                siriusEditorUserControl1.Rtc = null;
                siriusEditorUserControl1.Laser = null;
                siriusEditorUserControl1.Marker = null;
            }
        }
    }
}