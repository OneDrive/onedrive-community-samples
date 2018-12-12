using Microsoft.Graph;
using System;

namespace FilesExplorer.Models
{
    /// <summary>
    /// Represents a OneDrive item
    /// </summary>
    public abstract class OneDriveItem
    {
        /// <summary>
        /// Gets the name and the extension of the item
        /// </summary>
        public string Name => DriveItem.Name;

        /// <summary>
        /// Gets the OneDrive identifier
        /// </summary>
        public string Id => DriveItem.Id;

        /// <summary>
        /// Gets the full path of the item
        /// </summary>
        public string Path => $"{DriveItem.ParentReference.Path}/{DriveItem.Name}";

        /// <summary>
        /// Gets the internal Microsoft Graph object
        /// </summary>
        public DriveItem DriveItem { get; }

        public OneDriveItem(DriveItem item)
        {
            DriveItem = item ?? throw new ArgumentNullException(nameof(item));
        }
    }
}
