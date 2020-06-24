using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IRepositoryManager
    {
        IAccountRepository Account { get; }
        IEventRepository Event { get; }
        IAccountFriendRepository AccountFriend { get; }
        IEventParticipantRepository EventParticipant { get; }
        void Save();
    }
}
