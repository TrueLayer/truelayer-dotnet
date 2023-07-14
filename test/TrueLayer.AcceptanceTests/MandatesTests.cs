using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OneOf;
using Shouldly;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    using TrueLayer.Mandates.Model;

    public class MandatesTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public MandatesTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        public static IEnumerable<object[]> CreateTestMandateRequestTestData()
        {
            yield return new object[]
            {
                new CreateMandateRequest(
                    new OneOf<Mandate.VRPCommercialMandate, Mandate.VRPSweepingMandate>(),
                    Currencies.GBP,
                    new PaymentUserRequest(
                        name: "Jane Doe",
                        email: "jane.doe@example.com",
                        phone: "+442079460087",
                        dateOfBirth: new DateTime(1999, 1, 1),
                        address: new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St")),
                    new Constraints(ValidFrom: "2023-07-14", ValidTo: "2023-07-21", 500, new PeriodicLimits(
                        new Limit(1, PeriodAlignment.Consent, "100"),
                        new Limit(1, PeriodAlignment.Consent, "100"),
                        new Limit(1, PeriodAlignment.Consent, "100"),
                        new Limit(1, PeriodAlignment.Consent, "100"),
                        new Limit(1, PeriodAlignment.Consent, "100"),
                        new Limit(1, PeriodAlignment.Consent, "100")
                    )),
                    new Dictionary<string, string>()
                )
            };
        }

        [Theory]
        [MemberData(nameof(CreateTestMandateRequestTestData))]
        public async Task Can_create_mandate(CreateMandateRequest mandateRequest)
        {
            // Act
            var response = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Data!.id.ShouldNotBeNullOrWhiteSpace();
            response.Data.resourceToken.ShouldNotBeNullOrWhiteSpace();
            response.Data.user.ShouldNotBeNull();
            response.Data.user.Id.ShouldNotBeNullOrWhiteSpace();
        }

        [Theory]
        [MemberData(nameof(CreateTestMandateRequestTestData))]
        public async Task Can_revoke_mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.id;

            // Act
            var response = await _fixture.Client.Mandates.RevokeMandate(
                mandateId, idempotencyKey: Guid.NewGuid().ToString());

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
    }
}
