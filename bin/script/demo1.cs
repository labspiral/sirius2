// Powered by (c)SpiralLab.Sirius2 with C# script codes
// Written by hcchoi@spirallab.co.kr
// 2023 Coypright to (c)SpiralLab

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Windows.Forms;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Mathematics;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;

public class UserScript 
    : SpiralLab.Sirius2.Winforms.Script.ScriptBase
{
    
    [RefreshProperties(RefreshProperties.All)]
    [Browsable(true)]
    [ReadOnly(false)]
    [Category("Data")]
    [DisplayName("Start")]
    [Description("Starting Serial No")]
    public int StartSerialNo   
	{
		get { return startSerialNo; } 
		set {
			startSerialNo = value;
			NotifyPropertyChanged();
		}
	}
	int startSerialNo;
        
    [RefreshProperties(RefreshProperties.All)]
    [Browsable(true)]
    [ReadOnly(false)]
    [Category("Data")]
    [DisplayName("Current")]
    [Description("Current Serial No")]
    public int SerialNo
	{ 
		get { return serialNo; } 
		set {
			serialNo = value;
			NotifyPropertyChanged();
		}
	}	
	int serialNo;
	
    
    public UserScript()
        : base()
    {
        StartSerialNo = 1000;
        SerialNo = StartSerialNo;
    }       


    public override string OnTextConvert(IMarker marker, ITextConvertible textConvertible)
    {
        var currentEntity = marker.CurrentEntity;
        switch (currentEntity.Name)
        {
            case "MyBarcode1":
            case "MyText1":
                if (SerialNo > 9999)
                    SerialNo = StartSerialNo;
                return string.Format("SERIAL {0:D4}", SerialNo++);
            default:
                // Not modified
                return textConvertible.SourceText;
        }
    }
}       
 