/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Microsoft.VisualStudio.Project
{
	using System;
	using System.Diagnostics;

	public static class CCITracing
	{
		[ConditionalAttribute("Enable_CCIDiagnostics")]
		static void InternalTraceCall(int levels)
		{
			System.Diagnostics.StackFrame stack;
			stack = new System.Diagnostics.StackFrame(levels);
			System.Reflection.MethodBase method = stack.GetMethod();
			if(method != null)
			{
				string name = method.Name + " \tin class " + method.DeclaringType.Name;
				System.Diagnostics.Trace.WriteLine("Call Trace: \t" + name);
			}
		}

		[ConditionalAttribute("CCI_TRACING")]
		static public void TraceCall()
		{
			// skip this one as well
			CCITracing.InternalTraceCall(2);
		}

		[ConditionalAttribute("CCI_TRACING")]
		static public void TraceCall(string parameters)
		{
			CCITracing.InternalTraceCall(2);
			System.Diagnostics.Trace.WriteLine("\tParameters: \t" + parameters);
		}

		[ConditionalAttribute("CCI_TRACING")]
		static public void Trace(System.Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException("exception");

			CCITracing.InternalTraceCall(2);
			System.Diagnostics.Trace.WriteLine("ExceptionInfo: \t" + exception.ToString());
		}

		[ConditionalAttribute("CCI_TRACING")]
		static public void Trace(string output)
		{
			System.Diagnostics.Trace.WriteLine(output);
		}

		[ConditionalAttribute("CCI_TRACING")]
		static public void TraceData(string output)
		{
			System.Diagnostics.Trace.WriteLine("Data Trace: \t" + output);
		}

		[ConditionalAttribute("Enable_CCIFileOutput")]
		[ConditionalAttribute("CCI_TRACING")]
		static public void AddTraceLog(string fileName)
		{
			TextWriterTraceListener tw = new TextWriterTraceListener("c:\\mytrace.log");
			System.Diagnostics.Trace.Listeners.Add(tw);
		}
	}
}
