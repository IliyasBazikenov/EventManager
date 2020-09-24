using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
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

        public async Task<PagedList<Account>> GetAccountsAsync(AccountParameters accountParameters, bool trackChanges)
        {
            var accounts = await FindAll(trackChanges)
                .Search(accountParameters.SearchTerm)
                .Sort(accountParameters.OrderBy)
                .ToListAsync();

            return PagedList<Account>.ToPagedList(accounts, accountParameters.PageNumber, accountParameters.PageSize); 
        }

        public async Task<Account> GetAccountAsync(Guid accountId, bool trackChanges)
        {
            return await FindByCondition(a => a.Id.Equals(accountId), trackChanges)
                .SingleOrDefaultAsync();
        }

        public void CreateAccount(Account account)
        {
            Create(account);
        }

        public async Task<IEnumerable<Account>> GetByIdsAsync(IEnumerable<Guid> accountIds, bool trackChanges)
        {
            return await FindByCondition(a => accountIds.Contains(a.Id), trackChanges)
                .ToListAsync();
        }

        public void DeleteAccount(Account account)
        {
            Delete(account);
        }
    }
}
