using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts(bool trackChanges);
        Account GetAccount(Guid accountId, bool trackChanges);
        void CreateAccount(Account account);
        IEnumerable<Account> GetByIds(IEnumerable<Guid> accountIds, bool trackChanges);
    }
}
