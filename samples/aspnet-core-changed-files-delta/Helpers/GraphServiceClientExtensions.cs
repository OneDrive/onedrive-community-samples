using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspnetCore_Changed_Files.Helpers
{
    public static class GraphServiceClientExtensions
    {
        private static readonly Regex TokenRegex = new Regex(@"[&?]token=(?<t>[^&]+)|token='(?<t>[^']+)");

        public static async Task<DeltaFiles> GetRootFilesAsync(this GraphServiceClient client, string deltaToken)
        {
            var result = await client.Me.Drive.Root.Delta(deltaToken).Request()
                .Select("Id,Name,LastModifiedDateTime,Folder,WebUrl,Deleted")
                .GetAsync();

            deltaToken = TokenRegex.Match(result.AdditionalData["@odata.deltaLink"].ToString()).Groups["t"].Value;

            return new DeltaFiles
            {
                Files = result.Where(r => r.Folder == null).OrderByDescending(r => r.LastModifiedDateTime).ToArray(),
                DeltaToken = deltaToken
            };
        }
    }

    public class DeltaFiles
    {
        public string DeltaToken { get; set; }

        public IReadOnlyList<DriveItem> Files { get; set; }
    }
}
