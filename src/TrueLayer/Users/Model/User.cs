namespace TrueLayer.Users.Model
{
    /// <summary>
    /// Represents an end user
    /// </summary>
    public record User(string Id, string Name, string? Email = null, string? Phone = null);
}
