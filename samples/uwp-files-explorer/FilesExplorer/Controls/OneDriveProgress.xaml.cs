using FilesExplorer.Models;
using FilesExplorer.Services;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace FilesExplorer.Controls
{

    /// <summary>
    /// Control which show the list of upload and downloads
    /// </summary>
    public sealed partial class OneDriveProgress : UserControl
    {
        /// <summary>
        /// Keeps the list of upload and downloads
        /// </summary>
        private readonly ObservableCollection<OneDriveFileProgress> _progressItems;

        /// <summary>
        /// Event raised when an error occurrs while uploading or downloading
        /// </summary>
        public event EventHandler<Exception> Error;

        public OneDriveProgress()
        {
            this.InitializeComponent();

            // Bind the list to the ListView source
            this.items.ItemsSource = _progressItems = new ObservableCollection<OneDriveFileProgress>();
        }

        /// <summary>
        /// Gets or sets the service to use for OneDrive operations
        /// </summary>
        public AuthenticationService AuthenticationService { get; set; }

        /// <summary>
        /// Downloads the file locally
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task DownloadFileAsync(OneDriveFile file)
        {
            // Prepare the picker suggisting the same OneDrive file name
            var picker = new FileSavePicker
            {
                SuggestedFileName = file.Name,
                SuggestedStartLocation = PickerLocationId.Downloads,
                DefaultFileExtension = Path.GetExtension(file.Name),
            };
            picker.FileTypeChoices.Add(file.Name, new List<string> { picker.DefaultFileExtension });
            // Ask where save the file
            StorageFile storageFile = await picker.PickSaveFileAsync();
            // storageFile is null if the user cancel the pick operation
            if (storageFile != null)
            {
                // Add a new item at the beginning of the list
                var item = new OneDriveFileProgress(file);
                _progressItems.Insert(0, item);
                // Start the download operation
                await item.DownloadFileAsync(storageFile, file.DownloadUri);
            }
        }

        /// <summary>
        /// Cancels any operation in progress
        /// </summary>
        public void Cancel()
        {
            foreach (var item in _progressItems)
            {
                item.Cancel();
            }
            _progressItems.Clear();
        }

        /// <summary>
        /// Uploads a new file into the folder id specificied
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public async Task UploadFileAsync(string folderId)
        {
            // Check if session is set
            if (AuthenticationService == null) throw new InvalidOperationException($"No {nameof(AuthenticationService)} has been specified");

            // Open the picker for file selection
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.Downloads,
            };
            // User can select any file
            picker.FileTypeFilter.Add("*");
            StorageFile storageFile = await picker.PickSingleFileAsync();

            // storageFile is null if no file has been selected
            if (storageFile != null)
            {
                // Create the graph request builder for the drive
                IDriveRequestBuilder driveRequest = AuthenticationService.GraphClient.Me.Drive;

                // If folder id is null, the request refers to the root folder
                IDriveItemRequestBuilder driveItemsRequest;
                if (folderId == null)
                {
                    driveItemsRequest = driveRequest.Root;
                }
                else
                {
                    driveItemsRequest = driveRequest.Items[folderId];
                }

                try
                {
                    // Create an upload session for a file with the same name of the user selected file
                    UploadSession session = await driveItemsRequest
                         .ItemWithPath(storageFile.Name)
                         .CreateUploadSession()
                         .Request()
                         .PostAsync();

                    // Add a new upload item at the beginning
                    var item = new OneDriveFileProgress(storageFile.Name);
                    _progressItems.Insert(0, item);

                    // Start the upload process
                    await item.UploadFileAsync(AuthenticationService, storageFile, session);
                }
                catch (Exception ex)
                {
                    Error?.Invoke(this, ex);
                }
            }
        }
    }
}
