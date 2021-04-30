namespace TrueLayer
{
    /// <summary>
    /// Validation error response returned by TrueLayer APIs
    /// </summary>
    public class ErrorResponse
    {
        public string Error { get; set; }
        public string ErrorDescription { get; set; }
        public ErrorDetails ErrorDetails { get; set; }
    }
}
