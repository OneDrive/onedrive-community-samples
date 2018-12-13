using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ASPNET_File_Picker.Models;
using Microsoft.Graph;
using System.Net.Http.Headers;

namespace ASPNET_File_Picker.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<String> Index(SelectedFileViewModel model)
        {
            var graphClient = GetAuthenticatedClient(model.AccessToken);

            var result = await graphClient.Drives[model.DriveId].Items[model.FileId].Request().Expand("thumbnails").GetAsync();
            
            return result.Thumbnails.First().Large.Url;
        }

        /// <summary>
        /// Gets a new istance of <see cref="GraphServiceClient"/>
        /// </summary>
        /// <returns></returns>
        private GraphServiceClient GetAuthenticatedClient(string accessToken)
        {
            // Create Microsoft Graph client.
            return new GraphServiceClient(
                "https://graph.microsoft.com/v1.0",
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        // Set bearer authentication on header
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                    }));
        }
    }
}
