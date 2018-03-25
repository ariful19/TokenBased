using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace tockenBased.Models
{
    public class ApplicationAuthenticationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IEnumerable<string> vlus = null;
            if (request.Headers.TryGetValues("Token", out vlus))
            {
                string tok = vlus.First();
                if (string.IsNullOrEmpty(tok))
                {
                    var exreq = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    return base.SendAsync(request, cancellationToken).ContinueWith(task =>
                    {
                        return exreq;
                    });
                }
                TokenObj obj = TokenObj.Get(tok);
                if (!obj.isExpired())
                {
                    var userNameClaim = new Claim(ClaimTypes.Name, obj.userid);
                    var identity = new ClaimsIdentity(new[] { userNameClaim }, obj.ToTokenString());
                    var principal = new ClaimsPrincipal(identity);
                    Thread.CurrentPrincipal = principal;
                    if (System.Web.HttpContext.Current != null)
                    {
                        System.Web.HttpContext.Current.User = principal;
                    }

                    obj.AddTime();
                    var greq = request.CreateResponse(System.Net.HttpStatusCode.OK);
                    greq.Headers.Add("Token", obj.ToTokenString());
                    return base.SendAsync(request, cancellationToken).ContinueWith(task =>
                    {
                        return greq;
                    });
                }
                else
                {
                    var exreq = request.CreateResponse(System.Net.HttpStatusCode.Gone);
                    exreq.Headers.Add("Token", "false");
                    return base.SendAsync(request, cancellationToken).ContinueWith(task =>
                    {
                        return exreq;
                    });
                }
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}