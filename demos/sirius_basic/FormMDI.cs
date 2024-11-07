using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Mathematics;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Vision.Camera;
using SpiralLab.Sirius2.Vision.Common;
using SpiralLab.Sirius2.Vision.Inspector;
using SpiralLab.Sirius2.Vision.Process;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.UI;

namespace Demos
{
    public partial class FormMDI : Form
    {
        Form1 Editor { get; set; }
        Form2 Vision { get; set; }
        FormLog Log { get; set; }

        public FormMDI()
        {
            // Set language
            SiriusHelper.SetLanguage();

            InitializeComponent();

            // Initialize sirius2 library
            SiriusHelper.Initialize();

            this.Shown += FormMDI_Shown;
            this.FormClosing += FormMDI_FormClosing;

            // Create rtc, laser, powermeter, marker, dio ..., camera devices 
            SiriusHelper.CreateDevices(out var rtc, out var dInExt1, out var dInLaserPort, out var dOutExt1, out var dOutExt2, out var dOutLaserPort, out var laser, out var powerMeter, out var marker, out var remote, out var camera, out var inspector);
            Editor = new Form1(rtc, laser, marker, powerMeter, remote, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort);
            Vision = new Form2(camera, inspector);
        }

        private void FormMDI_Shown(object sender, EventArgs e)
        {
            Log = new FormLog();
            //Log.MdiParent = this;
            Log.Show();
            Log.Hide();

            Editor.MdiParent = this;
            Editor.Control.IsShowLogCtrl = false;
            Editor.Show();

            Vision.MdiParent = this;
            Vision.Control.IsShowLogCtrl = false;
            Vision.Show();

            //Editor.WindowState = FormWindowState.Maximized;
            LayoutMdi(MdiLayout.Cascade);
            //Editor.BringToFront();

            InspectorResultsHandler(Vision.Control.Inspector, Editor.Control.Marker);
        }
        private void FormMDI_FormClosing(object sender, FormClosingEventArgs e)
        {
            var form = new SpiralLab.Sirius2.Winforms.UI.MessageBox($"Do you really want to exit ?", "Warning", MessageBoxButtons.YesNo);
            DialogResult dialogResult = form.ShowDialog(this);
            if (dialogResult == DialogResult.Yes)
                e.Cancel = false;
            else
            {
                e.Cancel = true;
                return;
            }

            if (e.Cancel == false)
            {
                Editor.Cleanup();
                Editor.Dispose();
                //Editor.Close();

                Vision.Cleanup();
                Vision.Dispose();
                //Vision.Close();
            }
        }
        private void logScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (logScreenToolStripMenuItem.Checked)
            {
                Log.Show();
                Log.BringToFront();
            }
            else
            {
                Log.Hide();
            }
        }

        void InspectorResultsHandler(IInspector inspector, IMarker marker)
        {
            inspector.Tag = marker;
            inspector.OnEnded += Inspector_OnEnded;
        }
        private void Inspector_OnEnded(IInspector inspector, InspectorArg arg)
        {
            if (chbInspectionResultsIntoMarkerOffsets.Checked)
            {
                var marker = inspector.Tag as IMarker;
                Debug.Assert(marker != null);
                if (arg.OverAllResult)
                {
                    if (ConvertToOffset(arg, out List<SpiralLab.Sirius2.Mathematics.Offset> offsets))
                        marker.Offsets = offsets.ToArray();
                }
            }

            if (chbInspectionResultsIntoScannerFieldCorrection.Checked)
            {
                if (arg.OverAllResult)
                {
                    if (ConvertToOffset(arg, out List<SpiralLab.Sirius2.Mathematics.Offset> offsets))
                    {
                        int rows = 11;
                        int cols = 11;
                        double rowInterval = 10;
                        double colInterval = 10;
                        var kFactor = Editor.Control.Rtc.KFactor;
                        string fileName = Path.Combine(SpiralLab.Sirius2.Config.CorrectionPath, "scannerfieldcorrection2d.txt");
                        if (offsets.Count != rows * cols)
                        {
                            System.Windows.Forms.MessageBox.Show($"Inspection result data counts are not matched", "Scanner Field Correction 2D", MessageBoxButtons.OK);
                            return;
                        }
                        //sort result x, y data by y ASC and then x ASC order
                        var sorted = new List<Offset>(offsets.Count);
                        var tempList = offsets.OrderBy(r => ((Offset)r).Dy).ToList();
                        for (int r = 0; r < rows; r++)
                        {
                            var colList = tempList.Skip(r * cols).Take(cols).OrderBy(rr => ((Offset)rr).Dx).ToList();
                            sorted.AddRange(colList);
                        }

                        // create tuple reference x, y and founded(or measured) x, y positions
                        var tuple = new List<(double, double, double, double)>(rows * cols);
                        double left = -colInterval * (float)(int)(cols / 2);
                        double top = rowInterval * (float)(int)(rows / 2);
                        int i = 0;
                        for (int row = 0; row < rows; row++)
                        {
                            for (int col = 0; col < cols; col++)
                            {
                                tuple.Add((left + col * colInterval, top - row * rowInterval, sorted[i].Dx, sorted[i].Dy));
                                i++;
                            }
                        }

                        // save into file
                        SaveCorrectionFileFormat(fileName, rows, cols, rowInterval, colInterval, kFactor, tuple);
                        Process.Start("notepad.exe", fileName);
                    }
                }
            }
        }
        bool ConvertToOffset(InspectorArg arg, out List<SpiralLab.Sirius2.Mathematics.Offset> offsets)
        {
            offsets = new List<SpiralLab.Sirius2.Mathematics.Offset>(arg.Results.Count);
            foreach (var result in arg.Results)
            {
                if (result is IResultToOffset resultOffset)
                {
                    offsets.Add(resultOffset.ToOffset);
                }
            }
            return offsets.Count > 0;
        }
        void SaveCorrectionFileFormat(string fileName, int rows, int cols, double rowInterval, double colInterval, double kFactor, List<(double, double, double, double)> tuple)
        {
            Debug.Assert(rows > 0);
            Debug.Assert(cols > 0);
            Debug.Assert(rowInterval > 0);
            Debug.Assert(colInterval > 0);
            Debug.Assert(kFactor > 0);
            int i = 0;
            using (var stream = new StreamWriter(fileName))
            {
                stream.WriteLine($"; 2024 Copyright to (c)SpiralLAB");
                stream.WriteLine($"; Data format : row, col, reference, measured");
                stream.WriteLine($"# Correction data counts, {rows}, {cols}, {rowInterval}, {colInterval}, {kFactor}");
                for (int row = 0; row < rows; ++row)
                {
                    for (int col = 0; col < cols; ++col)
                    {
                        stream.WriteLine($"{row}, {col} : {tuple[i].Item1:F6}, {tuple[i].Item2:F6}, {tuple[i].Item3:F6}, {tuple[i].Item4:F6}");
                        i++;
                    }
                }
            }
        }
    
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor.Show();
            Vision.Show();
            LayoutMdi(MdiLayout.Cascade);
        }
        private void verticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor.Show();
            Vision.Show();
            LayoutMdi(MdiLayout.TileVertical); 
        }
        private void horizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor.Show();
            Vision.Show();
            LayoutMdi(MdiLayout.TileHorizontal);
        }
        private void editorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor.Show();
            Editor.WindowState = FormWindowState.Maximized;
            Editor.BringToFront();
        }
        private void visionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Vision.Show();
            Vision.WindowState = FormWindowState.Maximized;
            Vision.BringToFront();
        }

    }
}
