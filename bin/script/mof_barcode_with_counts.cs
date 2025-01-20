// Powered by (c)SpiralLab.Sirius2 with C# script codes
// Written by hcchoi@spirallab.co.kr
// 2024 Coypright to (c)SpiralLab

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
using SpiralLab.Sirius2.Winforms.Common;
using SpiralLab.Sirius2.Winforms.Entity;
using SpiralLab.Sirius2.Winforms.Marker;
using SpiralLab.Sirius2.Winforms.Remote;
using SpiralLab.Sirius2.Winforms.Script;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

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
    
    [Browsable(true)]
    [ReadOnly(false)]
    [Category("Trigger")]
    [DisplayName("Buffer Count")]
    [Description("Buffered Trigger Counts")]
    public uint BufferTriggerCounts 
    {
        get { return bufferTriggerCounts; } 
        set {
            bufferTriggerCounts = value;
            NotifyPropertyChanged();
        }
    }   
    uint bufferTriggerCounts;
     
    [Browsable(true)]
    [ReadOnly(false)]
    [Category("Trigger")]
    [DisplayName("Real Count")]
    [Description("Real Trigger Counts")]
    public uint RealTriggerCounts 
    {
        get {               
            uint counts = 0;
            var rtcFreeVariable = Marker.Rtc as IRtcFreeVariable;
            rtcFreeVariable.CtlReadVariable((uint)0, out counts);
            return counts; 
        }
        set {
            var rtcFreeVariable = Marker.Rtc as IRtcFreeVariable;
            rtcFreeVariable.CtlWriteVariable((uint)0, value);
            NotifyPropertyChanged();
        }        
    }   
    
    
     
    public UserScript(IMarker marker)
        : base(marker)
    {
        Name = "mof_barcode.cs";
        Description = "incremented serial no";
        Logger.Log(Logger.Types.Trace, "script [{0}]: instance has created", Name);
        
        StartSerialNo = 1;        
        SerialNo = 1;     
        MaxSerialNo = 10;
        BufferTriggerCounts = 0;
    }          
    // Marker has started
    public override void OnStarted(IMarker marker)
    {
		
		
    }
	
    // To convert text data
    public override string OnTextConvert(IMarker marker, ITextConvertible textConvertible)
    {
        var currentEntity = marker.CurrentEntity;
        switch (currentEntity.Name)
        {
            case "MyBarcode1":
                return string.Format("Barcode {0:D4}", SerialNo);
            case "MyText1":                
                return string.Format("Text1 {0:D4}", SerialNo);
            case "MyText2":             
                return string.Format("Text2 {0:D4}", SerialNo);
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
        BufferTriggerCounts++;      
        var rtcFreeVariable = Marker.Rtc as IRtcFreeVariable;
        rtcFreeVariable.ListWriteVariable((uint)0, BufferTriggerCounts);        
        
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
 