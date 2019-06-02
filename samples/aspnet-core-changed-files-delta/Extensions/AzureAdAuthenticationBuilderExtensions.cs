using AspnetCore_Changed_Files.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCore_Changed_Files.Extensions
{
    public static class AzureAdAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
            => builder.AddAzureAd(_ => { });

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        public class ConfigureAzureOptions : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureAdOptions _azureOptions;
            private readonly IGraphAuthProvider _authProvider;

            public AzureAdOptions GetAzureAdOptions() => _azureOptions;

            public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions, IGraphAuthProvider authProvider)
            {
                _authProvider = authProvider;
                _azureOptions = azureOptions.Value;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}common/v2.0";
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;
                options.RequireHttpsMetadata = false;
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                var allScopes = $"{_azureOptions.Scopes} {_azureOptions.GraphScopes}".Split(new[] { ' ' });
                foreach (var scope in allScopes) { options.Scope.Add(scope); }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Ensure that User.Identity.Name is set correctly after login
                    NameClaimType = "name",

                    // Instead of using the default validation (validating against a single issuer value, as we do in line of business apps),
                    // we inject our own multitenant validation logic
                    ValidateIssuer = false,

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                        // If your authentication logic is based on users then add your logic here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Home/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = async (context) =>
                    {
                        string code = context.ProtocolMessage.Code;
                        // Get access token using authorization code
                        AuthenticationResult result = await _authProvider.GetUserAccessTokenByAuthorizationCode(code);

                        context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                    },
                    // If your application needs to do authenticate single users, add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                };
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
