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
	using System.Collections;
	using Microsoft.VisualStudio.OLE.Interop;

	public class EnumSTATDATA : IEnumSTATDATA
	{
		private readonly IEnumerable _i;

		private readonly IEnumerator _e;

		public EnumSTATDATA(IEnumerable data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			this._i = data;
			this._e = data.GetEnumerator();
		}

		void IEnumSTATDATA.Clone(out IEnumSTATDATA clone)
		{
			clone = new EnumSTATDATA(_i);
		}

		int IEnumSTATDATA.Next(uint celt, STATDATA[] d, out uint fetched)
		{
			uint rc = 0;
			//uint size = (fetched != null) ? fetched[0] : 0;
			for(uint i = 0; i < celt; i++)
			{
				if(_e.MoveNext())
				{
					STATDATA sdata = (STATDATA)_e.Current;

					rc++;
					if(d != null && d.Length > i)
					{
						d[i] = sdata;
					}
				}
			}

			fetched = rc;
			return 0;
		}

		int IEnumSTATDATA.Reset()
		{
			_e.Reset();
			return 0;
		}

		int IEnumSTATDATA.Skip(uint celt)
		{
			for(uint i = 0; i < celt; i++)
			{
				_e.MoveNext();
			}

			return 0;
		}
	}
}
