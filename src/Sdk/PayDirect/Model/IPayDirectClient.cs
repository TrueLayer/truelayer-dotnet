using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TrueLayer.PayDirect.Model
{
    public interface IPayDirectClient
    {
        /// <summary>
        /// Retrieves the details and balances of your accounts
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<AccountBalance>> GetAccountBalances(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Initiate a deposit into your account
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<InitiateDepositResponse> InitiateDeposit(InitiateDepositRequest request, CancellationToken cancellationToken = default);
    }
}
