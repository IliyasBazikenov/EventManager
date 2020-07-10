using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public EventRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public Event GetEvent(Guid accountId, int eventId, bool trackChanges)
        {
            return FindByCondition(e => e.AccountId.Equals(accountId) && e.EventId.Equals(eventId), trackChanges)
                .SingleOrDefault();
        }

        public IEnumerable<Event> GetEvents(Guid accountId, bool trackChanges)
        {
            return FindByCondition(e => e.AccountId.Equals(accountId), trackChanges)
                .OrderBy(e => e.EventId)
                .ToList();
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
