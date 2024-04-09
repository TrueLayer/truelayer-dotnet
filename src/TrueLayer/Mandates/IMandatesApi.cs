using System.Threading;
using System.Threading.Tasks;
using OneOf;

namespace TrueLayer.Mandates
{
    using TrueLayer.Mandates.Model;
    using TrueLayer.Models;
    using AuthorizationResponseUnion = OneOf<
        Models.AuthorisationFlowResponse.AuthorizationFlowAuthorizing,
        Models.AuthorisationFlowResponse.AuthorizationFlowAuthorizationFailed>;
    using MandateDetailUnion = OneOf<
        Model.MandateDetail.AuthorizationRequiredMandateDetail,
        Model.MandateDetail.AuthorizingMandateDetail,
        Model.MandateDetail.AuthorizedMandateDetail,
        Model.MandateDetail.FailedMandateDetail,
        Model.MandateDetail.RevokedMandateDetail>;

    /// <summary>
    /// Provides access to the TrueLayer Payments API
    /// </summary>
    public interface IMandatesApi
    {
        /// <summary>
        /// Creates a new mandate
        /// </summary>
        /// <param name="mandateRequest">The mandate request details</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the created mandate if successful, otherwise problem details</returns>
        Task<ApiResponse<CreateMandateResponse>> CreateMandate(
            CreateMandateRequest mandateRequest, string idempotencyKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a mandate
        /// </summary>
        /// <param name="mandateId">The id of the mandate to retrieve</param>
        /// <param name="mandateType">The type of the mandate. Either sweeping or commercial</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the mandate if successful, otherwise problem details</returns>
        Task<ApiResponse<MandateDetailUnion>> GetMandate(
            string mandateId, MandateType mandateType , CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists mandates for a user
        /// </summary>
        /// <param name="query">The userId, cursor and limit of mandate</param>
        /// <param name="mandateType">The type of the mandate. Either sweeping or commercial</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the mandate if successful, otherwise problem details</returns>
        Task<ApiResponse<ResourceCollection<MandateDetailUnion>>> ListMandates(
            ListMandatesQuery query, MandateType mandateType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Start the authorization flow for a mandate.
        /// </summary>
        /// <param name="mandateId">The id of the mandate to retrieve</param>
        /// <param name="mandateType">The type of the mandate. Either sweeping or commercial</param>
        /// <param name="request">The start authorization flow request</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the mandate if successful, otherwise problem details</returns>
        Task<ApiResponse<AuthorizationResponseUnion>> StartAuthorizationFlow(
            string mandateId, StartAuthorizationFlowRequest request, string idempotencyKey, MandateType mandateType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Submit the provider details selected by the PSU.
        /// </summary>
        /// <param name="mandateId">The id of the mandate to retrieve</param>
        /// <param name="mandateType">The type of the mandate. Either sweeping or commercial</param>
        /// <param name="request">The provider selection request</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the mandate if successful, otherwise problem details</returns>
        Task<ApiResponse<AuthorizationResponseUnion>> SubmitProviderSelection(
            string mandateId, SubmitProviderSelectionRequest request, string idempotencyKey, MandateType mandateType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Submit the consent given by the user
        /// </summary>
        /// <param name="mandateId">The id of the mandate</param>
        /// <param name="mandateType">The type of the mandate. Either sweeping or commercial</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes the authorization flow action details if successful, otherwise problem details</returns>
        Task<ApiResponse<AuthorizationResponseUnion>> SubmitConsent(string mandateId, string idempotencyKey, MandateType mandateType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Confirmation Of Funds
        /// </summary>
        /// <param name="mandateId">The id of the mandate to retrieve</param>
        /// <param name="mandateType">The type of the mandate. Either sweeping or commercial</param>
        /// <param name="amountInMinor">The amount to be confirmed present in the bank account</param>
        /// <param name="currency">The currency of the mandate</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the mandate if successful, otherwise problem details</returns>
        Task<ApiResponse<GetConfirmationOfFundsResponse>> GetConfirmationOfFunds(
            string mandateId, int amountInMinor, string currency, MandateType mandateType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a mandates constraints
        /// </summary>
        /// <param name="mandateId">The id of the mandate constraints to retrieve</param>
        /// <param name="mandateType">The type of the mandate. Either sweeping or commercial</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes details of the mandate if successful, otherwise problem details</returns>
        Task<ApiResponse<GetConstraintsResponse>> GetMandateConstraints(
            string mandateId, MandateType mandateType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revoke mandate
        /// </summary>
        /// <param name="mandateId">The id of the mandate</param>
        /// <param name="idempotencyKey">
        /// An idempotency key to allow safe retrying without the operation being performed multiple times.
        /// The value should be unique for each operation, e.g. a UUID, with the same key being sent on a retry of the same request.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation</param>
        /// <returns>An API response that includes the payment details if successful, otherwise problem details</returns>
        Task<ApiResponse> RevokeMandate(string mandateId, string idempotencyKey, MandateType mandateType, CancellationToken cancellationToken = default);
    }
}
