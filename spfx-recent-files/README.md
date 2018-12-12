# SPFX File Explorer

## Summary

Sample SharePoint Framework web part that shows the recently modified files on OneDrive.

![SPFX File Explorer in action](./assets/TODO.gif)

## Used SharePoint Framework Version

![SPFx v1.7.0](https://img.shields.io/badge/SPFx-1.7.0-green.svg)

## Applies to

* [SharePoint client-side web parts](https://docs.microsoft.com/en-us/sharepoint/dev/spfx/web-parts/overview-client-side-web-parts)
* [Office 365 developer tenant](http://dev.office.com/sharepoint/docs/spfx/set-up-your-developer-tenant)

## Solution

Solution|Author(s)
--------|---------
spfx-file-explorer|Paolo Pialorsi (MCM, MVP, [PiaSys.com](https://piasys.com), [@PaoloPia](https://twitter.com/PaoloPia))
spfx-file-explorer|Guido Zambarda ([PiaSys.com](https://piasys.com))

## Version history

Version|Date|Comments
-------|----|--------
1.0.0|November 12, 2018|Initial release

## Disclaimer

**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

---

## Prerequisites

* Office 365 Developer tenant with a modern site collection

## Minimal Path to Awesome

* clone this repo
* in the command line run
  * `npm i`
  * `gulp serve --nobrowser`
* open the workbench `/_layouts/15/workbench.aspx` on your SharePoint site
* add the Recent Files web part

## Deployment

In order to deploy the sample solution in a real environment, or at least in order to skip using the debug mode, you need to execute the following steps:
* publish the solution on any hosting environment or CDN and update the _cdnBasePath_ property in the write-manifests.json file with the base URL of your hosting environment
* bundle and package the solution by executing the following commands in the command line:
  * `gulp bundle --ship`
  * `gulp package-solution --ship`
* upload the content of the ./temp/deploy subfolder of the sample root folder into the target hosting environment
* add to the "Apps for SharePoint" library of the AppCatalog in your tenant the spfx-file-explorer.spppkg file that you will find under the ./sharepoint/solution subfolder of the sample root folder
* add the spfx-file-eexplorer web part any target page where you want to use the web part

## Features

This project contains sample SharePoint Framework web part built using React and Office UI Fabric React. The web part displays the recent used documents on OneDrive, by default it shows the last 5 documents, you can change the number of documents to show in the web part settings.

This sample illustrates the following concepts on top of the SharePoint Framework:

* using Office UI Fabric React to build SharePoint Framework web part that seamlessly integrate with SharePoint
* using React to build SharePoint Framework web part
* displaying the recent documents using OneDrive
