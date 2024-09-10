namespace TrueLayer.Models
{
    /// <summary>
    /// Pagination object. Includes the next cursor to use as query parameters to fetch the page next to the one fetched.
    /// </summary>
    /// <param name="NextCursor">Cursor to be used as cursor query parameter to fetch the next page in pagination-ready endpoints.</param>
    public record Pagination(string NextCursor);
}
