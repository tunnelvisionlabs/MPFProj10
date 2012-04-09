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
    /// <summary>
    /// Defines drop types
    /// </summary>
    public enum DropDataType
    {
        None,

        /// <summary>
        /// Windows Explorer
        /// </summary>
        Shell,

        /// <summary>
        /// VSProject storage items
        /// </summary>
        VSStorage,

        /// <summary>
        /// VSProject reference items
        /// </summary>
        VSReference
    }
}
