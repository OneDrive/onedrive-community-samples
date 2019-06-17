# SPFx OneDrive File Picker

## Summary

Sample SharePoint Framework web part that allows the selection of images from OneDrive and display them.

![The SPFX File Picker in action](./assets/SPFX-File-Picker.gif)

## Used SharePoint Framework Version

![SPFx v1.7.0](https://img.shields.io/badge/SPFx-1.7.0-green.svg)

## Applies to

* [SharePoint client-side web parts](https://docs.microsoft.com/en-us/sharepoint/dev/spfx/web-parts/overview-client-side-web-parts)
* [Office 365 developer tenant](http://dev.office.com/sharepoint/docs/spfx/set-up-your-developer-tenant)

## Solution

Solution|Author(s)
--------|---------
spfx-file-picker|Paolo Pialorsi (MCM, MVP, [PiaSys.com](https://piasys.com), [@PaoloPia](https://twitter.com/PaoloPia))
spfx-file-picker|Guido Zambarda ([PiaSys.com](https://piasys.com))

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
* register the app in the [App Registration Portal](https://apps.dev.microsoft.com/) following [these instructions](#appRegistration)
* configure the *ClientId* of the newly registered app in the [OneDrivePicker.ts](./src/webparts/components/OneDrivePicker.ts) file in line 46, replacing the "[YOUR APP CLIENTID HERE]" placeholder
* in the command line run
  * `npm i`
  * `gulp serve --nobrowser`
* open the workbench `/_layouts/15/workbench.aspx` on your SharePoint site
* add the File Picker web part and play with it

## Deployment

In order to deploy the sample solution in a real environment, or at least in order to skip using the debug mode, you need to execute the following steps:
* publish the solution on any hosting environment or CDN and update the _cdnBasePath_ property in the write-manifests.json file with the base URL of your hosting environment
* bundle and package the solution by executing the following commands in the command line:
  * `gulp bundle --ship`
  * `gulp package-solution --ship`
* upload the content of the ./temp/deploy subfolder of the sample root folder into the target hosting environment
* add to the "Apps for SharePoint" library of the AppCatalog in your tenant the spfx-file-picker.spppkg file that you will find under the ./sharepoint/solution subfolder of the sample root folder
* add the FilePicker web part on any target page where you want to use it

<a name="appRegistration"></a>
## App Registration
In order to register the app in the App Registration Portal, follow these steps:
* Browse to the [App Registration Portal](https://apps.dev.microsoft.com/)
* Click the "Add an app" button
* Provide a value like "spfx-file-picker" for the Application Name field and press "Create"
    * Copy the value of the Application Id field provided in the app page and use it to configure the [OneDrivePicker.ts](./src/webparts/components/OneDrivePicker.ts) file
* In the "Platforms" section click the button "Add Platform" and select "Web"
    * Keep the "Allow Implicit Flow" flag selected
    * Provide in the "Redirect URLs" section all the URLs of the pages that will use the web part. For example, if you are using the web part from the page https://&lt;your-tenant&gt;.sharepoint.com/_layouts/15/workbench.aspx, provide that URL as one of the values for the redirect URLs
* In the "Microsoft Graph Permissions" section configure any of the "User.Read" or "Files.Read.All" permissions
* Click the "Save" button

For further details, you can read the official documentation about [OneDrive File Picker](https://docs.microsoft.com/en-us/onedrive/developer/controls/file-pickers/js-v72/open-file?view=odsp-graph-online), and the [OneDrive permission scopes](https://docs.microsoft.com/en-us/onedrive/developer/rest-api/concepts/permissions_reference?view=odsp-graph-online).

## Features

This project contains sample SharePoint Framework web part built using React and Office UI Fabric React. The web part allows to select and display images from your OneDrive, by default no image is selected.

This sample illustrates the following concepts on top of the SharePoint Framework:

* using Office UI Fabric React to build SharePoint Framework web part that seamlessly integrate with SharePoint
* using React to build SharePoint Framework web part
* selecting and displaying the images using OneDrive

<img src="https://telemetry.sharepointpnp.com/onedrive-community-samples/samples/spfx-file-picker" />
