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
	using System.Runtime.InteropServices;

	internal static partial class UnsafeNativeMethods
	{
		[DllImport(ExternDll.Kernel32, SetLastError = true, EntryPoint = "RtlMoveMemory")]
		internal static extern void MoveMemory(IntPtr destination, IntPtr source, UIntPtr size);

		[DllImport(ExternDll.Kernel32, SetLastError = true)]
		internal static extern SafeGlobalAllocHandle GlobalAlloc(GlobalAllocFlags flags, UIntPtr size);

		[DllImport(ExternDll.Kernel32, SetLastError = true)]
		internal static extern IntPtr GlobalFree(IntPtr handle);

		[DllImport(ExternDll.Kernel32, SetLastError = true)]
		internal static extern IntPtr GlobalLock(SafeGlobalAllocHandle h);

		[DllImport(ExternDll.Kernel32, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GlobalUnlock(SafeGlobalAllocHandle h);

		[DllImport(ExternDll.Kernel32, SetLastError = true)]
		internal static extern UIntPtr GlobalSize(SafeGlobalAllocHandle h);

		[DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OleSetClipboard(Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject);

		[DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OleGetClipboard(out Microsoft.VisualStudio.OLE.Interop.IDataObject dataObject);

		[DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OleFlushClipboard();

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OpenClipboard(IntPtr newOwner);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int EmptyClipboard();

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int CloseClipboard();

		[DllImport(ExternDll.Comctl32, CharSet = CharSet.Auto)]
		internal static extern int ImageList_GetImageCount(HandleRef himl);

		[DllImport(ExternDll.Comctl32, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ImageList_Draw(HandleRef himl, int i, HandleRef hdcDst, int x, int y, int fStyle);

		[DllImport(ExternDll.Shell32, SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern uint DragQueryFile(IntPtr hDrop, uint iFile, char[] lpszFile, uint cch);

		[DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern uint RegisterClipboardFormat(string format);
	}
}

