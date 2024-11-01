// Powered by (c)SpiralLab.Sirius2 with C# script codes
// Written by hcchoi@spirallab.co.kr
// 2024 Coypright to (c)SpiralLab

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Numerics;
using SpiralLab.Sirius2;
using SpiralLab.Sirius2.Vision;
using SpiralLab.Sirius2.Vision.Common;
using SpiralLab.Sirius2.Vision.Process;
using SpiralLab.Sirius2.Vision.Inspector;
using SpiralLab.Sirius2.Vision.Script;

public class UserScript : InspectorScriptBase
{    
    public UserScript(IInspector inspector)
        : base(inspector)
    {
        Name = "Default.cs";
        Description = "This is a default user script for vision";
		Logger.Log(Logger.Types.Trace, "script [{0}]: instance has created", Name);
    }   
    
    // Inspector has started
    public override void OnStarted(IInspector inspector, InspectorArg arg)
    {
		Logger.Log(Logger.Types.Trace, "script [{0}] inspector has started", Name);
    }
  
	// Inspect before each layer
    public override bool OnBeforeLayer(IInspector inspector, InspectorArg arg, ProcessLayer layer)
    {
		Logger.Log(Logger.Types.Trace, "script [{0}] do something before {1}", Name, layer.ToString());
        return true;
    }
	// Inspect after each layer
    public override bool OnAfterLayer(IInspector inspector, InspectorArg arg, ProcessLayer layer)
    {
		Logger.Log(Logger.Types.Trace, "script [{0}] do something after {1}", Name, layer.ToString());
        return true;
    }	
	// Inspect before each entity
    public override bool OnBeforeProcess(IInspector inspector, InspectorArg arg, IProcess process)
    {
		Logger.Log(Logger.Types.Trace, "script [{0}] do something before {1}", Name, process.ToString());
        return true;
    }
	// Inspect after each entity
    public override bool OnAfterProcess(IInspector inspector, InspectorArg arg, IProcess process)
    {
		Logger.Log(Logger.Types.Trace, "script [{0}] do something after {1}", Name, process.ToString());
        return true;
    }
	 // Inspector has before end
    public override bool OnBeforeEnd(IInspector inspector, InspectorArg arg)
	{
		return true;
	}
    // Inspector has ended
    public override void OnEnded(IInspector inspector, InspectorArg arg)
    {
		Logger.Log(Logger.Types.Trace, "script [{0}] inspector has ended with '{1}'", Name, arg.OverAllResult ? "GOOD" : "NG");
		Logger.Log(Logger.Types.Trace, "script [{0}] inspector result counts are {1}", Name, arg.Results.Count);		
		foreach(var result in arg.Results)
		{
			Logger.Log(Logger.Types.Trace, "{0}: {1}", result.TypeName, result.ToString());
		}
    }
}       
 