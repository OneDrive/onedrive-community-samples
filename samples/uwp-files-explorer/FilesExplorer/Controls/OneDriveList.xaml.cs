using FilesExplorer.Models;
using FilesExplorer.Services;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FilesExplorer.Controls
{
    /// <summary>
    /// Control which shows files and folders and allows to navigate into the folders
    /// </summary>
    public sealed partial class OneDriveList : UserControl
    {

        /// <summary>
        /// Keeps the list of the folder id where the user has navigated into
        /// </summary>
        private readonly Stack<string> _folders = new Stack<string>();
        /// <summary>
        /// Token used to cancel current load operation
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Event raised when the user click on a file
        /// </summary>
        public event EventHandler<OneDriveFile> DownloadFile;

        /// <summary>
        /// Event raised when control is loading a folder contents
        /// </summary>
        public event EventHandler FolderLoading;

        /// <summary>
        /// Event raised when control has finished to load folder contents
        /// </summary>
        public event EventHandler FolderLoaded;

        /// <summary>
        /// Event raised when an error occurs while loading the folder contents
        /// </summary>
        public event EventHandler<Exception> LoadingError;

        public OneDriveList()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the service to use for OneDrive operations
        /// </summary>
        public AuthenticationService AuthenticationService { get; set; }

        /// <summary>
        /// Gets the current folder id
        /// </summary>
        public string CurrentFolderId => _folders.Count > 0 ? _folders.Peek() : null;

        /// <summary>
        /// Loads folders and files contained by the root folder
        /// </summary>
        /// <returns></returns>
        public Task LoadRootFolderAsync()
        {
            _folders.Clear();
            _folders.Push(null);
            return LoadFolderAsync();
        }

        /// <summary>
        /// Goes to the parent folder and loads folders and files contained by the root folder
        /// </summary>
        /// <returns></returns>
        public Task LoadUpFolderAsync()
        {
            if (_folders.Count > 1)
            {
                _folders.Pop();
                return LoadFolderAsync(_folders.Peek());
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Refresh folders and files contained by the current folder
        /// </summary>
        /// <returns></returns>
        public Task RefreshFolderAsync()
        {
            if (_folders.Count > 0)
            {
                return LoadFolderAsync(_folders.Peek());
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Cancels any operation in progress
        /// </summary>
        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
            items.ItemsSource = null;
        }

        private void Items_Click(object sender, ItemClickEventArgs e)
        {
            // Evaluate action to do
            switch (e.ClickedItem)
            {
                // It's a file
                case OneDriveFile file:
                    DownloadFile?.Invoke(this, file);
                    break;
                // It's a folder
                case OneDriveFolder folder:
                    _folders.Push(folder.Id);
                    _ = LoadFolderAsync(folder.Id);
                    break;
            }
        }

        private async Task LoadFolderAsync(string id = null)
        {
            // Cancel any previous operation
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            // Check if session is set
            if (AuthenticationService == null) throw new InvalidOperationException($"No {nameof(AuthenticationService)} has been specified");

            // Keep a local copy of the token because the source can change while executing this function
            var token = _cancellationTokenSource.Token;

            // Add an option to the REST API in order to get thumbnails for each file
            // https://docs.microsoft.com/en-us/onedrive/developer/rest-api/api/driveitem_list_thumbnails
            var options = new[]
           {
                new QueryOption("$expand", "thumbnails"),
            };

            // Create the graph request builder for the drive
            IDriveRequestBuilder driveRequest = AuthenticationService.GraphClient.Me.Drive;

            // If folder id is null, the request refers to the root folder
            IDriveItemRequestBuilder driveItemsRequest;
            if (id == null)
            {
                driveItemsRequest = driveRequest.Root;
            }
            else
            {
                driveItemsRequest = driveRequest.Items[id];
            }

            // Raise the loading event
            FolderLoading?.Invoke(this, EventArgs.Empty);
            try
            {
                try
                {
                    // Make a API request loading 50 items per time
                    var page = await driveItemsRequest.Children.Request(options).Top(50).GetAsync(token);
                    token.ThrowIfCancellationRequested();

                    // Load each page
                    await LoadGridItemsAsync(page, token);
                    token.ThrowIfCancellationRequested();
                }
                finally
                {
                    // Raise the loaded event
                    FolderLoaded?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (OperationCanceledException)
            { }
            catch (Exception ex)
            {
                // Raise the error event
                LoadingError?.Invoke(this, ex);
            }
        }

        private async Task LoadGridItemsAsync(IDriveItemChildrenCollectionPage page, CancellationToken token)
        {
            // Collect items for each page
            var result = new List<OneDriveItem>();
            while (true)
            {
                // Iterate each item of the current page
                foreach (DriveItem item in page.CurrentPage)
                {
                    token.ThrowIfCancellationRequested();

                    // Add item to the list
                    result.Add(item.Folder != null ? (OneDriveItem)new OneDriveFolder(item) : new OneDriveFile(item));
                }

                // If NextPageRequest is not null there is another page to load
                if (page.NextPageRequest != null)
                {
                    // Load the next page
                    page = await page.NextPageRequest.GetAsync(token);
                    token.ThrowIfCancellationRequested();
                }
                else
                {
                    // No other pages to load
                    break;
                }
            }

            // Load items into the grid
            items.ItemsSource = result;
        }
    }
}
