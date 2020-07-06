using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetEvents(Guid accountId, bool trackChanges);
        Event GetEvent(Guid accountId, int eventId,bool trackChanges);
        void CreateEvent(Guid accountId, Event eventEntity);
    }
}
