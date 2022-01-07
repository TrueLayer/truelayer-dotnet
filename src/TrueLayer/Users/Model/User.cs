namespace TrueLayer.Users.Model
{
    /// <summary>
    /// Represents an end user
    /// </summary>
    public record User(string Id, string? Name = null, string? Email = null, string? Phone = null);
}
