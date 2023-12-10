// Powered by (c)SpiralLab.Sirius2 with C# script codes
// Written by hcchoi@spirallab.co.kr
// 2023 Coypright to (c)SpiralLab

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
	public UserScript()
		: base()
	{
	}		
	
    
	public override string OnTextConvert(IMarker marker, ITextConvertible textConvertible)
	{
		// Index of marker (0,1,2,...)
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
			default:
				// Not modified
				return textConvertible.SourceText;
		}
	}
}		
 