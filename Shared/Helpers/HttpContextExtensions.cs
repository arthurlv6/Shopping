using Microsoft.AspNetCore.Http;


namespace Shared
{
    public static class HttpContextExtensions
    {
        public static void InsertPaginationParameterInResponse(this HttpContext httpContext,
            double pagesQuantity)
        {
            httpContext.Response.Headers.Add("pagesQuantity", pagesQuantity.ToString());
        }
    }
}
