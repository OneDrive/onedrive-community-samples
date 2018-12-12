using Microsoft.Graph;
using System;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace FilesExplorer.Models
{
    /// <summary>
    /// Represents a OneDrive file
    /// </summary>
    public class OneDriveFile : OneDriveItem
    {
        /// <summary>
        /// Gets the thumbnail, if any, for the file, otherwhile returns null
        /// </summary>
        public ImageSource Thumbnail
        {
            get
            {
                var mediumThunbnail = DriveItem.Thumbnails.FirstOrDefault(t => t.Medium != null);
                if (mediumThunbnail != null)
                {
                    return new BitmapImage(new Uri(mediumThunbnail.Medium.Url));
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the HTTP uri to use to download the file's content
        /// </summary>
        public Uri DownloadUri => new Uri(DriveItem.AdditionalData["@microsoft.graph.downloadUrl"].ToString());

        public OneDriveFile(DriveItem item) : base(item)
        {

        }
    }
}
