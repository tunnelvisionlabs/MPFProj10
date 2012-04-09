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
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Security.Permissions;
	using Microsoft.VisualStudio.OLE.Interop;

	[SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public static class DragDropHelper
	{
		public static readonly ushort CF_VSREFPROJECTITEMS = checked((ushort)UnsafeNativeMethods.RegisterClipboardFormat("CF_VSREFPROJECTITEMS"));
		public static readonly ushort CF_VSSTGPROJECTITEMS = checked((ushort)UnsafeNativeMethods.RegisterClipboardFormat("CF_VSSTGPROJECTITEMS"));
		public static readonly ushort CF_VSPROJECTCLIPDESCRIPTOR = checked((ushort)UnsafeNativeMethods.RegisterClipboardFormat("CF_PROJECTCLIPBOARDDESCRIPTOR"));

		public static FORMATETC CreateFormatEtc(ushort format)
		{
			FORMATETC fmt = new FORMATETC();
			fmt.cfFormat = format;
			fmt.ptd = IntPtr.Zero;
			fmt.dwAspect = (uint)DVASPECT.DVASPECT_CONTENT;
			fmt.lindex = -1;
			fmt.tymed = (uint)TYMED.TYMED_HGLOBAL;
			return fmt;
		}

		public static int QueryGetData(Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject, ref FORMATETC fmtetc)
		{
			if (dataObject == null)
				throw new ArgumentNullException("dataObject");

			int returnValue = VSConstants.E_FAIL;
			FORMATETC[] af = new FORMATETC[1];
			af[0] = fmtetc;
			try
			{
				int result = ErrorHandler.ThrowOnFailure(dataObject.QueryGetData(af));
				if(result == VSConstants.S_OK)
				{
					fmtetc = af[0];
					returnValue = VSConstants.S_OK;
				}
			}
			catch(COMException e)
			{
				Trace.WriteLine("COMException : " + e.Message);
				returnValue = e.ErrorCode;
			}

			return returnValue;
		}

		public static STGMEDIUM GetData(Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject, ref FORMATETC fmtetc)
		{
			if (dataObject == null)
				throw new ArgumentNullException("dataObject");

			FORMATETC[] af = new FORMATETC[1];
			af[0] = fmtetc;
			STGMEDIUM[] sm = new STGMEDIUM[1];
			dataObject.GetData(af, sm);
			fmtetc = af[0];
			return sm[0];
		}

		/// <summary>
		/// Retrives data from a VS format.
		/// </summary>
		public static ReadOnlyCollection<string> GetDroppedFiles(ushort format, Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject, out DropDataType ddt)
		{
			ddt = DropDataType.None;
			List<string> droppedFiles = new List<string>();

			// try HDROP
			FORMATETC fmtetc = CreateFormatEtc(format);

			if(QueryGetData(dataObject, ref fmtetc) == VSConstants.S_OK)
			{
				STGMEDIUM stgmedium = DragDropHelper.GetData(dataObject, ref fmtetc);
				if(stgmedium.tymed == (uint)TYMED.TYMED_HGLOBAL)
				{
					// We are releasing the cloned hglobal here.
					IntPtr dropInfoHandle = stgmedium.unionmember;
					if(dropInfoHandle != IntPtr.Zero)
					{
						ddt = DropDataType.Shell;
						try
						{
							uint numFiles = UnsafeNativeMethods.DragQueryFile(dropInfoHandle, 0xFFFFFFFF, null, 0);

							// We are a directory based project thus a projref string is placed on the clipboard.
							// We assign the maximum length of a projref string.
							// The format of a projref is : <Proj Guid>|<project rel path>|<file path>
							uint lenght = (uint)Guid.Empty.ToString().Length + 2 * NativeMethods.MAX_PATH + 2;
							char[] moniker = new char[lenght + 1];
							for(uint fileIndex = 0; fileIndex < numFiles; fileIndex++)
							{
								uint queryFileLength = UnsafeNativeMethods.DragQueryFile(dropInfoHandle, fileIndex, moniker, lenght);
								string filename = new String(moniker, 0, (int)queryFileLength);
								droppedFiles.Add(filename);
							}
						}
						finally
						{
							Marshal.FreeHGlobal(dropInfoHandle);
						}
					}
				}
			}

			return droppedFiles.AsReadOnly();
		}

		public static string GetSourceProjectPath(Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject)
		{
			string projectPath = null;
			FORMATETC fmtetc = CreateFormatEtc(CF_VSPROJECTCLIPDESCRIPTOR);

			if(QueryGetData(dataObject, ref fmtetc) == VSConstants.S_OK)
			{
				STGMEDIUM stgmedium = DragDropHelper.GetData(dataObject, ref fmtetc);
				if(stgmedium.tymed == (uint)TYMED.TYMED_HGLOBAL && stgmedium.unionmember != IntPtr.Zero)
				{
					// We are releasing the cloned hglobal here.
					using (SafeGlobalAllocHandle dropInfoHandle = new SafeGlobalAllocHandle(stgmedium.unionmember, true))
					{
						projectPath = GetData(dropInfoHandle);
					}
				}
			}

			return projectPath;
		}

		/// <summary>
		/// Returns the data packed after the DROPFILES structure.
		/// </summary>
		/// <param name="dropHandle"></param>
		/// <returns></returns>
		internal static string GetData(SafeGlobalAllocHandle dropHandle)
		{
			IntPtr data = UnsafeNativeMethods.GlobalLock(dropHandle);
			try
			{
				_DROPFILES df = (_DROPFILES)Marshal.PtrToStructure(data, typeof(_DROPFILES));
				if(df.fWide != 0)
				{
					IntPtr pdata = new IntPtr((long)data + df.pFiles);
					return Marshal.PtrToStringUni(pdata);
				}
			}
			finally
			{
				if(data != null)
				{
					UnsafeNativeMethods.GlobalUnlock(dropHandle);
				}
			}

			return null;
		}

		internal static SafeGlobalAllocHandle CopyHGlobal(SafeGlobalAllocHandle data)
		{
			IntPtr src = UnsafeNativeMethods.GlobalLock(data);
			UIntPtr size = UnsafeNativeMethods.GlobalSize(data);
			SafeGlobalAllocHandle ptr = UnsafeNativeMethods.GlobalAlloc(0, size);
			IntPtr buffer = UnsafeNativeMethods.GlobalLock(ptr);

			try
			{
				UnsafeNativeMethods.MoveMemory(buffer, src, size);
			}
			finally
			{
				if(buffer != IntPtr.Zero)
				{
					UnsafeNativeMethods.GlobalUnlock(ptr);
				}

				if(src != IntPtr.Zero)
				{
					UnsafeNativeMethods.GlobalUnlock(data);
				}
			}

			return ptr;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "s")]
		public static void CopyStringToHGlobal(string s, IntPtr data, int bufferSize)
		{
			if (s == null)
				throw new ArgumentNullException("s");
			if (data == null)
				throw new ArgumentNullException("data");
			if (bufferSize < 0)
				throw new ArgumentOutOfRangeException("bufferSize");

			byte[] stringData = System.Text.Encoding.Unicode.GetBytes(s);
			if (bufferSize < stringData.Length + 2)
				throw new ArgumentException();

			Marshal.Copy(stringData, 0, data, stringData.Length);
			Marshal.WriteInt16(data, stringData.Length, 0);
		}
	}
}
