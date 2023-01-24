using WebApiAuthors.DTOs;

namespace WebApiAuthors.Utilities
{
    public static class IQueryableExtentions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable.Skip((paginationDTO.Page - 1) * paginationDTO.RecordsForPage)
                .Take(paginationDTO.RecordsForPage);
        }
    }
}
