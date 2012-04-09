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

	/// <summary>
	/// Defines a type converter.
	/// </summary>
	/// <remarks>This is needed to get rid of the type TypeConverter type that could not give back the Type we were passing to him.
	/// We do not want to use reflection to get the type back from the  ConverterTypeName. Also the GetType methos does not spwan converters from other assemblies.</remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class PropertyPageTypeConverterAttribute : Attribute
	{
		#region fields
		private readonly Type converterType;
		#endregion

		#region ctors
		public PropertyPageTypeConverterAttribute(Type converterType)
		{
			this.converterType = converterType;
		}
		#endregion

		#region properties
		public Type ConverterType
		{
			get
			{
				return this.converterType;
			}
		}
		#endregion
	}
}
