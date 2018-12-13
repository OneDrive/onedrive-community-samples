using System;

namespace ASPNET_File_Picker.Models
{
    public class SelectedFileViewModel
    {
        public string AccessToken { get; set; }

        public string FileId { get; set; }

        public string DriveId { get; set; }

        public string Thumbnail { get; set; }
    }
}