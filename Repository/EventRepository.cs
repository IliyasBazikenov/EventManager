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
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public EventRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<Event> GetEventAsync(Guid accountId, Guid eventId, bool trackChanges)
        {
            return await FindByCondition(e => e.AccountId.Equals(accountId) && e.EventId.Equals(eventId), trackChanges)
                .SingleOrDefaultAsync();

        }

        public async Task<PagedList<Event>> GetEventsAsync(Guid accountId, EventParameters eventParameters, bool trackChanges)
        {
            var events = await FindByCondition(e => e.AccountId.Equals(accountId), trackChanges)
                .FilterByDate(eventParameters.MinEventDate, eventParameters.MaxEventDate)
                .Search(eventParameters.SearchTerm)
                .Sort(eventParameters.OrderBy)
                .ToListAsync();
            return PagedList<Event>.ToPagedList(events, eventParameters.PageNumber, eventParameters.PageSize);
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
