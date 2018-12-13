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
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Utils;

    public static class AuthHelper
    {
        public const string ObjectIdentifierClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private static ClientCredential clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.AppKey);
        private const string AuthContextCacheKey = "authContext";

        /// <summary>
        /// Silently retrieve a new access token for the specified resource. If the request fails, null is returned.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static async Task<string> GetUserAccessTokenSilentAsync(string resource)
        {
            string signInUserId = GetUserId();
            if (!string.IsNullOrEmpty(signInUserId))
            {
                AuthenticationContext authContext = null;

                // Cache the authentication context in the session, so we don't have to reconstruct the cache for every call
                var session = System.Web.HttpContext.Current?.Session;
                if (session != null)
                {
                    authContext = session[AuthContextCacheKey] as AuthenticationContext;
                }

                if (authContext == null)
                {
                    authContext = new AuthenticationContext(SettingsHelper.Authority, false, new AzureTableTokenCache(signInUserId));

                    // Store the newly created authContext into the session cache
                    if (session != null)
                    {
                        session[AuthContextCacheKey] = authContext;
                    }
                }

                try
                {
                    var userCredential = new UserIdentifier(signInUserId, UserIdentifierType.UniqueId);
                    var authResult = await authContext.AcquireTokenSilentAsync(resource, clientCredential, userCredential);
                    return authResult.AccessToken;
                }
                catch (AdalSilentTokenAcquisitionException)
                {
                    // We don't really care about why we couldn't get a token silently, since the resolution will always be the same
                }
            }
            return null;
        }

        /// <summary>
        /// Return the signed in user's identifier
        /// </summary>
        /// <returns></returns>
        public static string GetUserId()
        {
            var claim = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(ObjectIdentifierClaim);
            if (null != claim)
            {
                return claim.Value;
            }
            return null;
        }
    }
}