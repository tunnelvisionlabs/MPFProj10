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

	public partial class ProjectNode
	{
		#region events
        public event EventHandler<ProjectPropertyChangedArgs> ProjectPropertyChanged;
		#endregion

		#region methods
		protected virtual void OnProjectPropertyChanged(ProjectPropertyChangedArgs e)
		{
			var t = ProjectPropertyChanged;
			if (t != null)
				t(this, e);
		}
		#endregion
	}

}