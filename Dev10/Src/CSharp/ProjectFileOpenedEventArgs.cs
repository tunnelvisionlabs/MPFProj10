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

	public class ProjectFileOpenedEventArgs : EventArgs
	{
		#region fields
		private readonly bool added;
		#endregion

		#region properties
		/// <summary>
		/// True if the project is added to the solution after the solution is opened. false if the project is added to the solution while the solution is being opened.
		/// </summary>
		public bool Added
		{
			get { return this.added; }
		}
		#endregion

		#region ctor
		public ProjectFileOpenedEventArgs(bool added)
		{
			this.added = added;
		}
		#endregion
	}
}
