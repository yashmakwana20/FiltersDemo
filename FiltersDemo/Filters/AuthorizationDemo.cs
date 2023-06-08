using System.Web.Http;
using System.Web.Http.Controllers;

namespace FiltersDemo.Filters
{
    public class AuthorizationDemo : AuthorizeAttribute
    {
        private readonly IConfiguration _configuration;

        public AuthorizationDemo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            IEnumerable<string> apiKeys;
            if (actionContext.Request.Headers.TryGetValues("apiKey",out apiKeys))
            {
                base.OnAuthorization(actionContext);
            }
            else
            {
                TokenManager tm = new TokenManager(_configuration);
                if (!tm.validateToken(apiKeys.FirstOrDefault()))
                {
                    base.OnAuthorization(actionContext);
                }
            }
        }

    }
}
