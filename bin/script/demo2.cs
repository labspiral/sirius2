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
	public UserScript(IMarker marker)
		: base(marker)
	{	
		Name = "Demo2.cs";
		Description = "This is a sample user script";
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
				return string.Format("ABC {0}", currentOffsetIndex);
			case "MyText1":
				return string.Format("DEF {0} {1}", DateTime.Now.ToString("HH:mm:ss"), currentOffsetIndex);
			default:
				// Not modified
				return textConvertible.SourceText;
		}
	}
}		
 