using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    class EventParticipantRepository : RepositoryBase<EventParticipant>, IEventParticipantRepository
    {
        public EventParticipantRepository(RepositoryContext repositoryContext) : 
            base (repositoryContext)
        {
                
        }
    }
}
