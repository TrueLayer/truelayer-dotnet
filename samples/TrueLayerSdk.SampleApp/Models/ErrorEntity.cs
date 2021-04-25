namespace TrueLayerSdk.SampleApp.Models
{
    public class ErrorEntity
    {
        public string ErrorCode { get; }
        public string ErrorDescription { get; }
        public string State { get; }

        public ErrorEntity(string errorCode, string errorDescription, string state = null)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
            State = state;
        }
    }
}
