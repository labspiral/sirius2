
namespace Demos
{
    partial class LaserForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LaserForm));
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnGuide = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblMaxPower = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblIndex = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.txtFrequency = new System.Windows.Forms.TextBox();
            this.txtMaxCurrent = new System.Windows.Forms.TextBox();
            this.txtSetCurrent = new System.Windows.Forms.TextBox();
            this.lblTHGActualTemp = new System.Windows.Forms.Label();
            this.lblSHGActualTemp = new System.Windows.Forms.Label();
            this.lblLaserStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.lblActualCurrent = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.lblDiodeVoltage = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.lblSHGSetTemp = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMonitoringPower = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.lblTHGSetTemp = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.lbLDDHours = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.chbGuide = new System.Windows.Forms.CheckBox();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.pnPEC = new System.Windows.Forms.Panel();
            this.pnGate = new System.Windows.Forms.Panel();
            this.pnPRF = new System.Windows.Forms.Panel();
            this.pnLDD = new System.Windows.Forms.Panel();
            this.pnShutter = new System.Windows.Forms.Panel();
            this.btnPECSource = new System.Windows.Forms.Button();
            this.btnGateSource = new System.Windows.Forms.Button();
            this.btnPRFSource = new System.Windows.Forms.Button();
            this.btnLDD = new System.Windows.Forms.Button();
            this.btnShutter = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(607, 41);
            this.panel2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Customized Laser UI";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnGuide
            // 
            this.pnGuide.BackColor = System.Drawing.Color.Green;
            this.pnGuide.Location = new System.Drawing.Point(102, 647);
            this.pnGuide.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnGuide.Name = "pnGuide";
            this.pnGuide.Size = new System.Drawing.Size(120, 6);
            this.pnGuide.TabIndex = 473;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(16, 57);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(72, 15);
            this.label15.TabIndex = 433;
            this.label15.Text = "Information";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(16, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 15);
            this.label4.TabIndex = 422;
            this.label4.Text = "Control";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel3.Controls.Add(this.lblName, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label12, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.lblMaxPower, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label11, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblIndex, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label9, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(102, 57);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(319, 75);
            this.tableLayoutPanel3.TabIndex = 481;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Location = new System.Drawing.Point(203, 48);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(114, 24);
            this.lblName.TabIndex = 443;
            this.lblName.Text = "NoName";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 15);
            this.label12.TabIndex = 442;
            this.label12.Text = "Name";
            // 
            // lblMaxPower
            // 
            this.lblMaxPower.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMaxPower.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMaxPower.Location = new System.Drawing.Point(203, 24);
            this.lblMaxPower.Name = "lblMaxPower";
            this.lblMaxPower.Size = new System.Drawing.Size(114, 24);
            this.lblMaxPower.TabIndex = 441;
            this.lblMaxPower.Text = "0";
            this.lblMaxPower.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 15);
            this.label11.TabIndex = 440;
            this.label11.Text = "Max Power (W)";
            // 
            // lblIndex
            // 
            this.lblIndex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblIndex.Location = new System.Drawing.Point(203, 0);
            this.lblIndex.Name = "lblIndex";
            this.lblIndex.Size = new System.Drawing.Size(114, 24);
            this.lblIndex.TabIndex = 439;
            this.lblIndex.Text = "0";
            this.lblIndex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 15);
            this.label9.TabIndex = 431;
            this.label9.Text = "Index";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 3;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel6.Controls.Add(this.txtFrequency, 1, 8);
            this.tableLayoutPanel6.Controls.Add(this.txtMaxCurrent, 2, 7);
            this.tableLayoutPanel6.Controls.Add(this.txtSetCurrent, 1, 7);
            this.tableLayoutPanel6.Controls.Add(this.lblTHGActualTemp, 2, 6);
            this.tableLayoutPanel6.Controls.Add(this.lblSHGActualTemp, 2, 5);
            this.tableLayoutPanel6.Controls.Add(this.lblLaserStatus, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.label40, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.lblActualCurrent, 1, 1);
            this.tableLayoutPanel6.Controls.Add(this.label44, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.lblDiodeVoltage, 1, 4);
            this.tableLayoutPanel6.Controls.Add(this.label51, 0, 5);
            this.tableLayoutPanel6.Controls.Add(this.lblSHGSetTemp, 1, 5);
            this.tableLayoutPanel6.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.lblMonitoringPower, 1, 2);
            this.tableLayoutPanel6.Controls.Add(this.label53, 0, 6);
            this.tableLayoutPanel6.Controls.Add(this.lblTHGSetTemp, 1, 6);
            this.tableLayoutPanel6.Controls.Add(this.label36, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.lbLDDHours, 1, 3);
            this.tableLayoutPanel6.Controls.Add(this.label6, 0, 7);
            this.tableLayoutPanel6.Controls.Add(this.label38, 0, 8);
            this.tableLayoutPanel6.Controls.Add(this.label7, 0, 9);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(102, 185);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 11;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(445, 267);
            this.tableLayoutPanel6.TabIndex = 482;
            // 
            // txtFrequency
            // 
            this.txtFrequency.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtFrequency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFrequency.Location = new System.Drawing.Point(203, 196);
            this.txtFrequency.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtFrequency.Name = "txtFrequency";
            this.txtFrequency.ReadOnly = true;
            this.txtFrequency.Size = new System.Drawing.Size(114, 21);
            this.txtFrequency.TabIndex = 501;
            this.txtFrequency.Text = "100";
            this.txtFrequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtMaxCurrent
            // 
            this.txtMaxCurrent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtMaxCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMaxCurrent.Location = new System.Drawing.Point(323, 172);
            this.txtMaxCurrent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMaxCurrent.Name = "txtMaxCurrent";
            this.txtMaxCurrent.ReadOnly = true;
            this.txtMaxCurrent.Size = new System.Drawing.Size(119, 21);
            this.txtMaxCurrent.TabIndex = 499;
            this.txtMaxCurrent.Text = "20";
            this.txtMaxCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtSetCurrent
            // 
            this.txtSetCurrent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtSetCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSetCurrent.Location = new System.Drawing.Point(203, 172);
            this.txtSetCurrent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSetCurrent.Name = "txtSetCurrent";
            this.txtSetCurrent.ReadOnly = true;
            this.txtSetCurrent.Size = new System.Drawing.Size(114, 21);
            this.txtSetCurrent.TabIndex = 497;
            this.txtSetCurrent.Text = "20";
            this.txtSetCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblTHGActualTemp
            // 
            this.lblTHGActualTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTHGActualTemp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTHGActualTemp.Location = new System.Drawing.Point(323, 144);
            this.lblTHGActualTemp.Name = "lblTHGActualTemp";
            this.lblTHGActualTemp.Size = new System.Drawing.Size(119, 24);
            this.lblTHGActualTemp.TabIndex = 495;
            this.lblTHGActualTemp.Text = "0";
            this.lblTHGActualTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSHGActualTemp
            // 
            this.lblSHGActualTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSHGActualTemp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSHGActualTemp.Location = new System.Drawing.Point(323, 120);
            this.lblSHGActualTemp.Name = "lblSHGActualTemp";
            this.lblSHGActualTemp.Size = new System.Drawing.Size(119, 24);
            this.lblSHGActualTemp.TabIndex = 494;
            this.lblSHGActualTemp.Text = "0";
            this.lblSHGActualTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLaserStatus
            // 
            this.lblLaserStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLaserStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLaserStatus.Location = new System.Drawing.Point(203, 0);
            this.lblLaserStatus.Name = "lblLaserStatus";
            this.lblLaserStatus.Size = new System.Drawing.Size(114, 24);
            this.lblLaserStatus.TabIndex = 472;
            this.lblLaserStatus.Text = "Ready";
            this.lblLaserStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 15);
            this.label3.TabIndex = 422;
            this.label3.Text = "Laser Status";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(3, 24);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(102, 15);
            this.label40.TabIndex = 473;
            this.label40.Text = "Actual Current (A)";
            // 
            // lblActualCurrent
            // 
            this.lblActualCurrent.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblActualCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblActualCurrent.Location = new System.Drawing.Point(203, 24);
            this.lblActualCurrent.Name = "lblActualCurrent";
            this.lblActualCurrent.Size = new System.Drawing.Size(114, 24);
            this.lblActualCurrent.TabIndex = 474;
            this.lblActualCurrent.Text = "20";
            this.lblActualCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(3, 96);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(58, 15);
            this.label44.TabIndex = 480;
            this.label44.Text = "Diode (V)";
            // 
            // lblDiodeVoltage
            // 
            this.lblDiodeVoltage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDiodeVoltage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDiodeVoltage.Location = new System.Drawing.Point(203, 96);
            this.lblDiodeVoltage.Name = "lblDiodeVoltage";
            this.lblDiodeVoltage.Size = new System.Drawing.Size(114, 24);
            this.lblDiodeVoltage.TabIndex = 481;
            this.lblDiodeVoltage.Text = "10";
            this.lblDiodeVoltage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(3, 120);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(154, 15);
            this.label51.TabIndex = 478;
            this.label51.Text = "SHG Set / Actual Temp (°C)";
            // 
            // lblSHGSetTemp
            // 
            this.lblSHGSetTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSHGSetTemp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSHGSetTemp.Location = new System.Drawing.Point(203, 120);
            this.lblSHGSetTemp.Name = "lblSHGSetTemp";
            this.lblSHGSetTemp.Size = new System.Drawing.Size(114, 24);
            this.lblSHGSetTemp.TabIndex = 479;
            this.lblSHGSetTemp.Text = "0";
            this.lblSHGSetTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 15);
            this.label2.TabIndex = 482;
            this.label2.Text = "Power Monitoring (W)";
            // 
            // lblMonitoringPower
            // 
            this.lblMonitoringPower.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMonitoringPower.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMonitoringPower.Location = new System.Drawing.Point(203, 48);
            this.lblMonitoringPower.Name = "lblMonitoringPower";
            this.lblMonitoringPower.Size = new System.Drawing.Size(114, 24);
            this.lblMonitoringPower.TabIndex = 483;
            this.lblMonitoringPower.Text = "20";
            this.lblMonitoringPower.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(3, 144);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(153, 15);
            this.label53.TabIndex = 484;
            this.label53.Text = "THG Set / Actual Temp (°C)";
            // 
            // lblTHGSetTemp
            // 
            this.lblTHGSetTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTHGSetTemp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTHGSetTemp.Location = new System.Drawing.Point(203, 144);
            this.lblTHGSetTemp.Name = "lblTHGSetTemp";
            this.lblTHGSetTemp.Size = new System.Drawing.Size(114, 24);
            this.lblTHGSetTemp.TabIndex = 485;
            this.lblTHGSetTemp.Text = "0";
            this.lblTHGSetTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(3, 72);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(63, 15);
            this.label36.TabIndex = 486;
            this.label36.Text = "LDD (Hrs)";
            // 
            // lbLDDHours
            // 
            this.lbLDDHours.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbLDDHours.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLDDHours.Location = new System.Drawing.Point(203, 72);
            this.lbLDDHours.Name = "lbLDDHours";
            this.lbLDDHours.Size = new System.Drawing.Size(114, 24);
            this.lbLDDHours.TabIndex = 487;
            this.lbLDDHours.Text = "10";
            this.lbLDDHours.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 15);
            this.label6.TabIndex = 496;
            this.label6.Text = "Set Current (A)";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(3, 192);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(90, 15);
            this.label38.TabIndex = 498;
            this.label38.Text = "Max Current (A)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 216);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 15);
            this.label7.TabIndex = 500;
            this.label7.Text = "Frequency (KHz)";
            // 
            // chbGuide
            // 
            this.chbGuide.Appearance = System.Windows.Forms.Appearance.Button;
            this.chbGuide.BackColor = System.Drawing.Color.WhiteSmoke;
            this.chbGuide.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chbGuide.Image = ((System.Drawing.Image)(resources.GetObject("chbGuide.Image")));
            this.chbGuide.Location = new System.Drawing.Point(102, 602);
            this.chbGuide.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chbGuide.Name = "chbGuide";
            this.chbGuide.Size = new System.Drawing.Size(120, 42);
            this.chbGuide.TabIndex = 486;
            this.chbGuide.Text = "Guide";
            this.chbGuide.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chbGuide.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.chbGuide.UseVisualStyleBackColor = false;
            // 
            // btnAbort
            // 
            this.btnAbort.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnAbort.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbort.Image = ((System.Drawing.Image)(resources.GetObject("btnAbort.Image")));
            this.btnAbort.Location = new System.Drawing.Point(230, 602);
            this.btnAbort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(120, 42);
            this.btnAbort.TabIndex = 485;
            this.btnAbort.Text = "Abort";
            this.btnAbort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAbort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAbort.UseVisualStyleBackColor = false;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.Image = ((System.Drawing.Image)(resources.GetObject("btnReset.Image")));
            this.btnReset.Location = new System.Drawing.Point(356, 602);
            this.btnReset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 42);
            this.btnReset.TabIndex = 484;
            this.btnReset.Text = "Reset";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReset.UseVisualStyleBackColor = false;
            // 
            // pnPEC
            // 
            this.pnPEC.BackColor = System.Drawing.Color.Green;
            this.pnPEC.Location = new System.Drawing.Point(356, 563);
            this.pnPEC.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnPEC.Name = "pnPEC";
            this.pnPEC.Size = new System.Drawing.Size(120, 6);
            this.pnPEC.TabIndex = 496;
            // 
            // pnGate
            // 
            this.pnGate.BackColor = System.Drawing.Color.Green;
            this.pnGate.Location = new System.Drawing.Point(228, 563);
            this.pnGate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnGate.Name = "pnGate";
            this.pnGate.Size = new System.Drawing.Size(120, 6);
            this.pnGate.TabIndex = 495;
            // 
            // pnPRF
            // 
            this.pnPRF.BackColor = System.Drawing.Color.Green;
            this.pnPRF.Location = new System.Drawing.Point(102, 563);
            this.pnPRF.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnPRF.Name = "pnPRF";
            this.pnPRF.Size = new System.Drawing.Size(120, 6);
            this.pnPRF.TabIndex = 494;
            // 
            // pnLDD
            // 
            this.pnLDD.BackColor = System.Drawing.Color.Green;
            this.pnLDD.Location = new System.Drawing.Point(230, 504);
            this.pnLDD.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnLDD.Name = "pnLDD";
            this.pnLDD.Size = new System.Drawing.Size(120, 6);
            this.pnLDD.TabIndex = 488;
            // 
            // pnShutter
            // 
            this.pnShutter.BackColor = System.Drawing.Color.Green;
            this.pnShutter.Location = new System.Drawing.Point(103, 504);
            this.pnShutter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnShutter.Name = "pnShutter";
            this.pnShutter.Size = new System.Drawing.Size(120, 6);
            this.pnShutter.TabIndex = 487;
            // 
            // btnPECSource
            // 
            this.btnPECSource.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnPECSource.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPECSource.Image = ((System.Drawing.Image)(resources.GetObject("btnPECSource.Image")));
            this.btnPECSource.Location = new System.Drawing.Point(354, 518);
            this.btnPECSource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPECSource.Name = "btnPECSource";
            this.btnPECSource.Size = new System.Drawing.Size(120, 42);
            this.btnPECSource.TabIndex = 493;
            this.btnPECSource.Text = "PEC Source External";
            this.btnPECSource.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPECSource.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPECSource.UseVisualStyleBackColor = false;
            // 
            // btnGateSource
            // 
            this.btnGateSource.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnGateSource.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGateSource.Image = ((System.Drawing.Image)(resources.GetObject("btnGateSource.Image")));
            this.btnGateSource.Location = new System.Drawing.Point(228, 518);
            this.btnGateSource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGateSource.Name = "btnGateSource";
            this.btnGateSource.Size = new System.Drawing.Size(120, 42);
            this.btnGateSource.TabIndex = 492;
            this.btnGateSource.Text = "Gate Source External";
            this.btnGateSource.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGateSource.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGateSource.UseVisualStyleBackColor = false;
            // 
            // btnPRFSource
            // 
            this.btnPRFSource.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnPRFSource.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPRFSource.Image = ((System.Drawing.Image)(resources.GetObject("btnPRFSource.Image")));
            this.btnPRFSource.Location = new System.Drawing.Point(102, 518);
            this.btnPRFSource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPRFSource.Name = "btnPRFSource";
            this.btnPRFSource.Size = new System.Drawing.Size(120, 42);
            this.btnPRFSource.TabIndex = 491;
            this.btnPRFSource.Text = "PRF Source External";
            this.btnPRFSource.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPRFSource.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPRFSource.UseVisualStyleBackColor = false;
            // 
            // btnLDD
            // 
            this.btnLDD.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnLDD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLDD.Image = ((System.Drawing.Image)(resources.GetObject("btnLDD.Image")));
            this.btnLDD.Location = new System.Drawing.Point(230, 459);
            this.btnLDD.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnLDD.Name = "btnLDD";
            this.btnLDD.Size = new System.Drawing.Size(120, 42);
            this.btnLDD.TabIndex = 490;
            this.btnLDD.Text = "LDD";
            this.btnLDD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLDD.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLDD.UseVisualStyleBackColor = false;
            // 
            // btnShutter
            // 
            this.btnShutter.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnShutter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShutter.Image = ((System.Drawing.Image)(resources.GetObject("btnShutter.Image")));
            this.btnShutter.Location = new System.Drawing.Point(102, 459);
            this.btnShutter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnShutter.Name = "btnShutter";
            this.btnShutter.Size = new System.Drawing.Size(120, 42);
            this.btnShutter.TabIndex = 489;
            this.btnShutter.Text = "Shutter";
            this.btnShutter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnShutter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnShutter.UseVisualStyleBackColor = false;
            // 
            // LaserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.pnPEC);
            this.Controls.Add(this.pnGate);
            this.Controls.Add(this.pnPRF);
            this.Controls.Add(this.pnLDD);
            this.Controls.Add(this.pnShutter);
            this.Controls.Add(this.btnPECSource);
            this.Controls.Add(this.btnGateSource);
            this.Controls.Add(this.btnPRFSource);
            this.Controls.Add(this.btnLDD);
            this.Controls.Add(this.btnShutter);
            this.Controls.Add(this.chbGuide);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.tableLayoutPanel6);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.pnGuide);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LaserForm";
            this.Size = new System.Drawing.Size(607, 709);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnGuide;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblMaxPower;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblIndex;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Label lblLaserStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label lblActualCurrent;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label lblDiodeVoltage;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblMonitoringPower;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Label lblTHGSetTemp;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label lbLDDHours;
        private System.Windows.Forms.Label lblTHGActualTemp;
        private System.Windows.Forms.Label lblSHGActualTemp;
        private System.Windows.Forms.Label lblSHGSetTemp;
        private System.Windows.Forms.CheckBox chbGuide;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Panel pnPEC;
        private System.Windows.Forms.Panel pnGate;
        private System.Windows.Forms.Panel pnPRF;
        private System.Windows.Forms.Panel pnLDD;
        private System.Windows.Forms.Panel pnShutter;
        private System.Windows.Forms.Button btnPECSource;
        private System.Windows.Forms.Button btnGateSource;
        private System.Windows.Forms.Button btnPRFSource;
        private System.Windows.Forms.Button btnLDD;
        private System.Windows.Forms.Button btnShutter;
        private System.Windows.Forms.TextBox txtFrequency;
        private System.Windows.Forms.TextBox txtMaxCurrent;
        private System.Windows.Forms.TextBox txtSetCurrent;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label7;
    }
}