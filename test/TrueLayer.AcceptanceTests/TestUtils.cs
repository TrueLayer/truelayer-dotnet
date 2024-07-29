using System;

namespace TrueLayer.AcceptanceTests;

public static class TestUtils
{
    public class HeadlessResourceAuthorization
    {
        public HeadlessResourceAuthorization(HeadlessResource resource, HeadlessResourceAction action)
        {
            Resource = resource;
            Action = action;
        }

        public HeadlessResource Resource { get; }
        public HeadlessResourceAction Action { get; }
        public string Path { get; set; } = null!;
        public string Payload { get; set; } = null!;
    }

    public enum HeadlessResourceAction
    {
        Invalid,
        Execute,
        Authorize,
        RejectAuthorization
    }

    public enum HeadlessResource {
        Invalid,
        Payments,
        Mandates
    }

    public static HeadlessResourceAuthorization RunAndAssertHeadlessResourceAuthorisation(
        HeadlessResource resource, HeadlessResourceAction action)
    {
        HeadlessResourceAuthorization testHeadlessResourceAuthorization =
            new HeadlessResourceAuthorization(resource, action);
        testHeadlessResourceAuthorization.Payload = $"{{\"action\":\"{action}\", \"redirect\": false}}";

        return resource switch
        {
            HeadlessResource.Payments => testHeadlessResourceAuthorization,
            HeadlessResource.Mandates => testHeadlessResourceAuthorization,
            _ => throw new ArgumentOutOfRangeException(nameof(resource), resource, null)
        };
    }
}
