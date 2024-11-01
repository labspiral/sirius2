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
 * Description : Example sirius2 editor for block and block insert
 * Laser drilling for PoP(Package on package) : https://smtnet.com/news/index.cfm?fuseaction=view_news&company_id=41637&news_id=9373
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
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Mathematics;
using SpiralLab.Sirius2.PowerMeter;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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

            // Set FOV area: WxH, it will be drawn as red rectangle
            SpiralLab.Sirius2.Winforms.Config.ViewFovSize = new SizeF(100, 100);

            // Create devices 
            EditorHelper.CreateDevices(out var rtc, out var dInExt1, out var dInLaserPort, out var dOutExt1, out var dOutExt2, out var dOutLaserPort, out var laser, out var powerMeter, out var marker, out var remote);

            // Assign devices into usercontrol
            siriusEditorUserControl1.Rtc = rtc;
            siriusEditorUserControl1.Laser = laser;
            siriusEditorUserControl1.Marker = marker;
            siriusEditorUserControl1.PowerMeter = powerMeter;
            siriusEditorUserControl1.Remote = remote;
            siriusEditorUserControl1.DIExt1 = dInExt1;
            siriusEditorUserControl1.DILaserPort = dInLaserPort;
            siriusEditorUserControl1.DOExt1 = dOutExt1;
            siriusEditorUserControl1.DOExt2 = dOutExt2;
            siriusEditorUserControl1.DOLaserPort = dOutLaserPort;

            var document = siriusEditorUserControl1.Document;
            var view = siriusEditorUserControl1.View;

            // To draw(or render) very small arcs with high resolution 
            // Edit: 0.020mm (default : 0.3mm)
            SpiralLab.Sirius2.Config.DivideArcToLinesDistance = 0.02;

            // Color of index '0' is White
            Color penColor = SpiralLab.Sirius2.Winforms.Config.PenColors[0];
            Color penColor2 = SpiralLab.Sirius2.Winforms.Config.PenColors[2];

            // Create drill hole
            // Reference link : https://smtnet.com/news/index.cfm?fuseaction=view_news&company_id=41637&news_id=9373
            var hole1 = EntityFactory.CreateArc(0, 0, 0.08, 0, 360);
            hole1.Color = penColor;
            var hole2 = EntityFactory.CreateArc(0, 0, 0.12, 60, 360);
            hole2.Color = penColor;
            var hole3 = EntityFactory.CreateArc(0, 0, 0.14, 120, 360);
            hole3.Color = penColor;
            var hole4 = EntityFactory.CreateArc(0, 0, 0.16, 180, 360);
            hole4.Color = penColor2;
            var hole5 = EntityFactory.CreateArc(0, 0, 0.18, 240, 360);
            hole5.Color = penColor;

            // Create hold as block entity
            var block = EntityFactory.CreateBlock("Hole", new IEntity[] { hole1, hole2, hole3, hole4, hole5 });
            document.ActAdd(block);

            // Create block inserts            
            const int rows = 8;
            const int cols = 10;
            // pitch 400um
            float interval = 0.45f;
            var targets = new List<IEntity>(rows * cols);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // . . . . . . . . . .
                    // . . . . . . . . . .
                    // . . . . . . . . . .
                    // . . . . . . . . . .
                    // . . . . . . . . . .
                    // . . . . . . . . . .
                    // . . . . . . . . . .
                    // . . . . . . . . . .

                    double x = -interval * (float)(cols - 1) / 2.0;
                    double y = interval * (float)(rows - 1) / 2.0;
                    var blockInsert = EntityFactory.CreateBlockInsert("Hole", 
                        new Vector3((float)(x + col * interval), (float)(y - row * interval), 0));
                    document.ActAdd(blockInsert);
                    targets.Add(blockInsert);
                }
            }

            // Convert holes as group entity
            document.ActGroup(targets.ToArray());

            // Zoom to fit by manually
            var bbox = BoundingBox.RealBoundingBox(document.ActiveLayer);
            siriusEditorUserControl1.View.Camera.ZoomFit(bbox);

            // Set pen parameters by manually
            bool founded = document.FindByPenColor(penColor, out var pen1);
            Debug.Assert(founded);
            pen1.JumpSpeed = 1000;
            pen1.MarkSpeed = 1000;
            pen1.Power = pen1.PowerMax * 0.5; // 50% power

            bool founded2 = document.FindByPenColor(penColor2, out var pen2);
            Debug.Assert(founded2);
            pen2.JumpSpeed = 500;
            pen2.MarkSpeed = 1000;
            pen2.Power = pen1.PowerMax * 0.3; // 30% power

            // Assign event handlers at Config
            EditorHelper.AttachEventHandlers();

            // Assign Document, View, Rtc, Laser into marker
            marker.Ready(document, view, rtc, laser, powerMeter, remote);

            // Assign 6 offsets positions
            //
            //    .          .          .
            // -20,10       0,10      20,10
            //
            //    .          .          .
            // -20,-10      0,-10     20,-10
            //
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

            // Do mark procedure as offset first
            if (marker is MarkerRtc markerRtc)
                markerRtc.MarkProcedure = MarkerRtc.MarkProcedures.OffsetFirst;
            else if (marker is MarkerSyncAxis markerRtcSyncAxis)
                markerRtcSyncAxis.MarkProcedure = MarkerSyncAxis.MarkProcedures.OffsetFirst;
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
                {
                    e.Cancel = true;
                    return;
                }
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
