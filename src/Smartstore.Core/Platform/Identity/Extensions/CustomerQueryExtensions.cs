using Microsoft.EntityFrameworkCore.Query;
using Smartstore.Core.Data;

namespace Smartstore.Core.Identity
{
    public static partial class CustomerQueryExtensions
    {
        /// <summary>
        /// Includes the the customer roles graph for eager loading.
        /// </summary>
        public static IIncludableQueryable<Customer, CustomerRole> IncludeCustomerRoles(this IQueryable<Customer> query)
        {
            Guard.NotNull(query, nameof(query));

            return query
                .Include(x => x.CustomerRoleMappings)
                .ThenInclude(x => x.CustomerRole);
        }

        /// <summary>
        /// Selects a customer by <see cref="Customer.Email"/>, <see cref="Customer.Username"/> or <see cref="Customer.CustomerNumber"/> (in that particular order).
        /// </summary>
        /// <param name="exactMatch">Whether to perform an exact or partial field match.</param>
        public static IQueryable<Customer> ApplyIdentFilter(this IQueryable<Customer> query,
            string email = null,
            string userName = null,
            string customerNumber = null,
            bool exactMatch = false)
        {
            Guard.NotNull(query, nameof(query));

            if (email.HasValue())
            {
                query = exactMatch ? query.Where(c => c.Email == email) : query.Where(c => c.Email.Contains(email));
            }

            return query;
        }

        /// <summary>
        /// Selects customers by <see cref="Customer.FullName"/> or <see cref="Customer.Company"/>.
        /// </summary>
        /// <param name="exactMatch">Whether to perform an exact or partial field match.</param>
        public static IQueryable<Customer> ApplySearchTermFilter(this IQueryable<Customer> query, string term, bool exactMatch = false)
        {
            Guard.NotNull(query, nameof(query));
            Guard.NotEmpty(term, nameof(term));

            query = exactMatch
                ? query.Where(c => c.FullName == term)
                : query.Where(c => c.FullName.Contains(term));

            return query;
        }

        /// <summary>
        /// Selects customers who have registered within a given time period and orders by <see cref="Customer.CreatedOnUtc"/> descending.
        /// </summary>
        /// <param name="fromUtc">Earliest (inclusive)</param>
        /// <param name="toUtc">Latest (inclusive)</param>
        public static IOrderedQueryable<Customer> ApplyRegistrationFilter(this IQueryable<Customer> query, DateTime? fromUtc, DateTime? toUtc)
        {
            Guard.NotNull(query, nameof(query));

            if (fromUtc.HasValue)
            {
                query = query.Where(c => fromUtc.Value <= c.CreatedOnUtc);
            }

            if (toUtc.HasValue)
            {
                query = query.Where(c => toUtc.Value >= c.CreatedOnUtc);
            }

            return query.OrderByDescending(x => x.CreatedOnUtc);
        }

        /// <summary>
        /// Selects customers who have been active within a given time period and orders by <see cref="Customer.LastActivityDateUtc"/> descending.
        /// </summary>
        /// <param name="fromUtc">Earliest (inclusive)</param>
        /// <param name="toUtc">Latest (inclusive)</param>
        public static IOrderedQueryable<Customer> ApplyLastActivityFilter(this IQueryable<Customer> query, DateTime? fromUtc, DateTime? toUtc)
        {
            Guard.NotNull(query, nameof(query));

            if (fromUtc.HasValue)
            {
                query = query.Where(c => fromUtc.Value <= c.LastActivityDateUtc);
            }

            if (toUtc.HasValue)
            {
                query = query.Where(c => toUtc.Value >= c.LastActivityDateUtc);
            }

            return query.OrderByDescending(x => x.LastActivityDateUtc);
        }

        /// <summary>
        /// Selects customers who are assigned to given customer roles.
        /// </summary>
        public static IQueryable<Customer> ApplyRolesFilter(this IQueryable<Customer> query, params int[] roleIds)
        {
            Guard.NotNull(query, nameof(query));

            if (roleIds.Length > 0)
            {
                var db = query.GetDbContext<SmartDbContext>();

                var customerIdsByRolesQuery = db.CustomerRoleMappings
                    .AsNoTracking()
                    .Where(x => roleIds.Contains(x.CustomerRoleId))
                    .Select(x => x.CustomerId);

                query = query.Where(x => customerIdsByRolesQuery.Contains(x.Id));
            }

            return query;
        }

        /// <summary>
        /// Selects customers who are currently online since <paramref name="minutes"/> and orders by <see cref="Customer.LastActivityDateUtc"/> descending.
        /// </summary>
        /// <param name="minutes"></param>
        public static IOrderedQueryable<Customer> ApplyOnlineCustomersFilter(this IQueryable<Customer> query, int minutes = 20)
        {
            Guard.NotNull(query, nameof(query));

            var fromUtc = DateTime.UtcNow.AddMinutes(-minutes);

            return query
                .Where(c => c.IsSystemAccount == false)
                .ApplyLastActivityFilter(fromUtc, null);
        }

        /// <summary>
        /// Selects customers who use given password <paramref name="format"/>.
        /// </summary>
        public static IQueryable<Customer> ApplyPasswordFormatFilter(this IQueryable<Customer> query, PasswordFormat format)
        {
            Guard.NotNull(query, nameof(query));

            int passwordFormatId = (int)format;
            return query.Where(c => c.PasswordFormatId == passwordFormatId);
        }
    }
}
