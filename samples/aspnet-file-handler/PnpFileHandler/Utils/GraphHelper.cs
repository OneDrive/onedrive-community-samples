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

namespace PnpFileHandler
{
    using Microsoft.Graph;
    using PnpFileHandler.Models;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;

    public class GraphHelper
    {
        private HttpClient httpClient = new HttpClient();
        public static readonly GraphHelper Default = new GraphHelper();

        public GraphServiceClient GetGraphServiceClient(string accessToken)
        {
            return new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) =>
            {
                // Add bearer access token
                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            }));
        }

        /// <summary>
        /// Parse either the POST data or stored cookie data to retrieve the file information from
        /// the request.
        /// </summary>
        /// <returns></returns>
        public static FileHandlerActivationParameters GetActivationParameters(HttpRequestBase request)
        {
            FileHandlerActivationParameters activationParameters = null;
            if (IsFileHandlerActivationRequest(request, out activationParameters))
            {
                return activationParameters;
            }
            return null;
        }

        public static bool IsFileHandlerActivationRequest(HttpRequestBase request, out FileHandlerActivationParameters activationParameters)
        {
            activationParameters = null;
            if (request.Form != null && request.Form.AllKeys.Any())
            {
                // Get from current request's form data
                activationParameters = new FileHandlerActivationParameters(request.Form);
                return true;
            }
            else
            {
                // If form data does not exist, it must be because of the sign in redirection. 
                // Read the cookie we saved before the redirection in RedirectToIdentityProvider callback in Startup.Auth.cs 
                var persistedRequestData = CookieStorage.Load(request);
                if (null != persistedRequestData)
                {
                    activationParameters = new FileHandlerActivationParameters(persistedRequestData);
                    return true;
                }
            }
            return false;
        }
    }
}
