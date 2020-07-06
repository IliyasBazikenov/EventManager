using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Account> GetAllAccounts(bool trackChanges)
        {
            return FindAll(trackChanges)
                .OrderBy(a => a.FirstName)
                .ToList();
        }

        public Account GetAccount(Guid accountId, bool trackChanges)
        {
            return FindByCondition(a => a.AccountId.Equals(accountId), trackChanges)
                .SingleOrDefault();
        }

        public void CreateAccount(Account account)
        {
            Create(account);
        }

        public IEnumerable<Account> GetByIds(IEnumerable<Guid> accountIds, bool trackChanges)
        {
            return FindByCondition(a => accountIds.Contains(a.AccountId), trackChanges)
                .ToList();
        }
    }
}
