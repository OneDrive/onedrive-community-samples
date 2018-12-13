using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PnpFileHandler.Models
{
    public class PnpFilePreviewModel
    {

        public PnpFilePreviewItemModel[] Templates { get; private set; }

        public static async Task<PnpFilePreviewModel> GetAsync(string resourceId, string uri)
        {
            // Retrieve an access token so we can make API calls
            string accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(resourceId);

            // Read the PnP file content
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", accessToken);
            using (Stream stream = await client.GetStreamAsync(uri))
            {
                XMLTemplateProvider provider = new XMLOpenXMLTemplateProvider(new OpenXMLConnector(stream));

                // Get the list of the templates
                return new PnpFilePreviewModel
                {
                    Templates = provider.GetTemplates().Select(p => new PnpFilePreviewItemModel
                    {
                        DisplayName = p.DisplayName,
                        ImagePreviewUrl = p.ImagePreviewUrl
                    }).ToArray()
                };
            }
        }
    }

    public class PnpFilePreviewItemModel
    {
        public string DisplayName { get; internal set; }

        public string ImagePreviewUrl { get; internal set; }
    }
}