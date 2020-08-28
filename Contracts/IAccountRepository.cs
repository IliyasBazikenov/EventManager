using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAccountRepository
    {
        Task<PagedList<Account>> GetAccountsAsync(AccountParameters accountParameters, bool trackChanges);
        Task<Account> GetAccountAsync(Guid accountId, bool trackChanges);
        void CreateAccount(Account account);
        Task<IEnumerable<Account>> GetByIdsAsync(IEnumerable<Guid> accountIds, bool trackChanges);
        void DeleteAccount(Account account);
    }
}
