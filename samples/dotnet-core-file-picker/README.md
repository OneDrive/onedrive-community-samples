# ASP.NET Core MVC File Picker

### Summary
This project shows how to use Microsoft Graph to query OneDrive for a specific item.


![ASP.NET Core File Picker UI](./assets/dotnet-core-file-picker.gif)

### Prerequisite
To use the ASP.NET Core MVC File Picker example you need the following:
* Visual Studio 2017 with at least [.NET Core 2.1 SDK](https://www.microsoft.com/net/download/core) installed on your development computer.
* Either a [personal Microsoft account](https://signup.live.com) or a [work or school account](https://dev.office.com/devprogram).
* The application ID and key from the application that you [register on the App Registration Portal](#register-the-app).

### Solution

Solution | Author(s)
---------|----------
dotnet-core-file-picker|Paolo Pialorsi (MCM, MVP, [PiaSys.com](https://piasys.com), [@PaoloPia](https://twitter.com/PaoloPia))

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 6th 2018 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

## How does the file picker works?
The file picker allows you to select a single file image from a popup showing your OneDrive content (filtered by images) and display the selected image in the web page.

## Register the app

This app uses the Azure AD v2.0 endpoint, so you'll register it on the [App Registration Portal](https://apps.dev.microsoft.com/).

1. Sign into the portal using either your personal or work or school account.

2. Choose **Add an app** next to 'Converged applications'.

3. Enter a name for the app, and choose **Create application**. (Don't check the Guided Setup box.)

   a. Enter a friendly name for the application.

   b. Copy the **Application Id**. This is the unique identifier for your app.

   c. Under **Application Secrets**, choose **Generate New Password**. Copy the password from the dialog. You won't be able to access this value again after you leave this dialog.

   >**Important**: Note that in production apps you should always use certificates as your application secrets, but for this sample we will use a simple shared secret password.

   d. Under **Platforms**, choose **Add platform**.

   e. Choose **Web**.

   f. Make sure the **Allow Implicit Flow** check box is selected, and add `https://localhost:44339/` (or the URL of your ASP.NET Core project) as a **Redirect URL**. This is the base callback URL for this sample.

   >The **Allow Implicit Flow** option enables the hybrid flow. During authentication, this enables the app to receive both sign-in info (the id_token) and artifacts (in this case, an authorization code) that the app can use to obtain an access token.
  
   g. Click **Save**.

   >You'll use the application ID and secret to configure the app in Visual Studio.

## Configure and run the template

1. Download or clone the ASP.NET Core MVC File Picker source code.
2. Open the **ASPNET-File-Picker.csproj** project file in Visual Studio 2017.
3. In Solution Explorer, open the **Index.cshtml** file in */Views/Home/*.
    a. Replace `ENTER_YOUR_APP_ID` with the application ID of your registered application.
4. Press F5 to build and run the sample. This will restore all NuGet package dependencies and start up the app.
5. Sign in with your personal account or your work or school account and grant the requested permissions (this will happen only the first time an application is requesting some specific permission).
6. Click on the **Open from OneDrive** button and in the popup select the image to be displayed.

## Key components of the sample
The following files contain code that's related to connecting to Microsoft Graph, open OneDrive and load a specific image file.

### Controllers

* [`HomeController.cs`](ASPNET-File-Picker/Controllers/HomeController.cs) Handles the request from the UI.

### Views

* [`Index.cshtml`](ASPNET-File-Picker/Views/Home/Index.cshtml) Contains the sample's UI to open the popup that allows the image selection.

<img src="https://telemetry.sharepointpnp.com/onedrive-community-samples/samples/dotnet-core-file-picker" />
