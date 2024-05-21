// Powered by (c)SpiralLab.Sirius2 with C# script codes
// Written by hcchoi@spirallab.co.kr
// 2023 Coypright to (c)SpiralLab
#region
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
#endregion

public class UserScript : MarkerScriptBase
{    
	[Browsable(true)]
    [ReadOnly(false)]
    [Category("Serial")]
    [DisplayName("Start")]
    [Description("Starting Serial No")]
    public uint StartSerialNo   
    {
        get { return startSerialNo; } 
        set {
            startSerialNo = value;
            NotifyPropertyChanged();
        }
    }
    uint startSerialNo;
    
    [Browsable(true)]
    [ReadOnly(false)]
    [Category("Serial")]
    [DisplayName("Max")]
    [Description("Max Serial No")]
    public uint MaxSerialNo { get; set; }
        
    [Browsable(true)]
    [ReadOnly(false)]
    [Category("Serial")]
    [DisplayName("Current")]
    [Description("Current(or Next) Serial No")]
    public uint SerialNo 
    {
        get { return serialNo; } 
        set {
            serialNo = value;
            NotifyPropertyChanged();
        }
    }   
    uint serialNo;
     
	 
	 
    public UserScript(IMarker marker)
        : base(marker)
    {
        Name = "mof_text.cs";
        Description = "out of ranged text and incremented serial no";
        Logger.Log(Logger.Types.Trace, "script [{0}]: instance has created", Name);
        		        
        StartSerialNo = 1;        
        SerialNo = 1;     
        MaxSerialNo = 10;
    }          
       
	// Mark before each entity
    public override bool OnBeforeEntity(IMarker marker, IEntity entity)
    {
		var rtcMoF = marker.Rtc as IRtcMoF;
		var currentEntity = marker.CurrentEntity;
        switch (currentEntity.Name)
        {          
            case "MyText1":                           
            case "MyText2":             
			case "MyText3":             
				var leftX = currentEntity.BBox.RealMin.X;
				return rtcMoF.ListMofWait(RtcEncoders.EncX, -leftX, RtcEncoderWaitConditions.Under);
        }		
        return true;
    }
	
    // To convert text data
    public override string OnTextConvert(IMarker marker, ITextConvertible textConvertible)
    {
        var currentEntity = marker.CurrentEntity;
        switch (currentEntity.Name)
        {          
            case "MyText1":                      
            case "MyText2":             
			case "MyText3":             
				return string.Format(textConvertible.SourceText, SerialNo);
            default:
                // Not modified
                return textConvertible.SourceText;
        }
    }   
  
    // Mark at EntityScriptEvent entity
    public override bool ListEvent(EntityScriptEvent entityScriptEvent)
    {       
		//Increase serial no
        SerialNo++;
        if (MaxSerialNo > 0)
        {            
            if (SerialNo > MaxSerialNo)
            {
                SerialNo = StartSerialNo;
            }
        }
        Logger.Log(Logger.Types.Trace, "serial no has changed to {0} at script event", SerialNo);
        return true;
    }   
    
    
    // Control event for external user
    public override bool CtlEvent(object userData = null)
    {
       //Reset serial no as start no
        Logger.Log(Logger.Types.Trace, "serial no has reset to start no: {0}", StartSerialNo);
        SerialNo = StartSerialNo;
        return true;
    }
}       
 