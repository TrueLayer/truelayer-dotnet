using System.Threading;
using System.Threading.Tasks;
using OneOf;

namespace TrueLayer.Mandates
{
    using TrueLayer.Mandates.Model;
    using MandateDetailUnion = OneOf<Model.MandateDetail.AuthorizationRequiredMandateDetail, Model.MandateDetail.AuthorizingMandateDetail, Model.MandateDetail.AuthorizedMandateDetail, Model.MandateDetail.FailedMandateDetail, Model.MandateDetail.RevokedMandateDetail>;

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
    }
}
