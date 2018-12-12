using Microsoft.Graph;

namespace FilesExplorer.Models
{

    /// <summary>
    /// Represents a OneDrive folder
    /// </summary>
    public class OneDriveFolder : OneDriveItem
    {
        public OneDriveFolder(DriveItem item) : base(item)
        {
        }
    }
}
