using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class AccountFriendRepository : RepositoryBase<AccountFriend>, IAccountFriendRepository
    {
        public AccountFriendRepository(RepositoryContext repositoryContext) :
            base(repositoryContext)
        {

        }
    }
}
