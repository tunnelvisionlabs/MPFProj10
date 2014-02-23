# Migration Guide

This migration guide does not cover all situations. However, it should provide a starting point for libraries currently using another release of MPFProj to switch to using this distribution instead.

## Renamed items

* Several properties named `ProjectMgr` were renamed to `ProjectManager`.
* `BaseURI` was renamed to `BaseUri`.
* `ID` was renamed to `Id`.
* `GetMkDocument` was renamed to `GetMKDocument`.
* `PrepareSelectedNodesForClipBoard` was renamed to `PrepareSelectedNodesForClipboard`.
* `ReDraw` was renamed to `Redraw`.
* `UIHierarchyElement` was renamed to `UIHierarchyElements`.
* The previously nested type `ProjectNode.ImageName` is now the top-level enumeration `ImageName`.
* `AddCATIDMapping` is now `AddCatIdMapping`.
* `AddFileToMsBuild` is now `AddFileToMSBuild`.

## Removed items

* `QueryStatusResult` was removed in favor of the existing enumeration [`vsCommandStatus`](http://msdn.microsoft.com/en-us/library/envdte.vscommandstatus.aspx). The member names differ slightly, but the relation should be obvious.
* `Microsoft.VisualStudio.Project.VsMenus` was removed, since it was just a duplication of [`Microsoft.VisualStudio.Shell.VsMenus`](http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.vsmenus.aspx).
* The `BuildAction` enumeration was removed. To support custom build actions, declare them in the custom `.targets` file for your project.

    ```xml
    <ItemGroup Condition="'$(BuildingInsideVisualStudio)'=='true'">
      <AvailableItemName Include="MyCustomBuildAction1" />
      <AvailableItemName Include="MyCustomBuildAction2" />
    </ItemGroup>
    ```

## Signature changes

* `ExecCommandOnNode` parameter *nCmdexecopt* now has the proper type [`OLECMDEXECOPT`](http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.ole.interop.olecmdexecopt.aspx) instead of just `uint`.
* `ProjectNode.Drop` parameter *pdwEffect* now has the type `DropEffects` instead of just `uint`. This may be changed to [`System.Windows.Forms.DragDropEffects`](http://msdn.microsoft.com/en-us/library/system.windows.forms.dragdropeffects.aspx) or [`System.Windows.DragDropEffects`](http://msdn.microsoft.com/en-us/library/system.windows.dragdropeffects.aspx).
* `GetProjectProperty`, `SetProjectProperty`, and several related methods have an additional parameter to specify the [`_PersistStorageType`](http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop._persiststoragetype.aspx). If your project system previously did not support user build projects, you can pass `_PersistStorageType.PST_PROJECT_FILE` to preserve the original behavior.
* `ProjectNode.SetConfiguration` and a few related methods have an additional parameter *platform* to specify the platform.
* The constructor for `ProjectNode` now requires the `ProjectPackage` be supplied as an argument. Previously, project systems would instantiate the `ProjectNode` and then manually set the `Package` property.

## Supporting Single File Generator Properties

The browse object properties for the single file generator are now implemented using `IExtenderProvider`. The extender provider is registered with the `ObjectExtenders` service when your `ProjectPackage` is initialized. However, this assumes that the browse object associated with your `FileNode` objects extends `FileNodeProperties` and uses the CATID `typeof(FileNodeProperties).GUID`. If you use a custom CATID for your `FileNode` browse object, you'll need to register the single file generator using the [`ObjectExtenders`](http://msdn.microsoft.com/en-us/library/envdte.objectextenders.aspx) interface.

Currently this can be accomplished using the following modifications to your `ProjectPackage`-derived class.

1. Declare the following two fields.

    ```csharp
    private SingleFileGeneratorNodeExtenderProvider _singleFileGeneratorNodeExtenderProvider;
    private int _singleFileGeneratorNodeExtenderCookie;
    ```

2. Register the extender provider in the `Initialize` method.

    ```csharp
    protected override void Initialize()
    {
        base.Initialize();

        ObjectExtenders objectExtenders = (ObjectExtenders)GetService(typeof(ObjectExtenders));
        _singleFileGeneratorNodeExtenderProvider = new SingleFileGeneratorNodeExtenderProvider();
        string extenderCatId = typeof(YourCustomFileNodeProperties).GUID.ToString("B");
        string extenderName = SingleFileGeneratorNodeExtenderProvider.Name;
        string localizedName = extenderName;
        _singleFileGeneratorNodeExtenderCookie = objectExtenders.RegisterExtenderProvider(extenderCatId, extenderName, _singleFileGeneratorNodeExtenderProvider, localizedName);
    }
    ```

3. Override the `Dispose` method to unregister the extender provider.

    ```csharp
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ObjectExtenders objectExtenders = (ObjectExtenders)GetService(typeof(ObjectExtenders));
            objectExtenders.UnregisterExtenderProvider(_singleFileGeneratorNodeExtenderCookie);
        }

        base.Dispose(disposing);
    }
    ```
