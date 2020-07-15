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
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public EventRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<Event> GetEventAsync(Guid accountId, int eventId, bool trackChanges)
        {
            return await FindByCondition(e => e.AccountId.Equals(accountId) && e.EventId.Equals(eventId), trackChanges)
                .SingleOrDefaultAsync();

        }

        public async Task<IEnumerable<Event>> GetEventsAsync(Guid accountId, bool trackChanges)
        {
            return await FindByCondition(e => e.AccountId.Equals(accountId), trackChanges)
                .OrderBy(e => e.EventId)
                .ToListAsync();
        }

        public void CreateEvent(Guid accountId, Event eventEntity)
        {
            eventEntity.AccountId = accountId;
            Create(eventEntity);
        }

        public void DeleteEvent(Event eventEntity)
        {
            Delete(eventEntity);
        }
    }
}
