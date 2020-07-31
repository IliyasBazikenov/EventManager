using Entities.Models;
using Repository.Extensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace Repository.Extensions
{
    public static class RepositoryEventExtension
    {
        public static IQueryable<Event> FilterByDate(this IQueryable<Event> events, DateTime minEventDate, DateTime maxEventDate)
        {
            if (minEventDate.CompareTo(DateTime.Today) == 0 && maxEventDate.CompareTo(DateTime.MaxValue) == 0)
                return events;

            return events.Where(e => (e.DateOfEvent >= minEventDate && e.DateOfEvent <= maxEventDate));
        }

        public static IQueryable<Event> Search(this IQueryable<Event> events, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return events;

            var lowerCaseTerm = searchTerm.Trim().ToLower();

            return events.Where(e => e.EventName.Contains(lowerCaseTerm));
        }

        public static IQueryable<Event> Sort(this IQueryable<Event> events, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return events.OrderBy(e => e.EventName);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Event>(orderByQueryString);
            
            if (string.IsNullOrWhiteSpace(orderQuery))
                return events.OrderBy(e => e.EventName);

            return events.OrderBy(orderQuery);
        }
    }
}
