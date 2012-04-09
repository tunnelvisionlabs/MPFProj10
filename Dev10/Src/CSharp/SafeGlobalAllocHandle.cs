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
    using Microsoft.Win32.SafeHandles;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Alloc")]
    public sealed class SafeGlobalAllocHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeGlobalAllocHandle(IntPtr handle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(handle);
        }

        private SafeGlobalAllocHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            UnsafeNativeMethods.GlobalFree(handle);
            return true;
        }
    }
}
