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
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Microsoft.VisualStudio.Shell.Interop;
	using MSBuild = Microsoft.Build.Evaluation;

	/// <summary>
	/// Defines a container for manipulating references
	/// </summary>
	/// <remarks>Normally this should be an internal interface but since it should be available for
	/// the aggregator it must be made public.</remarks>
	[ComVisible(true)]
	public interface IReferenceContainer
	{
		IList<ReferenceNode> EnumReferences();
		ReferenceNode AddReferenceFromSelectorData(VSCOMPONENTSELECTORDATA selectorData);
		ReferenceNode AddReferenceFromSelectorData(VSCOMPONENTSELECTORDATA selectorData, string wrapperTool);
		void LoadReferencesFromBuildProject(MSBuild.Project buildProject);
	}
}
