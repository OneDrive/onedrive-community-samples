using FilesExplorer.Models;
using FilesExplorer.Services;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FilesExplorer
{
    /// <summary>
    /// MainPage containing files browser and progress box
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public AuthenticationService AuthenticationService { get; }

        public MainPage()
        {
            // Read client id from App.xaml resources
            // Please refer to https://docs.microsoft.com/en-us/onedrive/developer/rest-api/getting-started/app-registration for instructions
            string clientId = App.Current.Resources["ClientId"] as string;
            if (String.IsNullOrWhiteSpace(clientId)) throw new InvalidOperationException("Please specificy the ClientId into App.xaml file");

            // Create service which mantains token session
            AuthenticationService = new AuthenticationService(clientId);

            InitializeComponent();

            // Set initial UI
            SetConnectionStatus();
        }

        private void SetConnectionStatus()
        {
            // Show or hide buttons and change text if token is available or not
            if (AuthenticationService.IsConnected)
            {
                instructions.Text = "Click on a folder to see the files or click on a file to download it.";
                up.Visibility = Visibility.Visible;
                upload.Visibility = Visibility.Visible;
                refresh.Visibility = Visibility.Visible;
                connect.Label = "Disconnect";
            }
            else
            {
                instructions.Text = "Please connect to a Microsoft personal or office/school account.";
                up.Visibility = Visibility.Collapsed;
                upload.Visibility = Visibility.Collapsed;
                refresh.Visibility = Visibility.Collapsed;
                connect.Label = "Connect";
            }
            // Enable the connect button, because it's disabled when user press connect button
            connect.IsEnabled = true;

            // Can go to the parent folder if user is not inside the root
            up.IsEnabled = list.CurrentFolderId != null;

            // Hide progress bar
            progressBar.Opacity = 0;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            // Toggle session status
            if (AuthenticationService.IsConnected)
            {
                // Cancel any current operation
                list.Cancel();
                progress.Cancel();

                // Cancel the user session
                _ = AuthenticationService.SignOutAsync();

                // Update UI
                SetConnectionStatus();
            }
            else
            {
                // Disable connect button
                // The first you press the button a UI is prompted for login and authorization process
                connect.IsEnabled = false;

                // Load the root folder and ignore task
                _ = list.LoadRootFolderAsync();
            }
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            // Go back to the previous folder and ignore task
            _ = list.LoadUpFolderAsync();
        }

        private async void Upload_Click(object sender, RoutedEventArgs e)
        {
            string currentFolderId = list.CurrentFolderId;

            // Start a upload process in the current folder
            await progress.UploadFileAsync(currentFolderId);

            // In the meanwhile the user can change the current folder or disconnect
            if (AuthenticationService.IsConnected && currentFolderId == list.CurrentFolderId)
            {
                // Update the folder only if the user is the same folder where the file is contained by
                _ = list.RefreshFolderAsync();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            _ = list.RefreshFolderAsync();
        }

        private void list_DownloadFile(object sender, OneDriveFile file)
        {
            _ = progress.DownloadFileAsync(file);
        }

        private void Controls_Error(object sender, Exception e)
        {
            // Skip cancel exception
            if (e is OperationCanceledException) return;

            // Show a dialog with the exception message
            var dialog = new MessageDialog(e.Message, "Error");
            _ = dialog.ShowAsync();
        }

        private void list_FolderLoaded(object sender, EventArgs e)
        {
            // Update the UI
            SetConnectionStatus();

            // Hide the progress bar
            progressBar.Opacity = 0;
        }

        private void list_FolderLoading(object sender, EventArgs e)
        {
            // Show progress bar
            progressBar.Opacity = 1;
        }

    }
}
