using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .OrderBy(a => a.FirstName)
                .ToListAsync();
        }

        public async Task<Account> GetAccountAsync(Guid accountId, bool trackChanges)
        {
            return await FindByCondition(a => a.AccountId.Equals(accountId), trackChanges)
                .SingleOrDefaultAsync();
        }

        public void CreateAccount(Account account)
        {
            Create(account);
        }

        public async Task<IEnumerable<Account>> GetByIdsAsync(IEnumerable<Guid> accountIds, bool trackChanges)
        {
            return await FindByCondition(a => accountIds.Contains(a.AccountId), trackChanges)
                .ToListAsync();
        }

        public void DeleteAccount(Account account)
        {
            Delete(account);
        }
    }
}
