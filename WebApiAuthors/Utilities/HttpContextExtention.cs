using Microsoft.EntityFrameworkCore;

namespace WebApiAuthors.Utilities
{
    public static class HttpContextExtention
    {
        public async static Task InsertParamtersHeadPagination<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }


            double count = await queryable.CountAsync();
            httpContext.Response.Headers.Add("TotalRegistrationAmount", count.ToString());
        }
    }
}
