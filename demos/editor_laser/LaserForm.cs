/*
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
 * Copyright (C) 2019-2023 SpiralLab. All rights reserved. 
 * Customized Laser Source UserControl
 * Description : 레이저 소스 폼
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
using SpiralLab.Sirius2.Laser;

namespace Demos
{
    /// <summary>
    /// Your laser usercontrol 
    /// </summary>
    public partial class LaserForm : UserControl
    {
        /// <summary>
        /// <see cref="ILaser">ILaser</see>
        /// </summary>
        public ILaser LaserSource { get; set; }

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        /// <summary>
        /// Constructor
        /// </summary>
        public LaserForm()
        {
            InitializeComponent();

            timer.Interval = 500;
            timer.Tick += Timer_Tick;
            VisibleChanged += LaserForm_VisibleChanged;

            chbGuide.CheckedChanged += chbGuide_CheckedChanged;
            btnAbort.Click += btnAbort_Click;
            btnReset.Click += btnReset_Click;
        }

        private void LaserForm_VisibleChanged(object sender, EventArgs e)
        {
            this.timer.Enabled = this.Visible;
            if (this.Visible)
            {
                if (null != LaserSource)
                {
                    chbGuide.Enabled = LaserSource.IsGuideControl;
                    if (LaserSource is ILaserGuideControl laserGuideControl)
                        chbGuide.Checked = laserGuideControl.IsGuideOn;
                }
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            this.UpdateStatus();
        }
        private void UpdateStatus()
        {
            if (null == LaserSource)
                return;
            this.lblIndex.Text = $"{LaserSource.Index}";
            this.lblName.Text = $"{LaserSource.Name}";
            this.lblMaxPower.Text = $"{LaserSource.MaxPowerWatt:F1} W";
            
            if(LaserSource.IsBusy)
                this.lblLaserStatus.Text = "Busy";
            else if(LaserSource.IsReady)
                this.lblLaserStatus.Text = "Ready";
            else if (LaserSource.IsError)
                this.lblLaserStatus.Text = "Error";
            else
                this.lblLaserStatus.Text = "Unknown";

            if (LaserSource is ILaserGuideControl laserGuideControl)
            {
                chbGuide.Checked = laserGuideControl.IsGuideOn;
                if (laserGuideControl.IsGuideOn)
                    pnGuide.BackColor = Color.Lime;
                else
                    pnGuide.BackColor = Color.Green;
            }
        }
        
        private void chbGuide_CheckedChanged(object sender, EventArgs e)
        {
            if (null == LaserSource)
                return;
            if (LaserSource is ILaserGuideControl laserGuideControl)
                laserGuideControl.CtlGuide(chbGuide.Checked);
        }
        private void btnAbort_Click(object sender, EventArgs e)
        {
            LaserSource?.CtlAbort();
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            LaserSource?.CtlReset();
        }
    }
}
