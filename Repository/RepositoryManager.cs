using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repositoryContext;
        private IAccountRepository _accountRepository;
        private IEventRepository _eventRepository;
        private IEventParticipantRepository _eventParticipantRepository;
        private IAccountFriendRepository _accountFriendRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public IEventRepository Event
        {
            get
            {
                if (_eventRepository == null)
                    _eventRepository = new EventRepository(_repositoryContext);

                return _eventRepository;
            }
        }
        public IAccountRepository Account
        {
            get
            {
                if (_accountRepository == null)
                    _accountRepository = new AccountRepository(_repositoryContext);

                return _accountRepository;
            }
        }

        public IEventParticipantRepository EventParticipant
        {
            get
            {
                if (_eventParticipantRepository == null)
                    _eventParticipantRepository = new EventParticipantRepository(_repositoryContext);

                return _eventParticipantRepository;
            }
        }
        public IAccountFriendRepository AccountFriend
        {
            get
            {
                if (_accountFriendRepository == null)
                    _accountFriendRepository = new AccountFriendRepository(_repositoryContext);

                return _accountFriendRepository;
            }
        }

        public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
    }
}
