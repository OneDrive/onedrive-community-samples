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

namespace PnpFileHandler.Models
{
    using Microsoft.Graph;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class PnpFileModel
    {
        private PnpFileModel(FileHandlerActivationParameters activationParameters)
        {
            this.ActivationParameters = activationParameters;
        }

        public FileHandlerActivationParameters ActivationParameters { get; set; }

        public string ErrorMessage { get; set; }

        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the uri to call to see the PnP contents
        /// </summary>
        public string PreviewUri { get; set; }

        /// <summary>
        /// Gets or sets the title of the file
        /// </summary>
        public string Title { get; set; }

        public static PnpFileModel GetError(FileHandlerActivationParameters parameters, Exception ex)
        {
            return new PnpFileModel(parameters) { ErrorMessage = ex.Message, ReadOnly = true };
        }

        public static async Task<PnpFileModel> GetAsync(FileHandlerActivationParameters parameters, System.Web.Mvc.UrlHelper url)
        {
            // Retrieve an access token so we can make API calls
            string accessToken = null;
            try
            {
                accessToken = await AuthHelper.GetUserAccessTokenSilentAsync(parameters.ResourceId);
            }
            catch (Exception ex)
            {
                return GetError(parameters, ex);
            }

            // Read the details of the specified item
            GraphServiceClient graphServiceClient = GraphHelper.Default.GetGraphServiceClient(accessToken);
            IDriveRequest request = new DriveRequest(parameters.ItemUrls[0], graphServiceClient, Array.Empty<Option>());
            Drive drive = await request.GetAsync();

            return new PnpFileModel(parameters)
            {
                Title = drive.Name,
                // Create uri to use to load the content
                PreviewUri = url.Action("PreviewInfo", new
                {
                    resourceId = parameters.ResourceId,
                    uri = parameters.ItemUrls[0] + "/content"
                })
            };
        }
    }
}