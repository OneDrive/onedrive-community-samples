using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspnetCore_Changed_Files.Models;
using AspnetCore_Changed_Files.Helpers;
using System.Security.Claims;
using Microsoft.Graph;

namespace AspnetCore_Changed_Files.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(
            [FromServices]IGraphSdkHelper helper,
            string deltaToken
            )
        {
            var viewModel = new IndexViewModel();

            // Get the files if user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                var deltaFiles = await GetRootFilesAsync(helper, deltaToken);

                // deltaFiles is null when user is no more authenticated
                if (deltaFiles != null)
                {
                    viewModel.Items = deltaFiles.Files;
                    viewModel.DeltaToken = deltaFiles.DeltaToken;
                }
                else
                {
                    return RedirectToAction("SignIn", "Account");
                }
            }

            return View(viewModel);
        }

        private async Task<DeltaFiles> GetRootFilesAsync(IGraphSdkHelper helper, string deltaToken)
        {
            try
            {
                var client = helper.GetAuthenticatedClient((ClaimsIdentity)User.Identity);
                return await client.GetRootFilesAsync(deltaToken);
            }
            catch (ServiceException se) when (se.Error.Code == "TokenNotFound")
            {
                // May be token cache has been lost
                return null;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
