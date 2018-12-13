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
    using System;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.OpenIdConnect;
    using Owin;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using PnpFileHandler.Utils;
    using Models;
    using Controllers;
    using Microsoft.IdentityModel.Tokens;

    public partial class Startup
    {
       
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions { });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = SettingsHelper.ClientId,
                    Authority = SettingsHelper.Authority,
                    ClientSecret = SettingsHelper.AppKey,
                    ResponseType = "code id_token",
                    Resource = "https://graph.microsoft.com",
                    PostLogoutRedirectUri = "/",

                    TokenValidationParameters = new TokenValidationParameters
                    {
                        // instead of using the default validation (validating against a single issuer value, as we do in line of business apps (single tenant apps)), 
                        // we turn off validation
                        //
                        // NOTE:
                        // * In a multitenant scenario you can never validate against a fixed issuer string, as every tenant will send a different one.
                        // * If you don’t care about validating tenants, as is the case for apps giving access to 1st party resources, you just turn off validation.
                        // * If you do care about validating tenants, think of the case in which your app sells access to premium content and you want to limit access only to the tenant that paid a fee, 
                        //       you still need to turn off the default validation but you do need to add logic that compares the incoming issuer to a list of tenants that paid you, 
                        //       and block access if that’s not the case.
                        // * Refer to the following sample for a custom validation logic: https://github.com/AzureADSamples/WebApp-WebAPI-MultiTenant-OpenIdConnect-DotNet
                        ValidateIssuer = false
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        SecurityTokenValidated = (context) =>
                        {
                            // If your authentication logic is based on users then add your logic here
                            return Task.FromResult(0);
                        },
                        AuthenticationFailed = (context) =>
                        {
                            // Pass in the context back to the app
                            string message = Uri.EscapeDataString(context.Exception.Message);
                            context.OwinContext.Response.Redirect("/Home/Error?msg=" + message);
                            context.HandleResponse(); // Suppress the exception
                            return Task.FromResult(0);
                        },
                        AuthorizationCodeReceived = async (context) =>
                        {
                            var code = context.Code;
                            ClientCredential credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.AppKey);

                            string tenantID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                            string signInUserId = context.AuthenticationTicket.Identity.FindFirst(AuthHelper.ObjectIdentifierClaim).Value;

                            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(SettingsHelper.Authority, 
                                new AzureTableTokenCache(signInUserId));

                            // Get the access token for AAD Graph. Doing this will also initialize the token cache associated with the authentication context
                            // In theory, you could acquire token for any service your application has access to here so that you can initialize the token cache
                            AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(code,
                                new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)),
                                credential,
                                SettingsHelper.AADGraphResourceId);
                        },
                        RedirectToIdentityProvider = (context) =>
                        {
                            // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                            // this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings
                            // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                            context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;

                            FileHandlerActivationParameters fileHandlerActivation;
                            if (GraphHelper.IsFileHandlerActivationRequest(new HttpRequestWrapper(HttpContext.Current.Request), out fileHandlerActivation))
                            {
                                // Add LoginHint and DomainHint if the request includes a form handler post
                                context.ProtocolMessage.LoginHint = fileHandlerActivation.UserId;
                                context.ProtocolMessage.DomainHint = "organizations";

                                // Save the form in the cookie to prevent it from getting lost in the login redirect
                                CookieStorage.Save(HttpContext.Current.Request.Form, HttpContext.Current.Response);
                            }

                            // Allow us to change the prompt in consent mode if the challenge properties specify a prompt type
                            var challengeProperties = context.OwinContext?.Authentication?.AuthenticationResponseChallenge?.Properties;
                            if (null != challengeProperties && challengeProperties.Dictionary.ContainsKey("prompt"))
                            {
                                context.ProtocolMessage.Prompt = challengeProperties.Dictionary["prompt"];
                            }

                            return Task.FromResult(0);
                        }
                    }
                });
        }
    }
}
