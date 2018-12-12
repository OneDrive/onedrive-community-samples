using System;
using System.Net.Http.Headers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace FilesExplorer.Services
{
    /// <summary>
    /// Class which mantains the user session token and gives access to Microsoft Graph API
    /// </summary>
    public class AuthenticationService
    {

        /// <summary>
        /// The scopes to use when user grants the application
        /// </summary>
        private static string[] Scopes = { "User.Read", "Files.ReadWrite" };

        /// <summary>
        /// Keeps the identity of the current user
        /// </summary>
        private readonly PublicClientApplication _identityClientApp;

        public AuthenticationService(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentException("Client id cannot be null or empty", nameof(clientId));
            }

            ClientId = clientId;
            _identityClientApp = new PublicClientApplication(clientId);
            GraphClient = GetAuthenticatedClient();
        }

        /// <summary>
        /// Gets the current application id
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Gets the token of the current user
        /// </summary>
        public string TokenForUser { get; private set; }

        /// <summary>
        /// Gets when current user's token expires
        /// </summary>
        public DateTimeOffset Expiration { get; private set; }

        /// <summary>
        /// Gets the client which allows to interact with the current user
        /// </summary>
        public GraphServiceClient GraphClient { get; private set; }

        /// <summary>
        /// Gets whether the user is logged in or not
        /// </summary>
        public bool IsConnected => TokenForUser != null && Expiration > DateTimeOffset.UtcNow.AddMinutes(1);

        /// <summary>
        /// Gets a new istance of <see cref="GraphServiceClient"/>
        /// </summary>
        /// <returns></returns>
        private GraphServiceClient GetAuthenticatedClient()
        {
            // Create Microsoft Graph client.
            return new GraphServiceClient(
                "https://graph.microsoft.com/v1.0",
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        await AcquireTokenForUserAsync();
                        // Set bearer authentication on header
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", TokenForUser);
                    }));
        }


        /// <summary>
        /// Get Token for User.
        /// </summary>
        /// <returns>Token for user.</returns>
        private async Task AcquireTokenForUserAsync()
        {
            // Get an access token for the given context and resourceId. An attempt is first made to 
            // acquire the token silently. If that fails, then we try to acquire the token by prompting the user.
            var accounts = await _identityClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    var authResult = await _identityClientApp.AcquireTokenSilentAsync(Scopes, accounts.First());
                    TokenForUser = authResult.AccessToken;
                    Expiration = authResult.ExpiresOn;
                    return;
                }
                catch (Exception)
                {
                    TokenForUser = null;
                    Expiration = DateTimeOffset.MinValue;
                }
            }

            // Cannot get the token silently. Ask user to log in
            if (!IsConnected)
            {
                var authResult = await _identityClientApp.AcquireTokenAsync(Scopes);

                // Set access token and expiration
                TokenForUser = authResult.AccessToken;
                Expiration = authResult.ExpiresOn;
            }
        }

        /// <summary>
        /// Signs the user out of the service.
        /// </summary>
        public async Task SignOutAsync()
        {
            foreach (IAccount account in await _identityClientApp.GetAccountsAsync())
            {
                await _identityClientApp.RemoveAsync(account);
            }
            GraphClient = GetAuthenticatedClient();

            // Reset token and expiration
            Expiration = DateTimeOffset.MinValue;
            TokenForUser = null;
        }

    }
}
