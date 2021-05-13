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
        /// Initiates a deposit for the specified user and registers their account details
        /// for future closed loop withdrawals
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DepositResponse> Deposit(DepositRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the details and status of a deposit
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="depositId">The deposit identifier</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Deposit> GetDeposit(Guid userId, Guid depositId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the accounts an end-user has previously deposited from
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<UserAcccount>> GetUserAcccounts(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiate a closed-loop withdrawal for the specified user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WithdrawalResponse> Withdraw(UserWithdrawalRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initiate an open-loop withdrawal for the specified user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WithdrawalResponse> Withdraw(WithdrawalRequest request, CancellationToken cancellationToken = default);
    }
}
