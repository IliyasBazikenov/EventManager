using Entities.Models;
using Repository.Extensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repository.Extensions
{
    public static class RepositoryAccountExtension
    {
        public static IQueryable<Account> Search(this IQueryable<Account> accounts, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return accounts;

            var lowerCaseTerm = searchTerm.Trim().ToLower();

            return accounts.Where(a => (a.FirstName.Trim() + " " + a.LastName.Trim()).Contains(searchTerm) );
        }

        public static IQueryable<Account> Sort(this IQueryable<Account> accounts, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return accounts.OrderBy(a => a.FirstName);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Account>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return accounts.OrderBy(a => a.FirstName);

            return accounts.OrderBy(orderQuery);
        }
    }
}
