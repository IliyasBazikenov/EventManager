using Contracts;
using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.Utility
{
    public class AccountLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<AccountDTO> _dataShaper;

        public AccountLinks(LinkGenerator linkGenerator, IDataShaper<AccountDTO> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<AccountDTO> accountDTOs,
            string fields, HttpContext httpContext)
        {
            List<Entity> shapedAccounts = ShapeData(accountDTOs, fields);

            if (ShouldGenerateLinks(httpContext))
                return ReturnLinkedAccounts(accountDTOs, fields, httpContext, shapedAccounts);

            return ReturnShapedAccounts(shapedAccounts);
        }

        private LinkResponse ReturnShapedAccounts(List<Entity> shapedAccounts)
        {
            return new LinkResponse { ShapedEntities = shapedAccounts };
        }

        private LinkResponse ReturnLinkedAccounts(IEnumerable<AccountDTO> accountDTOs, string fields, HttpContext httpContext, List<Entity> shapedAccounts)
        {
            var accountDTOsList = accountDTOs.ToList();

            for (int index = 0; index < accountDTOsList.Count; index++)
            {
                List<Link> accountLinks = CreateLinksForAccount(httpContext, accountDTOsList[index].AccountId, fields);
                shapedAccounts[index].Add("Links", accountLinks);
            }

            var accountCollection = new LinkCollectionWrapper<Entity>(shapedAccounts);
            LinkCollectionWrapper<Entity> linkedAccounts = CreateLinksForAccount(httpContext, accountCollection);

            return new LinkResponse { HasLinks = true, LinkedEntites = linkedAccounts };
        }

        private LinkCollectionWrapper<Entity> CreateLinksForAccount(HttpContext httpContext, LinkCollectionWrapper<Entity> accountsWrapper)
        {
            accountsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext,
                "GetAccounts", values: new { }),
                "self",
                "GET"));

            return accountsWrapper;
        }

        private List<Link> CreateLinksForAccount(HttpContext httpContext, Guid accountId, string fields)
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetAccount",
                values: new { accountId }),
                "self",
                "GET"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteAccount",
                values: new { accountId }),
                "delete_account",
                "DELETE"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateAccount",
                values: new { accountId }),
                "update_account",
                "PUT"),
                new Link(_linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateAccount",
                values: new { accountId }),
                "partially_update_account",
                "PATCH"),
            };

            return links;
        }

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", 
                StringComparison.InvariantCultureIgnoreCase);
        }

        private List<Entity> ShapeData(IEnumerable<AccountDTO> accountDTOs, string fields)
        {
            return _dataShaper.ShapeData(accountDTOs, fields)
                .Select(a => a.Entity)
                .ToList();
        }
    }
}
