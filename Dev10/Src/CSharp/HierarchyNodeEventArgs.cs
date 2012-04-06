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
	using EventArgs = System.EventArgs;

	/// <summary>
	/// This class is used for the events raised by a HierarchyNode object.
	/// </summary>
	public class HierarchyNodeEventArgs : EventArgs
	{
		private readonly HierarchyNode _child;

		public HierarchyNodeEventArgs(HierarchyNode child)
		{
			this._child = child;
		}

		public HierarchyNode Child
		{
			get { return this._child; }
		}
	}
}
