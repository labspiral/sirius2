// Powered by (c)SpiralLab.Sirius2 with C# script codes
// Written by hcchoi@spirallab.co.kr
// 2023 Coypright to (c)SpiralLab

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Laser;
using SpiralLab.Sirius2.Mathematics;
using SpiralLab.Sirius2.PowerMeter;
using SpiralLab.Sirius2.Scanner;
using SpiralLab.Sirius2.Scanner.Rtc;
using SpiralLab.Sirius2.Winforms;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.Remote;
using SpiralLab.Sirius2.Winforms.Script;
using OpenTK;

public class UserScript : MarkerScriptBase
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
    [Description("Current(or Next) Serial No")]
    public int SerialNo
	{ 
		get { return serialNo; } 
		set {
			serialNo = value;
			NotifyPropertyChanged();
		}
	}	
	int serialNo;
	
    
	public UserScript(IMarker marker)
		: base(marker)
    {
		Name = "Demo1.cs";
		Description = "This is a sample user script";
		
        StartSerialNo = 1000;
        SerialNo = StartSerialNo;
		
		Logger.Log(Logger.Types.Trace, "script [{0}]: instance has created", Name);
    }       

    public override string OnTextConvert(IMarker marker, ITextConvertible textConvertible)
    {
		var index = marker.Index;
		var currentLayer = marker.CurrentLayer;
		var currentLayerIndex = marker.CurrentLayerIndex;
		//var currentEntity = textConvertible as IEntity;
		var currentEntity = marker.CurrentEntity;
		var currentEntityIndex = marker.CurrentEntityIndex;
		var currentOffset = marker.CurrentOffset;
		var currentOffsetIndex = marker.CurrentOffsetIndex;
		
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
 