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
    using System.Diagnostics.CodeAnalysis;

    public enum ImageName
    {
        OfflineWebApp = 0,
        WebReferencesFolder = 1,
        OpenReferenceFolder = 2,
        ReferenceFolder = 3,
        Reference = 4,
        SdlWebReference = 5,
        DiscoWebReference = 6,
        Folder = 7,
        OpenFolder = 8,
        ExcludedFolder = 9,
        OpenExcludedFolder = 10,
        ExcludedFile = 11,
        DependentFile = 12,
        MissingFile = 13,
        WindowsForm = 14,
        WindowsUserControl = 15,
        WindowsComponent = 16,
        XmlSchema = 17,
        XmlFile = 18,
        WebForm = 19,
        WebService = 20,
        WebUserControl = 21,
        WebCustomUserControl = 22,
        AspPage = 23,
        GlobalApplicationClass = 24,
        WebConfig = 25,
        HtmlPage = 26,
        StyleSheet = 27,
        ScriptFile = 28,
        TextFile = 29,
        SettingsFile = 30,
        Resources = 31,
        Bitmap = 32,
        Icon = 33,
        Image = 34,
        ImageMap = 35,
        XWorld = 36,
        Audio = 37,
        Video = 38,
        Cab = 39,
        Jar = 40,
        DataEnvironment = 41,
        PreviewFile = 42,
        DanglingReference = 43,
        XsltFile = 44,
        Cursor = 45,
        AppDesignerFolder = 46,
        Data = 47,
        Application = 48,
        DataSet = 49,
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pfx")]
        Pfx = 50,
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Snk")]
        Snk = 51,

        ImageLast = 51
    }
}
