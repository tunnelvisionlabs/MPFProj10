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

    [Flags]
    public enum GlobalAllocFlags
    {
        None = 0,
        Fixed = 0x0000,
        Movable = 0x0002,
        ZeroInit = 0x0040,

        Handle = Movable | ZeroInit,
        Pointer = Fixed | ZeroInit,
    }
}
