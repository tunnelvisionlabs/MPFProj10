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
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Argument of the event raised when a project property is changed.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public class ProjectPropertyChangedArgs : EventArgs
	{
		private readonly string propertyName;
		private readonly string oldValue;
		private readonly string newValue;

		internal ProjectPropertyChangedArgs(string propertyName, string oldValue, string newValue)
		{
			this.propertyName = propertyName;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public string NewValue
		{
			get { return newValue; }
		}

		public string OldValue
		{
			get { return oldValue; }
		}

		public string PropertyName
		{
			get { return propertyName; }
		}
	}
}
