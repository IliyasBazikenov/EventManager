using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEventRepository
    {
        Task<PagedList<Event>> GetEventsAsync(Guid accountId, EventParameters eventParameters, bool trackChanges);
        Task<Event> GetEventAsync(Guid accountId, int eventId, bool trackChanges);
        void CreateEvent(Guid accountId, Event eventEntity);
        void DeleteEvent(Event eventEntity);
    }
}
