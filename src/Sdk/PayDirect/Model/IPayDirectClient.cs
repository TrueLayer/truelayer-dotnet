using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

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


        /// <summary>
        /// Retrieves the details and status of a deposit
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="depositId">The deposit identifier</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Deposit> GetDeposit(Guid userId, Guid depositId, CancellationToken cancellationToken = default);
    }
}
