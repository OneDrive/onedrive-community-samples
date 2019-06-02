using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspnetCore_Changed_Files.Helpers
{
    public class GraphSdkHelper : IGraphSdkHelper
    {
        private const string ObjectIdentifierType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private const string TenantIdType = "http://schemas.microsoft.com/identity/claims/tenantid";

        private readonly IGraphAuthProvider _authProvider;
        private GraphServiceClient _graphClient;

        public GraphSdkHelper(IGraphAuthProvider authProvider)
        {
            _authProvider = authProvider;
        }

        /// <summary>
        /// Get an authenticated Microsoft Graph Service client.
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        public GraphServiceClient GetAuthenticatedClient(ClaimsIdentity userIdentity)
        {
            _graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(
                async requestMessage =>
                {
                    // Get user's id for token cache.
                    var identifier = userIdentity.FindFirst(ObjectIdentifierType)?.Value + "." + userIdentity.FindFirst(TenantIdType)?.Value;

                    // Passing tenant ID to the sample auth provider to use as a cache key
                    var accessToken = await _authProvider.GetUserAccessTokenAsync(identifier);

                    // Append the access token to the request
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }));

            return _graphClient;
        }

    }

    public interface IGraphSdkHelper
    {
        GraphServiceClient GetAuthenticatedClient(ClaimsIdentity userIdentity);
    }
}
