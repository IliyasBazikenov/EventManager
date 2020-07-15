using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        IAccountRepository Account { get; }
        IEventRepository Event { get; }
        IAccountFriendRepository AccountFriend { get; }
        IEventParticipantRepository EventParticipant { get; }
        Task SaveAsync();
    }
}
