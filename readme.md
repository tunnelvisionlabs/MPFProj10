MPF for Projects (Visual Studio 2010 Release)
=============================================

This project is based on code originally published at:<br/>
	http://mpfproj10.codeplex.com/


Enhancements in this version include:
-------------------------------------

### General items

* Builds as a separate assembly
* A few bug fixes

### Improved performance

* Excellent project load and Show All Files performance, even for folder structures containing several thousand files


### Support for new commands

* Show All Files
* Open Containing Folder (projects and folders)

### Additional MSBuild features

* Support for MSBuild wildcard includes
* Support for the `CopyToOutputDirectory` file property
* Full support for user build projects (`*.user`)
* Support for project platforms as well as configurations

### Additional flexibility

* Support for projects that do not contain a References node
* Support for customizing the component selector (displayed by the `Add Reference...` command)
* Extensible build actions via the `AvailableItemName` property exposed by MSBuild targets files
* Extensible support for folder item types (previously only the `Folder` MSBuild item type was treated as a folder)
