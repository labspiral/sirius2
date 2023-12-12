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

public class UserScript : ScriptBase
{    
    public UserScript(IMarker marker)
        : base(marker)
    {
        Name = "Sample.cs";
        Description = "This is a sample user script";
    }   
    
    // Marker has started
    public override void OnMarkerStarted(IMarker marker)
    {
        if (null == marker.Remote)
            return;
        if (!marker.Remote.IsConnected)
            return;
        var text = string.Format("Status{0}Started{1}", ',', ';');
        marker.Remote.Send(text);   
    }
    // To convert text data
    public override string OnTextConvert(IMarker marker, ITextConvertible textConvertible)
    {
        // Not modified
        return textConvertible.SourceText;
    }
    public override bool OnAfterLayer(IMarker marker, EntityLayer layer)
    {
        return true;
    }
    public override bool OnBeforeLayer(IMarker marker, EntityLayer layer)
    {
        return true;
    }
    public override bool OnBeforeEntity(IMarker marker, IEntity entity)
    {
        return true;
    }
    public override bool OnAfterEntity(IMarker marker, IEntity entity)
    {
        return true;
    }
    // Marker has ended
    public override void OnMarkerEnded(IMarker marker, bool success, TimeSpan timeSpan)
    {
        if (null == marker.Remote)
            return;
        if (!marker.Remote.IsConnected)
            return;
        var text = string.Format("Status{0}Ended{1}", ',', ';');
        marker.Remote.Send(text);       
    }
}       
 