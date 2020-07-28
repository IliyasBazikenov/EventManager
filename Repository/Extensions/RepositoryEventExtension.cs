using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Extensions
{
    public static class RepositoryEventExtension
    {
        public static IQueryable<Event> FilterEvent(this IQueryable<Event> events, DateTime minEventDate, DateTime maxEventDate)
        {
            return events.Where(e => (e.DateOfEvent >= minEventDate && e.DateOfEvent <= maxEventDate));
        }

        public static IQueryable<Event> Search(this IQueryable<Event> events, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return events;

            var lowerCaseTerm = searchTerm.Trim().ToLower();

            return events.Where(e => e.EventName.Contains(lowerCaseTerm));
        }
    }
}
