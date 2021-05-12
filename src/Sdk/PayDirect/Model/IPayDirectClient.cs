using System.Threading;
using System.Threading.Tasks;

namespace TrueLayer.PayDirect.Model
{
    public interface IPayDirectClient
    {
        /// <summary>
        /// Retrieves the details and balances of your accounts
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<QueryResponse<AccountBalance>> GetAccountBalances(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Initiate a deposit into your account
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ApiResponse<InitiateDepositResponse>> InitiateDeposit(InitiateDepositRequest request, CancellationToken cancellationToken = default);
    }
}
