/*
 * PnP File Handler - Sample Code
 * Copyright (c) Microsoft Corporation
 * All rights reserved. 
 * 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the ""Software""), to deal in 
 * the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace PnpFileHandler.Controllers
{
    using PnpFileHandler;
    using PnpFileHandler.Models;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize]
    public class FileHandlerController : Controller
    {

        #region Methods registered in file handler manifest
       

        /// <summary>
        /// Generate a loading page to preview the content of the file
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Preview()
        {
            FileHandlerActivationParameters input = GraphHelper.GetActivationParameters(Request);

            var viewModel = await PnpFileModel.GetAsync(input, Url);
            return View(viewModel);
        }

        /// <summary>
        /// Generate a read-only preview of the file
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<ActionResult> PreviewInfo(
            string resourceId,
            string uri)
        {
            var viewModel = await PnpFilePreviewModel.GetAsync(resourceId, uri);

            return View(viewModel);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            
            var input = GraphHelper.GetActivationParameters(filterContext.HttpContext.Request);
            if (input != null)
            {
                // Ensure that the user context for a file handler matches the signed-in user context
                if (input.UserId != User.Identity.Name)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
        }

        #endregion               

    }
}