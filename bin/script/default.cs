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

public class UserScript : MarkerScriptBase
{    
    public UserScript(IMarker marker)
        : base(marker)
    {
        Name = "Default.cs";
        Description = "This is a default user script";
		Logger.Log(Logger.Types.Trace, "script [{0}]: instance has created", Name);
    }   
    
    // Marker has started
    public override void OnStarted(IMarker marker)
    {
    }
    // To convert text data
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
		
        // Do nothing. not modified
        return textConvertible.SourceText;
    }
	// Mark before each layer
    public override bool OnBeforeLayer(IMarker marker, EntityLayer layer)
    {
        return true;
    }
	// Mark after each layer
    public override bool OnAfterLayer(IMarker marker, EntityLayer layer)
    {
        return true;
    }	
	// Mark before each entity
    public override bool OnBeforeEntity(IMarker marker, IEntity entity)
    {
        return true;
    }
	// Mark after each entity
    public override bool OnAfterEntity(IMarker marker, IEntity entity)
    {
        return true;
    }
	// Event for script 
	public override bool OnScriptEvent(IMarker marker, EntityScriptEvent entity)
	{
		return true;
	}
    // Marker has ended
    public override void OnEnded(IMarker marker, bool success, TimeSpan timeSpan)
    {
    }
}       
 