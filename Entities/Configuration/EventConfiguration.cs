using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Configuration
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasData
                (
                   new Event
                   {
                       EventId= Guid.NewGuid(),
                       CreatedDate = DateTime.Now,
                       EventName = "Туса у децла",
                       ParticipantAmount = 0,
                       AccountId = Guid.Parse("e5dbaaab-1c30-47cd-931d-e2cf9efe20b1"),
                       DateOfEvent = DateTime.Parse("21-07-2020"),
                   }
                );
        }
    }
}
