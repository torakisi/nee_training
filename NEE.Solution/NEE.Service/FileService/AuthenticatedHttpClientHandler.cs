using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NEE.Service.FileService
{
    public class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        private readonly string _authToken;

        public AuthenticatedHttpClientHandler(string authToken)
        {
            _authToken = authToken ?? throw new ArgumentNullException(nameof(authToken));
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // See if the request has an authorize header
            var auth = request.Headers.Authorization;
            if (auth == null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _authToken);
            }

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }


}
