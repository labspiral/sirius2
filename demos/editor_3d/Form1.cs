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
            EditorHelper.CreateDevices(out var rtc, out var laser, out var powerMeter, out var marker, out var remote, this.siriusEditorUserControl1);

            // Assign devices into usercontrol
            siriusEditorUserControl1.Rtc = rtc;
            siriusEditorUserControl1.Laser = laser;
            siriusEditorUserControl1.Marker = marker;
            siriusEditorUserControl1.PowerMeter = powerMeter;
            siriusEditorUserControl1.Remote = remote;

            var document = siriusEditorUserControl1.Document;
            var view = siriusEditorUserControl1.View;

            float zUpper = 2;
            float zLower = -2;

            // Create entities for test
            var line1 = EntityFactory.CreateLine(new Vector3(-5, 5, zUpper), new Vector3(5, 5, zUpper));
            document.ActAdd(line1);
            var line2 = EntityFactory.CreateLine(new Vector3(-5, -5, zLower), new Vector3(5, -5, zLower));
            document.ActAdd(line2);
            var line3 = EntityFactory.CreateLine(new Vector3(-5, 5, zUpper), new Vector3(-5, -5, zUpper));
            document.ActAdd(line3);
            var line4 = EntityFactory.CreateLine(new Vector3(5, 5, zLower), new Vector3(5, -5, zLower));
            document.ActAdd(line4);

            float rotateY = 0;
            for (float z = zLower; z <= zUpper; z += 0.5f)
            {
                var arc = EntityFactory.CreateArc(new Vector3(-5, 5, z), 2.5, 0, 360);
                arc.RotateY(rotateY);
                document.ActAdd(arc);
                rotateY -= 2;
            }
            float rotateZ = 0;
            for (float z = zLower; z <= zUpper; z += 0.5f)
            {
                var rect = EntityFactory.CreateRectangle(new Vector3(5, 5, z), 2.5, 1);
                rect.RotateZ(rotateZ);
                document.ActAdd(rect);
                rotateZ += 10;
            }
            float rotateX = 0;
            for (float z = zLower; z <= zUpper; z += 0.5f)
            {
                var trepan = EntityFactory.CreateTrepan(new Vector3(5, -5, zLower), 1, 2.5, 5);
                trepan.RotateX(rotateX);
                document.ActAdd(trepan);
                rotateX += 2;
            }

            var spiral = EntityFactory.CreateSpiral(new Vector3(-5, -5, zLower), 2, 2.5, zUpper - zLower, 20, true);
            document.ActAdd(spiral);

            var text = EntityFactory.CreateText("Arial", $"SIRIUS2", FontStyle.Bold, 1);
            text.Name = "MyText1";
            text.Rotate(0, 30, 0);
            text.Translate(0, 0, zUpper);
            document.ActAdd(text);

            var dataMatrix = EntityFactory.CreateDataMatrix("SIRIUS2", Barcode2DCells.Circles, 3, 4, 4);
            dataMatrix.Name = "MyBarcode1";
            dataMatrix.Rotate(0, 30, 0);
            dataMatrix.Translate(0, 2, zUpper);
            document.ActAdd(dataMatrix);

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser, powerMeter);

            // Edit camera look at position
            // Press 'CTRL+R' to reset camera
            view.Camera.Position = new Vector3(30, -30, 200);
            view.Camera.LookAt(Vector3.Zero);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var document = siriusEditorUserControl1.Document;
            var marker = siriusEditorUserControl1.Marker;
            var laser = siriusEditorUserControl1.Laser;
            var rtc = siriusEditorUserControl1.Rtc;
            var powerMeter = siriusEditorUserControl1.PowerMeter;
            var remote = siriusEditorUserControl1.Remote;

            if (document.IsModified)
            {
                var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit without save ?", "Warning", MessageBoxButtons.YesNo);
                DialogResult dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }

            if (marker.IsBusy)
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
                siriusEditorUserControl1.Remote = null;
                siriusEditorUserControl1.PowerMeter = null;
                siriusEditorUserControl1.Marker = null;
                siriusEditorUserControl1.Rtc = null;
                siriusEditorUserControl1.Laser = null;
                EditorHelper.DestroyDevices(rtc, laser, powerMeter, marker, remote);
            }
        }
    }
}
