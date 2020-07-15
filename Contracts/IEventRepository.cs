using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetEventsAsync(Guid accountId, bool trackChanges);
        Task<Event> GetEventAsync(Guid accountId, int eventId,bool trackChanges);
        void CreateEvent(Guid accountId, Event eventEntity);
        void DeleteEvent(Event eventEntity);
    }
}
